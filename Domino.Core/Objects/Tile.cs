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

        public float Rotation { get; set; }
        public float Scale { get; set; }
        public const float MaxScale = 2f;
        public float TargetScale { get; set; } = MaxScale;
        public Vector2 Velocity { get; set; }
        public Vector2 LastPosition { get; set; }

        public int? HeadValue { get; set; }
        public int? TailValue { get; set; }
        
        public enum TileOwner { Boneyard, Player, Ai, Board }
        public TileOwner Owner { get; set; } = TileOwner.Boneyard;
        
        public Rectangle Bounds
        {
            get
            {
                float width = SourceRectangle.Width * Scale;
                float height = SourceRectangle.Height * Scale;

                float angle = MathHelper.WrapAngle(Rotation);

                if (Math.Abs(angle) > MathHelper.PiOver4 && Math.Abs(angle) <
                    (MathHelper.Pi - MathHelper.PiOver4))
                {
                    (width, height) = (height, width);
                }

                return new Rectangle(
                    (int)(Position.X - width / 2f),
                    (int)(Position.Y - height / 2f),
                    (int)width,
                    (int)height
                );
            }
        }

        public Tile(int upperValue, int lowerValue, Rectangle sourceRect)
        {
            UpperValue = upperValue;
            LowerValue = lowerValue;
            SourceRectangle = sourceRect;
            Position = Vector2.Zero;
            Scale = MaxScale;
        }
    }
}