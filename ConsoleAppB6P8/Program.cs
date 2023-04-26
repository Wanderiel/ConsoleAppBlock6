using System;
/*Net v.6
 *
 *Воин имеет увеличенный урон и большее здоровье
 *Плут имеет увеличенный шинс крита и увеличенный критический урон
 *Некромант возвращает себе часть здоровья при успешной атаке
 *Следыпыт сражается двумя оружиями в каждой руке, проводит две атаки
 *Чернокнижник при успешной атаке стакит магические метки, а при критическом попадании взрывает их
 */

namespace ConsoleAppB6P8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new();

            arena.Work();

            Console.ReadKey();
        }
    }

    public class Arena
    {
        public void Work()
        {
            Character character1 = ChooseCharacter();
            Character character2 = ChooseCharacter();

            Battle(character1, character2);

            if (character1.CurentHealth <= 0 && character2.CurentHealth <= 0)
                Console.WriteLine("\nНичья!");
            else if (character1.CurentHealth <= 0)
                Console.WriteLine($"\nПобедил {character2.Name}");
            else
                Console.WriteLine($"\nПобедил {character1.Name}");
        }

        private List<Character> GetNewRecruts() =>
            new List<Character>()
            {
                new Warrior(),
                new Necromancer(),
                new Rogue(),
                new Pathfinder(),
                new Warlock(),
            };

        private Character ChooseCharacter()
        {
            List<Character> characters = GetNewRecruts();

            Character character = null;

            bool isChosen = false;

            while (isChosen == false)
            {
                Console.Clear();

                for (int i = 1; i <= characters.Count; i++)
                {
                    Console.Write($"{i} - ");
                    characters[i - 1].ShowInfo();
                }

                Console.Write("\nВыбери бойца: ");

                string userInput = Console.ReadLine();

                if (int.TryParse(userInput, out int number))
                {
                    if (number > 0 && number <= characters.Count)
                    {
                        character = characters[number - 1];
                        isChosen = true;
                        Console.WriteLine($"Выбран {character.Name}");
                    }
                    else
                        Console.WriteLine("Неверный ввод");
                }
                else
                    Console.WriteLine("Ввести нужно число");

                Console.ReadKey();
            }

            return character;
        }

        private void Battle(Character character1, Character character2)
        {
            while (character1.CurentHealth > 0 && character2.CurentHealth > 0)
            {
                Console.Clear();
                character1.AppyAttak(character2);
                character2.AppyAttak(character1);

                Console.WriteLine();

                character1.ShowInfo();
                character2.ShowInfo();

                Thread.Sleep(1500);
            }
        }
    }

    public static class Randomizer
    {
        private static Random _random = new Random();

        public static int GetRandomNumber(int max) =>
            _random.Next(max) + 1;
    }

    public class Health
    {
        private int _maxValue;
        private int _curentValue;

        public Health(int maxHealth)
        {
            _maxValue = maxHealth;
            _curentValue = MaxValue;
        }

        public int MaxValue => _maxValue;
        public int CurentValue
        {
            get => _curentValue;
            private set => _curentValue = Math.Clamp(value, 0, MaxValue);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
                return;

            CurentValue -= damage;
        }

        public void Heal(int heal)
        {
            if (heal <= 0)
                return;

            CurentValue += heal;
        }
    }

    public class Damage
    {
        public Damage(int maxDamage = 8, int critical = 20)
        {
            MaxValue = maxDamage;
            Critical = critical;
        }

        public int MaxValue { get; }
        public int Critical { get; }

        public int GetRandomValue() =>
            Randomizer.GetRandomNumber(MaxValue);
    }

    public abstract class Character
    {
        private readonly Health _health;
        private readonly Damage _damage;
        private int _baseArmor;

        public Character(string name, Health health, int armor, Damage damage)
        {
            Name = name;
            _health = health;
            _baseArmor = armor;
            CurentArmor = _baseArmor;
            _damage = damage;
        }

        public string Name { get; }
        public int CurentHealth => _health.CurentValue;
        public int CurentArmor { get; private set; }

        public void ShowInfo()
        {
            const int Width1 = -12;
            const int Width2 = 3;

            Console.WriteLine($"{Name,Width1} | " +
                $"{_health.CurentValue,Width2} | " +
                $"{CurentArmor,Width2} | " +
                $"1-{_damage.MaxValue}");
        }

        protected void Attak(Character target)
        {
            CurentArmor = _baseArmor;
            int max = 20;
            int attak = Randomizer.GetRandomNumber(max);
            int damage;

            Console.WriteLine($"{Name} (атака по {target.Name}): {attak}");

            if (attak >= _damage.Critical)
            {
                damage = GetCriticalDamage();
                DealDamage($"Критическое попадание: урон {damage}", target, damage);
                return;
            }

            if (attak >= target.CurentArmor)
            {
                damage = GetDamage();
                DealDamage($"Попадание: урон {damage}", target, damage);
                return;
            }

            if (attak == 1)
            {
                Console.WriteLine($"Критический промах, {Name} подсавляется под следующую атаку");

                if (CurentArmor == _baseArmor)
                    CurentArmor--;
            }
            else
                Console.WriteLine("Промах!");
        }

        public virtual void AppyAttak(Character target) =>
            Attak(target);

        public void TakeDamage(int damage) => 
            _health.TakeDamage(damage);

        protected virtual int GetCriticalDamage() =>
            _damage.MaxValue;

        protected virtual int GetDamage() =>
            _damage.GetRandomValue();

        protected virtual void DealDamage(string mesage, Character target, int damage)
        {
            Console.WriteLine(mesage);
            target.TakeDamage(damage);
        }

        protected int GetMaxDamage() =>
            _damage.MaxValue;

        protected int GetRandomDamageValue() =>
            _damage.GetRandomValue();

        protected void Heal(int heal) => 
            _health.Heal(heal);
    }

    public class Warrior : Character
    {
        private int _damageBonus;

        public Warrior()
            : base("Воин", new Health(100), 13, new Damage(10))
        {
            _damageBonus = 2;
        }

        protected override int GetCriticalDamage() =>
            GetMaxDamage() + _damageBonus;

        protected override int GetDamage() =>
            GetRandomDamageValue() + _damageBonus;
    }

    public class Rogue : Character
    {
        public Rogue()
            : base("Плут", new Health(89), 12, new Damage(6, 18))
        {
        }

        protected override int GetCriticalDamage() =>
            GetMaxDamage() + GetRandomDamageValue();
    }

    public class Necromancer : Character
    {
        private int _percentHeal;

        public Necromancer()
            : base("Некромант", new Health(82), 11, new Damage())
        {
            _percentHeal = 50;
        }

        protected override void DealDamage(string mesage, Character target, int damage)
        {
            Console.WriteLine(mesage);
            target.TakeDamage(damage);

            RestoreHealth(damage);
        }

        private void RestoreHealth(int damage)
        {
            int fullPercent = 100;
            int heal = (_percentHeal * damage) / fullPercent;

            if (heal > 0)
            {
                Heal(heal);
                Console.WriteLine($"{Name} восстанавливает здоровье: {heal}");
            }
        }
    }

    public class Pathfinder : Character
    {
        public Pathfinder()
            : base("Следопыт", new Health(90), 12, new Damage())
        {
        }

        public override void AppyAttak(Character target)
        {
            Attak(target);
            Attak(target);
        }
    }

    public class Warlock : Character
    {
        private int _marks;

        public Warlock()
            : base("Чернокнижник", new Health(82), 11, new Damage(10))
        {
            _marks = 0;
        }

        protected override int GetCriticalDamage()
        {
            int damage = GetMaxDamage() + _marks;
            _marks = 0;

            return damage;
        }

        protected override int GetDamage()
        {
            _marks++;

            return base.GetDamage();
        }
    }
}