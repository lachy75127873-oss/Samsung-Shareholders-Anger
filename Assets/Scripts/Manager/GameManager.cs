using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private bool isDead;
    public bool IsDead{get{return isDead;} set{isDead=value;}}
    
    public PlayerController player;
    //골드
    public int gold;
    //게임오버
    
    
    //게임 매니저 초기화 로직
    public void Init() 
    {
        Debug.Log("GameManager initialized");
    }
    
    //Main_Title 이전 로딩씬 
    public void ReadyMain() 
    {
        Debug.Log("GameManager - 중단된 게임 여부 체크");
    }

    private void ResetStage()
    {
        //Player초기화 매서드 - playerAwake에 자기 등록 
        //Map초기화 매서드
    }

    private void GameOver()
    {
        //플레이어 사망 판정 
        isDead = true;
        //ScoreManager에서 점수 갱신하고
        // UIManager에서 플레이어 사망 UI띄우고
        //그 다음 홈으로 또는 종료 버튼 누를 시 LoadingScene로드하게
    }
    
    
    //setPlayer 에서 플레이어를 찯도록 Maintitle로 갔을 때 찾아라
    //
    
    //registPlayer 매서드 필요
    public void RegisterPlayer(PlayerController _player)
    {
        if (player != null)
        {
            ResetPlayerData(); //추후
        }
        player = _player;

        if (ManagerRoot.itemEffectManager != null)
        {
            
        }
    }

    public void UnregisterPlayer()
    {
        player = null;
        ResetPlayerData();
    }

    private void ResetPlayerData()
    {
        
    }
    

}

