using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class infomation : MonoBehaviour
{
    public TMP_Text fish_name;
    public UnityEngine.UI.Image fish_anime;
    public TMP_Text fish_descrip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //info.text = "what the fox say";
    }

    public void ShowInfomation_fish1()
    {
        fish_name.text = "fish1";
        fish_anime.color = Color.blue;
        fish_descrip.text = "info about fish1";
    }

    public void ShowInfomation_fish2()
    {
        fish_name.text = "fish2";
        fish_anime.color = Color.pink;
        fish_descrip.text = "info about fish2";
    }

    public void ShowInfomation_fish3()
    {
        fish_name.text = "fish3";
        fish_anime.color = Color.green;
        fish_descrip.text = "info about fish3";
    }

    public void ShowInfomation_fish4()
    {
        fish_name.text = "fish4";
        fish_anime.color = Color.yellow;
        fish_descrip.text = "info about fish4";
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
