//GlobalUsing
//Net v.6

namespace ConsoleAppB6P13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            World virtualSpace = new World();

            virtualSpace.Move();

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

        public Detail Give() => HasDetail ? _queue.Dequeue() : null;

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
            {
                if (_containers.ContainsKey(detail.Name) == false)
                    return false;

                if (_containers[detail.Name].HasDetail == false)
                    return false;
            }

            foreach (Detail detail in brokenDetails)
                details.Add(_containers[detail.Name].Give());

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
        public Storage Create(Dictionary<string, int> parts)
        {
            Dictionary<string, Container> containers = new Dictionary<string, Container>();
            int minCount = 5;
            int maxCount = 16;

            foreach (var config in parts)
            {
                Container container = new Container();
                int detailCount = Random.GetNumber(minCount, maxCount);

                for (int i = 0; i < detailCount; i++)
                    container.Take(new Detail(config.Key, config.Value));

                containers[config.Key] = container;
            }

            return new Storage(containers);
        }
    }

    public class CarService
    {
        private int _money;
        private readonly int _fine;
        private readonly int _penalty;
        private readonly Storage _storage;
        private Order _order;

        public CarService(Storage storage)
        {
            _money = 10000;
            _fine = 10000;
            _penalty = 20000;
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

            Console.WriteLine("\nУ вас новый клиент!");
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
                    TakeFine();
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
                Console.WriteLine($"Хорошая работа! Заработано {_order.Amount}");
            }
            else
            {
                _money -= _penalty;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"У вас нет нужных деталей, работа не выполнена.");
                Console.WriteLine($"Клиенту выплачена неустойка {_penalty}");
            }

            Console.ResetColor();
            Console.ReadKey();
        }

        private void TakeFine()
        {
            _money -= _fine;

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Клиент не обслужен. Выплачен штраф: {_fine}");
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

    public class CarServiceFactory
    {
        private readonly StorageFactory _storageFactory = new StorageFactory();

        public CarService Create(Dictionary<string, int> parts)
        {
            Storage storage = _storageFactory.Create(parts);

            return new CarService(storage);
        }
    }

    public class Order
    {
        private readonly double _jobMultiplier;
        private readonly List<Detail> _replacementDetails;

        public Order(List<Detail> replacementDetails)
        {
            _jobMultiplier = 1.15;
            _replacementDetails = replacementDetails;
            Amount = CalculateAmount();
        }

        public List<Detail> ReplacementDetails => new List<Detail>(_replacementDetails);
        public int Amount { get; }

        public void Print()
        {
            Console.WriteLine("Детали под замену:");

            if (_replacementDetails.Count == 0)
            {
                Console.WriteLine("Нет");
                return;
            }

            for (int i = 0; i < _replacementDetails.Count; i++)
            {
                int index = i + 1;
                Console.Write($"{index}. ");
                Console.WriteLine($"{_replacementDetails[i].Name}");
            }

            Console.WriteLine($"Стоимость ремонта: {Amount}");
        }

        private int CalculateAmount() =>
            (int)(_replacementDetails.Sum(detail => detail.Price) * _jobMultiplier);
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
        public Car Create(Dictionary<string, int> parts)
        {
            List<Detail> details = new List<Detail>();

            foreach (var config in parts)
            {
                Detail detail = new Detail(config.Key, config.Value);

                if (Random.NextBool())
                    detail.Break();

                details.Add(detail);
            }

            return new Car(details);
        }
    }

    public class World
    {
        private readonly Dictionary<string, int> _parts;
        private readonly CarFactory _carFactory = new CarFactory();
        private readonly CarServiceFactory _carServiceFactory = new CarServiceFactory();

        public World()
        {
            _parts = new Dictionary<string, int>()
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

        public void Move()
        {
            bool isWork = true;
            CarService carService = _carServiceFactory.Create(new Dictionary<string, int>(_parts));

            while (isWork)
            {
                Car car = _carFactory.Create(new Dictionary<string, int>(_parts));

                carService.Work(car);

                if (carService.IsBankrupt || carService.IsClosed)
                    isWork = false;
            }
        }
    }
}