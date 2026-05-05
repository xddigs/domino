using System;
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
        
        public Rectangle Bounds
        {
            get
            {
                int width = SourceRectangle.Width;
                int height = SourceRectangle.Height;

                float angle = MathHelper.WrapAngle(Rotation);
                if (Math.Abs(angle) > 0.5f && Math.Abs(angle) < 2.5f)
                {
                    width = SourceRectangle.Height;
                    height = SourceRectangle.Width;
                }

                return new Rectangle(
                    (int)(Position.X - width / 2f),
                    (int)(Position.Y - height / 2f),
                    width,
                    height
                );
            }
        }

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