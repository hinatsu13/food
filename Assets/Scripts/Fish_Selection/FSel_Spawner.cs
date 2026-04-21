using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class FSel_Spawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Spawning Data")]
    [Tooltip("Prefab of fish that can spawn.")]
    [SerializeField]public FishSpawning[] FishPrefab;

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
    public GameObject endScoreIndicator;
    public GameObject EndingPanel;
    public StarDisplay star;
    [Tooltip("How much score corelates to how many stars are displayed, with index representing the number of stars.")]
    public int[] winCondition;

    FSel_Fish _fih;
    float lastSpawn;
    int queue = 0;
    float spawnRate;
    bool isEnd = false;
    float speedModifier = 1;
    int lastFish;
    [Serializable]
    public struct FishSpawning {
        [SerializeField] public FSel_Fish FishPrefab;
        [SerializeField] public float spawnRate;
    }
    public enum SpawnMode
    {
        RandomSingle,
        RandomTimeBased,
        Queue
    }
    private float totalWeight;

    void Start()
    {
    }
    private void Awake()
    {
        foreach (var fish in FishPrefab) totalWeight += fish.spawnRate;
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
        //Pick a random point between 0 and the total sum
        float roll = Random.Range(0, totalWeight);
        float cumulative = 0;
        int selectedIndex = 0;

        //Iterate through the prefabs to see where the "roll" landed
        for (int i = 0; i < FishPrefab.Length; i++)
        {
            cumulative += FishPrefab[i].spawnRate;
            if (roll <= cumulative)
            {
                selectedIndex = i;
                break;
            }
        }
        //int goodBad = Random.Range(0, 2);
        //int rand = Random.Range(0, 4) + (goodBad * 4);
        Spawn(selectedIndex);
    }
    public void Spawn(int Type)
    {
        _fih = Instantiate(FishPrefab[Type].FishPrefab, transform.parent);
        lastFish = Type;
        _fih.transform.position = transform.position;
        _fih.moveSpeed += _fih.moveSpeed * speedModifier;
        FSel_ScoreManager.activeFish = _fih;
        _fih.OnCorrect.AddListener(OnCorrect);
        _fih.OnCorrect.AddListener(FSel_ScoreManager.OnCorrect);
        _fih.OnIncorrect.AddListener(OnIncorrect);
        _fih.OnIncorrect.AddListener(FSel_ScoreManager.OnIncorrect);
        _fih.OnDestroyed.AddListener(OnFishDestroyed);
        _fih.OnDiscard.AddListener(OnDiscard);
        _fih.OnDiscard.AddListener(FSel_ScoreManager.OnDiscard);
        _fih.OnFailed.AddListener(FSel_ScoreManager.OnFail);
    }
    public void OnCorrect(GameObject go)
    {
        spawnRate = Mathf.Clamp(spawnRate - (spawnRate * SpawnRateIncrement), minSpawnRate, baseSpawnRate);
        speedModifier += SpeedModifierIncrement;
    }
    public void OnDiscard(bool correct)
    {
        Debug.Log($"Discard correctly: {correct }");
        if (correct)
        {
            spawnRate = Mathf.Clamp(spawnRate - (spawnRate * SpawnRateIncrement), minSpawnRate, baseSpawnRate);
            speedModifier += SpeedModifierIncrement;
        }
        else
        {
            spawnRate = baseSpawnRate;
            speedModifier = 1;
        }
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
        star.displayStar(GetStarValue(FSel_ScoreManager.selectionScore), FSel_ScoreManager.selectionScore);
        StateManager.setFishSelection(FSel_ScoreManager.selectionScore);
        EndingPanel.SetActive(true);
        isEnd = true;
    }

    public int GetStarValue(int inputValue)
    {
        if (inputValue > winCondition[3]) return 3;
        if (inputValue > winCondition[2]) return 2;
        if (inputValue > winCondition[1]) return 1;
        if (inputValue > winCondition[0]) return 0;

        return 0;
    }
}
