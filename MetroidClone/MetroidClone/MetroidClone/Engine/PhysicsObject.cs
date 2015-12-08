﻿using MetroidClone.Engine.Solids;
using MetroidClone.Metroid;
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
        public Rectangle BoundingBox { get; protected set; }
        public Rectangle TranslatedBoundingBox
        {
            get
            {
                Rectangle translatedBoundingBox = BoundingBox;
                translatedBoundingBox.Offset(Position.ToPoint());
                return translatedBoundingBox;
            }
        }
        public bool CollideWithWalls = true;
        public Vector2 Speed = Vector2.Zero;
        Vector2 subPixelSpeed = Vector2.Zero;
        public Vector2 PositionPrevious = Vector2.Zero;
        protected Vector2 Friction = new Vector2(0.8f, 1);
        protected float Gravity = 0.2f;
        protected bool OnJumpThrough = false;
        protected bool OnGround = false;
        protected Vector2 WallBounce = Vector2.Zero;

        const float maxYSpeed = 15; //The maximum vertical speed.

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Speed *= Friction;

            //resolve speeds
            Speed.Y += Gravity;
            if (Speed.Y > maxYSpeed)
                Speed.Y = maxYSpeed;
            PositionPrevious = Position;

            //check collision
            if (CollideWithWalls)
            {
                MoveCheckingWallCollision();
                CheckOnGround();
            }
            else
                Position += Speed;

        }

        public override void Draw()
        {
            //Draw the current image of the sprite. By default, the size of the bounding box is used.
            if (CurrentSprite != null && Visible)
                Drawing.DrawSprite(CurrentSprite, Position, (int)CurrentImage, ImageScaling * new Vector2(BoundingBox.Width, BoundingBox.Height));
        }

        void CheckOnGround()
        {
            OnJumpThrough = false;
            OnGround = false;
            foreach (ISolid solid in World.GameObjects.OfType<ISolid>().ToList())
            {
                Rectangle box = TranslatedBoundingBox;
                box.Offset(0, 1); //Move the collision box down.
                if (solid.CollidesWith(box))
                {
                    OnGround = true;
                    Speed.Y = 0;
                    if (solid is JumpThrough)
                        OnJumpThrough = true;
                    else
                    {
                        OnJumpThrough = false;
                        break;
                    }
                }
            }
        }

        void MoveCheckingWallCollision()
        {
            //subPixelSpeed saved for the next frame
            Point roundedSpeed;
            subPixelSpeed += Speed;
            roundedSpeed = new Point((int)Math.Round(subPixelSpeed.X), (int)Math.Round(subPixelSpeed.Y));
            subPixelSpeed -= roundedSpeed.ToVector2();

            //move for X until collision
            for (int i = 0; i < Math.Abs(roundedSpeed.X); i++)
            {
                if (InsideWall(Math.Sign(roundedSpeed.X), 0, TranslatedBoundingBox))
                {
                    if (!InsideWall(Math.Sign(roundedSpeed.X), -1, TranslatedBoundingBox))
                    {
                        Position.X += Math.Sign(roundedSpeed.X);
                        Position.Y--;
                    }
                    else
                    {
                        Speed.X *= -WallBounce.X;
                        break;
                    }
                }
                else
                    Position.X += Math.Sign(roundedSpeed.X);
            }

            //move for Y until collision
            for (int i = 0; i < Math.Abs(roundedSpeed.Y); i++)
            {
                if (InsideWall(0, Math.Sign(roundedSpeed.Y), TranslatedBoundingBox))
                {
                    Speed.Y *= -WallBounce.Y;
                    break;
                }
                else
                    Position.Y += Math.Sign(roundedSpeed.Y); 
            }
        }

        protected bool InsideWall(Rectangle boundingbox)
        {
            //foreach (ISolid solid in World.Solids)
            List<ISolid> solids = World.Solids;
            int numberOfSolids = solids.Count;
            for (int i = 0; i < numberOfSolids; i++)
            {
                //Ignore collisions with jumpthroughs when going up.
                if (!(solids[i] is JumpThrough && Speed.Y < 0) && solids[i].CollidesWith(boundingbox))
                {
                    return true;
                }
            }
            return false;
        }

        protected bool InsideWall(Point position, Rectangle boundingbox)
        {
            Rectangle box = boundingbox;
            box.Offset(position);
            return InsideWall(box);
        }

        protected bool InsideWall(Vector2 position, Rectangle boundingbox)
        {
            return InsideWall(position.ToPoint(), boundingbox);
        }

        protected bool InsideWall(float x, float y, Rectangle boundingbox)
        {
            return InsideWall(new Point((int)x, (int)y), boundingbox);
        }

        protected bool CollidesWith(float x, float y, PhysicsObject obj)
        {
            return CollidesWith(new Vector2(x, y).ToPoint(), obj);
        }

        protected bool CollidesWith(Vector2 position, PhysicsObject obj)
        {
            return CollidesWith(position.ToPoint(), obj);
        }

        protected bool CollidesWith(Point position, PhysicsObject obj)
        {
            Rectangle bbox = BoundingBox;
            bbox.Offset(position);
            return bbox.Intersects(obj.TranslatedBoundingBox);
        }
    }
}
