//GlobalUsing
//Net v.6

namespace ConsoleAppB6P13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VirtualSpace timer = new VirtualSpace();

            timer.Tick();

            Console.WriteLine("\nМы закончили :)");
            Console.ReadKey();
        }
    }

    public static class Random
    {
        private static readonly System.Random _random = new System.Random();

        public static int GetNumber(int min, int max) => _random.Next(min, max);

        public static bool NextBool() => _random.Next(2) == 1;
    }

    public class Config
    {
        static Config()
        {
            Details = new Dictionary<string, int>()
            {
                { "Двигатель", 100000 },
                { "Кузов", 80000 },
                { "Рама", 30000 },
                { "Колеса", 10000 },
                { "Фары передние", 8000 },
                { "Фары задние", 7000 },
                { "Тормоза", 5000 },
                { "Аккумулятор", 500 },
            };
        }

        public static Dictionary<string, int> Details { get; }
        public static int Penalty = 20000;
    }

    public class Detail
    {
        public Detail(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public int Price { get; }
        public bool IsBroken { get; private set; } = false;

        public void Break() => IsBroken = true;
    }

    public class Container
    {
        private Queue<Detail> _queue = new Queue<Detail>();

        public int DetailCount => _queue.Count;
        public bool HasDetail => DetailCount > 0;

        public Detail Get() => _queue.Dequeue();

        public void Take(Detail detail) => _queue.Enqueue(detail);
    }

    public class Storage
    {
        private Dictionary<string, Container> _containers;

        public Storage(Dictionary<string, Container> containers)
        {
            _containers = containers;
        }

        public bool TryGet(List<Detail> brokenDetails, out List<Detail> details)
        {
            details = new List<Detail>();

            foreach (Detail detail in brokenDetails)
                if (_containers[detail.Name].HasDetail == false)
                    return false;

            foreach (Detail detail in brokenDetails)
                details.Add(_containers[detail.Name].Get());

            return true;
        }

        public void ShowInfo()
        {
            int index = 1;
            Console.WriteLine("Деталей на складе:");

            foreach (var container in _containers)
            {
                Console.Write($"{index}. ");
                Console.WriteLine($"{container.Key}: {container.Value.DetailCount}");
                index++;
            }
        }
    }

    public class StorageFactory
    {
        public Storage Create()
        {
            Dictionary<string, Container> containers = new Dictionary<string, Container>();
            int min = 5;
            int max = 16;

            foreach (var config in Config.Details)
            {
                Container container = new Container();
                int count = Random.GetNumber(min, max);

                for (int i = 0; i < count; i++)
                    container.Take(new Detail(config.Key, config.Value));

                containers[config.Key] = container;
            }

            return new Storage(containers);
        }
    }

    public class CarService
    {
        private int _money;
        private readonly Storage _storage;
        private Order _order;

        public CarService(Storage storage)
        {
            _money = 10000;
            _storage = storage;
        }

        public bool IsBankrupt => _money < 0;
        public bool IsClosed { get; private set; } = false;

        public void Work(Car car)
        {
            const ConsoleKey CommandRepair = ConsoleKey.R;
            const ConsoleKey CommandNext = ConsoleKey.N;
            const ConsoleKey CommandExit = ConsoleKey.Q;

            _order = new Order(car.BrokenDetails);

            Console.Clear();
            ShowInfo();

            Console.WriteLine("\nУ нас новый клиент!");
            _order.Print();

            Console.WriteLine($"\n[{CommandRepair}] - выполнить ремонт");
            Console.WriteLine($"[{CommandNext}] - взять следующего клиента");
            Console.WriteLine($"[{CommandExit}] - закрыть мастерскую");


            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case CommandRepair:
                    PerformRepairs(car);
                    break;

                case CommandNext:
                    TakePenalty();
                    break;

                case CommandExit:
                    IsClosed = true;
                    break;
            }
        }

        private void PerformRepairs(Car car)
        {
            if (_storage.TryGet(_order.ReplacementDetails, out List<Detail> details))
            {
                car.Repair(details);
                _money += _order.Amount;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Хорошая работа! Заработано ");
            }
            else
            {
                _money -= _order.Amount;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"У вас нет нужных деталей. Выплачена неустойка ");
            }

            Console.WriteLine(_order.Amount);
            Console.ResetColor();
            Console.ReadKey();
        }

        private void TakePenalty()
        {
            _money -= Config.Penalty;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Клиент не обслужен. Выплачен штраф: {Config.Penalty}");
            Console.ResetColor();
            Console.ReadKey();
        }

        private void ShowInfo()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Ваши деньги: {_money}");
            _storage.ShowInfo();
            Console.ResetColor();
        }
    }

    public class Order
    {
        private readonly double _jobMultiplier;

        public Order(List<Detail> replacementDetails)
        {
            _jobMultiplier = 1.15;
            ReplacementDetails = replacementDetails;
            Amount = CalculateAmount();
        }

        public List<Detail> ReplacementDetails { get; }
        public int Amount { get; }

        public void Print()
        {
            Console.WriteLine("Детали под замену:");

            if (ReplacementDetails.Count == 0)
            {
                Console.WriteLine("Нет");
                return;
            }

            for (int i = 0; i < ReplacementDetails.Count; i++)
            {
                int index = i + 1;
                Console.Write($"{index}. ");
                Console.WriteLine($"{ReplacementDetails[i].Name}");
            }

            Console.WriteLine($"Стоимость ремонта: {Amount}");
        }

        private int CalculateAmount() =>
            (int)(ReplacementDetails.Sum(detail => detail.Price) * _jobMultiplier);
    }

    public class Car
    {
        private readonly List<Detail> _details;

        public Car(List<Detail> details)
        {
            _details = details;
        }

        public bool NeedRepairs => BrokenDetails.Count > 0;
        public List<Detail> BrokenDetails => _details.Where(detail => detail.IsBroken == true).ToList();

        public void Repair(List<Detail> details)
        {
            _details.Except(BrokenDetails);
            _details.Union(details);
        }
    }

    public class CarFactory
    {
        public Car Create()
        {
            List<Detail> details = new List<Detail>();

            foreach (var config in Config.Details)
            {
                Detail detail = new Detail(config.Key, config.Value);

                if (Random.NextBool())
                    detail.Break();

                details.Add(detail);
            }

            return new Car(details);
        }
    }

    public class VirtualSpace
    {
        private readonly CarFactory _carFactory = new CarFactory();
        private readonly StorageFactory _storageFactory = new StorageFactory();
        private readonly CarService _carService;
        private int _time;

        public VirtualSpace()
        {
            _carService = new CarService(_storageFactory.Create());
        }

        public void Tick()
        {
            bool isWork = true;

            while (isWork)
            {
                Car car = _carFactory.Create();

                _carService.Work(car);

                Thread.Sleep(_time);

                if (_carService.IsBankrupt || _carService.IsClosed)
                    isWork = false;
            }
        }
    }
}