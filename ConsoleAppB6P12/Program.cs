//GlobalUsing
//Net v.6

namespace ConsoleAppB6P12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ZooFactory zooFactory = new ZooFactory();

            Zoo zoo = zooFactory.Create();

            zoo.Work();

            Console.WriteLine("\nВсего доброго.");
            Console.ReadKey();
        }
    }

    public class Animal
    {
        private readonly string _voice;

        public Animal(string name, string gender, string voice)
        {
            Name = name;
            Gender = gender;
            _voice = voice;
        }

        public string Name { get; }
        public string Gender { get; }

        public void MakeSound() => Console.WriteLine(_voice);

        public void ShowInfo()
        {
            const int Width = -15;

            Console.WriteLine($"{Name,Width} | {Gender}");
        }
    }

    public class Aviary
    {
        private readonly int _size;
        private readonly List<Animal> _animals;

        public Aviary(string name, int size)
        {
            Name = name;
            _size = size;
            _animals = new List<Animal>();
        }

        public string Name { get; }

        public void Add(Animal animal)
        {
            if (_animals.Count >= _size)
                return;

            _animals.Add(animal);
        }

        public void ShowInfo()
        {
            if (_animals.Count == 0)
                return;

            Console.Clear();

            foreach (Animal animal in _animals)
                animal.ShowInfo();

            Console.WriteLine();
            _animals.First().MakeSound();

            Console.ReadKey();
        }
    }

    public class Zoo
    {
        private readonly List<Aviary> _aviaries;

        public Zoo(List<Aviary> aviaries)
        {
            _aviaries = new List<Aviary>(aviaries);
        }

        public void Work()
        {
            bool isWalk = true;
            string commandExit = "Q";

            while (isWalk)
            {
                Console.Clear();

                for (int i = 1; i <= _aviaries.Count; i++)
                    Console.WriteLine($"{i}. Идти к вальеру \"{_aviaries[i - 1].Name}\"");

                Console.WriteLine($"{commandExit}. Покинуть зоопарк");
                Console.Write("\nКуда пойдём? ");

                string userInput = Console.ReadLine();

                if (userInput.ToUpper() == commandExit)
                {
                    isWalk = false;
                    continue;
                }

                if (int.TryParse(userInput, out int index) == false)
                    continue;

                index--;

                if (index < 0 || index >= _aviaries.Count)
                    continue;

                _aviaries[index].ShowInfo();
            }
        }
    }

    public class AnimalFactory
    {
        private readonly Random _random;
        private readonly string[] _genders;

        public AnimalFactory()
        {
            _random = new Random();
            _genders = new string[] { "муж.", "жен." };
        }

        public Animal CreateElephant()
        {
            int number = GetNumber();
            string[] names =
                {
                    "Слон",
                    "Слониха"
                };
            string[] voices =
                {
                    "Слон поднимает свой хобот и трубит приветствие.",
                    "Слониха стоит и хлопает своими большими ушами."
                };

            return new Animal(names[number], _genders[number], voices[number]);
        }

        public Animal CreateMonkey()
        {
            string name = "Обезьяна";
            string gender = _genders[_random.Next(_genders.Length)];
            string voice = "Обезьяна начинает метаться по клетке и кричать - \"Угу-уга-угу.\"";

            return new Animal(name, gender, voice);
        }

        public Animal CreateLion()
        {
            int number = GetNumber();
            string[] names =
                {
                    "Лев",
                    "Львица"
                };
            string[] voices =
                {
                    "Лев гордо издаёт свой рык, показывая всем, кто здесь хозяин.",
                    "Львица рыкнув на вас, лениво уходит в тень."
                };

            return new Animal(names[number], _genders[number], voices[number]);
        }

        public Animal CreateWolf()
        {
            int number = GetNumber();
            string[] names =
                {
                    "Волк",
                    "Волчица"
                };
            string[] voices =
                {
                    "Волк смотрит вам в глаза, а после издаёт протяжный вой.",
                    "Волчица скалится на вас"
                };

            return new Animal(names[number], _genders[number], voices[number]);
        }

        private int GetNumber() => _random.Next(_genders.Length);
    }

    public class AviaryFactory
    {
        private readonly AnimalFactory _animalFactory;

        public AviaryFactory()
        {
            _animalFactory = new AnimalFactory();
        }

        public Aviary CreateElephants(int size)
        {
            Aviary aviary = new Aviary("Слоны", size);

            for (int i = 0; i < size; i++)
                aviary.Add(_animalFactory.CreateElephant());

            return aviary;
        }

        public Aviary CreateMonkeys(int size)
        {
            Aviary aviary = new Aviary("Обезьяны", size);

            for (int i = 0; i < size; i++)
                aviary.Add(_animalFactory.CreateMonkey());

            return aviary;
        }

        public Aviary CreateLoins(int size)
        {
            Aviary aviary = new Aviary("Львы", size);

            for (int i = 0; i < size; i++)
                aviary.Add(_animalFactory.CreateLion());

            return aviary;
        }

        public Aviary CreateWolfs(int size)
        {
            Aviary aviary = new Aviary("Волки", size);

            for (int i = 0; i < size; i++)
                aviary.Add(_animalFactory.CreateWolf());

            return aviary;
        }
    }

    public class ZooFactory
    {
        private readonly AviaryFactory _aviaryFactory;

        public ZooFactory()
        {
            _aviaryFactory = new AviaryFactory();
        }

        public Zoo Create()
        {
            List<Aviary> aviaries = new List<Aviary>();
            int countAnimal = 8;

            aviaries.Add(_aviaryFactory.CreateElephants(countAnimal));
            aviaries.Add(_aviaryFactory.CreateMonkeys(countAnimal));
            aviaries.Add(_aviaryFactory.CreateLoins(countAnimal));
            aviaries.Add(_aviaryFactory.CreateWolfs(countAnimal));

            return new Zoo(aviaries);
        }
    }
}