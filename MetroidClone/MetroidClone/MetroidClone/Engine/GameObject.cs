using MetroidClone.Engine.Asset;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MetroidClone.Engine
{
    class GameObject
    {
        public Vector2 Position = Vector2.Zero; //The position where this object is located in the game world.
        public Vector2 DrawPosition => Position - World.Camera; //The position where this object is drawn.
        public virtual Vector2 CenterPosition => Position;
        public bool FlipX = false;
        public int GetFlip
        {
            get { return FlipX ? -1 : 1; }
        }

        public float Depth = 0;

        //Various useful objects
        public World World { protected get; set; }
        public DrawWrapper Drawing { protected get; set; }
        public AudioWrapper Audio { protected get; set; }
        public AssetManager Assets { protected get; set; }
        protected InputHelper Input;
        public bool Visible = true;

        //Information about the sprite. For objects without a sprite, CurrentSprite is null.
        protected Sprite CurrentSprite = null;
        protected float CurrentImage = 0;
        protected bool AnimationIsRepeating = false;
        protected float AnimationSpeed = 0;
        protected Vector2 ImageScaling;
        protected float ImageRotation = 0f;

        //Event handlers
        public event EventHandler AnimationFinished;

        public virtual bool ShouldUpdate => true; //Whether we should update this object.
        public virtual bool ShouldDrawGUI => false;

        public virtual void Create()
        {
            Input = InputHelper.Instance;
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
                Drawing.DrawSprite(CurrentSprite, DrawPosition, (int) CurrentImage, ImageScaling, null, ImageRotation); //Draw the current image of the sprite.
        }

        public virtual void DrawGUI()
        {
            //Do nothing by default.
        }

        //Plays an animation. 
        public void PlayAnimation(string sprite, float? startImage = null, bool repeat = true, float speed = 1, Vector2? scaling = null)
        {
            CurrentSprite = Assets.GetSprite(sprite);
            if (startImage != null)
                CurrentImage = (float)startImage;
            AnimationIsRepeating = repeat;
            AnimationSpeed = speed;
            ImageScaling = scaling ?? new Vector2(1f);
        }

        //Plays an animation with the option to define the direction instead of the scaling.
        public void PlayAnimation(string sprite, Direction direction, float? startImage = null, bool repeat = true, float speed = 1)
        {
            PlayAnimation(sprite, startImage, repeat, speed, new Vector2(direction == Direction.Right ? 1f: -1f, 1f));
        }

        //A more basic way to set a sprite, which may result in more readable code for some use cases.
        public void SetSprite(string sprite)
        {
            PlayAnimation(sprite);
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
