using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FSel_Belt : MonoBehaviour
{
    [Header("Belt Scroll Settings")]
    [Tooltip("Scroll speed in pixels per second.")]
    [SerializeField] private float scrollSpeed = 200f;

    private RectTransform _rectA;
    private RectTransform _rectB;
    private float _beltWidth;
    private float _startX;
    private float _offset;
    private bool _ready;

    IEnumerator Start()
    {
        // Wait one frame for Canvas to finish its layout pass
        yield return null;

        _rectA = GetComponent<RectTransform>();
        _beltWidth = _rectA.rect.width;
        _startX = _rectA.anchoredPosition.x;

        // Build a second belt Image manually (avoids copying FSel_Belt and double-execution)
        Image src = GetComponent<Image>();
        GameObject cloneGO = new GameObject("Belt_Loop");
        cloneGO.transform.SetParent(transform.parent, false);

        Image cloneImg    = cloneGO.AddComponent<Image>();
        cloneImg.sprite   = src.sprite;
        cloneImg.color    = src.color;
        cloneImg.material = src.material;
        cloneImg.type     = src.type;

        _rectB            = cloneGO.GetComponent<RectTransform>();
        _rectB.anchorMin  = _rectA.anchorMin;
        _rectB.anchorMax  = _rectA.anchorMax;
        _rectB.sizeDelta  = _rectA.sizeDelta;
        _rectB.pivot      = _rectA.pivot;
        _rectB.anchoredPosition = new Vector2(_startX + _beltWidth, _rectA.anchoredPosition.y);

        // Place clone behind original so draw order is correct
        cloneGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        _ready = true;
    }

    void Update()
    {
        if (!_ready) return;

        // Accumulate offset and wrap within one belt width for a clean loop
        _offset = (_offset + scrollSpeed * Time.deltaTime) % _beltWidth;

        _rectA.anchoredPosition = new Vector2(_startX - _offset,              _rectA.anchoredPosition.y);
        _rectB.anchoredPosition = new Vector2(_startX + _beltWidth - _offset, _rectB.anchoredPosition.y);
    }
}
