using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Domino.Core
{
    public class DominoGame : Game
    {
        // Resources for drawing.
        private readonly GraphicsDeviceManager _graphics;

        public static readonly bool IsMobile =
            OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

        public static readonly bool IsDesktop = OperatingSystem.IsMacOS() ||
                                                OperatingSystem.IsLinux() ||
                                                OperatingSystem.IsWindows();

        public DominoGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Services.AddService(_graphics);

            Content.RootDirectory = "Content";

            _graphics.SupportedOrientations =
                DisplayOrientation.LandscapeLeft |
                DisplayOrientation.LandscapeRight;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MonoGameOrange);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}