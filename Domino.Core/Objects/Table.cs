using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core.Objects
{
    public class Table
    {
        public List<Tile> Tiles { get; }
        public Texture2D Atlas { get; set; }
        private const int Cols = 7;
        private const int Rows = 4;
        
        public Table(Texture2D atlas)
        {
            Tiles = [];
            Atlas = atlas;
            this.Populate();
        }

        public void Populate()
        {
            int tileWidth = Atlas.Width / Cols; 
            int tileHeight = Atlas.Height / Rows;
            int spacing = 10; // Espacio entre fichas

            int count = 0;
            for (int i = 0; i <= 6; i++)
            {
                for (int j = i; j <= 6; j++)
                {
                    int column = count % Cols;
                    int row = count / Cols;

                    Rectangle sourceRect = new Rectangle(
                        column * tileWidth, 
                        row * tileHeight, 
                        tileWidth, 
                        tileHeight
                    );

                    var tile = new Tile(i, j, sourceRect)
                    {
                        Position = new Vector2(
                            column * (tileWidth + spacing) + 50, 
                            row * (tileHeight + spacing) + 50
                        )
                    };

                    Tiles.Add(tile);
                    count++;
                }
            }
        }
        
        public Tile GetTile(int val1, int val2)
        {
            return Tiles.Find(t => 
                (t.UpperValue == val1 && t.LowerValue == val2) || 
                (t.UpperValue == val2 && t.LowerValue == val1));
        }
        
        public void Shuffle()
        {
            var rng = new Random();
            var shuffled = Tiles.OrderBy(a => rng.Next()).ToList();
            Tiles.Clear();
            Tiles.AddRange(shuffled);
        }

        public void Update(GameTime gameTime)
        {
            
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in Tiles)
            {
                Vector2 origin = new Vector2(
                    tile.SourceRectangle.Width / 2f, 
                    tile.SourceRectangle.Height / 2f);
                
                Vector2 drawPosition = tile.Position + origin;

                spriteBatch.Draw(
                    texture: Atlas, 
                    position: drawPosition,
                    sourceRectangle: tile.SourceRectangle,
                    color: Color.White,
                    rotation: tile.Rotation,
                    origin: origin,
                    scale: tile.Scale,
                    effects: SpriteEffects.None, 
                    layerDepth: 0f
                );
            }
        }
    }
}