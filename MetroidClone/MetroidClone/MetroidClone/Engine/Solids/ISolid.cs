using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    interface ISolid 
    {
        bool CollidesWith(Rectangle box);
    }
}
