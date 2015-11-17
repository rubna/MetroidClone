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

        public void Update()
        {
            lastKeyboardState = keyBoardState;
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
    }
}
