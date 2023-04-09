using System;
//Net v.6

namespace ConsoleAppB6P7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Depot depot = new Depot();

            depot.Work();

            Console.Clear();
            Console.WriteLine("Всего доброго!");

            Console.ReadKey();
        }
    }

    public class Depot
    {
        private Random _random = new Random();
        private string _direction = string.Empty;
        private Train _currentTrain;
        private List<Train> _trains = new List<Train>();
        private TypeWagon[] _typeWagons = {
            new TypeWagon("К", 36),
            new TypeWagon("П", 54),
        };

        public void Work()
        {
            const string CommandCreateDirection = "1";
            const string CommandSellTickets = "2";
            const string CommandFormTrain = "3";
            const string CommandSendTrain = "4";
            const string CommandShowTrains = "5";
            const string CommandExit = "0";

            bool isOpen = true;

            _currentTrain = new Train(_typeWagons);

            while (isOpen)
            {
                Console.Clear();
                Console.WriteLine(
                    "Депо \"неСтучат колёса\":" +
                    "\n" +
                    $"\n{CommandCreateDirection}. Задать направление" +
                    $"\n{CommandSellTickets}. Продать билеты по направлению" +
                    $"\n{CommandFormTrain}. Сформировать поезд на направление" +
                    $"\n{CommandSendTrain}. Отправть поезд по направлению" +
                    $"\n{CommandShowTrains}. Посмотреть информацию по поездам" +
                    $"\n{CommandExit}. Выход из программы");

                ShowBottom();

                switch (Console.ReadLine())
                {
                    case CommandCreateDirection:
                        CreateDirection();
                        break;

                    case CommandSellTickets:
                        SellTickets();
                        break;

                    case CommandFormTrain:
                        FormTrain();
                        break;

                    case CommandSendTrain:
                        SendTrain();
                        break;

                    case CommandShowTrains:
                        ShowInfo();
                        break;

                    case CommandExit:
                        isOpen = false;
                        break;
                }

                Console.ReadKey();
            }
        }

        private void CreateDirection()
        {
            if (string.IsNullOrEmpty(_direction) == false)
            {
                Console.WriteLine("Направление уже задано...");
                return;
            }

            Console.Write("Введите имя направления: ");
            _direction = Console.ReadLine();

            if (string.IsNullOrEmpty(_direction))
            {
                Console.WriteLine("Неверный ввод. Направление не назначено!");
                return;
            }

            _currentTrain.TrySetDirection(_direction);
        }

        private void SellTickets()
        {
            int[] tickets = new int[_typeWagons.Length];

            for (int i = 0; i < tickets.Length; i++)
                tickets[i] = _random.Next(100) + 1;

            _currentTrain.TrySetTickets(tickets);
        }

        private void FormTrain() =>
            _currentTrain.TryFormTrain();

        private void SendTrain()
        {
            if (_currentTrain.TrySend())
            {
                _trains.Add(_currentTrain);
                _currentTrain = new Train(_typeWagons);
            }
        }

        private void ShowInfo()
        {
            Console.Clear();
            ShowHead();

            for (int i = 0; i < _trains.Count; i++)
                _trains[i].ShowInfo();

            _currentTrain.ShowInfo();
        }

        private void ShowHead()
        {
            const int Width1 = -25;
            const int Width2 = -19;
            const int Width3 = 16;

            string head = $"|  id | {"Направление",Width1} |" +
                $" {"Продано билетов",Width2} | {"Вагоны",Width2} | {"Статус",Width3} |";

            Console.WriteLine(head);
            Console.WriteLine(new string('=', head.Length));
        }

        private void ShowBottom()
        {
            int left = 0;
            int top = 9;
            int bottom = 18;

            Console.SetCursorPosition(left, bottom);
            ShowHead();
            _currentTrain.ShowInfo();
            Console.SetCursorPosition(left, top);
        }
    }

    public class Train
    {
        private static int _ids = 1;
        private int _id;
        private ITrainState _state = new StandsDepoState();
        private string _direction = "Не задано";
        private int[] _tickets;
        private TypeWagon[] _typeWagons;
        private int[] _wagons;

        public Train(TypeWagon[] typeWagons)
        {
            _id = _ids++;
            _typeWagons = typeWagons;
            _wagons = new int[typeWagons.Length];
            _tickets = new int[typeWagons.Length];
        }

        public void TrySetDirection(string direction)
        {
            if (TrySetState(new TicketSalesState()))
            {
                _direction = direction;
                Console.WriteLine("Направление для поезда успешно задано, можно продавать билеты.");
            }
        }

        public void TrySetTickets(int[] tickets)
        {
            for (int i = 0; i < tickets.Length; i++)
                if (tickets[i] <= 0)
                    return;

            if ((_state is TicketSalesState) == false)
            {
                Console.WriteLine("Продажа билетов закрыта.");
                return;
            }

            for (int i = 0;i < _tickets.Length;i++)
                _tickets[i] += tickets[i];

            Console.WriteLine($"Успешно продано билетов в количестве:");

            string places = Format(tickets);
            Console.WriteLine(places);
        }

        public void TryFormTrain()
        {
            if (TrySetState(new ReadyToShipState()))
            {
                for (int i = 0; i < _wagons.Length; i++)
                    _wagons[i] = _tickets[i] / _typeWagons[i].Seats + 1;

                Console.WriteLine("Продажа билетов остановлена, команда персонала набрана. Поезд готов к отбытию.");
            }
        }

        public bool TrySend()
        {
            if (TrySetState(new OnWayState()) == false)
                return false;

            Console.WriteLine($"Поезд успешно отправлен в путь по направению \"{_direction}\".");
            return true;
        }

        public void ShowInfo()
        {
            const int Width0 = 3;
            const int Width1 = -25;
            const int Width2 = -19;
            const int Width3 = 16;

            string places = Format(_tickets);
            string wagons = Format(_wagons);

            Console.WriteLine($"| {_id,Width0} | {_direction,Width1} |" +
                $" {places,Width2} | {wagons,Width2} | {_state.Name,Width3} |");
        }

        private bool TrySetState(ITrainState state)
        {
            if (_state.CanNextStep(state) == false)
                return false;

            _state = state;
            return true;
        }

        private string Format(int[] array)
        {
            const int Width = 3;

            List<string> result = new List<string>();

            for (int i = 0; i < _typeWagons.Length; i++)
                result.Add($"[{_typeWagons[i].Name} - {array[i],Width}]");

            return string.Join(' ', result);
        }
    }

    public class TypeWagon
    {
        public TypeWagon(string name, int seats)
        {
            Name = name;
            Seats = seats;
        }

        public string Name { get; }
        public int Seats { get; }
    }

    #region State Состояния поезда

    public interface ITrainState
    {
        public bool CanNextStep(ITrainState state);

        public string Name { get; }
    }

    public class StandsDepoState : ITrainState
    {
        public string Name => "В депо";

        public bool CanNextStep(ITrainState state)
        {
            if (state is TicketSalesState)
                return true;

            Console.WriteLine("Поезд находится в депо...");
            return false;
        }
    }

    public class TicketSalesState : ITrainState
    {
        public string Name => "Продажа билетов";

        public bool CanNextStep(ITrainState state)
        {
            if (state is ReadyToShipState)
                return true;

            Console.WriteLine("Идёт продажа билетов...");
            return false;
        }
    }

    public class ReadyToShipState : ITrainState
    {
        public string Name => "Готов к отправке";

        public bool CanNextStep(ITrainState state)
        {
            if (state is OnWayState)
                return true;

            Console.WriteLine("Ждём отправления поезда...");
            return false;
        }
    }

    public class OnWayState : ITrainState
    {
        public string Name => "В пути";

        public bool CanNextStep(ITrainState state)
        {
            Console.WriteLine("Поезд уже в пути...");
            return false;
        }
    }

    #endregion
}