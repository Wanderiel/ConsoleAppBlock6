using System;

namespace ConsoleAppB6P1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Player player = new Player(
                "Вандериэл", "Следопыт", 30,
                16, 12, 15, 13, 14, 10
            );

            player.ShowInfo(); 

            Console.ReadKey();
        }
    }

    public class Player
    { 
        private string _name;
        private string _rank;
        private int _health;
        private int _strength;
        private int _costitution;
        private int _dextery;
        private int _intellect;
        private int _wishdom;
        private int _charisma;

        public Player(string name, string rank, int health, 
            int strength, int constitution, int dextery, 
            int intellect, int wishdom, int charisma)
        {
            _name = name;
            _rank = rank;
            _health = health;
            _strength = strength;
            _costitution = constitution;
            _dextery= dextery;
            _intellect = intellect;
            _wishdom= wishdom;
            _charisma = charisma;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Имя:\t\t{_name}\nКласс:\t\t{_rank}\nЗдоровье:\t{_health}" +
                $"\nСила:\t\t{_strength}\nТелосложение:\t{_costitution}\nЛовкость:\t{_dextery}" +
                $"\nИнтеллект:\t{_intellect}\nМудрость:\t{_wishdom}\nХаризма:\t{_charisma}");
        }
    }
}