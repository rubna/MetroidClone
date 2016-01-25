using Microsoft.Xna.Framework;
using MetroidClone.Engine;

namespace MetroidClone.Metroid
{
    class GameOverMenu
    {
        enum Buttons
        {
            Restart,
            Quit,
            None
        }

        Buttons selectedButton;

        DrawWrapper drawing;
        public bool Restart, Quit;
        string gameOver = "GAME OVER", restart = "RESTART", quit = "QUIT";
        Rectangle restartButton, quitButton, cursor;
        Color restartColor = Color.DarkSlateGray, quitColor = Color.DarkSlateGray;

        public GameOverMenu(DrawWrapper Drawing)
        {
            drawing = Drawing;
            Restart = false;
            Quit = false;
            selectedButton = Buttons.None;
        }

        public void Update(GameTime gameTime, InputHelper Input)
        {
            restartButton = new Rectangle((int)drawing.GUISize.X / 2 - 300, (int)drawing.GUISize.Y / 2 + 100, 200, 100);
            quitButton = new Rectangle((int)drawing.GUISize.X / 2 + 100, (int)drawing.GUISize.Y / 2 + 100, 200, 100);
            cursor = Input.ControllerInUse ? new Rectangle(0, 0, 0, 0) : new Rectangle(Input.MouseCheckPosition().X, Input.MouseCheckPosition().Y, 1, 1);

            if (cursor.Intersects(restartButton) || selectedButton == Buttons.Restart)
            {
                restartColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    Restart = true;
                }
            }
            else
                restartColor.A = 255;

            if (cursor.Intersects(quitButton) || selectedButton == Buttons.Quit)
            {
                quitColor.A = 200;
                if (Input.MouseButtonCheckPressed(true) || Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    Quit = true;
                }
            }
            else
                quitColor.A = 255;

            if (Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickLeft))
            {
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Restart;
                else
                    selectedButton++;
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Restart;
            }

            if (Input.GamePadCheckPressed(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickRight))
            {
                if (selectedButton == Buttons.None)
                    selectedButton = Buttons.Quit;
                else if (selectedButton == Buttons.Restart)
                    selectedButton = Buttons.Quit;
                else
                    selectedButton--;
            }
        }

        public void DrawGUI()
        {
            //game over text
            drawing.DrawText("font48", gameOver, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2), Color.White, alignment: Engine.Asset.Font.Alignment.BottomCenter);

            //restart button
            drawing.DrawRectangleUnscaled(restartButton, restartColor);
            drawing.DrawText("font18", restart, restartButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //quit button
            drawing.DrawRectangleUnscaled(quitButton, quitColor);
            drawing.DrawText("font18", quit, quitButton.Center.ToVector2(), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);
        }
    }
}


