using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [SerializeField] protected float verticalOffset = 0.5f;
    [SerializeField] protected AudioClip collectSound;

    private Node currentNode;
    public Node CurrentNode
    {
        get => currentNode;
        protected set => currentNode = value;
    }

    public int ExpAmount { get; set; } = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Initialize(Node currentNode, int expAmount)
    {
        CurrentNode = currentNode;
        transform.position = currentNode.TopPosition + currentNode.transform.up * verticalOffset;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, currentNode.DirectionFromOrigin);

        ExpAmount = expAmount;
    }

    public void Collect()
    {
        Player.Instance.GainExperience(ExpAmount);
        if (collectSound != null)
        {
            AudioManager.Instance.PlaySfx(collectSound, 0.5f);
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (CurrentNode != null)
        {
            CurrentNode.ExpOrb = null;
        }
    }
}
