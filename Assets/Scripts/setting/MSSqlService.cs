using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Singleton REST client for the HRIS-backed SQL Server score API.
public class MSSqlService : MonoBehaviour
{
    private static MSSqlService _instance;
    public static MSSqlService Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MSSqlService");
                _instance = go.AddComponent<MSSqlService>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // Override via the Inspector or MSSqlService.ApiBaseUrl. Local backend
    // runs on http://localhost:3000; the deployed URL below targets the
    // legacy Mongo deployment and must be repointed when prod SQL is up.
    [SerializeField]
    private string apiBaseUrl = "http://localhost:3000";

    public static string ApiBaseUrl
    {
        get => Instance.apiBaseUrl;
        set => Instance.apiBaseUrl = value;
    }

    // ── Send scores ────────────────────────────────────────
    public static void SendScore(string personCode, int fishSelectionScore, int fishPrepScore,
        int fishCheckTempScore, int fishPackagingScore, int stageCount = 0, Action<bool> onComplete = null)
    {
        Instance.StartCoroutine(Instance.SendScoreCoroutine(
            personCode, fishSelectionScore, fishPrepScore, fishCheckTempScore, fishPackagingScore, stageCount, onComplete));
    }

    /// <summary>
    /// Parameterless overload — reads all values from StateManager.
    /// </summary>
    public static void SendScore(Action<bool> onComplete = null)
    {
        SendScore(
            StateManager.getPlayerCode(),
            StateManager.getFishSelection(),
            StateManager.getFishPrep(),
            StateManager.getFishCheckTemp(),
            StateManager.getFishPackaging(),
            StateManager.getStageCount(),
            onComplete
        );
    }

    private IEnumerator SendScoreCoroutine(string personCode, int fishSelectionScore, int fishPrepScore,
        int fishCheckTempScore, int fishPackagingScore, int stageCount, Action<bool> onComplete)
    {
        ScorePayload payload = new ScorePayload
        {
            personCode = personCode,
            fishSelectionScore = fishSelectionScore,
            fishPrepScore = fishPrepScore,
            fishCheckTempScore = fishCheckTempScore,
            fishPackagingScore = fishPackagingScore,
            stageCount = stageCount
        };

        string jsonData = JsonUtility.ToJson(payload);
        string url = apiBaseUrl.TrimEnd('/') + "/api/scores";

        Debug.Log($"[MSSqlService] Sending score to: {url}");
        Debug.Log($"[MSSqlService] Payload: {jsonData}");

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[MSSqlService] Score saved: {request.downloadHandler.text}");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"[MSSqlService] Failed to save score: {request.error}");
                Debug.LogError($"[MSSqlService] Response: {request.downloadHandler.text}");
                onComplete?.Invoke(false);
            }
        }
    }

    [Serializable]
    private class ScorePayload
    {
        public string personCode;
        public int fishSelectionScore;
        public int fishPrepScore;
        public int fishCheckTempScore;
        public int fishPackagingScore;
        public int stageCount;
    }

    // ── Fetch person + game data ───────────────────────────
    /// <summary>
    /// Fetches a person by PersonCode. Returns the response (with exists=true
    /// and person/gameData) on success, or null if the PersonCode is not in
    /// the HRIS roster (HTTP 404) or the request failed.
    /// </summary>
    public static void GetPerson(string personCode, Action<PersonResponse> onComplete)
    {
        Instance.StartCoroutine(Instance.GetPersonCoroutine(personCode, onComplete));
    }

    private IEnumerator GetPersonCoroutine(string personCode, Action<PersonResponse> onComplete)
    {
        string url = apiBaseUrl.TrimEnd('/') + "/api/person/" + UnityWebRequest.EscapeURL(personCode);
        Debug.Log("[MSSqlService] Fetching person: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            string json = request.downloadHandler != null ? request.downloadHandler.text : "";
            Debug.Log("[MSSqlService] Person response: " + json);

            if (request.result != UnityWebRequest.Result.Success)
            {
                // 404 is the "PersonCode not in roster" rejection path; treated
                // as a normal failed lookup, not an error to surface to the user.
                if (request.responseCode == 404)
                {
                    Debug.Log("[MSSqlService] PersonCode not in HRIS roster");
                }
                else
                {
                    Debug.LogError("[MSSqlService] Failed to fetch person: " + request.error);
                }
                onComplete?.Invoke(null);
                yield break;
            }

            PersonResponse response = JsonUtility.FromJson<PersonResponse>(json);

            if (response != null && response.exists)
            {
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
    }

    [Serializable]
    public class PersonInfo
    {
        public long personID;
        public string personCode;
        public string fnameT;
        public string lnameT;
        public string fnameE;
        public string lnameE;
        public string nickName;
        public string positionNameT;
        public string companyNameT;
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
