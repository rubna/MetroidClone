using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class NoShooting : IHackingOpportunity
    {
        public string Text => "The robot can't use their gun for a short while.";
        public bool IsTimeBased => true;
        public int Time => 8;

        public bool CanUse(World world)
        {
            return world.Player.UnlockedWeapons.Contains(Weapon.Gun);
        }

        public void Use(World world)
        {
            world.Player.ShootingSpeedMod = 0;
        }

        public void Stop(World world)
        {
            world.Player.ShootingSpeedMod = 1;
        }
    }

    class FastShooting : IHackingOpportunity
    {
        public string Text => "The robot shoots much faster for a short time.";
        public bool IsTimeBased => true;
        public int Time => 10;

        public bool CanUse(World world)
        {
            return world.Player.UnlockedWeapons.Contains(Weapon.Gun);
        }

        public void Use(World world)
        {
            world.Player.ShootingSpeedMod = 2;
        }

        public void Stop(World world)
        {
            world.Player.ShootingSpeedMod = 1;
        }
    }

    class ExtremeShooting : IHackingOpportunity
    {
        public string Text => "The robot can shoot at an extreme speed for a very short time.";
        public bool IsTimeBased => true;
        public int Time => 5;

        public bool CanUse(World world)
        {
            return world.Player.UnlockedWeapons.Contains(Weapon.Gun);
        }

        public void Use(World world)
        {
            world.Player.ShootingSpeedMod = 4;
        }

        public void Stop(World world)
        {
            world.Player.ShootingSpeedMod = 1;
        }
    }
}
