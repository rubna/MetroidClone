using MetroidClone.Engine;
using MetroidClone.Metroid.Abstract;
using MetroidClone.Metroid.Monsters;
using Microsoft.Xna.Framework;
using System.Linq;

namespace MetroidClone.Metroid.Player_Attacks
{
    class PlayerExplosion : PhysicsObject, IPlayerAttack
    {
        public float Damage => 20;
        float radiusTarget = 80;
        float radius = 0;
        float radiusSpeed = 0;
        float destroyTimer = 0f;
        float flickerTimer = 0;

        public override void Create()
        {
            base.Create();
            Depth = -10;
            Gravity = 0;
            foreach (Monster monster in World.GameObjects.OfType<Monster>())
            {
                if (monster.Visible && !(monster is Turret))
                {
                    Vector2 offset = (monster.Position - Position).ToPolar();
                    if (offset.X < radiusTarget)
                    {
                        monster.HitPoints -= (1 - offset.X / radiusTarget) * Damage;
                        monster.Speed = new Vector2((1 - offset.X / radiusTarget) * 12 + 4, offset.Y).ToCartesian();
                    }
                }
            }
            //push player away
            Vector2 off = (World.Player.Position - Position).ToPolar();
            if (off.X < radiusTarget)
            {
                World.Player.Speed = new Vector2((1 - off.X / radiusTarget) * 16 + 8, off.Y).ToCartesian();
            }

            //Open doors
            foreach (RocketDoor door in World.GameObjects.OfType<RocketDoor>())
            {
                if (door.Visible)
                {
                    Vector2 offset = (door.Position - Position).ToPolar();
                    if (offset.X < radiusTarget + 10)
                    {
                        door.Activated = true;
                        World.Tutorial.RocketDoorOpened = true;
                    }
                }
            }

            //Sound
            Audio.Play("Audio/Combat/explosion2");
        }

        public override void Update(GameTime gameTime)
        {
            radiusSpeed += (radiusTarget - radius) * 0.2f;
            radius += radiusSpeed;
            radiusSpeed *= 0.5f;

            base.Update(gameTime);
            destroyTimer += 0.05f;
            if (destroyTimer >= 1)
                Destroy();
        }

        public override void Draw()
        {
            flickerTimer += 0.25f;
            flickerTimer %= 2f;
            if (Visible)
            Drawing.DrawCircle(DrawPosition, radius, flickerTimer > 1 ? Color.Black : Color.White);
            base.Draw();
        }
    }
}
