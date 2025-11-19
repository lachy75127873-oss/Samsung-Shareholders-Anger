using System;
using UnityEngine;
using UnityEngine.UI;

public class ManagerRoot : Singleton<ManagerRoot>
{
    public static ScoreManager scoreManager;
    public static ResourceManager resourceManager;
    public static DataManager dataManager;
    public static ItemEffectManager itemEffectManager;

    [SerializeField] private GameManager _gameManager;
    public SceneController sceneController;
    public AudioManager audioManager;

    public static GameManager gameManager { get; private set; }

    protected override void Init() // 매니저들의 초기화 호출 순서 조절
    {
        Debug.Log("ManagerRoot 초기화 시작");

        scoreManager = new ScoreManager();
        resourceManager = new ResourceManager();
        dataManager = new DataManager("Data/Item", "Items");
        itemEffectManager = new ItemEffectManager();
        gameManager = _gameManager;

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
        itemEffectManager?.Update();
    }

    private void OnDestroy()
    {
        dataManager?.Release();
    }
}
