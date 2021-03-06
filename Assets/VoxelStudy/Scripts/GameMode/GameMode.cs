﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VoxelStudy
{
    public class GameMode : MonoBehaviour
    {
        public Transform patternParentContainer;
        public Transform characterPrefab;

        public RectTransform menuPanel;
        public RectTransform gameOverPanel;
        public RectTransform gamePlayPanel;

        public Transform initialFiller;
        private Transform filler;
        //Game difficulty
        private int numberOfPatternsAdded = 0;
        public int numberOfEasyPatterns = 20;
        public int numberOfMediumPatterns = 40;

        private bool spawnCharacter = true;
        private bool spawnStartPattern = true;
        private bool spawnConnectorPattern = false;
        private Transform activeCharacter = null;

        private float newPatternStartZ = 0.0f;
        private float patternSpawnDistance = 20.0f;
        private Tile[] levelTiles;
        private List<Transform> onScreenPatterns = new List<Transform>();

        private bool gameStarted = false;
        private bool gameOver = false;
        public float gameOverMenuDelay = 1.0f;

        //Score
        private Text gamePlayScoreText = null;
        private Text gameOverScoreText = null;
        private Text gameOverBestScoreText = null;
        private float activeCharacterInitialZ = 0.0f;
        private int score = 0;
        private int bestScore = 0;

        void Awake()
        {
            InitializeScoreTexts();
        }

        void Start()
        {
            PatternLoader.instance.LoadPatterns();

            // Instantiate Filler
            filler = (Transform)Instantiate(initialFiller);
            filler.transform.position = Vector3.zero;

            LoadGame();
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
                            levelTiles[j].Reset();
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
                    if (filler != null)
                        DestroyImmediate(filler.gameObject);
                    float amountToReset = -activeCharacter.position.z;
                    Camera.main.transform.position += new Vector3(0, 0, amountToReset);
                    for (int i = 0; i < onScreenPatterns.Count; i++)
                    {
                        onScreenPatterns[i].position += new Vector3(0, 0, amountToReset);
                    }
                    activeCharacter.position += new Vector3(0, 0, amountToReset);
                    newPatternStartZ += (amountToReset / PatternSettings.tiledSize);
                    activeCharacterInitialZ += Mathf.Abs(amountToReset);
                    return;
                }

                if ((newPatternStartZ - activeCharacter.position.z) < patternSpawnDistance)
                {
                    StartCoroutine(SpawnPattern());
                }
            }
        }

        void LateUpdate()
        {
            if (gameStarted && !gameOver)
            {
                if (!activeCharacter)
                {
                    //gameOver = true;
                    Invoke("GameOver", gameOverMenuDelay);
                    return;
                }

                //Vector2 playerCameraPosition = Camera.main.WorldToViewportPoint(activeCharacter.position + new Vector3(0, 0, -0.5f * PatternSettings.tiledSize));
                //if (playerCameraPosition.x > 1 || playerCameraPosition.y > 1)
                //{
                //    activeCharacter.GetComponent<TVNTPlayerController>().activate = false;
                //    GameOver();
                //}

                //cameraTarget.Translate(0, 0, -cameraTargetSpeed * Time.deltaTime);
                //if (activeCharacter.position.z < cameraTarget.position.z)
                //{
                //    cameraTarget.Translate(0, 0, -(cameraTarget.position.z - activeCharacter.position.z) * Time.deltaTime);
                //}

                int newScore = Mathf.FloorToInt((activeCharacterInitialZ + activeCharacter.position.z) / PatternSettings.tiledSize);
                if (newScore > score)
                {
                    score = newScore;
                    gamePlayScoreText.text = score.ToString();
                    if (score > bestScore)
                    {
                        bestScore = newScore;
                        PlayerPrefs.SetInt("BestScore", bestScore);
                    }
                }
            }
        }

        private void LoadGame()
        {
            bestScore = PlayerPrefs.GetInt("BestScore", 1);
            StartCoroutine(SpawnPattern());
        }

        public void StartGame()
        {
            menuPanel.gameObject.SetActive(false);
            gamePlayPanel.gameObject.SetActive(true);
            gameStarted = true;
            activeCharacter.GetComponent<BaseController>().activate = true;
        }

        private void GameOver()
        {
            gameOver = true;
            gameOverScoreText.text = score.ToString();
            gameOverBestScoreText.text = bestScore.ToString();
            gamePlayPanel.gameObject.SetActive(false);
            gameOverPanel.gameObject.SetActive(true);
        }

        public void RestartGame()
        {
            if (activeCharacter)
            {
                Destroy(activeCharacter.gameObject);
            }
            for (int i = onScreenPatterns.Count - 1; i > -1; i--)
            {
                Transform selectedPattern = onScreenPatterns[i];
                onScreenPatterns.RemoveAt(i);

                GroundCollider[] groundColliders = selectedPattern.GetComponentsInChildren<GroundCollider>();
                for (int j = 0; j < groundColliders.Length; j++)
                {
                    groundColliders[j].occupied = false;
                }
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
            gameOverPanel.gameObject.SetActive(false);
            SceneManager.LoadScene("BaseScene");
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
                    activeCharacterInitialZ = startPosition.z;
                    SmoothFollowCamera2D smoothFollowCamera2D = Camera.main.GetComponent<SmoothFollowCamera2D>();
                    smoothFollowCamera2D.target = activeCharacter;
                    Camera.main.transform.position = activeCharacter.position + smoothFollowCamera2D.targetOffset;
                    spawnCharacter = false;
                }
                yield return null;
            }
        }

        private void InitializeScoreTexts()
        {
            Text[] textComponents = gamePlayPanel.GetComponentsInChildren<Text>();
            foreach (Text t in textComponents)
            {
                if (t.gameObject.name == "Score")
                {
                    gamePlayScoreText = t;
                    break;
                }
            }

            textComponents = gameOverPanel.GetComponentsInChildren<Text>();
            foreach (Text t in textComponents)
            {
                if (t.gameObject.name == "Score")
                {
                    gameOverScoreText = t;
                }
                else if (t.gameObject.name == "Best")
                {
                    gameOverBestScoreText = t;
                }
            }
        }

    }
}
