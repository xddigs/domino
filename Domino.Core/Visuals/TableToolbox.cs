using System;
using Microsoft.Xna.Framework;

namespace Domino.Core.Visuals
{
    public static class TableToolbox
    {
        public static float GetExtentInDirection(
            float rotation,
            Vector2 dir,
            float width,
            float height)
        {
            Vector2 right = new Vector2((float)Math.Cos(rotation),
                (float)Math.Sin(rotation));
            Vector2 up = new Vector2(-right.Y, right.X);

            float projRight = Math.Abs(Vector2.Dot(
                right, dir)) * (width / 2f);
            float projUp = Math.Abs(
                Vector2.Dot(up, dir)) * (height / 2f);

            return projRight + projUp;
        }
    }
}