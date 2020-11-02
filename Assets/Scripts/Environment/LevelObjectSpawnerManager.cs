using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectSpawnerManager : MonoBehaviour
{
    [SerializeField, Tooltip("Bonus Object list")]
    private List<GameObject> m_levelBonusObjectsList = null;

    [SerializeField, Tooltip("Malus Object list")]
    private List<GameObject> m_levelMalusObjectsList = null;

    [SerializeField, Tooltip("The curve describe the probability of bonus to spawn. 1 - this probability is the porbability of malus spawning.")]
    private AnimationCurve m_difficultyAnimationCurve = null;

    [SerializeField, Tooltip("In seconds : gives the time between each spawn of object on the map depending on the game time elapsed")]
    private AnimationCurve m_spawnRateAnimationCurve = null;

    [SerializeField, Tooltip("Range in x spawn")]
    private float m_xSpawnRadius = 30.0f;

    [SerializeField, Tooltip("Range in z spawn")]
    private float m_zSpawnRadius = 100.0f;

    // Some variables used as buffers so we don't create local variables which can trigger the GC at the end of their execution
    // Some variables used as references so we don't need to make extra calls
    private GameObject m_playerReference;   // Will be used to know the player's position
    private GameObject m_objectToSpawnBuffer;
    private Coroutine m_spawnObjectCoroutineReference;
    private Vector3 m_spawnPositionBuffer;

    private float m_spawnAngleBuffer;
    private float m_spawnTimer;
    private float m_probabilityValueBuffer;
    private int m_indexValueBuffer;

    private void OnEnable()
    {
        GameManager.OnGameStart += StartSpawnObjectCoroutine;
        GameManager.OnGameOver += StopSpawnObjectCoroutine;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= StartSpawnObjectCoroutine;
        GameManager.OnGameOver -= StopSpawnObjectCoroutine;
    }

    private void Start()
    {
        m_playerReference = GameManager.instance.m_player; // Retreive player references

        if (m_playerReference == null)
            Debug.LogError("Failed to retreive player reference in the game manager!");

        if (m_spawnRateAnimationCurve == null || m_difficultyAnimationCurve == null)
            Debug.LogError("One of the animation curve is null in the inspector!");

        if (m_levelMalusObjectsList == null || m_levelBonusObjectsList == null)
            Debug.LogError("One of the bonus or malus object list is null in the inspector!");
    }

    private void StartSpawnObjectCoroutine()
    {
        if (m_spawnObjectCoroutineReference != null)
            StopSpawnObjectCoroutine();

        m_spawnObjectCoroutineReference = StartCoroutine(SpawnObjectCoroutine());
    }

    private void StopSpawnObjectCoroutine()
    {
        StopCoroutine(m_spawnObjectCoroutineReference);
    }

    /// <summary>
    /// Coroutine that handles the spawn of objects on the level (coins, traps, lasers)
    /// The coroutine starts when the player start a game and stops when the player dies.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnObjectCoroutine()
    {
        // Using animation curve to define the time between each spawns
        // Gives the number of seconds between each spawns depending on the current game time
        m_spawnTimer = m_spawnRateAnimationCurve.Evaluate(GameManager.instance.m_gameTime);

        yield return new WaitForSeconds(m_spawnTimer);

        // Using an animation curve to control the difficulty
        // Gives the probability of spawning a good object (coin) or a bad object (trap/laser)
        m_probabilityValueBuffer = m_difficultyAnimationCurve.Evaluate(GameManager.instance.m_gameTime);

        // The object is spawned at a certain range from the player so he can't see them spawn on the screen
        m_spawnPositionBuffer = ComputeSpawnPosition() + m_playerReference.transform.position;

        if (Random.Range(0.0f, 1.0f) < m_probabilityValueBuffer)
            m_objectToSpawnBuffer = PickRandomInList(m_levelBonusObjectsList);
        else
            m_objectToSpawnBuffer = PickRandomInList(m_levelMalusObjectsList);

        // Takes the object to spawn form the pool and set it at the right location
        if (m_objectToSpawnBuffer != null)
            PoolManager.instance.SpawnPooledObject(m_objectToSpawnBuffer, m_spawnPositionBuffer, Quaternion.identity);

        // Do the object spwing process again
        m_spawnObjectCoroutineReference = StartCoroutine(SpawnObjectCoroutine());
    }

    /// <summary>
    /// returns a position ON a circle of a given radius
    /// </summary>
    /// <returns></returns>
    private Vector3 ComputeSpawnPosition()
    {
        m_spawnAngleBuffer = Random.Range(0, Mathf.PI * 2);
        return new Vector3(Mathf.Cos(m_spawnAngleBuffer) * m_xSpawnRadius, 0.0f, Mathf.Sin(m_spawnAngleBuffer) * m_zSpawnRadius);
    }

    /// <summary>
    /// Method that pick a random game object in a list of game objects
    /// </summary>
    /// <param name="objectList"></param>
    /// <returns></returns>
    private GameObject PickRandomInList(List<GameObject> objectList)
    {
        if (objectList.Count > 0)
        {
            m_indexValueBuffer = Random.Range(0, objectList.Count);
            return objectList[m_indexValueBuffer];
        }
        return null;
    }


}
