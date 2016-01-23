using MetroidClone.Engine.Asset;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //The audio wrapper can be used to play sounds and music.
    class AudioWrapper
    {
        public bool AudioIsEnabled = false;

        public AssetManager Assets;
        private SoundEffectInstance musicInstance;

        public AudioWrapper(AssetManager assetsManager)
        {
           
            MediaPlayer.IsRepeating = true;
            Assets = assetsManager;
        }

        public void Play(Sound sound)
        {
            if (AudioIsEnabled)
                sound.SoundEffect.Play();
        }

        public void Play(string sound)
        {
            if (AudioIsEnabled)
                Play(Assets.GetSound(sound));
        }

        public void PlayLooping(SoundEffect song)
        {
            if (AudioIsEnabled)
            {
                musicInstance = song.CreateInstance();
                musicInstance.IsLooped = true;
                musicInstance.Play();
            }
        }

        public void PlayLooping(string song)
        {
            PlayLooping(Assets.GetSong(song));
        }
    }
}
