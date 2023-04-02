using System;
using System.Text.Json;

namespace ConsoleAppB6P6Expanded
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Demiurge demiurge = new Demiurge();

            demiurge.BreatheLife();

            Console.ReadKey();
        }
    }

    public class Demiurge
    {
        private readonly string _worldName = "World";

        private World _world = new World();

        public Demiurge()
        {
            CreateWorld();
        }

        public void BreatheLife() => _world.Live();

        private void CreateWorld()
        {
            CreateItems();
            CreatePersons();
        }

        private void CreateItems()
        {
            List<Item> items = new List<Item>();
            List<Item> loadItems = LoadItems($@"{_worldName}\items.json");
            List<Armor> loadArmors = LoadArmors($@"{_worldName}\armors.json");
            List<Weapon> loadWeapons = LoadWeapons($@"{_worldName}\weapons.json");

            items.AddRange(loadItems);
            items.AddRange(loadArmors);
            items.AddRange(loadWeapons);

            _world.RevealItems(items);
        }

        private List<Item> LoadItems(string path)
        {
            Stream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            List<Item> items = JsonSerializer.Deserialize<List<Item>>(fileStream);
            fileStream.Close();

            return items;
        }

        private List<Armor> LoadArmors(string path)
        {
            Stream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            List<Armor> armors = JsonSerializer.Deserialize<List<Armor>>(fileStream);
            fileStream.Close();

            return armors;
        }

        private List<Weapon> LoadWeapons(string path)
        {
            Stream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            List<Weapon> weapons = JsonSerializer.Deserialize<List<Weapon>>(fileStream);
            fileStream.Close();

            return weapons;
        }

        private void CreatePersons()
        {
            List<Person> persons = new List<Person>()
            {
                new Person("Дровакин", 16, 0, false),
                new Person("Эль'Ри'сад", 14, 0, true),
            };

            _world.RevealPersons(persons);
        }

        //private Inventory GetInventoryWithRandomItems()
        //{
        //    Random random = new Random();
        //    List<Item> items = _world.GetItems();
        //    List<Item> randomItems = new List<Item>();
        //    int countItem = 20;

        //    for (int i = 0; i < countItem; i++)
        //    {
        //        randomItems.Add(items[random.Next(items.Count)]);
        //    }

        //    return new Inventory(randomItems);
        //}
    }

    public class World
    {
        private Random _random = new Random();
        private List<Item> _items;
        private List<Person> _persons;

        public void RevealItems(List<Item> items) =>
            _items = items;

        public void RevealPersons(List<Person> persons) =>
            _persons = persons;

        public List<Item> GetItems() =>
            new List<Item>(_items);

        public void Live()
        {
            GiveRiches();
        }

        private void GiveRiches()
        {
            int min = 300;
            int max = 1001;
            int countItem = 30;

            for (int i = 1; i < _persons.Count; i++)
            {
                _persons[i].AddMoney(_random.Next(min, max));
                FillInventoryRandomItems(countItem, _persons[i]);
            }
        }

        private void FillInventoryRandomItems(int countItem, Person person)
        {
            for (int i = 0; i < countItem; i++)
            {
                Item item = GetRandomItem();
                person.AddToInventory(item);
            }
        }

        private Item GetRandomItem() =>
            _items[_random.Next(_items.Count)];
    }

    public class Person
    {
        private int _strength;
        private int _money;
        private Inventory _inventory = new Inventory();
        private bool _hasStorage;

        public Person(string name, int strength, int money, bool hasStorage)
        {
            Name = name;
            _strength = strength;
            _money = money;
            _hasStorage = hasStorage;
        }

        public string Name { get; }
        public double CarryWeight
        {
            get
            {
                int multiplier = 10;
                return _strength * multiplier;
            }
        }

        public void AddMoney(int money) =>
            _money += money;

        public void ShowInventory() =>
            _inventory.ShowItems();

        public void AddToInventory(Item item)
        {
            if (CanAddItem(item))
                _inventory.AddItem(item);
        }

        private bool CanAddItem(Item item)
        {
            if (_hasStorage)
                return true;

            double weigth = _inventory.TotalWeight + item.Weight;

            if (weigth > CarryWeight)
                return false;

            return true;
        }

        private bool CanPay(Item item)
        {
            if (_money < item.Price)
                return false;

            return true;
        }
    }

    public class Inventory
    {
        private List<Item> _items = new List<Item>();

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

    public class Armor : Item
    {
        public Armor(string name, string category, int defence, int price, double weight) :
            base(name, category, price, weight)
        {
            Defence = defence;
        }

        public int Defence { get; }

        public override void ShowInfo()
        {
            const int Width1 = -25;
            const int Width2 = 22;
            const int Width3 = 5;
            const int Width4 = 3;

            Console.WriteLine($"{Name,Width1} | {Category,Width2}  |  цена: {Price,Width3}  |  вес: {Weight,Width3}  |" +
                $"  Защита: {Defence,Width4}  |");
        }
    }

    public class Weapon : Item
    {
        public Weapon(string name, string category, int damage, int price, double weight) :
            base(name, category, price, weight)
        {
            Damage = damage;
        }

        public int Damage { get; }

        public override void ShowInfo()
        {
            const int Width1 = -25;
            const int Width2 = 22;
            const int Width3 = 5;

            Console.WriteLine($"{Name,Width1} | {Category,Width2}  |  цена: {Price,Width3}  |  вес: {Weight,Width3}  |" +
                $"  Урон: {Damage,Width3}  |");
        }
    }
}