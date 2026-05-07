using System;
using System.Collections.Generic;
using Domino.Core.Visuals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core.Systems
{
    public class Book
    {
        private readonly List<FloatingText> _texts = [];
        private readonly SpriteFont _font;
        private readonly Random _rng = new();

        public Book(SpriteFont font) => _font = font;

        public void Add(string text, Vector2 position, Color color)
        {
            _texts.Add(new FloatingText
            {
                Text = text,
                Position = position,
                Color = color,
                Velocity = new Vector2(
                    (float)_rng.NextDouble() * 2 - 1,
                    -1.5f),
                Lifespan = Constants.TextLifespan
            });
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _texts.Count - 1; i >= 0; i--)
            {
                _texts[i].Update(gameTime);
                if (_texts[i].IsDead) _texts.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = Vector2.Zero;
            foreach (var text in _texts)
            {
                spriteBatch.DrawString(
                    spriteFont: _font,
                    text: text.Text,
                    position: text.Position + Vector2.One,
                    color: Color.Black,
                    rotation: 0f,
                    origin: origin,
                    scale: Constants.TextScale,
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );
                spriteBatch.DrawString(
                    spriteFont: _font,
                    text: text.Text,
                    position: text.Position,
                    color: text.Color,
                    rotation: 0f,
                    origin: origin,
                    scale: Constants.TextScale,
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );
            }
        }
    }
}