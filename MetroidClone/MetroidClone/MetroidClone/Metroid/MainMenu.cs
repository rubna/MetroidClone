using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// creates the main menu. A large part of the code is based on code made by Nico Vermeir.
namespace MetroidClone.Metroid
{
    class MainMenu : GameObject
    {
        public bool StartGame = false;
        public bool ExitGame = false;
        public bool FullScreen = false;
        Rectangle startButtonRect;
        Rectangle exitButtonRect;
        InputHelper inputHelper = InputHelper.Instance;
       
        public void Update(GameTime gameTime)
        {

            
            //wait for mouseclick
            if (inputHelper.MouseButtonCheckPressed(true))
            {
                MouseClicked(inputHelper.MouseCheckPosition().X, inputHelper.MouseCheckPosition().Y);
            }
        }
        public void Draw2()
        {

            Drawing.DrawRectangle(new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.Y / 2) - 50, 200, 100), Color.Black);
            Drawing.DrawRectangle(new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.Y / 2) + 100, 200, 100), Color.Black);
            //draw the Mainmenu



        }

        void MouseClicked(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);
            if (!FullScreen)
            {
                startButtonRect = new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.Y / 2) - 50, 200, 100);
                exitButtonRect = new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.Y / 2) + 150, 200, 100);
            }
            else
            {
                startButtonRect = new Rectangle((int)(Drawing.ScreenSize.X) + 50, (int)(Drawing.ScreenSize.Y / 1.7f) + 25, 290, 125);
                exitButtonRect = new Rectangle((int)(Drawing.ScreenSize.X) + 50, (int)(Drawing.ScreenSize.Y / 1.7f) + 240, 290, 125);
            }
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

