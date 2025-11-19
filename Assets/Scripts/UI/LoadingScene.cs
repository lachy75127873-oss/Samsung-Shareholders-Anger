using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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
    private void Start()
    { StartCoroutine(UiManager.instance.LoadSceneProcess()); }
    /// <summary>
    ///호출하면 로딩씬으로 넘어간 뒤 sceneName의 씬으로 이동함.
    /// </summary>
    /// <param name="sceneName"></param>
    
}
