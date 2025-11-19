using UnityEngine;

public class PlayerItemRootRange : MonoBehaviour
{
    [SerializeField] CapsuleCollider capsuleCollider;
    private float defaultRadius;

    private void Reset()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        defaultRadius = capsuleCollider.radius;
    }

    public void ApplyMagnetRange(float range)
    {
        capsuleCollider.radius = range;
        Debug.Log($"[Magnet] 자석 범위 활성화: {range}m");
    }

    public void ApplyDefaultRange()
    {
        capsuleCollider.radius = defaultRadius;
    }
}