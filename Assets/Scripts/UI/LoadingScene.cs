using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.PackageManager;

public class LoadingScene : MonoBehaviour
{
    static string nextScene;
    /// <summary>
    /// 로딩바
    /// </summary>
    [Tooltip("로딩바")]
    [SerializeField] Image loadingBar;
    /// <summary>
    /// 로딩씬으로 넘어오면 로딩바를 실행함.
    /// </summary>
    private void Start()
    { StartCoroutine(LoadSceneProcess()); }
    /// <summary>
    ///호출하면 로딩씬으로 넘어간 뒤 sceneName의 씬으로 이동함.
    /// </summary>
    /// <param name="sceneName"></param>
    internal static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }
    /// <summary>
    /// 로딩바를 코루틴으로 실행
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(1);
        op.allowSceneActivation = false;
        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;
            if (op.progress < 0.9f)
            { loadingBar.fillAmount = op.progress; }
            else
            {
                timer += Time.unscaledDeltaTime;
                loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (loadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
