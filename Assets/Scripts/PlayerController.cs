using UnityEditor.Rendering.Analytics;
using UnityEngine;

public class PlayerController : UnitController
{
    [SerializeField] private int life;
    
    private void Update()
    {
        if (!GameManager.Instance.IsPlayerTurn()) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryMoveToClickedNode();
        }
    }

    private bool TryMoveToClickedNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Node targetNode = hit.collider.GetComponent<Node>();
            if (TryMoveTo(targetNode))
            {
                GameManager.Instance.EndPlayerTurn();
                return true;
            }
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }
}
