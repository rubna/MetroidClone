using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MetroidClone.Metroid;

namespace MetroidClone.Engine
{
    class World
    {
        public enum GameState
        {
            MainMenu,
            Playing,
            Paused
        }

        public List<GameObject> GameObjects; //Gameobjects that have been created before this update
        List<GameObject> GameObjectsToUpdate; //Gameobjects that should be updated during Update.
        List<GameObject> GameObjectsWithGUI; //Gameobjects with a GUI event.
        List<GameObject> AddedGameObjects = new List<GameObject>();
        List<GameObject> RemovedGameObjects = new List<GameObject>();

        public DrawWrapper DrawWrapper { get; set; }
        public AudioWrapper AudioWrapper { get; set; }
        public AssetManager AssetManager { get; set; }
        public Level Level;
        public Tutorial Tutorial;
        public MainMenu MainMenu;
        public PauseMenu PauseMenu;
        public Player Player;
        public static Random Random;
        public Vector2 Camera;
        public GameState PlayingState = GameState.Playing;
        private bool worldInitialized = false;

        //The width and height of the world.
        public float Width { get; protected set; } = WorldGenerator.LevelWidth * WorldGenerator.WorldWidth * TileWidth + 200;
        public float Height { get; protected set; } = WorldGenerator.LevelHeight * WorldGenerator.WorldHeight * TileWidth + 200;

        //The width and height of a tile.
        public const float TileWidth = 48;
        public const float TileHeight = 48;

        const float GridSize = 100f;
        public List<ISolid>[,] SolidGrid;


        public List<ISolid> Solids
        {
            get
            {
                return GameObjects.OfType<ISolid>().ToList();
            }
        }

        public World()
        {
            GameObjects = new List<GameObject>();
            GameObjectsToUpdate = new List<GameObject>();
            GameObjectsWithGUI = new List<GameObject>();

            Random = new Random();

            Camera = new Vector2(0);

            MainMenu = new MainMenu();
            PauseMenu = new PauseMenu();
        }

        public void Initialize()
        {
            GameObjects.Clear();
            AddObject(MainMenu);
            AddObject(PauseMenu);
            (new WorldGenerator()).Generate(this);
            Tutorial = new Tutorial();
            AddObject(Tutorial);
            //AudioWrapper.PlayLooping("Audio/Music/Area 1");
            UpdateCamera(true);

            foreach (GameObject gameObject in GameObjects)
            {
                if (gameObject.ShouldUpdate)
                    GameObjectsToUpdate.Add(gameObject);
            }

            UpdateSolidGrid();
            PathfindingGrid();
            //AStarMap = new AStarMap(PathfindingGrid());
        }

        public void UpdateSolidGrid()
        {
            int hCells = (int)(Width / GridSize), vCells = (int)(Height / GridSize);
            SolidGrid = new List<ISolid>[hCells, vCells];
            List<ISolid> solids = Solids;
            for (int i = 0; i < hCells; i++)
            {
                for (int j = 0; j < vCells; j++)
                {
                    //Get a bounding box with some extra padding around it.
                    Rectangle boundingbox = new Rectangle((int)((i - 1) * GridSize), (int)((j - 1) * GridSize), (int)GridSize * 3, (int)GridSize * 3);
                    int numberOfSolids = solids.Count;
                    SolidGrid[i, j] = new List<ISolid>();
                    for (int k = 0; k < numberOfSolids; k++)
                    {
                        if (!(solids[k] is Wall) || solids[k].CollidesWith(boundingbox))
                        {
                            SolidGrid[i, j].Add(solids[k]);
                        }
                    }
                }
            }
        }

        public bool[,] PathfindingGrid()
        {
            bool[,] isSolid = new bool[WorldGenerator.LevelWidth * WorldGenerator.WorldWidth, WorldGenerator.LevelHeight * WorldGenerator.WorldHeight];

            foreach (ISolid solid in Solids)
            {
                if (solid is Wall)
                {
                    Wall wall = solid as Wall;
                    isSolid[wall.BoundingBox.Left / (int)TileWidth, wall.BoundingBox.Top / (int)TileHeight] = true;
                }
            }

            return isSolid;
        }

        public void AddObject(GameObject gameObject)
        {
            AddObject(gameObject, Vector2.Zero);
        }

        public void AddObject(GameObject gameObject, float x, float y)
        {
            AddObject(gameObject, new Vector2(x, y));
        }

        public void AddObject(GameObject gameObject, Vector2 position)
        {
            gameObject.World = this;
            gameObject.Drawing = DrawWrapper;
            gameObject.Audio = AudioWrapper;
            gameObject.Position = position;
            gameObject.Assets = AssetManager;
            GameObjects.Add(gameObject);
            gameObject.Create();
            AddedGameObjects.Add(gameObject);
            if (gameObject.ShouldDrawGUI)
                GameObjectsWithGUI.Add(gameObject);
        }

        public void RemoveObject(GameObject gameObject)
        {
            GameObjects.Remove(gameObject);
            RemovedGameObjects.Add(gameObject);
            if (gameObject.ShouldDrawGUI)
                GameObjectsWithGUI.Remove(gameObject);
        }

        public void Update(GameTime gameTime)
        {
            AddedGameObjects.Clear();
            RemovedGameObjects.Clear();

            if (PlayingState == GameState.Playing)
            {
                foreach (GameObject gameObject in GameObjectsToUpdate)
                    gameObject.Update(gameTime);

                foreach (GameObject gameObject in AddedGameObjects)
                {
                    if (gameObject.ShouldUpdate)
                        GameObjectsToUpdate.Add(gameObject);
                }

                foreach (GameObject gameObject in RemovedGameObjects)
                    GameObjectsToUpdate.Remove(gameObject);

                UpdateCamera(); //Update the position of the camera.
                PauseMenu.ResumeGame = false;
                MainMenu.StartGame = false;
            }
            if (PlayingState == GameState.MainMenu)
            {
                MainMenu.Update(gameTime);
                PauseMenu.ExitGame = false;
            }
            if (PlayingState == GameState.Paused)
            {
                PauseMenu.Update(gameTime);
        }
            if (MainMenu.StartGame && worldInitialized == false)
            {
                Initialize();
                PlayingState = GameState.Playing;
                worldInitialized = true;
            }
            if (PauseMenu.ResumeGame)
                PlayingState = GameState.Playing;
            if (PauseMenu.ExitGame)
            {
                worldInitialized = false;
                PlayingState = GameState.MainMenu;
            }
        }

        void UpdateCamera(bool jumpToGoal = false)
        {
            float roomWidth = WorldGenerator.LevelWidth * TileWidth, roomHeight = WorldGenerator.LevelHeight * TileHeight;

            //Calculate the basic goal position.
            Vector2 GoalCamera = new Vector2((int)(Player.Position.X / roomWidth) * roomWidth, (int)(Player.Position.Y / roomHeight) * roomHeight);

            //Briefly show a small part of the next room if you bump into a wall there.
            if (Player.TimeSinceHWallCollision < 5)
            {
                if (Player.LastHCollisionDirection == Direction.Right && Player.Position.X % roomWidth > roomWidth - TileWidth)
                    GoalCamera.X += TileWidth;
                if (Player.LastHCollisionDirection == Direction.Left && Player.Position.X % roomWidth < TileWidth)
                    GoalCamera.X -= TileWidth;
            }
            if (Player.TimeSinceVWallCollision < 5)
            {
                if (Player.LastVCollisionDirection == Direction.Down && Player.Position.Y % roomHeight > roomHeight - TileHeight)
                    GoalCamera.Y += TileHeight;
            }
            if (Player.TimeSinceVWallCollision < 12)
            {
                if (Player.LastVCollisionDirection == Direction.Up && Player.Position.Y % roomHeight < TileHeight)
                    GoalCamera.Y -= TileHeight;
            }

            if (jumpToGoal)
                Camera = GoalCamera;
            else
            {
                //Move the camera towards the goal if the goal camera position if not equal to the camera position.
                if (Camera.X < GoalCamera.X)
                    Camera.X = Math.Min(GoalCamera.X, Camera.X + (GoalCamera.X - Camera.X) * 0.12f + 3f);
                if (Camera.X > GoalCamera.X)
                    Camera.X = Math.Max(GoalCamera.X, Camera.X + (GoalCamera.X - Camera.X) * 0.12f - 3f);
                if (Camera.Y < GoalCamera.Y)
                    Camera.Y = Math.Min(GoalCamera.Y, Camera.Y + (GoalCamera.Y - Camera.Y) * 0.12f + 3f);
                if (Camera.Y > GoalCamera.Y)
                    Camera.Y = Math.Max(GoalCamera.Y, Camera.Y + (GoalCamera.Y - Camera.Y) * 0.12f - 3f);
            }
        }

        public void Draw()
        {
            //Draw the background.
            float removeFromX = Camera.X % TileWidth, removeFromY = Camera.Y % TileHeight;
            int startX = (int)Camera.X / (int)TileWidth, startY = (int)Camera.Y / (int)TileHeight;
            Vector2 tileSize = new Vector2(TileWidth, TileHeight);

            //Make the tile placement look random (it isn't)
            for (int i = 0; i < WorldGenerator.LevelWidth + 1; i++)
                for (int j = 0; j < WorldGenerator.LevelHeight + 1; j++)
                {
                    int xpos = startX + i, ypos = startY + j;
                    DrawWrapper.DrawSprite("BackgroundTileset/background" + ((xpos % 3 + xpos % 9 + ypos + ypos % 5 + ypos % 9) % 4 + 1), new Vector2(i * 48 - removeFromX, j * 48 - removeFromY), 0f, tileSize);
                }

            //Only draw objects that are visible (within the view)
            if (PlayingState == GameState.Playing)
            {
            foreach (GameObject gameObject in GameObjects.OrderByDescending(x => x.Depth))
            {
                Vector2 drawPos = gameObject.CenterPosition - Camera;
                if (drawPos.X > -100 && drawPos.Y > -100 &&
                    drawPos.X < WorldGenerator.LevelWidth * TileWidth + 100 &&
                    drawPos.Y < WorldGenerator.LevelHeight * TileHeight + 100)
                {
                    gameObject.Draw();
                }
            }
            }
            if (PlayingState == GameState.Paused)
                PauseMenu.Draw2();
            if (PlayingState == GameState.MainMenu)
                MainMenu.Draw2();
        }

        public void DrawGUI()
        {
            //Call the Draw GUI event of all objects that have one.
            foreach (GameObject gameObject in GameObjectsWithGUI.OrderByDescending(x => x.Depth))
                gameObject.DrawGUI();
        }

        public List<ISolid> GetNearSolids(Vector2 position)
        {
            if (position.X > -GridSize && position.Y > -GridSize && position.X < Width && position.Y < Height)
                return SolidGrid[(int)(position.X / GridSize), (int)(position.Y / GridSize)];
            else
                return new List<ISolid>(); //Nothing here.
        }
    }
}
