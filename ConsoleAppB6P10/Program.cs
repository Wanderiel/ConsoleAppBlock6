//GlobalUsing
//Net v.6

namespace ConsoleAppB6P10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Unit barbarian = new Unit("Варвар", 13, new Health(33), new Weapon(new DamageTripleCritical(2, 12)));
            Unit warrior = new Unit("Воин", 16, new Health(35), new Weapon(new Damage(1, 8), 19));

            while (barbarian.Health > 0 && warrior.Health > 0)
            {
                Console.WriteLine($"{barbarian.Health} | {warrior.Health}");

                Thread.Sleep(2000);

                barbarian.Attack(warrior);
                warrior.Attack(barbarian);
            }

            Console.WriteLine($"{barbarian.Health} | {warrior.Health}");

            Console.ReadKey();
        }
    }

    #region Interfaces

    //public interface IDamage
    //{
    //    int Get();

    //    int GetCritical();
    //}

    //public interface IWeapon
    //{
    //    public void Hit(Unit target);

    //    public void HitCritical(Unit target);
    //}

    #endregion

    public class Config
    {
        private Config(int value) 
        {
            Value = value;
        }

        public int Value { get; }

        public static Config CreateOne() => new Config(1);
    }

    public static class Randomize
    {
        private static Random _random = new Random();

        public static int GetNumber(int max) =>
            _random.Next(max) + 1;

        public static int GetNumber(int min, int max) =>
            _random.Next(min, max + 1);
    }

    public class Damage
    {
        public Damage(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; }

        public int Max { get; }

        public int Get() => Randomize.GetNumber(Min, Max);

        public virtual int GetCritical() => Max + Get();
    }

    public class DamageTripleCritical : Damage
    {
        public DamageTripleCritical(int min, int max) : base(min, max)
        {
        }

        public override int GetCritical() => Max + base.GetCritical();
    }

    public class Health
    {
        private int _max;
        private int _currentValue;

        public Health(int maxHealth)
        {
            _max = maxHealth;
            _currentValue = Max;
        }

        public int Max => _max;
        public int CurrentValue
        {
            get => _currentValue;
            private set => _currentValue = Math.Clamp(value, 0, Max);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
                return;

            CurrentValue -= damage;
        }

        public void Heal(int heal)
        {
            if (heal <= 0)
                return;

            CurrentValue += heal;
        }
    }

    public class Weapon
    {
        private readonly int _baseAttack;
        private readonly int _criticalThreshold;
        private readonly Damage _damage;

        public Weapon(Damage damage, int criticalThreshold = 20)
        {
            _damage = damage;
            _baseAttack = 20;
            _criticalThreshold = Math.Min(criticalThreshold, _baseAttack);
        }

        public void Attack(Unit target)
        {
            int currentAttack = Randomize.GetNumber(_baseAttack);

            if (currentAttack == _baseAttack)
            {
                HitCritical(target);

                return;
            }

            if (currentAttack < target.Armor)
            {
                Console.WriteLine($"Промах: {target.Name} не получает урона");

                return;
            }

            if (currentAttack < _criticalThreshold)
            {
                HitCritical(target);

                return;
            }

            Hit(target);
        }

        private void Hit(Unit target)
        {
            int damage = _damage.Get();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            target.TakeDamage(damage);
        }

        private void HitCritical(Unit target)
        {
            int damage = _damage.GetCritical();
            Console.ForegroundColor = ConsoleColor.Red;
            target.TakeDamage(damage);
        }
    }

    public class Unit
    {
        private readonly Health _health;
        private readonly Weapon _weapon;

        public Unit(string name, int armor, Health health, Weapon weapon)
        {
            Name = name;
            Armor = armor;
            _health = health;
            _weapon = weapon;
        }

        public string Name { get; }
        public int Armor { get; }
        public int Health => _health.CurrentValue;

        public void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);

            Console.WriteLine($"{Name} получает урон {damage}");
            Console.ResetColor();
        }

        //public void TakeHeal(int heal) => _health.Heal(heal);

        public void Attack(Unit target)
        {
            Console.WriteLine($"{Name} атакует {target.Name}");

            _weapon.Attack(target);
        }
    }

    public class UnitsFabric
    {

    }

    public class Country
    {
        private List<Unit> _units;

        public Country(string name)
        {
            _units = new List<Unit>();
            Name = name;
        }

        public string Name { get; }

        public void Add(Unit unit) => _units.Add(unit);
    }
}