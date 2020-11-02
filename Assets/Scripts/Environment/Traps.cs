using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Traps can only be activated by the player. 
/// After activation and a short amount of time, the trap applies its effect to the objects in range.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Traps : MonoBehaviour, IPoolable
{
    // Events that send the game object in the range of effect in parameter
    public static Action<GameObject> OnDestructionTrapActivated;
    public static Action<GameObject, float, float> OnSlowTrapActivated;


    private enum TrapType
    {
        Destruction, //Trap that destroys enemies or decrease player's health
        Slow //Trap that slows the player or enemies
    }

    [SerializeField, Tooltip("List of explosion Fx.")]
    private List<GameObject> m_explosionFXList = null;

    [SerializeField, Tooltip("Audio played when traps are activated.")]
    private AudioSource m_activationAudioSource = null;

    [SerializeField, Tooltip("When the player activates a trap, number of second before the trap apply its effects")]
    private float m_timeBeforeActivation = 2.0f;

    [SerializeField, Tooltip("Range of effect of the trap in world units.")]
    private float m_radiusOfEffect = 20.0f;

    [SerializeField, Tooltip("The effectiveness of the slow on the affected objects.")]
    private float m_slowAmount = 1.0f;

    [SerializeField, Tooltip("How long the objects affected are slowed.")]
    private float m_slowDuration = 5.0f;

    [SerializeField, Tooltip("Type of the trap")]
    private TrapType m_trapType = TrapType.Destruction;

    private Coroutine m_trapActivationCoroutineReference;

    private void Awake()
    {
        DrawAreaOfEffect();
    }

    private void Start()
    {
        if (m_explosionFXList == null || m_explosionFXList.Count == 0)
            Debug.LogError("Explosion fx list is null or empty!");
        if (m_activationAudioSource == null)
            Debug.LogError("No audio source component for activation sound refered in the inspector!");
    }

    /// <summary>
    /// Only the player is able to set up traps
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            m_trapActivationCoroutineReference = StartCoroutine(TrapActivationCoroutine());
    }

    /// <summary>
    /// Coroutine that gets the objects within the radius of effect and apply the effect of the trap
    /// </summary>
    /// <returns></returns>
    private IEnumerator TrapActivationCoroutine()
    {
        m_activationAudioSource.Play();
        yield return new WaitForSeconds(m_timeBeforeActivation);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_radiusOfEffect);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.CompareTag("Enemy") || hitColliders[i].gameObject.CompareTag("Player"))
                ApplyTrapEffect(hitColliders[i].gameObject);
        }

        ReturnToPool();
    }

    /// <summary>
    /// Apply the effect of the traps by sending the events with the affected objects in parameters
    /// the objects receiving the events will wheck if they are corresponding to the object in parameter
    /// and apply the right effect on themselves (destruction or reduce movement speed)
    /// </summary>
    /// <param name="objectToAffect"></param>
    private void ApplyTrapEffect(GameObject objectToAffect)
    {
        switch (m_trapType)
        {
            case TrapType.Destruction:
                PoolManager.instance.SpawnPooledObject(PickRandomExplosionFX(), transform.position, Quaternion.identity);
                BroadcastDestructionTrapEvent(objectToAffect);
                break;
            case TrapType.Slow:
                PoolManager.instance.SpawnPooledObject(PickRandomExplosionFX(), transform.position, Quaternion.identity);
                BroadcastSlowTrapEvent(objectToAffect);
                break;
        }
    }

    private GameObject PickRandomExplosionFX()
    {
        return m_explosionFXList[UnityEngine.Random.Range(0, m_explosionFXList.Count)];
    }


    private void BroadcastDestructionTrapEvent(GameObject objectToAffect)
    {
        if (OnDestructionTrapActivated != null)
            OnDestructionTrapActivated(objectToAffect);
    }

    private void BroadcastSlowTrapEvent(GameObject objectToAffect)
    {
        if (OnSlowTrapActivated != null)
            OnSlowTrapActivated(objectToAffect, m_slowAmount, m_slowDuration);
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    private void DrawAreaOfEffect()
    {
        gameObject.DrawCircle(m_radiusOfEffect, 0.2f);
    }

}
