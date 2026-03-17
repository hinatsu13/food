using TMPro;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class FSel_Fish : MonoBehaviour
{
    [Header("Objective Condition")]
    [SerializeField] private int Species;//to check if the chosen fish is correct, Match it with species value in FSel_Fish

    [Header("Fish Config")]
    [SerializeField] public float moveSpeed;

    private bool IsInTrigger = false;
    private bool IsInBox = false;
    private bool IsInDiscard = false;
    private Vector3 lastPosition;
    private FSel_InputDetector hovered_storage;

    public UnityEvent<GameObject> OnSorted;
    public UnityEvent<GameObject> OnCorrect;
    public UnityEvent<GameObject> OnIncorrect;
    public UnityEvent OnFailed;
    public UnityEvent<bool> OnDiscard;
    public UnityEvent OnDestroyed;

    [SerializeField] private LayerMask FishLayer;
    private bool isQuitting = false;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Storage"))
        {
            hovered_storage = collision.GetComponent<FSel_InputDetector>();
            IsInBox = true;
        }
        if (collision.CompareTag("Discard"))
        {
            IsInDiscard = true;
        }
    }
    void OnApplicationQuit()
    {
        isQuitting = true;
    }
    private void OnDestroy()
    {
        if (isQuitting) return;
        OnDestroyed?.Invoke();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Storage"))
        {
            hovered_storage = null;
            IsInBox = false;
        }
        if (collision.CompareTag("Edge"))
        {
            if(hovered_storage == null && !IsInDiscard)
            {
                OnFailed?.Invoke();
                Destroy(gameObject);
            }
        }
        if (collision.CompareTag("Discard"))
        {
            IsInDiscard = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsInTrigger)
        {
            transform.position = transform.position - Vector3.right * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = Input.mousePosition;
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                if (IsPositionInsideTrigger(touch.position))
                {
                    lastPosition = transform.position;
                    IsInTrigger = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended && IsInTrigger)
            {
                //for when player release finger
                IsInTrigger = false;
                //Check for box
                if (IsInBox)
                {
                    CheckSort();
                }
                else
                {
                    transform.position = lastPosition;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (IsPositionInsideTrigger(Input.mousePosition))
            {
                lastPosition = transform.position;
                IsInTrigger = true;
            }
        }
        else if (Input.GetMouseButtonUp(0) && IsInTrigger)
        {
            //for when player release mouse
            IsInTrigger = false;
            //Check for box

                CheckSort();
        }
    }
    private bool IsPositionInsideTrigger(Vector2 screenPosition)
    {
        Collider2D hitcollider = Physics2D.OverlapPoint(screenPosition, FishLayer);
        //Debug.Log(hitcollider + $"{hitcollider == gameObject.GetComponent<Collider2D>()}");
        return hitcollider == gameObject.GetComponent<Collider2D>();
    }
    private void CheckSort()
    {
        if (IsInBox)
        {
            if (hovered_storage != null)
            {
                OnSorted.Invoke(hovered_storage.gameObject);
                if (hovered_storage.Species == Species)
                {
                    Debug.Log("Correct sorting");
                    OnCorrect.Invoke(hovered_storage.gameObject);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Incorrect sorting");
                    OnIncorrect.Invoke(hovered_storage.gameObject);
                    Destroy(gameObject);
                }
            }
        }else if (IsInDiscard)
        {
            if(Species < 0)
            {
                OnDiscard?.Invoke(true);
            }
            else
            {
                OnDiscard?.Invoke(false);
            }            
            Destroy(gameObject);
        }
        else
        {
            transform.position = lastPosition;
        }
    }



    public void endGame()
    {
        Debug.Log("Ending Fish");
        isQuitting = true;
        Destroy(gameObject);
    }
}
