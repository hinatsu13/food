using UnityEngine;
using UnityEngine.UI;
using TMPro; // แนะนำให้ใช้ TextMeshPro เพื่อความคมชัด

public class DressingLevelManager : MonoBehaviour
{
    [Header("Character Display")]
    public Image gloveRenderer;  // Image บนตัวละครสำหรับถุงมือ
    public Image outfitRenderer; // Image บนตัวละครสำหรับชุด
    
    [Header("UI Feedback")]
    public GameObject successPopup; // หน้าต่างที่จะเด้งเมื่อถูก
    public TextMeshProUGUI infoText; // ข้อความข้อมูลชุด
    public Button nextLevelButton;  // ปุ่มไปด่านต่อไป

    // State ปัจจุบัน
    private ClothingItemData currentGlove;
    private ClothingItemData currentOutfit;

    void Start()
    {
        // ซ่อน Popup ตอนเริ่ม
        // successPopup.SetActive(false);
        // nextLevelButton.interactable = false;
    }

    // ฟังก์ชันรับค่าเมื่อผู้เล่นกดเลือกของ
    public void SelectItem(ClothingItemData data)
    {
        // เติมบรรทัดนี้ลงไปเพื่อเช็คว่าปุ่มส่งข้อมูลมาถึง Manager ไหม
        Debug.Log("ได้รับข้อมูลชุด: " + data.name + " ประเภท: " + data.itemType);

        if (data.itemType == ItemType.Glove)
        {
            currentGlove = data;
            gloveRenderer.sprite = data.onCharacterSprite;
            gloveRenderer.gameObject.SetActive(true);
        }
        else if (data.itemType == ItemType.Outfit)
        {
            currentOutfit = data;
            outfitRenderer.sprite = data.onCharacterSprite;
            outfitRenderer.gameObject.SetActive(true);
        }

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        // 1. ต้องใส่ครบทั้ง 2 ส่วน
        if (currentGlove == null || currentOutfit == null) return;

        // 2. เช็ค ID ว่าตรงกันไหม (Core Logic)
        if (currentGlove.setID == currentOutfit.setID)
        {
            OnLevelComplete();
        }
        else
        {
            Debug.Log("ผิดคู่! ลองใหม่");
            // อาจจะเพิ่ม Effect แจ้งเตือนว่าผิดตรงนี้ได้
        }
    }

    private void OnLevelComplete()
    {
        Debug.Log("ถูกต้อง! setID: " + currentGlove.setID);
        
        // แสดงข้อมูลของชุด (ดึงมาจาก Outfit หรือ Glove ก็ได้เพราะ ID เดียวกัน)
        infoText.text = currentOutfit.description;
        
        // เปิด Popup และปุ่มไปต่อ
        successPopup.SetActive(true);
        // nextLevelButton.interactable = true;
    }
    
    public void LoadNextScene()
    {
        // โค้ดเปลี่ยน Scene
        // SceneManager.LoadScene("NextLevelName");
    }
}