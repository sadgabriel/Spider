using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [SerializeField] protected float verticalOffset = 0.5f;

    private Node currentNode;

    public Node CurrentNode
    {
        get => currentNode;
        protected set => currentNode = value;
    }

    public int ExpAmount { get; set; } = 10;

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
