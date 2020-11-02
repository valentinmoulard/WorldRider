using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    public enum RotationAxis
    {
        RIGHT,
        UP,
        FORWARD
    }

    [SerializeField, Tooltip("The rotation axis the object will rotate around.")]
    private RotationAxis m_rotationAxis = RotationAxis.UP;

    [SerializeField, Range(0.0f, 80.0f), Tooltip("The higher the value, the fastest the object will rotate.")]
    private float m_rotationSpeed = 5.0f;

    [SerializeField, Tooltip("If checked, the rotation will be counter clockwise.")]
    private bool m_CCWRotation = false;


    private int m_rotationSign;
    private float m_angleIncrementation;
    private Vector3 m_rotationAxisBuffer;


    private void Start()
    {
        m_rotationSign = 1;
    }


    void Update()
    {
        m_rotationSign = m_CCWRotation ? 1 : -1;

        switch(m_rotationAxis)
        {
            case RotationAxis.RIGHT:
                m_rotationAxisBuffer = Vector3.right;
                break;
            case RotationAxis.UP:
                m_rotationAxisBuffer = Vector3.up;
                break;
            case RotationAxis.FORWARD:
                m_rotationAxisBuffer = Vector3.forward;
                break;

        }

        m_angleIncrementation = m_rotationSign * m_rotationSpeed;
        transform.Rotate(m_rotationAxisBuffer, m_angleIncrementation);
    }
}
