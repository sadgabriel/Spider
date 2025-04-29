public interface IUnit
{
    Node CurrentNode { get; }
    bool TryMoveTo(Node targetNode);
    bool CanMoveTo(Node targetNode);
}