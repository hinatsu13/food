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
        Loading.Show();

        string currentPlayerCode = StateManager.getPlayerCode();
        string url = MSSqlService.ApiBaseUrl.TrimEnd('/') + "/api/scores";

        // Include personCode so the API returns this player's exact rank
        if (!string.IsNullOrEmpty(currentPlayerCode))
            url += "?personCode=" + UnityWebRequest.EscapeURL(currentPlayerCode);

        Debug.Log("[LeaderBoard] Fetching: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[LeaderBoard] Failed: " + request.error);
                Loading.Hide();
                yield break;
            }

            string json = request.downloadHandler.text;
            Debug.Log("[LeaderBoard] Response: " + json);

            LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(json);
            if (response == null || response.data == null)
            {
                Debug.LogError("[LeaderBoard] Failed to parse response");
                Loading.Hide();
                yield break;
            }

            PopulateLeaderboard(response);
            Loading.Hide();
        }
    }

    private void PopulateLeaderboard(LeaderboardResponse response)
    {
        ScoreEntry[] scores = response.data;

        // ── Fill Pedestal Top 3 ────────────────────────────
        if (scores.Length > 0 && firstPlaceName != null)
            firstPlaceName.text = BuildDisplayName(scores[0].nickName, scores[0].fnameE, scores[0].lnameE, scores[0].personCode);
        if (scores.Length > 1 && secondPlaceName != null)
            secondPlaceName.text = BuildDisplayName(scores[1].nickName, scores[1].fnameE, scores[1].lnameE, scores[1].personCode);
        if (scores.Length > 2 && thirdPlaceName != null)
            thirdPlaceName.text = BuildDisplayName(scores[2].nickName, scores[2].fnameE, scores[2].lnameE, scores[2].personCode);

        // ── Fill Player Rank Row (from server-computed rank) ──
        if (response.player != null && !string.IsNullOrEmpty(response.player.personCode))
        {
            if (playerRankName != null)
                playerRankName.text = BuildDisplayName(response.player.nickName, response.player.fnameE, response.player.lnameE, response.player.personCode);
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
        if (response.nextRank != null && !string.IsNullOrEmpty(response.nextRank.personCode))
        {
            if (nextRankName != null)
                nextRankName.text = BuildDisplayName(response.nextRank.nickName, response.nextRank.fnameE, response.nextRank.lnameE, response.nextRank.personCode);
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

    // Same display-name preference used by EnterNameManager: NickName → "FnameE LnameE" → PersonCode.
    private static string BuildDisplayName(string nickName, string fnameE, string lnameE, string personCode)
    {
        if (!string.IsNullOrEmpty(nickName)) return nickName;
        string combined = ((fnameE ?? "") + " " + (lnameE ?? "")).Trim();
        if (!string.IsNullOrEmpty(combined)) return combined;
        return personCode ?? "---";
    }

    // ── JSON Data Classes ──────────────────────────────────
    [Serializable]
    private class LeaderboardResponse
    {
        public bool success;
        public ScoreEntry[] data;
        public RankInfo player;
        public RankInfo nextRank;
    }

    [Serializable]
    private class ScoreEntry
    {
        public string personCode;
        public int totalScore;
        public string lastUpdated;
        public string fnameE;
        public string lnameE;
        public string nickName;
    }

    [Serializable]
    private class RankInfo
    {
        public int rank;
        public string personCode;
        public int totalScore;
        public string fnameE;
        public string lnameE;
        public string nickName;
    }
}
