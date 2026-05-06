using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core.UserInterface
{
    public class Button
    {
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsHovered { get; set; }
        public Action OnClick { get; set; }

        private readonly Texture2D _spriteSheet;
        private readonly Rectangle _sourceNormal;
        private readonly Rectangle _sourceHover;

        private float _currentScale;
        private float _targetScale = 1f;
        private const float BaseScale = 1f;

        public Button(Vector2 position, Action action, Texture2D spriteSheet)
        {
            Position = position;
            OnClick = action;
            _spriteSheet = spriteSheet;
            int frameWidth = spriteSheet.Width / 2;
            int frameHeight = spriteSheet.Height;

            _sourceNormal = new Rectangle(0, 0, frameWidth, frameHeight);
            _sourceHover =
                new Rectangle(frameWidth, 0, frameWidth, frameHeight);

            Bounds = new Rectangle((int)position.X, (int)position.Y, frameWidth,
                frameHeight);
            _currentScale = BaseScale;
        }

        public void Update(
            Vector2 mousePosition, 
            bool mouseJustClicked)
        {
            Bounds = new Rectangle(
                (int)(Position.X), 
                (int)(Position.Y), 
                _sourceNormal.Width, 
                _sourceNormal.Height);
            
            IsHovered = Bounds.Contains(mousePosition.ToPoint());

            if (IsHovered)
            {
                _targetScale = BaseScale * 1.2f;

                if (mouseJustClicked)
                {
                    _currentScale = BaseScale * 0.9f;
                    OnClick?.Invoke();
                }
            }
            else
            {
                _targetScale = BaseScale;
            }

            _currentScale = MathHelper.Lerp(
                _currentScale, 
                _targetScale, 0.15f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle source = IsHovered ? _sourceHover : _sourceNormal;
            Vector2 origin = new Vector2(
                _sourceNormal.Width / 2f,
                _sourceNormal.Height / 2f);
            
            Vector2 drawPos = new Vector2(
                Bounds.X + (Bounds.Width / 2f),
                Bounds.Y + (Bounds.Height / 2f)
            );

            spriteBatch.Draw(
                texture: _spriteSheet,
                position: drawPos,
                sourceRectangle: source,
                color: Color.White,
                rotation: 0f,
                origin: origin,
                scale: _currentScale,
                effects: SpriteEffects.None,
                layerDepth: 0f);
        }
    }
}