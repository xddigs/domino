using Domino.Core.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Domino.Core.Systems
{
    public class InputManager
    {
        public Table Table { get; }

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
                        if (Table.Tiles[i].Bounds.Contains(mousePosition))
                        {
                            selectedTile = Table.Tiles[i];
                            selectedTile.LastPosition = selectedTile.Position;
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
    
                    selectedTile.Scale = MathHelper.Lerp(
                        selectedTile.Scale, 1.5f,
                        scaleSpeed);

                    Vector2 targetPos = mousePosition; 

                    selectedTile.Position = Vector2.Lerp(
                        selectedTile.Position,
                        targetPos, lerpMouse);

                    Vector2 delta = selectedTile.Position -
                                    selectedTile.LastPosition;

                    float targetRotation = delta.X * rotationSpeed;

                    selectedTile.Rotation = MathHelper.Lerp(
                        selectedTile.Rotation,
                        targetRotation, lerpSpeed);
                    
                    selectedTile.LastPosition = selectedTile.Position;
                }
            }
            else
            {
                const float lerpSpeed = 0.15f;
                if (selectedTile != null)
                {
                    Table.TryPlaceTile(selectedTile);
                    selectedTile = null;
                }
                
                foreach (Tile t in Table.Tiles)
                {
                    if (Table.ActiveTiles.Contains(t)) 
                    {
                        t.Scale = MathHelper.Lerp(t.Scale, 1.0f, lerpSpeed);
                        continue; 
                    }

                    t.Rotation = MathHelper.Lerp(
                        t.Rotation, 
                        0, lerpSpeed);
                    
                    t.Scale = MathHelper.Lerp(
                        t.Scale, 
                        1.0f, 
                        lerpSpeed);
                    
                    t.Position = Vector2.Lerp(
                        t.Position, 
                        t.LastPosition, 
                        lerpSpeed);
                }
            }
        }
    }
}