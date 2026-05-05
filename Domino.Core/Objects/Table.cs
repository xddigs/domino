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
        public Texture2D Atlas { get; }

        private readonly Vector2 _headDirection = new(-1, 0);
        private readonly Vector2 _tailDirection = new(1, 0);

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
        private const int TilePadding = -2;
        private const float SnapThreshold = 100f;

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

            Vector2 centerOffset = new Vector2(tileWidth / 2f, tileHeight / 2f);

            float handTotalWidth = (ActiveHand * (tileWidth + Spacing)) -
                                   Spacing;

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

                tile.Position += centerOffset;
                tile.LastPosition = tile.Position;
            }
        }

        public void TryPlaceTile(Tile tile, Vector2 dropPosition)
        {
            if (ActiveTiles.Contains(tile)) return;

            if (ActiveTiles.Count == 0)
            {
                tile.Position = new Vector2(ScreenWidth / 2f,
                    ScreenHeight / 2f);
                tile.Rotation = 0f;
                tile.Scale = 1.0f;
                tile.LastPosition = tile.Position;
                tile.HeadValue = tile.UpperValue;
                tile.TailValue = tile.LowerValue;
                ActiveTiles.AddFirst(tile);
                return;
            }

            Tile head = ActiveTiles.First!.Value;
            Console.WriteLine($@"HEAD: {head.UpperValue}|{head.LowerValue}");

            float distHead = Vector2.Distance(dropPosition, head.Position);
            if (distHead < SnapThreshold)
            {
                if (CanConnect(
                        newTile: tile,
                        anchor: head,
                        isHead: true,
                        mustFlip: out bool mustFlip))
                {
                    SnapTo(
                        newTile: tile,
                        anchor: head,
                        isHead: true,
                        mustFlip: mustFlip);
                    tile.Scale = 1.0f;
                    tile.LastPosition = tile.Position;
                    ActiveTiles.AddFirst(tile);
                    return;
                }
            }

            Tile tail = ActiveTiles.Last!.Value;
            Console.WriteLine($@"TAIL: {tail.UpperValue}|{tail.LowerValue}");

            float distTail = Vector2.Distance(dropPosition, tail.Position);
            if (distTail < SnapThreshold)
            {
                if (CanConnect(
                        newTile: tile,
                        anchor: tail,
                        isHead: false,
                        mustFlip: out bool mustFlip))
                {
                    SnapTo(
                        newTile: tile,
                        anchor: tail,
                        isHead: false,
                        mustFlip: mustFlip);
                    tile.Scale = 1.0f;
                    tile.LastPosition = tile.Position;
                    ActiveTiles.AddLast(tile);
                    return;
                }
            }
        }

        private bool CanConnect(
            Tile newTile,
            Tile anchor,
            bool isHead,
            out bool mustFlip)
        {
            mustFlip = false;

            int openValue = GetOpenValue(anchor, isHead);
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

            return false;
        }

        private int GetOpenValue(Tile tile, bool isHead)
        {
            return isHead ? tile.HeadValue!.Value : tile.TailValue!.Value;
        }

        private Vector2 GetAnchorDirection(bool isHead)
        {
            return isHead ? _headDirection : _tailDirection;
        }

        private void SnapTo(Tile newTile, Tile anchor, bool isHead,
            bool mustFlip)
        {
            int tileHeight = Atlas.Height / Rows;
            int tileWidth = Atlas.Width / Cols;

            Vector2 outDirection = GetAnchorDirection(isHead);
            bool isDouble = newTile.UpperValue == newTile.LowerValue;
            bool anchorIsDouble = anchor.UpperValue == anchor.LowerValue;

            float distance;
            if (anchorIsDouble)
            {
                distance = (tileWidth / 2f) + (tileHeight / 2f) + TilePadding;
            }
            else if (isDouble)
            {
                distance = (tileHeight / 2f) + (tileWidth / 2f) + TilePadding;
            }
            else
            {
                distance = tileHeight + TilePadding;
            }

            newTile.Position = anchor.Position + (outDirection * distance);

            if (isDouble)
            {
                newTile.Rotation = 0f;
            }
            else
            {
                float baseRotation;
                if (Math.Abs(outDirection.X) > 0.5f)
                {
                    baseRotation = (outDirection.X < 0)
                        ? MathHelper.PiOver2
                        : -MathHelper.PiOver2;
                }
                else
                {
                    baseRotation = (outDirection.Y < 0) ? 0f : MathHelper.Pi;
                }

                newTile.Rotation =
                    mustFlip ? baseRotation + MathHelper.Pi : baseRotation;
            }

            newTile.LastPosition = newTile.Position;

            int connectingValue =
                isHead ? anchor.HeadValue!.Value : anchor.TailValue!.Value;

            if (isHead)
            {
                newTile.HeadValue = (newTile.UpperValue == connectingValue)
                    ? newTile.LowerValue
                    : newTile.UpperValue;
                newTile.TailValue = connectingValue;
            }
            else
            {
                newTile.TailValue = (newTile.UpperValue == connectingValue)
                    ? newTile.LowerValue
                    : newTile.UpperValue;
                newTile.HeadValue = connectingValue;
            }
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

                spriteBatch.Draw(
                    texture: Atlas,
                    position: tile.Position,
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