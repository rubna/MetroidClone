using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetroidClone.Engine
{
    class PhysicsObject : BoxObject
    {
        public bool CollideWithWalls = true;
        public Vector2 Speed = Vector2.Zero;
        Vector2 subPixelSpeed = Vector2.Zero;
        public Vector2 PositionPrevious = Vector2.Zero;
        public Vector2 Friction = new Vector2(0.8f, 1);
        protected float Gravity = 0.3f;
        protected bool OnJumpThrough = false;
        protected bool OnGround = false;
        protected bool OnSlope = false;
        protected Vector2 WallBounce = Vector2.Zero;
        protected bool IgnoreJumpThroughs = false;
        public float SpriteScale = 1f;

        //HadCollision stores whether there was a collision with a wall in the last update.
        public bool HadHCollision = false, HadVCollision = false;
        public Direction LastHCollisionDirection = Direction.Left, LastVCollisionDirection = Direction.Up; //In what direction was the last collision?

        const float maxSpeed = 15; //The maximum speed.

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Speed *= Friction;

            //resolve speeds
            Speed.Y += Gravity;

            //Enforce maximum speed
            if (Speed.X < -maxSpeed)
                Speed.X = -maxSpeed;
            if (Speed.Y < -maxSpeed)
                Speed.Y = -maxSpeed;
            if (Speed.X > maxSpeed)
                Speed.X = maxSpeed;
            if (Speed.Y > maxSpeed)
                Speed.Y = maxSpeed;

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

        void CheckOnGround()
        {
            OnJumpThrough = false;
            OnSlope = false;
            OnGround = false;
            List<ISolid> solids = World.GetNearSolids(Position);
            int numberOfSolids = solids.Count;
            for (int i = 0; i < numberOfSolids; i++)
            {
                ISolid solid = solids[i];
                if (solid == this) continue;

                Rectangle box = TranslatedBoundingBox;
                box.Offset(0, 1); //Move the collision box down.
                if (solid.CollidesWith(box))
                {
                    OnGround = true;
                    if (Speed.Y > 0)
                        Speed.Y = 0;
                    LastVCollisionDirection = Direction.Down;
                    HadVCollision = true;
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
            HadHCollision = false; //We haven't registered a collision yet.
            HadVCollision = false;

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
                    if (!InsideWall(Math.Sign(roundedSpeed.X), -2, TranslatedBoundingBox)) //For slopes
                    {
                        Position.X += Math.Sign(roundedSpeed.X);
                        Position.Y--;
                    }
                    else
                    {
                        if (Speed.X < 0)
                            LastHCollisionDirection = Direction.Left;
                        else if (Speed.X > 0)
                            LastHCollisionDirection = Direction.Right;
                        Speed.X *= -WallBounce.X;
                        HadHCollision = true;
                        break;
                    }
                }
                else
                {
                    Position.X += Math.Sign(roundedSpeed.X);
            }
            }

            //move for Y until collision
            for (int i = 0; i < Math.Abs(roundedSpeed.Y); i++)
            {
                if (InsideWall(0, Math.Sign(roundedSpeed.Y), TranslatedBoundingBox))
                {
                    if (Speed.Y < 0)
                        LastVCollisionDirection = Direction.Up;
                    else if (Speed.Y > 0)
                        LastVCollisionDirection = Direction.Down;
                    Speed.Y *= -WallBounce.Y;
                    HadVCollision = true;
                    break;
                }
                else
                    Position.Y += Math.Sign(roundedSpeed.Y);
            }
        }

        // Get the first collision with the specified type.
        protected ISolid GetCollisionWithSolid<T>(Rectangle boundingbox)
        {
            foreach (ISolid collider in World.GetNearSolids(Position).OfType<T>())
            {
                if (!(collider is JumpThrough && Speed.Y < 0) && collider.CollidesWith(boundingbox) && !(collider == this))
                {
                    return collider;
                }
            }
            return null;
        }

        protected bool InsideWall(Rectangle boundingbox)
        {
            List<ISolid> solids = World.GetNearSolids(boundingbox.Center.ToVector2());
            int numberOfSolids = solids.Count;
            for (int i = 0; i < numberOfSolids; i++)
            {
                //Ignore collisions with jumpthroughs when going up.
                if (!(solids[i] is JumpThrough && (Speed.Y < 0 || IgnoreJumpThroughs)) && solids[i].CollidesWith(boundingbox) && !(solids[i] == this))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether there is collision with ISolid and an offsetted boundingbox.
        /// </summary>
        protected bool InsideWall(Point offset, Rectangle boundingbox)
        {
            Rectangle box = boundingbox;
            box.Offset(offset);
            return InsideWall(box);
        }

        /// <summary>
        /// Returns collision with ISolid and an offsetted boundingbox.
        /// </summary>
        protected bool InsideWall(Vector2 position, Rectangle boundingbox)
        {
            return InsideWall(position.ToPoint(), boundingbox);
        }

        /// <summary>
        /// Returns collision with ISolid and an offsetted boundingbox.
        /// </summary>
        protected bool InsideWall(float x, float y, Rectangle boundingbox)
        {
            return InsideWall(new Point((int)x, (int)y), boundingbox);
        }

        /// <summary>
        /// Returns collision with ISolid and a PhysicsObject, if that object would be at position (x, y).
        /// </summary>
        protected bool CollidesWith(float xOffset, float yOffset, PhysicsObject obj)
        {
            return CollidesWith(new Vector2(xOffset, yOffset).ToPoint(), obj);
        }

        /// <summary>
        /// Returns collision with ISolid and a PhysicsObject, if that object would be at position (x, y).
        /// </summary>
        protected bool CollidesWith(Vector2 offset, PhysicsObject obj)
        {
            return CollidesWith(offset.ToPoint(), obj);
        }

        /// <summary>
        /// Returns collision with ISolid and a PhysicsObject, if that object would be at position.
        /// </summary>
        protected bool CollidesWith(Point offset, PhysicsObject obj)
        {
            Rectangle bbox = BoundingBox;
            bbox.Offset(offset);
            return bbox.Intersects(obj.TranslatedBoundingBox);
        }
    }
}
