using System;
using System.Text.Json;
using System.Threading.Channels;

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
        private Player _player;
        private Trader _trader;

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
                Console.Clear();
                Console.WriteLine($"Лавка \"Ломаная подкова\"" +
                    $"\n{CommandShowProducts}. Посмотреть товары" +
                    $"\n{CommandBuyProduct}. Купить товар" +
                    $"\n{CommandShowInventory}. Посмотреть инвентарь" +
                    $"\n{CommandExit}. Покинуть лавку");
                Console.WriteLine();

                _player.ShowPurse();

                switch (Console.ReadLine())
                {
                    case CommandShowProducts:
                        ShowProducts();
                        break;

                    case CommandBuyProduct:
                        BuyProduct();
                        break;

                    case CommandShowInventory:
                        ShowInventory();
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

            using Stream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                _items = JsonSerializer.Deserialize<List<Item>>(fileStream);
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

        private void BuyProduct()
        {
            ShowProducts();
            Console.WriteLine($"Какой товар хотите приобрести?");
            _player.ShowPurse();
            Console.Write("Введите индекс товара: ");

            if (int.TryParse(Console.ReadLine(), out int index) == false)
            {
                Console.WriteLine($"{_trader.Name} вас не понимает...");
            }
            else
            {
                if (_trader.TryGetItem(index, out Item item) == false)
                    return;

                if (_player.TryBuy(item))
                {
                    _trader.Sell(item);
                    Console.WriteLine($"Вы приобрели: {item.Name} за {item.Price}");
                }
            }
        }

        private void ShowInventory()
        {
            Console.Clear();
            _player.ShowInventory();
            _player.ShowPurse();
        }
    }

    public class Person
    {
        protected readonly Inventory Inventory = new Inventory();

        public Person(string name, int money)
        {
            Name = name;
            Money = money;
        }

        public string Name { get; }
        public int Money { get; protected set; }

        public void AddToInventory(Item item) =>
            Inventory.Add(item);

        public virtual void ShowInventory() =>
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

        public void ShowPurse() =>
            Console.WriteLine($"Золота в наличии - {Money}");

        public double InventoryWeight => 
            Inventory.TotalWeight;

        public bool TryBuy(Item item)
        {
            if (CanAdd(item) == false || CanPay(item) == false)
            {
                Console.WriteLine("Вы не можете это купить");

                return false;
            }

            TakeAwayMoney(item.Price);
            Inventory.Add(item);

            return true;
        }

        public override void ShowInventory()
        {
            Console.WriteLine($"{Name}: инвентарь (вес {InventoryWeight}/{CarryWeight})");
            base.ShowInventory();
        }

        private void TakeAwayMoney(int money) =>
            Money -= money;

        private bool CanAdd(Item item)
        {
            double totalWeigth = Inventory.TotalWeight + item.Weight;

            return totalWeigth <= CarryWeight;
        }

        private bool CanPay(Item item) =>
            Money >= item.Price;
    }

    public class Trader : Person
    {
        public Trader(string name) : base(name, 0) { }

        public void Sell(Item item)
        {
            AddMoney(item.Price);
            Inventory.Remove(item);
        }

        public bool TryGetItem(int index, out Item item)
        {
            item = Inventory.GetItem(index);

            if (item == null)
                return false;

            return true;
        }
 
        private void AddMoney(int money) =>
            Money += money;
   }

    public class Inventory
    {
        private List<Item> _items = new List<Item>();

        public int Count { get => _items.Count; }

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

        public void Add(Item item) =>
            _items.Add(item);

        public void Remove(Item item)
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
                Console.Write($"{i,Width} | ");
                _items[i].ShowInfo();
            }

            Console.WriteLine();
        }

        public Item GetItem(int index)
        {
            if (index < 0)
                return null;

            if (index >= Count)
                return null;

            return _items[index];
        }
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