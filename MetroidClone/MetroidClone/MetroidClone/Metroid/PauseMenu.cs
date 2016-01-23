using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// creates the pause menu.
namespace MetroidClone.Metroid
{
    class PauseMenu : Menu
    {

        private int resumeButtonCheck = 1;
        private int optionsButtonCheck = 1;
        private int exitButtonCheck = 1;
        public bool ResumeButtonIntersects;
        public bool OptionsButtonIntersects;
        public bool ExitButtonIntersects;

        public override void Update2(GameTime gameTime)
            {
            base.Update2(gameTime);
            Paused = false;
            MouseCheck((int)Input.MouseCheckPosition().X, (int)Input.MouseCheckPosition().Y);
            // change the color of the buttons if the mouse is on them and changes the gamestate if a button is clicked
            if (ResumeButtonIntersects)
            {
                resumeButtonCheck = 2;
                if (Input.MouseButtonCheckPressed(true))
                {
                    ResumeGame = true;
            }
        }
            else
                resumeButtonCheck = 1;
            if (OptionsButtonIntersects)
        {
                optionsButtonCheck = 2;
                if (Input.MouseButtonCheckPressed(true))
                {
                    GoToOptions = true;
        }
            }
            else
                optionsButtonCheck = 1;

            if (ExitButtonIntersects)
        {
                exitButtonCheck = 2;
                if (Input.MouseButtonCheckPressed(true))
            {
                    ExitMenu = true;
                }
            }
            else
                exitButtonCheck = 1;
            // resumes the game if the resume button is pressed
            if (ResumeGame)
            {
                World.PlayingState = World.GameState.Playing;
                ResumeGame = false;
            }
            // goes to the options menu if the options button is pressed
            if (GoToOptions)
            {
                World.PlayingState = World.GameState.OptionsMenu;
                GoToOptions = false;
                Paused = true;
            }
            // exits to main menu if the exit button is pressed
            if (ExitMenu)
            {
                World.Initialize();
                World.PlayingState = World.GameState.MainMenu;
                ExitMenu = false;
            }
        }
        public override void Draw2()
        { //draw the Pause menu
            ButtonNumber = 1;
            base.Draw2();
            DrawButton("resume", resumeButtonCheck);
            DrawButton("options", optionsButtonCheck);
            DrawButton("exit", exitButtonCheck);
            }
        void MouseCheck(int x, int y)
        {
            ButtonNumber = 1;
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);
            //creates rectangles for the buttons which make it possible to check if the button is pressed
            ResumeButtonIntersects = ButtonStateCheck(mouseClickRect, ResumeButtonIntersects);
            OptionsButtonIntersects = ButtonStateCheck(mouseClickRect, OptionsButtonIntersects);
            ExitButtonIntersects = ButtonStateCheck(mouseClickRect, ExitButtonIntersects);
        }
    }
}


