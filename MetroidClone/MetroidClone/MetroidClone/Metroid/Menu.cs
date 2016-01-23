using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// creates a menu. A large part of the code is based on code made by Nico Vermeir. link: http://www.spikie.be/blog/page/Building-a-main-menu-and-loading-screens-in-XNA.aspx
namespace MetroidClone.Metroid
{
    class Menu : GameObject
    {
        protected bool StartGame = false;
        protected bool GoToOptions = false;
        public bool ExitGame = false;
        protected bool ExitMenu = false;
        protected bool ResumeGame = false;
        public bool FullScreen = false;
        public bool Paused = false;
        protected int ButtonNumber = 1;


        public virtual void Update2(GameTime gameTime)
        {
        }
        public virtual void Draw2()
        {
        }
        protected void DrawButton(string buttonName, int i, int m = 0)
        {
            //Draws a button
            Vector2 TextSize = Drawing.MeasureText("font22", buttonName);
            Vector2 ButtonTextPosition = new Vector2(100, 50) - new Vector2(TextSize.X / 2, TextSize.Y / 2);
            if (!FullScreen)
            {
                Drawing.DrawRectangle(new Rectangle((int)(Drawing.GUISize.X / 2) - 100, (int)(Drawing.ScreenSize.Y / 2) - 200 + 150 * ButtonNumber, 200, 100), new Color(0, 0, 0, 0.5f * i));
                Drawing.DrawText("font22", buttonName, new Vector2((int)(Drawing.GUISize.X / 2), (int)(Drawing.ScreenSize.Y / 2) - 200 + 150 * ButtonNumber + ButtonTextPosition.Y), Color.White, alignment: Engine.Asset.Font.Alignment.TopCenter);
            }
            else
            {
                Drawing.DrawRectangle(new Rectangle((int)(Drawing.GUISize.X / 4) - 100, (int)(Drawing.ScreenSize.Y / 2) - 200 + 150 * ButtonNumber, 200, 100), new Color(0, 0, 0, 0.5f * i));
                Drawing.DrawText("font22", buttonName, new Vector2((int)(Drawing.GUISize.X / 2), (int)(Drawing.ScreenSize.Y / 1.7f) - 185 + 210 * ButtonNumber + ButtonTextPosition.Y), Color.White, 0, null, 2, alignment: Engine.Asset.Font.Alignment.TopCenter);
            }
                ButtonNumber++;
        }
        protected bool ButtonStateCheck(Rectangle mouse, bool b)
        {
            // checks if the mouse intersects with a button
            Rectangle button;
            if (!FullScreen)
            {
                button = new Rectangle((int)(Drawing.GUISize.X / 2) - 100 , (int)(Drawing.ScreenSize.Y / 2) - 200 + 150 * ButtonNumber, 200, 100);
             }
            else
            {
                button = new Rectangle((int)(Drawing.GUISize.X / 2) - 145, (int)(Drawing.ScreenSize.Y / 2) - 200 + 235 * ButtonNumber, 290, 150);
            }
            ButtonNumber++;
            if (mouse.Intersects(button))
                b = true;
            else
                b = false;
            return b;
        }
    }
}

