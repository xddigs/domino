using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Domino.Core.Systems
{
    public class Background
    {
        private readonly Texture2D _pixel;
        private readonly Effect _balatroEffect;
        private float _totalTime;

        public Background(
            GraphicsDevice graphicsDevice, 
            ContentManager content)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData([Color.White]);
            _balatroEffect = content.Load<Effect>("Background");
        }

        public void Update(GameTime gameTime)
        {
            _totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(
            SpriteBatch spriteBatch, 
            int screenWidth, 
            int screenHeight)
        {
            _balatroEffect.Parameters["iTime"]?.SetValue(_totalTime);
            _balatroEffect.Parameters["iResolution"]?.SetValue(
                new Vector2(screenWidth, screenHeight));

            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointWrap,
                effect: _balatroEffect
            );

            spriteBatch.Draw(
                texture: _pixel,
                destinationRectangle: new Rectangle(0, 0, screenWidth, 
                    screenHeight),
                color: Color.White
            );

            spriteBatch.End();
        }
    }
}