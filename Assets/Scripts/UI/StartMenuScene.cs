using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    /// 로딩씬 스크립트 넣는 곳
    /// </summary>
    [SerializeField]
    LoadingScene loadingScene;//싱글턴으로 로딩씬 처리해야 됨
    private void Awake()
    {
        lastScore.text = 0.ToString();
        highestScore.text = 0.ToString();
        percentScore.text = $"{0.ToString()}%";//임시임. 스코어 매니저랑 상의해야 됨.
        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(TurnOption);
        optionExitButton.onClick.AddListener(ExitOption);
    }
    void StartGame()
    { loadingScene.LoadScene(gameSceneName); }
    void TurnOption()
    { optionUI.SetActive(true); }
    void ExitOption()
    { optionUI.SetActive(false); }
}
