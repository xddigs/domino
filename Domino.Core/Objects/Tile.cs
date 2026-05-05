using Microsoft.Xna.Framework;

namespace Domino.Core.Objects
{
    public class Tile
    {
        public int UpperValue { get; set; }
        public int LowerValue { get; set; }
        public Rectangle SourceRectangle { get; set; }
        
        public Vector2 Position { get; set; }
        public bool IsDragging { get; set; }
        
        public float Rotation { get; set;}
        public float Scale { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 LastPosition { get; set; }
        
        public Rectangle Bounds => 
            new(
                (int)Position.X, 
                (int)Position.Y, 
                SourceRectangle.Width, 
                SourceRectangle.Height);

        public Tile(int upperValue, int lowerValue, Rectangle sourceRect)
        {
            UpperValue = upperValue;
            LowerValue = lowerValue;
            SourceRectangle = sourceRect;
            Position = Vector2.Zero;
            Scale = 1.0f;
        }
    }
}