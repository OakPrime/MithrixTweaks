using BepInEx;
using BepInEx.Configuration;
using System;

namespace MithrixMinions
{
    internal static class MMConfig
    {
        public static ConfigEntry<int> minionCount;
        public static ConfigEntry<int> waveCount;
        public static ConfigEntry<float> waveDmgMultiplier;

        public static void InitializeConfig()
        {
            var configFile = new ConfigFile(Paths.ConfigPath + "\\OakPrime.MithrixMinions.cfg", true);
            minionCount = configFile.Bind("Main", "Mithrix Lunar Exploder Count", 4, "Mithrix summons this many Lunar Exploders. Default:4. Vanilla:0. Max:12");
            minionCount.Value = Math.Clamp(minionCount.Value, 0, 12);
            waveCount = configFile.Bind("Main", "Mithrix Sky Slam Wave Count", 3, "Mithrix sky slam produces this many waves. Default:3. Vanilla:1. Valid Range:1-3.");
            waveCount.Value = Math.Clamp(waveCount.Value, 1, 3);
            waveDmgMultiplier = configFile.Bind("Main", "Mithrix Sky Slam Wave Damage Multiplier", 0.4f, "Mithrix sky slam damage is multiplied by this number. Default:0.4. Vanilla:1.");
            waveDmgMultiplier.Value = Math.Clamp(waveDmgMultiplier.Value, 0.0f, 100.0f);
        }
    }
}
