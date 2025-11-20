using System;
using System.Collections;
using System.Collections.Generic;
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
        //OnLoadingScene();
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
        //targetState = ScreenState.Main_Game;
        
        // 로딩씬으로
        //Debug.Log("3초 대기");
        //Invoke(nameof(OnLoadingScene), 3);
        
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
        //Debug.Log("3초 대기");
        //Invoke(nameof(OnLoadingScene), 3);
    }

    #endregion

    #region LoadingScene
   
    //"Loading"Scene에서 실행 매서드

    public void OnLoadingScene()
    {
        Debug.Log($"로딩 시작: {targetState}");
        StartCoroutine(LoadSceneRoutine(targetState.ToString()));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        Debug.Log($"LoadSceneRoutine 시작: {sceneName}");

        float timer = loadingTime;
        while (timer > 0f)
        {
            UiManager.Instance.LoadingBar(1f-timer/10);
            timer -= Time.deltaTime;
            yield return null;
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        if (op == null)
        {
            Debug.LogError($"씬 로드 실패: {sceneName}");
            yield break;
        }

        while (!op.isDone)
        {
            yield return null;
        }

        Debug.Log($"LoadSceneRoutine 완료: {sceneName}");
    }

    public void LoadTargetScene(ScreenState targetScreenState)
    {
        Debug.Log($"로딩 요청: {targetScreenState}");

        targetState = targetScreenState;

        // Loading 씬으로 이동
        SceneManager.LoadScene("Loading");
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
