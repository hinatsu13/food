using UnityEngine;

namespace Fish_Thaw
{
    public class EndSceneBTN : MonoBehaviour
    {
        [SerializeField] private GameObject panel1;
        [SerializeField] private GameObject panel2;
        [SerializeField] private GameObject panel3;
        [SerializeField] private GameObject targetObj;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            targetObj.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (panel1.activeSelf && panel2.activeSelf && panel3.activeSelf)
            {
                targetObj.SetActive(true);
            }
            else
            {
                targetObj.SetActive(false);
            }
        }
    }
    
}
