using Domino.Core.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Domino.Core.Systems
{
    public class InputManager
    {
        public Table Table { get; }
        public Vector2 LastMousePosition { get; set; }
        public Vector2 MousePosition { get; set; }
        public MouseState PreviousMouseState { get; set; }

        public bool IsMousePressed =>
            Mouse.GetState().LeftButton == ButtonState.Pressed &&
            PreviousMouseState.LeftButton == ButtonState.Released;

        public InputManager(Table table)
        {
            Table = table;
        }

        public void Update(DominoGame game, ref Tile selectedTile,
            GameTime gameTime)
        {
            var mState = Mouse.GetState();
            var kState = Keyboard.GetState();
            MousePosition = new Vector2(mState.X, 
                mState.Y);

            bool isLeftPressed = mState.LeftButton == ButtonState.Pressed;
            if (kState.IsKeyDown(Keys.Escape)) game.Exit();

            if (isLeftPressed)
            {
                if (selectedTile == null)
                {
                    for (int i = Table.Tiles.Count - 1; i >= 0; i--)
                    {
                        Tile currentTile = Table.Tiles[i];
                        if (!Table.ActiveTiles.Contains(currentTile)
                            && currentTile.Bounds.Contains(MousePosition)
                            && currentTile.Owner == Tile.TileOwner.Player)
                        {
                            selectedTile = currentTile;
                            selectedTile.Scale = Tile.MaxScale * 0.8f;
                            break;
                        }
                    }
                }
                else
                {
                    const float lerpMouse = 0.6f;
                    const float rotationIntensity = 0.02f;
                    const float lerpSpeed = 0.4f;

                    Vector2 mouseDelta = MousePosition - LastMousePosition;
                    selectedTile.Position = Vector2.Lerp(selectedTile.Position,
                        MousePosition, lerpMouse);
                    
                    float targetRotation = mouseDelta.X * rotationIntensity;
                    selectedTile.Rotation = MathHelper.Lerp(
                        selectedTile.Rotation, targetRotation, lerpSpeed);

                    Table.UpdateGhost(selectedTile, MousePosition);
                }
            }
            else
            {
                Table.UpdateGhost(null, Vector2.Zero);
                if (selectedTile != null)
                {
                    Table.TryPlaceTile(selectedTile, MousePosition);
                    selectedTile = null;
                }
            }

            const float lerpScaleSpeed = 0.15f;

            foreach (Tile t in Table.Tiles)
            {
                float targetScale = Tile.MaxScale;
                if (t == selectedTile)
                {
                    targetScale = Tile.MaxScale * 1.3f;
                }
                else if (selectedTile == null && !Table.ActiveTiles.Contains(t) 
                && t.Bounds.Contains(MousePosition))
                {
                    targetScale = Tile.MaxScale * 1.15f;
                }

                t.Scale = MathHelper.Lerp(t.Scale, targetScale, lerpScaleSpeed);
                if (!Table.ActiveTiles.Contains(t) && t != selectedTile)
                {
                    float lerpVelocity = 0.15f;

                    if (t.Owner is Tile.TileOwner.Player or Tile.TileOwner.Ai)
                    {
                        lerpVelocity = 0.25f;
                    }

                    t.Position = Vector2.Lerp(t.Position, t.LastPosition, 
                        lerpVelocity);
                    t.Rotation = MathHelper.Lerp(t.Rotation, 0, 0.15f);

                    if (Vector2.Distance(t.Position, t.LastPosition) < 0.1f) 
                        t.Position = t.LastPosition;
                }
            }

            LastMousePosition = MousePosition;
            PreviousMouseState = mState;
        }
    }
}