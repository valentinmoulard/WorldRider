using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0.0f, 30.0f), Tooltip("How fast can the player move.")]
    private float m_originalMovementSpeed = 2.0f;

    [SerializeField, Range(100.0f, 200.0f), Tooltip("How fast can the player rotate.")]
    private float m_rotationSpeed = 100.0f;

    private Coroutine m_slowSpeedCoroutineReference;
    private float m_movementSpeed;
    private bool m_isControlEnabled;
    private bool m_canMove;


    private void OnEnable()
    {
        GameManager.OnTitleScreen += InitializePlayerPosition;

        GameManager.OnTitleScreen += EnableMovement;
        GameManager.OnGameOver += DisableMovement;

        GameManager.OnGameStart += EnableControls;
        GameManager.OnGameOver += DisableControls;

        Controller.OnHold += Rotate;

        Traps.OnSlowTrapActivated += DecreaseSpeedFromTrap;
    }

    private void OnDisable()
    {
        GameManager.OnTitleScreen -= InitializePlayerPosition;

        GameManager.OnTitleScreen += EnableMovement;
        GameManager.OnGameOver += DisableMovement;

        GameManager.OnGameStart -= EnableControls;
        GameManager.OnGameOver -= DisableControls;

        Controller.OnHold -= Rotate;

        Traps.OnSlowTrapActivated -= DecreaseSpeedFromTrap;
    }

    private void Start()
    {
        m_movementSpeed = m_originalMovementSpeed;
    }

    private void Update()
    {
        if (m_canMove)
            transform.position += transform.forward * m_movementSpeed * Time.deltaTime;
    }

    private void InitializePlayerPosition()
    {
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
    }

    private void Rotate(Vector3 cursorPosition)
    {
        if (m_isControlEnabled)
        {
            if (cursorPosition.x < Screen.width / 2)
                transform.Rotate(Vector3.up * -m_rotationSpeed * Time.deltaTime);
            else
                transform.Rotate(Vector3.up * m_rotationSpeed * Time.deltaTime);
        }
    }

    private void DecreaseSpeedFromTrap(GameObject objectToAffect, float slowAmount, float slowDuration)
    {
        if (objectToAffect == gameObject)
            m_slowSpeedCoroutineReference = StartCoroutine(DecraseSpeedCoroutine(slowAmount, slowDuration));
    }

    private IEnumerator DecraseSpeedCoroutine(float slowAmount, float slowDuration)
    {
        m_movementSpeed -= slowAmount;
        Mathf.Clamp(m_movementSpeed, 0.0f, 100.0f);
        yield return new WaitForSeconds(slowDuration);
        m_movementSpeed = m_originalMovementSpeed;
    }

    private void EnableControls()
    {
        m_isControlEnabled = true;
    }

    private void DisableControls()
    {
        m_isControlEnabled = false;
    }

    private void EnableMovement()
    {
        m_canMove = true;
    }

    private void DisableMovement()
    {
        m_canMove = false;
    }

}
