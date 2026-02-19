using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public ClothingItemData itemData; // ลากไฟล์ Data มาใส่ใน Inspector
    public Image buttonIconDisplay;
    
    private DressingLevelManager manager;

    void Start()
    {
        // Setup รูปภาพปุ่มอัตโนมัติ
        if(itemData != null && buttonIconDisplay != null)
        {
            buttonIconDisplay.sprite = itemData.iconSprite;
        }
        
        // หา Manager ในฉาก (ควรมีตัวเดียว)
        manager = FindFirstObjectByType<DressingLevelManager>();
    }

    public void OnClick()
    {
        if (manager != null)
        {
            manager.SelectItem(itemData);
        }
    }
}