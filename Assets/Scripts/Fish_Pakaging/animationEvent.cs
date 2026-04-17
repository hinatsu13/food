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
            pakagingManager.showProduct();
            GetComponent<Animator>().SetTrigger("doneOiling");
        }
        return;
    }
}
