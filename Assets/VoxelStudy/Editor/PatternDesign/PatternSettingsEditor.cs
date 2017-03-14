using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;

namespace VoxelStudy
{
    [InitializeOnLoad]
    public class Startup
    {
        static Startup()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PatternSettingsContainer));
            FileStream stream = new FileStream(Application.dataPath + "/VoxelStudy/Game/Resources/XML/patternSettings.xml", FileMode.Open);
            PatternSettingsEditor.patternSettingsContainer = serializer.Deserialize(stream) as PatternSettingsContainer;
            stream.Close();

            PatternSettings.patternPath = PatternSettingsEditor.patternSettingsContainer.patternPath;
            PatternSettings.levelTilePath = PatternSettingsEditor.patternSettingsContainer.levelTilePath;
            PatternSettings.tiledSize = PatternSettingsEditor.patternSettingsContainer.tiledSize;
            PatternSettings.gridX = PatternSettingsEditor.patternSettingsContainer.gridX;
            PatternSettings.playerYOffset = PatternSettingsEditor.patternSettingsContainer.playerYOffset;
        }
    }

    public class PatternSettingsEditor : EditorWindow
    {

        public static PatternSettingsContainer patternSettingsContainer;

        public static string tempPatternPath;
        public static string tempLevelTilePath;
        public static float tempTiledSize;
        public static int tempGridX;
        public static float tempPlayerYOffset;

        private Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Voxel Study/Pattern Settings")]
        static void Init()
        {
            tempPatternPath = PatternSettings.patternPath;
            tempLevelTilePath = PatternSettings.levelTilePath;
            tempTiledSize = PatternSettings.tiledSize;
            tempGridX = PatternSettings.gridX;
            tempPlayerYOffset = PatternSettings.playerYOffset;

            PatternSettingsEditor window = (PatternSettingsEditor)EditorWindow.GetWindow(typeof(PatternSettingsEditor));
            window.titleContent = new GUIContent("Pattern Settings");
            window.Show();
        }

        void OnGUI()
        {
            bool toSave = false;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Pattern Directory
            GUILayout.Label("Pattern Directory:", EditorStyles.boldLabel);
            GUILayout.Label(PatternSettings.patternPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (GUILayout.Button("Choose Pattern Directory"))
            {
                tempPatternPath = EditorUtility.OpenFolderPanel("Choose Pattern Directory", "", "");
                if (tempPatternPath.StartsWith(Application.dataPath))
                {
                    tempPatternPath = tempPatternPath.Substring(Application.dataPath.Length + 1, tempPatternPath.Length - (Application.dataPath.Length + 1)) + "/";
                    if (PatternSettings.patternPath != tempPatternPath)
                    {
                        PatternSettings.patternPath = tempPatternPath;
                        toSave = true;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Wrong Location!", "The pattern path must be within the scope of the asset directory!", "Ok");
                }
            }

            EditorGUILayout.Space();

            // Tile Directory
            GUILayout.Label("Tile Directory:", EditorStyles.boldLabel);
            GUILayout.Label(PatternSettings.levelTilePath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (GUILayout.Button("Choose Tile Directory"))
            {
                tempLevelTilePath = EditorUtility.OpenFolderPanel("Choose Tile Directory", "", "");
                if (tempLevelTilePath.StartsWith(Application.dataPath))
                {
                    tempLevelTilePath = tempLevelTilePath.Substring(Application.dataPath.Length + 1, tempLevelTilePath.Length - (Application.dataPath.Length + 1)) + "/";
                    if (PatternSettings.levelTilePath != tempLevelTilePath)
                    {
                        PatternSettings.levelTilePath = tempLevelTilePath;
                        toSave = true;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Wrong Location!", "The level tile path must be within the scope of the asset directory!", "Ok");
                }
            }

            EditorGUILayout.Space();

            // Tile Size
            GUILayout.Label("Tile Size:", EditorStyles.boldLabel);
            tempTiledSize = EditorGUILayout.Slider(PatternSettings.tiledSize, 0.0f, 20.0f);
            if (Mathf.Approximately(tempTiledSize, PatternSettings.tiledSize) == false)
            {
                if (tempTiledSize > 0)
                {
                    PatternSettings.tiledSize = tempTiledSize;
                    toSave = true;
                }
            }

            EditorGUILayout.Space();

            // Grid X
            GUILayout.Label("Grid X:", EditorStyles.boldLabel);
            tempGridX = (int)EditorGUILayout.Slider(PatternSettings.gridX, 1.0f, 20.0f);
            if (tempGridX != PatternSettings.gridX)
            {
                if (tempGridX > 0)
                {
                    PatternSettings.gridX = tempGridX;
                    toSave = true;
                }
            }

            EditorGUILayout.Space();

            // Player Y Offset
            GUILayout.Label("Player Y Offset:", EditorStyles.boldLabel);
            tempPlayerYOffset = EditorGUILayout.FloatField("", PatternSettings.playerYOffset);
            if (Mathf.Approximately(tempPlayerYOffset, PatternSettings.playerYOffset) == false)
            {
                if (tempPlayerYOffset > 0)
                {
                    PatternSettings.playerYOffset = tempPlayerYOffset;
                    toSave = true;
                }
            }

            // If required to save then serialize data to XML
            if (toSave)
            {
                if (patternSettingsContainer == null)
                {
                    patternSettingsContainer = new PatternSettingsContainer();
                }
                patternSettingsContainer.patternPath = PatternSettings.patternPath;
                patternSettingsContainer.levelTilePath = PatternSettings.levelTilePath;
                patternSettingsContainer.tiledSize = PatternSettings.tiledSize;
                patternSettingsContainer.gridX = PatternSettings.gridX;
                patternSettingsContainer.playerYOffset = PatternSettings.playerYOffset;

                XmlSerializer serializer = new XmlSerializer(typeof(PatternSettingsContainer));
                FileStream stream = new FileStream(Application.dataPath + "/VoxelStudy/Game/Resources/XML/patternSettings.xml", FileMode.Create);
                serializer.Serialize(stream, patternSettingsContainer);
                stream.Close();
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();
        }
    }
}
