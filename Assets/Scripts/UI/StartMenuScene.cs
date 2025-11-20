using UnityEngine;
using UnityEngine.UI;

public class StartMenuScene : MonoBehaviour
{
    [Tooltip("종료버튼")]
    [SerializeField] internal Button gameQuitButton;
    /// <summary>
    /// 시작버튼
    /// </summary>
    [Tooltip("시작버튼")]
    [SerializeField] internal Button startButton;
    /// <summary>
    /// 게임씬의 이름을 써주세요.
    /// </summary>
    [Tooltip("게임씬의 이름을 써주세요.")]
    [SerializeField] internal string gameSceneName;
    /// <summary>
    /// 옵션버튼
    /// </summary>
    [Tooltip("옵션버튼")]
    [SerializeField] internal Button optionButton;
    /// <summary>
    /// 옵션 UI
    /// </summary>
    [Tooltip("옵션 UI")]
    [SerializeField] internal GameObject optionUI;
    /// <summary>
    /// 옵션 나가기 버튼
    /// </summary>
    [Tooltip("옵션 나가기 버튼")]
    [SerializeField] internal Button optionExitButton;
    /// <summary>
    /// 마지막 스코어
    /// </summary>
    [Tooltip("마지막 스코어")]
    [SerializeField] internal Text lastScore;
    /// <summary>
    /// 최고점수
    /// </summary>
    [Tooltip("최고점수")]
    [SerializeField] internal Text highestScore;
    /// <summary>
    /// 전판 점수 대비 퍼센트
    /// </summary>
    [Tooltip("전판 점수 대비 퍼센트")]
    [SerializeField] internal Text percentScore;
    /// <summary>
    /// 초기 세팅, 버튼에 함수 등록 + 스코어 출력
    /// </summary>
    private void Start()
    {
        UiManager.Instance.InputStartMenu(this);
        lastScore.text = 0.ToString();
        highestScore.text = 0.ToString();
        percentScore.text = $"{0.ToString()}%";//임시임. 스코어 매니저랑 상의해야 됨.
        startButton.onClick.AddListener(UiManager.Instance.StartGame);
        optionButton.onClick.AddListener(UiManager.Instance.TurnOption);
        optionExitButton.onClick.AddListener(UiManager.Instance.ExitOption);
        gameQuitButton.onClick.AddListener(UiManager.Instance.QuitGame);
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveAllListeners();
        optionButton.onClick.RemoveAllListeners();
        optionExitButton.onClick.RemoveAllListeners();
        gameQuitButton.onClick.RemoveAllListeners();
    }
}
