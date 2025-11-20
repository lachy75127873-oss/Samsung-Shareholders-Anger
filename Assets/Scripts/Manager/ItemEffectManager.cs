using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectManager
{
    private List<ActiveItem> activeItems = new List<ActiveItem>();

    // 원본 스탯 저장
    private float originalJumpPower;
    private float originalMaxHeight;

    public List<ActiveItem> GetActiveItems() => activeItems;

    public void SaveOriginalStats(float jumpPower, float maxHeight)
    {
        originalJumpPower = jumpPower;
        originalMaxHeight = maxHeight;
        Debug.Log($"원본 스탯 저장: 점프력={jumpPower}, 최대높이={maxHeight}");
    }

    public void Update()
    {
        for (int i = activeItems.Count - 1; i >= 0; i--)
        {
            activeItems[i].remainingTime -= Time.deltaTime;

            if (activeItems[i].remainingTime <= 0)
            {
                OnBuffExpired(activeItems[i]);
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
            case ItemData.ItemType.Magnet:
                ApplyBuff(itemData);
                break;
        }
    }

    private void ApplyCoin(ItemData itemData)
    {
        int coinValue = GetValue(itemData, 0, 1);
        ManagerRoot.Instance.scoreManager.totalScore += coinValue;
        Debug.Log("500점 추가");
    }

    private void ApplyBuff(ItemData itemData)
    {
        if (ManagerRoot.gameManager.player == null)
        {
            Debug.LogError("PlayerController가 등록되지 않았습니다!");
            return;
        }

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
                    ManagerRoot.gameManager.player.EnableMagnetRange(magnetRange);
                }
                break;
        }
    }

    private void OnBuffExpired(ActiveItem buff)
    {
        if (ManagerRoot.gameManager.player == null) return;

        switch (buff.itemData.itemType)
        {
            case ItemData.ItemType.JumpUp:
                RestoreJumpStats();
                break;

            case ItemData.ItemType.Magnet:
                ManagerRoot.gameManager.player.DisableMagnetRange();
                break;
        }
    }

    private void ApplyJumpStats(ItemData itemData)
    {
        float jumpPowerBonus = GetValue(itemData, 0, 0);
        float maxHeightBonus = GetValue(itemData, 1, 0);

        ManagerRoot.gameManager.player.SetJumpPower(originalJumpPower + jumpPowerBonus);
        ManagerRoot.gameManager.player.SetMaxHeight(originalMaxHeight + maxHeightBonus);
    }

    private void RestoreJumpStats()
    {
        ManagerRoot.gameManager.player.SetJumpPower(originalJumpPower);
        ManagerRoot.gameManager.player.SetMaxHeight(originalMaxHeight);
    }

    private int GetValue(ItemData itemData, int index, int defaultValue = 0)
    {
        if (itemData.value == null || itemData.value.Length <= index)
        {
            return defaultValue;
        }
        return itemData.value[index];
    }

    public void Reset()
    {
        activeItems.Clear();
        Debug.Log("ItemEffectManager 리셋 완료");
    }

    public void DebugLogStatus()
    {
        Debug.Log($"활성 버프 개수: {activeItems.Count}");

        if (activeItems.Count > 0)
        {
            foreach (var buff in activeItems)
            {
                Debug.Log($"  버프: {buff.itemData.itemName}");
                Debug.Log($"  타입: {buff.itemData.itemType}");
                Debug.Log($"  남은 시간: {buff.remainingTime:F2}초");
                Debug.Log($"  값: [{string.Join(", ", buff.itemData.value)}]");
            }
        }
        else
        {
            Debug.Log("활성화된 버프 없음");
        }

        Debug.Log($"원본 점프력: {originalJumpPower}");
        Debug.Log($"원본 최대높이: {originalMaxHeight}");

        if (ManagerRoot.gameManager.player != null)
        {
            Debug.Log($"현재 점프력: {ManagerRoot.gameManager.player.jumpPower}");
            Debug.Log($"현재 최대높이: {ManagerRoot.gameManager.player.MaxHeight}");
        }
    }
}
