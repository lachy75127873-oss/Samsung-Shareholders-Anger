using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemID;
    private ItemData itemData;
    private MeshRenderer meshRenderer;
    private Collider itemCollider;
    private AudioManager audioManager;

    #region 유니티 CallBack
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        itemCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        ResetItem();
    }

    private void Start()
    {
        itemData = ManagerRoot.Instance.dataManager.GetItemData(itemID);
        audioManager = ManagerRoot.Instance.audioManager;
    }
    #endregion

    private void ResetItem()
    {
        meshRenderer.enabled = true;
        itemCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ItemCheck"))
        {
            Debug.Log($"[Item] 아이템 사용: {itemData.itemName}");
            // 아이템 효과 적용
            ManagerRoot.gameManager.player.ApplyItemEffect(itemData);

            meshRenderer.enabled = false;
            itemCollider.enabled = false;

            if(itemID != 100)
                audioManager.PlayGetItem();
        }
    }
}
