﻿using Microsoft.Xna.Framework;

// creates the pause menu. A large part of the code is based on code made by Nico Vermeir.
namespace MetroidClone.Engine
{
    class PauseMenu : GameObject
    {
        public bool ResumeGame = false;
        public bool ExitGame = false;
        public bool FullScreen = false;
        InputHelper inputHelper = InputHelper.Instance;
        Rectangle resumeButtonRect;
        Rectangle exitButtonRect;

        public void UpdateMenu(GameTime gameTime)
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
            Drawing.DrawRectangle(new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.Y / 2) + 150, 200, 100), Color.Black);

            //draw the pause menu



        }

        void MouseClicked(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            if (!FullScreen)
            {
                resumeButtonRect = new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.Y / 2) - 50, 200, 100);
                exitButtonRect = new Rectangle((int)(Drawing.ScreenSize.X / 2), (int)(Drawing.ScreenSize.Y / 2) + 150, 200, 100);
            }
            else
            {
                resumeButtonRect = new Rectangle((int)(Drawing.ScreenSize.X) + 50, (int)(Drawing.ScreenSize.Y / 1.7f) + 25, 290, 125);
                exitButtonRect = new Rectangle((int)(Drawing.ScreenSize.X) + 50, (int)(Drawing.ScreenSize.Y / 1.7f) + 310, 290, 140);
            }
            if (mouseClickRect.Intersects(resumeButtonRect)) //player clicked Resume button
            {
                ResumeGame = true;
            }
            else if (mouseClickRect.Intersects(exitButtonRect)) //player clicked exit button
            {
                ExitGame = true;
            }
        }
    }
}

