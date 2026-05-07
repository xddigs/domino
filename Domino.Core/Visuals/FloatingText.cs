using Microsoft.Xna.Framework;

namespace Domino.Core.Visuals
{
    public class FloatingText
    {
        public string Text { get; init; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; init; }
        public Color Color { get; init; }
        public float Lifespan { get; set; }
        public float Alpha { get; set; } = 1f;

        public bool IsDead => Lifespan <= 0;

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity;
            Lifespan -= delta;
            Alpha = MathHelper.Clamp(Lifespan, 0, 1); 
        }
    }
}