using System;
//Net v.6

namespace ConsoleAppB6P8
{
    internal class Program
    {
        static void Main(string[] args)
        {

            AbilitySetBuilder abilitySetBuilder = new AbilitySetBuilder();

            AbilitySet abilitySet = abilitySetBuilder.GetWariorSet();

            Console.ReadKey();
        }
    }

    public static class AbilityType
    {
        public const string Strength = "Сила";
        public const string Constitution = "Телосложение";
        public const string Dextery = "Ловкость";
        public const string Intellect = "Интеллект";
        public const string Wishdom = "Мудрость";
        public const string Charisma = "Харизма";
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
        private const int NumberOfAbilities = 6;

        public AbilitySet(IEnumerable<Ability> abilities)
        {
            if (abilities == null)
                throw new ArgumentNullException(nameof(abilities));

            if (abilities.Count() != NumberOfAbilities)
                throw new InvalidOperationException("Неверное количество Ability");

            foreach (Ability ability in abilities)
            {
                switch (ability.Title)
                {
                    case AbilityType.Strength:
                        Strength = ability;
                        break;

                    case AbilityType.Constitution:
                        Constitution = ability;
                        break;

                    case AbilityType.Dextery:
                        Dextery = ability;
                        break;

                    case AbilityType.Intellect:
                        Intellect = ability;
                        break;

                    case AbilityType.Wishdom:
                        Wishdom = ability;
                        break;

                        case AbilityType.Charisma: 
                        Charisma = ability;
                        break;
                }
            }
        }

        public AbilitySet(
            int strength,
            int constitution,
            int dextery,
            int intellect,
            int wishdom,
            int charisma)
        {
            Strength = new Ability(AbilityType.Strength, strength);
            Constitution = new Ability(AbilityType.Constitution, constitution);
            Dextery = new Ability(AbilityType.Dextery, dextery);
            Intellect = new Ability("Интеллект", intellect);
            Wishdom = new Ability("Мудрость", wishdom);
            Charisma = new Ability("Хоризма", charisma);
        }

        public Ability Strength { get; }
        public Ability Constitution { get; }
        public Ability Dextery { get; }
        public Ability Intellect { get; }
        public Ability Wishdom { get; }
        public Ability Charisma { get; }
    }

    public class Character
    {
        private AbilitySet _abilitySet;
        public Character(string name, AbilitySet abilitySet)
        {
            Name = name;
            _abilitySet = abilitySet;
        }

        public string Name { get; }

        public void Show()
        {
            Console.WriteLine(Name);
        }
    }

    public class Warrior : Character
    {
        public Warrior(string name, AbilitySet abilitySet)
            : base(name, abilitySet)
        {

        }
    }

    public class AbilitySetBuilder
    {
        Dice _dice = new Dice();

        private int GenerateValue()
        {
            int count = 4;
            int faces = 6;
            int[] results = _dice.RollFew(count, faces);

            return results.Sum() - results.Min();
        }

        private int[] GetValues()
        {
            int[] values = new int[6];
            bool isCorrect = false;
            double average = 10.0 * values.Length;
            int divider = 2;

            while (isCorrect == false)
            {
                int sum = 0;

                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = GenerateValue();
                    sum += values[0];
                }

                int sumModifiers = (int)Math.Round(
                    (sum - average) / divider,
                    MidpointRounding.ToNegativeInfinity
                    );

                if (sumModifiers >= 4 && sumModifiers < 8)
                    isCorrect = true;
            }

            Array.Sort(values);

            return values;
        }

        public AbilitySet GetWariorSet()
        {
            int[] values = GetValues();

            return new AbilitySet(
                values[5],
                values[4],
                values[3],
                values[2],
                values[1],
                values[0]
                );
        }
    }

    public class CharacterBuilder
    {
        private List<Character> _characters = new List<Character>();

        public void SetCharacters()
        {
            _characters.Add(new Warrior("Тор", new AbilitySet(16, 14, 13, 10, 13, 11)));
        }

        public void ShowCharacter()
        {
            foreach (Character character in _characters)
                character.Show();
        }

        public Character ChooseHero()
        {
            Character hero = null;

            ShowCharacter();

            Console.Write("Номер гоероя? ");
            string userInput = Console.ReadLine();

            if (int.TryParse(userInput, out int index))
            {
                if (index < 0 || index >= _characters.Count)
                {
                    Console.WriteLine("горйо не найден");
                    return null;
                }

                hero = _characters[index];
            }

            return hero;
        }
    }

    public class Dice
    {
        Random _random = new Random();

        public int RollOne(int faces) =>
            _random.Next(faces) + 1;

        public int[] RollFew(int count, int faces)
        {
            int[] values = new int[count];

            for (int i = 0; i < count; i++)
                values[i] = RollOne(faces);

            return values;
        }
    }
}