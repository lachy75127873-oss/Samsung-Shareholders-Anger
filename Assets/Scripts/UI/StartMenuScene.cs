using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScene : MonoBehaviour
{
    /// <summary>
    /// 시작버튼
    /// </summary>
    [Tooltip("시작버튼")]
    [SerializeField] Button startButton;
    /// <summary>
    /// 게임씬의 이름을 써주세요.
    /// </summary>
    [Tooltip("게임씬의 이름을 써주세요.")]
    [SerializeField] string gameSceneName;
    /// <summary>
    /// 옵션버튼
    /// </summary>
    [Tooltip("옵션버튼")]
    [SerializeField] Button optionButton;
    /// <summary>
    /// 옵션 UI
    /// </summary>
    [Tooltip("옵션 UI")]
    [SerializeField] GameObject optionUI;
    /// <summary>
    /// 옵션 나가기 버튼
    /// </summary>
    [Tooltip("옵션 나가기 버튼")]
    [SerializeField] Button optionExitButton;
    /// <summary>
    /// 마지막 스코어
    /// </summary>
    [Tooltip("마지막 스코어")]
    [SerializeField] Text lastScore;
    /// <summary>
    /// 최고점수
    /// </summary>
    [Tooltip("최고점수")]
    [SerializeField] Text highestScore;
    /// <summary>
    /// 전판 점수 대비 퍼센트
    /// </summary>
    [Tooltip("전판 점수 대비 퍼센트")]
    [SerializeField] Text percentScore;
    /// <summary>
    /// 초기 세팅, 버튼에 함수 등록 + 스코어 출력
    /// </summary>
    private void Awake()
    {
        lastScore.text = 0.ToString();
        highestScore.text = 0.ToString();
        percentScore.text = $"{0.ToString()}%";//임시임. 스코어 매니저랑 상의해야 됨.
        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(TurnOption);
        optionExitButton.onClick.AddListener(ExitOption);
    }

    /// <summary>
    /// 게임 시작버튼을 누르면 게임씬으로 이동함.
    /// </summary>
    void StartGame()
    {
        ManagerRoot.Instance.sceneController.OnLoadingScene(ScreenState.Main_Game);
    }
    /// <summary>
    /// 옵션 버튼을 누르면 옵션창이 뜸.
    /// </summary>
    void TurnOption()
    { optionUI.SetActive(true); }
    /// <summary>
    /// 옵션창에서 나가기 누르면 옵션창이 꺼짐.
    /// </summary>
    void ExitOption()
    { optionUI.SetActive(false); }

    private void OnDestroy()
    {
        startButton.onClick.RemoveAllListeners();
        optionButton.onClick.RemoveAllListeners();
        optionExitButton.onClick.RemoveAllListeners();
    }
}
