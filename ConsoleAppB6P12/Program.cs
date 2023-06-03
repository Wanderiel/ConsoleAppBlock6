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
                }
                else if (int.TryParse(userInput, out int index))
                {
                    index--;

                    if (index >= 0 && index < _aviaries.Count)
                        _aviaries[index].ShowInfo();
                }
            }
        }
    }

    public class AnimalFactory
    {
        private readonly Random _random = new Random();

        public Animal CreateElephant()
        {
            Animal[] elephants =
            {
                new Animal
                (
                    "Слон",
                    "муж.",
                    "Слон поднимает свой хобот и трубит приветствие."
                ),
                new Animal
                (
                    "Слониха",
                    "жен.",
                    "Слониха стоит и хлопает своими большими ушами."
                ),
            };

            return elephants[_random.Next(elephants.Length)];
        }

        public Animal CreateMonkey()
        {
            string[] genders = { "муж.", "жен." };
            string gender = genders[_random.Next(genders.Length)];

            return new Animal
            (
                "Обезьяна",
                gender,
                "Обезьяна начинает метаться по клетке и кричать - \"Угу-уга-угу.\""
            );
        }

        public Animal CreateLion()
        {
            Animal[] lions =
{
                new Animal
                (
                    "Лев",
                    "муж.",
                    "Лев гордо издаёт свой рык, показывая всем, кто здесь хозяин."
                ),
                new Animal
                (
                    "Львица",
                    "жен.",
                    "Львица рыкнув на вас, лениво уходит в тень."
                ),
            };

            return lions[_random.Next(lions.Length)];
        }

        public Animal CreateWolf()
        {
            Animal[] wolfs =
{
                new Animal
                (
                    "Волк",
                    "муж.",
                    "Волк смотрит вам в глаза, а после издаёт протяжный вой."
                ),
                new Animal
                (
                    "Волчица",
                    "жен.",
                    "Волчица скалится на вас."
                ),
            };

            return wolfs[_random.Next(wolfs.Length)];
        }
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