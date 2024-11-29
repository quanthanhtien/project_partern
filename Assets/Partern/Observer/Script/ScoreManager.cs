using System;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    TMP_Text scoreText;
    
    int currentScore;

    private void Awake()
    {
        scoreText = GetComponent<TMP_Text>();
        Collectible.OnAnyCollected += AddScore;
        UpdateScoreText();
    }

    private void OnEnable() => Collectible.OnAnyCollected += AddScore;

    private void OnDisable() => Collectible.OnAnyCollected -= AddScore;

    void AddScore(int points)
    {
        currentScore += points;
        scoreText.text = currentScore.ToString();
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = $"Score: {currentScore}";
    }
}