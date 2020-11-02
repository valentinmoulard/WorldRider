using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Events that send the scores (mainly to update in game and game over UIs)
    public static Action<float> OnBroadcastedUpdatedScore;
    public static Action<float> OnBroadcastedFinalScore;

    private float m_currentScore;

    private void OnEnable()
    {
        GameManager.OnGameStart += ResetScore;
        GameManager.OnGameOver += BroadcastFinalScore;

        Enemy.OnBroadcastedScoreValue += IncreaseScore;
        Coins.OnCoinCollected += IncreaseScore;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= ResetScore;
        GameManager.OnGameOver -= BroadcastFinalScore;

        Enemy.OnBroadcastedScoreValue -= IncreaseScore;
        Coins.OnCoinCollected -= IncreaseScore;
    }

    private void ResetScore()
    {
        m_currentScore = 0.0f;
        BroadcastUpdatedScore();
    }

    private void IncreaseScore(float amountToIncrease)
    {
        m_currentScore += amountToIncrease;
        BroadcastUpdatedScore();
    }

    #region BROADCASTERS
    private void BroadcastUpdatedScore()
    {
        if (OnBroadcastedUpdatedScore != null)
            OnBroadcastedUpdatedScore(m_currentScore);
    }

    private void BroadcastFinalScore()
    {
        if (OnBroadcastedFinalScore != null)
            OnBroadcastedFinalScore(m_currentScore);
    }
    #endregion
}
