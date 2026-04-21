using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarDisplay : MonoBehaviour
{
    [Header("Assets")]
    [Tooltip("Assign star sprite coresponding the index to amount of stars")]
    public Sprite[] starSprite;
    public TextMeshProUGUI scoreText;

    public void displayStar(int amount)
    {
        GetComponent<Image>().sprite = starSprite[amount];
    }
    public void displayStar(int starAmount, int scoreAmount)
    {
        GetComponent<Image>().sprite = starSprite[starAmount];
        if (scoreText != null)
        {
            scoreText.text = scoreAmount.ToString();
        }
    }
}
