using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemID;
    private ItemData itemData;
    private MeshRenderer meshRenderer;
    private Collider itemCollider;

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
        itemData = ManagerRoot.dataManager.GetItemData(itemID);
    }
    #endregion

    private void ResetItem()
    {
        meshRenderer.enabled = true;
        itemCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Player player = other.GetComponent<PlayerController>(); // GameManger에서 플레이어 참조 가져오기
            //if(player!=null)
            //player.UseItme(itemData)

            meshRenderer.enabled = false;
            itemCollider.enabled = false;
        }
    }
}
