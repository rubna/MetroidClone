using MetroidClone.Engine.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //The audio wrapper can be used to play sounds and music.
    class AudioWrapper
    {
        public bool AudioIsEnabled;

        public AssetManager Assets;

        public AudioWrapper()
        {
            AudioIsEnabled = true;
        }

        public void Play(Sound sound)
        {
            sound.SoundEffect.Play();
        }

        public void Play(string sound)
        {
            Play(Assets.GetSound(sound));
        }
    }
}
