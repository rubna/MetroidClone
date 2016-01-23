﻿using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class Tutorial : GameObject
    {
        public override bool ShouldDrawGUI => true;
        //the range between the player and the object for its instruction to appear
        int tutorialRange = 500;

        Vector2 textSize;
        string currentText, previousText;
        
        //all instructions for the tutorial. kb => keyboard and gp => gamepad
        string kbmove = "Use A and D or Left and Right to move.";
        string gpmove = "Use the Left Thumbstick to move.";
        public bool Moved = false;

        string kbjump = "Press W or Up to jump.";
        string gpjump = "Press A or Left Thumbstick Up to jump.";
        public bool Jumped = false;

        string gun = "Good. Now walk over to the next room\nto pick up a gun.";
        public bool PickedUpGun = false;

        string kbgun = "Use Left Mouse Button to shoot.";
        string gpgun = "Use the Right Thumbstick to shoot.";
        public bool GunShot = false;
        
        string monster = "Kill the monster before it kills you!.";
        public bool MonsterKilled = false;

        string scrap = "Monsters can drop scrap metal on death.\nCollect scrap to build drones.";
        public bool ScrapCollected = false;

        string kbdrone = "Press E to build a drone using 25 scrap.\nDrones will help you fight monsters.";
        string gpdrone = "Press X to build a drone using 25 scrap.\nDrones will help you fight monsters.";
        public bool DroneBuild = false;

        string wrench = "Walk over the wrench to pick it up.";
        public bool PickedUpWrench = false;

        string kbwrench = "Press F or Middle Mouse Button to\nperform a melee attack.";
        string gpwrench = "Press B to perform a melee attack.";
        public bool WrenchUsed = false;

        string rocket = "Walk over the rocket launcher to pick it up.";
        public bool PickedUpRocket = false;

        string kbrocket = "Use Left Mouse Button to shoot,\nbut watch out:you have ammunition.";
        string gprocket = "Use the Right Thumbstick to shoot,\nbut watch out: you have ammunition.";
        public bool RocketShot = false;

        string kbswitch = "Press Q or scroll to switch between weapons.";
        string gpswitch = "Press Y to switch between weapons.";
        public bool WeaponSwitched = false;

        string ammo = "Monsters can also drop rocket ammunition.";
        public bool AmmoCollected = false;

        string gundoor = "Shoot the door with a gun to open it.";
        public bool GunDoorOpened = false;

        string wrenchdoor = "Hit the door with a wrench to open it.";
        public bool WrenchDoorOpened = false;

        string rocketdoor = "Blast the door with a rocket launcher to open it.";
        public bool RocketDoorOpened = false;

        public override void Create()
        {
            base.Create();
            currentText = Input.ControllerInUse ? gpmove : kbmove;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!GunDoorOpened)
                foreach (Door Door in World.GameObjects.OfType<Door>().ToList())
                {
                    if ((World.Player.Position - Door.Position).Length() <= tutorialRange)
                    {
                        if (currentText != gundoor)
                            previousText = currentText;
                        currentText = gundoor;
                    }
                    else
                        if (previousText != null)
                        currentText = previousText;
                }
            else
                currentText = null;

            if (!WrenchDoorOpened)
                foreach (MeleeDoor Door in World.GameObjects.OfType<MeleeDoor>().ToList())
                {
                    if ((World.Player.Position - Door.Position).Length() <= tutorialRange)
                    {
                        if (currentText != wrenchdoor)
                            previousText = currentText;
                        currentText = wrenchdoor;
                    }
                    else
                        if (previousText != null)
                        currentText = previousText;
                }
            else
                currentText = null;

            if (!RocketDoorOpened)
                foreach (DestroyableDoor Door in World.GameObjects.OfType<DestroyableDoor>().ToList())
                {
                    if ((World.Player.Position - Door.Position).Length() <= tutorialRange)
                    {
                        if (currentText != rocketdoor)
                            previousText = currentText;
                        currentText = rocketdoor;
                    }
                    else
                        if (previousText != null)
                        currentText = previousText;
                }
            else
                currentText = null;

            if (Moved && (currentText == kbmove || currentText == gpmove))
                currentText = Input.ControllerInUse ? gpjump : kbjump;

            if (Jumped && (currentText == kbjump || currentText == gpjump))
                currentText = gun;

            if (PickedUpGun && currentText == gun)
                currentText = Input.ControllerInUse ? gpgun : kbgun;

            if (GunShot && (currentText == gpgun || currentText == kbgun))
                currentText = null;

            if (!MonsterKilled && PickedUpGun)
                foreach (Monster Monster in World.GameObjects.OfType<Monster>().ToList())
                    if ((World.Player.Position - Monster.Position).Length() <= tutorialRange)
                        currentText = monster;

            if (MonsterKilled && currentText == monster)
                currentText = null;

            if (!ScrapCollected)
                foreach (Scrap Scrap in World.GameObjects.OfType<Scrap>().ToList())
                    if ((World.Player.Position - Scrap.Position).Length() <= tutorialRange)
                        currentText = scrap;

            if (ScrapCollected && currentText == scrap)
                currentText = Input.ControllerInUse ? gpdrone : kbdrone;

            if (DroneBuild && (currentText == gpdrone || currentText == kbdrone))
                currentText = null;

            if (!PickedUpWrench)
                foreach (WrenchPickup WrenchPickUp in World.GameObjects.OfType<WrenchPickup>().ToList())
                    if ((World.Player.Position - WrenchPickUp.Position).Length() <= tutorialRange)
                        currentText = wrench;

            if (PickedUpWrench && currentText == wrench)
                currentText = Input.ControllerInUse ? gpwrench : kbwrench;

            if (WrenchUsed && (currentText == gpwrench || currentText == kbwrench))
                currentText = null;

            if (!PickedUpRocket)
                foreach (RocketPickup RocketPickUp in World.GameObjects.OfType<RocketPickup>().ToList())
                    if ((World.Player.Position - RocketPickUp.Position).Length() <= tutorialRange)
                        currentText = rocket;

            if (PickedUpRocket && currentText == rocket)
                currentText = Input.ControllerInUse ? gprocket : kbrocket;

            if (RocketShot && (currentText == gprocket || currentText == kbrocket))
                currentText = Input.ControllerInUse ? gpswitch : kbswitch;

            if (WeaponSwitched)
                currentText = null;

            if (!AmmoCollected)
            {
                foreach (RocketAmmo RocketAmmo in World.GameObjects.OfType<RocketAmmo>().ToList())
                    if ((World.Player.Position - RocketAmmo.Position).Length() <= tutorialRange)
                        currentText = ammo;
                if (World.Player.RocketAmmo == 0)
                    currentText = ammo;
            }
            else
                currentText = null;
        }

        public override void DrawGUI()
        {
            if (currentText == null)
                return;
            Color guiColor = new Color(50, 50, 50, 200);
            textSize = Drawing.MeasureText("font18", currentText);
            Drawing.DrawRectangleUnscaled(new Rectangle(((int)Drawing.GUISize.X - (int)textSize.X) / 2 - 10, (int)Drawing.GUISize.Y - 20 - (int)textSize.Y, (int)textSize.X + 20, (int)textSize.Y + 10), guiColor);
            Drawing.DrawText("font18", currentText, new Vector2((int)Drawing.GUISize.X / 2, Drawing.GUISize.Y - 10), Color.White, alignment: Engine.Asset.Font.Alignment.BottomCenter);
        }
    }
}
