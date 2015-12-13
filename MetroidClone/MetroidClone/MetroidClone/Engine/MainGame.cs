using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MetroidClone.Engine
{
    class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        AssetManager assetManager;
        SpriteBatch spriteBatch;

        public static Profiler Profiler;

        private DrawWrapper drawWrapper;

        private World world;

        static void Main()
        {
            MainGame game = new MainGame();
            game.Run();
        }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            assetManager = new AssetManager(Content);

            Profiler = new Profiler();

            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
        }

        protected override void Initialize()
        {
            world = new World();
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferredBackBufferWidth = 24 * 20 * 2;
            graphics.PreferredBackBufferHeight = 24 * 15 * 2;

            graphics.ApplyChanges();

            IsFixedTimeStep = true;
            
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / 60f);

            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            drawWrapper = new DrawWrapper(spriteBatch, GraphicsDevice, assetManager);

            world.DrawWrapper = drawWrapper;
            world.AssetManager = assetManager;

            world.Initialize();
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            var inputHelper = InputHelper.Instance;

            Profiler.LogEventEnd("FinalizeStep");

            if (inputHelper.KeyboardCheckReleased(Keys.F12))
            {
                Profiler.ShowOutput();
            }

            Profiler.LogGameStepStart();

            Profiler.LogEventStart("Update");
            inputHelper.Update();

            if (inputHelper.KeyboardCheckPressed(Keys.Escape))
                Exit();

            world.Update(gameTime);
            base.Update(gameTime);
            Profiler.LogEventEnd("Update");
        }

        protected override void Draw(GameTime gameTime)
        {
            Profiler.LogEventStart("Draw");
            GraphicsDevice.Clear(Color.White);

            world.Draw();

            drawWrapper.EndOfDraw();

            base.Draw(gameTime);
            Profiler.LogEventEnd("Draw");

            Profiler.LogEventStart("FinalizeStep");
        }
    }
}
