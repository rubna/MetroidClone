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
        public bool CollideWithWalls = true;
        public Vector2 Speed = Vector2.Zero;
        Vector2 subPixelSpeed = Vector2.Zero;
        public Vector2 PositionPrevious = Vector2.Zero;
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
            if (Input.KeyboardCheckPressed(Keys.Up))
                Speed.Y = -4.5f;

            Speed.X *= xFriction;

            //resolve speeds
            Speed.Y += gravity;
            PositionPrevious = Position;

            //check collision
            if (CollideWithWalls)
                CheckWallCollision();
            else
                Position += Speed;

        }

        void CheckWallCollision()
        {
            //subPixelSpeed saved for the next frame
            Point roundedSpeed;
            subPixelSpeed += Speed;
            roundedSpeed = new Point((int)Math.Round(Speed.X), (int)Math.Round(Speed.Y));
            subPixelSpeed -= roundedSpeed.ToVector2();

            //move for X until collision
            for (int i = 0; i < Math.Abs(roundedSpeed.X); i++)
            {
                Rectangle nextBoundingBox = BoundingBox;
                nextBoundingBox.Offset(new Point(Math.Sign(roundedSpeed.X), 0));
                if (InsideWall(nextBoundingBox))
                {
                    Speed.X = 0;
                    break;
                }
                else
                    Position.X += Math.Sign(roundedSpeed.X);
            }

            //move for Y until collision
            for (int i = 0; i < Math.Abs(roundedSpeed.Y); i++)
            {
                Rectangle nextBoundingBox = BoundingBox;
                nextBoundingBox.Offset(new Point(0, Math.Sign(roundedSpeed.Y)));
                if (InsideWall(nextBoundingBox))
                {
                    Speed.Y = 0;
                    break;
                }
                else
                    Position.Y += Math.Sign(roundedSpeed.Y);
            }
        }

        bool InsideWall(Rectangle boundingbox)
        {
            Level level = World.Level;

            Point gridPosition = (new Vector2(Position.X / level.TileSize.X, Position.Y / level.TileSize.Y)).ToPoint();
            Point min = new Point(gridPosition.X - 3, gridPosition.Y - 3);
            Point max = new Point(gridPosition.X + 3, gridPosition.Y + 3);

            min = min.ClampPoint(Point.Zero, level.LevelDimensions);
            max = max.ClampPoint(Point.Zero, level.LevelDimensions);

            for (int xp = min.X; xp < max.X; xp++)
                for (int yp = min.Y; yp < max.Y; yp++)
                    if (level.Grid[xp, yp])
                    {
                        Rectangle tile = new Rectangle(xp * level.TileSize.X, yp * level.TileSize.Y, level.TileSize.X, level.TileSize.Y);
                        if (boundingbox.Intersects(tile))
                            return true;
                    }

            return false;
        }
    }
}
