//Net v.6

namespace ConsoleAppB6P9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Market market = new Market
            (
                new List<Product>()
                {
                    new Product("Хлеб ржаной", 40),
                    new Product("Хлеб пшеничный", 35),
                    new Product("Булочка с повидлом", 30),
                    new Product("Молоко", 50),
                    new Product("Творог", 72),
                    new Product("Сметана", 84),
                    new Product("Кефир", 45),
                    new Product("Масло сливочное", 158),
                    new Product("Колбаса охотничья", 259),
                    new Product("Колбаса докторская", 189),
                }
            );

            int countBuyers = 15;

            market.Open(countBuyers);

            Console.WriteLine("Магазин закрыт, приходите завтра ☻");

            Console.ReadKey();
        }
    }

    public static class Randomizer
    {
        private static Random _random = new Random();

        public static int GetRandomNumber(int max) =>
            _random.Next(max);

        public static bool GetRandomBool()
        {
            int max = 4;

            return _random.Next(max) == 0;
        }
    }

    public class Market
    {
        private Queue<Buyer> _buyers;
        private Cash _cash;
        private bool _isWork;
        private List<Product> _products;

        public Market(List<Product> products)
        {
            _buyers = new Queue<Buyer>();
            _cash = new Cash();
            _isWork = false;
            _products = products;
        }

        public void Open(int countBuyers)
        {
            if (_isWork)
                return;

            _isWork = true;

            for (int i = 0; i < countBuyers; i++)
                AddBuyer();

            Work();
        }

        private void AddBuyer()
        {
            int maxMoney = 500;
            int maxCountProducts = 10;
            int money = Randomizer.GetRandomNumber(maxMoney) + 1;
            int countProducts = Randomizer.GetRandomNumber(maxCountProducts) + 1;

            List<Product> products = new List<Product>(_products);
            Buyer buyer = new Buyer(money, countProducts);

            buyer.FillBasket(products);

            _buyers.Enqueue(buyer);
        }

        private void Work()
        {
            while (_buyers.Count > 0)
            {
                Console.WriteLine($"В очереди клиентов: {_buyers.Count}");
                Buyer buyer = _buyers.Dequeue();
                _cash.Serve(buyer);

                TryAddBuyer();
            }
        }

        private void TryAddBuyer()
        {
            if (Randomizer.GetRandomBool())
            {
                AddBuyer();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("В конец очереди встал ещё один клиент");
                Console.ResetColor();
            }
        }
    }

    public class Cash
    {
        private int _revenue = 0;
        private int _count = 0;

        public void Serve(Buyer buyer)
        {
            bool isBusy = true;
            int receiptAmount = 0;

            _count++;

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"\nОбслуживается клиент #{_count}");

            Thread.Sleep(1000);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Список покупок:");
            buyer.ShowBasket();
            Console.ResetColor();

            while (isBusy)
            {
                receiptAmount = GetReceiptAmount(buyer.Products);

                if (buyer.TryPay(receiptAmount))
                    isBusy = false;

                if (buyer.Products.Count == 0)
                    isBusy = false;
            }

            if (buyer.Products.Count > 0)
            {
                _revenue += receiptAmount;

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Товарный чек:");
                buyer.ShowBasket();
                Console.WriteLine($"Сумма чека: {receiptAmount}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Клиент ничего не купил");
                Console.ResetColor();
            }

            Console.WriteLine("Обслуживание завершено\n");

            Thread.Sleep(1500);

            ShowRevenue();
            Console.WriteLine();
        }

        public void ShowRevenue() =>
            Console.WriteLine($"Выручка: {_revenue}");

        private int GetReceiptAmount(List<Product> products)
        {
            int amount = 0;

            foreach (Product product in products)
                amount += product.Price;

            return amount;
        }
    }

    public class Buyer
    {
        private int _money;
        private int _countProduct;
        private Basket _basket;

        public Buyer(int money, int countProduct)
        {
            _money = money;
            _countProduct = countProduct;
            _basket = new Basket();
        }

        public List<Product> Products => _basket.Products;

        public void FillBasket(List<Product> products)
        {
            for (int i = 0; i < _countProduct; i++)
            {
                int number = Randomizer.GetRandomNumber(products.Count);
                AddToBasket(products[number]);
            }
        }

        public bool TryPay(int receiptAmount)
        {
            if (CanPay(receiptAmount) == false)
            {
                RemoveFromBasket();

                return false;
            }

            Pay(receiptAmount);

            return true;
        }

        public void ShowBasket() =>
            _basket.Show();

        private bool CanPay(int receiptAmount) =>
            _money >= receiptAmount;

        private void Pay(int receiptAmount) =>
            _money -= receiptAmount;

        private void AddToBasket(Product product) =>
            _basket.Add(product);

        private void RemoveFromBasket() => 
            _basket.RemoveProduct();
    }

    public class Basket
    {
        private List<Product> _products = new List<Product>();

        public List<Product> Products =>
            new List<Product>(_products);

        public void Add(Product product) =>
            _products.Add(product);

        public void RemoveProduct()
        {
            int number = Randomizer.GetRandomNumber(_products.Count);
            Product product = _products[number];

            _products.Remove(product);

            Console.WriteLine($"Удалено: {product.Name}");

            Thread.Sleep(1000);
        }

        public void Show()
        {
            foreach (Product product in _products)
                Console.WriteLine(product.Name);
        }
    }

    public class Product
    {
        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public int Price { get; }
    }
}