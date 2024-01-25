using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MithrixMinions
{

    //This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a small plugin that adds a relatively simple item to the game, and gives you that item whenever you press F2.

    //This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class MithrixMinions : BaseUnityPlugin
    {
        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "OakPrime";
        public const string PluginName = "MithrixMinions";
        public const string PluginVersion = "1.0.0";
        private RoR2.SpawnCard exploderSpawnCard = Addressables.LoadAssetAsync<RoR2.SpawnCard>((object)"RoR2/Base/LunarExploder/cscLunarExploder.asset").WaitForCompletion();


        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            Log.Init(Logger);
            try
            {
                Logger.LogDebug("test");

                /*RoR2.RoR2Application.onLoad += () =>
                {
                    Logger.LogDebug("test2");

                    Logger.LogDebug("eliteCat length : " + EliteCatalog.eliteDefs.Length);
                    foreach (EliteDef def in EliteCatalog.eliteDefs)
                    {
                        Logger.LogDebug(def.name + ": health factor: " + def.healthBoostCoefficient + " damage factor: " + def.damageBoostCoefficient);
                        if (def.name.Equals("edVoid")) {
                            //Logger.LogDebug("Old health factor: " + def.healthBoostCoefficient);
                            //Logger.LogDebug("Old damage factor: " + def.damageBoostCoefficient);

                            //def.healthBoostCoefficient = 1.42f;
                            def.damageBoostCoefficient = 10.0f;
                            //Logger.LogDebug("New health factor: " + def.healthBoostCoefficient);

                            //Logger.LogDebug("New damage factor: " + def.damageBoostCoefficient);

                        }
                    };
                };*/
                On.EntityStates.BrotherMonster.ExitSkyLeap.OnEnter += (orig, self) =>
                {
                    orig(self);
                    RoR2.SpawnCard spawnCard = this.exploderSpawnCard;
                    for (int i = 0; i < 4; i++)
                    {
                        GameObject spawnedInstance = spawnCard.DoSpawn(self.characterBody.footPosition, Quaternion.identity, new RoR2.DirectorSpawnRequest(spawnCard, new RoR2.DirectorPlacementRule()
                        {
                            placementMode = RoR2.DirectorPlacementRule.PlacementMode.Direct
                        }, RoR2.Run.instance.runRNG)
                        {
                            teamIndexOverride = new TeamIndex?(TeamIndex.Monster)
                        }).spawnedInstance;
                        NetworkServer.Spawn(spawnedInstance);
                        Logger.LogDebug("Spawned little guy");
                    }
                };
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + " - " + e.StackTrace);
            }
        }
    }
}
