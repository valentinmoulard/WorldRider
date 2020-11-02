﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinalScoreUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_text = null;

    private void OnEnable()
    {
        ScoreManager.OnBroadcastedFinalScore += UpdateUIScore;
    }

    private void OnDisable()
    {
        ScoreManager.OnBroadcastedFinalScore -= UpdateUIScore;
    }

    private void Start()
    {
        if (m_text == null)
            Debug.LogError("No text mesh pro component refered in the inspector!");
    }

    private void UpdateUIScore(float currentScore)
    {
        m_text.text = currentScore.ToString("F0");
    }
}
