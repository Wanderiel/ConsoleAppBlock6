using System;
using System.Text.Json;

namespace ConsoleAppB6P6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Shop shop = new Shop();

            shop.Work();

            Console.Clear();
            Console.WriteLine("Всего доброго!");

            Console.ReadKey();
        }
    }

    public class Shop
    {
        private Random _random = new Random();
        private List<Item> _items = new List<Item>();
        private Player? _player;
        private Trader? _trader;

        public Shop()
        {
            LoadItems();
            CreatePersons();
        }

        public void Work()
        {
            const string CommandShowProducts = "1";
            const string CommandBuyProduct = "2";
            const string CommandShowInventory = "3";
            const string CommandExit = "4";

            bool isTrading = true;

            while (isTrading)
            {
                int money = _player.GetMoney();

                Console.Clear();
                Console.WriteLine($"Лавка \"Ломаная подкова\"" +
                    $"\n{CommandShowProducts}. Посмотреть товары" +
                    $"\n{CommandBuyProduct}. Купить товар" +
                    $"\n{CommandShowInventory}. Посмотреть инвентарь" +
                    $"\n{CommandExit}. Покинуть лавку" +
                    $"\n\nЗолота в наличии - {money}");

                switch (Console.ReadLine())
                {
                    case CommandShowProducts:
                        ShowProducts();
                        break;

                    case CommandBuyProduct:
                        BuyProduct(money);
                        break;

                    case CommandShowInventory:
                        ShowInventory(money);
                        break;

                    case CommandExit:
                        isTrading = false;
                        break;
                
                }

                Console.ReadKey();
            }
        }

        private void LoadItems()
        {
            string path = "items.json";

            Stream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            _items = JsonSerializer.Deserialize<List<Item>>(fileStream);
            fileStream.Close();
        }

        private void CreatePersons()
        {
            int minMoney = 301;
            int maxMoney = 1001;

            _player = new Player("Дровакин", 160, _random.Next(minMoney, maxMoney));
            _trader = new Trader("Эль'Ри'сад");

            FillInventoryRandomItems();
        }

        private void FillInventoryRandomItems()
        {
            if (_trader == null)
                return;

            int countItem = 20;

            for (int i = 0; i < countItem; i++)
            {
                Item item = GetRandomItem();
                _trader.AddToInventory(item);
            }
        }

        private Item GetRandomItem() =>
            _items[_random.Next(_items.Count)];

        private void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine($"Торговец {_trader.Name}: список товаров");
            _trader.ShowInventory();
        }

        private void BuyProduct(int money)
        {
            ShowProducts();
            Console.Write($"Какой товар хотите приобрести? (Золто {money}) " +
                "\nВведите индекс товара: ");

            bool isSelected = int.TryParse(Console.ReadLine(), out int index);

            if (isSelected)
            {
                Item item = _trader.GetItem(index);

                if (item == null)
                    return;

                if (_player.TryBuyItem(item))
                {
                    _trader.SaleItem(item);
                    Console.WriteLine($"Вы приобрели: {item.Name} за {item.Price}");
                }
            }
            else
            {
                Console.WriteLine($"{_trader.Name} вас не понимает...");
            }
        }

        private void ShowInventory(int money)
        {
            double inventoryWeidth = _player.GetInventoryWeight();

            Console.Clear();
            Console.WriteLine($"{_player.Name}: инвентарь (вес {inventoryWeidth}/{_player.CarryWeight}), " +
                $"золота в наличии - {money}");
            _player.ShowInventory();
        }
    }

    public class Person
    {
        protected int Money;
        protected Inventory Inventory = new Inventory();

        public Person(string name, int money)
        {
            Name = name;
            Money = money;
        }

        public string Name { get; }

        public void AddToInventory(Item item) =>
            Inventory.AddItem(item);

        public void ShowInventory() =>
            Inventory.ShowItems();
    }

    public class Player : Person
    {
        public Player(string name, int carryWeight, int money)
            : base(name, money)
        {
            CarryWeight = carryWeight;
        }

        public double CarryWeight { get; }

        public int GetMoney() =>
            Money;

        public double GetInventoryWeight() =>
            Inventory.TotalWeight;

        public void TakeAwayMoney(int money) =>
            Money -= money;

        public bool TryBuyItem(Item item)
        {
            if (CanAddItem(item) && CanPay(item))
            {
                TakeAwayMoney(item.Price);
                Inventory.AddItem(item);
                return true;
            }

            Console.WriteLine("Вы не можете это купить");

            return false;
        }

        public bool CanAddItem(Item item)
        {
            double totalWeigth = Inventory.TotalWeight + item.Weight;

            return totalWeigth <= CarryWeight;
        }

        public bool CanPay(Item item) =>
            Money >= item.Price;
    }

    public class Trader : Person
    {
        public Trader(string name) : base(name, 0) { }

        public void SaleItem(Item item)
        {
            if (item == null)
                return;

            AddMoney(item.Price);
            Inventory.RemoveItem(item);
        }

        public void AddMoney(int money) =>
            Money += money;

        public Item GetItem(int index)
        {
            if (index < 0)
                return null;

            if (index > Inventory.Count)
                return null;

            return Inventory.GetItem(index);
        }
    }

    public class Inventory
    {
        private List<Item> _items = new List<Item>();

        public int Count
        {
            get => _items.Count;
        }

        public double TotalWeight
        {
            get
            {
                double weight = 0;

                foreach (Item item in _items)
                    weight += item.Weight;

                return weight;
            }
        }

        public void AddItem(Item item) =>
            _items.Add(item);

        public void RemoveItem(Item item)
        {
            if (item == null)
                return;

            _items.Remove(item);
        }

        public void ShowItems()
        {
            const int Width = 3;

            for (int i = 0; i < _items.Count; i++)
            {
                Console.Write($"{i,3} | ");
                _items[i].ShowInfo();
            }

            Console.WriteLine();
        }

        public Item GetItem(int index) =>
            _items[index];
    }

    public class Item
    {
        public Item(string name, string category, int price, double weight)
        {
            Name = name;
            Category = category;
            Price = price;
            Weight = weight;
        }

        public string Name { get; }
        public string Category { get; }
        public int Price { get; }
        public double Weight { get; }

        public virtual void ShowInfo()
        {
            const int Width1 = -25;
            const int Width2 = 22;
            const int Width3 = 5;

            Console.WriteLine($"{Name,Width1} | {Category,Width2}  |  цена: {Price,Width3}  |  вес: {Weight,Width3}  |");
        }
    }
}