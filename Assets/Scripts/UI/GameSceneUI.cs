using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
    [Header("주가 표시창")]
    /// <summary>
    /// 현재 주가를 띄우는 텍스트
    /// </summary>
    [Tooltip("현재 주가를 띄우는 텍스트")]
    [SerializeField] Text currentScore;
    /// <summary>
    /// 이전 스코어를 저장하는 변수.
    /// </summary>
    float previousScore = 0;
    /// <summary>
    /// 상승 하강 아이콘 위치
    /// </summary>
    [Tooltip("상승 하강 아이콘 위치")]
    [SerializeField] GameObject icon;
    /// <summary>
    /// 전일보다 주가가 낮을때 뜨는 아이콘
    /// </summary>
    [Tooltip("전일보다 주가가 낮을때 뜨는 아이콘")]
    [SerializeField] Sprite lowerIcon;
    /// <summary>
    /// 전일보다 주가가 높을때 뜨는 아이콘
    /// </summary>
    [Tooltip("전일보다 주가가 높을때 뜨는 아이콘")]
    [SerializeField] Sprite higherIcon;
    /// <summary>
    /// 전일대비 퍼센트 표시하는 텍스트
    /// </summary>
    [Tooltip("전일대비 퍼센트 표시하는 텍스트")]
    [SerializeField] Text currentPercent;
    [Header("알림 창")]
    /// <summary>
    /// 알람을 보여주는 퍼센트 수치
    /// </summary>
    [Tooltip("알람을 보여주는 퍼센트 수치")]
    [SerializeField] float showAlarm = 1.5f;
    /// <summary>
    /// 주가 상승시 알람이 뜨는 창
    /// </summary>
    [Tooltip("주가 상승시 알람이 뜨는 창")]
    [SerializeField] GameObject alarmShow;
    /// <summary>
    /// 창 표시 애니메이터
    /// </summary>
    [Tooltip("창 표시 애니메이터")]
    [SerializeField] Animator alarmAnimator;
    /// <summary>
    /// 뉴스 타이틀이 표시되는 텍스트
    /// </summary>
    [Tooltip("뉴스 타이틀이 표시되는 텍스트")]
    [SerializeField] Text newsTitle;
    /// <summary>
    /// 뉴스 내용이 표시되는 텍스트
    /// </summary>
    [Tooltip("뉴스 내용이 표시되는 텍스트")]
    [SerializeField] Text newsDescribe;
    [Header("아이템 습득 창")]
    /// <summary>
    /// 아이템 창
    /// </summary>
    [Tooltip("아이템 창")]
    [SerializeField] GameObject itemShow;
    /// <summary>
    /// 아이템 아이콘이 표시되는 위치
    /// </summary>
    [Tooltip("아이템 아이콘이 표시되는 위치")]
    [SerializeField] GameObject itemIcon;
    /// <summary>
    /// 아이템 쿨타임용 바
    /// </summary>
    [Tooltip("아이템 쿨타임용 바")]
    [SerializeField] Image itemCooltime;
    /// <summary>
    /// 아이템 이름이 표시되는 텍스트
    /// </summary>
    [Tooltip("아이템 이름이 표시되는 텍스트")]
    [SerializeField] Text itemName;
    /// <summary>
    /// 아이템 설명이 표시되는 텍스트
    /// </summary>
    [Tooltip("아이템 설명이 표시되는 텍스트")]
    [SerializeField] Text itemDescribe;
    /// <summary>
    /// 아이템 아이콘 1
    /// </summary>
    [Tooltip("아이템 아이콘 1")]
    [SerializeField] Sprite itemIcon1;
    [Tooltip("종료 UI")]
    [SerializeField] GameObject endUI;
    [SerializeField] Text currentEndScore;
    [SerializeField] Text highestEndScore;
    [SerializeField] Button goToMain;
    [SerializeField] Button restart;
    [SerializeField] string mainSceneName;
    [SerializeField] string gameSceneName;
    /// <summary>
    /// 퍼센트 수치를 재기 시작하는 최소값
    /// </summary>
    const byte startPercent = 1;
    /// <summary>
    /// 시작시 기본세팅
    /// </summary>
    private void Awake()
    {
        currentScore.text = 0f.ToString();
        currentPercent.text = $"{0f.ToString()}%";//수정해야 됨.
        alarmShow.SetActive(false);
        itemShow.SetActive(false);
        goToMain.onClick.AddListener(GoToMenu);
        restart.onClick.AddListener(Restart);
        previousScore = 0;//이전 회차의 점수를 넣어야 함
    }
    /// <summary>
    /// score에 넣은 값이 현재 주가로 표시됨.
    /// </summary>
    /// <param name="score"></param>
    internal void InputScore(float score)//점수가 어디있는지 물어서 세팅해야 됨.
    {
        currentScore.text = score.ToString("N2");
        if (previousScore > 0)
        {
            float percent = ((score - previousScore) / previousScore) * 100;
            if (percent % showAlarm == 0)
            {
                alarmShow.SetActive(true);
                alarmAnimator.SetBool("isActive", true);
                newsTitle.text = $"지금 삼성전자 가격이 {percent:N2}% 상승했어요";
                newsDescribe.text = $"금일 장 중 최고가를 기록했어요. ({score:N2}원)";
                CancelInvoke("AlarmOff");
                Invoke("AlarmOff", 10);
            }
        }
        else
        {
            float percent = ((score - startPercent) / startPercent) * 100;
            if (percent % showAlarm == 0)
            {
                alarmShow.SetActive(true);
                alarmAnimator.SetBool("isActive", true);
                newsTitle.text = $"정규장 개시! 시초가 ({score:N2}원) 형성";
                newsDescribe.text = $"금일 장 중 최고가를 기록했어요. ({score:N2}원)";
                previousScore = score;
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
        alarmAnimator.SetBool("isActive", false);
        alarmShow.SetActive(false);
    }
    void EndGame()
    {
        currentEndScore.text = 0.ToString();
        highestEndScore.text = 0.ToString();//점수넣는 자리
        endUI.SetActive(true);
    }
    void GoToMenu()
    { LoadingScene.LoadScene(mainSceneName); }
    void Restart()
    { LoadingScene.LoadScene(gameSceneName); }
    /// <summary>
    /// 아이템 습득시 UI를 띄우며 쿨타임 시작.
    /// </summary>
    internal void GetItem()//아이템 종류는 어떻게 구별하지
    { 
        itemShow.SetActive(true);
        itemName.text = string.Empty;
        itemDescribe.text = string.Empty;
        itemCooltime.fillAmount = 0f;


    }
}
