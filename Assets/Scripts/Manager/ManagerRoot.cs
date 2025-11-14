using System;
using UnityEngine.UI;

public class ManagerRoot : Singleton<ManagerRoot>
{
    public static GameManager gameManager;

    protected override void Init() // 매니저들의 초기화 호출 순서 조절
    {
        gameManager = new GameManager();

        gameManager.Init();
    }
}
