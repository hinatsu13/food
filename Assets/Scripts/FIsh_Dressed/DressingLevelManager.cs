using UnityEngine;
using UnityEngine.UI;

public class DressingLevelManager : MonoBehaviour
{
    [Header("Character Images")]
    public Image gloveRenderer;
    public Image outfitRenderer;

    [Header("UI Controls")]
    public Button confirmButton;

    [Header("Overlays")]
    public GameObject successOverlay;
    public GameObject failOverlay;

    [Header("Correct dress")]
    [SerializeField] private int correct_set;

    private ClothingItemData currentGlove;
    private ClothingItemData currentOutfit;

    void Start()
    {
        InitializeGame();
    }

    // ==========================================
    // Core Game Flow Functions (ฟังก์ชันหลัก)
    // ==========================================

    // ฟังก์ชันนี้ถูกเรียกจากปุ่มเลือกชุด (ItemButton)
    public void SelectItem(ClothingItemData data)
    {
        ApplyItemData(data);
        UpdateConfirmButtonState();
    }

    // ฟังก์ชันนี้ผูกกับปุ่ม Confirm (ตรวจคำตอบ)
    public void CheckAnswer()
    {
        if (!IsFullyDressed()) return;

        if (currentGlove.setID == correct_set && currentOutfit.setID == correct_set)
        {
            ShowOverlay(successOverlay);
        }
        else
        {
            ShowOverlay(failOverlay);
        }
    }

    // ฟังก์ชันนี้ผูกกับปุ่ม Retry (เริ่มใหม่)
    public void Retry()
    {
        HideAllOverlays();
        ResetClothing();
        UpdateConfirmButtonState();
    }

    // ==========================================
    // Single Responsibility Helpers (ฟังก์ชันย่อย)
    // ==========================================

    private void InitializeGame()
    {
        HideAllOverlays();
        ResetClothing();
        UpdateConfirmButtonState();
    }

    private void ApplyItemData(ClothingItemData data)
    {
        if (data.itemType == ItemType.Glove)
        {
            currentGlove = data;
            UpdateCharacterVisual(gloveRenderer, data.onCharacterSprite);
        }
        else if (data.itemType == ItemType.Outfit)
        {
            currentOutfit = data;
            UpdateCharacterVisual(outfitRenderer, data.onCharacterSprite);
        }
    }

    private void UpdateCharacterVisual(Image rendererImage, Sprite newSprite)
    {
        rendererImage.sprite = newSprite;
        rendererImage.gameObject.SetActive(true);
        
        // ป้องกันปัญหารูปโปร่งใส
        var tempColor = rendererImage.color;
        tempColor.a = 1f;
        rendererImage.color = tempColor;
    }

    private void ResetClothing()
    {
        // 1. ลบข้อมูลที่จำไว้
        currentGlove = null;
        currentOutfit = null;

        // 2. ซ่อนรูปบนตัวละคร
        if (gloveRenderer != null) gloveRenderer.gameObject.SetActive(false);
        if (outfitRenderer != null) outfitRenderer.gameObject.SetActive(false);
    }

    private bool IsFullyDressed()
    {
        return currentGlove != null && currentOutfit != null;
    }

    private void UpdateConfirmButtonState()
    {
        if (confirmButton != null)
        {
            confirmButton.interactable = IsFullyDressed();
        }
    }

    private void ShowOverlay(GameObject overlay)
    {
        if (overlay != null) overlay.SetActive(true);
    }

    private void HideAllOverlays()
    {
        if (successOverlay != null) successOverlay.SetActive(false);
        if (failOverlay != null) failOverlay.SetActive(false);
    }
}