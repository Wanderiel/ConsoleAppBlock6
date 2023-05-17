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

    public interface IWeapon
    {
        public void Hit(Unit target);

        public void HitCritical(Unit target);
    }

    public interface IAttack
    {
        bool GetAttackResult(Defences defences, out bool critical);
    }

    #endregion

    //public class Config
    //{
    //    private Config(int value)
    //    {
    //        Value = value;
    //    }

    //    public int Value { get; }

    //    public static Config CreateOne() => new Config(1);
    //}

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

    public class Defences
    {
        private readonly int _defence;
        private int _armor;

        public Defences(int defence, int armor, int magicBarrier)
        {
            _defence = defence;
            _armor = armor;
            MagicBarrier = magicBarrier;
        }

        public int MagicBarrier { get; }

        public int CalculateArmorClass(int ignoreArmor = 0) =>
            Math.Clamp(_armor - ignoreArmor, 0, _armor) + _defence;
    }

    public class Attak : IAttack
    {
        private int _baseAttack = 20;
        private int _criticalThreshold;

        public Attak(int criticalThreshold = 20)
        {
            _criticalThreshold = criticalThreshold;
        }

        public bool GetAttackResult(Defences defences, out bool critical)
        {
            critical = false;
            int currentAttack = Randomize.GetNumber(_baseAttack);
            int armorClass = defences.CalculateArmorClass();

            if (currentAttack == _baseAttack)
            {
                critical = true;

                return true;
            }

            if (currentAttack < armorClass)
                return false;

            if (currentAttack >= _criticalThreshold)
                critical = true;

            return true;
        }
    }

    public class AttackIgnoreArmor : IAttack
    {
        private Attak _attak;
        private int _ignoreArmor;

        public AttackIgnoreArmor(Attak attak, int ignoreArmor)
        {
            _attak = attak;
            _ignoreArmor = ignoreArmor;
        }

        public bool GetAttackResult(Defences defences, out bool critical)
        {
            bool result = _attak.GetAttackResult(defences, out critical);
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

            if (currentAttack < target.Defences.CalculateArmorClass())
            {
                Console.WriteLine($"Промах: {target.Name} не получает урона");

                return;
            }

            if (currentAttack >= _criticalThreshold)
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

        public Unit(string name, int armor, Health health, Defences defences, Weapon weapon)
        {
            Name = name;
            //Armor = armor;
            _health = health;
            Defences = defences;
            _weapon = weapon;
        }

        public string Name { get; }
        //public int Armor { get; }
        public Defences Defences { get; }

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