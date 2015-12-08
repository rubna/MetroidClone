using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FloppyWieners
{
    public class DrawWrapper
    {
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;
        public Vector2 ScreenSize;
        private BasicEffect basicEffect;
        public float GlobalScale { get; private set; } = 1f;

        bool BeginSpriteBatchCalled = false;

        public DrawWrapper(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            GlobalScale = 1;
            this.spriteBatch = spriteBatch;
            this.graphicsDevice = graphicsDevice;
            ScreenSize = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            basicEffect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.Identity,
            };

            SetProjectionMatrix();
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

        public void DrawRectangle(Vector2 topLeft, Vector2 bottomRight, Color color)
        {
            var verts = new List<Vector2>
            {
                topLeft,
                new Vector2(bottomRight.X, topLeft.Y),
                new Vector2(topLeft.X, bottomRight.Y),
                bottomRight
            };

            DrawPrimitive(PrimitiveType.TriangleStrip, verts, color);

        }
    }
}
