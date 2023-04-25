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

            Console.ReadKey();
        }
    }

    //public static class DiceRoller
    //{
    //    private static Random _random = new Random();

    //    public static int RollDiceSum(HitDice hitDice)
    //    {
    //        if (CanRoll(hitDice) == false)
    //            return 0;

    //        if (hitDice.Count == 1)
    //            return RollOne(hitDice.Sides);
    //        else
    //            return RollDices(hitDice).Sum();
    //    }

    //    public static int[] RollDices(HitDice hitDice)
    //    {
    //        if (CanRoll(hitDice) == false)
    //            return new int[] { 0 };

    //        int[] values = new int[hitDice.Count];

    //        for (int i = 0; i < hitDice.Count; i++)
    //            values[i] = RollOne(hitDice.Sides);

    //        return values;
    //    }

    //    private static bool CanRoll(HitDice hitDice) //вынести в HitDice
    //    {
    //        if (hitDice == null)
    //            return false;

    //        if (hitDice.Count < 0)
    //            return false;

    //        return true;
    //    }

    //    private static int RollOne(int sides) =>
    //        _random.Next(sides) + 1;
    //}

    public interface IRollStrategy
    {
        RollResult Roll(int count, int sides);
    }

    public class HitDice
    {
        private IRollStrategy _rollStrategy;

        public HitDice(int count, int sides, IRollStrategy rollStrategy)
        {
            Count = count;
            Sides = sides;
            _rollStrategy = rollStrategy;
        }

        public int Count { get; }
        public int Sides { get; }

        public RollResult Roll()
        {
            return _rollStrategy.Roll(Count, Sides);
        }
    }

    public class RollResult
    {
        public RollResult(int sum, int[] results)
        {
            Sum = sum;
            Results = results;
        }

        public int Sum { get; }
        public int[] Results { get; }
    }

    public class RollStrategy : IRollStrategy
    {
        private Random _random = new Random();

        public RollResult Roll(int count, int sides)
        {
            int sum = 0;
            List<int> results = new List<int>();

            for (int i = 0; i < count; i++)
            {
                int result = RollOne(sides);
                results.Add(result);
                sum += result;
            }

            return new RollResult(sum, results.ToArray());
        }

        private int RollOne(int sides) =>
            _random.Next(sides) + 1;
    }

    public class BestResultRollStrategy : IRollStrategy
    {
        private readonly int _bestResultCount;
        private RollStrategy _rollStrategy;

        public BestResultRollStrategy(int bestResultCount)
        {
            _rollStrategy = new RollStrategy();
            _bestResultCount = bestResultCount;
        }

        public RollResult Roll(int count, int sides)
        {
            int bestResultCount = count < _bestResultCount ? count : _bestResultCount;

            RollResult rollResult = _rollStrategy.Roll(count, sides);
            Array.Sort(rollResult.Results);
            int[] sortedResults = rollResult.Results[^bestResultCount..^0];

            return new RollResult(sortedResults.Sum(), sortedResults);
        }
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

    public class Strength : Ability
    {
        public Strength(int value) : base("Сила", value)
        {
        }
    }

    public class Constitution : Ability
    {
        public Constitution(int value) : base("Телосложение", value)
        {
        }
    }

    public class Dexterity : Ability
    {
        public Dexterity(int value) : base("Ловкость", value)
        {
        }
    }

    public class Intelligence : Ability
    {
        public Intelligence(int value) : base("Интеллект", value)
        {
        }
    }

    public class Wisdom : Ability
    {
        public Wisdom(int value) : base("Мудрость", value)
        {
        }
    }

    public class Charisma : Ability
    {
        public Charisma(int value) : base("Харизма", value)
        {
        }
    }

    public class AbilitySet
    {
        public AbilitySet(
            Strength strength,
            Constitution constitution,
            Dexterity dexterity,
            Intelligence intelligence,
            Wisdom wisdom,
            Charisma charisma)
        {
            Strength = strength;
            Constitution = constitution;
            Dexterity = dexterity;
            Intelligence = intelligence;
            Wisdom = wisdom;
            Charisma = charisma;
        }

        public Strength Strength { get; }
        public Constitution Constitution { get; }
        public Dexterity Dexterity { get; }
        public Intelligence Intelligence { get; }
        public Wisdom Wisdom { get; }
        public Charisma Charisma { get; }

        public void Show()
        {
            Print(Strength);
            Print(Constitution);
            Print(Dexterity);
            Print(Intelligence);
            Print(Wisdom);
            Print(Charisma);
        }

        private void Print(Ability ability) =>
            Console.WriteLine($"{ability.Title}: {ability.Value} [{ability.Modifier}]");
    }

    //public interface IHealth
    //{
    //    int MaxHealth { get; }
    //    int Value { get; }

    //    void Hit(int damage);

    //    void Heal(int heal);
    //}

    public class Health
    {
        private int _maxHealth;
        private int _health;
        private readonly Constitution _constitution;

        public Health(int maxHealth, Constitution constitution)
        {
            _maxHealth = maxHealth;
            _constitution = constitution;
            _health = MaxHealth;
        }

        public int MaxHealth => _maxHealth + _constitution.Value;
        public int Value
        {
            get => _health;
            private set => _health = Math.Clamp(value, 0, MaxHealth);
        }

        public void Hit(int damage)
        {
            if (damage <= 0)
                return;

            Value -= damage;
        }

        public void Heal(int heal)
        {
            if (heal <= 0)
                return;

            Value += heal;
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

    public class ArmorClass : Defenсe
    {
        public ArmorClass(int value)
            : base("Класс Брони", value)
        {
        }
    }

    public class Fortitude : Defenсe
    {
        public Fortitude(int value)
            : base("Стойкость", value)
        {
        }
    }

    public class Reflex : Defenсe
    {
        public Reflex(int value)
            : base("Рефлекс", value)
        {
        }
    }

    public class Will : Defenсe
    {
        public Will(int value)
            : base("Воля", value)
        {
        }
    }

    public class DefenceSet
    {
        public DefenceSet(ArmorClass armorClass, Fortitude fortitude, Reflex reflex, Will will)
        {
            ArmorClass = armorClass;
            Fortitude = fortitude;
            Reflex = reflex;
            Will = will;
        }

        public ArmorClass ArmorClass { get; }
        public Fortitude Fortitude { get; }
        public Reflex Reflex { get; }
        public Will Will { get; }

        public void Show()
        {
            Print(ArmorClass);
            Print(Fortitude);
            Print(Reflex);
            Print(Will);
        }

        private void Print(Defenсe defence) =>
            Console.WriteLine($"{defence.Title}: {defence.Value}");
    }

    public class Rank
    {
        public Rank(int firstHealth, DefenceSet rankDefenceSet)
        {
            FirstHealth = firstHealth;
            RankDefenceSet = rankDefenceSet;
        }

        public int FirstHealth { get; }
        public DefenceSet RankDefenceSet { get; }
    }

    interface IPower
    {
        //стратегия?? или команда??
        bool CanApply(); //в лес

        void Reset(); // в лес
    }

    //оружие кидает свой кубик само!!

    public class Power
    {
        protected Ability _ability;
        protected string _defenceAbility;
        protected HitDice _hitDice;

        public Power(string title, Ability ability, string defenceType)
        {
            Title = title;
            _ability = ability;
            _defenceAbility = defenceType;
            _hitDice = new HitDice(1, 20, new RollStrategy());
        }

        public string Title { get; }

        public int GetAttack()
        {
            RollResult rollResult = _hitDice.Roll();

            return rollResult.Sum + _ability.Modifier;
        }
    }

    //public class AtWillPower : Power, IPower
    //{
    //    public AtWillPower(string title, Ability ability, string defenceType)
    //        : base(title, ability, defenceType)
    //    {
    //    }

    //    public bool CanApply() => true;

    //    public int GetHit(HitDice hitDice) =>
    //        DiceRoller.RollDices(hitDice).Sum();

    //    public void Reset() { }
    //}

    //public class EncounterPower : Power, IPower
    //{
    //    private bool _isReady;

    //    public EncounterPower(string title, Ability ability, string defenceType)
    //        : base(title, ability, defenceType)
    //    {
    //        _isReady = true;
    //    }

    //    public bool CanApply() => _isReady;

    //    public int GetHit(HitDice hitDice)
    //    {
    //        int count = 2;
    //        int damadeWeapon = 0;

    //        for (int i = 0; i < count; i++)
    //            damadeWeapon += DiceRoller.RollDiceSum(hitDice);

    //        return damadeWeapon + _ability.Modifier;
    //    }

    //    public void Reset()
    //    {
    //        _isReady = true;
    //    }
    //}

    //public class DailyPower : Power, IPower
    //{
    //    private bool _isReady;

    //    public DailyPower(string title, Ability ability, string defenceType)
    //        : base(title, ability, defenceType)
    //    {
    //        _isReady = true;
    //    }

    //    public bool CanApply() => _isReady;

    //    public int GetHit(HitDice hitDice)
    //    {
    //        int count = 3;
    //        int damadeWeapon = 0;

    //        for (int i = 0; i < count; i++)
    //            damadeWeapon += DiceRoller.RollDiceSum(hitDice);

    //        return damadeWeapon + _ability.Modifier;
    //    }

    //    public void Reset()
    //    {
    //        _isReady = true;
    //    }
    //}

    public abstract class AbilitySetBuilder
    {
        private int _count = 4;
        private int _sides = 6;
        private HitDice _hitDice;
        protected Strength _strength;
        protected Constitution _constitution;
        protected Dexterity _dexterity;
        protected Intelligence _intelligence;
        protected Wisdom _wisdom;
        protected Charisma _charisma;

        public AbilitySetBuilder()
        {
            _hitDice = new HitDice(_count, _sides, new BestResultRollStrategy(3));
        }

        public AbilitySet Create()
        {
            bool isCorrect = false;

            while (isCorrect == false)
            {
                Stack<int> values = GetValues();
                SetAbilites(values);

                int sumModifiers =
                    _strength.Modifier +
                    _constitution.Modifier +
                    _dexterity.Modifier +
                    _intelligence.Modifier +
                    _wisdom.Modifier +
                    _charisma.Modifier;

                if (sumModifiers >= 4 && sumModifiers < 8)
                    isCorrect = true;
            }

            return new AbilitySet(_strength, _constitution, _dexterity, _intelligence, _wisdom, _charisma);
        }

        protected abstract void SetAbilites(Stack<int> values);

        private Stack<int> GetValues()
        {
            int count = 6;
            int[] values = new int[count];

            for (int i = 0; i < values.Length; i++)
                values[i] = GenerateValue();

            Array.Sort(values);

            return new Stack<int>(values);
        }

        private int GenerateValue()
        {
            RollResult rollResult = _hitDice.Roll();

            return rollResult.Sum;
        }
    }

    public class WarriorAbilitySetBuilder : AbilitySetBuilder
    {
        protected override void SetAbilites(Stack<int> values)
        {
            _strength = new Strength(values.Pop());
            _constitution = new Constitution(values.Pop());
            _dexterity = new Dexterity(values.Pop());
            _wisdom = new Wisdom(values.Pop());
            _intelligence = new Intelligence(values.Pop());
            _charisma = new Charisma(values.Pop());
        }
    }

    public class PathfinderAbilitySetBuilder : AbilitySetBuilder
    {
        protected override void SetAbilites(Stack<int> values)
        {
            _strength = new Strength(values.Pop());
            _dexterity = new Dexterity(values.Pop());
            _wisdom = new Wisdom(values.Pop());
            _constitution = new Constitution(values.Pop());
            _intelligence = new Intelligence(values.Pop());
            _charisma = new Charisma(values.Pop());
        }
    }

    public class RanegerAbilitySetBuilder : AbilitySetBuilder
    {
        protected override void SetAbilites(Stack<int> values)
        {
            _dexterity = new Dexterity(values.Pop());
            _strength = new Strength(values.Pop());
            _wisdom = new Wisdom(values.Pop());
            _constitution = new Constitution(values.Pop());
            _intelligence = new Intelligence(values.Pop());
            _charisma = new Charisma(values.Pop());
        }
    }

    public class PaladinAbilitySetBuilder : AbilitySetBuilder
    {
        protected override void SetAbilites(Stack<int> values)
        {
            _strength = new Strength(values.Pop());
            _charisma = new Charisma(values.Pop());
            _wisdom = new Wisdom(values.Pop());
            _constitution = new Constitution(values.Pop());
            _intelligence = new Intelligence(values.Pop());
            _dexterity = new Dexterity(values.Pop());
        }
    }

    public class RogueAbilitySetBuilder : AbilitySetBuilder
    {
        protected override void SetAbilites(Stack<int> values)
        {
            _dexterity = new Dexterity(values.Pop());
            _strength = new Strength(values.Pop());
            _charisma = new Charisma(values.Pop());
            _intelligence = new Intelligence(values.Pop());
            _constitution = new Constitution(values.Pop());
            _wisdom = new Wisdom(values.Pop());
        }
    }

    public class WizardAbilitySetBuilder : AbilitySetBuilder
    {
        protected override void SetAbilites(Stack<int> values)
        {
            _intelligence = new Intelligence(values.Pop());
            _wisdom = new Wisdom(values.Pop());
            _dexterity = new Dexterity(values.Pop());
            _constitution = new Constitution(values.Pop());
            _charisma = new Charisma(values.Pop());
            _strength = new Strength(values.Pop());
        }
    }

    public class Character
    {
        private readonly Health _health;
        private readonly DefenceSet _defenceSet;
        private readonly Rank _rank;

        public Character(string name, AbilitySet abilitySet, Rank rank)
        {
            Name = name;
            AbilitySet = abilitySet;
            _rank = rank;
            _health = new Health(_rank.FirstHealth, AbilitySet.Constitution);
        }

        public string Name { get; }
        public AbilitySet AbilitySet { get; }
        public DefenceSet DefenсeSet => _defenceSet;
        public Health Health { get; }

        public void ShowInfo()
        {
            Console.WriteLine(Name);
            Console.WriteLine($"Здоровье - {_health.Value}");
            AbilitySet.Show();
            DefenсeSet.Show();
        }
    }

    public abstract class CharacterBuilder
    {
        public Character? Character { get; protected set; }

        public Character Build(string name)
        {
            AbilitySet abilitySet = CreateAbilitySet();
            Rank rank = CreateRank();

            return new Character(name, abilitySet, rank);
        }

        protected abstract AbilitySet CreateAbilitySet();

        protected abstract Rank CreateRank();

        //protected abstract Power SetPowerSet(); //
    }

    public class Master
    {
        public Character Create(CharacterBuilder characterBuilder) =>
           characterBuilder.Build("Жмара");
    }

    public class WarriorBuilder : CharacterBuilder
    {
        protected override AbilitySet CreateAbilitySet()
        {
            WarriorAbilitySetBuilder warriorAbilitySetBuilder = new WarriorAbilitySetBuilder();
            return warriorAbilitySetBuilder.Create();
        }

        protected override Rank CreateRank()
        {
            int firstHealth = 15;
            DefenceSet rankDefenceSet = new DefenceSet(
                new ArmorClass(0),
                new Fortitude(2),
                new Reflex(0),
                new Will(0)
                );

            return new Rank(firstHealth, rankDefenceSet);
        }
    }
}