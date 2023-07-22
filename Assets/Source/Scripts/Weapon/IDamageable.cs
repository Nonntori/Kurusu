public interface IDamageable 
{
    public int CurrentHealth { get; }
    public int MaxHealth { get; }

    public delegate void Damaged(int damage);
    public event Damaged OnDamaged;

    public void TakeDamage(int damage);

}
