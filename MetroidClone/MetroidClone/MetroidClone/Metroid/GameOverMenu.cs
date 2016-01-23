using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// creates the game over menu. 
namespace MetroidClone.Metroid
{
    class GameOverMenu : Menu
    {

        private int restartButtonCheck = 1;
        private int exitButtonCheck = 1;
        public bool RestartButtonIntersects;
        public bool ExitButtonIntersects;

        public override void Update2(GameTime gameTime)
        {
            base.Update2(gameTime);
            MouseCheck((int)Input.MouseCheckPosition().X, (int)Input.MouseCheckPosition().Y);
            // change the color of the buttons if the mouse is on them and changes the gamestate if a button is clicked
            if (RestartButtonIntersects)
            {
                restartButtonCheck = 2;
                if (Input.MouseButtonCheckPressed(true))
                {
                    StartGame = true;
                }
            }
            else
                restartButtonCheck = 1;

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

            // starts the game if the start button is pressed
            if (StartGame)
            {
                World.Initialize();
                World.PlayingState = World.GameState.Playing;
                StartGame = false;
            }
            // goes to the options menu if the options button is pressed
            if (ExitMenu)
            {
                World.Initialize();
                World.PlayingState = World.GameState.MainMenu;
                ExitMenu = false;
            }
        }
        public override void Draw2()
        { //draw the Mainmenu
            base.Draw2();
            Vector2 TextSize = Drawing.MeasureText("font48", "You Are Dead");
            if (!FullScreen)
                Drawing.DrawText("font48", "You Are Dead", new Vector2(Drawing.GUISize.X / 2, Drawing.ScreenSize.Y / 2 - TextSize.Y / 2 - 150), Color.Red, alignment: Engine.Asset.Font.Alignment.TopCenter);
            else
                Drawing.DrawText("font48", "You Are Dead", new Vector2(Drawing.GUISize.X / 2, Drawing.ScreenSize.Y / 2 - TextSize.Y / 2 - 150), Color.Red, 0, null, 2, alignment: Engine.Asset.Font.Alignment.TopCenter);
            Vector2 TextSize2 = Drawing.MeasureText("font22", "score:" + World.Player.Score.ToString());
            if (!FullScreen)
                Drawing.DrawText("font22", "score:" + World.Player.Score.ToString(), new Vector2(Drawing.GUISize.X / 2, Drawing.ScreenSize.Y / 2 - TextSize2.Y / 2), Color.Black, alignment: Engine.Asset.Font.Alignment.TopCenter);
            else
                Drawing.DrawText("font22", "score:" + World.Player.Score.ToString(), new Vector2(Drawing.GUISize.X / 2, (Drawing.ScreenSize.Y / 1.7f)), Color.Black, 0, null, 2, alignment: Engine.Asset.Font.Alignment.TopCenter);
            ButtonNumber = 2;
            DrawButton("restart", restartButtonCheck);
            DrawButton("exit", exitButtonCheck);
            ButtonNumber = 1;
        }
        void MouseCheck(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);
            //creates rectangles for the buttons which make it possible to check if the button is pressed
            ButtonNumber = 2;
            RestartButtonIntersects = ButtonStateCheck(mouseClickRect, RestartButtonIntersects);
            ExitButtonIntersects = ButtonStateCheck(mouseClickRect, ExitButtonIntersects);
            ButtonNumber = 1;
        }
    }
}
