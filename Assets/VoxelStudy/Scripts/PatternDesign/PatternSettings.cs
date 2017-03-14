using UnityEngine;
using System.Xml.Serialization;
using System.IO;

namespace VoxelStudy
{
	public class PatternSettings
	{
	    public static string patternPath;
	    public static string levelTilePath;
	    public static float tiledSize;
	    public static int gridX;
	    public static float playerYOffset;
	
	    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	    private static void PatternSettingsLoad()
	    {
	        TextAsset _xml = Resources.Load<TextAsset>("XML/patternSettings");
	        XmlSerializer serializer = new XmlSerializer(typeof(PatternSettingsContainer));
	        StringReader reader = new StringReader(_xml.ToString());
	        PatternSettingsContainer patternSettingsContainer = serializer.Deserialize(reader) as PatternSettingsContainer;
	        reader.Close();
	
	        PatternSettings.patternPath = patternSettingsContainer.patternPath;
	        PatternSettings.levelTilePath = patternSettingsContainer.levelTilePath;
	        PatternSettings.tiledSize = patternSettingsContainer.tiledSize;
	        PatternSettings.gridX = patternSettingsContainer.gridX;
	        PatternSettings.playerYOffset = patternSettingsContainer.playerYOffset;
	    }
	}
}