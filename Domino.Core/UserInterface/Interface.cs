using System.Collections.Generic;
using Domino.Core.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core.UserInterface
{
    public class Interface
    {
        public Table Table { get; set; }
        public List<Button> Buttons { get; }
        private readonly Texture2D _buttonTexture;

        public Interface(Table table, Texture2D buttonTexture)
        {
            Table = table;
            _buttonTexture = buttonTexture;
            Buttons = [
                new Button(
                    position: new Vector2(73, 450),
                    spriteSheet: _buttonTexture)
            ];
        }

        public void Update(
            GameTime gameTime, 
            Vector2 mousePosition, 
            bool mouseJustClicked)
        {
            foreach (var button in Buttons)
            {
                button.Update(
                    mousePosition: mousePosition, 
                    mouseJustClicked: mouseJustClicked);
            }
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