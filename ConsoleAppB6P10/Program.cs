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

            Platoon platoon1 = platoonFactory.Create("First", 3);
            Platoon platoon2 = platoonFactory.Create("Second", 3);

            while (platoon1.IsAlive && platoon2.IsAlive)
            {
                platoon2.TakeDamage(platoon1.GetNext());
                platoon1.TakeDamage(platoon2.GetNext());

                platoon2.BuryDead();
                platoon1.BuryDead();

                Thread.Sleep(500);
            }

            if (platoon1.IsAlive == platoon2.IsAlive)
                Console.WriteLine("\nНичья");
            else if (platoon1.IsAlive)
                platoon1.PrintWin();
            else
                platoon2.PrintWin();

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

        void TakeDamage(int damage);
    }

    public interface IUnit : IActor
    {
        void ExecuteAttack(IUnit target);
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

        private List<IDamage> _lowDamages = new List<IDamage>()
        {
            new Damage(1, 6),
            new Damage(1, 8),
            new Damage(2, 4),
            new DamageTripleCritical(1, 6),
            new DamageTripleCritical(1, 8),
            new DamageTripleCritical(2, 4),
        };

        private List<IDamage> _highDamages = new List<IDamage>()
        {
            new Damage(1, 10),
            new Damage(2, 12),
            new DamageTripleCritical(1, 10),
            new DamageTripleCritical(2, 12),
        };

        List<IAttack> _attacks = new List<IAttack>()
        {
            new PhysicalAttack(17),
            new PhysicalAttack(18, 1),
            new PhysicalAttack(19),
            new PhysicalAttack(19, 1),
            new PhysicalAttack(ignoreArmor: 2),
            new PhysicalAttack(),
        };

        private List<DefenceStats> _lowArmor = new List<DefenceStats>()
        {
            new DefenceStats(12 , 1, 15),
            new DefenceStats(12 , 2, 14),
            new DefenceStats(13 , 1, 14),
            new DefenceStats(13 , 2, 13),
        };

        private List<DefenceStats> _heavyArmor = new List<DefenceStats>()
        {
            new DefenceStats(10 , 4, 14),
            new DefenceStats(10 , 5, 13),
            new DefenceStats(10 , 6, 12),
            new DefenceStats(11 , 3, 14),
            new DefenceStats(11 , 4, 13),
            new DefenceStats(12 , 3, 13),
        };

        public List<string> Names => new List<string>(_names);
        public List<IDamage> LowDamages => new List<IDamage>(_lowDamages);
        public List<IDamage> HighDamages => new List<IDamage>(_highDamages);
        public List<IAttack> Attacks => new List<IAttack>(_attacks);
        public List<DefenceStats> LowArmor => new List<DefenceStats>(_lowArmor);
        public List<DefenceStats> HeavyArmor => new List<DefenceStats>(_heavyArmor);
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

    public class Damage : IDamage
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

    public class DamageTripleCritical : Damage, IDamage
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

    public class AttackSpeed
    {
        private int _attackSpeed;
        private int _attackScale;
        private int _attackScaleMax;

        public AttackSpeed()
        {
            _attackSpeed = Randomize.GetNumber(10, 30);
            _attackScale = 0;
            _attackScaleMax = 100;
        }

        public bool HasAttacked()
        {
            _attackScale += _attackSpeed;

            if (_attackScale >= _attackScaleMax)
            {
                _attackScale %= _attackScaleMax;

                return true;
            }

            return false;
        }
    }

    //public class Weapon
    //{
    //    private readonly Damage _damage;

    //    public Weapon(Damage damage)
    //    {
    //        _damage = damage;
    //    }

    //    public int GetDamage() => _damage.Get();

    //    public int GetCriticalDamage() => _damage.GetCritical();
    //}

    public class Actor : IActor
    {
        private readonly Health _health;

        public Actor(string name, int health, DefenceStats defenceStats)
        {
            Name = name;
            _health = new Health(health);
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

        public void TakeHeal(int heal)
        {
            _health.Heal(heal);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Name} восстанавливает здоровье [{heal}]");
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
            _attackSpeed = new AttackSpeed();
            _damageBonus = 3;
        }

        public void ExecuteAttack(IUnit target)
        {
            Attack(target);

            if (_attackSpeed.HasAttacked())
                Attack(target);
        }

        private void Attack(IUnit target)
        {
            Console.Write($"{Name} атакует {target.Name}");

            if (_attack.HasHit(target.DefenceStats, out bool isCritical))
                if (isCritical)
                    target.TakeDamage(_damage.GetCritical() + _damageBonus);
                else
                    target.TakeDamage(_damage.Get() + _damageBonus);
            else
                Console.WriteLine($"Промах: {target.Name} не получает урона");
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

        public void ExecuteAttack(IUnit target) => Attack(target);

        private void Attack(IUnit target)
        {
            Console.Write($"{Name} атакует {target.Name}");

            if (_attack.HasHit(target.DefenceStats, out bool isCritical))
                if (isCritical)
                {
                    target.TakeDamage(_damage.GetCritical());

                    Attack(target);
                }
                else
                    target.TakeDamage(_damage.Get());
            else
                Console.WriteLine($"Промах: {target.Name} не получает урона");
        }
    }

    public class СhampionTorm : Actor, IUnit
    {
        private readonly IAttack _attack;
        private readonly IDamage _damage;
        private readonly int _heal;

        public СhampionTorm(string name, int health, DefenceStats defenceStats, IAttack attack, IDamage damage)
            : base(name, health, defenceStats)
        {
            _attack = attack;
            _damage = damage;
            _heal = Randomize.GetNumber(5, 10);
        }

        public void ExecuteAttack(IUnit target) => Attack(target);

        private void Attack(IUnit target)
        {
            Console.Write($"{Name} атакует {target.Name}");

            if (_attack.HasHit(target.DefenceStats, out bool isCritical))
                if (isCritical)
                {
                    target.TakeDamage(_damage.GetCritical());

                    TakeHeal(_heal);
                }
                else
                    target.TakeDamage(_damage.Get());
            else
                Console.WriteLine($"Промах: {target.Name} не получает урона");
        }
    }

    public class UnitsFactory
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
            _config.Names[Randomize.GetNumber(_config.Names.Count)];

        private int GetHealth()
        {
            int min = 30;
            int max = 35;

            return Randomize.GetNumber(min, max);
        }

        private DefenceStats GetLowArmor() =>
            _config.LowArmor[Randomize.GetNumber(_config.LowArmor.Count)];

        private DefenceStats GetHeavyArmor() =>
            _config.HeavyArmor[Randomize.GetNumber(_config.HeavyArmor.Count)];

        private IAttack GetAttack() =>
            _config.Attacks[Randomize.GetNumber(_config.Attacks.Count)];

        private IDamage GetLowDamage() =>
            _config.LowDamages[Randomize.GetNumber(_config.LowDamages.Count)];

        private IDamage GetHighDamage() =>
            _config.HighDamages[Randomize.GetNumber(_config.HighDamages.Count)];
    }

    public class Platoon
    {
        private List<IUnit> _units;

        public Platoon(string name, List<IUnit> units)
        {
            _units = units;
            Name = name;
        }

        public string Name { get; }
        public bool IsAlive => _units.Count > 0;

        public void TakeDamage(IUnit unit)
        {
            Console.Write($"[{Name}] ");
            IUnit target = _units[Randomize.GetNumber(_units.Count)];
            unit.ExecuteAttack(target);
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
                if (_units[i].Health == 0)
                {
                    Console.Write($"[{Name}] Воин {_units[i].Name} мёртв. ");

                    _units.RemoveAt(i);

                    Console.WriteLine($"[В отряде {_units.Count}]");
                }
        }

        public void PrintWin()
        {
            Console.WriteLine($"\nПобеда за {Name}. В отряде осталось [{_units.Count}]");
        }
    }

    public class PlatoonFactory
    {
        private readonly UnitsFactory _unitsFactory = new UnitsFactory();

        public Platoon Create(string name, int countUnit)
        {
            List<IUnit> units = new List<IUnit>();

            for (int i = 0; i < countUnit; i++)
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
                int j = Randomize.GetNumber(units.Count);
                (units[i], units[j]) = (units[j], units[i]);
            }
        }
    }
}