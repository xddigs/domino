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
        public LinkedList<Tile> ActiveTiles { get; }
        public Texture2D Atlas { get; set; }

        private const int Cols = 7;
        private const int Rows = 4;
        private const int ScreenWidth = 1280;
        private const int ScreenHeight = 720;
        private const int ActiveHand = 7;
        private const int TotalActive = 14;
        private const int Spacing = 15;
        private const int BottomMargin = 40;
        private const int OffscreenOffset = 100;
        private const int BoneyardX = 60;
        private const int StackOffset = 1;
        private const int TilePadding = 2;
        private const float SnapThreshold = 60f;

        public Table(Texture2D atlas)
        {
            Tiles = [];
            ActiveTiles = [];
            Atlas = atlas;
            Populate();
            Shuffle();
            Layout();
        }

        public void Populate()
        {
            int tileWidth = Atlas.Width / Cols;
            int tileHeight = Atlas.Height / Rows;

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

                    Tiles.Add(new Tile(i, j, sourceRect));
                    count++;
                }
            }
        }

        public void Layout()
        {
            if (Atlas == null) return;

            int tileWidth = Atlas.Width / Cols;
            int tileHeight = Atlas.Height / Rows;

            float handTotalWidth = (ActiveHand *
                                    (tileWidth + Spacing)) - Spacing;

            float handStartX = (ScreenWidth - handTotalWidth) / 2f;
            float boneyardCenterY = (ScreenHeight / 2f) - (tileHeight / 2f);

            for (int i = 0; i < Tiles.Count; i++)
            {
                var tile = Tiles[i];

                if (i < ActiveHand)
                {
                    tile.Position = new Vector2(
                        handStartX + i * (tileWidth + Spacing),
                        ScreenHeight - tileHeight - BottomMargin
                    );
                }
                else if (i < TotalActive)
                {
                    int index = i - ActiveHand;
                    tile.Position = new Vector2(
                        handStartX + index * (tileWidth + Spacing),
                        -tileHeight - OffscreenOffset
                    );
                }
                else
                {
                    int stackIndex = i - TotalActive;
                    tile.Position = new Vector2(
                        BoneyardX + (stackIndex * StackOffset),
                        boneyardCenterY + (stackIndex * StackOffset)
                    );
                }

                tile.LastPosition = tile.Position;
            }
        }

        public bool TryPlaceTile(Tile tile)
        {
            if (ActiveTiles.Count == 0)
            {
                tile.Position =
                    new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
                tile.Rotation = 0f;
                tile.LastPosition = tile.Position;
                ActiveTiles.AddFirst(tile);
                return true;
            }

            Tile head = ActiveTiles.First!.Value;
            Tile tail = ActiveTiles.Last!.Value;

            if (Vector2.Distance(tile.Position,
                    head.Position) < SnapThreshold)
            {
                if (CanConnect(tile, head, out bool mustFlip))
                {
                    SnapTo(tile, head, true, mustFlip);
                    tile.Scale = 1.0f;
                    ActiveTiles.AddFirst(tile);
                    return true;
                }
            }

            if (Vector2.Distance(tile.Position,
                    tail.Position) < SnapThreshold)
            {
                if (CanConnect(tile, tail, out bool mustFlip))
                {
                    SnapTo(tile, tail, false, mustFlip);
                    ActiveTiles.AddLast(tile);
                    return true;
                }
            }

            return false;
        }

        private bool CanConnect(Tile newTile, Tile anchor, out bool mustFlip)
        {
            mustFlip = false;
            bool isHead = ActiveTiles.First!.Value == anchor;
            int openValue = isHead ? anchor.UpperValue : anchor.LowerValue;

            if (isHead)
            {
                if (newTile.LowerValue == openValue)
                {
                    mustFlip = false;
                    return true;
                }

                if (newTile.UpperValue == openValue)
                {
                    mustFlip = true;
                    return true;
                }
            }
            else
            {
                if (newTile.UpperValue == openValue)
                {
                    mustFlip = false;
                    return true;
                }

                if (newTile.LowerValue == openValue)
                {
                    mustFlip = true;
                    return true;
                }
            }

            return false;
        }

        private void SnapTo(
            Tile newTile,
            Tile anchor,
            bool isHead,
            bool mustFlip)
        {
            float direction = isHead ? -1f : 1f;
            int tileWidth = Atlas.Width / Cols;

            newTile.Position = new Vector2(
                anchor.Position.X + (tileWidth + TilePadding) * direction,
                anchor.Position.Y
            );

            newTile.Rotation = mustFlip ? MathHelper.Pi : 0f;
            newTile.LastPosition = newTile.Position;
        }

        public void Shuffle()
        {
            var rng = new Random();
            var shuffled = Tiles.OrderBy(a => rng.Next()).ToList();
            Tiles.Clear();
            Tiles.AddRange(shuffled);
            Layout();
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