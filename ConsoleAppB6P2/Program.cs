using System;

namespace ConsoleAppB6P2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Player player = new Player('☺', 10, 0);
            Renderer renderer = new Renderer();

            renderer.Draw(player.PositionX, player.PositionY, player.Symbol);

            Console.ReadKey();
        }
    }

    public class Renderer
    {
        private int _maxX = 100;
        private int _maxY = 25;

        public void Draw(int positionX, int positionY, char symbol = '@')
        {

            if (positionX < 0 && positionX > _maxX || positionY < 0 && positionY > _maxY)
                return;

            Console.SetCursorPosition(positionX, positionY);
            Console.Write(symbol);
        }
    }

    public class Player
    {
        public char Symbol { get; private set; }
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }

        public Player(char symbol, int positionX, int positionY)
        {
            Symbol = symbol;
            PositionX = positionX;
            PositionY = positionY;
        }
    }
}