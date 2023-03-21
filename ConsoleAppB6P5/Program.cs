using System;

namespace ConsoleAppB6P5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Category category = new Category(0);

            Rack rack = new Rack(1, "Сказки");

            rack.AddBook();
            rack.ShowPlaces();

            Console.ReadKey();
        }
    }

    public class Storage
    {


    }

    public class Rack
    {
        private static int _tiersCount = 6;
        private static int _tiersCapacity = 15;
        private Book[,] _books = new Book[_tiersCount, _tiersCapacity];

        public Rack(int id, string category)
        {
            Id = id;
            Category = category;
        }

        public int Id { get; }
        public string Category { get; }

        public void AddBook()
        {
            _books[3, 5] = new Book();
            _books[1, 2] = new Book();
            _books[2, 7] = new Book();
            _books[0, 12] = new Book();
        }

        public void ShowPlaces()
        {
            const int Width = 5;
            string line = new string('▬', _tiersCapacity * (Width + 1) + 1);

            Console.WriteLine(line);

            for (int i = 0; i < _tiersCount; i++)
            {
                Console.Write("|");

                for (int j = 0; j < _tiersCapacity; j++)
                {
                    Console.BackgroundColor = _books[i, j] == null ?
                        ConsoleColor.DarkGreen : ConsoleColor.DarkRed;

                    string place = $"{i + 1}{j + 1} ";
                    Console.Write($"{place,Width}");
                    Console.ResetColor();
                    Console.Write("|");
                }

                Console.WriteLine($"\n{line}");
            }
        }
    }

    public class Book
    {
        public Book() { }

        public Book(int id, string title, string autor, int publicationYear, string description)
        {
            Id = id;
            Title = title;
            Autor = autor;
            PublicationYear = publicationYear;
            Description = description;
        }

        public int Id { get; }
        public string Title { get; }
        public string Autor { get; }
        public int PublicationYear { get; }
        public string Description { get; }

        public void ShowInfo()
        {
            Console.WriteLine($"{Id}, {Title}, {Autor}, {PublicationYear}" +
                $"\n{Description}");
        }
    }

    public class Category
    {
        private int _id;
        private string _name;

        public Category(int maxId)
        {
            Create(maxId);
        }

        public void Create(int maxId)
        {
            Console.Write("\nВведите название категории: ");
            string name = Console.ReadLine();

            _id = maxId + 1;
            _name = name == string.Empty ? $"Категория {_id}" : name;
        }
    }
}