using System;
using UnityEngine;
using UnityEngine.UI;

public class ManagerRoot : Singleton<ManagerRoot>
{
   public static GameManager gameManager;
   public static ScoreManager scoreManager;
   public static ResourceManager resourceManager;
   
   [SerializeField]private SceneController sceneController; 

    protected override void Init() // 매니저들의 초기화 호출 순서 조절
    {
        gameManager = new GameManager();
        scoreManager = new ScoreManager();
        resourceManager = new ResourceManager();

        gameManager.Init();
        scoreManager.Init();
        resourceManager.Init();
        sceneController.Init();
        
    }
}
