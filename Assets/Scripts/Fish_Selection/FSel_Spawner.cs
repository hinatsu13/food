using UnityEngine;
using UnityEngine.Events;

public class FSel_Spawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Spawning Data")]
    [Tooltip("Prefab of fish that can spawn.")]
    public FSel_Fish[] FishPrefab;

    [Header("Spawning Config")]
    [Tooltip("Index of fish to spawn in order, use the index in FishPrefab.")]
    public int[] SpawningQueue;
    public bool randomMode = true;

    [Header("StaticClass config")]
    public GameObject ScoreIndicator;
    public GameObject EndingPanel;

    FSel_Fish _fih;
    float lastSpawn;
    int queue = 0;
    public UnityEvent OnEnd;

    void Start()
    {
    }
    private void Awake()
    {
        lastSpawn = Time.time;
        if(randomMode)
        {
            Debug.Log("Enter");
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
    }
    public void OnFishDestroyed()
    {
        if (randomMode)
        {
            SpawnFish();
        }
        else if (queue < SpawningQueue.Length)
        {
            SpawnFish(queue);
        }
        else
        {
            OnEndGame();
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
    public void SpawnFish()
    {
        int rand = Random.Range(0, 7);
        _fih = Instantiate(FishPrefab[rand], GetComponentInParent<Canvas>().transform);
        _fih.transform.position = transform.position;
        FSel_ScoreManager.activeFish = _fih;
        _fih.OnCorrect.AddListener(FSel_ScoreManager.OnCorrect);
        _fih.OnIncorrect.AddListener(FSel_ScoreManager.OnIncorrect);
        _fih.OnDestroyed.AddListener(OnFishDestroyed);
    }
    public void OnEndGame()
    {
        EndingPanel.SetActive(true);
        OnEnd?.Invoke();
    }
}
