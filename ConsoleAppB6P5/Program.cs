using System;

namespace ConsoleAppB6P5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StaffMember staffMember = new StaffMember();

            staffMember.Work();

            Console.Clear();
            Console.WriteLine("Ввсего доброго!");

            Console.ReadKey();
        }
    }

    public class StaffMember
    {
        private Storage _storage = new Storage();

        public void Work()
        {
            const string CommandAddBook = "1";
            const string CommandRemoveBook = "2";
            const string CommandShowAllBooks = "3";
            const string CommandFindBookByTitle = "4";
            const string CommandFindBookByAuthor = "5";
            const string CommandFindBookByCategory = "6";
            const string CommandFindBookByPublicationYear = "7";
            const string CommandExit = "0";

            bool isWorking = true;

            while (isWorking)
            {
                Console.Clear();
                Console.WriteLine($"Меню:" +
                    $"\n{CommandAddBook}. Добавить книгу" +
                    $"\n{CommandRemoveBook}. Удалить книгу" +
                    $"\n{CommandShowAllBooks}. Показать список книг" +
                    $"\n{CommandFindBookByTitle}. Найти книги по названию" +
                    $"\n{CommandFindBookByAuthor}. Найти книги по автору" +
                    $"\n{CommandFindBookByCategory}. Найти книги по жанру" +
                    $"\n{CommandFindBookByPublicationYear}. Найти книги по году издания" +
                    $"\n{CommandExit}. Выйти из программы");

                switch (Console.ReadLine())
                {
                    case CommandAddBook:
                        AddBook();
                        break;

                    case CommandRemoveBook:
                        RemoveBook();
                        break;

                    case CommandShowAllBooks:
                        ShowAllBooks();
                        break;

                    case CommandFindBookByTitle:
                        FindBooksByTitle();
                        break;

                    case CommandFindBookByAuthor:
                        FindBooksByAuthor();
                        break;

                    case CommandFindBookByCategory:
                        FindBooksByCategory();
                        break;

                    case CommandFindBookByPublicationYear:
                        FindBooksByPublicationYear();
                        break;

                    case CommandExit:
                        isWorking = false;
                        break;
                }
            }
        }

        private void AddBook()
        {
            Console.Clear();
            Console.Write("Введите название книги: ");
            string name = Console.ReadLine();

            if (string.IsNullOrEmpty(name))
                return;

            Console.Write("Введите ФИО/псевдоним автора: ");
            string author = Console.ReadLine();

            if (string.IsNullOrEmpty(author))
                return;

            Console.Write("Укажите жанр книги: ");
            string category = Console.ReadLine();

            if (string.IsNullOrEmpty(category))
                return;

            Console.Write("Укажите год публикации: ");

            if (int.TryParse(Console.ReadLine(), out int year) == false)
                return;

            Book book = new Book(name, author, category, year);
            _storage.AddBook(book);

            Console.WriteLine("Книга успешно добавлена");
            Console.ReadKey();
        }

        private void RemoveBook()
        {
            Console.Clear();
            Console.Write("Введите индекс книги: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                id--;
                _storage.RemoveBook(id);
            }
        }

        private void PrintBooksInfo(List<Book> books)
        {
            if (books.Count == 0)
                Console.WriteLine("Пусто...");
            else
                foreach (Book book in books)
                    book.ShowInfo();

            Console.ReadKey();
        }

        private void ShowAllBooks()
        {
            Console.Clear();
            Console.WriteLine("Список всех книг:");

            List<Book> books = _storage.GetAllBooks();

            PrintBooksInfo(books);
        }

        private void FindBooksByTitle()
        {
            Console.Clear();
            Console.Write("Введите название (или часть) книги: ");

            List<Book> books = _storage.GetBooksByTitle(Console.ReadLine());

            PrintBooksInfo(books);
        }

        private void FindBooksByAuthor()
        {
            Console.Clear();
            Console.Write("Введите автора (или часть) книги: ");

            List<Book> books = _storage.GetBooksByAuthor(Console.ReadLine());

            PrintBooksInfo(books);
        }

        private void FindBooksByCategory()
        {
            Console.Clear();
            Console.Write("Введите жанр (или часть) книги: ");

            List<Book> books = _storage.GetBooksByCategory(Console.ReadLine());

            PrintBooksInfo(books);
        }

        private void FindBooksByPublicationYear()
        {
            Console.Clear();
            Console.Write("Введите год издания: ");

            List<Book> books = _storage.GetBooksByPublicationYear(Console.ReadLine());

            PrintBooksInfo(books);
        }
    }

    public class Storage
    {
        private List<Book> _books = new List<Book>();

        public void AddBook(Book book) => _books.Add(book);

        public void RemoveBook(int index)
        {
            if (TryGetBook(index, out Book book))
            {
                _books.Remove(book);
                Console.WriteLine("Книга успешно удалена");
                Console.ReadKey();
            }
        }

        public List<Book> GetAllBooks() => new List<Book>(_books);

        public List<Book> GetBooksByTitle(string title)
        {
            List<Book> books = new List<Book>();
            title = title.Trim().ToLower();

            if (string.IsNullOrEmpty(title) == false)
                foreach (Book book in _books)
                    if (book.Title.ToLower().Contains(title))
                        books.Add(book);

            return books;
        }

        public List<Book> GetBooksByAuthor(string author)
        {
            List<Book> books = new List<Book>();
            author = author.Trim().ToLower();

            if (string.IsNullOrEmpty(author) == false)
                foreach (Book book in _books)
                    if (book.Autor.ToLower().Contains(author))
                        books.Add(book);

            return books;
        }

        public List<Book> GetBooksByCategory(string category)
        {
            List<Book> books = new List<Book>();
            category = category.Trim().ToLower();

            if (string.IsNullOrEmpty(category) == false)
                foreach (Book book in _books)
                    if (book.Category.ToLower().Contains(category))
                        books.Add(book);

            return books;
        }

        public List<Book> GetBooksByPublicationYear(string input)
        {
            List<Book> books = new List<Book>();

            if (int.TryParse(input, out int year))
                foreach (Book book in _books)
                    if (book.PublicationYear == year)
                        books.Add(book);

            return books;
        }

        private bool TryGetBook(int index, out Book book)
        {
            if (index < 0 || index >= _books.Count)
            {
                book = null;

                return false;
            }

            book = _books[index];

            return true;
        }
    }

    public class Book
    {
        public Book(string title, string author, string category, int publicationYear)
        {
            Title = title;
            Autor = author;
            Category = category;
            PublicationYear = publicationYear;
        }

        public string Title { get; }
        public string Category { get; }
        public string Autor { get; }
        public int PublicationYear { get; }

        public void ShowInfo()
        {
            Console.WriteLine($"{Title}, {Autor}, {Category}, {PublicationYear}");
        }
    }
}