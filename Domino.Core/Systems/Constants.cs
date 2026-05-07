using Microsoft.Xna.Framework;

namespace Domino.Core.Systems
{
    public static class Constants
    {
        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 720;
        
        public const int Cols = 7;
        public const int Rows = 4;
        public const int BottomMargin = 40;
        public const int OponentHandOffset = -150;
        public const int BoneyardX = 60;
        public const int StackOffset = 1;
        public const int TilePadding = -4;
        public const int Spacing = 20;
        public const int MaxTilesPerRow = 4;
        public const float SnapThreshold = 100f;

        public static readonly Vector2 ButtonLocation = new(73, 600);

        public const float ParticleScale = 6f;
        public const float ParticleBaseAlpha = 1f;
        public const int ParticleQuantity = 8;
        public const float ParticleLifespanMin = 0.5f;
        public const float ParticleLifespanMax = 0.8f;
        public const float ParticleBorderThickness = 4f;
        public const float TextLifespan = 1.0f;
        public const float TextScale = 1.8f;
        
        public const float DefaultLifespan = 1.5f;
        public const float InitialVelocityY = -1.2f;
        public const float PopInDuration = 0.2f;
        public const float SettleDuration = 0.2f;
            
        public const float StartScale = 0f;
        public const float MaxPopScale = 1.3f;
        public const float FinalScale = 1.0f;
        
        public const bool ParticleHasBorder = true;
        
        public static readonly Color ParticleColor = new(255, 240, 197);
        public static readonly Color ParticleBorderColor = Color.Black;
        
    }
}