using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VoxelStudy
{
    public class Pattern : MonoBehaviour
    {
        public enum Type
        {
            Introductory,
            Connector,
            Easy,
            Medium,
            Hard,
        };

        private bool drawArrow = true;
        public Type type = Type.Introductory;
        public int gridZ = 5;
        public int gridY = 0;
        [HideInInspector]
        public float zOffset = 0.0f;


#if UNITY_EDITOR
        [MenuItem("Voxel Study/New Pattern")]
        static void CreateNewPattern()
        {
            ClearAllPatterns();
            GameObject newPattern = new GameObject("Pattern");
            newPattern.AddComponent<Pattern>();
            newPattern.tag = "Pattern";
            Selection.activeGameObject = newPattern;
        }

        [MenuItem("Voxel Study/Show All Patterns")]
        static void ViewAllPattern()
        {
            ClearAllPatterns();
            string patternPrefabFolder = Application.dataPath + "/" + PatternSettings.patternPath;
            string[] patternPrefabNames = System.IO.Directory.GetFiles(patternPrefabFolder, "*.prefab");

            Transform allPatternContainer = new GameObject("All Patterns").transform;
            allPatternContainer.tag = "Pattern";

            int numberOfPatternsInColumn = 10;
            int patternSpacer = 1;
            float patternX = 0.0f;
            float patternZ = 0.0f;

            for (int i = 0; i < patternPrefabNames.Length; i++)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(patternPrefabNames[i]);
                GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath("Assets/" + PatternSettings.patternPath + filename + ".prefab", typeof(GameObject)) as GameObject;
                GameObject selectedPattern = PrefabUtility.InstantiatePrefab(selectedPrefab) as GameObject;

                if ((i % numberOfPatternsInColumn) == 0)
                {
                    patternZ = 0;
                    patternX = (i / numberOfPatternsInColumn) * (PatternSettings.gridX + patternSpacer) * PatternSettings.tiledSize;
                }

                selectedPattern.transform.parent = allPatternContainer;
                selectedPattern.transform.localPosition = new Vector3(patternX, 0.0f, patternZ);
                Selection.activeTransform = selectedPattern.transform;
                patternZ += (selectedPattern.GetComponent<Pattern>().gridZ + patternSpacer) * PatternSettings.tiledSize;
                selectedPattern.GetComponent<Pattern>().drawArrow = false;
            }
        }

        [MenuItem("Voxel Study/Hide All Patterns")]
        static void ClearAllPatterns()
        {
            GameObject[] allPatterns = GameObject.FindGameObjectsWithTag("Pattern");
            foreach (GameObject pattern in allPatterns)
                DestroyImmediate(pattern);
        }

        private void OnDrawGizmosSelected()
        {
            float gridWidth = PatternSettings.tiledSize * PatternSettings.gridX;
            float gridHeight = PatternSettings.tiledSize * gridZ;
            float gridLayer = PatternSettings.tiledSize * gridY;

            Vector3 bottomLeft = new Vector3(transform.position.x, transform.position.y + gridLayer, transform.position.z);
            Vector3 bottomRight = new Vector3(transform.position.x + gridWidth, transform.position.y + gridLayer, transform.position.z);
            Vector3 topLeft = new Vector3(transform.position.x, transform.position.y + gridLayer, transform.position.z + gridHeight);
            Vector3 topRight = new Vector3(transform.position.x + gridWidth, transform.position.y + gridLayer, transform.position.z + gridHeight);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topLeft, bottomLeft);
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);

            if (drawArrow)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(topRight + new Vector3(2, 0, 0), bottomRight + new Vector3(2, 0, 0));
                Gizmos.DrawLine(topRight + new Vector3(2, 0, 0), topRight + new Vector3(1, 0, -1));
                Gizmos.DrawLine(topRight + new Vector3(2, 0, 0), topRight + new Vector3(3, 0, -1));
                Handles.Label(bottomRight + new Vector3(1, 0, 0), "DIRECTION");
                Handles.Label(bottomLeft + new Vector3(gridWidth / 2.0f, 0, -2), "Layer: " + gridY);
            }

            Gizmos.color = Color.grey;
            for (int i = 1; i < PatternSettings.gridX; i++)
                Gizmos.DrawLine(bottomLeft + new Vector3(PatternSettings.tiledSize * i, 0, 0), topLeft + new Vector3(PatternSettings.tiledSize * i, 0, 0));

            for (int i = 1; i < gridZ; i++)
                Gizmos.DrawLine(bottomLeft + new Vector3(0, 0, i * PatternSettings.tiledSize), bottomRight + new Vector3(0, 0, i * PatternSettings.tiledSize));
        }
#endif
    }
}
