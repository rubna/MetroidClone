using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace MetroidClone.Engine
{
    internal class InputHelper
    {
        private static InputHelper instance;
        public static InputHelper Instance
        {
            get { return instance ?? (instance = new InputHelper()); }
        }
        public bool ControllerInUse = false;

        private KeyboardState keyBoardState, lastKeyboardState;
        private GamePadState gamePadState, lastGamePadState;
        private MouseState mouseState, lastMouseState;
        private Stopwatch vibrateStopwatch = new Stopwatch();
        private double vibrateTime;

        //Updates the keyboard and mouse states or the gamepad state. can be switched between by pressing Enter on keyboard or
        //Start on gamepad. If there is no controller connected, it will automatically switch back to keyboard controls.
        public void Update()
        {
            if (!ControllerInUse)
            {
                lastKeyboardState = keyBoardState;
                keyBoardState = Keyboard.GetState();
                lastMouseState = mouseState;
                mouseState = Mouse.GetState();
                if (KeyboardCheckPressed(Keys.Enter))
                {
                    ControllerInUse = true;
                    GamePadVibrate(1, 1, 200);
                }
            }
            else
            {
                lastGamePadState = gamePadState;
                gamePadState = GamePad.GetState(PlayerIndex.One);
                if (vibrateStopwatch.ElapsedMilliseconds >= vibrateTime)
                {
                    vibrateStopwatch.Reset();
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
                }
                if (GamePadCheckDown(Buttons.Start) || !gamePadState.IsConnected)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
                    ControllerInUse = false;
                }
            }
        }

        public bool KeyboardCheckDown(Keys k)
        {
            return keyBoardState.IsKeyDown(k);
        }

        public bool KeyboardCheckReleased(Keys k)
        {
            return keyBoardState.IsKeyUp(k) && lastKeyboardState.IsKeyDown(k);
        }

        public bool KeyboardCheckPressed(Keys k)
        {
            return keyBoardState.IsKeyDown(k) && lastKeyboardState.IsKeyUp(k);
        }
        
        //Get the mouse position on the game window.
        public Point MouseCheckPosition()
        {
            return new Point(mouseState.X, mouseState.Y);
        }

        //Get the mouse position within the game view.
        public Point MouseCheckUnscaledPosition(DrawWrapper drawWrapper)
        {
            Rectangle displayRect = drawWrapper.DisplayRect;
            return new Point((mouseState.X - displayRect.X) * World.TileWidth * WorldGenerator.LevelWidth / displayRect.Width, (mouseState.Y - displayRect.Y) * World.TileHeight * WorldGenerator.LevelHeight / displayRect.Height);
        }

        public bool MouseButtonCheckDown(bool left)
        {
            if (left)
                return mouseState.LeftButton == ButtonState.Pressed;
            return mouseState.RightButton == ButtonState.Pressed;
        }

        public bool MouseButtonCheckReleased(bool left)
        {
            if (left)
                return mouseState.LeftButton == ButtonState.Released;
            return mouseState.RightButton == ButtonState.Released;
        }

        public bool MouseButtonCheckPressed(bool left)
        {
            if (left)
                return mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
            return mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released;
        }

        public bool MouseWheelCheckScroll(bool up)
        {
            if (up)
                return mouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue;
            return mouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue;
        }

        public bool MouseWheelPressed()
        {
            return mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released;
        }

        public bool ControllerCheckConnected()
        {
            return gamePadState.IsConnected;
        }

        public Vector2 ThumbStickCheckDirection(bool left)
        {
            if (!ControllerInUse)
                return Vector2.Zero;

            if (left)
                return gamePadState.ThumbSticks.Left;
            return gamePadState.ThumbSticks.Right;
        }
        
        public bool ThumbStickCheckDown(bool left)
        {
            if (!ControllerInUse)
                return false;

            if (left)
                return (gamePadState.ThumbSticks.Left.X != 0 || gamePadState.ThumbSticks.Left.Y != 0);
            return (gamePadState.ThumbSticks.Right.X != 0 || gamePadState.ThumbSticks.Right.Y != 0);
        }

        public bool GamePadCheckDown(Buttons b)
        {
            if (!ControllerInUse)
                return false;

            return gamePadState.IsButtonDown(b);
        }

        public bool GamePadCheckReleased(Buttons b)
        {
            if (!ControllerInUse)
                return false;

            return gamePadState.IsButtonUp(b) && lastGamePadState.IsButtonDown(b);
        }

        public bool GamePadCheckPressed(Buttons b)
        {
            if (!ControllerInUse)
                return false;

            return gamePadState.IsButtonDown(b) && lastGamePadState.IsButtonUp(b);
        }

        public bool GamePadTriggerCheckDown(bool left)
        {
            if (left)
                return gamePadState.Triggers.Left != 0;

            return gamePadState.Triggers.Right != 0;
        }

        public bool GamePadTriggerCheckPressed(bool left)
        {
            if (left)
                return gamePadState.Triggers.Left != 0 && lastGamePadState.Triggers.Left == 0;

            return gamePadState.Triggers.Right != 0 && lastGamePadState.Triggers.Right == 0;
        }

        public bool GamePadTriggerCheckReleased(bool left)
        {
            if (left)
                return gamePadState.Triggers.Left == 0 && lastGamePadState.Triggers.Left != 0;

            return gamePadState.Triggers.Right == 0 && lastGamePadState.Triggers.Right != 0;
        }

        public void GamePadVibrate(float leftMotor, float rightMotor, double time)
        {
            if (!ControllerInUse)
                return;
            vibrateStopwatch.Restart();
            if (time > vibrateTime - vibrateStopwatch.ElapsedMilliseconds)
                vibrateTime = time;
            GamePad.SetVibration(PlayerIndex.One, leftMotor, rightMotor);
        }
    }
}

