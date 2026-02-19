using UnityEngine;

[CreateAssetMenu(fileName = "NewClothingItem", menuName = "Game/Clothing Item")]
public class ClothingItemData : ScriptableObject
{
    [Header("Data")]
    public int setID; // ID สำหรับเช็คคู่ (เช่น 1, 2)
    public ItemType itemType; // ประเภท (Glove หรือ Outfit)
    
    [Header("Visuals")]
    public Sprite iconSprite; // รูปที่จะโชว์บนปุ่ม UI
    public Sprite onCharacterSprite; // รูปที่จะไปแปะบนตัวละคร
    
    [Header("Info")]
    [TextArea] public string description; // ข้อมูลที่จะโชว์เมื่อจับคู่ถูก
}

public enum ItemType
{
    Glove,
    Outfit
}