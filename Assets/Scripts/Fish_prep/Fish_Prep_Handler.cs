using UnityEngine;
using UnityEngine.UI;

public class Fish_Prep_Handler : MonoBehaviour
{
    [SerializeField]
    private GameObject game_Panel;
    [SerializeField]
    private GameObject cutscene_Panel;
    [SerializeField]
    private GameObject fillet_Fish;
    [SerializeField]
    private GameObject guts_The_Fish;

    [SerializeField] 
    private GameObject clean_Sparkle;// Assign in Inspector
    [SerializeField]
    private GameObject Clean_Fish;
    [SerializeField]
    private GameObject show_After_Cutscene;
    [SerializeField]
    private GameObject image_Person;    

    public void SplitFish()
    {
        Debug.Log("The fish is filleted!");
        // Disable the fish
        fillet_Fish.SetActive(false);
        guts_The_Fish.SetActive(true);
    }

    public void CleanFish()
    {
        //Turn Off Old guts
        guts_The_Fish.SetActive(false);
        Clean_Fish.SetActive(true);
    }

    public void Cutscene_Trigger()
    {
        Clean_Fish.SetActive(false);
        clean_Sparkle.SetActive(true);
        Invoke(nameof(TriggerCutscene), 5.0f);
        Invoke(nameof(finish_Sniffing), 10.0f);
    }

    public void finish_Sniffing()
    {
        show_After_Cutscene.SetActive(true);
        image_Person.SetActive(false);
    }
    public void TriggerCutscene()
    {
        //Open first hide the close one
        cutscene_Panel.SetActive(true); 
        game_Panel.SetActive(false);
    }
}
