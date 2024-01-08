using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TMP_Text leftScore;
    public TMP_Text rightScore;

    public GameObject EndPanel;
    public GameObject blueWins;
    public GameObject redWins;
    public GameObject tie;

    public AudioSource endSound;
    public AudioSource goalSound;

    private int leftNum = 0;
    private int rightNum = 0;
    public int ballCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        ballCount = GameObject.Find("Balls").transform.childCount;
        // EndPanel = GameObject.Find("EndPanel");
        // blueWins = GameObject.Find("BlueWins");
        // redWins = GameObject.Find("RedWins");
        // tie = GameObject.Find("Tie");
    }

    private void Update()
    {
        Restart();
    }

    private void Restart()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
            InitializeGame();
        }
    }

    private void CheckEnding()
    {
        if (Mathf.Abs(leftNum - rightNum) > ballCount - (leftNum + rightNum))
        {
            EndPanel.SetActive(true);
            if (leftNum > rightNum) GameEnds(PlayerController.Player.Left);
            else if (leftNum < rightNum) GameEnds(PlayerController.Player.Right);
            else if (leftNum == rightNum) GameEnds(PlayerController.Player.None);
        }
    }

    private void GameEnds(PlayerController.Player winner)
    {
        endSound.Play();
        if (winner == PlayerController.Player.Left)
        {
            blueWins.SetActive(true);
            redWins.SetActive(false);
            tie.SetActive(false);
        }
        else if (winner == PlayerController.Player.Right)
        {
            redWins.SetActive(true);
            blueWins.SetActive(false);
            tie.SetActive(false);
        }
        else if (winner == PlayerController.Player.None)
        {
            tie.SetActive(true);
            redWins.SetActive(false);
            blueWins.SetActive(false);
        }
    }

    public void AddScore(PlayerController.Player scorer)
    {
        goalSound.Play();
        if (scorer == PlayerController.Player.Left)
        {
            int currentScore = int.Parse(leftScore.text);
            currentScore++;
            leftNum = currentScore;
            leftScore.text = currentScore.ToString();
        }
        else if (scorer == PlayerController.Player.Right)
        {
            int currentScore = int.Parse(rightScore.text);
            currentScore++;
            rightNum = currentScore;
            rightScore.text = currentScore.ToString();
        }

        CheckEnding();
    }
}
