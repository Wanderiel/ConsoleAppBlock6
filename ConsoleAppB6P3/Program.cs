using System;
using System.Text.Json;

namespace ConsoleAppB6P3
{
    internal class Program
    {
        static void Main()
        {
            Work work = new Work();

            work.Start();

            Console.ReadKey();
        }
    }

    public class Work
    {
        private Controller _controller = new Controller();
        private Printer _printer = new Printer();
        private Database _database;

        public void Start()
        {
            const string LoadDatabaseCommand = "1";
            const string NewDatabaseCommand = "2";
            const string ShowPlayersCommand = "3";
            const string NewPlayerCommand = "4";
            const string RemovePlayerCommand = "5";
            const string BanPlayerCommand = "6";
            const string UnbanPlayerCommand = "7";
            const string SaveDatabaseCommand = "8";
            const string ExitCommand = "9";

            bool isWorkig = true;

            _printer.PrintMessage("Добро подаловать в приложение по управлению базой данных игроков.",
                _printer.WorkingColor);
            Console.ReadKey();

            while (isWorkig)
            {
                Console.Clear();

                _printer.PrintMessage(
                    $"{LoadDatabaseCommand}. Загрузить базу данных" +
                    $"\n{NewDatabaseCommand}. Создать новую базу данных" +
                    $"\n{ShowPlayersCommand}. Список игроков" +
                    $"\n{NewPlayerCommand}. Создать нового игрока" +
                    $"\n{RemovePlayerCommand}. Удалить игрока" +
                    $"\n{BanPlayerCommand}. Заблокировать игрока" +
                    $"\n{UnbanPlayerCommand}. Разблокировать игрока" +
                    $"\n{SaveDatabaseCommand}. Сохранить базу данных" +
                    $"\n{ExitCommand}. Выход из программы");

                _printer.PrintMessage("\nЧто вы хотите сделать:", _printer.WorkingColor);

                switch (Console.ReadLine())
                {
                    case LoadDatabaseCommand:
                        _database = _controller.LoadDatabase();
                        break;

                    case NewDatabaseCommand:
                        _database = new Database();
                        break;

                    case ShowPlayersCommand:
                        TryShowPlayers();
                        break;

                    case NewPlayerCommand:
                        TryCreatePlayer();
                        break;

                    case RemovePlayerCommand:
                        TryRemovePlayer();
                        break;

                    case BanPlayerCommand:
                        TryBanPlayer();
                        break;

                    case UnbanPlayerCommand:
                        TryUnbanPlayer();
                        break;

                    case SaveDatabaseCommand:
                        _controller.SaveDatabase(_database.GetPlayers());
                        break;

                    case ExitCommand:
                        isWorkig = false;
                        break;
                }
            }

            _printer.PrintMessage("\nВсего доброго", _printer.WorkingColor);
        }

        private bool IsDatabase()
        {
            if (_database == null)
            {
                _printer.PrintMessage("База данных не загружена или не существует", _printer.AlertColor);
                Console.ReadLine();
                return false;
            }

            return true;
        }

        private void TryShowPlayers()
        {
            if (IsDatabase())
                _database.PrintPayers();
        }

        private void TryCreatePlayer()
        {
            if (IsDatabase())
                _database.CreatePlayer();
        }

        private void TryRemovePlayer()
        {
            if (IsDatabase())
                _database.RemovePlayer();
        }

        private void TryBanPlayer()
        {
            if (IsDatabase())
                _database.BanPlayer();
        }

        private void TryUnbanPlayer()
        {
            if (IsDatabase())
                _database.UnbanPlayer();
        }
    }

    public class Controller
    {
        private string _path = "Database.json";
        private Printer _printer = new Printer();

        public void SaveDatabase(List<Player> players)
        {
            Stream stream = new FileStream(_path, FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(stream, players);
            stream.Close();

            _printer.PrintMessage("База данных сохраннена", _printer.WarningColor);
            Console.ReadKey();
        }

        public Database LoadDatabase()
        {
            Database database = new Database();

            if (File.Exists(_path) == false)
            {
                _printer.PrintMessage("База данных не существует", _printer.AlertColor);
                Console.ReadKey();
            }
            else
            {
                Stream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read);
                List<Player> players = JsonSerializer.Deserialize<List<Player>>(fileStream);
                fileStream.Close();

                database.Attach(players);
                _printer.PrintMessage("Бада данных успешно загружена", _printer.WarningColor);
                Console.ReadKey();
            }

            return database;
        }
    }

    public class Player
    {
        public Player(int id, string name, string rank, int level, bool isBanned = false)
        {
            Id = id;
            Name = name;
            Rank = rank;
            Level = level;
            IsBanned = isBanned;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Rank { get; private set; }
        public int Level { get; private set; }
        public bool IsBanned { get; private set; }

        public void Ban() => IsBanned = true;

        public void Unban() => IsBanned = false;
    }

    public class Printer
    {
        private const ConsoleColor DefaultColor = ConsoleColor.White;

        public ConsoleColor WorkingColor { get; } = ConsoleColor.DarkGray;
        public ConsoleColor AlertColor { get; } = ConsoleColor.Red;
        public ConsoleColor WarningColor { get; } = ConsoleColor.White;

        public void PrintMessage(string message, ConsoleColor color = DefaultColor)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public class Database
    {
        private List<Player> _players = new List<Player>();
        private Printer _printer = new Printer();

        public void CreatePlayer()
        {
            int id = _players.Count == 0 ? 0 : _players.Last().Id + 1;

            _printer.PrintMessage("Введите имя игрока: ", _printer.WorkingColor);
            string name = Console.ReadLine();

            _printer.PrintMessage("Введите класс игрока: ", _printer.WorkingColor);
            string rank = Console.ReadLine();

            int level = 1;

            _players.Add(new Player(id, name, rank, level));

            _printer.PrintMessage("Игрок успешно создан", _printer.WarningColor);
            Console.ReadKey();
        }

        public void BanPlayer()
        {
            Console.Clear();
            _printer.PrintMessage("Инициализация процедуры блокировки игрока.", _printer.WorkingColor);

            if (TryGetPlayer(out Player player))
            {
                player.Ban();
                _printer.PrintMessage("Игрок успешно заблокирован", _printer.WarningColor);
                Console.ReadKey();
            }
        }

        public void UnbanPlayer()
        {
            Console.Clear();
            _printer.PrintMessage("Инициализация процедуры разблокировки игрока.", _printer.WorkingColor);

            if (TryGetPlayer(out Player player))
            {
                player.Unban();
                _printer.PrintMessage("Игрок успешно разблокирован", _printer.WarningColor);
                Console.ReadKey();
            }
        }

        public void RemovePlayer()
        {
            Console.Clear();
            _printer.PrintMessage("Инициализация процедуры удаления игрока.", _printer.WorkingColor);

            if (TryGetPlayer(out Player player))
            {
                _players.Remove(player);
                _printer.PrintMessage("Игрок успешно удалён", _printer.WarningColor);
                Console.ReadKey();
            }
        }

        public void PrintPayers()
        {
            const int Width1 = 3;
            const int Width2 = 12;
            const int Width3 = 5;

            Console.Clear();
            _printer.PrintMessage($"{"Id",Width1}|{"Имя",Width2}|" +
                    $"{"Класс",Width2}|{"Лвл",Width1}| " +
                    $"{"Блок",Width3}|");

            foreach (Player player in _players)
                _printer.PrintMessage($"{player.Id,Width1}|{player.Name,Width2}|" +
                    $"{player.Rank,Width2}|{player.Level,Width1}| " +
                    $"{player.IsBanned,Width3}|", _printer.WorkingColor);

            Console.ReadKey();
        }

        public List<Player> GetPlayers()
        {
            List<Player> listPlayers = new List<Player>();

            for (int i = 0; i < _players.Count; i++)
            {
                listPlayers.Add(_players[i]);
            }

            return listPlayers;
        }

        public void Attach(List<Player> players) => _players = players;

        private bool TryGetPlayer(out Player player)
        {
            if (_players.Count == 0)
            {
                _printer.PrintMessage("В базе нет игроков", _printer.AlertColor);
                Console.ReadKey();

                player = null;
                return false;
            }

            _printer.PrintMessage("Введите id игрока: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                foreach (Player nextPlayer in _players)
                {
                    if (nextPlayer.Id == id)
                    {
                        player = nextPlayer;
                        return true;
                    }
                }
            }

            _printer.PrintMessage("Игрок с таким id не найден", _printer.AlertColor);
            Console.ReadKey();

            player = null;
            return false;
        }
    }
}
