using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace RoddingHotkey.Common
{
    internal class RodModConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("MainSettings")]
        [DefaultValue(true)]
        public bool RodIfMouseOutsideRange;
    }
}
