using System;
using static System.Net.Mime.MediaTypeNames;
//Net v.6

namespace ConsoleAppB6P8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Arena arena = new Arena();

            arena.Attak();

            Console.ReadKey();
        }
    }

    public class Arena
    {
        Warrior _warrior = new Warrior(new Health(100), 13, new Damage(10));
        Necromancer _necromancer = new Necromancer(new Health(82), 11, new Damage());
        Rogue _rogue = new Rogue(new Health(89), 12, new Damage());

        public void Attak()
        {
            Character character1 = _warrior;
            Character character2 = _rogue;

            while (character1.Health > 0 && character2.Health > 0)
            {
                Console.Clear();
                character1.Attak(character2);
                character2.Attak(character1);

                Console.WriteLine();

                character1.ShowInfo();
                character2.ShowInfo();

                Thread.Sleep(2000);
            }
        }
    }

    public static class Randomizer
    {
        private static Random _random;
        static Randomizer()
        {
            _random = new Random();
        }

        public static int GetRandom(int max) =>
            _random.Next(max) + 1;
    }

    public class Health
    {
        private int _maxHealth;
        private int _health;

        public Health(int maxHealth)
        {
            _maxHealth = maxHealth;
            _health = MaxHealth;
        }

        public int MaxHealth => _maxHealth;
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

    public class Damage
    {
        public Damage(int maxDamage = 8)
        {
            Max = maxDamage;
        }

        public int Max { get; }

        public int Deal() =>
            Randomizer.GetRandom(Max);
    }

    public abstract class Character
    {
        protected readonly Health _health;
        protected readonly Damage _damage;
        protected readonly int _armor;

        public Character(string name, Health health, int armor, Damage damage)
        {
            Name = name;
            _health = health;
            _armor = armor;
            Armor = _armor;
            _damage = damage;
        }

        public string Name { get; }
        public int Health => _health.Value;
        public int Armor { get; private set; }

        public void ShowInfo() =>
            Console.WriteLine($"{Name} | {_health.Value} | {Armor} | 1-{_damage.Max}");

        public virtual void Attak(Character target)
        {
            Armor = _armor;
            int max = 20;
            int attak = Randomizer.GetRandom(max);
            int damage;

            Console.WriteLine($"{Name} (атака по {target.Name}): {attak}");

            if (attak == max)
            {
                damage = GetCriticalDamage();
                DealDamage($"Критическое попадание: урон {damage}", target, damage);
                return;
            }

            if (attak >= target.Armor)
            {
                damage = GetDamage();
                DealDamage($"Попадание: урон {damage}", target, damage);
                return;
            }

            if (attak == 1)
            {
                Console.WriteLine($"Критический промах, {Name} подсавляется под следующую атаку");
                Armor--;
            }
            else
                Console.WriteLine("Промах!");
        }

        public void TakeDamage(int damage)
        {
            _health.Hit(damage);
        }

        protected virtual int GetCriticalDamage() =>
            _damage.Max;

        protected virtual int GetDamage() =>
            _damage.Deal();

        protected virtual void DealDamage(string mesage, Character target, int damage)
        {
            Console.WriteLine(mesage);
            target.TakeDamage(damage);
        }
    }

    public class Warrior : Character
    {
        private int _damageBonus;
        public Warrior(Health health, int armor, Damage damage)
            : base("Воин", health, armor, damage)
        {
            _damageBonus = 2;
        }

        protected override int GetCriticalDamage() =>
            _damage.Max + _damageBonus;

        protected override int GetDamage() =>
            _damage.Deal() + _damageBonus;
    }

    public class Rogue : Character
    {
        public Rogue(Health health, int armor, Damage damage)
            : base("Плут", health, armor, damage)
        {
        }

        protected override int GetCriticalDamage() =>
            _damage.Max + _damage.Deal();
    }

    public class Necromancer : Character
    {
        private int _percentHeal;
        public Necromancer(Health health, int armor, Damage damage)
            : base("Некромант", health, armor, damage)
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
                _health.Heal(heal);
                Console.WriteLine($"{Name} восстанавливает здоровье: {heal}");
            }
        }
    }
}