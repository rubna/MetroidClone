﻿using System;
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

        //Splits text like "1x0.5" into two parts; the 1 and the 0.5.
        private Vector2 GetSizeVector(string text)
        {
            string[] sizeParts = text.Split('x');
            if (sizeParts.Length == 2)
            {
                try
                {
                    return new Vector2(float.Parse(sizeParts[0]), float.Parse(sizeParts[1]));
                }
                catch (FormatException e)
                {
                    throw new InvalidDataException("This string wasn't in the correct format to split into a size vector! The two values should be numbers.", e);
                }
            }
            else
                throw new InvalidDataException("This string wasn't in the correct format to split into a size vector!");
        }

        public AssetManager(ContentManager content)
        {
            this.content = content;
            assets = new Dictionary<string, IAsset>();

            string spriteInfoData = File.ReadAllText("Content/SpriteInfo.txt");
            string[] spriteInfoDataLines = spriteInfoData.Split('\n');

            spriteInfo = new Dictionary<string, Dictionary<string, Vector2>>();

            //Check all data lines
            foreach (string spriteInfoDataLine in spriteInfoDataLines)
            {
                if (spriteInfoDataLine != "" && !spriteInfoDataLine.StartsWith("//")) //If the line has any information and isn't a comment.
                {
                    //Split the line into parts
                    string[] spriteInfoDataParts = spriteInfoDataLine.Split(' ');
                    //Save the parts into variables and make the origin and sheet size lower case.
                    string spriteName = spriteInfoDataParts[0], spriteOrigin = spriteInfoDataParts[1].ToLower(), sheetSize = spriteInfoDataParts[2].ToLower();

                    spriteInfo[spriteName] = new Dictionary<string, Vector2>(); //Create a dictionary for the information.

                    if (spriteInfoDataParts.Length >= 2) //Check if the entry contains information about the origin.
                    {
                        if (spriteOrigin.Contains("x"))
                            spriteInfo[spriteName]["origin"] = GetSizeVector(spriteOrigin);
                        else
                        {
                            //By default, the origin is in the middle.
                            //Texts like "topleft" can be used to change it.
                            Vector2 originVector = new Vector2(0.5f, 0.5f);
                            if (spriteOrigin.Contains("top"))
                                originVector.Y = 0f;
                            else if (spriteOrigin.Contains("bottom"))
                                originVector.Y = 1f;
                            if (spriteOrigin.Contains("left"))
                                originVector.X = 0f;
                            else if (spriteOrigin.Contains("right"))
                                originVector.X = 1f;
                            spriteInfo[spriteName]["origin"] = originVector;
                        }
                    }
                    else
                        spriteInfo[spriteName]["origin"] = new Vector2(0.5f, 0.5f);

                    if (spriteInfoDataParts.Length >= 3) //Check if the entry contains information about the sheet size.
                    {
                        if (sheetSize.Contains("x"))
                            spriteInfo[spriteName]["sheetsize"] = GetSizeVector(sheetSize); //It's a sprite sheet.
                        else //It must be a sprite strip.
                        {
                            try
                            {
                                //Store the sprite strip as a nx1 sprite sheet.
                                spriteInfo[spriteName]["sheetsize"] = new Vector2(float.Parse(sheetSize), 1f);
                            }
                            catch (FormatException e)
                            {
                                throw new InvalidDataException("The sheet size wasn't formatted correctly in the SpriteInfo file! " +
                                    "It should be formatted as a single number or as 'nxn' where n are numbers.", e);
                            }
                        }
                            
                    }
                    else
                        spriteInfo[spriteName]["sheetsize"] = new Vector2(1f, 1f);
                }
            }
        }

        public Sprite GetSprite(string name)
        {
            if (! assets.ContainsKey(name))
            {
                assets[name] = new Sprite(content.Load<Texture2D>(name), spriteInfo[name]["origin"], spriteInfo[name]["sheetsize"]);
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
