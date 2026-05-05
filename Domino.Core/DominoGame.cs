using System;
using Domino.Core.Objects;
using Domino.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Domino.Core
{
    public class DominoGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        public Table Table { get; set; }
        public Texture2D Atlas { get; set; }        
        private Tile _selectedTile;
        
        public InputManager Input { get; set; }
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
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
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
            Input = new InputManager(table: Table);
            _selectedTile = null;
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update(
                game: this,
                selectedTile: ref _selectedTile,
                gameTime: gameTime);
            
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