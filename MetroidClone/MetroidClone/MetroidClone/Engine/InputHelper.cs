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
            }
            else
            {
                lastGamePadState = gamePadState;
                gamePadState = GamePad.GetState(PlayerIndex.One);
                //stops the controller from vibrating
                if (vibrateStopwatch.ElapsedMilliseconds >= vibrateTime)
                {
                    vibrateStopwatch.Reset();
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
                }
                if (!gamePadState.IsConnected)
                    ControllerInUse = false;
            }
        }

        //switches between controls: keyboard and mouse / controller
        public void SwitchControls()
        {
            if (ControllerInUse)
            {
                gamePadState = lastGamePadState;
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            }
            else
            {
                keyBoardState = lastKeyboardState;
                mouseState = lastMouseState;
            }
            ControllerInUse = !ControllerInUse;
        }

        //checks if a key is down
        public bool KeyboardCheckDown(Keys k)
        {
            return keyBoardState.IsKeyDown(k);
        }

        //checks if a key was down and now up
        public bool KeyboardCheckReleased(Keys k)
        {
            return keyBoardState.IsKeyUp(k) && lastKeyboardState.IsKeyDown(k);
        }

        //checks if a key was up and now down
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

        //checks if a mouse button is down
        public bool MouseButtonCheckDown(bool left)
        {
            if (left)
                return mouseState.LeftButton == ButtonState.Pressed;
            return mouseState.RightButton == ButtonState.Pressed;
        }

        //checks if a mouse button was down and now up
        public bool MouseButtonCheckReleased(bool left)
        {
            if (left)
                return mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed;
            return mouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed;
        }

        //checks if a mouse button was up and now down
        public bool MouseButtonCheckPressed(bool left)
        {
            if (left)
                return mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
            return mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released;
        }

        //chekcs if there has been scrolled
        public bool MouseWheelCheckScroll(bool up)
        {
            if (up)
                return mouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue;
            return mouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue;
        }

        //checks if the MMB was up and now down
        public bool MouseWheelPressed()
        {
            return mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released;
        }

        //checks if the controller is connected
        public bool ControllerCheckConnected()
        {
            return gamePadState.IsConnected;
        }

        //gives direction of a thumbstick
        public Vector2 ThumbStickCheckDirection(bool left)
        {
            if (!ControllerInUse)
                return Vector2.Zero;

            if (left)
                return gamePadState.ThumbSticks.Left;
            return gamePadState.ThumbSticks.Right;
        }
        
        //checks if a thumbstick isn't idle
        public bool ThumbStickCheckDown(bool left)
        {
            if (!ControllerInUse)
                return false;

            if (left)
                return (gamePadState.ThumbSticks.Left.X != 0 || gamePadState.ThumbSticks.Left.Y != 0);
            return (gamePadState.ThumbSticks.Right.X != 0 || gamePadState.ThumbSticks.Right.Y != 0);
        }

        //checks if a button is down
        public bool GamePadCheckDown(Buttons b)
        {
            if (!ControllerInUse)
                return false;

            return gamePadState.IsButtonDown(b);
        }

        //checks if a button is up and was down
        public bool GamePadCheckReleased(Buttons b)
        {
            if (!ControllerInUse)
                return false;

            return gamePadState.IsButtonUp(b) && lastGamePadState.IsButtonDown(b);
        }

        //checks if a button is down and was up
        public bool GamePadCheckPressed(Buttons b)
        {
            if (!ControllerInUse)
                return false;

            return gamePadState.IsButtonDown(b) && lastGamePadState.IsButtonUp(b);
        }

        //checks if a trigger is down
        public bool GamePadTriggerCheckDown(bool left)
        {
            if (left)
                return gamePadState.Triggers.Left != 0;

            return gamePadState.Triggers.Right != 0;
        }

        //checks if a trigger is down and was up
        public bool GamePadTriggerCheckPressed(bool left)
        {
            if (left)
                return gamePadState.Triggers.Left != 0 && lastGamePadState.Triggers.Left == 0;

            return gamePadState.Triggers.Right != 0 && lastGamePadState.Triggers.Right == 0;
        }

        //checks if a trigger is up and was down
        public bool GamePadTriggerCheckReleased(bool left)
        {
            if (left)
                return gamePadState.Triggers.Left == 0 && lastGamePadState.Triggers.Left != 0;

            return gamePadState.Triggers.Right == 0 && lastGamePadState.Triggers.Right != 0;
        }

        //vibrates the controller for a given time
        public void GamePadVibrate(float leftMotor, float rightMotor, double time)
        {
            if (!ControllerInUse)
                return;
            vibrateStopwatch.Restart();
            //if the time the controller still has to vibrate is bigger than what it is given here, it will keep the old value
            if (time > vibrateTime - vibrateStopwatch.ElapsedMilliseconds)
                vibrateTime = time;
            GamePad.SetVibration(PlayerIndex.One, leftMotor, rightMotor);
        }
    }
}

