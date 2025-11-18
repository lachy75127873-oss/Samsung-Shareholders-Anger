using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectManager
{
    private List<ActiveItem> activeItems = new List<ActiveItem>();
    private int totalCoins = 0; //scoreManger 또는 GameManger에서 관리할거 임시용

    public void Update()
    {
        for (int i = activeItems.Count - 1; i >= 0; i--)
        {
            activeItems[i].remainingTime -= Time.deltaTime;

            if (activeItems[i].remainingTime <= 0)
            {
                Debug.Log($"{activeItems[i].itemData.itemName} 효과 종료");
                activeItems.RemoveAt(i);
            }
        }
    }

    public void ApplyItem(ItemData itemData)
    {
        if (itemData == null) return;

        switch (itemData.itemType)
        {
            case ItemData.ItemType.Coin:
                ApplyCoin(itemData);
                break;

            case ItemData.ItemType.JumpUp:
                ApplyBuff(itemData);
                break;

            case ItemData.ItemType.Magnet:
                ApplyBuff(itemData);
                break;
        }
    }

    private void ApplyCoin(ItemData itemData)
    {
        totalCoins += 1;
        Debug.Log($"코인 획득: +{1} (총: {totalCoins})");
    }

    private void ApplyBuff(ItemData itemData)
    {
        ActiveItem existing = activeItems.Find(x => x.itemData.itemType == itemData.itemType);

        if (existing != null)
        {
            existing.remainingTime = itemData.duration;
            existing.itemData = itemData;
            Debug.Log($"{itemData.itemName} 갱신 ({itemData.duration}초)");
        }
        else
        {
            ActiveItem buff = new ActiveItem
            {
                itemData = itemData,
                remainingTime = itemData.duration,
                isActive = true
            };

            activeItems.Add(buff);
            Debug.Log($"{itemData.itemName} 활성화 ({itemData.duration}초)");
        }
    }
}
