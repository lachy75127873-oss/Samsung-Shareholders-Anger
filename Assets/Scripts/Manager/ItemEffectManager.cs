using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectManager
{
    private List<ActiveItem> activeItems = new List<ActiveItem>();
    private int totalCoins = 0; //scoreManger 또는 GameManger에서 관리할거 임시용

    private PlayerController playerController;

    // 원본 스탯 저장
    private float originalJumpPower;
    private float originalMaxHeight;

    public int GetTotalCoins() => totalCoins;
    public List<ActiveItem> GetActiveItems() => activeItems;

    public ItemEffectManager(PlayerController player)
    {
        playerController = player;
    }

    public void Update()
    {
        for (int i = activeItems.Count - 1; i >= 0; i--)
        {
            activeItems[i].remainingTime -= Time.deltaTime;

            if (activeItems[i].remainingTime <= 0)
            {
                // 버프 만료 처리
                OnBuffExpired(activeItems[i]);

                Debug.Log($"{activeItems[i].itemData.itemName} 효과 종료");
                activeItems.RemoveAt(i);
            }
        }
    }

    public void SaveOriginalStats(float jumpPower, float maxHeight)
    {
        originalJumpPower = jumpPower;
        originalMaxHeight = maxHeight;
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
        int coinValue = GetValue(itemData, 0, 1);
        totalCoins += coinValue;
        Debug.Log($"코인 획득: +{coinValue} (총: {totalCoins})");
    }

    private void ApplyBuff(ItemData itemData)
    {
        ActiveItem existing = activeItems.Find(x => x.itemData.itemType == itemData.itemType);

        bool isNewBuff = false;

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
            isNewBuff = true;
            Debug.Log($"{itemData.itemName} 활성화 ({itemData.duration}초)");
        }

        switch (itemData.itemType)
        {
            case ItemData.ItemType.JumpUp:
                ApplyJumpStats(itemData);
                break;

            case ItemData.ItemType.Magnet:
                if (isNewBuff)
                {
                    float magnetRange = GetValue(itemData, 0, 5);
                    playerController.EnableMagnetRange(magnetRange);
                }
                break;
        }
    }

    private void OnBuffExpired(ActiveItem buff)
    {
        if (buff.itemData.itemType == ItemData.ItemType.JumpUp)
        {
            RestoreJumpStats();
        }
        else if (buff.itemData.itemType == ItemData.ItemType.Magnet)
        {
            playerController.DisableMagnetRange();
        }
    }

    private void ApplyJumpStats(ItemData itemData)
    {
        float jumpPowerBonus = GetValue(itemData, 0, 0);
        float maxHeightBonus = GetValue(itemData, 1, 0);

        playerController.SetJumpPower(originalJumpPower + jumpPowerBonus);
        playerController.SetMaxHeight(originalMaxHeight + maxHeightBonus);
    }

    private void RestoreJumpStats()
    {
        playerController.SetJumpPower(originalJumpPower);
        playerController.SetMaxHeight(originalMaxHeight);
    }

    /// <summary>
    /// Json의 value배열에서 값 가져오기
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="index"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private int GetValue(ItemData itemData, int index, int defaultValue = 0)
    {
        if (itemData.value == null || itemData.value.Length <= index)
        {
            return defaultValue;
        }
        return itemData.value[index];
    }
}
