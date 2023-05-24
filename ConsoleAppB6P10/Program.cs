//GlobalUsing
//Net v.6
/*
 * Воин быстро машет оружием, может провести дополнительную атаку, имеет бонус урона
 * Варвар яростный в битве, при крите проводит дополнительную атаку
 * Чемпион Торма (бог) при крите получает лечение от своего бога
 */

namespace ConsoleAppB6P10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PlatoonFactory platoonFactory = new PlatoonFactory();
            int numberUnitsPerType = 3;

            Platoon platoon1 = platoonFactory.Create("First", numberUnitsPerType);
            Platoon platoon2 = platoonFactory.Create("Second", numberUnitsPerType);

            Battleground battleground = new Battleground();

            battleground.ConductBattle(platoon1, platoon2);

            Console.ReadKey();
        }
    }

    #region Interfaces

    public interface IDamage
    {
        int Get();

        int GetCritical();
    }

    public interface IAttack
    {
        bool HasHit(DefenceStats defences, out bool isCritical);
    }

    public interface IActor
    {
        string Name { get; }
        int Health { get; }
        DefenceStats DefenceStats { get; }
    }

    public interface IUnit : IActor
    {
        void TakeDamage(int damage);

        void Attack(IUnit target);
    }

    #endregion

    public static class Random
    {
        private static System.Random _random = new System.Random();

        public static int GetNumber(int max) =>
            _random.Next(max);

        public static int GetNumber(int min, int max) =>
            _random.Next(min, max + 1);
    }

    public class Config
    {
        public string[] Names => new string[]
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
        public IDamage[] LowDamages => new IDamage[]
        {
            new Damage(1, 6),
            new Damage(1, 8),
            new Damage(2, 4),
            new TripleCritDamage(1, 6),
            new TripleCritDamage(1, 8),
            new TripleCritDamage(2, 4),
        };
        public IDamage[] HighDamages => new IDamage[]
        {
            new Damage(1, 10),
            new Damage(2, 12),
            new TripleCritDamage(1, 10),
            new TripleCritDamage(2, 12),
        };
        public IAttack[] Attacks => new IAttack[]
        {
            new PhysicalAttack(17),
            new PhysicalAttack(18, 1),
            new PhysicalAttack(19),
            new PhysicalAttack(19, 1),
            new PhysicalAttack(ignoredArmor: 2),
            new PhysicalAttack(),
        };
        public DefenceStats[] LowArmor => new DefenceStats[]
        {
            new DefenceStats(12, 1, 15),
            new DefenceStats(12, 2, 14),
            new DefenceStats(13, 1, 14),
            new DefenceStats(13, 2, 13),
        };
        public DefenceStats[] HeavyArmor => new DefenceStats[]
        {
            new DefenceStats(10 , 4, 14),
            new DefenceStats(10 , 5, 13),
            new DefenceStats(10 , 6, 12),
            new DefenceStats(11 , 3, 14),
            new DefenceStats(11 , 4, 13),
            new DefenceStats(12 , 3, 13),
        };
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

        public void Heal(int value)
        {
            if (value <= 0)
                return;

            CurrentValue += value;
        }
    }

    public class DefenceStats
    {
        private readonly int _base;
        private int _armor;

        public DefenceStats(int @base, int armor, int magicBarrier)
        {
            _base = @base;
            _armor = armor;
            MagicBarrier = magicBarrier;
        }

        public int MagicBarrier { get; }

        public int CalculateArmorClass(int ignoredArmor) =>
            Math.Clamp(_armor - ignoredArmor, 0, _armor) + _base;
    }

    public class Damage : IDamage
    {
        private readonly int _min;
        private readonly int _max;

        public Damage(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public int Get() => Random.GetNumber(_min, _max);

        public virtual int GetCritical() => _max + Get();
    }

    public class TripleCritDamage : Damage
    {
        private readonly int _max;

        public TripleCritDamage(int min, int max) : base(min, max)
        {
            _max = max;
        }

        public override int GetCritical() => _max + base.GetCritical();
    }

    public class PhysicalAttack : IAttack
    {
        private readonly int _base = 20;
        private readonly int _criticalThreshold;
        private readonly int _ignoredArmor;

        public PhysicalAttack(int criticalThreshold = 20, int ignoredArmor = 0)
        {
            _criticalThreshold = criticalThreshold;
            _ignoredArmor = ignoredArmor;
        }

        public bool HasHit(DefenceStats defences, out bool isCritical)
        {
            int currentAttack = Random.GetNumber(_base) + 1;
            int armorClass = defences.CalculateArmorClass(_ignoredArmor);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" [физ. {currentAttack}]");
            Console.ResetColor();

            if (currentAttack == _base)
            {
                isCritical = true;

                return true;
            }

            isCritical = currentAttack >= _criticalThreshold;

            return currentAttack >= armorClass;
        }
    }

    public class MagicAttack : IAttack
    {
        private int _base = 20;

        public bool HasHit(DefenceStats defences, out bool isCritical)
        {
            isCritical = false;
            int current = Random.GetNumber(_base) + 1;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($" [маг. {current}]");
            Console.ResetColor();

            if (current == _base)
            {
                isCritical = true;

                return true;
            }

            return current >= defences.MagicBarrier;
        }
    }

    public class AttackSpeed
    {
        private int _speed;
        private int _scale;
        private int _scaleMax;

        public AttackSpeed(int min, int max)
        {
            _speed = Random.GetNumber(min, max);
            _scale = 0;
            _scaleMax = 100;
        }

        public bool CanUseOptionalAttack()
        {
            _scale += _speed;

            if (_scale < _scaleMax)
                return false;

            _scale %= _scaleMax;

            return true;
        }
    }

    public class Actor : IActor
    {
        private readonly Health _health;

        public Actor(string name, int maxHealth, DefenceStats defenceStats)
        {
            Name = name;
            _health = new Health(maxHealth);
            DefenceStats = defenceStats;
        }

        public string Name { get; }
        public DefenceStats DefenceStats { get; }
        public int Health => _health.CurrentValue;

        public void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Name} получает урон {damage}");
            Console.ResetColor();
        }

        public void TakeHeal(int value)
        {
            _health.Heal(value);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Name} восстанавливает здоровье [{value}]");
            Console.ResetColor();
        }
    }

    public class Warrior : Actor, IUnit
    {
        private readonly IAttack _attack;
        private readonly IDamage _damage;
        private readonly AttackSpeed _attackSpeed;
        private readonly int _damageBonus;

        public Warrior(string name, int health, DefenceStats defenceStats, IAttack attack, IDamage damage)
            : base(name, health, defenceStats)
        {
            _attack = attack;
            _damage = damage;
            int minSpeed = 10;
            int maxSpeed = 30;
            _attackSpeed = new AttackSpeed(minSpeed, maxSpeed);
            _damageBonus = 3;
        }

        public void Attack(IUnit target)
        {
            Console.Write($"{Name} атакует {target.Name}");

            if (_attack.HasHit(target.DefenceStats, out bool isCritical))
            {
                if (isCritical)
                    target.TakeDamage(_damage.GetCritical() + _damageBonus);
                else
                    target.TakeDamage(_damage.Get() + _damageBonus);
            }
            else
            {
                Console.WriteLine($"Промах: {target.Name} не получает урона");
            }

            if (_attackSpeed.CanUseOptionalAttack())
                Attack(target);
        }
    }

    public class Barbarian : Actor, IUnit
    {
        private readonly IAttack _attack;
        private readonly IDamage _damage;

        public Barbarian(string name, int health, DefenceStats defenceStats, IAttack attack, IDamage damage)
            : base(name, health, defenceStats)
        {
            _attack = attack;
            _damage = damage;
        }

        public void Attack(IUnit target)
        {
            Console.Write($"{Name} атакует {target.Name}");

            if (_attack.HasHit(target.DefenceStats, out bool isCritical) == false)
            {
                Console.WriteLine($"Промах: {target.Name} не получает урона");
                return;
            }

            if (isCritical)
            {
                target.TakeDamage(_damage.GetCritical());
                Attack(target);
            }
            else
            {
                target.TakeDamage(_damage.Get());
            }
        }
    }

    public class СhampionTorm : Actor, IUnit
    {
        private readonly IAttack _attack;
        private readonly IDamage _damage;
        private readonly int _healValue;

        public СhampionTorm(string name, int health, DefenceStats defenceStats, IAttack attack, IDamage damage)
            : base(name, health, defenceStats)
        {
            _attack = attack;
            _damage = damage;
            int min = 5;
            int max = 10;
            _healValue = Random.GetNumber(min, max);
        }

        public void Attack(IUnit target)
        {
            Console.Write($"{Name} атакует {target.Name}");

            if (_attack.HasHit(target.DefenceStats, out bool isCritical) == false)
            {
                Console.WriteLine($"Промах: {target.Name} не получает урона");
                return;
            }

            if (isCritical)
            {
                target.TakeDamage(_damage.GetCritical());
                TakeHeal(_healValue);
            }
            else
            {
                target.TakeDamage(_damage.Get());
            }
        }
    }

    public class UnitFactory
    {
        private readonly Config _config = new Config();

        public Warrior CreateWarrior()
        {
            string name = "(Воин) " + GetName();
            int health = GetHealth();
            DefenceStats defensiveStats = GetHeavyArmor();
            IAttack attack = GetAttack();
            IDamage damage = GetLowDamage();

            return new Warrior(name, health, defensiveStats, attack, damage);
        }

        public Barbarian CreateBarbarian()
        {
            string name = "(Варвар) " + GetName();
            int health = GetHealth();
            DefenceStats defensiveStats = GetLowArmor();
            IAttack attack = GetAttack();
            IDamage damage = GetHighDamage();

            return new Barbarian(name, health, defensiveStats, attack, damage);
        }

        public СhampionTorm CreateСhampionTorm()
        {
            string name = "(Чемпион Торма) " + GetName();
            int health = GetHealth();
            DefenceStats defensiveStats = GetHeavyArmor();
            IAttack attack = new MagicAttack();
            IDamage damage = GetLowDamage();

            return new СhampionTorm(name, health, defensiveStats, attack, damage);
        }

        private string GetName() =>
            _config.Names[Random.GetNumber(_config.Names.Length)];

        private int GetHealth()
        {
            int min = 30;
            int max = 35;

            return Random.GetNumber(min, max);
        }

        private DefenceStats GetLowArmor() =>
            _config.LowArmor[Random.GetNumber(_config.LowArmor.Length)];

        private DefenceStats GetHeavyArmor() =>
            _config.HeavyArmor[Random.GetNumber(_config.HeavyArmor.Length)];

        private IAttack GetAttack() =>
            _config.Attacks[Random.GetNumber(_config.Attacks.Length)];

        private IDamage GetLowDamage() =>
            _config.LowDamages[Random.GetNumber(_config.LowDamages.Length)];

        private IDamage GetHighDamage() =>
            _config.HighDamages[Random.GetNumber(_config.HighDamages.Length)];
    }

    public class Platoon
    {
        private readonly List<IUnit> _units;

        public Platoon(string name, IEnumerable<IUnit> units)
        {
            _units = new List<IUnit>(units);
            Name = name;
        }

        public string Name { get; }
        public bool IsAlive => _units.Count > 0;

        public void TakeDamage(IUnit unit)
        {
            Console.Write($"[{Name}] ");
            IUnit target = _units[Random.GetNumber(_units.Count)];
            unit.Attack(target);
        }

        public IUnit GetNext()
        {
            IUnit unit = _units[0];
            _units.Remove(unit);
            _units.Add(unit);

            return unit;
        }

        public void BuryDead()
        {
            for (int i = _units.Count - 1; i >= 0; i--)
            {
                if (_units[i].Health == 0)
                {
                    Console.Write($"[{Name}] Воин {_units[i].Name} мёртв. ");
                    _units.RemoveAt(i);
                    Console.WriteLine($"[В отряде {_units.Count}]");
                }
            }
        }

        public void PrintWin() =>
            Console.WriteLine($"\nПобеда за {Name}. В отряде осталось [{_units.Count}]");
    }

    public class PlatoonFactory
    {
        private readonly UnitFactory _unitsFactory = new UnitFactory();

        public Platoon Create(string name, int numberUnitsPerType)
        {
            List<IUnit> units = new List<IUnit>();

            for (int i = 0; i < numberUnitsPerType; i++)
            {
                units.Add(_unitsFactory.CreateWarrior());
                units.Add(_unitsFactory.CreateBarbarian());
                units.Add(_unitsFactory.CreateСhampionTorm());
            }

            Shufle(units);

            return new Platoon(name, units);
        }

        private void Shufle(List<IUnit> units)
        {
            for (int i = 0; i < units.Count; i++)
            {
                int j = Random.GetNumber(units.Count);
                (units[i], units[j]) = (units[j], units[i]);
            }
        }
    }

    public class Battleground
    {
        public void ConductBattle(Platoon platoon1, Platoon platoon2)
        {
            while (platoon1.IsAlive && platoon2.IsAlive)
            {
                platoon2.TakeDamage(platoon1.GetNext());
                platoon1.TakeDamage(platoon2.GetNext());

                platoon2.BuryDead();
                platoon1.BuryDead();
            }

            if (platoon1.IsAlive == platoon2.IsAlive)
                Console.WriteLine("\nНичья");
            else if (platoon1.IsAlive)
                platoon1.PrintWin();
            else
                platoon2.PrintWin();
        }
    }
}