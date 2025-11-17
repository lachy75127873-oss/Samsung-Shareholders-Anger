using UnityEngine;

public interface IObstacle
{
    public void Init();
    public void Interact(GameObject player);
}
