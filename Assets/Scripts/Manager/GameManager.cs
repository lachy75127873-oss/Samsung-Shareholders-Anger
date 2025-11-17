using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    //게임 매니저 초기화 로직
    public void Init() 
    {
        Debug.Log("GameManager initialized");
        
        TempSetReady(); 
    }
    
    //게임 매니저 초기화 로직
    private void TempSetReady() 
    {
        Debug.Log("기본 세팅 완료");
        
    }
    
}

