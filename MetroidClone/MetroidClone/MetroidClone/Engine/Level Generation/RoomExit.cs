﻿using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    //Very simple class to store information about exit of rooms in the game world.
    class RoomExit
    {
        public Point Position;
        public Direction Direction;

        public RoomExit(Point position, Direction direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}
