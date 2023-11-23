using Microsoft.Xna.Framework;
using RoddingHotkey.Common.Systems;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.ModBrowser;

namespace RoddingHotkey.Common.Players
{
    internal class KeybindPlayer : ModPlayer
    {
        bool roddingDelayed = false;
        bool doNotRod = false;

        static string GetDeathMessage(string playerName)
        {
            int deathMessageID = Main.rand.Next(0, 6);

            switch (deathMessageID)
            {
                case 0: return playerName + " forgot that Chaos State exists.";
                case 1: return playerName + " didn't materialize.";
                case 2: return playerName + "'s legs appeared where their head should be.";
                case 3: return playerName + " tried to teleport; instead, they died.";
                case 4: return playerName + " didn't have as much HP as they thought they did.";
                case 5: return playerName + "'s legs appeared where their legs should be.";
                default: return "Brofes didn't code this mod as well as they thought they did. Anyway, " + playerName + " teleported too much.";
            }
        }

        Vector2 GetClampedMouseLocation()
        {
            if (Player.HasItemInInventoryOrOpenVoidBag(ItemID.RodOfHarmony))
                return Main.MouseWorld;

            Vector2 mouseWorld = Main.MouseWorld;
            Vector2 playerWorld = Player.position;

            Vector2 mouseRelativeToPlayer = mouseWorld - playerWorld;

            if (mouseRelativeToPlayer.X > 960)
                mouseRelativeToPlayer.X = 960;
            if (mouseRelativeToPlayer.X < -960)
                mouseRelativeToPlayer.X = -960;

            if (mouseRelativeToPlayer.Y > 600)
                mouseRelativeToPlayer.Y = 600;
            if (mouseRelativeToPlayer.Y < -600)
                mouseRelativeToPlayer.Y = -600;

            return playerWorld + mouseRelativeToPlayer;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            bool rodPositionOutsideRange = GetClampedMouseLocation() != Main.MouseWorld;

            if (!ModContent.GetInstance<RodModConfig>().RodIfMouseOutsideRange && rodPositionOutsideRange && !Player.HasItemInInventoryOrOpenVoidBag(ItemID.RodOfHarmony))
            {
                return;
            }
            if (KeybindSystem.RoddingKeybind.JustPressed)
            {
                // Use the rod if not using an item. If using an item, delay the rod usage until after the item usage is over.
                if (Player.itemTime <= 0)
                {
                    UseRod();
                    Player.itemTime = 30;
                    doNotRod = true;
                }
                else if (!roddingDelayed && !doNotRod)
                {
                    roddingDelayed = true;
                    Player.itemTime += 30;
                }
            }
        }

        public override void PreUpdate()
        {
            if (roddingDelayed && Player.itemTime <= 30)
            {
                roddingDelayed = false;
                doNotRod = true;
                UseRod();
            }
            if (doNotRod && Player.itemTime <= 5)
            {
                doNotRod = false;
            }
        }

        void UseRod()
        {
            if (Player.respawnTimer == 0 && (Player.HasItemInInventoryOrOpenVoidBag(ItemID.RodofDiscord) || Player.HasItemInInventoryOrOpenVoidBag(ItemID.RodOfHarmony)))
            {
                if (!Collision.SolidCollision(GetClampedMouseLocation() - new Vector2(Player.width / 2, Player.height), Player.width, Player.height))
                {
                    Player.Teleport(GetClampedMouseLocation() - new Vector2(Player.width / 2, Player.height), TeleportationStyleID.RodOfDiscord);

                    if (!Player.HasItemInInventoryOrOpenVoidBag(ItemID.RodOfHarmony))
                    {
                        if (Player.HasBuff(BuffID.ChaosState))
                        {
                            Vector2 playerVelocity = Player.velocity;
                            Player.statLife -= Player.statLifeMax2 / 7;
                            if (Player.statLife <= 0)
                                Player.KillMe(PlayerDeathReason.ByCustomReason(GetDeathMessage(Player.name)), 0, 0);
                            Player.velocity = playerVelocity;
                        }
                        Player.AddBuff(BuffID.ChaosState, 360);
                    }
                }
            }
        }
    }
}
