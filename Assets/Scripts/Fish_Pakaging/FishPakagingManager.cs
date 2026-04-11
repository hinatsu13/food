using System;
using UnityEngine;
using static packagingData;

public class PakagingManager : MonoBehaviour
{
    [Header("Where to display the menu")]
    [SerializeField] public GameObject menuObject;
    [Header("Animator")]
    [SerializeField] public Animator animator;

    [Header("Use when randomising recipe, if the randomized type of meat is flake, then only the oil in these array are compatible")]
    private int[] flakeOil = {(int)OilType.Spicy, (int)OilType.Salt, (int)OilType.Mineral, (int)OilType.Shoyu, (int)OilType.Soy};
    [Header("Use when randomising recipe, if the randomized type of meat is solid, then only the oil in these array are compatible")]
    private int[] solidOil = {(int)OilType.Soy, (int)OilType.Olive, (int)OilType.SunFlower};
    [Header("Use when displaying oil name")]
    private string[] oilName = {"None", "น้ำมันรสพริก", "น้ำเกลือ", "น้ำแร่", "ซอสโชยุ", "น้ำมันถั่วเหลือง", "น้ำมันมะกอก", "น้ำมันดอทานตะวัน"};

    [Header("Save the randomized recipe in here")]
    private PackageRecipe goalRecipe;
    [Header("Save the recipe that user input in here")]
    public PackageRecipe userRecipe;
    [Header("Use to track the current oil in the nozzle")]
    private int currentOil = 0;

    
    void Awake()
    {
        //randomized type of meat, and assigned it as a boolean

        //once gotten meat type, use the type to randomized the oil using the flakeOil or solidOil Array

        //randomized the weight as 1 2 or 3 to represent the 3 colors

        //update the menuObject to reflect the recipe
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

        //update the currentOil

        //set the oil name in the nozzle using the oilName, the position should be the same as the value in OilType
    }
    public void previousOil()
    {
        //use to cycle oil, with the left button on the nozzle


        //update the currentOil

        //set the oil name in the nozzle using the oilName, the position should be the same as the value in OilType
    }
    public void check()
    {
        //Use to link with the final button after putting in the oil

        //check if the userRecipe have the same component with the goalRecipe

        //show the ending screen
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
}
