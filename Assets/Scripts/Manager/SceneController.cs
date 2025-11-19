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
    
    private Coroutine coroutine; // 로딩씬용 코루틴
    [SerializeField] private float loadingTime = 1.5f;
    
    //씬컨트롤러 초기화 로직
    public void Init() 
    {
        Debug.Log("SceneManager initialized");
        SubscribeSceneManager(); // 초기화 시 구독
        targetState = ScreenState.None;
    }

    //sceneLoaded는 씬이 로드됐을 때의 상황을 전제
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
            case "TempCDW": //예비 인트로 씬이라 가정
                OnIntroScene();
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
    private void OnIntroScene()
    {
        Debug.Log("인트로 화면 동작 중");
        /*
         * UI - 인트로 
         */
        
        targetState = ScreenState.Main_Title;
        
        //로딩씬으로
        OnLoadingScene();
    }


    #region "Main_Title"Scene 실행 매서드

    private void OnMainTitle()
    {
        Debug.Log("OnMainTitle");
        
        /*
         * UIManager - 메인 타이틀 UI키기
         * UIManager - start버튼 누를 시,
         *
         *ScoreManager - 최고점수 보여주기
         *
         * GameManager - setPlayer
         */
        Debug.Log("MainTitle UI");
        targetState = ScreenState.Main_Game;
        
        // 로딩씬으로
        Debug.Log("3초 대기");
        Invoke(nameof(OnLoadingScene), 3);
        
    }

    #endregion

    #region "Main_Game"Scene 실행 매서드

    private void OnMainGame()
    {
        Debug.Log("OnMainGame");
        //UIManager야 게임 UI 켜라
        
        //ScoreManager - 점수 초기화
        
        //GameManager - 스테이지 초기화(플레이어 정보 & 맵 배치 등)(속도, 생사, 위치 등)
        
        //GameManager = 게임오버 판정
        
        //ScoreManager - 최고점수 갱신
        
        // 로딩씬으로
        Debug.Log("3초 대기");
        Invoke(nameof(OnLoadingScene), 3);
    }

    #endregion

    #region LoadingScene
   
    //"Loading"Scene에서 실행 매서드

    public void LoadTargetScreen(ScreenState state)
    {
        //외부에서 씬 전환하고 싶을 때 호출할 필요가 있어보여서...?
        // OnLoadingScene 을 지역변수가 아니라 매개변수 사용하도록 바꾸면 될듯??
    }
    private void OnLoadingScene()
    {
        Debug.Log("OnLoadingScene");
        
        //로딩씬으로 이동
        SceneManager.LoadScene("Loading");
        
        //UIManager - 로딩씬 켜라 -> 알아서 켜도 괜춘할듯
        Debug.Log("Loading UI");
        
       //비동기식 로딩씬 await asynk -> 참고만
       Debug.Log("로딩 시작");
       StartCoroutine(LoadSceneRoutine(targetState.ToString()));
       Debug.Log($"{targetState.ToString()}으로의 로딩완료");
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        float timer = loadingTime;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        
        // 로딩 끝날 때까지 반복
        while (!op.isDone)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("로딩 코루틴 완료");
        
    }
    
    private void LoadMainTitle()
    {
        Debug.Log("LoadToMainTitle");
    }

    private void LoadMainGame()
    {
        Debug.Log("LoadMainGame");
    }
    
    #endregion
    

}
