using UnityEngine;

public class PlayerItemRootRange : MonoBehaviour
{
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField][Range(0, 10)] float magnetRadius;
    private float defaultRadius;

    private void Reset()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        defaultRadius = capsuleCollider.radius;
    }

    public void ApplyMagnetRange()
    {
        capsuleCollider.radius = magnetRadius;
    }
    public void ApplyDefaultRange()
    {
        capsuleCollider.radius = defaultRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}