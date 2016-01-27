using Microsoft.Xna.Framework;
using MetroidClone.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MetroidClone.Metroid
{
    class OptionsMenu
    {
        //for extended comments see MainMenu.cs, because it is very similar
        
        enum MenuButton
        {
            Sound,
            Music,
            Controller,
            Fullscreen,
            Quit,
            None
        }

        MenuButton selectedButton;

        DrawWrapper drawing;
        AudioWrapper audio;
        GraphicsDeviceManager graphics;
        InputHelper input;
        public bool Quit;
        string sound = "Sound", music = "Music", controller = "Controller", fullscreen = "Fullscreen", quit = "Back";
        Rectangle soundButton, musicButton, controllerButton, fullscreenButton, quitButton, cursor;
        Color soundColor, musicColor, controllerColor, fullscreenColor, quitColor = Color.DarkSlateGray;

        public OptionsMenu(DrawWrapper Drawing, GraphicsDeviceManager Graphics, AudioWrapper Audio, InputHelper Input)
        {
            drawing = Drawing;
            audio = Audio;
            graphics = Graphics;
            input = Input;
            Quit = false;
            selectedButton = MenuButton.None;

            soundColor = audio.AudioIsEnabled ? Color.DarkGreen : Color.DarkRed;
            musicColor = audio.MusicIsEnabled ? Color.DarkGreen : Color.DarkRed;
            controllerColor = input.ControllerInUse ? Color.DarkGreen : Color.DarkRed;
            fullscreenColor = graphics.IsFullScreen ? Color.DarkGreen : Color.DarkRed;

            soundButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 300, 200, 100);
            musicButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 150, 200, 100);
            //controllerButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 50, 200, 100);
            fullscreenButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2, 200, 100);
            quitButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 + 150, 200, 100);
        }

        public void Update(GameTime gameTime)
        {
            soundButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 300, 200, 100);
            musicButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 150, 200, 100);
            //controllerButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 50, 200, 100);
            fullscreenButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2, 200, 100);
            quitButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 + 150, 200, 100);

            cursor = input.ControllerInUse ? new Rectangle(0, 0, 0, 0) : new Rectangle(input.MouseCheckPosition().X, input.MouseCheckPosition().Y, 1, 1);


            if (cursor.Intersects(soundButton) || selectedButton == MenuButton.Sound)
            {
                soundColor.A = 200;
                if (input.MouseButtonCheckPressed(true) || input.GamePadCheckPressed(Buttons.A))
                {
                    audio.AudioIsEnabled = !audio.AudioIsEnabled;
                    soundColor = audio.AudioIsEnabled ? Color.DarkGreen : Color.DarkRed;
                }
            }
            else soundColor.A = 255;

            if (cursor.Intersects(musicButton) || selectedButton == MenuButton.Music)
            {
                musicColor.A = 200;
                if (input.MouseButtonCheckPressed(true) || input.GamePadCheckPressed(Buttons.A))
                {
                    audio.MusicIsEnabled = !audio.MusicIsEnabled;
                    musicColor = audio.MusicIsEnabled ? Color.DarkGreen : Color.DarkRed;
                    audio.StopOrPlayMusic(audio.MusicIsEnabled);
                }
            }
            else musicColor.A = 255;

            /*if (cursor.Intersects(controllerButton) || selectedButton == MenuButton.Controller)
            {
                controllerColor.A = 200;
                if (input.MouseButtonCheckPressed(true) || input.GamePadCheckPressed(Buttons.A))
                {
                    controllerColor = controllerColor == Color.DarkGreen ? Color.DarkRed : Color.DarkGreen;
                    input.SwitchControls();
                    selectedButton = input.ControllerInUse ? MenuButton.Controller : MenuButton.None;
                }
            }
            else controllerColor.A = 255;*/

            if (cursor.Intersects(fullscreenButton) || selectedButton == MenuButton.Fullscreen)
            {
                fullscreenColor.A = 200;
                if (input.MouseButtonCheckPressed(true) || input.GamePadCheckPressed(Buttons.A))
                {
                    SwitchFullscreen();
                    fullscreenColor = graphics.IsFullScreen ? Color.DarkGreen : Color.DarkRed;
                }
            }
            else fullscreenColor.A = 255;

            if (cursor.Intersects(quitButton) || selectedButton == MenuButton.Quit)
            {
                quitColor.A = 200;
                if (input.MouseButtonCheckPressed(true) || input.GamePadCheckPressed(Buttons.A))
                    Quit = true;
            }
            else quitColor.A = 255;

            if (input.GamePadCheckPressed(Buttons.LeftThumbstickDown))
            {
                if (selectedButton == MenuButton.None)
                    selectedButton = MenuButton.Sound;
                else
                    selectedButton++;
                if (selectedButton == MenuButton.None)
                    selectedButton = MenuButton.Sound;
            }

            if (input.GamePadCheckPressed(Buttons.LeftThumbstickUp))
            {
                if (selectedButton == MenuButton.None)
                    selectedButton = MenuButton.Quit;
                else if (selectedButton == MenuButton.Sound)
                    selectedButton = MenuButton.Quit;
                else
                    selectedButton--;
            }
        }

        public void DrawGUI()
        {
            //sound button
            drawing.DrawRectangleUnscaled(soundButton, soundColor);
            drawing.DrawText("font18", sound, soundButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //music button
            drawing.DrawRectangleUnscaled(musicButton, musicColor);
            drawing.DrawText("font18", music, musicButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //controller button
            //drawing.DrawRectangleUnscaled(controllerButton, controllerColor);
            //drawing.DrawText("font18", controller, controllerButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //fullscreen button
            drawing.DrawRectangleUnscaled(fullscreenButton, fullscreenColor);
            drawing.DrawText("font18", fullscreen, fullscreenButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //quit button
            drawing.DrawRectangleUnscaled(quitButton, quitColor);
            drawing.DrawText("font18", quit, quitButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);
        }

        //switches to fullscreen and back, depending on the current state
        void SwitchFullscreen()
        {
            if (!graphics.IsFullScreen)
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 24 * 20 * 2;
                graphics.PreferredBackBufferHeight = 24 * 15 * 2;
                graphics.IsFullScreen = false;
            }
            drawing.SmartScale(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            graphics.ApplyChanges();

        }
    }
}


