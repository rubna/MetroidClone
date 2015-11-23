using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using MetroidClone.Engine.Asset;

namespace MetroidClone.Engine
{
    class AssetManager
    {
        Dictionary<string, IAsset> assets;
        ContentManager content;
        
        public AssetManager(ContentManager content)
        {
            this.content = content;
            assets = new Dictionary<string, IAsset>();
        }

        public Sprite GetSprite(string name)
        {
            if (! assets.ContainsKey(name))
            {
                assets[name] = new Sprite(content.Load<Texture2D>(name), new Vector2(0f)); //todo
            }
            return assets[name] as Sprite;
        }

        public Sound GetSound(string name)
        {
            if (! assets.ContainsKey(name))
            {
                assets[name] = new Sound(content.Load<SoundEffect>(name));
            }
            return assets[name] as Sound;
        }
    }
}
