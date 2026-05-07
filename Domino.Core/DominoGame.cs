using System;
using Domino.Core.Objects;
using Domino.Core.Systems;
using Domino.Core.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Domino.Core
{
    public class DominoGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Matrix GlobalTransform { get; private set; }

        public Random Random { get; } = new();

        private Background _background;
        public Table Table { get; set; }
        public Texture2D Atlas { get; set; }
        public Texture2D BackTile { get; set; }
        private Tile _selectedTile;

        public InputManager Input { get; set; }
        public Interface Interface { get; set; }
        public Texture2D Button { get; set; }
        public Machine Machine { get; set; }

        public static readonly bool IsMobile = OperatingSystem.IsAndroid() ||
                                               OperatingSystem.IsIOS();

        public static readonly bool IsDesktop = OperatingSystem.IsMacOS() ||
                                                OperatingSystem.IsLinux() ||
                                                OperatingSystem.IsWindows();

        public DominoGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Services.AddService(_graphics);
            _graphics.PreferredBackBufferWidth = Constants.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Constants.ScreenHeight;
            _graphics.ApplyChanges();
            IsMouseVisible = true;

            Content.RootDirectory = "Content";

            _graphics.SupportedOrientations =
                DisplayOrientation.LandscapeLeft |
                DisplayOrientation.LandscapeRight;
            
            _graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = false;
            Window.ClientSizeChanged += (s, e) => UpdateResolution();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Atlas = Content.Load<Texture2D>("Sprites/dominoes");
            BackTile = Content.Load<Texture2D>("Sprites/back_domino");
            Button = Content.Load<Texture2D>("Interface/button");

            SoundEffect snap = Content.Load<SoundEffect>("Sounds/click");
            SpriteFont font = Content.Load<SpriteFont>
                ("Fonts/Monocraft-Semibold");

            Table = new Table(
                atlas: Atlas,
                backTile: BackTile,
                font: font,
                snap: snap);

            Machine = new Machine(table: Table);
            Input = new InputManager(table: Table);
            Interface = new Interface(table: Table, buttonTexture: Button);
            _background = new Background(GraphicsDevice, Content);

            _selectedTile = null;
            base.LoadContent();
            UpdateResolution();
        }

        protected override void Update(GameTime gameTime)
        {
            var currentMouseState = Mouse.GetState();
            _background.Update(gameTime);

            bool wasMousePressed = Input.IsMousePressed;
            Table.Update(gameTime);
            Input.Update(
                game: this,
                selectedTile: ref _selectedTile,
                gameTime: gameTime);

            Machine.Update(gameTime);

            Interface.Update(
                gameTime: gameTime,
                mousePosition: Input.MousePosition,
                mouseJustClicked: wasMousePressed);
            Input.PreviousMouseState = currentMouseState;
            base.Update(gameTime);
        }

        private void UpdateResolution()
        {
            const float targetAspectRatio = 
                (float)Constants.ScreenWidth / Constants.ScreenHeight;
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;
            float windowAspectRatio = (float)width / height;

            float scale;
            if (windowAspectRatio > targetAspectRatio)
            {
                scale = (float)height / Constants.ScreenHeight;
            }
            else
            {
                scale = (float)width / Constants.ScreenWidth;
            }

            GlobalTransform = Matrix.CreateScale(scale, scale, 1.0f);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _background.Draw(
                spriteBatch: _spriteBatch,
                screenWidth: Constants.ScreenWidth,
                screenHeight: Constants.ScreenHeight);

            _spriteBatch.Begin(
                transformMatrix: GlobalTransform,
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: DepthStencilState.None,
                rasterizerState: RasterizerState.CullCounterClockwise
            );
            Table.Draw(
                spriteBatch: _spriteBatch,
                selectedTile: _selectedTile);
            _spriteBatch.End();

            Interface.Draw(spriteBatch: _spriteBatch);
            base.Draw(gameTime);
        }
    }
}