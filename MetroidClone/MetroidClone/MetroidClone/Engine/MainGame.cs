using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MetroidClone.Metroid;

namespace MetroidClone.Engine
{
    class MainGame : Game
    {
        public GraphicsDeviceManager Graphics;
        AssetManager assetManager;
        SpriteBatch spriteBatch;

        public static Profiler Profiler;

        private DrawWrapper drawWrapper;
        private AudioWrapper audioWrapper;

        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            Options,
            GameOver
        }
        GameState currentState, previousState;
        MainMenu mainMenu;
        PauseMenu pauseMenu;
        OptionsMenu optionsMenu;
        GameOverMenu gameOverMenu;

        private World world;

        static void Main()
        {
            MainGame game = new MainGame();
            game.Run();
        }

        public MainGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            assetManager = new AssetManager(Content);

            Profiler = new Profiler();

            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            drawWrapper = new DrawWrapper(spriteBatch, GraphicsDevice, assetManager);
            audioWrapper = new AudioWrapper(assetManager);

            world = new World();
            mainMenu = new MainMenu(drawWrapper);

            Graphics.PreferMultiSampling = true;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.PreferredBackBufferWidth = 24 * 20 * 2;
            Graphics.PreferredBackBufferHeight = 24 * 15 * 2;

            Graphics.ApplyChanges();
           
            IsFixedTimeStep = true;
            
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / 60f);

            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            world.DrawWrapper = drawWrapper;
            world.AudioWrapper = audioWrapper;
            world.AssetManager = assetManager;
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
                Profiler.ShowOutput();

            Profiler.LogGameStepStart();

            Profiler.LogEventStart("Update");
            inputHelper.Update();

            //updating menus or world
            switch (currentState)
            {
                case GameState.MainMenu:
                    mainMenu.Update(gameTime, inputHelper);
                    if (mainMenu.ExitGame)
                        Exit();
                    else if (mainMenu.Options)
                    {
                        previousState = currentState;
                        currentState = GameState.Options;
                        optionsMenu = new OptionsMenu(drawWrapper, Graphics, audioWrapper, inputHelper);
                        mainMenu = null;
                    }
                    else if (mainMenu.Start)
                    {
                        world.Initialize();
                        currentState = GameState.Playing;
                        mainMenu = null;
                    }
                    break;
                case GameState.Playing:
                    world.Update(gameTime);
                    if (inputHelper.KeyboardCheckPressed(Keys.Escape) || inputHelper.GamePadCheckPressed(Buttons.Start))
                    {
                        currentState = GameState.Paused;
                        pauseMenu = new PauseMenu(drawWrapper);
                    }
                    if (world.Player.Dead)
                    {
                        currentState = GameState.GameOver;
                        gameOverMenu = new GameOverMenu(drawWrapper);
                    }
                    break;
                case GameState.Paused:
                    pauseMenu.Update(gameTime, inputHelper);
                    if (pauseMenu.Resume)
                    {
                        currentState = GameState.Playing;
                    }
                    else if (pauseMenu.Options)
                    {
                        previousState = currentState;
                        currentState = GameState.Options;
                        optionsMenu = new OptionsMenu(drawWrapper, Graphics, audioWrapper, inputHelper);
                        pauseMenu = null;
                    }
                    else if (pauseMenu.Quit)
                    {
                        currentState = GameState.MainMenu;
                        world = new World();
                        LoadContent();
                        mainMenu = new MainMenu(drawWrapper);
                        pauseMenu = null;
                    }
                    break;
                case GameState.Options:
                    optionsMenu.Update(gameTime);
                    if (optionsMenu.Quit)
                    {
                        if (previousState == GameState.Paused)
                            currentState = GameState.Playing;
                        else
                        {
                            currentState = GameState.MainMenu;
                            mainMenu = new MainMenu(drawWrapper);
                        }
                        optionsMenu = null;
                    }
                    break;
                case GameState.GameOver:
                    gameOverMenu.Update(gameTime, inputHelper);
                    audioWrapper.StopOrPlayMusic(false);
                    if (gameOverMenu.Restart)
                    {
                        world = new World();
                        LoadContent();
                        world.Initialize();
                        currentState = GameState.Playing;
                        gameOverMenu = null;
                    }
                    else if (gameOverMenu.Quit)
                    {
                        world = new World();
                        LoadContent();
                        currentState = GameState.MainMenu;
                        gameOverMenu = null;
                        mainMenu = new MainMenu(drawWrapper);
                    }
                    break;
            }

            Profiler.LogEventEnd("Update");
        }

        protected override void Draw(GameTime gameTime)
        {
            Profiler.LogEventStart("Draw");
            GraphicsDevice.Clear(Color.White);

            drawWrapper.BeginDraw(); //Start drawing
            world.Draw(); //Draw the game world
            drawWrapper.BeginDrawGUI(); //Start drawing the GUI
            switch (currentState)
            {
                case GameState.MainMenu:    
                    mainMenu.DrawGUI(); //Draw the Main Menu
                    break;
                case GameState.Playing:
                    world.DrawGUI(); //Draw the GUI
                    break;
                case GameState.Paused:
                    pauseMenu.DrawGUI();
                    break;
                case GameState.Options:
                    optionsMenu.DrawGUI();
                    break;
                case GameState.GameOver:
                    gameOverMenu.DrawGUI();
                    break;
            }
            drawWrapper.EndOfDraw(); //Stop drawing

            base.Draw(gameTime);
            Profiler.LogEventEnd("Draw");

            Profiler.LogEventStart("FinalizeStep");
        }
    }
}
