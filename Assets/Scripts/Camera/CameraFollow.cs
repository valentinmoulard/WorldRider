using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script follows the object given in the inspector from an offset position
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField, Tooltip("Offset position from where the camera will see the object to follow")]
    private Vector3 m_offsetVector = Vector3.zero;

    private GameObject m_objectToFollow = null;

    void Start()
    {
        m_objectToFollow = GameManager.instance.m_player;

        if (m_objectToFollow != null)
        {
            transform.position = m_objectToFollow.transform.position + m_offsetVector;
            transform.LookAt(m_objectToFollow.transform);
        }
        else
        {
            Debug.LogError("No object to follow referenced in the inspector!");
        }
    }

    void Update()
    {
        if (m_objectToFollow != null)
            transform.position = m_objectToFollow.transform.position + m_offsetVector;
    }
}
