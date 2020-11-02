using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Action that sends an event when the player dies (Triggers the game over event)
    public static Action OnPlayerDeath;

    // timer used when triggering the invincibility feedback. Time elapsed before swithcing the state of the player model to true or false
    private const float DELAY_INVINCIBILITY_FEEDBACK = 0.2f;

    [SerializeField, Tooltip("Reference to the car model.")]
    private GameObject m_playerModel = null;

    [SerializeField, Tooltip("List of the player model component such as lights of the car.")]
    private List<GameObject> m_playerModelComponents = null;

    [SerializeField, Tooltip("Reference of the explosion fx to play when the player dies.")]
    private GameObject m_explosionFX = null;

    [SerializeField, Tooltip("Audio played when the player hits an enemy car.")]
    private AudioSource m_carScratchAudioSource = null;

    [SerializeField, Tooltip("Invincibility time when the player is hit by an enemy.")]
    private float m_invincibilityTime = 2.0f;

    [SerializeField, Tooltip("Health of the player at the start of a game.")]
    private int m_playerOriginalHealth = 3;


    private Coroutine m_invincibilityCoroutine;
    private float m_timerBuffer;
    private int m_currentHealth;
    private bool m_isHitable;

    private void OnEnable()
    {
        Traps.OnDestructionTrapActivated += DecreaseHealth;
        Laser.OnLaserTriggered += DecreaseHealth;
        GameManager.OnTitleScreen += InitializePlayer;
    }

    private void OnDisable()
    {
        Traps.OnDestructionTrapActivated -= DecreaseHealth;
        Laser.OnLaserTriggered -= DecreaseHealth;
        GameManager.OnTitleScreen -= InitializePlayer;
    }


    private void Awake()
    {
        if (m_playerModel == null)
            Debug.LogError("The player model is missing in the inspector! (Needed for invincibility feedback)");
        if (m_explosionFX == null)
            Debug.LogError("No explosion fx prefab referenced in the inspector!");
        if (m_carScratchAudioSource == null)
            Debug.LogError("No car scratch audio source referenced in the inspector!");
        if (m_playerModelComponents == null || m_playerModelComponents.Count == 0)
            Debug.LogError("The player component list is null or empty! Refer the components like lights.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && m_isHitable)
        {
            m_carScratchAudioSource.Play();
            DecreaseHealth(gameObject);
        }
    }

    private void InitializePlayer()
    {
        m_playerModel.SetActive(true);
        SwitchPlayerModelComponentState(true);

        m_currentHealth = m_playerOriginalHealth;
        m_isHitable = true;
        m_timerBuffer = 0.0f;
    }

    private void DecreaseHealth(GameObject obj)
    {
        if (obj == gameObject)
        {
            if (m_currentHealth > 0)
            {
                m_currentHealth--;

                StartInvincibilityCoroutine();

                if (m_currentHealth == 0)
                    Kill();
            }
        }
    }

    /// <summary>
    /// Method called when the player lost his last health point
    /// Spawns an explosion fx, set the model of the player at false
    /// Broadcasts the player's death event
    /// </summary>
    private void Kill()
    {
        if (m_invincibilityCoroutine != null)
            StopCoroutine(m_invincibilityCoroutine);

        PoolManager.instance.SpawnPooledObject(m_explosionFX, transform.position, Quaternion.identity);

        SwitchPlayerModelComponentState(false);
        m_playerModel.SetActive(false);
        m_isHitable = false;

        BroadcastPlayerDeathEvent();
    }

    /// <summary>
    /// Method that starts the invincibility coroutine
    /// </summary>
    private void StartInvincibilityCoroutine()
    {
        if (m_invincibilityCoroutine != null)
            StopCoroutine(m_invincibilityCoroutine);

        m_invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
    }


    /// <summary>
    /// Coroutine that manages the invincibility behavior and the visual feedback
    /// </summary>
    /// <returns></returns>
    private IEnumerator InvincibilityCoroutine()
    {
        m_isHitable = false;
        bool modelState = false;

        while (m_timerBuffer < m_invincibilityTime)
        {
            m_playerModel.SetActive(modelState);
            modelState = !modelState;
            m_timerBuffer += DELAY_INVINCIBILITY_FEEDBACK;
            yield return new WaitForSeconds(DELAY_INVINCIBILITY_FEEDBACK);
        }

        m_timerBuffer = 0.0f;
        m_playerModel.SetActive(true);
        m_isHitable = true;
    }

    private void SwitchPlayerModelComponentState(bool state)
    {
        for (int i = 0; i < m_playerModelComponents.Count; i++)
            m_playerModelComponents[i].SetActive(state);
    }

    private void BroadcastPlayerDeathEvent()
    {
        if (OnPlayerDeath != null)
            OnPlayerDeath();
    }


}
