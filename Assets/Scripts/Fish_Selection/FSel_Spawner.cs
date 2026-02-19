using UnityEngine;

public class FSel_Spawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Spawning Data")]
    [Tooltip("Prefab of fish that can spawn.")]
    public FSel_Fish[] FishPrefab;

    [Header("Spawning Config")]
    [Tooltip("Index of fish to spawn in order, use the index in FishPrefab.")]
    public int[] SpawningQueue;

    [Header("StaticClass config")]
    public GameObject ScoreIndicator;

    FSel_Fish _fih;
    float lastSpawn;
    int queue = 0;

    void Start()
    {
        lastSpawn = Time.time;
    }
    private void Awake()
    {
        SpawnFish(queue);
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
    }
    public void OnFishDestroyed()
    {
        if (queue < SpawningQueue.Length)
        {
            SpawnFish(queue);
        }
        else
        {
            Debug.Log("Show end screen, move on to other scene");
        }
    }
    public void SpawnFish(int index)
    {
        if (_fih != null)
        {
            _fih.OnSorted.RemoveAllListeners();
        }
        _fih = Instantiate(FishPrefab[SpawningQueue[queue]], GetComponentInParent<Canvas>().transform);
        _fih.transform.position = transform.position;
        FSel_ScoreManager.activeFish = _fih;
        _fih.OnCorrect.AddListener(FSel_ScoreManager.OnCorrect);
        _fih.OnIncorrect.AddListener(FSel_ScoreManager.OnIncorrect);
        _fih.OnDestroyed.AddListener(OnFishDestroyed);
        queue++;
    }
}
