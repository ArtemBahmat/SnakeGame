using System;

namespace SnakeGame
{
    public class SnakePart : IEatable
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
    }
}
