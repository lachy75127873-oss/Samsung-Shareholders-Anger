using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour, IObstacle
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float activationDistance;
    [SerializeField] private bool isActivation;
    private Camera mainCam;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // GameMangaer에서 현재 플레이어 들고 있게 한다음 게임매니저 참조해서 하는게 성능 상은 더 좋음
        Init();
    }

    private void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= activationDistance && !isActivation)
        {
            isActivation = true;
            OnActive();
        }

        if (isActivation)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }

        if (transform.position.z < mainCam.transform.position.z - 10f)
        {
            gameObject.SetActive(false);
        }
    }

    public void Init()
    {
        mainCam = Camera.main;
        isActivation = false;
    }

    public void Interact(GameObject player)
    {
        // 플레이어와 충돌 시

    }

    public void OnActive()
    {
        Debug.Log("기차 활성화!");
    }
}
