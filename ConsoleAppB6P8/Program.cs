using System;
//Net v.6
//По мотивам Dungeons & Dragons 4-ой редакции

namespace ConsoleAppB6P8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Master master = new Master();

            Character player = master.Create(new WarriorBuilder());

            player.ShowInfo();

            Weapon longSword = new Weapon("Длинный меч", 3, new HitDice(1, 8));

            Console.ReadKey();
        }
    }

    public static class AbilityType
    {
        public const string Strength = "Сила";
        public const string Constitution = "Телосложение";
        public const string Dexterity = "Ловкость";
        public const string Intelligence = "Интеллект";
        public const string Wisdom = "Мудрость";
        public const string Charisma = "Харизма";
    }

    public static class DefenсesType
    {
        public const string ArmorClass = "Класс Брони";
        public const string Fortitude = "Стойкость";
        public const string Reflex = "Рефлекс";
        public const string Will = "Воля";
    }

    public static class PowersType
    {
        public const string AtWill = "Не ограничено";
        public const string Encounter = "На сцену";
        public const string Daily = "На день";
    }

    public static class WeaponsTitle
    {
        public const string Dagger = "Кинжал";
        public const string BatleAxe = "Боевой топор";
        public const string LongSword = "Длинный меч";
        public const string Warhammer = "Боевой молот";
        public const string Falchion = "Фальшион";
        public const string LongBow = "Длинный лук";
    }

    public static class DiceRoller
    {
        private static Random _random = new Random();

        public static int RollDiceSum(HitDice hitDice)
        {
            if (CanRoll(hitDice) == false)
                return 0;

            if (hitDice.Count == 1)
                return RollOne(hitDice.Sides);
            else
                return RollDices(hitDice).Sum();
        }

        public static int[] RollDices(HitDice hitDice)
        {
            if (CanRoll(hitDice) == false)
                return new int[] { 0 };

            int[] values = new int[hitDice.Count];

            for (int i = 0; i < hitDice.Count; i++)
                values[i] = RollOne(hitDice.Sides);

            return values;
        }

        private static bool CanRoll(HitDice hitDice) //вынести в HitDice
        {
            if (hitDice == null)
                return false;

            if (hitDice.Count < 0)
                return false;

            return true;
        }

        private static int RollOne(int sides) =>
            _random.Next(sides) + 1;
    }

    public class HitDice
    {
        public HitDice(int count, int sides)
        {
            Count = count;
            Sides = sides;
        }

        public int Count { get; }
        public int Sides { get; }
    }

    public class Weapon
    {
        public Weapon(string title, int proficiency, HitDice hitDice)
        {
            Title = title;
            Proficiency = proficiency;
            HitDice = hitDice;
        }

        public string Title { get; }
        public int Proficiency { get; }
        public HitDice HitDice { get; }
    }

    public class Ability
    {
        public Ability(string title, int value)
        {
            Title = title;
            Value = value;
        }

        public string Title { get; }
        public int Value { get; }
        public int Modifier => CalculateModifier(Value);

        private int CalculateModifier(int value)
        {
            double average = 10.0;
            int divider = 2;

            return (int)Math.Round((value - average) / divider, MidpointRounding.ToNegativeInfinity);
        }
    }

    public class AbilitySet
    {
        private readonly Dictionary<string, Ability> _abilities;

        public AbilitySet(Dictionary<string, Ability> abilities)
        {
            _abilities = new Dictionary<string, Ability>(abilities);
        }

        public Ability Strength => _abilities[AbilityType.Strength];
        public Ability Constitution => _abilities[AbilityType.Constitution];
        public Ability Dexterity => _abilities[AbilityType.Dexterity];
        public Ability Intelligence => _abilities[AbilityType.Intelligence];
        public Ability Wisdom => _abilities[AbilityType.Wisdom];
        public Ability Charisma => _abilities[AbilityType.Charisma];

        public void ShowInfo()
        {
            foreach (string key in _abilities.Keys)
                Console.WriteLine($"{key} - {_abilities[key].Value} ({_abilities[key].Modifier})");
        }
    }

    public class Health
    {
        private int _maxHealth;
        private int _health;

        public Health(int rankValue, Ability constitution)
        {
            _maxHealth = rankValue + constitution.Value;
            _health = _maxHealth;
        }

        public int Value => _health;

        public void GetHit(int damage)
        {
            if (damage <= 0)
                return;

            _health -= damage;
        }

        public void GetHeal(int heal)
        {
            if (heal <= 0)
                return;

            _health += heal;

            if (_health > _maxHealth)
                _health = _maxHealth;
        }
    }

    public class Defenсe
    {
        public Defenсe(string title, int value)
        {
            Title = title;
            Value = value;
        }

        public string Title { get; }
        public int Value { get; }
    }

    interface IPower
    {
        bool CanApply();

        void Reset();
    }

    public class Power
    {
        protected Ability _ability;
        protected string _defenceAbility;

        public Power(string title, Ability ability, string defenceType)
        {
            Title = title;
            _ability = ability;
            _defenceAbility = defenceType;
        }

        public string Title { get; }

        public int GetAttack()
        {
            HitDice attackDice = new HitDice(1, 20);

            return DiceRoller.RollDiceSum(attackDice) + _ability.Modifier;
        }
    }

    public class AtWillPower : Power, IPower
    {
        public AtWillPower(string title, Ability ability, string defenceType)
            : base(title, ability, defenceType)
        {
        }

        public bool CanApply() => true;

        public int GetHit(HitDice hitDice) =>
            DiceRoller.RollDices(hitDice).Sum();

        public void Reset() { }
    }

    public class EncounterPower : Power, IPower
    {
        private bool _isReady;

        public EncounterPower(string title, Ability ability, string defenceType)
            : base(title, ability, defenceType)
        {
            _isReady = true;
        }

        public bool CanApply() => _isReady;

        public int GetHit(HitDice hitDice)
        {
            int count = 2;
            int damadeWeapon = 0;

            for (int i = 0; i < count; i++)
                damadeWeapon += DiceRoller.RollDiceSum(hitDice);

            return damadeWeapon + _ability.Modifier;
        }

        public void Reset()
        {
            _isReady = true;
        }
    }

    public class DailyPower : Power, IPower
    {
        private bool _isReady;

        public DailyPower(string title, Ability ability, string defenceType)
            : base(title, ability, defenceType)
        {
            _isReady = true;
        }

        public bool CanApply() => _isReady;

        public int GetHit(HitDice hitDice)
        {
            int count = 3;
            int damadeWeapon = 0;

            for (int i = 0; i < count; i++)
                damadeWeapon += DiceRoller.RollDiceSum(hitDice);

            return damadeWeapon + _ability.Modifier;
        }

        public void Reset()
        {
            _isReady = true;
        }
    }

    public class DefenсeSet
    {
        private readonly Dictionary<string, Defenсe> _defenсes;

        public DefenсeSet(Dictionary<string, Defenсe> defenсes)
        {
            _defenсes = new Dictionary<string, Defenсe>(defenсes);
        }

        public Defenсe ArmorClass => _defenсes[DefenсesType.ArmorClass];
        public Defenсe Fortitude => _defenсes[DefenсesType.Fortitude];
        public Defenсe Reflex => _defenсes[DefenсesType.Reflex];
        public Defenсe Will => _defenсes[DefenсesType.Will];

        public void ShowInfo()
        {
            foreach (var defenсe in _defenсes)
                Console.WriteLine($"{defenсe.Key}: {defenсe.Value.Value}");
        }
    }

    public abstract class AbilitySetBuilder
    {
        public AbilitySet GetAbilitySet()
        {
            Dictionary<string, Ability> abilities = new Dictionary<string, Ability>();
            bool isCorrect = false;
            string[] titles = GetAbilites();

            CheckAbilities(titles);

            while (isCorrect == false)
            {
                abilities.Clear();

                int[] values = GetValues(titles.Length);

                for (int i = 0; i < values.Length; i++)
                    abilities.Add(titles[i], new Ability(titles[i], values[i]));

                AbilitySet abilitySet = new AbilitySet(abilities);

                int sumModifiers =
                    abilitySet.Strength.Modifier +
                    abilitySet.Constitution.Modifier +
                    abilitySet.Dexterity.Modifier +
                    abilitySet.Intelligence.Modifier +
                    abilitySet.Wisdom.Modifier +
                    abilitySet.Charisma.Modifier;

                if (sumModifiers >= 4 && sumModifiers < 8)
                    isCorrect = true;
            }

            return new AbilitySet(abilities);
        }

        protected abstract string[] GetAbilites();

        private void CheckAbilities(string[] titles)
        {
            string[] checkTitles = new string[] {
                AbilityType.Strength,
                AbilityType.Constitution,
                AbilityType.Dexterity,
                AbilityType.Intelligence,
                AbilityType.Wisdom,
                AbilityType.Charisma
            };

            foreach (string title in checkTitles)
                if (Array.IndexOf(titles, title) == -1)
                    throw new Exception($"Нет нужной способности - {title}");
        }

        private int GenerateValue()
        {
            HitDice hitDice = new HitDice(4, 6);

            int[] results = DiceRoller.RollDices(hitDice);

            return results.Sum() - results.Min();
        }

        private int[] GetValues(int count)
        {
            int[] values = new int[count];

            for (int i = 0; i < values.Length; i++)
                values[i] = GenerateValue();

            Array.Sort(values);
            Array.Reverse(values);

            return values;
        }
    }

    public class WarriorAbilitySetBuilder : AbilitySetBuilder
    {
        protected override string[] GetAbilites()
        {
            return new string[] {
                AbilityType.Strength,
                AbilityType.Constitution,
                AbilityType.Dexterity,
                AbilityType.Wisdom,
                AbilityType.Intelligence,
                AbilityType.Charisma,
            };
        }
    }

    public class PathfinderAbilitySetBuilder : AbilitySetBuilder
    {
        protected override string[] GetAbilites()
        {
            return new string[] {
                AbilityType.Strength,
                AbilityType.Dexterity,
                AbilityType.Wisdom,
                AbilityType.Constitution,
                AbilityType.Intelligence,
                AbilityType.Charisma,
            };
        }
    }

    public class RanegerAbilitySetBuilder : AbilitySetBuilder
    {
        protected override string[] GetAbilites()
        {
            return new string[] {
                AbilityType.Dexterity,
                AbilityType.Strength,
                AbilityType.Wisdom,
                AbilityType.Constitution,
                AbilityType.Intelligence,
                AbilityType.Charisma,
            };
        }
    }

    public class PaladinAbilitySetBuilder : AbilitySetBuilder
    {
        protected override string[] GetAbilites()
        {
            return new string[] {
                AbilityType.Strength,
                AbilityType.Charisma,
                AbilityType.Wisdom,
                AbilityType.Constitution,
                AbilityType.Intelligence,
                AbilityType.Dexterity,
            };
        }
    }

    public class RogueAbilitySetBuilder : AbilitySetBuilder
    {
        protected override string[] GetAbilites()
        {
            return new string[] {
                AbilityType.Dexterity,
                AbilityType.Strength,
                AbilityType.Charisma,
                AbilityType.Intelligence,
                AbilityType.Constitution,
                AbilityType.Wisdom,
            };
        }
    }

    public class WizardAbilitySetBuilder : AbilitySetBuilder
    {
        protected override string[] GetAbilites()
        {
            return new string[] {
                AbilityType.Intelligence,
                AbilityType.Wisdom,
                AbilityType.Dexterity,
                AbilityType.Constitution,
                AbilityType.Charisma,
                AbilityType.Strength,
            };
        }
    }

    public class DefenсeBuilder
    {
        public DefenсeSet GetDefenceSet(
            AbilitySet abilitySet,
            Dictionary<string, int> defenсeBonuses,
            bool isHeavy
            )
        {
            CheckDefenсes(defenсeBonuses);

            int baseValue = 10;
            Dictionary<string, Defenсe> defenсes = new Dictionary<string, Defenсe>();
            Dictionary<string, int> defenсeValeus = new Dictionary<string, int>()
            {
                { DefenсesType.ArmorClass, Math.Max(abilitySet.Dexterity.Modifier, abilitySet.Intelligence.Modifier)},
                { DefenсesType.Fortitude, Math.Max(abilitySet.Strength.Modifier, abilitySet.Constitution.Modifier)},
                { DefenсesType.Reflex, Math.Max(abilitySet.Dexterity.Modifier, abilitySet.Intelligence.Modifier)},
                { DefenсesType.Will, Math.Max(abilitySet.Wisdom.Modifier, abilitySet.Charisma.Modifier)},
            };

            if (isHeavy)
                defenсeValeus[DefenсesType.ArmorClass] = 0;

            foreach (string key in defenсeValeus.Keys)
            {
                defenсeValeus[key] += baseValue + defenсeBonuses[key];
                defenсes.Add(key, new Defenсe(key, defenсeValeus[key]));
            }

            return new DefenсeSet(defenсes);
        }

        private void CheckDefenсes(Dictionary<string, int> defenсes)
        {
            string[] checkTitles = new string[] {
                DefenсesType.ArmorClass,
                DefenсesType.Fortitude,
                DefenсesType.Reflex,
                DefenсesType.Will,
            };

            foreach (string title in checkTitles)
                if (defenсes.ContainsKey(title) == false)
                    throw new Exception($"Нет нужной защиты - {title}");
        }
    }

    public class Character
    {
        public Character(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public AbilitySet AbilitySet { get; private set; }
        public DefenсeSet DefenсeSet { get; private set; }
        public Health Health { get; private set; }

        public void SetAbilities(AbilitySet abilitySet) =>
            AbilitySet = abilitySet;

        public void SetDefenсes(DefenсeSet defenсeSet) =>
            DefenсeSet = defenсeSet;

        public void SetHealth(int rankValue) =>
            Health = new Health(rankValue, AbilitySet.Constitution);

        public void ShowInfo()
        {
            Console.WriteLine(Name);
            Console.WriteLine($"Здоровье - {Health.Value}");
            AbilitySet.ShowInfo();
            DefenсeSet.ShowInfo();
        }
    }

    public abstract class CharacterBuilder
    {
        protected string _name;

        public Character? Character { get; protected set; }

        public void SetName()
        {
            _name = "Njh";
        }


        public void CreateCharacter() =>
            Character = new Character(_name);

        public abstract void SetAbilitySet();

        public abstract void SetDefenсeSet();

        public abstract void SetHealth();

        public abstract void SetPowerSet();
    }

    public class Master
    {
        public Character Create(CharacterBuilder characterBuilder)
        {
            characterBuilder.SetName();
            characterBuilder.CreateCharacter();
            characterBuilder.SetAbilitySet();
            characterBuilder.SetDefenсeSet();
            characterBuilder.SetHealth();
            characterBuilder.SetPowerSet();

            return characterBuilder.Character;
        }
    }

    public class WarriorBuilder : CharacterBuilder
    {
        public override void SetAbilitySet()
        {
            WarriorAbilitySetBuilder warriorAbilitySetBuilder = new WarriorAbilitySetBuilder();
            AbilitySet abilitySet = warriorAbilitySetBuilder.GetAbilitySet();
            Character.SetAbilities(abilitySet);
        }

        public override void SetDefenсeSet()
        {
            int armorDefenсe = 6;
            int shieldDefenсe = 1;
            bool isHeavy = true;
            DefenсeBuilder defenсeBuilder = new DefenсeBuilder();
            Dictionary<string, int> defenсeBonuses = new Dictionary<string, int>()
            {
                [DefenсesType.ArmorClass] = armorDefenсe + shieldDefenсe,
                [DefenсesType.Fortitude] = 2,
                [DefenсesType.Reflex] = 0 + shieldDefenсe,
                [DefenсesType.Will] = 0,
            };
            DefenсeSet defenсeSet = defenсeBuilder.GetDefenceSet(Character.AbilitySet, defenсeBonuses, isHeavy);
            Character.SetDefenсes(defenсeSet);
        }

        public override void SetHealth()
        {
            int rankValue = 15;
            Character.SetHealth(rankValue);
        }

        public override void SetPowerSet()
        {
            return;
        }
    }
}