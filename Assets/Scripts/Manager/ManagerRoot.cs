using System;
using UnityEngine;
using UnityEngine.UI;

public class ManagerRoot : Singleton<ManagerRoot>
{
    public static GameManager gameManager;
    public static ScoreManager scoreManager;
    public static ResourceManager resourceManager;
    public static DataManager dataManager;
    public static ItemEffectManager itemEffectManager;

    [SerializeField] private SceneController sceneController;
    [SerializeField] private AudioManager audioManager;

    protected override void Init() // 매니저들의 초기화 호출 순서 조절
    {
        Debug.Log("ManagerRoot 초기화 시작");

        gameManager = new GameManager();
        scoreManager = new ScoreManager();
        resourceManager = new ResourceManager();
        dataManager = new DataManager("Data/Item", "Items");
        itemEffectManager = new ItemEffectManager();

        gameManager?.Init();
        dataManager?.Init();
        scoreManager?.Init();
        resourceManager?.Init();
        sceneController?.Init();
        audioManager?.Init();

        Debug.Log("ManagerRoot 초기화 완료");
    }

    private void Update()
    {
        scoreManager?.Update();
        itemEffectManager?.Update();
    }

    private void OnDestroy()
    {
        dataManager?.Release();
    }
}
