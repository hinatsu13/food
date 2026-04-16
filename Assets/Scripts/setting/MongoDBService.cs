using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MongoDBService : MonoBehaviour
{
    private static MongoDBService _instance;
    public static MongoDBService Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MongoDBService");
                _instance = go.AddComponent<MongoDBService>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // ⚠️ Change this to your deployed backend URL
    // Local testing: "http://localhost:3000"
    // After deploying to Render/Railway: "https://your-app-name.onrender.com"
    [SerializeField]
    private string apiBaseUrl = "https://food-theta-nine.vercel.app/";

    public static string ApiBaseUrl
    {
        get => Instance.apiBaseUrl;
        set => Instance.apiBaseUrl = value;
    }

    /// <summary>
    /// Sends the score data to MongoDB via the backend API.
    /// </summary>
    public static void SendScore(string playerName, int fishSelectionScore, int fishPrepScore,
        int fishCheckTempScore, int fishPackagingScore, Action<bool> onComplete = null)
    {
        Instance.StartCoroutine(Instance.SendScoreCoroutine(
            playerName, fishSelectionScore, fishPrepScore, fishCheckTempScore, fishPackagingScore, onComplete));
    }

    /// <summary>
    /// Parameterless overload — reads all values from StateManager.
    /// </summary>
    public static void SendScore(Action<bool> onComplete = null)
    {
        SendScore(
            StateManager.getPlayerName(),
            StateManager.getFishSelection(),
            StateManager.getFishPrep(),
            StateManager.getFishCheckTemp(),
            StateManager.getFishPackaging(),
            onComplete
        );
    }

    private IEnumerator SendScoreCoroutine(string playerName, int fishSelectionScore, int fishPrepScore,
        int fishCheckTempScore, int fishPackagingScore, Action<bool> onComplete)
    {
        // Build JSON payload
        ScorePayload payload = new ScorePayload
        {
            playerName = playerName,
            fishSelectionScore = fishSelectionScore,
            fishPrepScore = fishPrepScore,
            fishCheckTempScore = fishCheckTempScore,
            fishPackagingScore = fishPackagingScore
        };

        string jsonData = JsonUtility.ToJson(payload);
        string url = apiBaseUrl.TrimEnd('/') + "/api/scores";

        Debug.Log($"[MongoDBService] Sending score to: {url}");
        Debug.Log($"[MongoDBService] Payload: {jsonData}");

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[MongoDBService] ✅ Score saved successfully: {request.downloadHandler.text}");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"[MongoDBService] ❌ Failed to save score: {request.error}");
                Debug.LogError($"[MongoDBService] Response: {request.downloadHandler.text}");
                onComplete?.Invoke(false);
            }
        }
    }


    [Serializable]
    private class ScorePayload
    {
        public string playerName;
        public int fishSelectionScore;
        public int fishPrepScore;
        public int fishCheckTempScore;
        public int fishPackagingScore;
    }

    /// <summary>
    /// Fetches a player's data from MongoDB. Callback returns PlayerData if found, null if not.
    /// </summary>
    public static void GetPlayer(string playerName, Action<PlayerData> onComplete)
    {
        Instance.StartCoroutine(Instance.GetPlayerCoroutine(playerName, onComplete));
    }

    private IEnumerator GetPlayerCoroutine(string playerName, Action<PlayerData> onComplete)
    {
        string url = apiBaseUrl.TrimEnd('/') + "/api/player/" + UnityWebRequest.EscapeURL(playerName);
        Debug.Log("[MongoDBService] Fetching player: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[MongoDBService] Failed to fetch player: " + request.error);
                onComplete?.Invoke(null);
                yield break;
            }

            string json = request.downloadHandler.text;
            Debug.Log("[MongoDBService] Player response: " + json);

            PlayerResponse response = JsonUtility.FromJson<PlayerResponse>(json);

            if (response != null && response.exists && response.data != null)
            {
                onComplete?.Invoke(response.data);
            }
            else
            {
                onComplete?.Invoke(null);
            }
        }
    }

    [Serializable]
    private class PlayerResponse
    {
        public bool success;
        public bool exists;
        public PlayerData data;
    }

    [Serializable]
    public class PlayerData
    {
        public string playerName;
        public int fishSelectionScore;
        public int fishPrepScore;
        public int fishCheckTempScore;
        public int fishPackagingScore;
        public int totalScore;
    }
}
