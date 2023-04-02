using System;
using System.Text.Json;
using System.Xml.Linq;

namespace ConsoleAppB6P6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Shop shop = new Shop();

            shop.Work();

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
            CreateItems();
            CreatePersons();
        }

        public void Work()
        {
            bool isOpen = true;

            while (isOpen)
            {

            }
        }

        private void CreateItems()
        {
            _items = LoadItems("items.json");
        }

        private List<Item> LoadItems(string path)
        {
            Stream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            List<Item> items = JsonSerializer.Deserialize<List<Item>>(fileStream);
            fileStream.Close();

            return items;
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

            int countItem = 30;

            for (int i = 0; i < countItem; i++)
            {
                Item item = GetRandomItem();
                _trader.AddToInventory(item);
            }
        }

        private Item GetRandomItem() =>
            _items[_random.Next(_items.Count)];
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

        public void TakeAwayMoney(int money) =>
            Money -= money;

        private bool CanAddItem(Item item)
        {
            double totalWeigth = Inventory.TotalWeight + item.Weight;

            return totalWeigth <= CarryWeight;
        }

        private bool CanPay(Item item) =>
            Money < item.Price;

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
            foreach (Item item in _items)
                item.ShowInfo();

            Console.WriteLine();
        }

        public Item GetItem(int index) =>
            _items[index];

        public List<Item> GetItems() =>
            new List<Item>(_items);
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