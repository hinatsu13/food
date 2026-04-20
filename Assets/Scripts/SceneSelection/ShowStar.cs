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

    private void Awake()
    {

		if(gStage == gameStage.FishSelection)
        {
            starCount = StateManager.GetStarValue(StateManager.getFishSelection(), winCondition[2], winCondition[1], winCondition[0]);
        }
        else if (gStage == gameStage.FishPreperation)
        {
            starCount = StateManager.GetStarValue(StateManager.getFishPrep(), winCondition[2], winCondition[1], winCondition[0]);
        }
        else if (gStage == gameStage.FishTemperature)
        {
            starCount = StateManager.GetStarValue(StateManager.getFishCheckTemp(), winCondition[2], winCondition[1], winCondition[0]);
        }
        else if (gStage == gameStage.FishPackaging)
        {
            starCount = StateManager.GetStarValue(StateManager.getFishPackaging(), winCondition[2], winCondition[1], winCondition[0]);
        }

        for (int i = 0; i < starCount; i++)
        {
            star[i].gameObject.SetActive(true);
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
