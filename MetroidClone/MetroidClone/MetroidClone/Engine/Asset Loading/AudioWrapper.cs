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
        //used in OpionsMenu. if these are off, the soundeffects and music won't play
        public bool AudioIsEnabled = true;
        public bool MusicIsEnabled = true;

        public AssetManager Assets;
        
        //the music player
        private SoundEffectInstance musicInstance;

        public AudioWrapper(AssetManager assetsManager)
        {
            Assets = assetsManager;
        }

        public void Play(Sound sound)
        {
            if (AudioIsEnabled)
                sound.SoundEffect.Play();
        }

        public void Play(string sound)
        {
            Play(Assets.GetSound(sound));
        }

        public void StopOrPlayMusic(bool MusicIsEnabled)
        {
            if (MusicIsEnabled)
                musicInstance?.Play();
            else
                musicInstance?.Stop();
        }

        public void PlayLooping(SoundEffect song)
        {
            if (MusicIsEnabled)
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
