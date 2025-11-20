using UnityEngine;
using UnityEngine.UI;
public class GameSceneUI : MonoBehaviour
{
    [Header("스코어 UI")]
    [SerializeField] private GameObject scoreUI;

    [Header("주가 표시창")]
    /// <summary>
    /// 현재 주가를 띄우는 텍스트
    /// </summary>
    [Tooltip("현재 주가를 띄우는 텍스트")]
    [SerializeField] internal Text currentScore;
    /// <summary>
    /// 이전 스코어를 저장하는 변수.
    /// </summary>
    internal float previousScore = 0;
    /// <summary>
    /// 상승 하강 아이콘 위치
    /// </summary>
    [Tooltip("상승 하강 아이콘 위치")]
    [SerializeField] internal GameObject icon;
    /// <summary>
    /// 전일보다 주가가 낮을때 뜨는 아이콘
    /// </summary>
    [Tooltip("최고가 보다 주가가 낮을때 뜨는 아이콘")]
    [SerializeField] internal Sprite lowerIcon;
    /// <summary>
    /// 전일보다 주가가 높을때 뜨는 아이콘
    /// </summary>
    [Tooltip("최고가 보다 주가가 높을때 뜨는 아이콘")]
    [SerializeField] internal Sprite higherIcon;
    /// <summary>
    /// 전일대비 퍼센트 표시하는 텍스트
    /// </summary>
    [Tooltip("최고가 대비 퍼센트 표시하는 텍스트")]
    [SerializeField] internal Text currentPercent;
    /// <summary>
    /// 옵션 재생버튼
    /// </summary>
    [Tooltip("옵션 재생버튼")]
    [SerializeField] internal Button OptionButton;
    [Header("알림 창")]
    /// <summary>
    /// 알람을 보여주는 퍼센트 수치
    /// </summary>
    [Tooltip("알람을 보여주는 퍼센트 수치")]
    [SerializeField] internal float showAlarm = 1.5f;
    /// <summary>
    /// 주가 상승시 알람이 뜨는 창
    /// </summary>
    [Tooltip("주가 상승시 알람이 뜨는 창")]
    [SerializeField] internal GameObject alarmShow;
    /// <summary>
    /// 창 표시 애니메이터
    /// </summary>
    [Tooltip("창 표시 애니메이터")]
    [SerializeField] internal Animator alarmAnimator;
    /// <summary>
    /// 뉴스 타이틀이 표시되는 텍스트
    /// </summary>
    [Tooltip("뉴스 타이틀이 표시되는 텍스트")]
    [SerializeField] internal Text newsTitle;
    /// <summary>
    /// 뉴스 내용이 표시되는 텍스트
    /// </summary>
    [Tooltip("뉴스 내용이 표시되는 텍스트")]
    [SerializeField] internal Text newsDescribe;
    [Header("아이템 습득 창")]
    /// <summary>
    /// 아이템 창
    /// </summary>
    [Tooltip("아이템 창")]
    [SerializeField] internal GameObject itemUIPrefeb;
    /// <summary>
    /// 아이템 창 2
    /// </summary>
    [Tooltip("아이템 창 2")]
    [SerializeField] internal GameObject itemUIPrefeb2;
    /// <summary>
    /// 종료 UI
    /// </summary>
    [Tooltip("종료 UI")]
    [SerializeField] internal GameObject endUI;
    [SerializeField] internal Image endIcon;
    [SerializeField] internal Sprite highIcon;
    [SerializeField] internal Sprite lowIcon;
    [SerializeField] internal Text currentEndScore;
    [SerializeField] internal Text highestEndScore;
    [SerializeField] internal Text EndPercent;
    [SerializeField] internal Button goToMain;
    [SerializeField] internal Button restart;
    [SerializeField] internal string mainSceneName;
    [SerializeField] internal string gameSceneName;
    [SerializeField] internal GameObject EndNews1;
    [SerializeField] internal GameObject EndNews2;
    [SerializeField] internal GameObject EndNews3;
    [SerializeField] internal GameObject EndNews4;
    /// <summary>
    /// 퍼센트 수치를 재기 시작하는 최소값
    /// </summary>
    internal const byte startPercent = 1;
    /// <summary>
    /// 시작시 기본세팅
    /// </summary>
    internal bool firstItem = false;
    internal bool secondItem = false;

    private void Awake()
    {
        UiManager.Instance.InputGameMenu(this);

        // 초기화
        if (currentScore != null)
            currentScore.text = "0";

        if (currentPercent != null)
            currentPercent.text = "0%";

        if (alarmShow != null)
            alarmShow.SetActive(false);

        if (endUI != null)
            endUI.SetActive(false);

        if (EndPercent != null)
            EndPercent.text = "0%";
        // 버튼 이벤트
        if (goToMain != null)
            goToMain.onClick.AddListener(UiManager.Instance.GoToMenu);

        if (restart != null)
            restart.onClick.AddListener(UiManager.Instance.Restart);

        previousScore = 0;
    }

    public void ShowGameOver(int currentScore, int bestScore)
    {
        Debug.Log($"게임 오버 - 현재: {currentScore}, 최고: {bestScore}");
        scoreUI.SetActive(false);
        // 점수 표시
        if (currentEndScore != null)
        {
            currentEndScore.text = currentScore.ToString("N0");
        }

        if (highestEndScore != null)
        {
            highestEndScore.text = bestScore.ToString("N0");
        }
        if (EndPercent != null)
        {
            float floCurScore = currentScore;
            float floBestScore = bestScore;
            float floPercent = currentScore / bestScore;
            byte roundPercent = (byte)Mathf.Round(floPercent * 100);
            if (roundPercent <= 0)
            {
                endIcon.sprite = lowerIcon;
                EndPercent.color = Color.red;
            }
            else
            {
                endIcon.sprite = higherIcon;
                EndPercent.color = Color.green;
            }
            EndPercent.text = $"{roundPercent.ToString("N0")}%";
            if (roundPercent < 50)
            {
                EndNews1.SetActive(true);
                EndNews2.SetActive(false);
                EndNews3.SetActive(false);
                EndNews4.SetActive(false);
            }
            else if (roundPercent > 50 && roundPercent < 80)
            {
                EndNews1.SetActive(false);
                EndNews2.SetActive(true);
                EndNews3.SetActive(false);
                EndNews4.SetActive(false);
            }
            else if (roundPercent > 80 && roundPercent < 100)
            {
                EndNews1.SetActive(false);
                EndNews2.SetActive(false);
                EndNews3.SetActive(true);
                EndNews4.SetActive(false);
            }
            else if (roundPercent < 100)
            {
                EndNews1.SetActive(false);
                EndNews2.SetActive(false);
                EndNews3.SetActive(false);
                EndNews4.SetActive(true);
            }
        }

        // 게임 오버 UI 활성화
        if (endUI != null)
        {
            endUI.SetActive(true);
        }
    }

    /// <summary>
    /// 아이템 습득시 UI를 띄우며 쿨타임 시작.
    /// </summary>
    private void OnDestroy()
    {
        goToMain.onClick.RemoveAllListeners();
        restart.onClick.RemoveAllListeners();
    }
}
