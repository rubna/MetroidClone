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

        public void Update()
        {
            if (!ControllerInUse)
            {
                lastKeyboardState = keyBoardState;
                keyBoardState = Keyboard.GetState();
                lastMouseState = mouseState;
                mouseState = Mouse.GetState();
                if (KeyboardCheckDown(Keys.Enter))
                    ControllerInUse = true;
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
                    ControllerInUse = false;
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
        public Point MouseCheckPosition()
        {
            return new Point(mouseState.X, mouseState.Y);
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

        public bool ControllerCheckConnected()
        {
            return gamePadState.IsConnected;
        }
        public Vector2 ThumbStickCheckDirection(bool left)
        {
            if (left)
                return gamePadState.ThumbSticks.Left * (1 / gamePadState.ThumbSticks.Left.Length());
            return new Vector2(gamePadState.ThumbSticks.Right.X, -gamePadState.ThumbSticks.Right.Y) * (1 / gamePadState.ThumbSticks.Right.Length());
        }
        public bool ThumbStickCheckDown(bool left)
        {
            if (left)
                return (gamePadState.ThumbSticks.Left.X != 0 || gamePadState.ThumbSticks.Left.Y != 0);
            return (gamePadState.ThumbSticks.Right.X != 0 || gamePadState.ThumbSticks.Right.Y != 0);
        }
        public bool GamePadCheckDown(Buttons b)
        {
            return gamePadState.IsButtonDown(b);
        }
        public bool GamePadCheckReleased(Buttons b)
        {
            return gamePadState.IsButtonUp(b) && lastGamePadState.IsButtonDown(b);
        }
        public bool GamePadCheckPressed(Buttons b)
        {
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
            vibrateStopwatch.Restart();
            if (time > vibrateTime - vibrateStopwatch.ElapsedMilliseconds)
                vibrateTime = time;
            GamePad.SetVibration(PlayerIndex.One, leftMotor, rightMotor);
        }
    }
}

