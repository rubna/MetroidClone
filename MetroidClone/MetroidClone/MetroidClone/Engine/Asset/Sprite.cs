using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MetroidClone.Engine.Asset
{
    public class Sprite : IAsset
    {
        public Texture2D Texture { get; protected set; }
        public Vector2 Origin { get; protected set; }
        public Vector2 SheetSize { get; protected set; }
        public float Width { get { return Texture.Width / SheetSize.X; } } //The width of a subimage.
        public float Height { get { return Texture.Height / SheetSize.Y; } } //The height of a subimage.
        public float FullWidth { get { return Texture.Width; } } //The width of the whole sprite.
        public float FullHeight { get { return Texture.Height; } } //The height of the whole sprite.

        public Sprite(Texture2D texture, Vector2? origin = null, Vector2? sheetsize = null)
        {
            Texture = texture;
            Origin = origin ?? new Vector2(0.5f, 0.5f);
            SheetSize = sheetsize ?? new Vector2(1f, 1f);
        }

        public Rectangle GetImageRectangle(Vector2 subimage)
        {

            return new Rectangle((int) (FullWidth * subimage.X), (int) (FullHeight * subimage.Y),
                (int) (FullWidth * (subimage.X + 1)), (int) (FullHeight * (subimage.Y + 1)));
        }
    }
}
