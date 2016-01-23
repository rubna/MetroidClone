using MetroidClone.Engine;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    abstract class Monster : PhysicsObject
    {
        protected MonsterState State;
        protected enum MonsterState { Moving, Attacking, Jumping, PatrollingLeft, PatrollingRight, ChangeState }

        public int HitPoints = 1;
        public int Damage = 10;
        protected Vector2 SpeedOnHit = Vector2.Zero;

        public Monster()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //check collision player attacks
            foreach (IPlayerAttack attackInterface in World.GameObjects.OfType<IPlayerAttack>().ToList())
            {
                PhysicsObject attack = attackInterface as PhysicsObject;
                if (TranslatedBoundingBox.Intersects(attack.TranslatedBoundingBox))
                {
                    Hurt(Math.Sign(Position.X - attack.Position.X), true);
                    attack.Destroy();
                }
            }
        }

        void Hurt(int xDirection, bool hitByPlayer)
        {
            // if am attack from the player hits
            HitPoints--;
            if (SpeedOnHit != Vector2.Zero)
                Speed = new Vector2(xDirection * SpeedOnHit.X, SpeedOnHit.Y);
            if (HitPoints <= 0)
            {
                Destroy();
                if (hitByPlayer)
                {
                    World.Player.Score += 20;
                    Console.Write("Score: ");
                    Console.WriteLine(World.Player.Score);
                }
            }
        }

        public override void Destroy()
        {
            World.Tutorial.MonsterKilled = true;
            // When a monster is destroyed, you have a chance that a healthpack, rocket ammo, scrap metal or nothing will drop
            float ammoChance = (1 - ((float)World.Player.RocketAmmo / (float)World.Player.MaximumRocketAmmo)) * 40;
            float scrapChance = 40;
            float healthChance = (1 - ((float)World.Player.HitPoints / (float)World.Player.MaxHitPoints)) * 40;
            float randomLoot = World.Random.Next(101);
            if (randomLoot <= healthChance)
                World.AddObject(new HealthDrop(Damage), Position);
            else if (randomLoot <= ammoChance + healthChance)
                World.AddObject(new RocketAmmo(), Position);
            else if (randomLoot <= scrapChance + ammoChance + healthChance)
                World.AddObject(new Scrap(), Position);
            base.Destroy();
        }

        //Check if a bullet could reach the player
        protected bool CanReachPlayer()
        {
            Vector2 emulatedBulletPos = Position;
            Vector2 dir = World.Player.Position - Position;
            dir.Normalize();

            while ((emulatedBulletPos - World.Player.Position).Length() > 8)
            {
                emulatedBulletPos += MonsterBullet.baseSpeed * dir;

                if (InsideWall(new Rectangle((int) emulatedBulletPos.X - 4, (int) emulatedBulletPos.Y - 4, 8, 8)))
            {
                    return false;
            }
            }

            return true;
        }
    }
}
