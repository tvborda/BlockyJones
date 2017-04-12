using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelStudy
{
    public class GameMode : MonoBehaviour
    {
        //The variable that keeps track of whether a start pattern needs to be spawned
        private bool spawnStartPattern = true;
        //used to keep track of whether the active character has been initialized
        private bool spawnCharacter = true;


        void Start()
        {
            PatternLoader.instance.LoadPatterns();
            LoadGame();
        }

        private void LoadGame()
        {
            //StartCoroutine(SpawnPattern());
        }

        void Update()
        {
    		
        }

//        private IEnumerator SpawnPattern()
//        {
//            while (spawnStartPattern || activeCharacter.position.z - (newPatternStartZ * PatternSettings.tiledSize) < patternSpawnDistance)
//            {
//                patternIndex++;
//                numberOfPatternsAdded++;
//                Transform selectedPatternPrefab = null;
//
//                if (!spawnStartPattern)
//                {
//                    if (patternIndex % 5 != 0)
//                    {
//                        if (numberOfPatternsAdded <= numberOfEasyPatterns)
//                        {
//                            selectedPatternPrefab = PatternLoader.instance.GetEasyPattern(onScreenPatternEntrances[onScreenPatternEntrances.Count - 1]);
//                        }
//                        else if (numberOfPatternsAdded <= numberOfEasyPatterns + numberOfMediumPatterns)
//                        {
//                            selectedPatternPrefab = PatternLoader.instance.GetMediumPattern(onScreenPatternEntrances[onScreenPatternEntrances.Count - 1]);
//                        }
//                        else
//                        {
//                            selectedPatternPrefab = PatternLoader.instance.GetHardPattern(onScreenPatternEntrances[onScreenPatternEntrances.Count - 1]);
//                        }
//                    }
//                    else
//                    {
//                        selectedPatternPrefab = PatternLoader.instance.GetConnectorPattern();
//                    }
//                }
//                else
//                {
//                    selectedPatternPrefab = PatternLoader.instance.GetStartPattern();
//                    spawnStartPattern = false;
//                }
//
//                Transform selectedPattern = (Transform)Instantiate(selectedPatternPrefab);
//                selectedPattern.name = selectedPatternPrefab.name; //to prevent the clone thing that just irritates me personally :)
//                Pattern patternScript = selectedPattern.GetComponent<Pattern>();
//
//                bool evenGridZ = patternScript.gridZ % 2 == 0 ? true : false;
//                float halfGridZ = patternScript.gridZ * 0.5f;
//                if (evenGridZ)
//                {
//                    newPatternStartZ += 0.5f;
//                }
//                newPatternStartZ -= halfGridZ;
//
//                selectedPattern.parent = patternParentContainer;
//                selectedPattern.localPosition = new Vector3(0, 0, newPatternStartZ * PatternSettings.tiledSize);
//
//                levelTiles = selectedPattern.GetComponentsInChildren<LevelTiles>();
//                for (int i = 0; i < levelTiles.Length; i++)
//                {
//                    levelTiles[i].Initialize();
//                }
//
//                newPatternStartZ -= halfGridZ;
//                if (evenGridZ)
//                {
//                    newPatternStartZ -= 0.5f;
//                }
//
//                onScreenPatterns.Add(selectedPattern);
//                onScreenPatternEdges.Add(-(halfGridZ + 1) * PatternSettings.tiledSize);
//                onScreenPatternEntrances.Add(patternScript.topEntrances);
//
//                if (spawnCharacter)
//                {
//                    activeCharacter = (Transform)Instantiate(characterPrefab, Vector3.zero, Quaternion.Euler(initialCharacterRotation));
//                    activeCharacter.parent = patternParentContainer;
//                    activeCharacter.GetComponent<TVNTCharacterController>().movementStyle = movementStyle;
//                    activeCharacter.GetComponent<TVNTPlayerController>().tapToMove = false;
//                    //Find the start tile and the place the character over it
//                    //there should be a start tile in the start pattern
//                    Vector3 startPosition = GameObject.Find("Start_Symbol").transform.position + new Vector3(0, PatternSettings.playerYOffset, 0);
//                    activeCharacter.position = startPosition;
//                    characterInitialPosition = startPosition.z;
//                    cameraTarget.position = activeCharacter.position;
//                    Camera.main.GetComponent<SmoothFollowCamera2D>().target = cameraTarget;
//                    spawnCharacter = false;
//                }
//                yield return null;
//            }
//        }
    }
}
