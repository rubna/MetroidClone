using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// creates the main menu. 
namespace MetroidClone.Metroid
{
    class MainMenu : Menu
    {
       
        private int startButtonCheck = 1;
        private int optionsButtonCheck = 1;
        private int exitButtonCheck = 1;
        public bool StartButtonIntersects;
        public bool OptionsButtonIntersects;
        public bool ExitButtonIntersects;

        public void UpdateMenu(GameTime gameTime)
        {
            MouseCheck((int)Input.MouseCheckPosition().X, (int)Input.MouseCheckPosition().Y);
            // change the color of the buttons if the mouse is on them and changes the gamestate if a button is clicked
            if (StartButtonIntersects)
            {
                startButtonCheck = 2;
                if (Input.MouseButtonCheckPressed(true))
                {
                    StartGame = true;
            }

        }
            else
                startButtonCheck = 1;
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
                    ExitGame = true;
                }
            }
            else
                exitButtonCheck = 1;

            // goes to the options menu if the options button is pressed
            if (GoToOptions)
            {
                World.PlayingState = World.GameState.OptionsMenu;
                GoToOptions = false;
            }
            // starts the game if the start button is pressed
            if (StartGame)
            {
                World.AddObject(World.Tutorial);
                World.PlayingState = World.GameState.Playing;
                StartGame = false;
            }
        }
        public void DrawMenu()
        { //draw the Mainmenu
            
            DrawButton("start", startButtonCheck);
            DrawButton("options", optionsButtonCheck);
            DrawButton("exit", exitButtonCheck);
            ButtonNumber = 1;
           

            }
        void MouseCheck(int x, int y)
            {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);
            //creates rectangles for the buttons which make it possible to check if the button is pressed
            StartButtonIntersects = ButtonStateCheck(mouseClickRect, StartButtonIntersects);
            OptionsButtonIntersects = ButtonStateCheck(mouseClickRect, OptionsButtonIntersects);
            ExitButtonIntersects = ButtonStateCheck(mouseClickRect, ExitButtonIntersects);
            ButtonNumber = 1;
        }
    }
}
