using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData",menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    /// <summary>
    /// 아이템은 자동생성이 아니라 맵에 미리 배치이므로 Instantiate또는 Pool용 GameObject 불필요
    /// </summary>
    [Header("기본 설정")]
    public int id; // Json과 매칭활 고유 ID
    public string itemName;
    public string description;

    [Header("아이템 속성")]
    public ItemType itemType;
    public int[] value; // 아이템 값
    public float duration; // 아이템 지속시간

    public enum ItemType
    {
        Coin,
        JumpUp,
        Magnet
    }

    public virtual void LoadFromJson(ItemJsonData jsonData)
    {
        id = jsonData.id;
        itemName = jsonData.itemName;
        description = jsonData.description;
        value = jsonData.value;
        duration = jsonData.duration;

        // enum 파싱
        if (System.Enum.TryParse(jsonData.itemType, out ItemType type))
        {
            itemType = type;
        }
    }
}
