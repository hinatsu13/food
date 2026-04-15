using UnityEngine;

public static class StateManager
{
    public static int FishSelectionScore;
    public static int FishPrepScore;
    public static int FishPackagingScore;

    public static void setFishSelection(int score)
    {
        FishSelectionScore = score;
    }
    public static void setFishPrep(int score)
    {
        FishPrepScore = score;
    }
    public static void setFishPackaging(int score)
    {
        FishPackagingScore = score;
    }
    public static void SendPacket()
    {

    }
}
