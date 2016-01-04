using MetroidClone.Engine.Solids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    /// <summary>
    /// GameObject with physics, collisions and a BoundingBox.
    /// </summary>
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
        protected float Gravity = 0.3f;
        protected bool OnJumpThrough = false;
        protected bool OnGround = false;
        protected bool OnSlope = false;
        protected Vector2 WallBounce = Vector2.Zero;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Speed *= Friction;

            //resolve speeds
            Speed.Y += Gravity;
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
            OnSlope = false;
            OnGround = false;
            foreach (ISolid solid in World.GameObjects.OfType<ISolid>().ToList())
            {
                if (solid == this) continue;

                Rectangle box = TranslatedBoundingBox;
                box.Offset(0, 1);
                if (solid.CollidesWith(box))
                {
                    OnGround = true;
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
            foreach (ISolid solid in World.Solids)
            {
                if (solid == this) continue;

                if (solid.CollidesWith(boundingbox))
                {
                    if (solid is JumpThrough && Speed.Y < 0)
                            continue;
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
        protected bool CollidesWith(float x, float y, PhysicsObject obj)
        {
            return CollidesWith(new Vector2(xOffset, yOffset).ToPoint(), obj);
        }

        /// <summary>
        /// Returns collision with ISolid and a PhysicsObject, if that object would be at position (x, y).
        /// </summary>
        protected bool CollidesWith(Vector2 position, PhysicsObject obj)
        {
            return CollidesWith(offset.ToPoint(), obj);
        }

        /// <summary>
        /// Returns collision with ISolid and a PhysicsObject, if that object would be at position.
        /// </summary>
        protected bool CollidesWith(Point position, PhysicsObject obj)
        {
            Rectangle bbox = BoundingBox;
            bbox.Offset(offset);
            return bbox.Intersects(obj.TranslatedBoundingBox);
        }
    }
}
