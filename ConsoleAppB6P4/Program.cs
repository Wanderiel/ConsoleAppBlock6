using System;

namespace ConsoleAppB6P4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> points = new Dictionary<string, int>
            {
                { "2", 2 },
                { "3", 3 },
                { "4", 4 },
                { "5", 5 },
                { "6", 6 },
                { "7", 7 },
                { "8", 8 },
                { "9", 9 },
                { "10", 10 },
                { "В", 2 },
                { "Д", 3 },
                { "К", 4 },
                { "Т", 11 }
            };

            Game21 game21 = new Game21(points);

            game21.Play();

            Console.WriteLine("\nИгра окончена");

            Console.ReadKey();
        }
    }

    /// <summary>
    /// Игра 21 (Очко)
    /// </summary>
    public class Game21
    {
        private const string CommandTakeCard = "1";
        private const string CommandEndTurn = "2";
        private const string CommandOutGame = "3";

        private Player[] _players;
        private Statistic _statistic;
        private Croupier _croupier = new Croupier();

        private Dictionary<string, int> _points;

        private int _minPlayersount = 2;
        private int _playersCount;

        public Game21(Dictionary<string, int> points)
        {
            _points = points;
            _playersCount = GetPlayersCount();
        }

        private int GetPlayersCount()
        {
            int maxUsers = 4;

            Console.Write($"Сколько игроков будут пытаться обыграть казино?\n[не больше {maxUsers}]: ");

            if (int.TryParse(Console.ReadLine(), out int playersCount))
            {
                playersCount = (playersCount > maxUsers ? maxUsers : playersCount) + 1;

                SeatPlayers(playersCount);

                return playersCount;
            }

            return 0;
        }

        public void Play()
        {
            if (_playersCount < _minPlayersount)
                return;

            ResetStatistic();

            StartNextGame();
        }

        /// <summary>
        /// Рассадить игроков
        /// </summary>
        private void SeatPlayers(int playersCount)
        {
            _players = new Player[playersCount];

            _players[0] = new Player("Казино");

            for (int i = 1; i < _players.Length; i++)
            {
                _players[i] = new Player($"Игрок{i}");
            }
        }

        private void ResetStatistic()
        {
            _statistic = new Statistic(_players.Length);

            _statistic.SetName(0, _players[0].Name);

            for (int i = 1; i < _players.Length; i++)
            {
                _statistic.SetName(i, _players[i].Name);
            }
        }

        private void ResetHands()
        {
            foreach (Player player in _players)
                player.ResetHand();
        }

        private void StartNextGame()
        {
            while (_statistic.IsPlaying)
            {
                _statistic.AddGamesCount();

                ResetHands();

                _croupier.TakeDeck();

                GivePairAllPlayers();

                CalculatePoints();

                if (ChekGoldenPoint())
                {
                    ShouwHeader(0);
                    Console.WriteLine("Поздравляем! Пара Тузов на руах!");

                    continue;
                }

                ExecuteNextTurn();

                ChekWin();

                _statistic.CheckGameOver();
            }

        }

        private void GivePairAllPlayers()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                if (_statistic.InGame[i])
                {
                    TakeCard(i);
                    TakeCard(i);
                }
            }
        }

        private void TakeCard(int numbrePlayer)
        {
            Card card = _croupier.TakeNextCard();

            _players[numbrePlayer].TakeCard(card);

            int score = GetPoints(_players[numbrePlayer].GetCardsOnHands());

            _statistic.SetPoints(numbrePlayer, score);
        }

        private void ShowCards(int playerNumbers)
        {
            foreach (Card card in _players[playerNumbers].GetCardsOnHands())
                Console.Write($"{card.Name}{card.Suit}({_points[card.Name]}) |");

            Console.WriteLine();
        }

        private void CalculatePoints()
        {
            for (int i = 0; i < _players.Length; i++)
                _statistic.SetPoints(i, GetPoints(_players[i].GetCardsOnHands()));
        }

        private int GetPoints(List<Card> cards)
        {
            int points = 0;

            foreach (Card card in cards)
                if (_points.ContainsKey(card.Name))
                    points += _points[card.Name];

            return points;
        }

        private bool ChekGoldenPoint()
        {
            bool isGoldenPoint = false;

            for (int i = 0; i < _players.Length; i++)
            {
                if (_statistic.InGame[i])
                {
                    List<Card> cards = _players[i].GetCardsOnHands();
                    int maxCardCount = 2;

                    if (cards.Count == 0 && cards.Count > maxCardCount)
                        continue;

                    if (cards[0].Name == "Т" && cards[1].Name == "Т")
                    {
                        _statistic.AddWon(i);

                        isGoldenPoint = true;
                    }
                }
            }

            return isGoldenPoint;
        }

        private void ExecuteNextTurn()
        {
            for (int i = 1; i < _players.Length; i++)
            {
                if (_statistic.InGame[i] == false)
                    continue;

                ExecuteTurnPlayer(i);
            }

            ExecuteTurnCasino();
        }

        private void ExecuteTurnPlayer(int playerNumber)
        {
            bool isTurn = true;

            while (isTurn)
            {
                ShouwHeader(playerNumber);
                ShowMenu();

                switch (Console.ReadLine())
                {
                    case CommandTakeCard:
                        isTurn = TryTakeCard(playerNumber);
                        break;

                    case CommandEndTurn:
                        isTurn = false;
                        break;

                    case CommandOutGame:
                        isTurn = LeaveGame(playerNumber);
                        break;
                }
            }

        }

        private void ShouwHeader(int playerNumber)
        {
            Console.Clear();
            _statistic.Show(playerNumber);

            Console.Write($"\nХодит: {_players[playerNumber].Name}\nКарты на руках: ");

            ShowCards(playerNumber);
        }

        private void ShowMenu()
        {
            Console.WriteLine($"\n{CommandTakeCard} Полчить карту" +
                $"\n{CommandEndTurn} Завершить ход" +
                $"\n{CommandOutGame} Покинуть игру\n");
        }

        private bool TryTakeCard(int playerNumber)
        {
            int maxPoints = 21;

            TakeCard(playerNumber);

            if (_statistic.Points[playerNumber] > maxPoints)
            {
                Console.WriteLine("Перебор!!!");
                Console.ReadKey();

                return false;
            }

            return true;
        }

        private bool LeaveGame(int playerNumber)
        {
            _statistic.SetPoints(playerNumber, 0);
            _statistic.OutGame(playerNumber);

            return false;
        }


        private void ExecuteTurnCasino()
        {
            int maxPoints = 21;
            int winPoints = GetWinPoints();
            bool isTurn = true;

            while (isTurn)
            {
                int points = _statistic.Points[0];
                int difference = maxPoints - points;

                if (points < winPoints && difference > 2)
                    TakeCard(0);
                else
                    isTurn = false;
            }

            ShouwHeader(0);

            Console.WriteLine("Для следующего хода нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void ChekWin()
        {
            int winPionts = GetWinPoints();

            for (int i = 0; i < _players.Length; i++)
            {
                if (winPionts == _statistic.Points[i])
                    _statistic.AddWon(i);
            }
        }

        private int GetWinPoints()
        {
            int maxPoints = 21;
            int winPoints = int.MinValue;

            for (int i = 0; i < _players.Length; i++)
            {
                int points = _statistic.Points[i];

                if (points > winPoints && points <= maxPoints)
                    winPoints = _statistic.Points[i];
            }

            return winPoints;
        }
    }

    public class Statistic
    {
        public Statistic(int playersCount)
        {
            Name = new string[playersCount];
            InGame = new bool[playersCount];
            Winnings = new int[playersCount];
            Points = new int[playersCount];

            AllowToGame(playersCount);
        }

        public bool IsPlaying { get; private set; } = true;
        public int GamesCount { get; private set; } = 0;
        public string[] Name { get; private set; }
        public bool[] InGame { get; private set; }
        public int[] Winnings { get; private set; }
        public int[] Points { get; private set; }

        public void CheckGameOver()
        {
            int inGame = 0;

            for (int i = 0; i < InGame.Length; i++)
            {
                if (InGame[i])
                    inGame++;
            }

            if (inGame <= 1)
                IsPlaying = false;
        }

        public void AddGamesCount() => GamesCount++;

        public void AddWon(int playerNumber) => Winnings[playerNumber]++;

        public void SetPoints(int playerNumber, int points) => Points[playerNumber] = points;

        public void OutGame(int playerNumbers) => InGame[playerNumbers] = false;

        public void SetName(int playerNumbers, string name) => Name[playerNumbers] = name;

        public void Show(int playerNumber)
        {
            const int CountWidth = 3;
            const int Width = 8;

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Игра {GamesCount,CountWidth} |{"Счёт",Width} |" +
                $"{"Побед",Width} |{"Играет",Width} |");
            Console.ResetColor();

            for (int i = 0; i < Name.Length; i++)
            {
                if (i == playerNumber)
                    Console.ForegroundColor = ConsoleColor.Green;

                if (InGame[i] == false)
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine($"{Name[i],Width} |" +
                    $"{Points[i],Width} |" +
                    $"{Winnings[i],Width} |" +
                    $"{InGame[i],Width} |");

                Console.ResetColor();
            }

            Console.WriteLine();
        }

        private void AllowToGame(int playersCount)
        {
            for (int i = 0; i < playersCount; i++)
                InGame[i] = true;
        }
    }

    public class Player
    {
        private List<Card> _hands = new List<Card>();

        public Player(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void TakeCard(Card card)
        {
            if (card == null)
                return;

            _hands.Add(card);
        }

        public List<Card> GetCardsOnHands()
        {
            List<Card> cards = new List<Card>();

            foreach (Card card in _hands)
                cards.Add(card);

            return cards;
        }

        public void ResetHand()
        {
            _hands.Clear();
        }
    }

    public class Croupier
    {
        private Deck _deck;

        public void TakeDeck() => _deck = new Deck();

        public Card TakeNextCard() => _deck.GiveCard();
    }

    public class Card
    {
        public Card(string name, string suit)
        {
            Suit = suit;
            Name = name;
        }

        public string Suit { get; private set; }
        public string Name { get; private set; }
    }

    public class Deck
    {
        private Stack<Card> _cards = new Stack<Card>();

        public Deck()
        {
            Create();
            Shuffle();
        }

        public Card GiveCard() => _cards.Count == 0 ? null : _cards.Pop();

        private void Create()
        {
            string[] nominals = new string[]
                { "2", "3", "4", "5", "6", "7", "8", "9", "10", "В", "Д", "К", "Т" };
            string[] suits = new string[] { "♥", "♦", "♠", "♣" };

            for (int i = 0; i < nominals.Length; i++)
            {
                for (int j = 0; j < suits.Length; j++)
                {
                    Card card = new Card(nominals[i], suits[j]);
                    _cards.Push(card);
                }
            }
        }

        private void Shuffle()
        {
            Random random = new Random();
            int maxRandom = _cards.Count;

            Card[] cards = _cards.ToArray();

            for (int i = 0; i < cards.Length; i++)
            {
                int posinion = random.Next(maxRandom);
                (cards[i], cards[posinion]) = (cards[posinion], cards[i]);
            }

            _cards = ToStack(cards);
        }

        private Stack<Card> ToStack(Card[] cards)
        {
            Stack<Card> stack = new Stack<Card>();

            foreach (Card card in cards)
                stack.Push(card);

            return stack;
        }
    }
}