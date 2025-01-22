using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    TMP_Text scoreText;

    int currentScore;

    private void Awake()
    {
        scoreText = GetComponent<TMP_Text>();
        UpdateScoreText();
    }

    private void OnEnable()
    {
        Collectible.OnAnyCollected += AddScore;
        SubCollectible.OnSubCollected += SubScore;
    }

    private void OnDisable()
    {
        Collectible.OnAnyCollected -= AddScore;
        SubCollectible.OnSubCollected -= SubScore;
    }

    void AddScore(int points)
    {
        currentScore += points;
        scoreText.text = currentScore.ToString();
        UpdateScoreText();
    }

    void SubScore(int points)
    {
        currentScore -= points;
        scoreText.text = currentScore.ToString();
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = $"Score: {currentScore}";
    }
}
