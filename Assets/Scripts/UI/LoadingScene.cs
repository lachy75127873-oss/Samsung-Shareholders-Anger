using UnityEngine;
using UnityEngine.UI;


public class LoadingScene : MonoBehaviour
{
    internal static string nextScene;
    /// <summary>
    /// 로딩바
    /// </summary>
    [Tooltip("로딩바")]
    [SerializeField] internal Image loadingBar;
    /// <summary>
    /// 로딩씬으로 넘어오면 로딩바를 실행함.
    /// </summary>
    private void Awake()
    {
        if (UiManager.Instance != null)
        {
            UiManager.Instance.InputloadingScene(this);
        }
    }

    private void Start()
    {
        Debug.Log("[LoadingScene] 로딩 씬 시작");

        // SceneController의 로딩 프로세스 시작 (저장된 targetState 사용)
        if (ManagerRoot.Instance?.sceneController != null)
        {
            ManagerRoot.Instance.sceneController.OnLoadingScene();
        }
        else
        {
            Debug.LogError("SceneController를 찾을 수 없습니다!");
        }
    }
}
