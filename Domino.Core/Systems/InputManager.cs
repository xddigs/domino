using Domino.Core.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Domino.Core.Systems
{
    public class InputManager
    {
        public Table Table { get; }
        private Vector2 _lastMousePosition;

        public InputManager(Table table)
        {
            Table = table;
        }

        public void Update(
            DominoGame game,
            ref Tile selectedTile,
            GameTime gameTime)
        {
            var currentMouseState = Mouse.GetState();
            var currentKeyboardState = Keyboard.GetState();

            Vector2 mousePosition = new Vector2(
                currentMouseState.X, currentMouseState.Y);

            bool isLeftPressed = currentMouseState.LeftButton ==
                                 ButtonState.Pressed;
            bool isEscapePressed = currentKeyboardState.IsKeyDown(Keys.Escape);

            if (isEscapePressed) game.Exit();
            if (isLeftPressed)
            {
                if (selectedTile == null)
                {
                    for (int i = Table.Tiles.Count - 1; i >= 0; i--)
                    {
                        Tile currentTile = Table.Tiles[i];
                        if (!Table.ActiveTiles.Contains(currentTile) 
                            && currentTile.Bounds.Contains(mousePosition))
                        {
                            selectedTile = currentTile;
                            break;
                        }
                    }
                }
                else
                {
                    const float rotationSpeed = 0.06f;
                    const float scaleSpeed = 0.2f;
                    const float lerpSpeed = 0.4f;
                    const float lerpMouse = 0.6f;
                    
                    Vector2 mouseDelta = mousePosition - _lastMousePosition;
                    selectedTile.Scale = MathHelper.Lerp(
                        selectedTile.Scale, 
                        Tile.MaxScale * 1.3f,
                        scaleSpeed);

                    selectedTile.Position = Vector2.Lerp(
                        selectedTile.Position,
                        mousePosition, lerpMouse);

                    const float rotationIntensity = 0.02f; 
                    float targetRotation = mouseDelta.X * rotationIntensity;

                    selectedTile.Rotation = MathHelper.Lerp(
                        selectedTile.Rotation,
                        targetRotation, lerpSpeed);

                    Table.UpdateGhost(
                        draggingTile: selectedTile,
                        mousePosition: mousePosition);
                }
            }
            else
            {
                const float lerpSpeed = 0.15f;
                Table.UpdateGhost(null, Vector2.Zero);
                Tile releasedTile = selectedTile;
                if (selectedTile != null)
                {
                    Vector2 releaseMousePosition = mousePosition;
                    Table.TryPlaceTile(selectedTile, releaseMousePosition);
                    selectedTile = null;
                }

                foreach (Tile t in Table.Tiles)
                {
                    if (Table.ActiveTiles.Contains(t))
                    {
                        t.Scale = MathHelper.Lerp(
                            t.Scale, 
                            Tile.MaxScale,
                            lerpSpeed);
                        continue;
                    }

                    t.Rotation = MathHelper.Lerp(
                        t.Rotation,
                        0, lerpSpeed);

                    t.Scale = MathHelper.Lerp(
                        t.Scale,
                        Tile.MaxScale,
                        lerpSpeed);

                    t.Position = Vector2.Lerp(
                        t.Position,
                        t.LastPosition,
                        lerpSpeed);
                }
            }
            _lastMousePosition = mousePosition;
        }
    }
}