using System;
//Net v.6
//По мотивам Dungeons & Dragons 4-ой редакции

namespace ConsoleAppB6P8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BaseCharacterBuilder_old builder = new BaseCharacterBuilder_old();

            Character player = builder.CreatePathfinder();

            player.ShowInfo();

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

    public static class DefensesType
    {
        public const string ArmorClass = "Класс Брони";
        public const string Fortitude = "Стойкость";
        public const string Reflex = "Рефлекс";
        public const string Will = "Воля";
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
            CheckAbilities(abilities);

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

        private void CheckAbilities(Dictionary<string, Ability> abilities)
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
                if (abilities.ContainsKey(title) == false)
                    throw new Exception($"Нет нужной способности - {title}");
        }
    }

    public class Defense
    {
        public Defense(string title, int value)
        {
            Title = title;
            Value = value;
        }

        public string Title { get; }
        public int Value { get; }
    }

    public class DefenseSet
    {
        private readonly Dictionary<string, Defense> _defenses;

        public DefenseSet(Dictionary<string, Defense> defenses)
        {
            CheckDefenses(defenses);

            _defenses = new Dictionary<string, Defense>(defenses);
        }

        public Defense ArmorClass => _defenses[DefensesType.ArmorClass];
        public Defense Fortitude => _defenses[DefensesType.Fortitude];
        public Defense Reflex => _defenses[DefensesType.Reflex];
        public Defense Will => _defenses[DefensesType.Will];

        public void ShowInfo()
        {
            foreach (var defense in _defenses)
                Console.WriteLine($"{defense.Key}: {defense.Value.Value}");
        }

        private void CheckDefenses(Dictionary<string, Defense> defenses)
        {
            string[] checkTitles = new string[] {
                DefensesType.ArmorClass,
                DefensesType.Fortitude,
                DefensesType.Reflex,
                DefensesType.Will,
            };

            foreach (string title in checkTitles)
                if (defenses.ContainsKey(title) == false)
                    throw new Exception($"Нет нужной защиты - {title}");
        }
    }

    public abstract class BaseAbilitySetBuilder
    {
        private Dice _dice = new Dice();

        public AbilitySet GetAbilitySet()
        {
            Dictionary<string, Ability> abilities = new Dictionary<string, Ability>();
            bool isCorrect = false;
            string[] titles = GetAbilites();

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

        private int GenerateValue()
        {
            int count = 4;
            int faces = 6;
            int[] results = _dice.RollFew(count, faces);

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

    public class WarriorAbilitySetBuilder : BaseAbilitySetBuilder
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

    public class PathfinderAbilitySetBuilder : BaseAbilitySetBuilder
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

    public class RanegerAbilitySetBuilder : BaseAbilitySetBuilder
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

    public class PaladinAbilitySetBuilder : BaseAbilitySetBuilder
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

    public class RogueAbilitySetBuilder : BaseAbilitySetBuilder
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

    public class WizardAbilitySetBuilder : BaseAbilitySetBuilder
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

    public class DefenseBuilder
    {
        public DefenseSet GetDefenceSet(
            AbilitySet abilitySet,
            Dictionary<string, int> rankBonuses,
            Dictionary<string, int> defenseBonuses
            )
        {
            int baseValue = 10;
            Dictionary<string, Defense> defenses = new Dictionary<string, Defense>();
            Dictionary<string, int> defenseValeus = new Dictionary<string, int>()
            {
                { DefensesType.ArmorClass, Math.Max(abilitySet.Dexterity.Modifier, abilitySet.Intelligence.Modifier)},
                { DefensesType.Fortitude, Math.Max(abilitySet.Strength.Modifier, abilitySet.Constitution.Modifier)},
                { DefensesType.Reflex, Math.Max(abilitySet.Dexterity.Modifier, abilitySet.Intelligence.Modifier)},
                { DefensesType.Will, Math.Max(abilitySet.Wisdom.Modifier, abilitySet.Charisma.Modifier)},
            };

            foreach (string key in defenseValeus.Keys)
            {
                defenseValeus[key] += baseValue + rankBonuses[key] + defenseBonuses[key];
                defenses.Add(key, new Defense(key, defenseValeus[key]));
            }

            return new DefenseSet(defenses);
        }
    }

    public class Character
    {
        private AbilitySet _abilitySet;
        private DefenseSet _defenseSet;

        public Character()
        {
        }

        public Character(string name, AbilitySet abilitySet, DefenseSet defenseSet)
        {
            Name = name;
            _abilitySet = abilitySet;
            _defenseSet = defenseSet;
        }

        public string Name { get; }

        public void ShowInfo()
        {
            Console.WriteLine(Name);
            _abilitySet.ShowInfo();
            _defenseSet.ShowInfo();
        }
    }

    public class Warrior : Character
    {
        public Warrior(string name, AbilitySet abilitySet, DefenseSet defenseSet)
            : base(name, abilitySet, defenseSet)
        {
        }
    }

    public class BaseCharacterBuilder_old
    {
        private List<Character> _characters = new List<Character>();
        DefenseBuilder _defenseBuilder = new DefenseBuilder();
        private AbilitySet _abilitySet;
        private DefenseSet _defenseSet;

        public Character CreateWarrior()
        {
            WarriorAbilitySetBuilder warriorAbilitySetBuilder = new WarriorAbilitySetBuilder();
            Dictionary<string, int> defenseBonuses = new Dictionary<string, int>()
            {
                { DefensesType.ArmorClass, 6 },
                { DefensesType.Fortitude, 0 },
                { DefensesType.Reflex, 0 },
                { DefensesType.Will, 0 },
            };
            Dictionary<string, int> rankBonuses = new Dictionary<string, int>()
            {
                { DefensesType.ArmorClass, 2 },
                { DefensesType.Fortitude, 0 },
                { DefensesType.Reflex, 0 },
                { DefensesType.Will, 0 },
            };

            _abilitySet = warriorAbilitySetBuilder.GetAbilitySet();
            _defenseSet = _defenseBuilder.GetDefenceSet(_abilitySet, defenseBonuses, rankBonuses);
            
            return new Character("Тор", _abilitySet, _defenseSet);
        }

        public Character CreatePathfinder()
        {
            PathfinderAbilitySetBuilder pathfinderAbilitySetBuilder = new PathfinderAbilitySetBuilder();
            Dictionary<string, int> defenseBonuses = new Dictionary<string, int>()
            {
                { DefensesType.ArmorClass, 2 },
                { DefensesType.Fortitude, 0 },
                { DefensesType.Reflex, 0 },
                { DefensesType.Will, 0 },
            };
            Dictionary<string, int> rankBonuses = new Dictionary<string, int>()
            {
                { DefensesType.ArmorClass, 0 },
                { DefensesType.Fortitude, 1 },
                { DefensesType.Reflex, 1 },
                { DefensesType.Will, 0 },
            };

            _abilitySet = pathfinderAbilitySetBuilder.GetAbilitySet();
            _defenseSet = _defenseBuilder.GetDefenceSet(_abilitySet, defenseBonuses, rankBonuses);

            return new Character("Вандериэл", _abilitySet, _defenseSet);
        }


        public void SetCharacters()
        {
            //_characters.Add(new Warrior("Тор", new AbilitySet(16, 14, 13, 10, 13, 11)));
        }

        public void ShowCharacter()
        {
            foreach (Character character in _characters)
                character.ShowInfo();
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

    abstract class BaseCharacterBuilder
    {
        public Character Character { get; private set; }

        public void CreateCharacter() =>
            Character = new Character();

        public abstract void SetAbilitySet();

        public abstract int SetDefenseSet();

        public abstract void SetPowerSet();
    }

    public class WarriorBuilder : BaseCharacterBuilder
    {
        public override void SetAbilitySet()
        {
            throw new NotImplementedException();
        }

        public override int SetDefenseSet()
        {
            throw new NotImplementedException();
        }

        public override void SetPowerSet()
        {
            throw new NotImplementedException();
        }
    }
}