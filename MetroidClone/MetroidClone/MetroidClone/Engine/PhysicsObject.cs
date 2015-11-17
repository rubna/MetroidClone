using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    class PhysicsObject : GameObject
    {
        protected Rectangle OriginalBoundingBox;

        public Rectangle BoundingBox
        {
            get
            {
                Rectangle translatedBoundingBox = OriginalBoundingBox;
                translatedBoundingBox.Offset(Position.ToPoint());
                return translatedBoundingBox;
            }
        }
        public bool CheckWallCollision = true;
        public Vector2 Speed = Vector2.Zero;
        float xFriction = 0.8f;
        float gravity = 0.2f;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //move around
            if (Input.KeyboardCheckDown(Keys.Left))
                Speed.X -= 0.5f;
            if (Input.KeyboardCheckDown(Keys.Right))
                Speed.X += 0.5f;
            if (Input.KeyboardCheckPressed(Keys.X))
                Speed.Y = -4;

            Speed.X *= xFriction;

            //resolve speeds
            Speed.Y += gravity;
            Position += Speed;

            //check collision
            Level level = World.Level;
            Vector2 lol = Vector2.Zero;

            Point gridPosition = (new Vector2(Position.X / level.TileSize.X, Position.Y / level.TileSize.Y)).ToPoint();
            Point min = new Point(gridPosition.X - 3, gridPosition.Y - 3);
            Point max = new Point(gridPosition.X + 3, gridPosition.Y + 3);

            min = min.ClampPoint(Point.Zero, level.LevelDimensions);
            max = max.ClampPoint(Point.Zero, level.LevelDimensions);

            for (int xp = min.X; xp < max.X; xp++)
                for (int yp = min.Y; yp < max.Y; yp++)
                    if (level.Grid[xp,yp])
                    {
                        Rectangle tile = new Rectangle(xp * level.TileSize.X, yp * level.TileSize.Y, level.TileSize.X, level.TileSize.Y);
                        
                        if (BoundingBox.Intersects(tile))
                        {
                            if (BoundingBox.Top > tile.Center.Y)
                            {
                                Position.Y = tile.Bottom;
                                Speed.Y = 0;
                            }
                            else
                            if (BoundingBox.Bottom < tile.Center.Y)
                            {
                                Position.Y = tile.Top - BoundingBox.Height;
                                Speed.Y = 0;
                            }
                            else
                            if (BoundingBox.Left > tile.Center.X)
                            {
                                Position.X = tile.Right;
                                Speed.X = 0;
                            }
                            else
                            if (BoundingBox.Right < tile.Center.X)
                            {
                                Position.X = tile.Left - BoundingBox.Width;
                                Speed.X = 0;
                            }
                        }
                    }
        }
    }
}
