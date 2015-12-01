using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MetroidClone.Engine.Asset;

namespace MetroidClone.Engine
{
    public class DrawWrapper
    {
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;
        
        public Vector2 ScreenSize { get; private set; }
        public bool BeginSpriteBatchCalled { get; private set; }

        public float GlobalScale { get; set; }

        private BasicEffect basicEffect;

        private int deviceWidth, lastDeviceWidth;

        public AssetManager Assets;

        public DrawWrapper(SpriteBatch batch, GraphicsDevice device, AssetManager assetsManager)
        {
            GlobalScale = 2f;

            spriteBatch = batch;
            graphicsDevice = device;
            deviceWidth = graphicsDevice.Viewport.Width;

            Assets = assetsManager;

            basicEffect = new BasicEffect(device)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.Identity,
            };

            SetProjectionMatrix();
            ScreenSize = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        }

        public void DrawPrimitive(PrimitiveType primitiveType, IEnumerable<Vector2> vertices, Color color)
        {
            DrawPrimitive(primitiveType, vertices.Select(v => new VertexPositionColor(new Vector3(GlobalScale * v, 0f), color)));
        }

        private void DrawPrimitive(PrimitiveType primitiveType, IEnumerable<VertexPositionColor> vertices)
        {
            EndSpriteBatch();
            basicEffect.CurrentTechnique.Passes[0].Apply();

            var data = vertices.ToArray();

            graphicsDevice.DrawUserPrimitives(primitiveType, data, 0, GetPrimitiveCount(primitiveType, data.Length));
        }

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            var verts = new List<Vector2>
            {
                new Vector2(rectangle.X, rectangle.Bottom),
                new Vector2(rectangle.X, rectangle.Y),
                new Vector2(rectangle.Right, rectangle.Bottom),
                new Vector2(rectangle.Right, rectangle.Y)
            };

            DrawPrimitive(PrimitiveType.TriangleStrip, verts, color);
        }

        public void DrawCircle(Vector2 position, float radius, Color color, int precision = 24)
        {
            if (precision <= 0)
                return;
            List<Vector2> verts = new List<Vector2>();
            double circleStep = 1f / precision * 2f * Math.PI;
            for (int i = 0; i <= precision; i++)
            {
                verts.Add(position); //Add the center
                //Add the point on the circle
                verts.Add(position + radius * new Vector2((float)Math.Cos(i * circleStep), (float)Math.Sin(i * circleStep)));
            }
            DrawPrimitive(PrimitiveType.TriangleStrip, verts, color);
        }

        public void DrawSprite(Sprite sprite, Vector2 position, Vector2? subimage = null, Vector2? size = null, Color? color = null, float rotation = 0f)
        {
            BeginSpriteBatch();

            Vector2 usedSize = size ?? sprite.Size; //The used size is either the specified size or the default size of the sprite.

            //Make it possible to mirror or flip the sprite by using a negative size.
            SpriteEffects usedSpriteEffect = SpriteEffects.None;
            if (usedSize.X < 0)
            {
                usedSize.X = Math.Abs(usedSize.X);
                usedSpriteEffect = usedSpriteEffect | SpriteEffects.FlipHorizontally;
            }
            if (usedSize.Y < 0)
            {
                usedSize.Y = Math.Abs(usedSize.Y);
                usedSpriteEffect = usedSpriteEffect | SpriteEffects.FlipVertically;
            }

            //Draw the given subimage of the sprite with the given parameters.
            //The position and scaling are affected by the global scaling.
            //Some parameters are optional, so they are set to the default if not specified.
            spriteBatch.Draw(sprite.Texture, GlobalScale * position, sprite.GetImageRectangle(subimage ?? new Vector2(0f, 0f)),
                color ?? Color.White, rotation, sprite.Origin * sprite.Size, usedSize / sprite.Size * GlobalScale, usedSpriteEffect, 0f);
        }

        public void DrawSprite(string sprite, Vector2 position, Vector2? subimage = null, Vector2? size = null, Color? color = null, float rotation = 0f)
        {
            DrawSprite(Assets.GetSprite(sprite), position, subimage, size, color, rotation);
        }

        public void DrawSprite(Sprite sprite, Vector2 position, float subimage, Vector2? size = null, Color? color = null, float rotation = 0f)
        {
            DrawSprite(sprite, position, new Vector2(subimage % sprite.SheetSize.X, (int) subimage / (int) sprite.SheetSize.X), size, color, rotation);
        }

        public void DrawSprite(string sprite, Vector2 position, float subimage, Vector2? size = null, Color? color = null, float rotation = 0f)
        {
            Sprite drawSprite = Assets.GetSprite(sprite);
            DrawSprite(drawSprite, position, new Vector2(subimage % drawSprite.SheetSize.X, (int)subimage / (int)drawSprite.SheetSize.X), size, color, rotation);
        }

        private void BeginSpriteBatch()
        {
            if (!BeginSpriteBatchCalled)
            {
                BeginSpriteBatchCalled = true;
                spriteBatch.Begin();
            }
        }

        private void EndSpriteBatch()
        {
            if (BeginSpriteBatchCalled)
            {
                BeginSpriteBatchCalled = false;
                spriteBatch.End();
            }
        }

        public void EndOfDraw()
        {
            EndSpriteBatch();
        }

        private int GetPrimitiveCount(PrimitiveType primitiveType, int count)
        {
            switch (primitiveType)
            {
                case PrimitiveType.TriangleList:
                    return count / 3;
                case PrimitiveType.LineList:
                    return count / 2;
                case PrimitiveType.LineStrip:
                case PrimitiveType.TriangleStrip:
                    return count - 2;
            }

            return -1;
        }

        public void SetProjectionMatrix()
        {
            var projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
            var halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            basicEffect.Projection = halfPixelOffset * projection;
        }

        
    }
}
