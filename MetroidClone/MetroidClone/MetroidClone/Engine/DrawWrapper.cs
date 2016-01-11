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
        const int standardWidth = 24 * 20 * 2;
        const int standardHeight = 24 * 15 * 2;

        private SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;
        
        public Vector2 ScreenSize { get; private set; }
        public bool BeginSpriteBatchCalled { get; private set; }

        public float GlobalScale { get; set; }

        private BasicEffect basicEffect, guiEffect;
        private BasicEffect currentEffect;

        private int deviceWidth, lastDeviceWidth;

        //The size of the whole game view (excluding black bars), and the size of the GUI (including black bars).
        float displayLeft = 0, displayTop = 0, displayWidth = 0, displayHeight = 0;
        public Vector2 GUISize { get { return new Vector2(displayWidth + displayLeft * 2, displayHeight + displayTop * 2); } }

        public AssetManager Assets;

        public DrawWrapper(SpriteBatch batch, GraphicsDevice device, AssetManager assetsManager)
        {
            GlobalScale = 1f;

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

            guiEffect = (BasicEffect) basicEffect.Clone();

            SetProjectionMatrix(standardWidth, standardHeight);
            ScreenSize = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            displayWidth = standardWidth;
            displayHeight = standardHeight;
        }

        public void DrawPrimitive(PrimitiveType primitiveType, IEnumerable<Vector2> vertices, Color color)
        {
            DrawPrimitive(primitiveType, vertices.Select(v => new VertexPositionColor(new Vector3(GlobalScale * v, 0f), color)));
        }

        private void DrawPrimitive(PrimitiveType primitiveType, IEnumerable<VertexPositionColor> vertices)
        {
            EndSpriteBatch();

            currentEffect.CurrentTechnique.Passes[0].Apply();

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

        public void DrawRectangleUnscaled(Rectangle rectangle, Color color)
        {
            DrawRectangle(new Rectangle((int) (rectangle.Left / GlobalScale), (int) (rectangle.Top / GlobalScale), (int) (rectangle.Width / GlobalScale),
                (int) (rectangle.Height / GlobalScale)), color);
        }

        public void DrawCircle(Vector2 position, float radius, Color color, int precision = 24)
        {
            if (precision <= 0)
                return;
            List<Vector2> verts = new List<Vector2>();
            float circleStep = 1f / precision * 360;
            for (int i = 0; i <= precision; i++)
            {
                verts.Add(position); //Add the center
                //Add the point on the circle
                verts.Add(position + new Vector2(radius, i * circleStep).ToCartesian());
            }
            DrawPrimitive(PrimitiveType.TriangleStrip, verts, color);
        }

        public void DrawTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color color)
        {
            List<Vector2> verts = new List<Vector2>() { v1, v2, v3 };

            DrawPrimitive(PrimitiveType.TriangleList, verts, color);
        }

        public void DrawLine(Vector2 from, Vector2 to, float width, Color color)
        {
            Vector2 offset = new Vector2(width / 2, (to - from).Angle() + 90).ToCartesian();
            List<Vector2> verts = new List<Vector2>()
            {
                from + offset,
                from - offset,
                to + offset,
                to - offset
            };

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
            spriteBatch.Draw(sprite.Texture, GlobalScale * position + new Vector2(displayLeft, displayTop),
                sprite.GetImageRectangle(subimage ?? new Vector2(0f, 0f)), color ?? Color.White, rotation,
                sprite.Origin * sprite.Size, usedSize / sprite.Size * GlobalScale, usedSpriteEffect, 0f);
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

        //Methods to draw text.

        public void DrawText(Font font, string text, Vector2 position, Color? color = null, float rotation = 0f, Vector2? origin = null, float scale = 1f, Font.Alignment alignment = Font.Alignment.TopLeft)
        {
            BeginSpriteBatch();

            Vector2 finalOrigin = origin ?? new Vector2(0f, 0f);

            //Apply the alignment.
            if (alignment != Font.Alignment.TopLeft)
            {
                Vector2 textMeasure = font.Measure(text);
                if (alignment == Font.Alignment.MiddleLeft || alignment == Font.Alignment.MiddleCenter || alignment == Font.Alignment.MiddleRight)
                    finalOrigin.Y += textMeasure.Y / 2;
                if (alignment == Font.Alignment.BottomLeft || alignment == Font.Alignment.BottomCenter || alignment == Font.Alignment.BottomRight)
                    finalOrigin.Y += textMeasure.Y;
                if (alignment == Font.Alignment.TopCenter || alignment == Font.Alignment.MiddleCenter || alignment == Font.Alignment.BottomCenter)
                    finalOrigin.X += textMeasure.X / 2;
                if (alignment == Font.Alignment.TopRight || alignment == Font.Alignment.MiddleRight || alignment == Font.Alignment.BottomRight)
                    finalOrigin.X += textMeasure.X;
            }

            spriteBatch.DrawString(font.SpriteFont, text, position, color ?? Color.Black, rotation, finalOrigin, scale, SpriteEffects.None, 0f);
        }

        public void DrawText(string font, string text, Vector2 position, Color? color = null, float rotation = 0f, Vector2? origin = null, float scale = 1f, Font.Alignment alignment = Font.Alignment.TopLeft)
        {
            BeginSpriteBatch();

            DrawText(Assets.GetFont(font), text, position, color, rotation, origin, scale, alignment);
        }

        public Vector2 MeasureText(Font font, string text)
        {
            return font.Measure(text);
        }

        public Vector2 MeasureText(string font, string text)
        {
            return MeasureText(Assets.GetFont(font), text);
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

        public void BeginDraw()
        {
            currentEffect = basicEffect;
        }

        public void BeginDrawGUI()
        {
            EndSpriteBatch();

            //Draw the GUI/HUD
            currentEffect = guiEffect;
            DrawBlackBars(); //Black bars around the view.
        }

        public void EndOfDraw()
        {
            EndSpriteBatch(); //End the sprite batch.

            currentEffect = basicEffect; //Then reset the draw effect.
        }

        //Draw black bars around the view
        protected void DrawBlackBars()
        {
            DrawRectangleUnscaled(new Rectangle(0, 0, (int)displayLeft, (int)displayHeight), Color.Black);
            DrawRectangleUnscaled(new Rectangle(0, 0, (int)displayWidth, (int)displayTop), Color.Black);
            DrawRectangleUnscaled(new Rectangle((int)displayWidth + (int) displayLeft, 0, (int)displayLeft, (int)displayHeight), Color.Black);
            DrawRectangleUnscaled(new Rectangle(0, (int)displayHeight + (int) displayTop, (int)displayWidth, (int)displayTop), Color.Black);
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

        public void SetProjectionMatrix(int width, int height)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);
            
            Matrix offset = Matrix.CreateTranslation(-0.5f + displayLeft, -0.5f + displayTop, 0);
            basicEffect.Projection = offset * projection;

            Matrix offset2 = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            guiEffect.Projection = offset2 * projection;
        }

        // Scales the game to a certain width and height.
        public void SmartScale(int width, int height)
        {
            float maxWidth = width, maxHeight = height;

            GlobalScale = Math.Min(maxWidth / standardWidth, maxHeight / standardHeight);
            
            displayWidth = maxWidth * GlobalScale / (maxWidth / standardWidth);
            displayHeight = maxHeight * GlobalScale / (maxHeight / standardHeight);
            displayLeft = (width - displayWidth) / 2;
            displayTop = (height - displayHeight) / 2;
        
            SetProjectionMatrix(width, height);
        }
    }
}
