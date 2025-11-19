using System.Collections.Generic;
using UnityEngine;

public class MapPooling : MonoBehaviour
{
    [SerializeField] // 난이도에 맞는 프리팹 가져오기
    private GameObject[] easyMaps;
    [SerializeField]
    private GameObject[] normalMaps;
    [SerializeField]
    private GameObject[] hardMaps;
   
    //각각의 프리팹들을 저장하는 리스트
    private List<GameObject> easyMappool = new List<GameObject>();
    private List<GameObject> normalMappool = new List<GameObject>();
    private List<GameObject> hardMappool = new List<GameObject>();

    //비활성화 된 맵을 담고 랜덤으로 하나를 뽑는 리스트
    List<GameObject> RandMap = new List<GameObject>();

    private float spawnPos = 0f;
    private Transform player;
    private List<GameObject> currentMap = new List<GameObject>();

    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> initialRotations = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, bool> initialActives = new Dictionary<GameObject, bool>();


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어 위치받아오기
        InitPool();
        ActivateMap();
    }
    private void Update()
    {
        if (player.position.z >= spawnPos - 120f) // 플레이어 위치가 120 마다 (스타트에서 맵한개 생성후 플레이어가 0이상에서 시작하기 때문에 시작시 맵 2개 생성후 진행
        {
            ActivateMap(); // 맵생성
        }
    }

    void SaveInitialStates(GameObject map)
    {
        foreach (Transform child in map.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("Train") || child.CompareTag("Coin") ||
                child.name.Contains("Train") || child.name.Contains("Coin"))
            {
                var obj = child.gameObject;
                initialPositions[obj] = obj.transform.localPosition;
                initialRotations[obj] = obj.transform.localRotation;
                initialActives[obj] = obj.activeSelf;
            }
        }
    }
    void ResetMapContents(GameObject map)
    {
        foreach (Transform child in map.GetComponentsInChildren<Transform>(true))
        {
            var obj = child.gameObject;
            if (initialPositions.ContainsKey(obj))
            {
                obj.transform.localPosition = initialPositions[obj];
                obj.transform.localRotation = initialRotations[obj];
                obj.SetActive(initialActives[obj]);

                // Rigidbody 초기화
                var rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }


    #region"Pooling"
    private void InitPool() // 프리팹 받아온걸 생성 후 비활성화, 풀에 넣기
    {
        for (int i = 0; i < easyMaps.Length; i++) // 받아온 프리팹 길이만큼 반복
        {
            for(int j = 0; j < 2; j++)
            {
                GameObject obj = Instantiate(easyMaps[i], transform); // 자식으로 프리팹들 생성후
                obj.SetActive(false); // 비활성화
                easyMappool.Add(obj); // 풀에 넣기
            }
        }
        for (int i = 0; i < normalMaps.Length; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject obj = Instantiate(normalMaps[i], transform);
                obj.SetActive(false);
                normalMappool.Add(obj);
            }
        }
        for (int i = 0; i < hardMaps.Length; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject obj = Instantiate(hardMaps[i], transform);
                obj.SetActive(false);
                hardMappool.Add(obj);
            }
        }
    }
    #endregion

    #region"ActiveMap"
    void ActivateMap() // 프리팹 활성화
    {
        GameObject map = GetRandomeMap(); // map = GetInactiveMap()에서 반환된 랜덤으로 뽑은 맵을 담는다
        if (map != null) // map이 null이 아니라면
        {
            ResetMapContents(map);
            map.transform.localPosition = new Vector3(0, 0, spawnPos); // 랜덤으로 뽑은 맵의 포지션을 새롭게 정해준다
            map.SetActive(true); // 그 맵을 활성화 시켜준다
            if (!currentMap.Contains(map))
            {
                currentMap.Add(map);
            }
            spawnPos += 120f; // 다음 맵 위치를 조정하기위해 프리팹(맵)의 전체길이를 추가하여 뒤에 이어붙힌다
        }
        if (currentMap.Count >= 3) // 3번째 생성할때
        {
            currentMap[0].SetActive(false); // 첫번째 생성된 맵 비활성화
            currentMap.RemoveAt(0); // 리스트에서 삭제하여 다음맵 0번으로 조정
        }
    }
    #endregion

    #region"SetRandomMap"
    GameObject GetRandomeMap() // 랜덤으로 뽑기
    {
        RandMap.Clear();
        int level = Mathf.FloorToInt(player.transform.position.z / 500f); // 거리 500마다 레벨 증가
        level = Mathf.Clamp(level, 1, 5);

        switch (level)
        {
            case 1:
                SetEasyMap();
                break;
            case 2:
                SetEasyMap();
                SetNormalMap();
                break;
            case 3:
                SetNormalMap();
                break;
            case 4:
                SetNormalMap();
                SetHardMap();
                break;
            case 5:
                SetHardMap();
                break;
        }
        if (RandMap.Count > 0) // 만약 비활성화 된것이 1개라도 있을때
        {
            int rand = Random.Range(0, RandMap.Count); // 비활성화 된 오브젝트를 담은 리스트의 길이값 사이 중 하나를 뽑아
            return RandMap[rand]; // 반환시킨다
        }

        return null; // 모든것이 활성화 되어있다면 null을 반환한다
    }
    #endregion

    #region"DeactiveList"
    private void SetEasyMap()
    {
        foreach (GameObject obj in easyMappool) // mapPool길이 만큼 돌면서
        {
            if (!obj.activeInHierarchy) // 비활성화 된것들을
                RandMap.Add(obj); // 리스트에 담는다
        }
    }
    private void SetNormalMap()
    {
        foreach (GameObject obj in normalMappool)
        {
            if (!obj.activeInHierarchy)
                RandMap.Add(obj);
        }
    }
    private void SetHardMap()
    {
        foreach (GameObject obj in hardMappool)
        {
            if (!obj.activeInHierarchy)
                RandMap.Add(obj);
        }
    }
    #endregion
}