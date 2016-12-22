using System;
using SnakeGame.Interfaces;

namespace SnakeGame.Model
{
    public class SnakePart : IEatable, IComparable<SnakePart>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public SnakePart() : this(0, 0) { }

        public SnakePart(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void GenerateFood(int maxTileWidth, int maxTileHeight)
        {
            Random random = new Random();

            X = random.Next(0, maxTileWidth);
            Y = random.Next(0, maxTileHeight);
        }

        public int CompareTo(SnakePart snakePart)
        {
            int result = 0;

            int x = this.X.CompareTo(snakePart.X);
            int y = this.Y.CompareTo(snakePart.Y);

            if (!(x == 0 && y == 0))
            {
                result = -1;
            }

            return result;
        }
    }
}
