using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static packagingData;
using Image = UnityEngine.UI.Image;

public class PakagingManager : MonoBehaviour
{
    [Header("Where to display the menu")]
    [SerializeField] public GameObject menuObject;
    [SerializeField] public GameObject flackCheck;
    [SerializeField] public GameObject solidCheck;
    [SerializeField] public TextMeshProUGUI weight_Value;
    [SerializeField] public TextMeshProUGUI oilText;
    [Header("Sprite List")]
    [SerializeField] private Image can_Sprite;
    [Header("Weight Display")]
    [SerializeField] public TextMeshProUGUI scale_Value;
    [SerializeField] public GameObject Light;
    [SerializeField] public GameObject Med;
    [SerializeField] public GameObject Heavy;

    [Header("Nozzle Text")]
    [SerializeField] public TextMeshProUGUI Nozzle_Text;

    [Header("Animator")]
    [SerializeField] public Animator animator;

    [Header("Star Displayer")]
    [SerializeField] public StarDisplay endScreen;
    [Header("Final Sprite Reference")]
    [SerializeField] public Image fishCan;
    [SerializeField] public Sprite SunflowerSteak;
    [SerializeField] public Sprite SoySteak;
    [SerializeField] public Sprite OliveSteak;
    [SerializeField] public Sprite MineralFlakes;
    [SerializeField] public Sprite SoyFlakes;
    [SerializeField] public Sprite SaltFlakes;
    [SerializeField] public Sprite ShoyuFlakes;
    [SerializeField] public Sprite SpicyFlakes;

    [Tooltip("Use when randomising recipe, if the randomized type of meat is flake, then only the oil in these array are compatible")]
    private int[] flakeOil = {(int)OilType.Spicy, (int)OilType.Salt, (int)OilType.Mineral, (int)OilType.Shoyu, (int)OilType.Soy};
    [Tooltip("Use when randomising recipe, if the randomized type of meat is solid, then only the oil in these array are compatible")]
    private int[] solidOil = {(int)OilType.Soy, (int)OilType.Olive, (int)OilType.SunFlower};
    [Tooltip("Use when displaying oil name")]
    [Header("Oil Name")]
    [SerializeField] private string[] oilName = {"None", "Spicy", "Salt", "Mineral", "Shoyu", "Soy Oil", "Olive Oil", "Sunflower Oil"};

    [Tooltip("Save the randomized recipe in here")]
    [SerializeField] private PackageRecipe goalRecipe;
    [Tooltip("Save the recipe that user input in here")]
    [SerializeField] public PackageRecipe userRecipe;
    [Tooltip("Use to track the current oil in the nozzle")]
    //Mock Weighted Value
    private int currentOil = 0;
    private int LightValue;
    private int MedValue;
    private int HighValue;
    private int score = 0;


    void Awake()
    {
    
        //randomized type of meat, and assigned it as a boolean
        bool isFlake = UnityEngine.Random.Range(0,2) == 1;
        int randomIndex = -1;
        int randomOil = -1;
        //once gotten meat type, use the type to randomized the oil using the flakeOil or solidOil Array
        if (isFlake)
        {
            randomIndex = UnityEngine.Random.Range(0,flakeOil.Count());
            randomOil = flakeOil[randomIndex];
        }
        else
        {
            randomIndex = UnityEngine.Random.Range(0,solidOil.Count());
            randomOil = solidOil[randomIndex];
        }
        goalRecipe = new PackageRecipe();
        //randomized the weight as 1 2 or 3 to represent the 3 colors
        int currentWeight = UnityEngine.Random.Range(1,4);
        //update the menuObject to reflect the recipe
        
        //Random mock weight
        LightValue = UnityEngine.Random.Range(9,41);
        MedValue = UnityEngine.Random.Range(40,91);
        HighValue = UnityEngine.Random.Range(99,151);
        
        if(randomOil != -1)
        {
            goalRecipe.setRecipe(isFlake, currentWeight, randomOil);
            if (isFlake)
            {
                flackCheck.SetActive(true);
            }
            else
            {
                solidCheck.SetActive(true);
            }
            if (currentWeight == 1)
            {
                Light.SetActive(true);
                weight_Value.text = LightValue.ToString();
            }else if (currentWeight == 2)
            {
                Med.SetActive(true);
                weight_Value.text = MedValue.ToString();
            }else if (currentWeight == 3)
            {
                Heavy.SetActive(true);
                weight_Value.text = HighValue.ToString();
            }
            oilText.text = oilName[randomOil];
            GoalProduct();
        }
    }

    public void SelectMeat(bool isFlake)
    {
        //Use to link onclick button when choosing meat type
        //assign the value into userRecipe and the isFlake parameter in animator
        userRecipe.isFlake = isFlake;
        animator.SetBool("isFlake", isFlake);
        //call the animator trigger to update the sprite
        animator.SetTrigger("AddFish");
        //set the button to continue to active

        //checking input with recipe
        if (goalRecipe.isFlake == userRecipe.isFlake)
        {
            score++;
            StateManager.setFishPackaging(score);
        }
    }
    public void PutOnScale()
    {
        //Use to link onclick button that appear after selecting meat
        //Trigger "PutOnScale" on the animator to set the scene
        animator.SetTrigger("PutOnScale");
        //disable the button, do it here or do it on the button onclick event to set itself to be inactive
    }
    public void SelectWeight(int weight)
    {
        //When clicking/tapping the screen when selecting weight
        //Assign value to userRecipe
        userRecipe.weight = weight;
        if (weight == 1)
        {
            Light.SetActive(true);
            scale_Value.text = LightValue.ToString();
        }else if (weight == 2)
        {
            Med.SetActive(true);
            scale_Value.text = MedValue.ToString();
        }else if (weight == 3)
        {
            Heavy.SetActive(true);
            scale_Value.text = HighValue.ToString();
        }
        if (goalRecipe.weight == userRecipe.weight)
        {
            score++;
            StateManager.setFishPackaging(score);
        }
        //set the button to continue to active
    }
    public void PutOffScale()
    {
        //Use to link onclick button that appear after selecting Weight
        //Trigger "RemoveScale" on the animator to set the scene
        animator.SetTrigger("RemoveScale");
        //disable the button, do it here or do it on the button onclick event to set itself to be inactive
    }
    public void addOil()
    {
        //use to link to the center bottom button that will insert the oil into the can
        //set the oil int in the userRecipe
        userRecipe.oilType = currentOil;
        //set the OilType in the animator to the currentOil then trigger the "addingOil" trigger,
        //if this broke check the condition in the animator that thing keep unassigning condition
        animator.SetInteger("OilType", currentOil);
        animator.SetTrigger("AddingOil");
    }
    public void nextOil()
    {
        //use to cycle oil, with the right button on the nozzle
        currentOil++;
        Nozzle_Text.text = oilName[currentOil % 8];
        
        //set the oil name in the nozzle using the oilName, the position should be the same as the value in OilType
    }
    public void previousOil()
    {
        //use to cycle oil, with the left button on the nozzle
        if(currentOil == 0)
        {
            currentOil = 7;
        }
        else
        {
            currentOil--;
        }
        Nozzle_Text.text = oilName[Mathf.Abs(currentOil % 8)];

        //set the oil name in the nozzle using the oilName, the position should be the same as the value in OilType
    }
    public void check()
    {
        //show the ending screen
        if (goalRecipe.oilType == userRecipe.oilType)
        {
            score++;
            if (score == 3)
            {
                animator.SetBool("Correct", true);
            }
            StateManager.setFishPackaging(score);
        }
        endScreen.displayStar(score);
        StateManager.setFishPackaging(score);
    }
    public void showProduct()
    {
        if (userRecipe.isFlake)
        {
            switch (userRecipe.oilType)
            {
                case 1:
                    fishCan.sprite = SpicyFlakes;
                    break; 
                case 2:
                    fishCan.sprite = SaltFlakes;
                    break;
                case 3:
                    fishCan.sprite = MineralFlakes;
                    break;
                case 4:
                    fishCan.sprite = ShoyuFlakes;
                    break;
                case 5:
                    fishCan.sprite = SoyFlakes;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (userRecipe.oilType)
            {
                case 6:
                    fishCan.sprite = OliveSteak;
                    break;
                case 7:
                    fishCan.sprite = SunflowerSteak;
                    break;
                case 5:
                    fishCan.sprite = SoySteak;
                    break;
                default:
                    break;
            }
        }
    }
    public void GoalProduct()
    {
        if (goalRecipe.isFlake)
        {
            switch (goalRecipe.oilType)
            {
                case 1:
                    can_Sprite.sprite = SpicyFlakes;
                    break; 
                case 2:
                    can_Sprite.sprite = SaltFlakes;
                    break;
                case 3:
                    can_Sprite.sprite = MineralFlakes;
                    break;
                case 4:
                    can_Sprite.sprite = ShoyuFlakes;
                    break;
                case 5:
                    can_Sprite.sprite = SoyFlakes;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (goalRecipe.oilType)
            {
                case 6:
                    can_Sprite.sprite = OliveSteak;
                    break;
                case 7:
                    can_Sprite.sprite = SunflowerSteak;
                    break;
                case 5:
                    can_Sprite.sprite = SoySteak;
                    break;
                default:
                    break;
            }
        }
    }
}
public static class packagingData
{
    public enum OilType
    {
        Spicy = 1,
        Salt = 2,
        Mineral = 3,
        Shoyu = 4,
        Soy = 5,
        Olive = 6,
        SunFlower = 7,
    }
}

public struct PackageRecipe
{
    public bool isFlake;
    public int weight;
    public int oilType;
    public void setRecipe(bool flake, int set_weight, int set_oilType)
    {
        isFlake = flake;
        weight = set_weight;
        oilType = set_oilType;
    }
}
