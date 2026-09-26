using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

// Singleton REST client (UnityWebRequest) for the Node score API, which is
// MongoDB-backed. The client never talks to MongoDB directly.
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

    // Override via MongoDBService.ApiBaseUrl at startup — not the Inspector:
    // Instance always creates its own GameObject, so a scene copy is ignored.
    [SerializeField]
    private string apiBaseUrl = "http://localhost:3000";

    public static string ApiBaseUrl
    {
        get => Instance.apiBaseUrl;
        set => Instance.apiBaseUrl = value;
    }

    // JWT issued by GET /api/person/:code; required on score writes. Lives
    // only in memory — a fresh login is required after app restart or
    // token expiry (server-side default: 12h).
    private static string _token;
    public static bool HasSession => !string.IsNullOrEmpty(_token);

    // ── Send scores ────────────────────────────────────────
    public static void SendScore(int fishSelectionScore, int fishPrepScore,
        int fishCheckTempScore, int fishPackagingScore, int stageCount = 0, Action<bool> onComplete = null)
    {
        Instance.StartCoroutine(Instance.SendScoreCoroutine(
            fishSelectionScore, fishPrepScore, fishCheckTempScore, fishPackagingScore, stageCount, onComplete));
    }

    /// <summary>
    /// Parameterless overload — reads all values from StateManager.
    /// </summary>
    public static void SendScore(Action<bool> onComplete = null)
    {
        SendScore(
            StateManager.getFishSelection(),
            StateManager.getFishPrep(),
            StateManager.getFishCheckTemp(),
            StateManager.getFishPackaging(),
            StateManager.getStageCount(),
            onComplete
        );
    }

    private IEnumerator SendScoreCoroutine(int fishSelectionScore, int fishPrepScore,
        int fishCheckTempScore, int fishPackagingScore, int stageCount, Action<bool> onComplete)
    {
        if (string.IsNullOrEmpty(_token))
        {
            Debug.LogError("[MongoDBService] No auth token — log in first");
            onComplete?.Invoke(false);
            yield break;
        }

        ScorePayload payload = new ScorePayload
        {
            fishSelectionScore = fishSelectionScore,
            fishPrepScore = fishPrepScore,
            fishCheckTempScore = fishCheckTempScore,
            fishPackagingScore = fishPackagingScore,
            stageCount = stageCount
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
            request.SetRequestHeader("Authorization", "Bearer " + _token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[MongoDBService] Score saved: {request.downloadHandler.text}");
                onComplete?.Invoke(true);
            }
            else
            {
                // 401 means the server rejected the token (missing/expired). The
                // caller should treat this as "session expired, log in again".
                if (request.responseCode == 401)
                {
                    Debug.LogError("[MongoDBService] Session expired — token rejected");
                    _token = null;
                }
                else
                {
                    Debug.LogError($"[MongoDBService] Failed to save score: {request.error}");
                    Debug.LogError($"[MongoDBService] Response: {request.downloadHandler.text}");
                }
                onComplete?.Invoke(false);
            }
        }
    }

    [Serializable]
    private class ScorePayload
    {
        public int fishSelectionScore;
        public int fishPrepScore;
        public int fishCheckTempScore;
        public int fishPackagingScore;
        public int stageCount;
    }

    // ── Employee ID rule (must match server.js) ────────────
    private static readonly Regex EmployeeIdPattern = new Regex("^[A-Z0-9-]{1,50}$");

    /// <summary>
    /// Trims and uppercases an Employee ID so "ab12" and "AB12" are the same
    /// player. Returns null when the result is not ^[A-Z0-9-]{1,50}$.
    /// </summary>
    public static string NormalizeEmployeeId(string raw)
    {
        if (raw == null) return null;
        string code = raw.Trim().ToUpperInvariant();
        return EmployeeIdPattern.IsMatch(code) ? code : null;
    }

    // ── Log in + fetch game data ───────────────────────────
    /// <summary>
    /// Logs in with an Employee ID. There is no roster: any well-formed ID is
    /// accepted, and a brand-new one comes back with exists=false and zeroed
    /// gameData. Returns null for an invalid ID (HTTP 400) or a network/server
    /// failure. As a side effect, stores the JWT from the response for use on
    /// subsequent score writes.
    /// </summary>
    public static void GetPerson(string personCode, Action<PersonResponse> onComplete)
    {
        Instance.StartCoroutine(Instance.GetPersonCoroutine(personCode, onComplete));
    }

    private IEnumerator GetPersonCoroutine(string personCode, Action<PersonResponse> onComplete)
    {
        string url = apiBaseUrl.TrimEnd('/') + "/api/person/" + UnityWebRequest.EscapeURL(personCode);
        Debug.Log("[MongoDBService] Fetching person: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            string json = request.downloadHandler != null ? request.downloadHandler.text : "";
            Debug.Log("[MongoDBService] Person response: " + json);

            if (request.result != UnityWebRequest.Result.Success)
            {
                // 400 is the server's "malformed Employee ID" rejection; a
                // normal outcome of user input, not an error.
                if (request.responseCode == 400)
                {
                    Debug.Log("[MongoDBService] Invalid Employee ID");
                }
                else
                {
                    Debug.LogError("[MongoDBService] Failed to fetch person: " + request.error);
                }
                onComplete?.Invoke(null);
                yield break;
            }

            PersonResponse response = JsonUtility.FromJson<PersonResponse>(json);

            // success, not exists: a brand-new ID (exists=false) is a valid login.
            if (response != null && response.success)
            {
                _token = response.token;
                onComplete?.Invoke(response);
            }
            else
            {
                onComplete?.Invoke(null);
            }
        }
    }

    [Serializable]
    public class PersonResponse
    {
        public bool success;
        public bool exists;
        public PersonInfo person;
        public GameData gameData;
        public string token;
    }

    [Serializable]
    public class PersonInfo
    {
        // The normalized Employee ID — the only identity; there are no names.
        public string personCode;
    }

    [Serializable]
    public class GameData
    {
        public int fishSelectionScore;
        public int fishPrepScore;
        public int fishCheckTempScore;
        public int fishPackagingScore;
        public int stageCount;
        public int totalScore;
    }
}
