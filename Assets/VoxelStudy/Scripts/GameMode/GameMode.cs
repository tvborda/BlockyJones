using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelStudy
{
    public class GameMode : MonoBehaviour
    {
        public Transform patternParentContainer;
        public Transform characterPrefab;

        //Game difficulty
        private int numberOfPatternsAdded = 0;
        public int numberOfEasyPatterns = 20;
        public int numberOfMediumPatterns = 40;

        private bool spawnCharacter = true;
        private bool spawnStartPattern = true;
        private bool spawnConnectorPattern = false;
        private Transform activeCharacter = null;

        private float newPatternStartZ = 0;
        private float patternSpawnDistance = 20.0f;
        private Tile[] levelTiles;
        private List<Transform> onScreenPatterns = new List<Transform>();

        private bool gameStarted = false;
        private bool gameOver = false;

        void Start()
        {
            PatternLoader.instance.LoadPatterns();
            LoadGame();
        }

        public void StartGame()
        {
            activeCharacter.GetComponent<BaseController>().activate = true;
            gameStarted = true;
        }

        void FixedUpdate()
        {
            if (gameStarted && !gameOver && activeCharacter)
            {
                for (int i = 0; i < onScreenPatterns.Count; i++)
                {
                    Transform selectedPattern = onScreenPatterns[i];
                    Pattern patternScript = selectedPattern.GetComponent<Pattern>();
                    float patternZ = selectedPattern.position.z + (patternScript.gridZ * PatternSettings.tiledSize);

                    if (Camera.main.transform.position.z > (patternZ + 10.0f))
                    {
                        onScreenPatterns.RemoveAt(i);
                        levelTiles = selectedPattern.GetComponentsInChildren<Tile>();
                        for (int j = 0; j < levelTiles.Length; j++)
                        {
                            if (levelTiles[j].instancePrefab)
                            {
                                TileObjectPool.instance.ReleaseObject(levelTiles[j].instancePrefab);
                            }
                            levelTiles[j].instancePrefab = null;
                        }
                        Destroy(selectedPattern.gameObject);
                    }
                }

                if ((activeCharacter.position.z > 20) && activeCharacter.GetComponent<BaseController>().idle)
                {
                    float amountToReset = -activeCharacter.position.z;
                    Camera.main.transform.position += new Vector3(0, 0, amountToReset);
                    for (int i = 0; i < onScreenPatterns.Count; i++)
                    {
                        onScreenPatterns[i].position += new Vector3(0, 0, amountToReset);
                    }
                    activeCharacter.position += new Vector3(0, 0, amountToReset);
                    newPatternStartZ += (amountToReset / PatternSettings.tiledSize);
                    return;
                }

                if ((newPatternStartZ - activeCharacter.position.z) < patternSpawnDistance)
                {
                    StartCoroutine(SpawnPattern());
                }
            }
        }

        private void LoadGame()
        {
            StartCoroutine(SpawnPattern());
        }

        private IEnumerator SpawnPattern()
        {
            while (spawnStartPattern || (newPatternStartZ * PatternSettings.tiledSize) - activeCharacter.position.z < patternSpawnDistance)
            {
                numberOfPatternsAdded++;
                Transform selectedPatternPrefab = null;

                if (!spawnStartPattern)
                {
                    if (spawnConnectorPattern)
                    {
                        selectedPatternPrefab = PatternLoader.instance.GetConnectorPattern();
                    }
                    else
                    { 
                        if (numberOfPatternsAdded <= numberOfEasyPatterns)
                        {
                            selectedPatternPrefab = PatternLoader.instance.GetEasyPattern();
                        }
                        else if (numberOfPatternsAdded <= numberOfEasyPatterns + numberOfMediumPatterns)
                        {
                            selectedPatternPrefab = PatternLoader.instance.GetMediumPattern();
                        }
                        else
                        {
                            selectedPatternPrefab = PatternLoader.instance.GetHardPattern();
                        }
                    }
                    spawnConnectorPattern = !spawnConnectorPattern;
                }
                else
                {
                    selectedPatternPrefab = PatternLoader.instance.GetIntroductoryPattern();
                    spawnStartPattern = false;
                }

                Transform selectedPattern = (Transform)Instantiate(selectedPatternPrefab);
                selectedPattern.name = selectedPatternPrefab.name;
                Pattern patternScript = selectedPattern.GetComponent<Pattern>();

                selectedPattern.parent = patternParentContainer;
                selectedPattern.localPosition = new Vector3(0, 0, newPatternStartZ * PatternSettings.tiledSize);
                newPatternStartZ += patternScript.gridZ;

                levelTiles = selectedPattern.GetComponentsInChildren<Tile>();
                for (int i = 0; i < levelTiles.Length; i++)
                {
                    levelTiles[i].Initialize();
                }

                onScreenPatterns.Add(selectedPattern);

                if (spawnCharacter)
                {
                    activeCharacter = (Transform)Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
                    activeCharacter.parent = patternParentContainer;
                    float xCenterTile = Mathf.Floor(PatternSettings.gridX / 2.0f) * PatternSettings.tiledSize;
                    float tileOffset = PatternSettings.tiledSize / 2.0f;

                    Vector3 startPosition = new Vector3(xCenterTile + tileOffset, PatternSettings.tiledSize, tileOffset);
                    activeCharacter.position = startPosition;
                    Camera.main.GetComponent<SmoothFollowCamera2D>().target = activeCharacter;
                    spawnCharacter = false;
                }
                yield return null;
            }
        }
    }
}
