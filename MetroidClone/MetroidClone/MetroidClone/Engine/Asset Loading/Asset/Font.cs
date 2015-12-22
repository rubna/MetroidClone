using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MetroidClone.Engine.Asset
{
    // A font asset.
    public class Font : IAsset
    {
        public enum Alignment { TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight }
        public SpriteFont SpriteFont { get; protected set; }

        public Font(SpriteFont font)
        {
            SpriteFont = font;
        }

        public Vector2 Measure(string text)
        {
            return SpriteFont.MeasureString(text);
        }
    }
}
