using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour, IPoolable
{
    // Sends the event with the game object in contact with the laser in parameter
    public static Action<GameObject> OnLaserTriggered;

    [SerializeField, Tooltip("Exlectric sound effect.")]
    private AudioSource m_electricSoundAudioSource = null;

    private void Start()
    {
        if (m_electricSoundAudioSource == null)
            Debug.LogError("No electric sound refered in the inspector!", gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            m_electricSoundAudioSource.Play();      // Plays the audio
            BroadcastLaserEvent(other.gameObject);  // Sends the event with the object that triggered the laser
        }
    }

    private void BroadcastLaserEvent(GameObject objectToAffect)
    {
        if (OnLaserTriggered != null)
            OnLaserTriggered(objectToAffect);
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
