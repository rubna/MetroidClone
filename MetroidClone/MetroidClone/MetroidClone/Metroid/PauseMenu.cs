using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// creates the pause menu. A large part of the code is based on code made by Nico Vermeir.
namespace MetroidClone.Metroid
{
    class PauseMenu
    {
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
        }

        public void Update(GameTime gameTime, InputHelper Input)
        {
            resumeButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 200, 200, 100);
            optionsButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 - 50, 200, 100);
            quitButton = new Rectangle((int)drawing.GUISize.X / 2 - 100, (int)drawing.GUISize.Y / 2 + 100, 200, 100);
            cursor = new Rectangle(Input.MouseCheckPosition().X, Input.MouseCheckPosition().Y, 1, 1);

            if (!Input.ControllerInUse)
            {
                if (cursor.Intersects(resumeButton))
                {
                    resumeColor.A = 200;
                    if (Input.MouseButtonCheckPressed(true))
                    {
                        Resume = true;
                    }
                }
                else
                    resumeColor.A = 255;

                if (cursor.Intersects(optionsButton))
                {
                    optionsColor.A = 200;
                    if (Input.MouseButtonCheckPressed(true))
                    {
                        Options = true;
                    }
                }
                else
                    optionsColor.A = 255;

                if (cursor.Intersects(quitButton))
                {
                    quitColor.A = 200;
                    if (Input.MouseButtonCheckPressed(true))
                    {
                        Quit = true;
                    }
                }
                else
                    quitColor.A = 255;
            }
        }

        public void DrawGUI()
        {
            //resume button
            drawing.DrawRectangleUnscaled(resumeButton, resumeColor);
            drawing.DrawText("font18", resume, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2 - 150), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //options button
            drawing.DrawRectangleUnscaled(optionsButton, optionsColor);
            drawing.DrawText("font18", options, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);

            //quit button
            drawing.DrawRectangleUnscaled(quitButton, quitColor);
            drawing.DrawText("font18", quit, new Vector2(drawing.GUISize.X / 2, drawing.GUISize.Y / 2 + 150), Color.White, alignment: Engine.Asset.Font.Alignment.MiddleCenter);
        }
    }
}