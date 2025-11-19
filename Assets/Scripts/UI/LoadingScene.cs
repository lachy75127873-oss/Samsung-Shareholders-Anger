using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    /// <summary>
    /// 로딩바
    /// </summary>
    [Tooltip("로딩바")]
    [SerializeField] Image loadingBar;
    /// <summary>
    /// 씬 이동시 호출하면 로딩하며 sceneName의 씬으로 이동함.
    /// </summary>
    /// <param name="sceneName"></param>
    internal void LoadScene(string sceneName)
    { StartCoroutine(LoadingAsync(sceneName)); }
    IEnumerator LoadingAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = true;
        while (!asyncOperation.isDone)
        {
            loadingBar.fillAmount = asyncOperation.progress;
            yield return null;
        }
    }
}
