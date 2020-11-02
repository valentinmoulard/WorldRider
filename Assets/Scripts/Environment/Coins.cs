using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour, IPoolable
{
    // Variable used to let the coin be alive for x seconds because the sound of the coin needs to be played before returning to the pool
    private const float DELAY_BEFORE_RETURNING_TO_POOL = 2.0f;

    // Sends the event with the score value of the current coin
    public static Action<float> OnCoinCollected;

    [SerializeField, Tooltip("Refer here the model and the lights of the coin.")]
    private List<GameObject> m_coinModelComponentList = null;

    [SerializeField, Tooltip("Sound played when a coin is collected.")]
    private AudioSource m_coinCollectedAudioSource = null;

    [SerializeField, Tooltip("Score value of the current coin.")]
    private float m_scoreValue = 1.0f;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_coinCollectedAudioSource.Play();          //Plays the collected coin audio
            SwitchCoinComponentState(false);            // Sets the model and light off
            BroadcastCoinCollectedEvent();              // Sends the coin collected event
            StartCoroutine(CoinPoolReturnCoroutine());  //Start coroutine of returning to the pool
        }
    }

    private void Start()
    {
        if (m_coinModelComponentList == null || m_coinModelComponentList.Count == 0)
            Debug.LogError("The coin Components are not refered in the inspector (coin model and light)!");

        if (m_coinCollectedAudioSource == null)
            Debug.LogError("No coin collected audio referenced in the inspector!");
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    private void BroadcastCoinCollectedEvent()
    {
        if (OnCoinCollected != null)
            OnCoinCollected(m_scoreValue);
    }

    /// <summary>
    /// Coroutine for the coin to return to the pool
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoinPoolReturnCoroutine()
    {
        yield return new WaitForSeconds(DELAY_BEFORE_RETURNING_TO_POOL);
        SwitchCoinComponentState(true);
        ReturnToPool();
    }

    /// <summary>
    /// Methods that manage the active state of the coin components
    /// </summary>
    /// <param name="state"></param>
    private void SwitchCoinComponentState(bool state)
    {
        for (int i = 0; i < m_coinModelComponentList.Count; i++)
            m_coinModelComponentList[i].SetActive(state);
    }

}
