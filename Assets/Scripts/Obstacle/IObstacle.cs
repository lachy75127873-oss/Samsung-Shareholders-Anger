using UnityEngine;

public interface IObstacle
{
    /// <summary>
    /// 장애물 초기화
    /// </summary>
    public void Init();

    /// <summary>
    /// 플레이어와 충돌 시 
    /// </summary>
    public void Interact(GameObject player);
}
