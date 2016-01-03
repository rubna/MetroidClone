using MetroidClone.Engine.Asset;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MetroidClone.Engine
{
    /// <summary>
    /// Main class for Updated and Drawn objects in the game. Have access to World, Drawing, Assets, InputHelper. 
    /// </summary>
    class GameObject
    {
        public Vector2 Position = Vector2.Zero;
        public bool FlipX = false;
        public int GetFlip
        {
            get
            {
                if (FlipX)
                    return -1;
                else
                    return 1;
            }
        }

        public float Depth = 0;

        //Access grants to World, DrawWrapper, AssetManager and InputHelper.
        public World World;
        public DrawWrapper Drawing;
        public AssetManager Assets;
        protected InputHelper Input;
        public bool Visible = true;

        //Information about the sprite. For objects without a sprite, CurrentSprite is null.
        protected Sprite CurrentSprite;
        protected float CurrentImage;
        protected bool AnimationIsRepeating;
        protected float AnimationSpeed;
        public Vector2 ImageScaling { get; set; } = Vector2.One;
        public float ImageRotation { get; set; } = 0;

        //Event handlers
        public event EventHandler AnimationFinished;

        //Enum
        protected enum Direction { Left, Right };

        public virtual void Create()
        {
            Input = InputHelper.Instance;

            CurrentSprite = null;
            CurrentImage = 0;
            AnimationIsRepeating = false;
            AnimationSpeed = 0;
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateAnimation();
        }

        protected void UpdateAnimation()
        {
            if (CurrentSprite?.ImageNumber > 1) //If the current sprite isn't null, check if it should be animated.
            {
                CurrentImage += AnimationSpeed;
                if (CurrentImage >= CurrentSprite.ImageNumber)
                {
                    CurrentImage -= CurrentSprite.ImageNumber;
                    OnAnimationFinished(EventArgs.Empty);
                }
            }
        }

        public virtual void Draw()
        {
            if (CurrentSprite != null && Visible)
                Drawing.DrawSprite(CurrentSprite, Position, (int) CurrentImage, ImageScaling, null, ImageRotation); //Draw the current image of the sprite.
        }

        //Plays an animation. 
        protected void PlayAnimation(string sprite, float? startImage = null, bool repeat = true, float speed = 1, Vector2? scaling = null)
        {
            CurrentSprite = Assets.GetSprite(sprite);
            if (startImage != null)
                CurrentImage = (float)startImage;
            AnimationIsRepeating = repeat;
            AnimationSpeed = speed;
            ImageScaling = scaling ?? new Vector2(1f); ;
        }

        //Plays an animation with the option to define the direction instead of the scaling.
        protected void PlayAnimation(string sprite, Direction direction, float? startImage = null, bool repeat = true, float speed = 1)
        {
            PlayAnimation(sprite, startImage, repeat, speed, new Vector2(direction == Direction.Right ? 1f: -1f, 1f));
        }

        //Methods to call when events occur
        protected virtual void OnAnimationFinished(EventArgs e)
        {
            if (AnimationFinished != null)
                AnimationFinished(this, e);
        }

        public virtual void Destroy()
        {
            World.RemoveObject(this);
        }
    }
}
