using FloppyWieners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FloppyWieners
{
    class MainGame : Game
    {
        World world;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        static void Main()
        {
            MainGame game = new MainGame();
            game.Run();
        }
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / 60f);

            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            world = new World();
            world.Drawing = new DrawWrapper(spriteBatch, GraphicsDevice);

            world.Create();
        }

        protected override void Update(GameTime gameTime)
        {
            world.Update();
            base.Update(gameTime);

            if (InputHelper.Instance.KeyboardCheckDown(Keys.Escape))
                Exit();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            world.Draw();
            base.Draw(gameTime);
        }
    }
}

