using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, Tooltip("The list of enemy prefabs to spawn")]
    private List<GameObject> m_enemyPrefabsRefenreces = null;

    [SerializeField, Tooltip("The enemies will spawn at this distance in world units.")]
    private float m_spawnRadius = 100.0f;

    [SerializeField, Tooltip("Delay before each spawns in seconds.")]
    private float m_spawnTime = 5.0f;

    // Buffer variables and references variables
    private GameObject m_objectToChase;
    private Vector3 m_spawnPositionBuffer;
    private Vector3 m_directionToPlayerBuffer;
    private Quaternion m_spawnRotationBuffer;
    private float m_timerBuffer = 0.0f;
    private float m_spawnAngleBuffer;
    private float m_rotationAngleBuffer;
    private int m_randomIndexInListBuffer;
    private bool m_canSpawn;

    private void OnEnable()
    {
        GameManager.OnGameStart += EnableSpawn;
        GameManager.OnGameOver += DisableSPawn;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= EnableSpawn;
        GameManager.OnGameOver -= DisableSPawn;
    }

    private void Start()
    {
        if (m_enemyPrefabsRefenreces == null || m_enemyPrefabsRefenreces.Count == 0)
            Debug.LogError("The prefab list is null or empty!");

        m_objectToChase = GameManager.instance.m_player;
    }

    void Update()
    {
        if (m_canSpawn)
        {
            m_timerBuffer += Time.deltaTime;
            if (m_timerBuffer > m_spawnTime)
            {
                m_timerBuffer = 0.0f;

                SpawnEnemy();
            }
        }
    }

    /// <summary>
    /// Method that picks a random enemy in the enemy prefabs list and determine the position and rotation. Then takes the enemy from the pool and set it correctly.
    /// </summary>
    private void SpawnEnemy()
    {
        m_randomIndexInListBuffer = Random.Range(0, m_enemyPrefabsRefenreces.Count);

        m_spawnPositionBuffer = ComputeSpawnPosition() + m_objectToChase.transform.position;

        m_spawnRotationBuffer = GetEnemyRotationTowardsPlayer(m_spawnPositionBuffer);

        PoolManager.instance.SpawnPooledObject(m_enemyPrefabsRefenreces[m_randomIndexInListBuffer], m_spawnPositionBuffer, m_spawnRotationBuffer);
    }

    /// <summary>
    /// Methods that determine a position on a circle of a defined radius
    /// </summary>
    /// <returns></returns>
    private Vector3 ComputeSpawnPosition()
    {
        m_spawnAngleBuffer = Random.Range(0, Mathf.PI * 2);
        return new Vector3(Mathf.Cos(m_spawnAngleBuffer) * m_spawnRadius, 0.0f, Mathf.Sin(m_spawnAngleBuffer) * m_spawnRadius);
    }

    /// <summary>
    /// Mehtods that determine the rotation of the enmy to spawn depending on the position of the player. The enemy must face the player as it moves towards the player.
    /// </summary>
    /// <param name="spawnPosition"></param>
    /// <returns></returns>
    private Quaternion GetEnemyRotationTowardsPlayer(Vector3 spawnPosition)
    {
        if (m_objectToChase != null)
            m_directionToPlayerBuffer = (m_objectToChase.transform.position - spawnPosition).normalized;
        else
            m_directionToPlayerBuffer = Vector3.forward;


        m_rotationAngleBuffer = Vector3.SignedAngle(Vector3.forward, m_directionToPlayerBuffer, Vector3.up);

        return Quaternion.Euler(new Vector3(0f, m_rotationAngleBuffer, 0f));
    }


    private void EnableSpawn()
    {
        m_canSpawn = true;
    }

    private void DisableSPawn()
    {
        m_canSpawn = false;
    }

}
