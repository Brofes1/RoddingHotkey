using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace RoddingHotkey.Common.Systems
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind RoddingKeybind { get; private set; }

        public override void Load()
        {
            RoddingKeybind = KeybindLoader.RegisterKeybind(Mod, "RodKeybind", "X");
        }

        public override void Unload()
        {
            RoddingKeybind = null;
        }
    }
}
