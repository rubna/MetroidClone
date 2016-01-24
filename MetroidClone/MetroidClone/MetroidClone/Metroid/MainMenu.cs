using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class MainMenu
    {
        enum Buttons
        {
            Start,
            Options,
            ExitGame,
            None
        }

        Buttons selectedButton;

        DrawWrapper drawing;
        public bool Start, Options, ExitGame;
        string start = "START", options = "OPTIONS", exit = "EXIT";
        Rectangle startButton, optionsButton, exitButton, cursor;
        Color startColor = Color.DarkSlateGray, optionsColor = Color.DarkSlateGray, exitColor = Color.DarkSlateGray;

        public MainMenu(DrawWrapper Drawing)
        {
            drawing = Drawing;
            ExitGame = false;
            Start = false;
            Options = false;
        }

        public void Update(GameTime gameTime, InputHelper Input)
        {
            startButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 200, 200, 100);
            optionsButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 50, 200, 100);
            exitButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 + 100, 200, 100);

            cursor = Input.ControllerInUse ? new Rectangle(0, 0, 0, 0) : new Rectangle(Input.MouseCheckPosition().X, Input.MouseCheckPosition().Y, 1, 1);

            if (cursor.Intersects(startButton) || selectedButton == Buttons.Start)
            {
                startColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    Start = true;
                }
            }
            else startColor.A = 255;

            if (cursor.Intersects(optionsButton) || selectedButton == Buttons.Options)
            {
                optionsColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    Options = true;
                }
            }
            else optionsColor.A = 255;

            if (cursor.Intersects(exitButton) || selectedButton == Buttons.ExitGame)
            {
                exitColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    ExitGame = true;
                }
            }
            else exitColor.A = 255;

            if (Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickDown))
            {
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Start;
                else
                    selectedButton++;
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Start;
            }

            if (Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickUp))
            {
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.ExitGame;
                else if (selectedButton == Buttons.Start)
                    selectedButton = Buttons.ExitGame;
                else
                    selectedButton--;
            }
        }

        public void DrawGUI()
        {
            //start button
            drawing.DrawRectangleUnscaled(startButton, startColor);
            drawing.DrawText("font18", start, new Vector2(drawing.GUISize.X /2, drawing.GUISize.Y / 2 - 150), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //options button
            drawing.DrawRectangleUnscaled(optionsButton, optionsColor);
            drawing.DrawText("font18", options, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //quit button
            drawing.DrawRectangleUnscaled(exitButton, exitColor);
            drawing.DrawText("font18", exit, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2 + 150), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);
        }
    }
}
