namespace Core.Castle
{
    public class Tower
    {
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }

        public Tower() : this(50, 20)
        {
        }

        public Tower(int maxHealth, int health)
        {
            MaxHealth = maxHealth;
            Health = health;
        }

        public void Damage(int value)
        {
            Health -= value;

            if (Health < 0)
                Health = 0;
        }

        public void Heal(int value)
        {
            Health += value;

            if (Health > MaxHealth)
                Health = MaxHealth;
        }
    }
}