using UnityEngine;

public class CollapsRecipe : MonoBehaviour
{
    [Header("MenuObject for showing menu to player.")]
    [SerializeField] private GameObject menuSheet;
    [Header("Should the menu be shown at the start of the game.")]
    [SerializeField] bool isShown = true;
    private void Start()
    {
        menuSheet.SetActive(isShown);
    }
    public void toggleMenu()
    {
        isShown = !isShown;
        menuSheet.SetActive(isShown);
    }
}
