using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core.Systems
{
    public class Background
    {
        private readonly Texture2D _pixel;
        private const float Speed = 50f;
        private const int TileSize = 64;
        private Vector2 _offset;

        public Background(GraphicsDevice graphicsDevice)
        {
            _pixel = new Texture2D(graphicsDevice, 2, 2);
            var data = new Color[4];
            
            Color color1 = new Color(10, 125, 255);
            Color color2 = new Color(120, 155, 237);

            data[0] = color1; data[1] = color2;
            data[2] = color2;  data[3] = color1;

            _pixel.SetData(data);
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            _offset.X += Speed * delta;
            _offset.Y += Speed * delta;

            _offset.X %= (TileSize * 2);
            _offset.Y %= (TileSize * 2);
        }

        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointWrap,
                depthStencilState: DepthStencilState.None,
                rasterizerState: RasterizerState.CullCounterClockwise
            );
            spriteBatch.Draw(
                texture: _pixel,
                destinationRectangle: new Rectangle(0, 0, 
                    screenWidth, screenHeight),
                sourceRectangle: new Rectangle(
                    (int)(_offset.X), 
                    (int)(_offset.Y), 
                    screenWidth / TileSize, 
                    screenHeight / TileSize),
                color: Color.White
            );
            spriteBatch.End();
        }
    }
}