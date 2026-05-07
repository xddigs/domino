using Domino.Core.Systems;
using Microsoft.Xna.Framework;

namespace Domino.Core.Visuals
{
    public class FloatingText
    {
        public string Text { get; init; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; init; }
        public Color Color { get; init; }
        
        public float MaxLifespan { get; init; }
        public float Lifespan { get; set; }
        
        public float Alpha { get; private set; }
        public float Scale { get; private set; }

        public bool IsDead => Lifespan <= 0;

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            Position += Velocity;
            Lifespan -= delta;

            float lifePercent = MathHelper.Clamp(Lifespan / MaxLifespan, 0, 1);
            float age = 1f - lifePercent; 

            Alpha = lifePercent;

            if (age < Constants.PopInDuration)
            {
                Scale = MathHelper.Lerp(
                    Constants.StartScale, 
                    Constants.MaxPopScale, 
                    age / Constants.PopInDuration);
            }
            else if (age < (Constants.PopInDuration + Constants.SettleDuration))
            {
                float settleAge = age - Constants.PopInDuration;
                Scale = MathHelper.Lerp(
                    Constants.MaxPopScale, 
                    Constants.FinalScale, 
                    settleAge / Constants.SettleDuration);
            }
            else
            {
                Scale = Constants.FinalScale;
            }
        }
    }
}