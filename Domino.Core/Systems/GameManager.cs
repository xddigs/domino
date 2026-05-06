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
    }
}