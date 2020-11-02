using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    // Event sent when the enemy is killed. Gives the score value of the current enemy.
    public static Action<float> OnBroadcastedScoreValue;

    [SerializeField, Tooltip("Gameobject of the explosion FX to play.")]
    private GameObject m_explosionFX = null;

    [SerializeField, Tooltip("Current score value.")]
    private float m_scoreValue = 2;


    private void OnEnable()
    {
        Traps.OnDestructionTrapActivated += Kill;
        Laser.OnLaserTriggered += Kill;
    }

    private void OnDisable()
    {
        Traps.OnDestructionTrapActivated -= Kill;
        Laser.OnLaserTriggered -= Kill;
    }

    private void Start()
    {
        if (m_explosionFX == null)
            Debug.LogError("No explosion fx prefab referenced in the inspector!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            Kill(gameObject);
    }


    private void Kill(GameObject objectToAffect)
    {
        if (objectToAffect == gameObject)
        {
            BroadcastScoreValue();

            PoolManager.instance.SpawnPooledObject(m_explosionFX, transform.position, Quaternion.identity); //When the enemy dies, spawns an explosion FX
            ReturnToPool();
        }
    }

    /// <summary>
    /// Methods that send the score value of the object. The score manager will catch this event and will increase the total score
    /// </summary>
    private void BroadcastScoreValue()
    {
        if (OnBroadcastedScoreValue != null)
            OnBroadcastedScoreValue(m_scoreValue);
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
