using TMPro;
using UnityEngine;

public static class FSel_ScoreManager 
{
    public static FSel_Fish activeFish;
    public static int selectionScore;
    public static GameObject scoreIndicator;
    public static FSel_Timer timer;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="go">gameobject for displaying score</param>
    public static void OnCorrect(GameObject go)
    {
        selectionScore += 10;
        go.GetComponent<FSel_InputDetector>().ShowResult(true);
        DisplayScore();
    }
    public static void OnIncorrect(GameObject go)
    {
        selectionScore -= 10;
        go.GetComponent<FSel_InputDetector>().ShowResult(false);
        DisplayScore();
    }
    public static void DisplayScore()
    {
        if (scoreIndicator == null) return;
        scoreIndicator.GetComponent<TextMeshProUGUI>().SetText(selectionScore.ToString());

    }
}
