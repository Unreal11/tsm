namespace Core.Castle
{
    public class Wall
    {
        public int MaxHealth { get; set; }
        public int Health { get; set; }

        public Wall() : this(50, 10)
        {
        }

        public Wall(int maxHealth, int health)
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