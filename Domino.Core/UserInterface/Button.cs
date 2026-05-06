using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Domino.Core.UserInterface
{
    public class Button
    {
        public string Text { get; set; }
        private readonly Texture2D _texture2D;
        public Action OnClick { get; set; }
        public Rectangle Bounds { get; set; }
        public Vector2 Position { get; set; }
        public bool IsPressed { get; set; }
        public bool IsHovered { get; set; }
        
        public Button(Texture2D texture2D, Action action, Vector2 position)
        {
            _texture2D = texture2D;
            OnClick = action;
            Position = position;
            Bounds = new Rectangle(
                (int)position.X, (int)position.Y,
                _texture2D.Width, _texture2D.Height);
        }

        public void Update(
            GameTime gameTime, 
            Rectangle mousePosition, 
            MouseState mouseClicked)
        {
            IsHovered = Bounds.Intersects(mousePosition);
            IsPressed = mouseClicked.LeftButton == ButtonState.Pressed;
            
            if (IsHovered && IsPressed)
            {
                IsHovered = IsPressed = false;
                OnClick?.Invoke();
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture2D, Position, Color.White);
        }
    }
}