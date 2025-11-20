using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class UiManager : MonoBehaviour
{
    private static UiManager instance;
    public static UiManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
        }
        else if (instance == this)
        {
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    { }
    StartMenuScene startMenuScene;
    LoadingScene loadingScene;
    GameSceneUI gameSceneUI;

    internal void InputStartMenu(StartMenuScene start)
    { startMenuScene = start; }
    internal void InputloadingScene(LoadingScene load)
    { loadingScene = load; }
    internal void InputGameMenu(GameSceneUI game)
    { gameSceneUI = game; }
    //--------------------매뉴씬
    /// <summary>
    /// 게임 시작버튼을 누르면 게임씬으로 이동함.
    /// </summary>
    internal void StartGame()
    { UiManager.LoadScene(startMenuScene.gameSceneName); }
    /// <summary>
    /// 옵션 버튼을 누르면 옵션창이 뜸.
    /// </summary>
    internal void TurnOption()
    { startMenuScene.optionUI.SetActive(true); }
    /// <summary>
    /// 옵션창에서 나가기 누르면 옵션창이 꺼짐.
    /// </summary>
    internal void ExitOption()
    { startMenuScene.optionUI.SetActive(false); }
    //--------------로딩씬
    internal static void LoadScene(string sceneName)
    {
        LoadingScene.nextScene = sceneName;
        // sceneName에 따라 적절한 ScreenState 결정
        ScreenState targetState = GetScreenStateFromSceneName(sceneName);

        // SceneController에게 로딩 위임
        ManagerRoot.Instance.sceneController.LoadTargetScene(targetState);
    }
    /// <summary>
    /// 로딩바를 코루틴으로 실행
    /// </summary>
    /// <returns></returns>
    internal IEnumerator LoadSceneProcess()
    {
        // loadingScene이 등록될 때까지 대기
        float waitTime = 0f;
        while (loadingScene == null && waitTime < 2f)
        {
            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // AsyncOperation을 메서드 시작 부분에서 한 번만 선언
        AsyncOperation op;

        if (loadingScene == null)
        {
            // loadingBar 없이 씬만 로드
            op = SceneManager.LoadSceneAsync(LoadingScene.nextScene);
            yield return op;
            yield break;
        }

        if (loadingScene.loadingBar == null)
        {
            // loadingBar 없이 씬만 로드
            op = SceneManager.LoadSceneAsync(LoadingScene.nextScene);
            yield return op;
            yield break;
        }

        // 정상 로딩 프로세스
        op = SceneManager.LoadSceneAsync(LoadingScene.nextScene);
        op.allowSceneActivation = false;
        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                loadingScene.loadingBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                loadingScene.loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                if (loadingScene.loadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    internal void LoadingBar(float progress)
    { loadingScene.loadingBar.fillAmount = progress; }
    //--------------게임씬
    /// <summary>
    /// score에 넣은 값이 현재 주가로 표시됨.
    /// </summary>
    /// <param name="score"></param>
    internal void InputScore(float score)//점수가 어디있는지 물어서 세팅해야 됨.
    {
        if (gameSceneUI == null) return;

        // 점수 업데이트
        if (gameSceneUI.currentScore != null)
        {
            gameSceneUI.currentScore.text = score.ToString("N0");
        }
    }
    /// <summary>
    /// 알람을 사라지게 하는 함수.
    /// </summary>
    void AlarmOff()
    {
        gameSceneUI.alarmAnimator.SetBool("isActive", false);
        gameSceneUI.alarmShow.SetActive(false);
    }
    internal void GetItem(string name, string describe)//아이템 종류는 어떻게 구별하지
    {
        gameSceneUI.itemUIPrefeb.SetActive(true);
        gameSceneUI.itemUIPrefeb2.SetActive(true);
    }

    void ItemOff(GameObject itemUI)
    { itemUI.SetActive(false); }
    /// <summary>
    /// 게임을 끝내는 함수
    /// </summary>
    public void EndGame()
    {
        // ScoreManager에서 점수 가져오기
        int currentScore = ManagerRoot.Instance.scoreManager.GetTotalScore();
        int bestScore = ManagerRoot.Instance.scoreManager.bestScore;

        Debug.Log($"게임 오버 - 현재: {currentScore}, 최고: {bestScore}");

        // GameSceneUI에 점수 표시
        gameSceneUI.ShowGameOver(currentScore, bestScore);

        // 최고 점수 저장
        ManagerRoot.Instance.scoreManager.SetBestScore();
    }
    /// <summary>
    /// 매뉴로 가는 키
    /// </summary>
    internal void GoToMenu()
    { UiManager.LoadScene(gameSceneUI.mainSceneName); }
    /// <summary>
    /// 재시작 키
    /// </summary>
    internal void Restart()
    { UiManager.LoadScene(gameSceneUI.gameSceneName); }


    private static ScreenState GetScreenStateFromSceneName(string sceneName)
    {
        switch (sceneName)
        {
            case "Main_Title":
                return ScreenState.Main_Title;
            case "Main_Game":
                return ScreenState.Main_Game;
            default:
                Debug.LogWarning($"알 수 없는 씬 이름: {sceneName}");
                return ScreenState.Main_Game;
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
