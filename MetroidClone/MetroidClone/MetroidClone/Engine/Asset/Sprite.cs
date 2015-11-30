using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MetroidClone.Engine.Asset
{
    class Sprite : IAsset
    {
        public Texture2D Texture { get; protected set; }
        public Vector2 Origin { get; protected set; }
        public Vector2 SheetSize { get; protected set; }

        public Sprite(Texture2D texture, Vector2? origin = null, Vector2? sheetsize = null)
        {
            Texture = texture;
            Origin = origin ?? new Vector2(0.5f, 0.5f);
            SheetSize = sheetsize ?? new Vector2(1f, 1f);
        }
    }
}
