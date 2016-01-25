using Microsoft.Xna.Framework;
using MetroidClone.Engine;

namespace MetroidClone.Metroid
{
    class PauseMenu
    {
        enum Buttons
        {
            Resume,
            Options,
            Quit,
            None
        }

        Buttons selectedButton;

        DrawWrapper drawing;
        public bool Resume, Options, Quit;
        string resume = "RESUME", options = "OPTIONS", quit = "QUIT";
        Rectangle resumeButton, optionsButton, quitButton, cursor;
        Color resumeColor = Color.DarkSlateGray, optionsColor = Color.DarkSlateGray, quitColor = Color.DarkSlateGray;

        public PauseMenu(DrawWrapper Drawing)
        {
            drawing = Drawing;
            Resume = false;
            Options = false;
            Quit = false;
            selectedButton = Buttons.None;
        }

        public void Update(GameTime gameTime, InputHelper Input)
        {
            resumeButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 200, 200, 100);
            optionsButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 50, 200, 100);
            quitButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 + 100, 200, 100);
            cursor = Input.ControllerInUse ? new Rectangle(0, 0, 0, 0) : new Rectangle(Input.MouseCheckPosition().X, Input.MouseCheckPosition().Y, 1, 1);

            if (cursor.Intersects(resumeButton) || selectedButton == Buttons.Resume)
            {
                resumeColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    Resume = true;
                }
            }
            else resumeColor.A = 255;

            if (cursor.Intersects(optionsButton) || selectedButton == Buttons.Options)
            {
                optionsColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    Options = true;
                }
            }
            else optionsColor.A = 255;

            if (cursor.Intersects(quitButton) || selectedButton == Buttons.Quit)
            {
                quitColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    Quit = true;
                }
            }
            else quitColor.A = 255;

            if (Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickDown))
            {
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Resume;
                else
                    selectedButton++;
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Resume;
            }

            if (Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickUp))
            {
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Quit;
                else if (selectedButton == Buttons.Resume)
                    selectedButton = Buttons.Quit;
                else
                    selectedButton--;
            }
        }

        public void DrawGUI()
        {
            //resume button
            drawing.DrawRectangleUnscaled(resumeButton, resumeColor);
            drawing.DrawText("font18", resume, resumeButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //options button
            drawing.DrawRectangleUnscaled(optionsButton, optionsColor);
            drawing.DrawText("font18", options, optionsButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //quit button
            drawing.DrawRectangleUnscaled(quitButton, quitColor);
            drawing.DrawText("font18", quit, quitButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);
        }
    }
}


