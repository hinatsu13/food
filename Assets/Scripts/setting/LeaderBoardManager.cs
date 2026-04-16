using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("Pedestal Top 3 Names")]
    [SerializeField] private TextMeshProUGUI firstPlaceName;
    [SerializeField] private TextMeshProUGUI secondPlaceName;
    [SerializeField] private TextMeshProUGUI thirdPlaceName;

    [Header("Player Rank Row")]
    [SerializeField] private TextMeshProUGUI playerRankName;
    [SerializeField] private TextMeshProUGUI playerRankScore;

    [Header("Next Rank Row")]
    [SerializeField] private TextMeshProUGUI nextRankName;
    [SerializeField] private TextMeshProUGUI nextRankScore;



    private void Start()
    {
        StartCoroutine(FetchLeaderboard());
    }

    private IEnumerator FetchLeaderboard()
    {
        string currentPlayer = StateManager.getPlayerName();
        string url = MongoDBService.ApiBaseUrl.TrimEnd('/') + "/api/scores";

        // Include player name so the API returns their exact rank
        if (!string.IsNullOrEmpty(currentPlayer))
            url += "?playerName=" + UnityWebRequest.EscapeURL(currentPlayer);

        Debug.Log("[LeaderBoard] Fetching: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[LeaderBoard] Failed: " + request.error);
                yield break;
            }

            string json = request.downloadHandler.text;
            Debug.Log("[LeaderBoard] Response: " + json);

            LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(json);
            if (response == null || response.data == null)
            {
                Debug.LogError("[LeaderBoard] Failed to parse response");
                yield break;
            }

            PopulateLeaderboard(response);
        }
    }

    private void PopulateLeaderboard(LeaderboardResponse response)
    {
        ScoreEntry[] scores = response.data;

        // ── Fill Pedestal Top 3 ────────────────────────────
        if (scores.Length > 0 && firstPlaceName != null)
            firstPlaceName.text = scores[0].playerName;
        if (scores.Length > 1 && secondPlaceName != null)
            secondPlaceName.text = scores[1].playerName;
        if (scores.Length > 2 && thirdPlaceName != null)
            thirdPlaceName.text = scores[2].playerName;

        // ── Fill Player Rank Row (from server-computed rank) ──
        if (response.player != null && !string.IsNullOrEmpty(response.player.playerName))
        {
            if (playerRankName != null)
                playerRankName.text = response.player.playerName;
            if (playerRankScore != null)
                playerRankScore.text = response.player.totalScore.ToString();
        }
        else
        {
            // Player not in DB yet — show current session data
            if (playerRankName != null)
                playerRankName.text = StateManager.getPlayerName() ?? "---";
            if (playerRankScore != null)
                playerRankScore.text = StateManager.getTotalScore().ToString();
        }

        // ── Fill Next Rank Row (person above the player) ──
        if (response.nextRank != null && !string.IsNullOrEmpty(response.nextRank.playerName))
        {
            if (nextRankName != null)
                nextRankName.text = response.nextRank.playerName;
            if (nextRankScore != null)
                nextRankScore.text = response.nextRank.totalScore.ToString();
        }
        else
        {
            // Player is #1 or not ranked
            if (nextRankName != null)
                nextRankName.text = "---";
            if (nextRankScore != null)
                nextRankScore.text = "---";
        }
    }

    // ── JSON Data Classes ──────────────────────────────────
    [Serializable]
    private class LeaderboardResponse
    {
        public bool success;
        public ScoreEntry[] data;
        public int totalPlayers;
        public RankInfo player;
        public RankInfo nextRank;
    }

    [Serializable]
    private class ScoreEntry
    {
        public string _id;
        public string playerName;
        public int fishSelectionScore;
        public int fishPrepScore;
        public int fishCheckTempScore;
        public int fishPackagingScore;
        public int totalScore;
        public string createdAt;
    }

    [Serializable]
    private class RankInfo
    {
        public int rank;
        public string playerName;
        public int totalScore;
    }
}
