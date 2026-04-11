using System;
using UnityEngine;
using static packagingData;

public class PakagingManager : MonoBehaviour
{
    [Header("Where to display the menu")]
    [SerializeField] public GameObject menuObject;
    [Header("Animator")]
    [SerializeField] public Animator animator;
    private int[] flakeOil = {(int)OilType.Spicy, (int)OilType.Salt, (int)OilType.Mineral, (int)OilType.Shoyu, (int)OilType.Soy};
    private int[] solidOil = {(int)OilType.Soy, (int)OilType.Olive, (int)OilType.SunFlower};
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

struct PackageRecipe
{
    bool isFlake;
    int weight;
    OilType oilType;
}