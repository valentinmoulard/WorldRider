using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBumperUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_text = null;

    [SerializeField]
    private float m_initialSize = 100.0f;

    [SerializeField]
    private float m_sizeVariation = 50.0f;

    [SerializeField]
    private float m_sizeVariationSpeed = 10.0f;

    private bool m_isSizeIncreasing;

    private void Start()
    {
        if (m_text == null)
            Debug.LogError("No text referenced in the inspector!", gameObject);

        m_isSizeIncreasing = true;
        m_text.fontSize = m_initialSize;
    }


    void Update()
    {
        m_text.fontSize += m_isSizeIncreasing ? m_sizeVariationSpeed * Time.deltaTime : -m_sizeVariationSpeed * Time.deltaTime;

        if (m_text.fontSize > m_initialSize + m_sizeVariation)
            m_isSizeIncreasing = false;
        if (m_text.fontSize < m_initialSize)
            m_isSizeIncreasing = true;

    }
}
