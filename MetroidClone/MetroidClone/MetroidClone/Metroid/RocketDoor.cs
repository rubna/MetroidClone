using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class RocketDoor : Door
    {
        protected override string closedSprite { get { return "doorspriteclosedorange"; } }
        protected override string standardSprite { get { return "doorspriteopen"; } }
    }
}
