using MetroidClone.Engine;
using MetroidClone.Engine.Asset;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    //Draw the GUI/HUD
    class GUI : GameObject
    {
        public override bool ShouldDrawGUI => true;

        const string fontName = "font16";
        const int yCorrection = -5; //Some fonts are measured wrongly if the measurement is not corrected.

        //Draw a number of lines, containing information about the game.
        public override void DrawGUI()
        {
            Color guiColor = new Color(50, 50, 50, 200);
            List<string> lines = new List<string>();
            List<float?> lineFillAmounts = new List<float?>();
            int guiWidth = 0, guiHeight = 10;

            //Health
            lines.Add("Health: " + World.Player.HitPoints);
            lineFillAmounts.Add((float)World.Player.HitPoints / World.Player.MaxHitPoints);

            //Score
            lines.Add("Score: " + World.Player.Score);
            lineFillAmounts.Add(null);

            //Scrap
            if (World.Tutorial.ScrapCollected)
            {
                lines.Add("Scrap: " + World.Player.CollectedScrap);
                lineFillAmounts.Add(null);
            }

            //Rockets
            if (World.Player.UnlockedWeapons.Contains(Weapon.Rocket))
            {
                lines.Add("Rockets: " + World.Player.RocketAmmo);
                lineFillAmounts.Add(null);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                guiWidth = Math.Max(guiWidth, (int)Drawing.MeasureText(fontName, lines[i]).X + 10);
                guiHeight += (int)Drawing.MeasureText(fontName, lines[i]).Y + yCorrection;
            }

            int currentX = (int)Drawing.GUISize.X - guiWidth - 5;
            int currentY = (int)Drawing.GUISize.Y - guiHeight - 5;

            //Draw background rectangle
            Drawing.DrawRectangleUnscaled(new Rectangle(currentX - 5, currentY - 5, guiWidth, guiHeight), guiColor);
            
            for (int i = 0; i < lines.Count; i++)
            {
                int height = (int)Drawing.MeasureText(fontName, lines[i]).Y + yCorrection;

                string[] splitString = lines[i].Split(' ');
                Color textColor = Color.White;

                //Draw colored background for things like health.
                if (lineFillAmounts[i] != null)
                {
                    Drawing.DrawRectangleUnscaled(new Rectangle(currentX - 5, currentY + 2, guiWidth, height - 4), new Color(255, 80, 80));
                    Drawing.DrawRectangleUnscaled(new Rectangle(currentX - 5, currentY + 2, (int) (guiWidth * (float) lineFillAmounts[i]), height - 4), new Color(150, 255, 80));
                    textColor = Color.Black;
                }

                if (splitString.Length == 1)
                    Drawing.DrawText(fontName, lines[i], new Vector2(currentX, currentY), textColor);
                else
                {
                    Drawing.DrawText(fontName, splitString[0], new Vector2(currentX, currentY), textColor);
                    Drawing.DrawText(fontName, splitString[1], new Vector2(currentX + guiWidth - 10, currentY), textColor, alignment: Font.Alignment.TopRight);
                }
                currentY += height;
            }
        }
    }
}
