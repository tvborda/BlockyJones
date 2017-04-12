using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelStudy;

public class PatternLoader : MonoBehaviour {

    public static PatternLoader instance = null;
    public bool isPersistant = true;

    [HideInInspector]
    public string patternLocation = "";

    [HideInInspector]
    private Transform[] patterns;
    [HideInInspector]
    private List<Transform> introductoryPatterns = new List<Transform> ();
    [HideInInspector]
    private List<Transform> connectorPatterns = new List<Transform>();
    [HideInInspector]
    private List<Transform> easyPatterns = new List<Transform>();
    [HideInInspector]
    private List<Transform> mediumPatterns = new List<Transform>();
    [HideInInspector]
    private List<Transform> hardPatterns = new List<Transform>();

    [HideInInspector]
    public bool patternsLoaded = false;
    private List<int> tempEntranceList = new List<int> ();

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }
        if (isPersistant) {
            DontDestroyOnLoad (gameObject);
        }
    }

    public void LoadPatterns() {
        if (patternsLoaded == false) {
            Debug.Log ("Loading Patterns...");
            patterns = Resources.LoadAll<Transform> (patternLocation);
            for (int i = 0; i < patterns.Length; i++) {
                Pattern indexedPatternScript = patterns [i].GetComponent<Pattern> ();
                switch (indexedPatternScript.type) {
                    case Pattern.Type.Introductory:
                        introductoryPatterns.Add (patterns [i]);
                        break;
                    case Pattern.Type.Connector:
                        connectorPatterns.Add (patterns [i]);
                        break;
                    case Pattern.Type.Easy:
                        easyPatterns.Add (patterns [i]);
                        break;
                    case Pattern.Type.Medium:
                        mediumPatterns.Add (patterns [i]);
                        break;
                    case Pattern.Type.Hard:
                        hardPatterns.Add (patterns [i]);
                        break;
                }
            }
            patternsLoaded = true;
        }
    }

    public Transform GetIntroductoryPattern() {
        Transform pattern = null;
        if (introductoryPatterns.Count > 0) {
            pattern = introductoryPatterns [Random.Range (0, introductoryPatterns.Count)];
        } else {
            Debug.LogWarning ("There are no Introductory patterns");
        }
        return pattern;
    }
        
    public Transform GetConnectorPattern() {
        Transform pattern = null;
        if (connectorPatterns.Count > 0) {
            pattern = connectorPatterns [Random.Range (0, connectorPatterns.Count)];
        }  else {
            Debug.LogWarning ("There are no Connector patterns");
        }
        return pattern;
    }

    public Transform GetEasyPattern() {
        Transform pattern = null;
        if (easyPatterns.Count > 0) {
            pattern = easyPatterns [Random.Range (0, easyPatterns.Count)];
        } else {
            Debug.LogWarning ("There are no Easy patterns");
        }
        return pattern;
    }

    public Transform GetMediumPattern(int entranceInt = -1) {
        Transform pattern = null;
        if (mediumPatterns.Count > 0) {
            pattern = mediumPatterns [Random.Range (0, mediumPatterns.Count)];
        } else {
            Debug.LogWarning ("There are no Medium patterns");
        }
        return pattern;
    }

    public Transform GetHardPattern(int entranceInt = -1) {
        Transform pattern = null;
        if (hardPatterns.Count > 0) {
            pattern = hardPatterns [Random.Range (0, hardPatterns.Count)];
        } else {
            Debug.LogWarning ("There are no Hard patterns");
        }
        return pattern;
    }
}
