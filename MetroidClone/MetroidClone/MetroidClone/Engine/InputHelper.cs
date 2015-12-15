using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MetroidClone.Engine
{
    internal class InputHelper
    {
        private static InputHelper instance;
        public static InputHelper Instance
        {
            get { return instance ?? (instance = new InputHelper()); }
        }

        private KeyboardState keyBoardState, lastKeyboardState;
        private GamePadState gamePadState, lastGamePadState;

        public void Update()
        {
            lastGamePadState = gamePadState;
            lastKeyboardState = keyBoardState;
            gamePadState = GamePad.GetState(PlayerIndex.One);
            keyBoardState = Keyboard.GetState();
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
        public Vector2 ThumbStickCheckDirection(bool left)
        {
            if (left)
                return gamePadState.ThumbSticks.Left.ToCartesian();
            return gamePadState.ThumbSticks.Right.ToCartesian();
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
        public bool GamePadTriggerDown(bool left)
        {
            if (left)
                return gamePadState.Triggers.Left != 0;
            return gamePadState.Triggers.Right != 0;
        }
        public bool GamePadTriggerPressed(bool left)
        {
            if (left)
                return gamePadState.Triggers.Left != 0 && lastGamePadState.Triggers.Left == 0;
            return gamePadState.Triggers.Right != 0 && lastGamePadState.Triggers.Right == 0;
        }
        public bool GamePadTriggerReleased(bool left)
        {
            if (left)
                return gamePadState.Triggers.Left == 0 && lastGamePadState.Triggers.Left != 0;
            return gamePadState.Triggers.Right == 0 && lastGamePadState.Triggers.Right != 0;
        }
    }
}
