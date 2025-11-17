using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController
{
    //씬컨트롤러 초기화 로직
    public void Init() 
    {
        Debug.Log("SceneManager initialized");
        SubscribeSceneManager(); // 초기화 시 구독
    }

    private void SubscribeSceneManager()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void UnsubscribeSceneManager()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "TempCDW": //시험용 코드
                break;
            case "Loading":
                break;
            case "Main_Title":
                break;
            case "Main_Game":
                break;
            default:
                break;
        }
    }
    
    //"TempCDW"씬에서 실행 매서드
    private void ToDoCDW()
    {
        Debug.Log("ToDoCDW");
        SceneManager.LoadScene("Loading"); // 로딩씬으로 이동
    }
    
    //"Loading"Scene에서 실행 매서드
    private void OnLoadingScene()
    {
        Debug.Log("OnLoadingScene");
    }
    
    //"Main_Title"Scene에서 실행 매서드
    private void OnMainTitle()
    {
        Debug.Log("OnMainTitle");
    }

    //"Main_Game"Scene에서 실행 매서드
    private void OnMainGame()
    {
        Debug.Log("OnMainGame");
    }


}
