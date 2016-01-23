using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class MeleeDoor : Door
    {
        protected override string closedSprite { get { return "doorspriteclosedred"; } }
        protected override string standardSprite { get { return "doorspriteopen"; } }
    }
}
