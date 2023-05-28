//GlobalUsing
//Net v.6

namespace ConsoleAppB6P11
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Aquarium aquarium = new Aquarium(20);
            aquarium.Work();

            Console.WriteLine("\nИгра окончена.");
            Console.ReadKey();
        }
    }

    public interface IFish
    {
        IFish Clone();

        void ShowInfo();
    }

    public static class Random
    {
        private static readonly System.Random _random = new System.Random();

        public static int GetNumber(int max) => _random.Next(max);

        public static double NextDouble() => _random.NextDouble();
    }

    public class Aquarium
    {
        private readonly int _size;
        private readonly FishGenerator _fishGenerator;
        private readonly List<IFish> _fish;
        private readonly double _agingRate;
        private int _toxicity;

        public Aquarium(int size)
        {
            _size = size;
            _fishGenerator = new FishGenerator();
            _fish = new List<IFish>();
            _agingRate = 0.1;
            _toxicity = 0;
        }

        private bool IsAlive => _toxicity < _size;

        public void Work()
        {
            const ConsoleKey CommandAddFish = ConsoleKey.Insert;
            const ConsoleKey CommandRemoveFish = ConsoleKey.Delete;
            const ConsoleKey CommandClear = ConsoleKey.PageUp;
            const ConsoleKey CommandExit = ConsoleKey.Escape;

            bool isWork = true;
            int waitingTime = 500;

            while (isWork)
            {
                Console.Clear();
                Console.WriteLine($"[{CommandAddFish}] - добавить рыбку");
                Console.WriteLine($"[{CommandRemoveFish}] - удалить рыбку");
                Console.WriteLine($"[{CommandClear}] - очистить аквариум");
                Console.WriteLine($"[{CommandExit}] - уйти от аквариума\n");

                ShowInfo();

                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    switch (key)
                    {
                        case ConsoleKey.Insert:
                            Add();
                            break;

                        case ConsoleKey.Delete:
                            Remove();
                            break;

                        case ConsoleKey.PageUp:
                            Clear();
                            break;

                        case ConsoleKey.Escape:
                            isWork = false;
                            break;
                    }
                }

                GrowOldFish();

                if (IsAlive == false)
                    isWork = false;

                Thread.Sleep(waitingTime);
            }
        }

        private void Add()
        {
            if (_fish.Count >= _size)
                return;

            _fish.Add(_fishGenerator.Create());
        }

        private void Remove()
        {
            Console.WriteLine("Введите номер рыбки для удаления из аквариума");
            string userInput = Console.ReadLine();

            if (int.TryParse(userInput, out int index) == false)
                return;

            if (index < 1 || index > _fish.Count)
                return;

            _fish.RemoveAt(index - 1);
        }

        private void Clear() => _fish.Clear();

        private void GrowOldFish()
        {
            CalculateToxicity();

            foreach (Fish fish in _fish)
                fish.GrowOld(_agingRate * (_toxicity + 1));
        }

        private void ShowInfo()
        {
            const int Width = 2;

            for (int i = 1; i <= _fish.Count; i++)
            {
                Console.Write($"{i,Width} | ");
                _fish[i - 1].ShowInfo();
            }

            Console.WriteLine($"\nТоксичность: {_toxicity}");
        }

        private void CalculateToxicity()
        {
            int toxicity = 0;

            foreach (Fish fish in _fish)
                if (fish.IsAlive == false)
                    toxicity++;

            _toxicity = toxicity;
        }
    }

    public class FishGenerator
    {
        private List<IFish> _fish;

        public FishGenerator()
        {
            _fish = new List<IFish>()
            {
                new Betta(),
                new Barbus(),
                new Glofish(),
                new Parrotfish(),
                new Angelfish(),
                new BlueDolphin(),
            };
        }

        public IFish Create() => _fish[Random.GetNumber(_fish.Count)].Clone();
    }

    public abstract class Fish : IFish
    {
        private readonly string _name;
        private double _maxLifeTime;
        private double _lifeTime;

        public Fish(string name, double maxLifeTime, ConsoleColor color)
        {
            _name = name;
            _maxLifeTime = maxLifeTime;
            _lifeTime = Random.NextDouble();
            Color = color;
        }

        public bool IsAlive => _lifeTime < _maxLifeTime;
        public ConsoleColor Color { get; }

        public abstract IFish Clone();

        public void GrowOld(double age)
        {
            if (IsAlive)
                _lifeTime += age;
        }

        public void ShowInfo()
        {
            const int Width1 = -15;
            const int Width2 = 2;
            int lifeTime = (int)_lifeTime;
            string status = IsAlive ? "жива" : "мертва";

            Console.ForegroundColor = Color;
            Console.WriteLine($"{_name,Width1} | {lifeTime,Width2} | {status}");
            Console.ResetColor();
        }
    }

    public class Betta : Fish
    {
        public Betta()
            : base("Петушок", 3, ConsoleColor.Red)
        {
        }

        public override IFish Clone() => new Betta();
    }

    public class Barbus : Fish
    {
        public Barbus()
            : base("Барбус", 4, ConsoleColor.DarkGray)
        {
        }

        public override IFish Clone() => new Barbus();
    }

    public class Glofish : Fish
    {
        public Glofish()
            : base("Глофиш", 5, ConsoleColor.Green)
        {
        }

        public override IFish Clone() => new Glofish();
    }

    public class Parrotfish : Fish
    {
        public Parrotfish()
            : base("Рыба-попугай", 10, ConsoleColor.DarkYellow)
        {
        }

        public override IFish Clone() => new Parrotfish();
    }

    public class Angelfish : Fish
    {
        public Angelfish()
            : base("Скалярия", 15, ConsoleColor.Magenta)
        {
        }

        public override IFish Clone() => new Angelfish();
    }

    public class BlueDolphin : Fish
    {
        public BlueDolphin()
            : base("Голубой дельфин", 15, ConsoleColor.Cyan)
        {
        }

        public override IFish Clone() => new BlueDolphin();
    }
}