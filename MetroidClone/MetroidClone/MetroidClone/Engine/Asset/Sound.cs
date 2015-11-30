using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine.Asset
{
    public class Sound : IAsset
    {
        public SoundEffect SoundEffect { get; protected set; }
        public float Volume { get; protected set; }

        public Sound(SoundEffect soundEffect, float volume = 1f)
        {
            SoundEffect = soundEffect;
            Volume = volume;
        }
    }
}
