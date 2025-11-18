using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ScreenState
{
    None,
    Loading,
    Main_Title,
    Main_Game,
    Quit
}

public class SceneController : MonoBehaviour
{
    private ScreenState targetState; //다음 상태 지정용 enum
    private Coroutine coroutine; //
    
    //씬컨트롤러 초기화 로직
    public void Init() 
    {
        Debug.Log("SceneManager initialized");
        SubscribeSceneManager(); // 초기화 시 구독
        targetState = ScreenState.None;
    }

    private void SubscribeSceneManager()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "TempCDW": //시험용 코드
                ToDoCDW();
                break;
            case "Loading":
                OnLoadingScene();
                break;
            case "Main_Title":
                OnMainTitle();
                break;
            case "Main_Game":
                OnMainGame();
                break;
            default:
                break;
        }
    }
    
    //"TempCDW"씬에서 실행 매서드
    private void ToDoCDW()
    {
        Debug.Log("ToDoCDW");
        
        SceneManager.LoadScene("Loading"); 
    }
    
    //"Loading"Scene에서 실행 매서드
    private void OnLoadingScene()
    {
        Debug.Log("OnLoadingScene");
        
        //UIManager - 로딩씬 켜라
        Debug.Log("Loading UI");

        switch (targetState)
        {
            case ScreenState.None: 
                LoadMainTitle(); //타이틀 화면으로
                break;
            case ScreenState.Main_Title:
                LoadTitleToGame(); //게임 화면으로
                break; 
            case ScreenState.Main_Game:
                LoadGameToTitle(); //타이틀 화면으로
                break;
            default:
                break;
        }
        
        // ManagerRoot.gameManager.ReadyMain(); //중단된 게임 여부 검사, 다음 씬이 어딘지 결정
        // ManagerRoot.resourceManager.ReadyMain(); //MainTitle 배경음효과음 
        // ManagerRoot.scoreManager.ReadyMain(); //최고점수 등록
        
        
       //비동기식 로딩씬 await asynk -> 참고만
        StartCoroutine(LoadSceneRoutine(targetState.ToString()));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        float timer = 1.5f;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        // 로딩 끝날 때까지 반복
        while (!op.isDone||timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
        }
        
        Debug.Log("로딩 코루틴 완료");
        
    }
    
    private void LoadMainTitle()
    {
        Debug.Log("LoadToMainTitle");
        targetState = ScreenState.Main_Title;
    }

    private void LoadTitleToGame()
    {
        Debug.Log("TitleToGame");
        targetState = ScreenState.Main_Game;
        StartCoroutine(LoadSceneRoutine(targetState.ToString()));
    }

    private void LoadGameToTitle()
    {
        Debug.Log("GameToTitle");
        targetState = ScreenState.Main_Title;
    }


    //"Main_Title"Scene에서 실행 매서드
    private void OnMainTitle()
    {
        Debug.Log("OnMainTitle");
        
        //UIManager - 메인 타이틀 유아이 켜라
        Debug.Log("MainTitle UI");
        
        //UI에서 Main_Game신호 도달 시 씬전환
        targetState = ScreenState.Main_Game;
        
        // 로딩씬 
        SceneManager.LoadScene(ScreenState.Loading.ToString());
        
    }

    //"Main_Game"Scene에서 실행 매서드
    private void OnMainGame()
    {
        Debug.Log("OnMainGame");
        //UIManager야 게임 UI 켜라
    }


    

    

}
