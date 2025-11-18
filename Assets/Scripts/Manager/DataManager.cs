using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager
{
    private string jsonAddress;
    private string itemDataLabel;

    [Header("아이템 데이터 리스트")]
    [SerializeField] private List<ItemData> itemDataList = new List<ItemData>();

    private Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();
    private ItemJsonDatabase jsonDatabase;


    /// <summary>
    /// ScriptableObject 데이터 경로 설정(default: item)
    /// </summary>
    /// <param name="_jsonFileName"></param>
    /// <param name="_dataPath"></param>
    public DataManager(string _jsonAddress = "Data/Item", string _itemDataLabel = "Items")
    {
        this.jsonAddress = _jsonAddress;
        this.itemDataLabel = _itemDataLabel;
    }

    #region 초기화
    public void Init()
    {
        LoadItemDataAssets();
        LoadJsonData();
        InitializeItems();

        Debug.Log($"ItemDataManager 초기화 완료: {itemDictionary.Count}개 아이템");
    }
    #endregion

    #region Json파싱
    private void LoadJsonData()
    {

        var handle = Addressables.LoadAssetAsync<TextAsset>(jsonAddress);
        TextAsset jsonFile = handle.WaitForCompletion();

        if (jsonFile != null)
        {
            jsonDatabase = JsonUtility.FromJson<ItemJsonDatabase>(jsonFile.text);
        }
        else
        {
            jsonDatabase = new ItemJsonDatabase();
        }
    }
    #endregion

    #region 아이템 초기화
    private void LoadItemDataAssets()
    {
        var handle = Addressables.LoadAssetsAsync<ItemData>(itemDataLabel, (itemData) => { itemDataList.Add(itemData); });

        handle.WaitForCompletion();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"ItemData 로드 완료: {itemDataList.Count}개 ({itemDataLabel})");
        }
        else
        {
            Debug.LogError($"ItemData 로드 실패: {handle.OperationException}");
        }
    }

    private void InitializeItems()
    {
        itemDictionary.Clear();

        foreach (var itemData in itemDataList)
        {
            if (itemData != null)
            {
                Debug.Log($"[ItemData] ID: {itemData.id}, 이름: {itemData.name}");
            }
        }

        foreach (var itemData in itemDataList)
        {
            if (itemData == null) continue;

            ItemJsonData jsonData = jsonDatabase.items.Find(x => x.id == itemData.id);

            if (jsonData != null)
            {
                itemData.LoadFromJson(jsonData);
                itemDictionary[itemData.id] = itemData;
            }
            else
            {
                itemDictionary[itemData.id] = itemData;
            }
        }

        Debug.Log($"초기화 완료: {itemDictionary.Count}개");
    }
    #endregion

    #region 아이템 데이터 가져오는 메서드
    public ItemData GetItemData(int id)
    {
        if (itemDictionary.ContainsKey(id))
        {
            return itemDictionary[id];
        }

        Debug.LogWarning($"ID {id}에 해당하는 아이템 없음");
        return null;
    }

    public List<ItemData> GetItemsByType(ItemData.ItemType type)
    {
        return itemDataList.FindAll(x => x.itemType == type);
    }
    #endregion

    #region 리소스 해제
    // ManagerRoot에서 호출
    public void Release()
    {
        Addressables.Release(itemDataList);
        itemDataList.Clear();
        itemDictionary.Clear();

        Debug.Log("DataManager 리소스 해제");
    }
    #endregion
}