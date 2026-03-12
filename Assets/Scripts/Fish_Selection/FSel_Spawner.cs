using UnityEngine;
using UnityEngine.Events;

public class FSel_Spawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Spawning Data")]
    [Tooltip("Prefab of fish that can spawn.")]
    public FSel_Fish[] FishPrefab;

    [Header("Spawning Config")]
    [Tooltip("How does the fish spawn.\n" +
        "RandomSingle will spawn new fish once the old one are sorted/destroyed.\n" +
        "RandomTimeBased will spawn fish based on spawn rate, ehich will incrementally increase in speed\n" +
        "Queue will spawn fish according to a defined list of fish within the queue.")]
    public SpawnMode spawnMode = SpawnMode.RandomTimeBased;
    [Tooltip("Used to define a base spawn interval in seconds. [RandomTimebased Spawnmode]")]
    public float baseSpawnRate = 4;
    [Tooltip("Used to define a minimum/fastest spawn interval in seconds. [RandomTimebased Spawnmode]")]
    public float minSpawnRate = 1.5f;
    [Tooltip("Used to define a how much to reduce the spawn intervals on correct sorting in percentage. [RandomTimebased Spawnmode]")]
    public float SpawnRateIncrement = 0.1f;
    [Tooltip("How much speed the fish will get with each combo.")]
    public float SpeedModifierIncrement = 0.05f;
    [Tooltip("Index of fish to spawn in order, use the index in FishPrefab. [Queue Spawnmode]")]
    public int[] SpawningQueue;

    [Header("StaticClass config")]
    public GameObject ScoreIndicator;
    public GameObject EndingPanel;

    FSel_Fish _fih;
    float lastSpawn;
    int queue = 0;
    float spawnRate;
    bool isEnd = false;
    float speedModifier = 1;

    public enum SpawnMode
    {
        RandomSingle,
        RandomTimeBased,
        Queue
    }

    void Start()
    {
    }
    private void Awake()
    {
        lastSpawn = Time.time;
        spawnRate = baseSpawnRate;
        if(spawnMode == SpawnMode.RandomSingle || spawnMode == SpawnMode.RandomTimeBased)
        {
            //Use normal spawning when Random or Timebased
            SpawnFish();
        }
        else
        {
            SpawnFish(queue);
        }
        FSel_ScoreManager.scoreIndicator = ScoreIndicator;
        FSel_ScoreManager.selectionScore = 0;
        FSel_ScoreManager.DisplayScore();
    }
    void OnDrawGizmos() 
    {
        // Set the color for the gizmo
        Gizmos.color = Color.blue;
        // Draw a sphere at the GameObject's position
        Gizmos.DrawLine(transform.position, transform.position - Vector3.right * 30);
        Gizmos.DrawLine(transform.position - Vector3.right * 30 + Vector3.up * 20, transform.position - Vector3.right * 30 - Vector3.up * 20);
    }
    // Update is called once per frame
    void Update()
    {
        if (spawnMode == SpawnMode.RandomTimeBased && Time.time - lastSpawn > spawnRate && !isEnd)
        {
            SpawnFish();
            lastSpawn = Time.time;
        }
        else if (spawnMode == SpawnMode.Queue && queue < SpawningQueue.Length && !isEnd)
        {
            SpawnFish(queue);
        }
    }
    public void OnFishDestroyed()
    {
        if (spawnMode == SpawnMode.RandomSingle)
        {
            SpawnFish();
        }
        else if (spawnMode == SpawnMode.Queue && queue < SpawningQueue.Length)
        {
            SpawnFish(queue);
        }
    }
    public void SpawnFish(int index)
    {
        if (_fih != null)
        {
            _fih.OnSorted.RemoveAllListeners();
        }
        Spawn(SpawningQueue[queue]);
        queue++;
    }
    public void SpawnFish()
    {
        if (_fih != null)
        {
            _fih.OnSorted.RemoveAllListeners();
        }
        int rand = Random.Range(0, 7);
        Spawn(rand);
    }
    public void Spawn(int Type)
    {
        _fih = Instantiate(FishPrefab[Type], transform);
        _fih.transform.position = transform.position;
        _fih.moveSpeed += _fih.moveSpeed * speedModifier;
        FSel_ScoreManager.activeFish = _fih;
        _fih.OnCorrect.AddListener(OnCorrect);
        _fih.OnCorrect.AddListener(FSel_ScoreManager.OnCorrect);
        _fih.OnIncorrect.AddListener(OnIncorrect);
        _fih.OnIncorrect.AddListener(FSel_ScoreManager.OnIncorrect);
        _fih.OnDestroyed.AddListener(OnFishDestroyed);
        _fih.OnDiscard.AddListener(OnDiscard);
    }
    public void OnCorrect(GameObject go)
    {
        spawnRate = Mathf.Clamp(spawnRate - (spawnRate * SpawnRateIncrement), minSpawnRate, baseSpawnRate);
        speedModifier += SpeedModifierIncrement;
    }
    public void OnDiscard()
    {

    }
    public void OnIncorrect(GameObject go)
    {
        spawnRate = baseSpawnRate;
        speedModifier = 1;
    }
    public void OnEndGame()
    {
        if(_fih != null)
        {
            _fih.endGame();
        }
        EndingPanel.SetActive(true);
        isEnd = true;
    }
}
