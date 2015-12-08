using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    interface ISolid 
    {
        bool CollidesWith(Rectangle box);
    }
}
