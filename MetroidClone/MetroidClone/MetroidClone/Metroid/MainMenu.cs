using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MetroidClone.Metroid
{
    class MainMenu
    {
        public bool ExitGame;
        public bool Initialize;

        DrawWrapper drawing;

        string start = "START", options = "OPTIONS", exit = "EXIT";
        Rectangle startButton, optionsButton, exitButton, cursor;
        Color startColor = Color.DarkSlateGray, optionsColor = Color.DarkSlateGray, exitColor = Color.DarkSlateGray;

        public MainMenu(DrawWrapper Drawing)
        {
            drawing = Drawing;
            ExitGame = false;
            Initialize = false;
        }

        public void Update(GameTime gameTime, InputHelper input)
        {
            startButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 200, 200, 100);
            optionsButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 50, 200, 100);
            exitButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 + 100, 200, 100);
            cursor = new Rectangle(input.MouseCheckPosition().X, input.MouseCheckPosition().Y, 1, 1);
            
            if (!input.ControllerInUse)
            {
                if (cursor.Intersects(startButton))
                {
                    startColor.A = 200;
                    if (input.MouseButtonCheckPressed(true))
                    {
                        Initialize = true;
                    }
                }
                else
                    startColor.A = 255;

                if (cursor.Intersects(optionsButton))
                {
                    optionsColor.A = 200;
                    if (input.MouseButtonCheckPressed(true))
                    {

                    }
                }
                else optionsColor.A = 255;

                if (cursor.Intersects(exitButton))
                {
                    exitColor.A = 200;
                    if (input.MouseButtonCheckPressed(true))
                    {
                        ExitGame = true;
                    }
                }
                else
                    exitColor.A = 255;
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
