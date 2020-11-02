using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IPoolable))]
public class PoolableDistance : MonoBehaviour
{
    [SerializeField, Tooltip("If the distance between the player and the object is greater than the value, it returns to the pool")]
    private float m_maxDistance = 200.0f;

    [SerializeField, Tooltip("Check every x seconds the distance betweent the player and the object")]
    private float m_distanceCheckDelay = 5.0f;

    private Coroutine m_distanceCheckCoroutine;
    private IPoolable m_poolable;

    void Start()
    {
        m_poolable = this.GetComponent<IPoolable>();

        if (m_poolable == null)
            Debug.LogError("No poolable object implementing the poolable interface has been found!", this.gameObject);

        m_distanceCheckCoroutine = StartCoroutine(DistanceCheckCoroutine());
    }

    private IEnumerator DistanceCheckCoroutine()
    {
        yield return new WaitForSeconds(m_distanceCheckDelay);
        CheckDistance();
    }

    private void CheckDistance()
    {
        if (Vector3.Distance(GameManager.instance.m_player.transform.position, gameObject.transform.position) > m_maxDistance)
            ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (m_distanceCheckCoroutine != null)
            StopCoroutine(m_distanceCheckCoroutine);

        m_poolable.ReturnToPool();
    }
}
