using MetroidClone.Engine;
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
        string gunInRoom = "Pick up the gun!";
        public bool PickedUpGun = false;

        string kbgun = "Use Left Mouse Button to shoot.";
        string gpgun = "Use the Right Thumbstick to shoot.";
        public bool GunShot = false;
        
        string monster = "Kill the monster before it kills you!";
        public bool MonsterKilled = false;

        string scrap = "Monsters can drop scrap metal on death.\nCollect scrap to build drones.";
        public bool ScrapCollected = false;

        string hpbonus = "Monsters can drop useful bonuses. This one heals you a bit.";
        public bool HealthBonusCollected = false;

        string kbdrone = "Press E to build a drone using 25 scrap.\nDrones will help you fight monsters.";
        string gpdrone = "Press X to build a drone using 25 scrap.\nDrones will help you fight monsters.";
        public bool DroneBuild = false;

        string wrench = "Walk over the wrench to pick it up.";
        public bool PickedUpWrench = false;

        string kbwrench = "Use the Right Mouse Button to\nperform a melee attack.";
        string gpwrench = "Press B to perform a melee attack.";
        public bool WrenchUsed = false;

        string rocket = "Walk over the rocket launcher to pick it up.";
        public bool PickedUpRocket = false;

        string kbrocket = "Use the Left Mouse Button to shoot,\nbut watch out: your amount of ammunition is limited.";
        string gprocket = "Use the Right Thumbstick to shoot,\nbut watch out: your amount of ammunition is limited.";
        public bool RocketShot = false;

        string kbswitch = "Press Q or scroll to switch between weapons.";
        string gpswitch = "Press Y to switch between weapons.";
        public bool WeaponSwitched = false;

        string ammo = "Monsters can drop rocket ammunition.";
        public bool AmmoCollected = false;

        string gundoor = "Shoot the door with your brand new gun to open it.";
        public bool GunDoorOpened = false;

        string wrenchdoor = "Hit red doors like this one with your wrench to open them.";
        public bool WrenchDoorOpened = false;

        string wrenchdoorNoWrench = "Hit red doors like this one with a wrench to open them.\n(You'll have to find a wrench first!)";

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
                    if ((World.Player.Position - Door.Position).Length() <= tutorialRange && !World.PointOutOfView(Door.Position) && !Door.Activated)
                    {
                        if (currentText != gundoor)
                            previousText = currentText;
                        currentText = gundoor;
                    }
                    else
                        if (previousText != null)
                        currentText = previousText;
                }
            else if (currentText == gundoor)
                currentText = null;

            if (!WrenchDoorOpened)
            {
                bool doorInRange = false;

                foreach (MeleeDoor Door in World.GameObjects.OfType<MeleeDoor>().ToList())
                {
                    if ((World.Player.Position - Door.Position).Length() <= tutorialRange && !World.PointOutOfView(Door.Position) && !Door.Activated)
                    {
                        if (currentText != wrenchdoor && currentText != wrenchdoorNoWrench)
                            previousText = currentText;
                        currentText = World.Player.UnlockedWeapons.Contains(Weapon.Wrench) ? wrenchdoor : wrenchdoorNoWrench;
                        doorInRange = true;
                    }
                }

                if (!doorInRange)
                {
                    if (currentText == wrenchdoor || currentText == wrenchdoorNoWrench)
                    {
                        if (previousText != null)
                            currentText = previousText;
                        else
                            currentText = null;
                    }
                }
            }
            else if (currentText == wrenchdoor || currentText == wrenchdoorNoWrench)
                currentText = null;

            /*if (!RocketDoorOpened)
                foreach (RocketDoor Door in World.GameObjects.OfType<RocketDoor>().ToList())
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
                currentText = null;*/

            if (Moved && (currentText == kbmove || currentText == gpmove))
                currentText = Input.ControllerInUse ? gpjump : kbjump;

            if (Jumped && (currentText == kbjump || currentText == gpjump || currentText == gun))
            {
                if (World.Player.Position.X < World.TileWidth * WorldGenerator.LevelWidth)
                    currentText = gun;
                else
                    currentText = gunInRoom;
            }

            if (PickedUpGun && (currentText == gun || currentText == gunInRoom))
            {
                currentText = Input.ControllerInUse ? gpgun : kbgun;
            }

            if (GunShot && (currentText == gpgun || currentText == kbgun))
                currentText = null;

            if (!MonsterKilled && PickedUpGun)
                foreach (Monster Monster in World.GameObjects.OfType<Monster>().ToList())
                    if ((World.Player.Position - Monster.Position).Length() <= tutorialRange && !World.PointOutOfView(Monster.Position))
                        currentText = monster;

            if (MonsterKilled && currentText == monster)
                currentText = null;

            if (!HealthBonusCollected)
            {
                foreach (HealthDrop HealthBonus in World.GameObjects.OfType<HealthDrop>().ToList())
                    if ((World.Player.Position - HealthBonus.Position).Length() <= tutorialRange && !World.PointOutOfView(HealthBonus.Position))
                        currentText = hpbonus;
            }
            else if (currentText == hpbonus)
                currentText = null;

            if (!ScrapCollected)
                foreach (Scrap Scrap in World.GameObjects.OfType<Scrap>().ToList())
                    if ((World.Player.Position - Scrap.Position).Length() <= tutorialRange && !World.PointOutOfView(Scrap.Position))
                        currentText = scrap;

            if (ScrapCollected && currentText == scrap)
                currentText = Input.ControllerInUse ? gpdrone : kbdrone;

            if (DroneBuild && (currentText == gpdrone || currentText == kbdrone))
                currentText = null;

            if (!PickedUpWrench)
                foreach (WrenchPickup WrenchPickUp in World.GameObjects.OfType<WrenchPickup>().ToList())
                    if ((World.Player.Position - WrenchPickUp.Position).Length() <= tutorialRange && !World.PointOutOfView(WrenchPickUp.Position))
                        currentText = wrench;

            if (PickedUpWrench && currentText == wrench)
                currentText = Input.ControllerInUse ? gpwrench : kbwrench;

            if (WrenchUsed && (currentText == gpwrench || currentText == kbwrench))
                currentText = null;

            if (!PickedUpRocket)
                foreach (RocketPickup RocketPickUp in World.GameObjects.OfType<RocketPickup>().ToList())
                    if ((World.Player.Position - RocketPickUp.Position).Length() <= tutorialRange && !World.PointOutOfView(RocketPickUp.Position))
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
                    if ((World.Player.Position - RocketAmmo.Position).Length() <= tutorialRange && !World.PointOutOfView(RocketAmmo.Position))
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
            Color guiColor = new Color(0, 0, 0, 50);
            textSize = Drawing.MeasureText("font18", currentText);
            Drawing.DrawRectangleUnscaled(new Rectangle(((int)Drawing.GUISize.X - (int)textSize.X) / 2 - 10, (int)Drawing.GUISize.Y - 20 - (int)textSize.Y, (int)textSize.X + 20, (int)textSize.Y + 10), guiColor);
            Drawing.DrawText("font18", currentText, new Vector2((int)Drawing.GUISize.X / 2, Drawing.GUISize.Y - 10), Color.White, alignment: Engine.Asset.Font.Alignment.BottomCenter);
        }
    }
}
