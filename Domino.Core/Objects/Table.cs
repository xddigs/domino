using System.Collections.Generic;
using Domino.Core.Systems;
using Domino.Core.Visuals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core.Objects
{
    public class Table
    {
        public Rules Rules { get; }
        public LayoutManager LayoutManager { get; }
        public GameManager GameManager { get; }
        public Snapper Snapper { get; }
        public Ghost Ghost { get; set; }
        
        public List<Tile> Tiles { get; }
        public LinkedList<Tile> ActiveTiles { get; }
        public Texture2D Atlas { get; }
        public Texture2D BackTile { get; }
        public Tile StartingTile { get; set; }

        public Turn Turn { get; set; }

        public Vector2 CurrentHeadDir { get; set; } = new(-1, 0);
        public Vector2 CurrentTailDir { get; set; } = new(1, 0);
        public int HeadRowCount { get; set; }
        public int TailRowCount { get; set; }
        public int HeadSegmentIndex { get; set; }
        public int TailSegmentIndex { get; set; }
        
        public bool IsGameOver { get; set; }

        public Table(Texture2D atlas, Texture2D backTile)
        {
            Rules = new Rules(this);
            LayoutManager = new LayoutManager(this);
            GameManager = new GameManager(this);
            Snapper = new Snapper(this);
            Ghost = new Ghost(this);
            
            Tiles = [];
            ActiveTiles = [];
            Atlas = atlas;
            BackTile = backTile;
            Turn = Turn.Player;
            GameManager.Populate();
            GameManager.Shuffle();
            LayoutManager.SetLayout();
            GameManager.Deal();

            foreach (var t in Tiles)
                t.Position = t.LastPosition;

            Rules.FirstTurn();
        }

        public void Reboot()
        {
            IsGameOver = false;
            Tiles.Clear();
            ActiveTiles.Clear();

            HeadRowCount = 0;
            TailRowCount = 0;
            HeadSegmentIndex = 0;
            TailSegmentIndex = 0;
            CurrentHeadDir = new Vector2(-1, 0);
            CurrentTailDir = new Vector2(1, 0);
            StartingTile = null;

            GameManager.Populate();
            GameManager.Shuffle();
            LayoutManager.SetLayout();
            GameManager.Deal();
            Rules.FirstTurn();
        }

        public void TryPlaceTile(Tile tile, Vector2 dropPosition)
        {
            if (ActiveTiles.Contains(tile)) return;
            if (IsGameOver) return;

            if (ActiveTiles.Count == 0)
            {
                if (tile != StartingTile) return;
                tile.Position = new Vector2(
                    x: Constants.ScreenWidth / 2f,
                    y: Constants.ScreenHeight / 2f);
                tile.Rotation = 0f;
                tile.LastPosition = tile.Position;
                tile.HeadValue = tile.UpperValue;
                tile.TailValue = tile.LowerValue;
                tile.Owner = Tile.TileOwner.Board;
                ActiveTiles.AddFirst(tile);
                HeadRowCount = 0;
                TailRowCount = 0;
                CurrentHeadDir = new Vector2(-1, 0);
                CurrentTailDir = new Vector2(1, 0);
                LayoutManager.SetLayout();
                Turn = GameManager.SwitchTurn();
                return;
            }

            Tile head = ActiveTiles.First!.Value;

            float distHead = Vector2.Distance(dropPosition, head.Position);
            if (distHead < Constants.SnapThreshold)
            {
                if (Rules.CanConnect(
                        newTile: tile,
                        anchor: head,
                        isHead: true,
                        mustFlip: out bool mustFlip))
                {
                    Snapper.SnapTo(
                        newTile: tile,
                        anchor: head,
                        isHead: true,
                        mustFlip: mustFlip);
                    tile.LastPosition = tile.Position;
                    tile.Owner = Tile.TileOwner.Board;
                    ActiveTiles.AddFirst(tile);
                    LayoutManager.SetLayout();
                    if (!Rules.GameStatus())
                    {
                        Turn = GameManager.SwitchTurn();
                    }

                    return;
                }
            }

            Tile tail = ActiveTiles.Last!.Value;

            float distTail = Vector2.Distance(dropPosition, tail.Position);
            if (distTail < Constants.SnapThreshold)
            {
                if (Rules.CanConnect(
                        newTile: tile,
                        anchor: tail,
                        isHead: false,
                        mustFlip: out bool mustFlip))
                {
                    Snapper.SnapTo(
                        newTile: tile,
                        anchor: tail,
                        isHead: false,
                        mustFlip: mustFlip);
                    tile.LastPosition = tile.Position;
                    tile.Owner = Tile.TileOwner.Board;
                    ActiveTiles.AddLast(tile);
                    LayoutManager.SetLayout();
                    Turn = GameManager.SwitchTurn();
                    return;
                }
            }
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

            if (Ghost.ShowGhost && Ghost.GhostTile != null)
            {
                Vector2 ghostOrigin = new Vector2(
                    x: Ghost.GhostTile.SourceRectangle.Width / 2f,
                    y: Ghost.GhostTile.SourceRectangle.Height / 2f);

                spriteBatch.Draw(
                    texture: Atlas,
                    position: Ghost.GhostPosition,
                    sourceRectangle: Ghost.GhostTile.SourceRectangle,
                    color: Color.White * 0.15f,
                    rotation: Ghost.GhostRotation,
                    origin: ghostOrigin,
                    scale: Ghost.GhostTile.Scale,
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