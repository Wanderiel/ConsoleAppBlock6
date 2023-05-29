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

        public void ShowInfo() => Console.WriteLine($"{Name} | {Gender}");
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
            foreach (var animal in _animals)
                animal.ShowInfo();

            _animals[0].MakeSound();
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

        }
    }

    public class AnimalFactory
    {
        private readonly Random _random;
        private readonly string[] _genders;

        public AnimalFactory()
        {
            _random = new Random();
            _genders = new string[] { "м.", "ж." };
        }

        public Animal CreateElephant()
        {
            int number = _random.Next(_genders.Length);
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
            int number = _random.Next(_genders.Length);
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
            int number = _random.Next(_genders.Length);
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