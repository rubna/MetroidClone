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

        public Sprite(Texture2D texture, Vector2 origin)
        {
            Texture = texture;
            Origin = origin;
        }
    }
}
