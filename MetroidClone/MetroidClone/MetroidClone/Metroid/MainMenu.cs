using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace MetroidClone.Metroid
{
    class MainMenu : GameObject
    {
        public bool StartGame = false;
        public bool ExitGame = false;
        MouseState mouseState;
        MouseState previousMouseState;
        private Vector2 startButtonPosition;
        private Vector2 exitButtonPosition;


        public void Update(GameTime gameTime)
        {
            //wait for mouseclick
            mouseState = Mouse.GetState();
            if (previousMouseState.LeftButton == ButtonState.Pressed &&
                mouseState.LeftButton == ButtonState.Released)
            {
                MouseClicked(mouseState.X, mouseState.Y);
            }
            previousMouseState = mouseState;
        }
        public void Draw2()
        {

            Drawing.DrawRectangle(new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.Y / 2) - 50, 200, 100), Color.Black);
            Drawing.DrawRectangle(new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.X / 2), 200, 100), Color.Black);
            //draw the Mainmenu



        }

        void MouseClicked(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            Rectangle startButtonRect = new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.Y / 2) - 50, 200, 100);
            Rectangle exitButtonRect = new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.X / 2), 200, 100);
            if (mouseClickRect.Intersects(startButtonRect)) //player clicked start button
            {
                StartGame = true;
            }
            else if (mouseClickRect.Intersects(exitButtonRect)) //player clicked exit button
            {
                ExitGame = true;
            }
        }
    }
}

