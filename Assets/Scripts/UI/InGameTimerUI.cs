using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameTimerUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_text = null;

    private void Start()
    {
        if (m_text == null)
            Debug.LogError("No text mesh pro component refered in the inspector!");
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        m_text.text = GameManager.instance.m_gameTime.ToString("F1");
    }
}
