using Domino.Core.Objects;
using Microsoft.Xna.Framework;

namespace Domino.Core.Systems
{
    public class Ghost
    {
        public Table Table { get; }
        public Tile GhostTile { get; private set; }
        public Vector2 GhostPosition { get; private set; }
        public float GhostRotation { get; private set; }
        public bool ShowGhost { get; private set; }

        public Ghost(Table table)
        {
            Table = table;
        }

        public void UpdateGhost(Tile draggingTile, Vector2 mousePosition)
        {
            ShowGhost = false;
            if (Table.ActiveTiles.Count == 0) return;

            Tile head = Table.ActiveTiles.First!.Value;
            Tile tail = Table.ActiveTiles.Last!.Value;

            float distHead = Vector2.Distance(mousePosition, head.Position);
            float distTail = Vector2.Distance(mousePosition, tail.Position);

            Tile anchor = null;
            bool isHead = false;

            if (distHead < Constants.SnapThreshold)
            {
                anchor = head;
                isHead = true;
            }
            else if (distTail < Constants.SnapThreshold)
            {
                anchor = tail;
            }

            if (anchor != null && Table.Rules.CanConnect(draggingTile, anchor, 
                isHead,
                    out bool mustFlip))
            {
                GhostTile = draggingTile;

                Vector2 originalPos = draggingTile.Position;
                float originalRot = draggingTile.Rotation;
                Vector2 originalLastPos = draggingTile.LastPosition;

                Table.Snapper.SnapTo(
                    newTile: draggingTile,
                    anchor: anchor,
                    isHead: isHead,
                    mustFlip: mustFlip,
                    isPreview: true);

                GhostPosition = draggingTile.Position;
                GhostRotation = draggingTile.Rotation;

                draggingTile.Position = originalPos;
                draggingTile.Rotation = originalRot;
                draggingTile.LastPosition = originalLastPos;

                ShowGhost = true;
            }
        }
    }
}

