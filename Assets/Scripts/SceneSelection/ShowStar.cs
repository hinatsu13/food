using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ShowStar : MonoBehaviour
{
    public GameObject[] star;
	public gameStage gStage;
    public int starCount;
    [Tooltip("How much score corelates to how many stars are displayed, with index representing the number of stars.")]
    public int[] winCondition;

    private void Start()
    {

		if(gStage == gameStage.FishSelection)
        {
            StateManager.GetStarValue(StateManager.getFishSelection(), winCondition[2], winCondition[1], winCondition[0]);
        }
        else if (gStage == gameStage.FishPreperation)
        {
            StateManager.GetStarValue(StateManager.getFishPrep(), winCondition[2], winCondition[1], winCondition[0]);
        }
        else if (gStage == gameStage.FishTemperature)
        {
            StateManager.GetStarValue(StateManager.getFishCheckTemp(), winCondition[2], winCondition[1], winCondition[0]);
        }
        else if (gStage == gameStage.FishPackaging)
        {
            StateManager.GetStarValue(StateManager.getFishPackaging(), winCondition[2], winCondition[1], winCondition[0]);
        }

    }

    public enum gameStage
	{
		FishSelection,
		FishPreperation,
		FishTemperature,
		FishPackaging,
	}

}
