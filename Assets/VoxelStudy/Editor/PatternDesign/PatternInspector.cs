using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace VoxelStudy
{
    [CustomEditor(typeof(Pattern))]
    public class PatternInspector : Editor
    {

        Pattern pattern;
        bool inEditMode = false;
        int selectedLevelTile = 0;
        private string[] levelTileNames;

        public void OnEnable()
        {
            pattern = (Pattern)target;

            string levelTilesFolder = Application.dataPath + "/" + PatternSettings.levelTilePath;
            levelTileNames = System.IO.Directory.GetFiles(levelTilesFolder, "*.prefab");

            for (int i = 0; i < levelTileNames.Length; i++)
                levelTileNames[i] = System.IO.Path.GetFileNameWithoutExtension(levelTileNames[i]);

            if (PrefabUtility.GetPrefabType(pattern.gameObject) != PrefabType.Prefab)
                InitializePattern();

            Tools.current = Tool.View;
            Tools.viewTool = ViewTool.FPS;
        }

        private void InitializePattern()
        {
            Tile[] children = pattern.GetComponentsInChildren<Tile>();
            for (int i = 0; i < children.Length; i++)
                children[i].Initialize();
        }

        private void InitializePatternFlipX()
        {
            Tile[] children = pattern.GetComponentsInChildren<Tile>();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].Initialize();
                float posX = (PatternSettings.gridX - 1) * PatternSettings.tiledSize;
                children[i].transform.localPosition = new Vector3(posX - children[i].transform.localPosition.x, 
                                                                  children[i].transform.localPosition.y, 
                                                                  children[i].transform.localPosition.z);
                Vector3 newTilePosition = GetTileCoordinate(children[i].transform.position);
                children[i].name = string.Format("Tile_{0}_{1}_{2}", newTilePosition.x, newTilePosition.y, newTilePosition.z);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            bool showForm = false;
            switch (PrefabUtility.GetPrefabType(pattern.gameObject))
            {
                case PrefabType.None:
                case PrefabType.DisconnectedPrefabInstance:
                    if (GUILayout.Button("Save Pattern"))
                        SaveDialog();
                    showForm = true;
                    break;

                case PrefabType.Prefab:
                    if (GUILayout.Button("Edit Pattern"))
                        EditPattern();
                    break;

                case PrefabType.PrefabInstance:
                    if (!(pattern.transform.parent && (pattern.transform.parent.name == "All Patterns")))
                    {
                        if (GUILayout.Button("Update Pattern")) UpdatePrefab();
                        showForm = true;
                    }
                    break;

                default:
                    break;
            }

            GUILayout.Space(5);
            pattern.type = (Pattern.Type)EditorGUILayout.EnumPopup("Pattern Type", pattern.type);

            if (showForm)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Pattern Size");
                int tempGridZ = (int)EditorGUILayout.Slider(pattern.gridZ, 1.0f, 20.0f);
                if (tempGridZ != pattern.gridZ)
                {
                    if (TilesOffGrid(tempGridZ))
                        Debug.Log("Can't change pattern size, please remove tiles first!");
                    else
                        pattern.gridZ = tempGridZ;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                GUILayout.Label("Grid Controls");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Clear"))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?",
                        "Do you want to clear this pattern of all the level tiles?",
                        "Yes", "No"))
                    {
                        ClearGrid();
                    }
                }

                if (GUILayout.Button("Flip"))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?",
                        "Do you want to flip the pattern along it's X-axis?",
                        "Yes", "No"))
                    {
                        InitializePatternFlipX();
                    }
                }

                if (GUILayout.Button("Refresh"))
                {
                    InitializePattern();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Layer: " + pattern.gridY);
                if (GUILayout.Button("Move Up"))
                    MoveGridUp();

                if (GUILayout.Button("Move Down"))
                    MoveGridDown();
                GUILayout.EndHorizontal();

                if ((levelTileNames != null) && (levelTileNames.Length > 0))
                {
                    if (inEditMode)
                    {
                        GUI.color = Color.green;
                        if (GUILayout.Button("Disable Editing"))
                        {
                            inEditMode = false;
                        }
                    }
                    else
                    {
                        GUI.color = Color.white;
                        if (GUILayout.Button("Enable Editing"))
                        {
                            inEditMode = true;
                        }
                    }

                    GUILayout.Space(5);
                    GUI.color = Color.white;
                    GUILayout.Label("Choose Level Tile To Place");
                    selectedLevelTile = GUILayout.SelectionGrid(selectedLevelTile, levelTileNames, 3);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            SceneView.RepaintAll();
            if (inEditMode)
            {
                UpdateMouseHitPosition();
                UpdateMarkerPosition();
                Event current = Event.current;

                if (IsPositionOnGrid(mouseHitPosition, pattern.gridZ))
                {
                    if (current.type == EventType.mouseDown || current.type == EventType.MouseDrag)
                    {
                        if (current.button == 0)
                        {
                            Draw();
                            current.Use();
                        }
                        else if (current.button == 1)
                        {
                            Erase();
                            current.Use();
                        }
                    }
                }
                Selection.activeGameObject = pattern.gameObject;

                Handles.BeginGUI();
                GUI.Label(new Rect(10, Screen.height - 80, 100, 100), "LMB: Draw");
                GUI.Label(new Rect(10, Screen.height - 65, 100, 100), "RMB: Erase");
                Handles.EndGUI();
            }
        }

        #region Preview Window
        private PreviewRenderUtility _previewRenderUtility;
        private void ValidateData()
        {
            if (_previewRenderUtility == null)
            {
                _previewRenderUtility = new PreviewRenderUtility();

                _previewRenderUtility.m_Camera.transform.position = Vector3.zero;
                _previewRenderUtility.m_Camera.transform.rotation = Quaternion.Euler(45, -20, 0);
                _previewRenderUtility.m_Camera.orthographic = true;
                _previewRenderUtility.m_Camera.orthographicSize = 16;
                _previewRenderUtility.m_Camera.nearClipPlane = -100.0f;
                _previewRenderUtility.m_Camera.farClipPlane = 100.0f;
            }
        }

        public override bool HasPreviewGUI()
        {
            ValidateData();
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                _previewRenderUtility.BeginPreview(r, background);

                float maxHeight = 0.0f;
                Tile[] levelTiles = (target as Pattern).gameObject.GetComponentsInChildren<Tile>();
                for (int i = 0; i < levelTiles.Length; i++)
                {
                    if (levelTiles[i].transform.position.y > maxHeight)
                        maxHeight = levelTiles[i].transform.position.y;

                    Transform prefab = levelTiles[i].tilePrefab;
                    MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>();
                    for (int j = 0; j < meshFilters.Length; j++)
                    {
                        Vector3 scale = Vector3.Scale(meshFilters[j].transform.lossyScale, levelTiles[i].transform.localScale);
                        Vector3 pos = levelTiles[i].transform.position + (meshFilters[j].transform.position - meshFilters[j].transform.root.position);
                        Quaternion rot = Quaternion.Euler(levelTiles[i].transform.rotation.eulerAngles + meshFilters[j].transform.rotation.eulerAngles);
                        Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, scale);

                        Material mat = meshFilters[j].transform.GetComponent<MeshRenderer>().sharedMaterial;

                        int subMeshCount = meshFilters[j].sharedMesh.subMeshCount;

                        _previewRenderUtility.DrawMesh(meshFilters[j].sharedMesh, matrix, mat, 0);
                    }
                }

                _previewRenderUtility.m_Camera.transform.position = new Vector3(PatternSettings.gridX * PatternSettings.tiledSize / 2.0f,
                    maxHeight / 2.0f, pattern.gridZ * PatternSettings.tiledSize / 2.0f);
                _previewRenderUtility.m_Camera.Render();
                Texture resultRender = _previewRenderUtility.EndPreview();
                GUI.DrawTexture(r, resultRender, ScaleMode.StretchToFill, false);
            }
        }

        void OnDestroy()
        {
            _previewRenderUtility.Cleanup();
        }
        #endregion

        #region Save Pattern as Prefabs Functions
        private void SaveDialog()
        {
            string newName = "Pattern_" + (GetLastPrefabNumber() + 1);

            if (EditorUtility.DisplayDialog("Which one do I use?",
                "Current Name : " + pattern.name + "\nNew Name : " + newName,
                "Use New Name", "Use Existing Name"))
            {
                pattern.name = newName;
            }

            SavePrefab();
        }

        private int GetLastPrefabNumber()
        {
            int lastPrefabNumber = 0;
            string patternPrefabFolder = Application.dataPath + "/" + PatternSettings.patternPath;
            string[] patternPrefabNames = System.IO.Directory.GetFiles(patternPrefabFolder, "*.prefab");

            for (int i = 0; i < patternPrefabNames.Length; i++)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(patternPrefabNames[i]);
                int indexOfNumberStart = filename.IndexOf("_");
                string strPrefabNumber = filename.Substring(indexOfNumberStart + 1, (filename.Length - 1) - indexOfNumberStart);
                int prefabNumber = 0;
                int.TryParse(strPrefabNumber, out prefabNumber);
                if (prefabNumber > lastPrefabNumber)
                {
                    lastPrefabNumber = prefabNumber;
                }
            }
            return lastPrefabNumber;
        }

        private void SavePrefab()
        {
            string currentName = pattern.name;
            string path = "Assets/" + PatternSettings.patternPath + currentName + ".prefab";

            if (AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)))
            {
                if (EditorUtility.DisplayDialog("Are you sure?",
                    "The prefab already exists. Do you want to overwrite it?",
                    "Yes", "No"))
                {
                    pattern.name = currentName;
                    SaveUpdatePrefab(path);
                }
            }
            else
            {
                pattern.name = currentName;
                SaveUpdatePrefab(path);
            }
        }

        private void SaveUpdatePrefab(string prefabPath)
        {
            AssetDatabase.Refresh();
            PrefabUtility.CreatePrefab(prefabPath, pattern.gameObject, ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.Refresh();
        }

        private void UpdatePrefab()
        {
            string currentName = pattern.name;
            string path = "Assets/" + PatternSettings.patternPath + currentName + ".prefab";
            SaveUpdatePrefab(path);
        }

        private void EditPattern()
        {
            GameObject allPatternContainer = GameObject.Find("All Patterns");
            if (allPatternContainer)
            {
                DestroyImmediate(allPatternContainer);
            }

            GameObject existingPattern = GameObject.FindGameObjectWithTag("Pattern");
            if (existingPattern)
            {
                if (EditorUtility.DisplayDialog("Are you sure?",
                    "Do you want to delete the current pattern on screen?",
                    "Yes", "No"))
                {
                    DestroyImmediate(existingPattern);
                }
            }
            GameObject selectedPattern = PrefabUtility.InstantiatePrefab(pattern.gameObject) as GameObject;
            Selection.activeGameObject = selectedPattern;
        }
        #endregion

        #region Pattern Edit Functions
        private Vector3 mouseHitPosition;
        private Vector3 markerCoordinate;

        private void UpdateMouseHitPosition()
        {
            Vector3 gridLayerPlane = PatternSettings.tiledSize * pattern.gridY * Vector3.up;
            Plane gridPlane = new Plane(Vector3.up, (pattern.transform.position + gridLayerPlane));
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 hit = Vector3.zero;
            float dist;
            if (gridPlane.Raycast(ray, out dist))
                hit = ray.GetPoint(dist);
            mouseHitPosition = hit;
        }

        private void UpdateMarkerPosition()
        {
            markerCoordinate = GetTileCoordinate(mouseHitPosition);

            markerCoordinate.x = Mathf.Clamp(Mathf.Floor(markerCoordinate.x), 0.0f, (float)(PatternSettings.gridX - 1));
            markerCoordinate.y = pattern.gridY;
            markerCoordinate.z = Mathf.Clamp(Mathf.Floor(markerCoordinate.z), 0.0f, (float)(pattern.gridZ - 1));

            Handles.color = Color.green;
            Vector3 tileCenterOffset = Vector3.one * (PatternSettings.tiledSize / 2.0f);
            Handles.DrawWireCube(pattern.transform.position + (markerCoordinate * PatternSettings.tiledSize) + tileCenterOffset,
                new Vector3(PatternSettings.tiledSize, PatternSettings.tiledSize, PatternSettings.tiledSize));
            Handles.color = Color.white;
        }

        private Vector3 GetTileCoordinate(Vector3 fromPosition)
        {
            Vector3 pos = (fromPosition - pattern.transform.position) / PatternSettings.tiledSize;
            pos.x = Mathf.Floor(pos.x);
            pos.y = Mathf.Floor(pos.x);
            pos.z = Mathf.Floor(pos.z);
            return pos;
        }

        private bool IsPositionOnGrid(Vector3 testPosition, int testGridZ)
        {
            testPosition -= pattern.transform.position;
            float gridWidth = PatternSettings.tiledSize * PatternSettings.gridX;
            float gridHeight = PatternSettings.tiledSize * testGridZ;
            return !((testPosition.x < 0.0f) || (testPosition.x >= gridWidth) || (testPosition.z < 0.0f) || (testPosition.z >= gridHeight));
        }

        private void Draw()
        {
            GameObject tile = GameObject.Find(string.Format("Tile_{0}_{1}_{2}", markerCoordinate.x, markerCoordinate.y, markerCoordinate.z));
            if (tile != null && tile.transform.parent == pattern.transform)
            {
                // Tile already exist on that position on the grid
                if (tile.GetComponent<Tile>().tilePrefab.name == levelTileNames[selectedLevelTile])
                {
                    return;
                }
                else
                {
                    Erase();
                }
            }

            string prefabPath = "Assets/" + PatternSettings.levelTilePath + levelTileNames[selectedLevelTile] + ".prefab";
            GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
            tile = PrefabUtility.InstantiatePrefab(selectedPrefab) as GameObject;
            tile.GetComponent<Tile>().Initialize();
            Undo.RegisterCreatedObjectUndo(tile.gameObject, "Created Game Tile");
            tile.transform.parent = pattern.transform;
            tile.transform.localPosition = new Vector3(markerCoordinate.x * PatternSettings.tiledSize,
                                                       pattern.gridY * PatternSettings.tiledSize,
                                                       markerCoordinate.z * PatternSettings.tiledSize);
            tile.name = string.Format("Tile_{0}_{1}_{2}", markerCoordinate.x, markerCoordinate.y, markerCoordinate.z);
        }

        private void Erase()
        {
            GameObject tile = GameObject.Find(string.Format("Tile_{0}_{1}_{2}", markerCoordinate.x, markerCoordinate.y, markerCoordinate.z));
            if (tile != null && tile.transform.parent == pattern.transform)
            {
                Undo.DestroyObjectImmediate(tile.gameObject);
                DestroyImmediate(tile);
            }
        }

        private void ClearGrid()
        {
            int childCount = pattern.transform.childCount;
            while (childCount > 0)
            {
                childCount--;
                DestroyImmediate(pattern.transform.GetChild(childCount).gameObject);
            }
        }

        private bool TilesOffGrid(int testGridZ)
        {
            Tile[] children = pattern.GetComponentsInChildren<Tile>();
            for (int i = 0; i < children.Length; i++)
                if (!IsPositionOnGrid(children[i].transform.position, testGridZ))
                    return true;
            return false;
        }

        private void MoveGridUp()
        {
            pattern.gridY++;
        }

        private void MoveGridDown()
        {
            if (pattern.gridY > 0)
                pattern.gridY--;
        }
        #endregion
    }
}
