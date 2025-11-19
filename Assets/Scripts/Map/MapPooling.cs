using System.Collections.Generic;
using UnityEngine;

public class MapPooling : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mapPrefabs; // 프리팹 받아오기
    private List<GameObject> mapPool = new List<GameObject>(); // 풀 만들기
    private float spawnPos = 0f;
    private Transform player;
    private List<GameObject> currentMap = new List<GameObject>();
    private int mapCount = 0;
    void Start()
    {
        InitPool();
        ActivateRandomMap();
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어 위치받아오기
    }
    private void Update()
    {
        if (player.position.z >= spawnPos - 120f) // 플레이어 위치가 120 마다 (스타트에서 맵한개 생성후 플레이어가 0이상에서 시작하기 때문에 시작시 맵 2개 생성후 진행
        {
            ActivateRandomMap(); // 맵생성
        }
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
    void ActivateRandomMap() // 프리팹 활성화
    {
        GameObject map = GetRandomeMap(); // map = GetInactiveMap()에서 반환된 랜덤으로 뽑은 맵을 담는다
        if (currentMap != null&&mapCount >= 3) // 3번째 생성할때
        {
            currentMap[0].SetActive(false); // 첫번째 생성된 맵 비활성화
            currentMap.Remove(currentMap[0]); // 리스트에서 삭제하여 다음맵 0번으로 조정
        }
        if (map != null) // map이 null이 아니라면
        {
            map.transform.localPosition = new Vector3(0, 0, spawnPos); // 랜덤으로 뽑은 맵의 포지션을 새롭게 정해준다
            map.SetActive(true); // 그 맵을 활성화 시켜준다
            currentMap.Add(map);
            spawnPos += 120f; // 다음 맵 위치를 조정하기위해 프리팹(맵)의 전체길이를 추가하여 뒤에 이어붙힌다
            mapCount++;
        }
    }

    GameObject GetRandomeMap() // 랜덤으로 뽑기
    {
        List<GameObject> inactiveMaps = new List<GameObject>();
        foreach (GameObject obj in mapPool) // mapPool길이 만큼 돌면서
        {
            if (!obj.activeInHierarchy) // 비활성화 된것들을
                inactiveMaps.Add(obj); // 리스트에 담는다
        }

        if (inactiveMaps.Count > 0) // 만약 비활성화 된것이 1개라도 있을때
        {
            int rand = Random.Range(0, inactiveMaps.Count); // 비활성화 된 오브젝트를 담은 리스트의 길이값 사이 중 하나를 뽑아
            return inactiveMaps[rand]; // 반환시킨다
        }

        return null; // 모든것이 활성화 되어있다면 null을 반환한다
    }
}
