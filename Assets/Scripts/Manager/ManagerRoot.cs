using System;
using UnityEngine;
using UnityEngine.UI;

public class ManagerRoot : Singleton<ManagerRoot>
{
    [SerializeField] private static GameManager gameManager;
    [SerializeField] private static SceneController sceneController;
    [SerializeField] private static ScoreManager scoreManager;
    [SerializeField] private static ResourceManager resourceManager;

    protected override void Init() // 매니저들의 초기화 호출 순서 조절
    {
        gameManager = new GameManager();
        sceneController = new SceneController();
        scoreManager = new ScoreManager();
        resourceManager = new ResourceManager();

        gameManager.Init();
        scoreManager.Init();
        resourceManager.Init();
        sceneController.Init(); 
        
    }
}
