using System;
using Domino.Core.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Domino.Core
{
    public class DominoGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        public Table Table { get; set; }
        public Texture2D Atlas { get; set; }        
        private Tile _selectedTile;
        
        public Random Random { get; } = new();
        public int Score { get; set; }
        public bool IsGameOver { get; set; }

        public static readonly bool IsMobile =
            OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

        public static readonly bool IsDesktop = OperatingSystem.IsMacOS() ||
                                                OperatingSystem.IsLinux() ||
                                                OperatingSystem.IsWindows();

        public DominoGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Services.AddService(_graphics);
            IsMouseVisible = true;
            
            Content.RootDirectory = "Content";

            _graphics.SupportedOrientations =
                DisplayOrientation.LandscapeLeft |
                DisplayOrientation.LandscapeRight;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Atlas = Content.Load<Texture2D>("Sprites/dominoes");
            Table = new Table(Atlas);
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (_selectedTile == null)
                {
                    for (int i = Table.Tiles.Count - 1; i >= 0; i--)
                    {
                        if (Table.Tiles[i].Bounds.Contains(mousePosition))
                        {
                            _selectedTile = Table.Tiles[i];
                            break;
                        }
                    }
                }
                else
                {
                    Vector2 targetPos = mousePosition - new Vector2(
                        _selectedTile.SourceRectangle.Width / 2f, 
                        _selectedTile.SourceRectangle.Height / 2f);
                    
                    _selectedTile.Position = Vector2.Lerp(
                        _selectedTile.Position, targetPos, 0.4f);
                    
                    Vector2 delta = _selectedTile.Position - 
                                    _selectedTile.LastPosition;
                    
                    float targetRotation = delta.X * 0.04f;
                    _selectedTile.Rotation = MathHelper.Lerp(
                        _selectedTile.Rotation, 
                        targetRotation, 0.5f);
                    
                    _selectedTile.LastPosition = _selectedTile.Position;
                }
            }
            else
            {
                _selectedTile = null;
                foreach(var t in Table.Tiles) {
                    t.Rotation = MathHelper.Lerp(t.Rotation, 0, 0.1f);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MonoGameOrange);
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: DepthStencilState.None,
                rasterizerState: RasterizerState.CullCounterClockwise
            );
            Table.Draw(spriteBatch: _spriteBatch);   
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}