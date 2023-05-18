//GlobalUsing
//Net v.6

namespace ConsoleAppB6P10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Unit barbarian = new Unit(
            //    "Варвар",
            //    33,
            //    new DefenceStats(12, 1, 15),
            //    new Weapon(
            //        new DamageTripleCritical(2, 12),
            //        new PhysicalAttack()
            //        )
            //    );

            //Unit warrior = new Unit(
            //    "Воин",
            //    35,
            //    new DefenceStats(10, 6, 12),
            //    new Weapon(
            //        new Damage(1, 8),
            //        new PhysicalAttack(19)
            //        )
            //    );

            //while (barbarian.Health > 0 && warrior.Health > 0)
            //{
            //    Console.WriteLine($"\n{barbarian.Name} [{barbarian.Health}] | {warrior.Name} [{warrior.Health}]");

            //    Thread.Sleep(2000);

            //    barbarian.Attack(warrior);
            //    warrior.Attack(barbarian);
            //}

            //Console.WriteLine($"\n{barbarian.Name} [{barbarian.Health}] | {warrior.Name} [{warrior.Health}]");

            PlatoonFactory platoonFactory = new PlatoonFactory();

            Platoon platoon1 = platoonFactory.Create("first", 10);
            Platoon platoon2 = platoonFactory.Create("sec", 10);

            while (platoon1.IsAlive && platoon2.IsAlive)
            {
                platoon2.TakeDamage(platoon1.GetNext());
                platoon1.TakeDamage(platoon2.GetNext());

                platoon1.BuryDead();
                platoon2.BuryDead();

                Thread.Sleep(500);
            }

            if (platoon1.IsAlive == platoon2.IsAlive)
                Console.WriteLine("Ничья");
            else if (platoon1.IsAlive)
                Console.WriteLine($"Победа за {platoon1.Name}");
            else
                Console.WriteLine($"Победа за {platoon2.Name}");

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
        bool HasHit(DefenceStats defences, out bool isCritical);
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
            new PhysicalAttack(18, 1),
            new PhysicalAttack(19),
            new PhysicalAttack(ignoreArmor: 1),
            new PhysicalAttack(),
            new MagicAttack(),
        };

        private List<DefenceStats> _defensiveStats = new List<DefenceStats>()
        {
            new DefenceStats(10 , 4, 15),
            new DefenceStats(10 , 5, 14),
            new DefenceStats(10 , 6, 13),
            new DefenceStats(11 , 3, 15),
            new DefenceStats(11 , 4, 14),
            new DefenceStats(12 , 1, 16),
            new DefenceStats(12 , 2, 15),
            new DefenceStats(12 , 3, 14),
            new DefenceStats(13 , 1, 15),
            new DefenceStats(13 , 2, 14),
        };

        public List<string> Names => new List<string>(_names);
        public List<Damage> Damages => new List<Damage>(_damages);
        public List<IAttack> Attacks => new List<IAttack>(_attacks);
        public List<DefenceStats> DefensiveStats => new List<DefenceStats>(_defensiveStats);
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

    public class DefenceStats
    {
        private readonly int _defence;
        private int _armor;

        public DefenceStats(int defence, int armor, int magicBarrier)
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

    public class PhysicalAttack : IAttack
    {
        private int _baseAttack = 20;
        private int _criticalThreshold;
        private int _ignoreArmor;

        public PhysicalAttack(int criticalThreshold = 20, int ignoreArmor = 0)
        {
            _criticalThreshold = criticalThreshold;
            _ignoreArmor = ignoreArmor;
        }

        public bool HasHit(DefenceStats defences, out bool isCritical)
        {
            isCritical = false;
            int currentAttack = Randomize.GetNumber(_baseAttack) + 1;
            int armorClass = defences.CalculateArmorClass(_ignoreArmor);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" [физ. {currentAttack}]");
            Console.ResetColor();

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

        public bool HasHit(DefenceStats defences, out bool isCritical)
        {
            isCritical = false;
            int currentAttack = Randomize.GetNumber(_baseAttack) + 1;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($" [маг. {currentAttack}]");
            Console.ResetColor();

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
        private readonly IAttack _attack;

        public Weapon(Damage damage, IAttack attack)
        {
            _damage = damage;
            _attack = attack;
        }

        public void Attack(Unit target)
        {
            if (_attack.HasHit(target.DefenceStats, out bool isCritical))
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

        public Unit(string name, int health, DefenceStats defenceStats, Weapon weapon)
        {
            Name = name;
            _health = new Health(health);
            DefenceStats = defenceStats;
            _weapon = weapon;
        }

        public string Name { get; }
        public DefenceStats DefenceStats { get; }

        public int Health => _health.CurrentValue;

        public void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);

            Console.WriteLine($"{Name} получает урон {damage}");
            Console.ResetColor();
        }

        public void TakeHeal(int heal) => _health.Heal(heal);

        public void Attack(Unit target)
        {
            Console.Write($"{Name} атакует {target.Name}");

            _weapon.Attack(target);
        }
    }

    public class UnitsFactory
    {
        private Config _config = new Config();

        public Unit Create()
        {
            string name = GetName();
            int health = GetHealth();
            DefenceStats defensiveStats = GetDefensiveStats();
            Weapon weapon = GetWeapon();

            return new Unit(name, health, defensiveStats, weapon);
        }


        private string GetName() => _config.Names[Randomize.GetNumber(_config.Names.Count)];

        private int GetHealth()
        {
            int min = 30;
            int max = 35;

            return Randomize.GetNumber(min, max);
        }

        private DefenceStats GetDefensiveStats() =>
            _config.DefensiveStats[Randomize.GetNumber(_config.DefensiveStats.Count)];

        private Weapon GetWeapon()
        {
            Damage damage = _config.Damages[Randomize.GetNumber(_config.Damages.Count)];
            IAttack attack = _config.Attacks[Randomize.GetNumber(_config.Attacks.Count)];

            return new Weapon(damage, attack);
        }
    }

    public class PlatoonFactory
    {
        private readonly UnitsFactory _unitsFactory = new UnitsFactory();

        public Platoon Create(string name, int countUnit)
        {
            List<Unit> units = new List<Unit>();

            for (int i = 0; i < countUnit; i++)
                units.Add(_unitsFactory.Create());

            return new Platoon(name, units);
        }
    }

    public class Platoon
    {
        private List<Unit> _units;

        public Platoon(string name, List<Unit> units)
        {
            _units = units;
            Name = name;
        }

        public string Name { get; }
        public bool IsAlive => _units.Count > 0;

        public void TakeDamage(Unit unit)
        {
            Unit target = _units[Randomize.GetNumber(_units.Count)];
            unit.Attack(target);
        }

        public Unit GetNext()
        {
            Unit unit = _units[0];
            _units.Remove(unit);
            _units.Add(unit);

            return unit;
        }

        public void BuryDead()
        {
            for (int i = _units.Count - 1; i >= 0; i--)
                if (_units[i].Health == 0)
                    _units.RemoveAt(i);
        }
    }
}