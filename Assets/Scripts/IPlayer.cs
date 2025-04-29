public interface IPlayer : IUnit
{
    int Life { get; }
    void TakeDamage(int damage);
}