using System;
using System.Collections.Generic;
using System.Linq;
using Domino.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core.Objects
{
    public class Table
    {
        private static readonly Random Rng = new();
        public List<Tile> Tiles { get; }
        public LinkedList<Tile> ActiveTiles { get; }
        public Texture2D Atlas { get; }
        public Texture2D BackTile { get; }
        
        public Turn Turn { get; set; }
        
        private Vector2 _currentHeadDir = new(-1, 0);
        private Vector2 _currentTailDir = new(1, 0);
        private int _headRowCount;
        private int _tailRowCount;
        
        private Tile _ghostTile;
        private Vector2 _ghostPosition;
        private float _ghostRotation;
        private bool _showGhost;
        
        private const int Cols = 7;
        private const int Rows = 4;
        private const int ScreenWidth = 1280;
        private const int ScreenHeight = 720;
        private const int BottomMargin = 40;
        private const int OponentHandOffset = -150;
        private const int BoneyardX = 60;
        private const int StackOffset = 1;
        private const int TilePadding = -4;
        private const int Spacing = 20;
        private const float SnapThreshold = 100f;
        private const int MaxTilesPerRow = 4;

        public Table(Texture2D atlas, Texture2D backTile)
        {
            Tiles = [];
            ActiveTiles = [];
            Atlas = atlas;
            BackTile = backTile;
            Turn = Turn.Player;
            Populate();
            Shuffle();
            Layout();
            Deal();
            
            foreach (var t in Tiles) 
                t.Position = t.LastPosition;
            
            FirstTurn();
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

            float scaledWidth = (Atlas.Width / Cols) * Tile.MaxScale;
            float scaledHeight = (Atlas.Height / Rows) * Tile.MaxScale;
            Vector2 centerOffset =
                new Vector2(scaledWidth / 2f, scaledHeight / 2f);

            var playerTiles = Tiles.Where(t => t.Owner == Tile.TileOwner.Player)
                .ToList();
            float playerHandWidth =
                (playerTiles.Count * (scaledWidth + Spacing)) - Spacing;
            float playerStartX = (ScreenWidth - playerHandWidth) / 2f;

            for (int i = 0; i < playerTiles.Count; i++)
            {
                playerTiles[i].LastPosition = new Vector2(
                    playerStartX + i * (scaledWidth + Spacing),
                    ScreenHeight - scaledHeight - BottomMargin) + centerOffset;
            }

            var aiTiles = Tiles.Where(
                t => t.Owner == Tile.TileOwner.Machine).ToList();
            
            float aiHandWidth = (aiTiles.Count * 
                                 (scaledWidth + Spacing)) - Spacing;
            float aiStartX = (ScreenWidth - aiHandWidth) / 2f;

            for (int i = 0; i < aiTiles.Count; i++)
            {
                aiTiles[i].LastPosition = new Vector2(
                    aiStartX + i * (scaledWidth + Spacing),
                    -scaledHeight - OponentHandOffset
                ) + centerOffset;
            }

            var boneyardTiles = Tiles.Where(
                t => t.Owner == Tile.TileOwner.Boneyard).ToList();
            float boneyardCenterY = (ScreenHeight / 2f) - (scaledHeight / 2f);

            for (int i = 0; i < boneyardTiles.Count; i++)
            {
                boneyardTiles[i].LastPosition = new Vector2(
                    BoneyardX + (i * StackOffset),
                    boneyardCenterY + (i * StackOffset)
                ) + centerOffset;
            }
        }
        
        public void Deal()
        {
            for (int i = 0; i < 7; i++)
            {
                Rob(Tile.TileOwner.Player);
            }

            for (int i = 0; i < 7; i++)
            {
                Rob(Tile.TileOwner.Machine);
            }
        }
        
        public Turn SwitchTurn()
        {
            return Turn == Turn.Machine
                ? Turn.Player : Turn.Machine;
        }
        
        public void FirstTurn()
        {
            int playerMaxDouble = GetMaxDouble(Tile.TileOwner.Player);
            int aiMaxDouble = GetMaxDouble(Tile.TileOwner.Machine);

            if (playerMaxDouble > aiMaxDouble)
                Turn = Turn.Player;
            else if (aiMaxDouble > playerMaxDouble)
                Turn = Turn.Machine;
            else
                Turn = Turn.Player; 
        }

        private int GetMaxDouble(Tile.TileOwner owner)
        {
            var doubles = Tiles.Where(t => t.Owner == owner 
                && t.UpperValue == t.LowerValue).ToList();
            return doubles.Count != 0 ? doubles.Max(t => t.UpperValue) : -1;
        }
        
        public void UpdateGhost(Tile draggingTile, Vector2 mousePosition)
        {
            _showGhost = false;
            if (ActiveTiles.Count == 0) return;

            Tile head = ActiveTiles.First!.Value;
            Tile tail = ActiveTiles.Last!.Value;

            float distHead = Vector2.Distance(mousePosition, head.Position);
            float distTail = Vector2.Distance(mousePosition, tail.Position);

            Tile anchor = null;
            bool isHead = false;

            if (distHead < SnapThreshold)
            {
                anchor = head;
                isHead = true;
            }
            else if (distTail < SnapThreshold)
            {
                anchor = tail;
            }

            if (anchor != null && CanConnect(draggingTile, anchor, isHead,
                    out bool mustFlip))
            {
                _ghostTile = draggingTile;

                Vector2 originalPos = draggingTile.Position;
                float originalRot = draggingTile.Rotation;
                Vector2 originalLastPos = draggingTile.LastPosition;

                SnapTo(
                    newTile: draggingTile, 
                    anchor: anchor, 
                    isHead: isHead, 
                    mustFlip: mustFlip, 
                    isPreview: true);

                _ghostPosition = draggingTile.Position;
                _ghostRotation = draggingTile.Rotation;

                draggingTile.Position = originalPos;
                draggingTile.Rotation = originalRot;
                draggingTile.LastPosition = originalLastPos;

                _showGhost = true;
            }
        }

        public void TryPlaceTile(Tile tile, Vector2 dropPosition)
        {
            if (ActiveTiles.Contains(tile)) return;

            if (ActiveTiles.Count == 0)
            {
                tile.Position = new Vector2(
                    x: ScreenWidth / 2f,
                    y: ScreenHeight / 2f);
                tile.Rotation = 0f;
                tile.LastPosition = tile.Position;
                tile.HeadValue = tile.UpperValue;
                tile.TailValue = tile.LowerValue;
                tile.Owner = Tile.TileOwner.Board;
                ActiveTiles.AddFirst(tile);
                _headRowCount = 0;
                _tailRowCount = 0;
                _currentHeadDir = new Vector2(-1, 0);
                _currentTailDir = new Vector2(1, 0);
                Layout();
                Turn = SwitchTurn();
                return;
            }

            Tile head = ActiveTiles.First!.Value;

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
                    tile.LastPosition = tile.Position;
                    tile.Owner = Tile.TileOwner.Board;
                    ActiveTiles.AddFirst(tile);
                    Layout();
                    Turn = SwitchTurn();
                    return;
                }
            }

            Tile tail = ActiveTiles.Last!.Value;

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
                    tile.LastPosition = tile.Position;
                    tile.Owner = Tile.TileOwner.Board;
                    ActiveTiles.AddLast(tile);
                    Layout();
                    Turn = SwitchTurn();
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

        private void SnapTo(Tile newTile, Tile anchor, bool isHead,
            bool mustFlip, bool isPreview = false)
        {
            int baseWidth = Atlas.Width / Cols;
            int baseHeight = Atlas.Height / Rows;
            float scaledWidth = baseWidth * Tile.MaxScale;
            float scaledHeight = baseHeight * Tile.MaxScale;

            Vector2 outDirection = isHead ? _currentHeadDir : _currentTailDir;
            int currentCount = isHead ? _headRowCount : _tailRowCount;
            
            bool isDouble = newTile.UpperValue == newTile.LowerValue;
            bool anchorIsDouble = anchor.UpperValue == anchor.LowerValue;

            bool isTurning = false;
            if (currentCount >= MaxTilesPerRow)
            {
                outDirection = new Vector2(0, 1); 
                isTurning = true;
            }

            float distance = (anchorIsDouble || isDouble || isTurning) 
                ? (scaledWidth / 2f) + (scaledHeight / 2f) + TilePadding
                : scaledHeight + TilePadding;

            newTile.Position = anchor.Position + (outDirection * distance);

            if (isDouble && !isTurning)
            {
                newTile.Rotation = 0f;
            }
            else
            {
                float baseRotation = (Math.Abs(outDirection.X) > 0.5f)
                    ? (outDirection.X < 0 ? MathHelper.PiOver2 : -MathHelper.PiOver2)
                    : MathHelper.PiOver2;

                newTile.Rotation = mustFlip ? baseRotation + MathHelper.Pi : baseRotation;
            }

            if (!isPreview)
            {
                if (isTurning)
                {
                    if (isHead) {
                        _headRowCount = 0;
                        _currentHeadDir.X *= -1; 
                    } else {
                        _tailRowCount = 0;
                        _currentTailDir.X *= -1;
                    }
                }
                else
                {
                    if (isHead) _headRowCount++; else _tailRowCount++;
                }
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

        public void Rob(Tile.TileOwner newOwner)
        {
            var tileToRob = Tiles.LastOrDefault(
                t => t.Owner == Tile.TileOwner.Boneyard);

            if (tileToRob != null)
            {
                Vector2 inBoneyard = tileToRob.Position;
                tileToRob.Owner = newOwner;
                Layout();
                tileToRob.Position = inBoneyard;
            }
        }

        public void Shuffle()
        {
            var shuffled = Tiles.OrderBy(a => Rng.Next()).ToList();
            Tiles.Clear();
            Tiles.AddRange(shuffled);
            Layout();
        }

        public void Draw(SpriteBatch spriteBatch, Tile selectedTile)
        {
            foreach (var tile in Tiles)
            {
                if (tile == selectedTile) continue;

                bool isFaceUp = tile.Owner is Tile.TileOwner.Player 
                    or Tile.TileOwner.Board;
    
                Texture2D texture = isFaceUp ? Atlas : BackTile;
                Rectangle? source = isFaceUp ? tile.SourceRectangle : null;

                Vector2 origin = isFaceUp 
                    ? new Vector2(
                        x: tile.SourceRectangle.Width / 2f, 
                        y: tile.SourceRectangle.Height / 2f)
                    : new Vector2(
                        x: BackTile.Width / 2f, 
                        y: BackTile.Height / 2f);

                spriteBatch.Draw(
                    texture: texture,
                    position: tile.Position,
                    sourceRectangle: source,
                    color: Color.White,
                    rotation: tile.Rotation,
                    origin: origin,
                    scale: tile.Scale,
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );
            }

            if (_showGhost && _ghostTile != null)
            {
                Vector2 ghostOrigin = new Vector2(
                    x: _ghostTile.SourceRectangle.Width / 2f,
                    y: _ghostTile.SourceRectangle.Height / 2f);

                spriteBatch.Draw(
                    texture: Atlas,
                    position: _ghostPosition,
                    sourceRectangle: _ghostTile.SourceRectangle,
                    color: Color.White * 0.15f,
                    rotation: _ghostRotation,
                    origin: ghostOrigin,
                    scale: _ghostTile.Scale,
                    effects: SpriteEffects.None,
                    layerDepth: 0.1f
                );
            }

            if (selectedTile != null)
            {
                Vector2 origin = new Vector2(
                    selectedTile.SourceRectangle.Width / 2f,
                    selectedTile.SourceRectangle.Height / 2f);

                spriteBatch.Draw(
                    texture: Atlas,
                    position: selectedTile.Position,
                    sourceRectangle: selectedTile.SourceRectangle,
                    color: Color.White,
                    selectedTile.Rotation,
                    origin: origin,
                    scale: selectedTile.Scale,
                    effects: SpriteEffects.None,
                    layerDepth: 0.2f
                );
            }
        }
    }
}