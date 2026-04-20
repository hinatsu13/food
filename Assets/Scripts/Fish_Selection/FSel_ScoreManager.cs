using TMPro;
using UnityEngine;

public static class FSel_ScoreManager 
{
    public static FSel_Fish activeFish;
    public static int selectionScore;
    public static GameObject scoreIndicator;
    public static GameObject endScoreIndicator;
    public static FSel_InputDetector discardIndicator;
    public static FSel_SetUpEdge edge;
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

    public static void OnDiscard(bool correct)
    {
        if (correct)
        {
            selectionScore += 5;
        }
        else
        {
            selectionScore -= 5;
        }
        discardIndicator.ShowResult(correct);
        DisplayScore();
    }

    public static void OnFail()
    {
        selectionScore -= 5;
        edge.displayIndicator();
        DisplayScore();
    }
    public static void DisplayScore()
    {
        if (scoreIndicator == null) return;
        scoreIndicator.GetComponent<TextMeshProUGUI>().SetText(selectionScore.ToString());

    }
}
