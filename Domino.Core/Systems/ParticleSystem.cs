using System;
using System.Collections.Generic;
using Domino.Core.Visuals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core.Systems
{
    public class ParticleSystem
    {
        private readonly List<Particle> _particles = [];
        private readonly Random _rng = new();
        private readonly Texture2D _pixel;

        public ParticleSystem(GraphicsDevice graphicsDevice)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData([Color.White]);
        }

        public void Emit(Vector2 position, Color color, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _particles.Add(new Particle
                {
                    Position = position,
                    Velocity = new Vector2(
                        (float)_rng.NextDouble() * 4 - 2,
                        (float)_rng.NextDouble() * 4 - 2),
                    Color = color,
                    Lifespan = Constants.ParticleLifespanMin +
                               (float)_rng.NextDouble() *
                               (Constants.ParticleLifespanMax -
                                Constants.ParticleLifespanMin)
                });
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                _particles[i].Update(gameTime);
                if (_particles[i].IsDead) _particles.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(0.5f, 0.5f);
            foreach (var p in _particles)
            {
                if (Constants.ParticleHasBorder)
                {
                    spriteBatch.Draw(
                        texture: _pixel,
                        position: p.Position,
                        sourceRectangle: null,
                        color: Constants.ParticleBorderColor,
                        rotation: 0f,
                        origin: origin,
                        scale: Constants.ParticleScale + 
                        Constants.ParticleBorderThickness,
                        effects: SpriteEffects.None,
                        layerDepth: 0f);
                }
                
                spriteBatch.Draw(
                    texture: _pixel,
                    position: p.Position,
                    sourceRectangle: null,
                    color: p.Color,
                    rotation: 0f,
                    origin: origin,
                    scale: Constants.ParticleScale,
                    effects: SpriteEffects.None,
                    layerDepth: 0f);
            }
        }
    }
}