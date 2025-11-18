using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPooling : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mapPrefabs; // 프리팹 받아오기
    private List<GameObject> mapPool = new List<GameObject>(); // 풀 만들기

    void Start()
    {
        InitPool();
    }

    private void InitPool() // 프리팹 받아온걸 생성 후 비활성화, 풀에 넣기
    {
        for (int i = 0; i < mapPrefabs.Length; i++) // 받아온 프리팹 길이만큼 반복
        {
            GameObject obj = Instantiate(mapPrefabs[i], transform); // 자식으로 프리팹들 생성후
            obj.SetActive(false); // 비활성화
            mapPool.Add(obj); // 풀에 넣기
        }
    }
    
    void Update()
    {
        
    }
}
