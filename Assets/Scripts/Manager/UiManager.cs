using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public UiManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = this;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else if (instance == this)
        {
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {}
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
        SceneManager.LoadScene("Loading");
    }
    /// <summary>
    /// 로딩바를 코루틴으로 실행
    /// </summary>
    /// <returns></returns>
    internal IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(LoadingScene.nextScene);
        op.allowSceneActivation = false;
        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;
            if (op.progress < 0.9f)
            { loadingScene.loadingBar.fillAmount = op.progress; }
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
    //--------------게임씬
    /// <summary>
    /// score에 넣은 값이 현재 주가로 표시됨.
    /// </summary>
    /// <param name="score"></param>
    internal void InputScore(float score)//점수가 어디있는지 물어서 세팅해야 됨.
    {
        gameSceneUI.currentScore.text = score.ToString("N2");
        if (gameSceneUI.previousScore > 0)
        {
            float percent = ((score - gameSceneUI.previousScore) / gameSceneUI.previousScore) * 100;
            if (percent % gameSceneUI.showAlarm == 0)
            {
                gameSceneUI.alarmShow.SetActive(true);
                gameSceneUI.alarmAnimator.SetBool("isActive", true);
                gameSceneUI.newsTitle.text = $"지금 삼성전자 가격이 {percent:N2}% 상승했어요";
                gameSceneUI.newsDescribe.text = $"금일 장 중 최고가를 기록했어요. ({score:N2}원)";
                CancelInvoke("AlarmOff");
                Invoke("AlarmOff", 10);
            }
        }
        else
        {
            float percent = ((score - GameSceneUI.startPercent) / GameSceneUI.startPercent) * 100;
            if (percent % gameSceneUI.showAlarm == 0)
            {
                gameSceneUI.alarmShow.SetActive(true);
                gameSceneUI.alarmAnimator.SetBool("isActive", true);
                gameSceneUI.newsTitle.text = $"정규장 개시! 시초가 ({score:N2}원) 형성";
                gameSceneUI.newsDescribe.text = $"금일 장 중 최고가를 기록했어요. ({score:N2}원)";
                gameSceneUI.previousScore = score;
                CancelInvoke("AlarmOff");
                Invoke("AlarmOff", 10);
            }
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
        gameSceneUI.currentEndScore.text = 0.ToString();
        gameSceneUI.highestEndScore.text = 0.ToString();//점수넣는 자리
        gameSceneUI.endUI.SetActive(true);
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

}
