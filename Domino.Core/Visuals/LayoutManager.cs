using System.Linq;
using Domino.Core.Objects;
using Domino.Core.Systems;
using Microsoft.Xna.Framework;

namespace Domino.Core.Visuals
{
    public class LayoutManager
    {
        public Table Table { get; }

        public LayoutManager(Table table)
        {
            Table = table;
        }

        public void SetLayout()
        {
            if (Table.Atlas == null) return;

            float scaledWidth =
                (Table.Atlas.Width / Constants.Cols) * Tile.MaxScale;
            float scaledHeight =
                (Table.Atlas.Height / Constants.Rows) * Tile.MaxScale;
            Vector2 centerOffset =
                new Vector2(scaledWidth / 2f, scaledHeight / 2f);

            var playerTiles = Table.Tiles
                .Where(t => t.Owner == Tile.TileOwner.Player)
                .ToList();
            float playerHandWidth =
                (playerTiles.Count * (scaledWidth +
                                      Constants.Spacing)) - Constants.Spacing;
            float playerStartX =
                (Constants.ScreenWidth - playerHandWidth) / 2f;

            for (int i = 0; i < playerTiles.Count; i++)
            {
                playerTiles[i].LastPosition = new Vector2(
                                                  playerStartX +
                                                  i * (scaledWidth +
                                                      Constants.Spacing),
                                                  Constants.ScreenHeight -
                                                  scaledHeight -
                                                  Constants.BottomMargin)
                                              + centerOffset;
            }

            var aiTiles = Table.Tiles
                .Where(t => t.Owner == Tile.TileOwner.Machine)
                .ToList();

            float aiHandWidth = (aiTiles.Count *
                                 (scaledWidth + Constants.Spacing)) -
                                Constants.Spacing;
            float aiStartX = (Constants.ScreenWidth - aiHandWidth) / 2f;

            for (int i = 0; i < aiTiles.Count; i++)
            {
                aiTiles[i].LastPosition = new Vector2(
                    aiStartX + i * (scaledWidth + Constants.Spacing),
                    -scaledHeight - Constants.OponentHandOffset
                ) + centerOffset;
            }

            var boneyardTiles = Table.Tiles
                .Where(t => t.Owner == Tile.TileOwner.Boneyard).ToList();
            float boneyardCenterY = 100 - (scaledHeight / 2f);

            for (int i = 0; i < boneyardTiles.Count; i++)
            {
                boneyardTiles[i].LastPosition = new Vector2(
                    Constants.BoneyardX + (i * Constants.StackOffset),
                    boneyardCenterY + (i * Constants.StackOffset)
                ) + centerOffset;
            }
        }
    }
}