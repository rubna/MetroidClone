using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    // The hackThisGame system makes it possible for people who watch the game to change things about the game.
    // This makes watching the game of another player more interesting.

    class HackThisGame : GameObject
    {
        const string baseURI = "http://wagtailgames.com/HiddenHorseEntertainment/";
        const string connectURI = baseURI + "connect.php"; //The URI to register the game to the system.
        const string setOptionsURI = baseURI + "set_options.php"; //The URI to set the current options of the game.
        const string getResultURI = baseURI + "get_result.php"; //The URI to get the result of options.

        public HackThisGame()
        {
            
        }

        public override void DrawGUI()
        {
            const string basicInfo = "Game ID:";
            Drawing.DrawRectangleUnscaled(new Rectangle(10, (int) Drawing.GUISize.Y - 80, (int) Drawing.MeasureText("font14", basicInfo).X + 20, 70),
                new Color(50, 50, 50, 200));
            Drawing.DrawText("font14", basicInfo, new Vector2(20, (int)Drawing.GUISize.Y - 75), Color.White);
            Drawing.DrawText("font22", "BBBA", new Vector2(20, (int)Drawing.GUISize.Y - 50), Color.White);
        }
    }
}
