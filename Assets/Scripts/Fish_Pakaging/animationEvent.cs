using UnityEngine;

public class animationEvent : MonoBehaviour
{
    public bool isFinal = false;
    public void finalButton()
    {
        if (!isFinal)
        {
            isFinal = true;
            GetComponent<Animator>().SetTrigger("doneOiling");
        }
        return;
    }
}
