using Domino.Core.Systems;
using Microsoft.Xna.Framework;

namespace Domino.Core.Visuals
{
    public class Particle
    {
        public Vector2 Position { get; set;}
        public Vector2 Velocity { get; init; }
        public Color Color { get; init; }
        public float Lifespan { get; set; }
        public float Alpha { get; set; } = 1.0f;

        public bool IsDead => Lifespan <= 0;

        public void Update(GameTime gameTime)
        {
            Position += Velocity;
            Lifespan -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            Alpha = MathHelper.Clamp(Lifespan, 0, 1) * 
                    Constants.ParticleBaseAlpha;
        }
    }
}