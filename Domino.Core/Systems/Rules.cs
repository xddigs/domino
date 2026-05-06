using System.Linq;
using Domino.Core.Objects;

namespace Domino.Core.Systems
{
    public class Rules
    {
        public Table Table { get; }

        public Rules(Table table)
        {
            Table = table;
        }

        public void FirstTurn()
        {
            if (Table.ActiveTiles.Count > 0) return;

            var playerTiles = Table.Tiles
                .Where(t => t.Owner == Tile.TileOwner.Player)
                .ToList();
            var aiTiles = Table.Tiles
                .Where(t => t.Owner == Tile.TileOwner.Machine)
                .ToList();

            if (!playerTiles.Any() || aiTiles.Count == 0) return;

            var playerDoubles = playerTiles
                .Where(t => t.UpperValue == t.LowerValue).ToList();
            var aiDoubles = aiTiles.Where(t => t.UpperValue == t.LowerValue)
                .ToList();

            int pMax = playerDoubles.Any()
                ? playerDoubles.Max(t => t.UpperValue)
                : -1;
            int aMax = aiDoubles.Count != 0
                ? aiDoubles.Max(t => t.UpperValue)
                : -1;

            if (pMax != -1 || aMax != -1)
            {
                if (pMax > aMax)
                {
                    Table.Turn = Turn.Player;
                    Table.StartingTile =
                        playerDoubles.First(t => t.UpperValue == pMax);
                }
                else
                {
                    Table.Turn = Turn.Machine;
                    Table.StartingTile =
                        aiDoubles.First(t => t.UpperValue == aMax);
                }
            }
            else
            {
                var pBest = playerTiles
                    .OrderByDescending(t => t.UpperValue + t.LowerValue)
                    .FirstOrDefault();

                var aBest = aiTiles
                    .OrderByDescending(t => t.UpperValue + t.LowerValue)
                    .FirstOrDefault();

                if (pBest != null && aBest != null)
                {
                    if ((pBest.UpperValue + pBest.LowerValue)
                        >= (aBest.UpperValue + aBest.LowerValue))
                    {
                        Table.Turn = Turn.Player;
                        Table.StartingTile = pBest;
                    }
                    else
                    {
                        Table.Turn = Turn.Machine;
                        Table.StartingTile = aBest;
                    }
                }
            }
        }

        public bool CanConnect(
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

        public bool IsGameBlocked()
        {
            if (Table.Tiles.Any(t => t.Owner == Tile.TileOwner.Boneyard))
                return false;

            int head = Table.ActiveTiles.First!.Value.HeadValue!.Value;
            int tail = Table.ActiveTiles.Last!.Value.TailValue!.Value;

            bool playerCanMove = Table.Tiles
                .Where(t => t.Owner == Tile.TileOwner.Player)
                .Any(t => t.UpperValue == head || t.LowerValue == head ||
                          t.UpperValue == tail || t.LowerValue == tail);

            bool aiCanMove = Table.Tiles
                .Where(t => t.Owner == Tile.TileOwner.Machine)
                .Any(t => t.UpperValue == head || t.LowerValue == head ||
                          t.UpperValue == tail || t.LowerValue == tail);

            return !playerCanMove && !aiCanMove;
        }

        public bool CheckWin(Tile.TileOwner owner)
        {
            return Table.Tiles.All(t => t.Owner != owner);
        }
        
        public bool GameStatus()
        {
            if (CheckWin(Tile.TileOwner.Player) ||
                CheckWin(Tile.TileOwner.Machine) ||
                IsGameBlocked())
            {
                Table.IsGameOver = true;
                return true;
            }

            return false;
        }
    }
}