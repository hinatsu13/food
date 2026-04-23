using System;
using UnityEngine;

public static class StateManager
{
    private static string PlayerName;
    private static int FishSelectionScore = 0;
    private static int FishPrepScore = 0;
    private static int FishCheckTempScore = 0;
    private static int FishPackagingScore = 0;

    // ── Setters ────────────────────────────────────────────
    public static void setPlayerName(string name)
    {
        PlayerName = name;
    }

    public static void setFishSelection(int score)
    {
        FishSelectionScore = score;
    }
    
    public static void setFishPrep(int score)
    {
        FishPrepScore = score;
    }

    public static void setFishCheckTemp(int score)
    {
        FishCheckTempScore = score;
    }

    public static void setFishPackaging(int score)
    {
        FishPackagingScore = score;
    }

    // ── Getters ────────────────────────────────────────────
    public static string getPlayerName() => PlayerName;
    public static int getFishSelection() => FishSelectionScore;
    public static int getFishPrep() => FishPrepScore;
    public static int getFishCheckTemp() => FishCheckTempScore;
    public static int getFishPackaging() => FishPackagingScore;
    public static int getTotalScore() => FishSelectionScore + FishPrepScore + FishCheckTempScore + FishPackagingScore;

    // ── Send Scores to MongoDB ─────────────────────────────
    public static void SendPacket(Action<bool> onComplete = null)
    {
        Loading.Show();
        Debug.Log("=== Sending Scores to MongoDB ===");
        Debug.Log("Player Name: " + PlayerName);
        Debug.Log("Fish Selection Score: " + FishSelectionScore);
        Debug.Log("Fish Prep Score: " + FishPrepScore);
        Debug.Log("Fish Check Temp Score: " + FishCheckTempScore);
        Debug.Log("Fish Packaging Score: " + FishPackagingScore);
        Debug.Log("Total Score: " + getTotalScore());

        MongoDBService.SendScore(
            PlayerName,
            FishSelectionScore,
            FishPrepScore,
            FishCheckTempScore,
            FishPackagingScore,
            onComplete
        );
        Loading.Hide();
    }

    public static int GetStarValue(int inputValue, int threeStarCond, int twoStarCond, int oneStarCond)
    {
        if (inputValue > threeStarCond) return 3;
        if (inputValue > twoStarCond) return 2;
        if (inputValue > oneStarCond) return 1;

        return 0;
    }
    public static int GetStarValue(int inputValue)
    {
        //condition are there to make sure the input value are 0 1 2 or 3
        if (inputValue > 3) return 3;
        if (inputValue > 2) return 2;
        if (inputValue > 1) return 1;

        return 0;
    }
}
