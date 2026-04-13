using UnityEngine;
using UnityEngine.UI;

public class StarDisplay : MonoBehaviour
{
    [Header("Assets")]
    [Tooltip("Assign star sprite coresponding the index to amount of stars")]
    public Sprite[] starSprite;

    public void displayStar(int amount)
    {
        GetComponent<Image>().sprite = starSprite[amount];
    }
}
