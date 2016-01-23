using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// creates the options menu. 
namespace MetroidClone.Metroid
{
    class OptionsMenu : Menu
    {

        private int exitButtonCheck = 1;
        public bool ExitButtonIntersects;
        private int soundButtonCheck = 1;
        public bool SoundButtonIntersects;
        private int musicButtonCheck = 1;
        public bool MusicButtonIntersects;
        private int controllerButtonCheck = 1;
        public bool ControllerButtonIntersects;
        private int fullScreenButtonCheck = 1;
        public bool FullScreenButtonIntersects;
        public bool SoundOn = true;
        public bool MusicOn = true;
        public bool ControllerOn = false;
        public bool SwitchFullScreen = false;
        private int time = 0;

        public void UpdateMenu(GameTime gameTime)
        {
            
            MouseCheck((int)Input.MouseCheckPosition().X, (int)Input.MouseCheckPosition().Y);
            // changes the color of the buttons if the mouse is on them and changes the gamestate if a button is clicked
            soundButtonCheck = ButtonIntersects(SoundButtonIntersects, soundButtonCheck);
            if (soundButtonCheck == 2 && Input.MouseButtonCheckPressed(true))
                SoundOn = !SoundOn;
            musicButtonCheck = ButtonIntersects(MusicButtonIntersects, musicButtonCheck);
            if (musicButtonCheck == 2 && Input.MouseButtonCheckPressed(true))
                MusicOn = !MusicOn;
            controllerButtonCheck = ButtonIntersects(ControllerButtonIntersects, controllerButtonCheck);
            if (controllerButtonCheck == 2 && Input.MouseButtonCheckPressed(true) && time > 10)
                ControllerOn = !ControllerOn;
            exitButtonCheck = ButtonIntersects(ExitButtonIntersects, exitButtonCheck);
            if (exitButtonCheck == 2 && Input.MouseButtonCheckPressed(true))
                ExitMenu = true;
            fullScreenButtonCheck = ButtonIntersects(FullScreenButtonIntersects, fullScreenButtonCheck);
            if (fullScreenButtonCheck == 2 && Input.MouseButtonCheckPressed(true))
                SwitchFullScreen = true;
            // goes back to the previous menu if the exit button is pressed
            if (ExitMenu)
            {
                if (Paused)
                    World.PlayingState = World.GameState.Paused;
                else
                    World.PlayingState = World.GameState.MainMenu;
                time = 0;
                ExitMenu = false;
                Paused = false;
            }
            if (SoundOn == false)
                World.AudioWrapper.AudioIsEnabled = false;
            else
                World.AudioWrapper.AudioIsEnabled = true;
            time ++;
        }
        public void DrawMenu()
        { //draw the Mainmenu
            ButtonNumber = 0;
            DrawOptionState(SoundOn);
            DrawButton("sound", soundButtonCheck);
            DrawOptionState(MusicOn);
            DrawButton("music", musicButtonCheck);
            DrawOptionState(ControllerOn);
            DrawButton("controller", controllerButtonCheck);
            DrawOptionState(FullScreen);
            DrawButton("fullscreen", fullScreenButtonCheck);
            DrawButton("exit options", exitButtonCheck, 1);
        }

        void MouseCheck(int x, int y)
        {
            ButtonNumber = 0;
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);
            //creates rectangles for the buttons which make it possible to check if the button is pressed
            SoundButtonIntersects = ButtonStateCheck(mouseClickRect, SoundButtonIntersects);
            MusicButtonIntersects = ButtonStateCheck(mouseClickRect, SoundButtonIntersects);
            ControllerButtonIntersects = ButtonStateCheck(mouseClickRect, ControllerButtonIntersects);
            FullScreenButtonIntersects = ButtonStateCheck(mouseClickRect, FullScreenButtonIntersects);
            ExitButtonIntersects = ButtonStateCheck(mouseClickRect, ExitButtonIntersects); 
        }
        
        void DrawOptionState(bool on)
        {

            //draws the text wich indicates if a option is turned on or off.
            if (!FullScreen)
            {
                if (on)
                {
                    Vector2 TextSize = Drawing.MeasureText("font22", "on");
                    Drawing.DrawText("font22", "on", new Vector2((int)(Drawing.ScreenSize.X / 2) + 250, (int)(Drawing.ScreenSize.Y / 2) - 150 - TextSize.Y / 2 + 150 * ButtonNumber), Color.Green);
                }
                else
                {
                    Vector2 TextSize = Drawing.MeasureText("font22", "off");
                    Drawing.DrawText("font22", "off", new Vector2((int)(Drawing.ScreenSize.X / 2) + 250, (int)(Drawing.ScreenSize.Y / 2) - 150 - TextSize.Y / 2 + 150 * ButtonNumber), Color.Red);
                }
            }
            else
            {
                if (on)
                {
                    Vector2 TextSize = Drawing.MeasureText("font22", "on");
                    Drawing.DrawText("font22", "on", new Vector2((int)(Drawing.ScreenSize.X / 2) + 800, (int)(Drawing.ScreenSize.Y / 1.7f) - 185 - TextSize.Y / 2 + 230 * ButtonNumber), Color.Green, 0, null, 2);
                }
                else
                {
                    Vector2 TextSize = Drawing.MeasureText("font22", "off");
                    Drawing.DrawText("font22", "off", new Vector2((int)(Drawing.ScreenSize.X / 2) + 800, (int)(Drawing.ScreenSize.Y / 1.7f) - 185 - TextSize.Y / 2 + 230 * ButtonNumber), Color.Red, 0, null, 2);
                }
            }
        }

        int ButtonIntersects(bool buttonIntersects,  int buttonCheck)
        {
            if (buttonIntersects)
                buttonCheck = 2;
            else
                buttonCheck = 1;
            return buttonCheck;
        }
    }
}

