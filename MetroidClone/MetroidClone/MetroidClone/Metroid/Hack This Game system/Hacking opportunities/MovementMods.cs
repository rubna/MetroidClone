using MetroidClone.Engine;

namespace MetroidClone.Metroid
{
    //With this effect, the player will move slower.
    class SlowerMovement : IHackingOpportunity
    {
        public string Text => "The robot moves much slower for a while.";
        public bool IsTimeBased => true;
        public int Time => 20;

        public bool CanUse(World world)
        {
            return true;
        }

        public void Use(World world)
        {
            world.Player.movementSpeedModifier /= 2f;
        }

        public void Stop(World world)
        {
            world.Player.movementSpeedModifier *= 2f;
        }
    }

    //With this effect, the player will move faster.
    class FasterMovement : IHackingOpportunity
    {
        public string Text => "The robot moves much faster for a while.";
        public bool IsTimeBased => true;
        public int Time => 20;

        public bool CanUse(World world)
        {
            return true;
        }

        public void Use(World world)
        {
            world.Player.movementSpeedModifier *= 2f;
        }

        public void Stop(World world)
        {
            world.Player.movementSpeedModifier /= 2f;
        }
    }

    //With this effect, the player will jump higher.
    class HigherJump : IHackingOpportunity
    {
        public string Text => "The robot jumps higher for some time.";
        public bool IsTimeBased => true;
        public int Time => 15;

        public bool CanUse(World world)
        {
            return true;
        }

        public void Use(World world)
        {
            world.Player.jumpHeightModifier *= 1.18f;
        }

        public void Stop(World world)
        {
            world.Player.jumpHeightModifier /= 1.18f;
        }
    }

    //With this effect, the player will jump lower.
    class LowerJump : IHackingOpportunity
    {
        public string Text => "The robot jumps lower for some time.";
        public bool IsTimeBased => true;
        public int Time => 15;

        public bool CanUse(World world)
        {
            return true;
        }

        public void Use(World world)
        {
            world.Player.jumpHeightModifier /= 1.2f;
        }

        public void Stop(World world)
        {
            world.Player.jumpHeightModifier *= 1.2f;
        }
    }

    //With this effect, the player will teleport to a random position

}
