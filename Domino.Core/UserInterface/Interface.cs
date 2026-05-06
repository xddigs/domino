using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core.UserInterface
{
    public class Interface
    {
        public List<Button> Buttons { get; }
        private readonly Texture2D _buttonTexture;

        public Interface(Texture2D buttonTexture)
        {
            _buttonTexture = buttonTexture;
            Buttons = [
                new Button(
                    texture2D: _buttonTexture,
                    action: () => Console.WriteLine(@"Button 1"),
                    position: new Vector2(100, 100))
            ];
        }

        public void Update(GameTime gameTime)
        {
            
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: DepthStencilState.None,
                rasterizerState: RasterizerState.CullCounterClockwise
            );
            foreach (var button in Buttons)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}