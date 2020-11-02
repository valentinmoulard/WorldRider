using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Reference of the object to chase. The player in the current case
    private GameObject m_objectToChase;

    [SerializeField, Tooltip("How fast the enemy moves.")]
    private float m_originalMovementSpeed = 2.0f;

    [SerializeField, Tooltip("How fast can this enemy rotate.")]
    private float m_rotationSpeed = 50.0f;


    private Coroutine m_slowSpeedCoroutineReference;
    private Quaternion m_desiredRotation;
    private Vector3 m_directionToTarget;
    private float m_angle;
    private float m_movementSpeed;


    private void OnEnable()
    {
        InitialzeEnemy();
        Traps.OnSlowTrapActivated += DecreaseSpeedFromTrap;
    }

    private void OnDisable()
    {
        Traps.OnSlowTrapActivated -= DecreaseSpeedFromTrap;
    }


    void Update()
    {
        transform.position += transform.forward * m_movementSpeed * Time.deltaTime;

        Rotate();
    }

    private void InitialzeEnemy()
    {
        m_objectToChase = GameManager.instance.m_player;
        m_movementSpeed = m_originalMovementSpeed;
    }


    private void Rotate()
    {
        if (m_objectToChase != null)
            m_directionToTarget = (m_objectToChase.transform.position - gameObject.transform.position).normalized;
        else
            m_directionToTarget = Vector3.forward;

        m_desiredRotation = Quaternion.LookRotation(m_directionToTarget);

        // Using lerp so the rotation is smooth and not instantaniously
        transform.rotation = Quaternion.Lerp(transform.rotation, m_desiredRotation, m_rotationSpeed * Time.deltaTime);
    }


    private void DecreaseSpeedFromTrap(GameObject objectToAffect, float slowAmount, float slowDuration)
    {
        if (objectToAffect == gameObject)
        {
            if (m_slowSpeedCoroutineReference != null)
                StopCoroutine(m_slowSpeedCoroutineReference);

            m_slowSpeedCoroutineReference = StartCoroutine(DecraseSpeedCoroutine(slowAmount, slowDuration));
        }
    }

    /// <summary>
    /// Coroutine that decrease the current speed of the enemy for a defined amout of time. Restores the original speed after that.
    /// </summary>
    /// <param name="slowAmount"></param>
    /// <param name="slowDuration"></param>
    /// <returns></returns>
    private IEnumerator DecraseSpeedCoroutine(float slowAmount, float slowDuration)
    {
        m_movementSpeed -= slowAmount;
        Mathf.Clamp(m_movementSpeed, 0.0f, 100.0f);
        yield return new WaitForSeconds(slowDuration);
        m_movementSpeed = m_originalMovementSpeed;
    }


}
