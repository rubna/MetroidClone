using Microsoft.Xna.Framework;
using MetroidClone.Engine;

namespace MetroidClone.Metroid
{
    class OptionsMenu
    {
        enum Buttons
        {
            Sound,
            Music,
            Controller,
            Fullscreen,
            Quit,
            None
        }

        Buttons selectedButton;

        DrawWrapper drawing;
        public bool Sound, Music, Fullscreen, Quit;
        string sound = "SOUND", music = "MUSIC", controller = "CONTROLLER", fullscreen = "FULLSCREEN", quit = "EXIT";
        Rectangle soundButton, musicButton, controllerButton, fullscreenButton, quitButton, cursor;
        Color soundColor = Color.DarkGreen, musicColor = Color.DarkGreen, controllerColor = Color.DarkGreen,
            fullscreenColor = Color.DarkRed, quitColor = Color.DarkSlateGray;

        public OptionsMenu(DrawWrapper Drawing)
        {
            drawing = Drawing;
            Sound = false;
            Music = false;
            Fullscreen = false;
            Quit = false;
            selectedButton = Buttons.None;
        }

        public void Update(GameTime gameTime, InputHelper Input)
        {
            soundButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 350, 200, 100);
            musicButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 200, 200, 100);
            controllerButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 50, 200, 100);
            fullscreenButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 + 100, 200, 100);
            quitButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 + 250, 200, 100);
            
            cursor = Input.ControllerInUse ? new Rectangle(0, 0, 0, 0) : new Rectangle(Input.MouseCheckPosition().X, Input.MouseCheckPosition().Y, 1, 1);


            if (cursor.Intersects(soundButton) || selectedButton == Buttons.Sound)
            {
                soundColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    soundColor = soundColor.R == 0 ? Color.DarkRed : Color.DarkGreen;
                    Sound = true;
                }
            }
            else soundColor.A = 255;

            if (cursor.Intersects(musicButton) || selectedButton == Buttons.Music)
            {
                musicColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    musicColor = musicColor.R == 0 ? Color.DarkRed : Color.DarkGreen;
                    Music = true;
                }
            }
            else musicColor.A = 255;

            if (cursor.Intersects(controllerButton) || selectedButton == Buttons.Controller)
            {
                controllerColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    controllerColor = controllerColor.R == 0 ? Color.DarkRed : Color.DarkGreen;
                    Input.SwitchControls();
                    selectedButton = Input.ControllerInUse ? Buttons.Controller : Buttons.None;
                }
            }
            else controllerColor.A = 255;

            if (cursor.Intersects(fullscreenButton) || selectedButton == Buttons.Fullscreen)
            {
                fullscreenColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    fullscreenColor = fullscreenColor.R == 0 ? Color.DarkRed : Color.DarkGreen;
                    Fullscreen = true;
                }
            }
            else fullscreenColor.A = 255;

            if (cursor.Intersects(quitButton) || selectedButton == Buttons.Quit)
            {
                quitColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                    Quit = true;
            }
            else quitColor.A = 255;

            if (Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickDown))
            {
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Sound;
                else
                    selectedButton++;
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Sound;
            }

            if (Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickUp))
            {
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Quit;
                else if (selectedButton == Buttons.Sound)
                    selectedButton = Buttons.Quit;
                else
                    selectedButton--;
            }
        }

        public void DrawGUI()
        {
            //sound button
            drawing.DrawRectangleUnscaled(soundButton, soundColor);
            drawing.DrawText("font18", sound, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2 - 300), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //music button
            drawing.DrawRectangleUnscaled(musicButton, musicColor);
            drawing.DrawText("font18", music, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2 - 150), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //controller button
            drawing.DrawRectangleUnscaled(controllerButton, controllerColor);
            drawing.DrawText("font18", controller, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //fullscreen button
            drawing.DrawRectangleUnscaled(fullscreenButton, fullscreenColor);
            drawing.DrawText("font18", fullscreen, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2 + 150), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //quit button
            drawing.DrawRectangleUnscaled(quitButton, quitColor);
            drawing.DrawText("font18", quit, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2 + 300), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);
        }
    }
}


