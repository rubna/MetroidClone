using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        public DrawWrapper(SpriteBatch batch, GraphicsDevice device)
        {
            GlobalScale = 2f;

            spriteBatch = batch;
            graphicsDevice = device;
            deviceWidth = graphicsDevice.Viewport.Width;

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
