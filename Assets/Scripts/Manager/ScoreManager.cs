using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager 
{
    private int score;

    public int bestScore;
    //스코어 매니저 초기화 로직
    public int bonusScore;
    public int totalScore;
    public int combo;

    public void Update()
    {
        CalculateScore();
    }
    
    public void Init()
    {
        Debug.Log("ScoreManage initialized)");
        score = 0;
        combo = 0;
        bonusScore = 500;
        totalScore = 0;
    }
    
    //MainTitle 이전 LoadingScene
    private void ReadyMain()
    {
        Debug.Log("ScoreManage - 최고점수 로드");
    }

    private void CalculateScore()
    {
        int sc = (int)(ManagerRoot.gameManager.player.transform.position.z * 100);
        int calcBonus = (bonusScore * combo);
        score = sc;
        totalScore =score + calcBonus;
        Debug.Log("calcBonus" + calcBonus);
    }

    public void SetBestScore()
    {
        //최고점수 저장
        if (!PlayerPrefs.HasKey("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", totalScore);
            Debug.Log("Bestsco 처음 저장"+totalScore);
        }
        else  if(totalScore > bestScore)
        {
            bestScore =totalScore;
            PlayerPrefs.SetInt("BestScore", totalScore);
            Debug.Log("Bestscor 저장"+bestScore);
        }
    }
    
    public void Reset()
    {
        score = 0;
        totalScore = 0;
        bonusScore = 0;
        combo = 0;
    }
    
    
}
