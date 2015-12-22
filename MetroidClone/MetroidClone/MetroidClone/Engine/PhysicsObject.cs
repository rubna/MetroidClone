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

        protected Rectangle DrawBoundingBox { get {
            return new Rectangle(TranslatedBoundingBox.Left - (int)World.Camera.X, TranslatedBoundingBox.Top - (int)World.Camera.Y,
                TranslatedBoundingBox.Width, TranslatedBoundingBox.Height); } }

        //HadCollision stores whether there was a collision with a wall in the last update.
        public bool HadHCollision = false, HadVCollision = false;
        public Engine.Direction LastHCollisionDirection = Engine.Direction.Left, LastVCollisionDirection = Engine.Direction.Up; //In what direction was the last collision?

        const float maxSpeed = 15; //The maximum speed.

        public override void Update(GameTime gameTime)
        {
            MainGame.Profiler.LogEventStart("PhysicsObject Update");

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

            MainGame.Profiler.LogEventEnd("PhysicsObject Update");

        }

        public override void Draw()
        {
            //Draw the current image of the sprite. By default, the size of the bounding box is used.
            if (CurrentSprite != null && Visible)
                Drawing.DrawSprite(CurrentSprite, DrawPosition, (int)CurrentImage, ImageScaling * new Vector2(BoundingBox.Width, BoundingBox.Height));
        }

        void CheckOnGround()
        {
            OnJumpThrough = false;
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
                    LastVCollisionDirection = Engine.Direction.Down;
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
                    if (!InsideWall(Math.Sign(roundedSpeed.X), -1, TranslatedBoundingBox))
                    {
                        Position.X += Math.Sign(roundedSpeed.X);
                        Position.Y--;
                    }
                    else
                    {
                        if (Speed.X < 0)
                            LastHCollisionDirection = Engine.Direction.Left;
                        else if (Speed.X > 0)
                            LastHCollisionDirection = Engine.Direction.Right;
                        Speed.X *= -WallBounce.X;
                        HadHCollision = true;
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
                    if (Speed.Y < 0)
                        LastVCollisionDirection = Engine.Direction.Up;
                    else if (Speed.Y > 0)
                        LastVCollisionDirection = Engine.Direction.Down;
                    Speed.Y *= -WallBounce.Y;
                    HadVCollision = true;
                    break;
                }
                else
                    Position.Y += Math.Sign(roundedSpeed.Y);
            }
        }

        protected bool InsideWall(Rectangle boundingbox)
        {
            List<ISolid> solids = World.GetNearSolids(Position);
            int numberOfSolids = solids.Count;
            for (int i = 0; i < numberOfSolids; i++)
            {
                //Ignore collisions with jumpthroughs when going up.
                if (!(solids[i] is JumpThrough && Speed.Y < 0) && solids[i].CollidesWith(boundingbox) && !(solids[i] == this))
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

        protected bool CollidesWith(float xOffset, float yOffset, PhysicsObject obj)
        {
            return CollidesWith(new Vector2(xOffset, yOffset).ToPoint(), obj);
        }

        protected bool CollidesWith(Vector2 offset, PhysicsObject obj)
        {
            return CollidesWith(offset.ToPoint(), obj);
        }

        protected bool CollidesWith(Point offset, PhysicsObject obj)
        {
            Rectangle bbox = BoundingBox;
            bbox.Offset(offset);
            return bbox.Intersects(obj.TranslatedBoundingBox);
        }
    }
}
