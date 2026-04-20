using UnityEngine;

public class animationEvent : MonoBehaviour
{
    public bool isFinal = false;
    public PakagingManager pakagingManager;

    public void finalButton()
    {
        if (!isFinal)
        {
            isFinal = true;
            GetComponent<Animator>().SetTrigger("doneOiling");
        }
        return;
    }
    public void assignLable()
    {
        pakagingManager.showProduct();
    }
}
