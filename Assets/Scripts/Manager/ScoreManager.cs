using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score;
    private int accumulatedBonus; // 누적 보너스 저장!
    public int bestScore;
    //스코어 매니저 초기화 로직
    public int bonusScore;
    public int totalScore;
    public int combo;
    public int lastScore;

    public int GetCurrentScore() => score;
    public int GetTotalScore() => totalScore;
    public int GetCombo() => combo;
    public int GetLastScore() => lastScore;

    public void Update()
    {
        if (ManagerRoot.gameManager?.player != null)
        {
            CalculateScore();
            UpdateUI();
        }
    }

    public void Init()
    {
        Debug.Log("ScoreManage initialized)");
        score = 0;
        combo = 0;
        bonusScore = 500;
        totalScore = 0;
        accumulatedBonus = 0;

        LoadBestScore();
    }
    
    //MainTitle 이전 LoadingScene
    private void ReadyMain()
    {
        Debug.Log("ScoreManage - 최고점수 로드");
    }

    private void LoadBestScore()
    {
        if (PlayerPrefs.HasKey("BestScore"))
        {
            bestScore = PlayerPrefs.GetInt("BestScore");
            Debug.Log($"최고 점수 로드: {bestScore}");
        }
        else
        {
            bestScore = 0;
        }
    }

    private void CalculateScore()
    {
        score = (int)(ManagerRoot.gameManager.player.transform.position.z * 100);
        totalScore = score + accumulatedBonus;
    }

    
    public void Clear()
    {
        score = 0;
        totalScore = 0;
        bonusScore = 0;
        combo = 0;
    }

    #region 점수 UI업데이트
    private void UpdateUI()
    {
        // UiManager를 통해 GameSceneUI 업데이트
        if (UiManager.Instance != null)
        {
            UiManager.Instance.InputScore(totalScore);
        }
    }

    public void AddComboBonus()
    {
        combo++;

        // 현재 콤보 * 500을 누적!
        int currentBonus = combo * bonusScore;
        accumulatedBonus += currentBonus;

        UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        totalScore = score + accumulatedBonus;
        UpdateUI();
    }

    public void SetBestScore()
    {
        if (!PlayerPrefs.HasKey("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", totalScore);
            bestScore = totalScore;
            Debug.Log($"최고 점수 첫 저장: {totalScore}");
        }
        else if (totalScore > bestScore)
        {
            bestScore = totalScore;
            PlayerPrefs.SetInt("BestScore", totalScore);
            Debug.Log($"최고 점수 갱신: {bestScore}");
        }

        PlayerPrefs.Save();
    }
    #endregion
}
