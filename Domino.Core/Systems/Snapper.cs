using System;
using Domino.Core.Objects;
using Domino.Core.Visuals;
using Microsoft.Xna.Framework;

namespace Domino.Core.Systems
{
    public class Snapper
    {
        public Table Table { get; }

        public Snapper(Table table)
        {
            Table = table;
        }

        public void SnapTo(
            Tile newTile,
            Tile anchor,
            bool isHead,
            bool mustFlip,
            bool isPreview = false)
        {
            int baseWidth = Table.Atlas.Width / Constants.Cols;
            int baseHeight = Table.Atlas.Height / Constants.Rows;
            float scaledWidth = baseWidth * Tile.MaxScale;
            float scaledHeight = baseHeight * Tile.MaxScale;

            Vector2 outDir =
                isHead ? Table.CurrentHeadDir : Table.CurrentTailDir;
            int currentCount =
                isHead ? Table.HeadRowCount : Table.TailRowCount;

            bool isDouble = newTile.UpperValue == newTile.LowerValue;
            bool isTurning = currentCount >= Constants.MaxTilesPerRow;

            Vector2 moveDir = isTurning ? new Vector2(0, 1) : outDir;

            bool anchorHorizontal = Math.Abs(
                anchor.Rotation % MathHelper.Pi) < 0.1f;

            bool newHorizontal = !(isTurning || isDouble);

            float anchorExtent = TableToolbox.GetExtentInDirection(
                rotation: anchor.Rotation,
                dir: moveDir,
                width: scaledWidth,
                height: scaledHeight);
            float newExtent = TableToolbox.GetExtentInDirection(
                rotation: (isTurning || isDouble)
                    ? 0f
                    : (mustFlip
                        ? (float)Math.Atan2(outDir.Y, outDir.X) -
                        MathHelper.PiOver2 + MathHelper.Pi
                        : (float)Math.Atan2(outDir.Y, outDir.X) -
                          MathHelper.PiOver2),
                dir: moveDir,
                width: scaledWidth,
                height: scaledHeight
            );

            float distance = anchorExtent + newExtent + Constants.TilePadding;

            newTile.Position = anchor.Position + moveDir * distance;
            if (isTurning || isDouble)
            {
                newTile.Rotation = 0f;
            }
            else
            {
                float baseRot = (float)Math.Atan2(outDir.Y, outDir.X) -
                                MathHelper.PiOver2;
                newTile.Rotation =
                    mustFlip ? baseRot + MathHelper.Pi : baseRot;
            }

            if (!isPreview)
            {
                if (isTurning)
                {
                    if (isHead) {
                        Table.HeadRowCount = 0;
                        Table.CurrentHeadDir = Table.CurrentHeadDir with
                        {
                            X = Table.CurrentHeadDir.X * -1
                        };
                    }
                    else
                    {
                        Table.TailRowCount = 0;
                        Table.CurrentTailDir = Table.CurrentTailDir with
                        {
                            X = Table.CurrentTailDir.X * -1
                        };
                    }
                }
                else
                {
                    if (isHead) Table.HeadRowCount++;
                    else Table.TailRowCount++;
                }
            }

            newTile.LastPosition = newTile.Position;
            int connVal = isHead
                ? anchor.HeadValue!.Value
                : anchor.TailValue!.Value;
            if (isHead)
            {
                newTile.HeadValue = (newTile.UpperValue == connVal)
                    ? newTile.LowerValue
                    : newTile.UpperValue;
                newTile.TailValue = connVal;
            }
            else
            {
                newTile.TailValue = (newTile.UpperValue == connVal)
                    ? newTile.LowerValue
                    : newTile.UpperValue;
                newTile.HeadValue = connVal;
            }
        }
    }
}