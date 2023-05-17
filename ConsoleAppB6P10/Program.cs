//GlobalUsing
//Net v.6

namespace ConsoleAppB6P10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Unit barbarian = new Unit(
                "Варвар",
                new Health(33),
                new DefensiveStats(12, 1, 15),
                new Weapon(
                    new DamageTripleCritical(2, 12),
                    new PhysicalAttak()
                    )
                );

            Unit warrior = new Unit(
                "Воин",
                new Health(35),
                new DefensiveStats(10, 6, 12),
                new Weapon(
                    new Damage(1, 8),
                    new PhysicalAttak(19)
                    )
                );

            while (barbarian.Health > 0 && warrior.Health > 0)
            {
                Console.WriteLine($"\n{barbarian.Name} [{barbarian.Health}] | {warrior.Name} [{warrior.Health}]");

                Thread.Sleep(2000);

                barbarian.Attack(warrior);
                warrior.Attack(barbarian);
            }

            Console.WriteLine($"\n{barbarian.Name} [{barbarian.Health}] | {warrior.Name} [{warrior.Health}]");

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

    public interface IAttack
    {
        bool GetResult(DefensiveStats defences, out bool isCritical);
    }

    #endregion

    public static class Randomize
    {
        private static Random _random = new Random();

        public static int GetNumber(int max) =>
            _random.Next(max);

        public static int GetNumber(int min, int max) =>
            _random.Next(min, max + 1);
    }

    public class Config
    {
        private List<string> _names = new List<string>()
        {
            "Клев",
            "Мару",
            "Сива",
            "Лори",
            "Ныть",
            "Субо",
            "Зерт",
            "Утер",
            "Фенк",
            "Ярго",
            "Хирд",
            "Сува",
            "Эрли",
            "Чива",
            "Меда",
        };

        private List<Damage> _damages = new List<Damage>()
        {
            new Damage(1, 6),
            new Damage(1, 8),
            new Damage(2, 4),
            new Damage(1, 10),
            new Damage(2, 12),
        };

        List<IAttack> _attacks = new List<IAttack>()
        {
            new PhysicalAttak(18, 1),
            new PhysicalAttak(19),
            new PhysicalAttak(ignoreArmor: 1),
            new PhysicalAttak(),
            new MagicAttack(),
        };

        private List<DefensiveStats> _defensiveStats = new List<DefensiveStats>()
        {
            new DefensiveStats(10 , 4, 15),
            new DefensiveStats(10 , 5, 14),
            new DefensiveStats(10 , 6, 13),
            new DefensiveStats(11 , 3, 15),
            new DefensiveStats(11 , 4, 14),
            new DefensiveStats(12 , 1, 16),
            new DefensiveStats(12 , 2, 15),
            new DefensiveStats(12 , 3, 14),
            new DefensiveStats(13 , 1, 15),
            new DefensiveStats(13 , 2, 14),
        };

        public List<string> Names => new List<string>(_names);
        public List<Damage> Damages => new List<Damage>(_damages);
        public List<IAttack> Attacks => new List<IAttack>(_attacks);
        public List<DefensiveStats> DefensiveStats => new List<DefensiveStats>(_defensiveStats);
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

    public class DefensiveStats
    {
        private readonly int _defence;
        private int _armor;

        public DefensiveStats(int defence, int armor, int magicBarrier)
        {
            _defence = defence;
            _armor = armor;
            MagicBarrier = magicBarrier;
        }

        public int MagicBarrier { get; }

        public int CalculateArmorClass(int ignoreArmor) =>
            Math.Clamp(_armor - ignoreArmor, 0, _armor) + _defence;
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

    public class PhysicalAttak : IAttack
    {
        private int _baseAttack = 20;
        private int _criticalThreshold;
        private int _ignoreArmor;

        public PhysicalAttak(int criticalThreshold = 20, int ignoreArmor = 0)
        {
            _criticalThreshold = criticalThreshold;
            _ignoreArmor = ignoreArmor;
        }

        public bool GetResult(DefensiveStats defences, out bool isCritical)
        {
            isCritical = false;
            int currentAttack = Randomize.GetNumber(_baseAttack) + 1;
            int armorClass = defences.CalculateArmorClass(_ignoreArmor);

            Console.WriteLine($" ({currentAttack})");

            if (currentAttack == _baseAttack)
            {
                isCritical = true;

                return true;
            }

            if (currentAttack < armorClass)
                return false;

            if (currentAttack >= _criticalThreshold)
                isCritical = true;

            return true;
        }
    }

    public class MagicAttack : IAttack
    {
        private int _baseAttack = 20;

        public bool GetResult(DefensiveStats defences, out bool isCritical)
        {
            isCritical = false;
            int currentAttack = Randomize.GetNumber(_baseAttack) + 1;

            Console.WriteLine($" ({currentAttack})");

            if (currentAttack == _baseAttack)
            {
                isCritical = true;

                return true;
            }

            if (currentAttack < defences.MagicBarrier)
                return false;

            return true;
        }
    }

    public class Weapon
    {
        private readonly Damage _damage;
        private readonly IAttack _atack;

        public Weapon(Damage damage, IAttack atack)
        {
            _damage = damage;
            _atack = atack;
        }

        public void Attack(Unit target)
        {
            if (_atack.GetResult(target.DefensiveStats, out bool isCritical))
                if (isCritical)
                    HitCritical(target);
                else
                    Hit(target);

            Console.WriteLine($"Промах: {target.Name} не получает урона");
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

        public Unit(string name, Health health, DefensiveStats defensiveStats, Weapon weapon)
        {
            Name = name;
            _health = health;
            DefensiveStats = defensiveStats;
            _weapon = weapon;
        }

        public string Name { get; }
        public DefensiveStats DefensiveStats { get; }

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
            Console.Write($"{Name} атакует {target.Name}");

            _weapon.Attack(target);
        }
    }

    public class UnitsFabric
    {
        private Config _config = new Config();

        public Unit Create()
        {
            string name = SetName();
            Health health = SetHealth();
            DefensiveStats defensiveStats = SetDefensiveStats();
            Weapon weapon = SetWeapon();

            return new Unit(name, health, defensiveStats, weapon);
        }


        private string SetName() => _config.Names[Randomize.GetNumber(_config.Names.Count)];

        private Health SetHealth()
        {
            int min = 50;
            int max = 60;
            int health = Randomize.GetNumber(min, max);

            return new Health(health);
        }

        private DefensiveStats SetDefensiveStats() =>
            _config.DefensiveStats[Randomize.GetNumber(_config.DefensiveStats.Count)];

        private Weapon SetWeapon()
        {
            Damage damage = _config.Damages[Randomize.GetNumber(_config.Damages.Count)];
            IAttack attack = _config.Attacks[Randomize.GetNumber(_config.Attacks.Count)];

            return new Weapon(damage, attack);
        }
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