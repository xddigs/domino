using System;
using System.Linq;
using Domino.Core.Objects;
using Microsoft.Xna.Framework;

namespace Domino.Core.Systems
{
    public class GameManager
    {
        private static readonly Random Rng = new();
        public Table Table { get; }

        public GameManager(Table table)
        {
            Table = table;
        }

        public void Populate()
        {
            int tileWidth = Table.Atlas.Width / Constants.Cols;
            int tileHeight = Table.Atlas.Height / Constants.Rows;

            int count = 0;
            for (int i = 0; i <= 6; i++)
            {
                for (int j = i; j <= 6; j++)
                {
                    int column = count % Constants.Cols;
                    int row = count / Constants.Cols;

                    Rectangle sourceRect = new Rectangle(
                        column * tileWidth,
                        row * tileHeight,
                        tileWidth,
                        tileHeight
                    );

                    Table.Tiles.Add(new Tile(i, j, sourceRect));
                    count++;
                }
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

        public void Rob(Tile.TileOwner newOwner)
        {
            var tileToRob = Table.Tiles.LastOrDefault(t => t.Owner == Tile
                .TileOwner.Boneyard);

            if (tileToRob != null)
            {
                Vector2 inBoneyard = tileToRob.Position;
                tileToRob.Owner = newOwner;
                Table.LayoutManager.SetLayout();
                tileToRob.Position = inBoneyard;
                if (Table.ActiveTiles.Count == 0) Table.Rules.FirstTurn();
            }
        }

        public void Shuffle()
        {
            var shuffled = Table.Tiles.OrderBy(a => Rng.Next()).ToList();
            Table.Tiles.Clear();
            Table.Tiles.AddRange(shuffled);
            Table.LayoutManager.SetLayout();
        }

        public Turn SwitchTurn()
        {
            return Table.Turn == Turn.Machine
                ? Turn.Player
                : Turn.Machine;
        }
        
        public void TryPlaceTile(Tile tile, Vector2 dropPosition)
        {
            if (Table.ActiveTiles.Contains(tile)) return;
            if (Table.IsGameOver) return;

            if (Table.ActiveTiles.Count == 0)
            {
                if (tile != Table.StartingTile) return;
                tile.Position = new Vector2(
                    x: Constants.ScreenWidth / 2f,
                    y: Constants.ScreenHeight / 2f);
                tile.Rotation = 0f;
                tile.LastPosition = tile.Position;
                tile.HeadValue = tile.UpperValue;
                tile.TailValue = tile.LowerValue;
                tile.Owner = Tile.TileOwner.Board;
                Table.ActiveTiles.AddFirst(tile);
                Table.HeadRowCount = 0;
                Table.TailRowCount = 0;
                Table.CurrentHeadDir = new Vector2(-1, 0);
                Table.CurrentTailDir = new Vector2(1, 0);
                Table.LayoutManager.SetLayout();
                Table.Turn = SwitchTurn();
                return;
            }

            Tile head = Table.ActiveTiles.First!.Value;

            float distHead = Vector2.Distance(dropPosition, head.Position);
            if (distHead < Constants.SnapThreshold)
            {
                if (Table.Rules.CanConnect(
                        newTile: tile,
                        anchor: head,
                        isHead: true,
                        mustFlip: out bool mustFlip))
                {
                    Table.Snapper.SnapTo(
                        newTile: tile,
                        anchor: head,
                        isHead: true,
                        mustFlip: mustFlip);
                    tile.LastPosition = tile.Position;
                    tile.Owner = Tile.TileOwner.Board;
                    Table.ActiveTiles.AddFirst(tile);
                    Table.LayoutManager.SetLayout();
                    if (!Table.Rules.GameStatus())
                    {
                        Table.Turn = SwitchTurn();
                    }

                    return;
                }
            }

            Tile tail = Table.ActiveTiles.Last!.Value;

            float distTail = Vector2.Distance(dropPosition, tail.Position);
            if (distTail < Constants.SnapThreshold)
            {
                if (Table.Rules.CanConnect(
                        newTile: tile,
                        anchor: tail,
                        isHead: false,
                        mustFlip: out bool mustFlip))
                {
                    Table.Snapper.SnapTo(
                        newTile: tile,
                        anchor: tail,
                        isHead: false,
                        mustFlip: mustFlip);
                    tile.LastPosition = tile.Position;
                    tile.Owner = Tile.TileOwner.Board;
                    Table.ActiveTiles.AddLast(tile);
                    Table.LayoutManager.SetLayout();
                    Table.Turn = SwitchTurn();
                    return;
                }
            }
        }
    }
}