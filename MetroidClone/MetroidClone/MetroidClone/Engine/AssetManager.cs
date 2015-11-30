using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using MetroidClone.Engine.Asset;
using System.IO;

namespace MetroidClone.Engine
{
    class AssetManager
    {
        Dictionary<string, IAsset> assets;
        ContentManager content;
        Dictionary<string, Dictionary<string, Vector2>> spriteInfo;

        public AssetManager(ContentManager content)
        {
            this.content = content;
            assets = new Dictionary<string, IAsset>();

            string spriteInfoData = File.ReadAllText("Content/SpriteInfo.txt");
            string[] spriteInfoDataLines = spriteInfoData.Split('\n');

            foreach (string spriteInfoDataLine in spriteInfoDataLines)
            {
                var spriteInfoDataParts = spriteInfoDataLine.Split(' ');
                if (spriteInfoDataParts.Length >= 3)
                {
                    if (! spriteInfoDataParts[0].StartsWith("//"))
                    {

                    }
                }
            }
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
