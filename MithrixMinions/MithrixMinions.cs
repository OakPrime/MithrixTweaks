using BepInEx;
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
        public const string PluginName = "MithrixTweaks";
        public const string PluginVersion = "1.0.4";
        private RoR2.SpawnCard minionSpawnCard = Addressables.LoadAssetAsync<RoR2.SpawnCard>((object)"RoR2/Base/LunarExploder/cscLunarExploder.asset").WaitForCompletion();
        private float timer;
        private const float WAVECOOLDOWN = 1.4f;


        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            Log.Init(Logger);
            try
            {
                MMConfig.InitializeConfig();

                if (MMConfig.waveDmgMultiplier.Value != 1.0)
                {
                    RoR2.RoR2Application.onLoad += () =>
                    {
                        EntityStates.BrotherMonster.ExitSkyLeap.waveProjectileDamageCoefficient *= MMConfig.waveDmgMultiplier.Value;
                    };
                }
                if (MMConfig.waveCount.Value > 1)
                {
                    var wavesCompleted = 1;
                    On.EntityStates.BrotherMonster.ExitSkyLeap.OnEnter += (orig, self) =>
                    {
                        orig(self);
                        timer = WAVECOOLDOWN;
                        wavesCompleted = 1;
                    };
                    On.EntityStates.BrotherMonster.ExitSkyLeap.FixedUpdate += (orig, self) =>
                    {
                        orig(self);
                        //Log.LogDebug("Timer: " + timer);
                        timer -= Time.fixedDeltaTime;
                        //Log.LogDebug("Timer: " + timer);
                        //Log.LogDebug("Fixed age: " + self.fixedAge);
                        //if (self.fixedAge )
                        if (timer <= 0.0f && wavesCompleted < MMConfig.waveCount.Value)
                        {
                            //Log.LogDebug("Fired wave");
                            self.FireRingAuthority();
                            timer = WAVECOOLDOWN;
                            wavesCompleted++;
                        }
                    };
                }
                

                if (MMConfig.minionCount.Value > 0)
                {
                    On.EntityStates.BrotherMonster.HoldSkyLeap.OnEnter += (orig, self) =>
                    {
                        orig(self);
                        MithrixMinionsBehavior behavior = self.characterBody.GetComponent<MithrixMinionsBehavior>();
                        if (behavior != null)
                        {
                            behavior.KillMinions();
                        }
                    };
                    On.EntityStates.BrotherMonster.ExitSkyLeap.OnEnter += (orig, self) =>
                    {
                        orig(self);
                        //Logger.LogDebug("Duration: " + self.duration);
                        MithrixMinionsBehavior behavior = self.characterBody.GetComponent<MithrixMinionsBehavior>();
                        if (behavior == null)
                        {
                            //Log.LogDebug("New behavior created");
                            behavior = self.characterBody.gameObject.AddComponent<MithrixMinionsBehavior>();
                        }
                        Vector3[] relativePos = { new Vector3(25.0f, 0.0f, 25.0f), new Vector3(-25.0f, 0.0f, -25.0f), new Vector3(25.0f, 0.0f, -25.0f), new Vector3(-25.0f, 0.0f, 25.0f) };
                        for (int i = 0; i < MMConfig.minionCount.Value; i++)
                        {
                            GameObject spawnedInstance = this.minionSpawnCard.DoSpawn(self.characterBody.footPosition + relativePos[i%4] + (relativePos[i%4] * (i/4) * 0.5f), Quaternion.identity, new RoR2.DirectorSpawnRequest(this.minionSpawnCard, new RoR2.DirectorPlacementRule()
                            {
                                placementMode = RoR2.DirectorPlacementRule.PlacementMode.Direct
                            }, RoR2.Run.instance.runRNG)
                            {
                                teamIndexOverride = new TeamIndex?(TeamIndex.Monster)
                            }).spawnedInstance;
                            NetworkServer.Spawn(spawnedInstance);
                            CharacterBody minionBody = spawnedInstance.gameObject.GetComponent<CharacterMaster>().GetBody();
                            minionBody.teamComponent.teamIndex = self.characterBody.teamComponent.teamIndex; //prone to null errors, may need to set entire teamComponent
                            behavior.AddMinion(spawnedInstance.gameObject.GetComponent<CharacterMaster>().GetBody()); //bug prob here
                            //Logger.LogDebug("Minion body status: " + ((bool)(UnityEngine.Object)spawnedInstance.gameObject.GetComponent<CharacterBody>() ? "not null" : "null"));
                            //Logger.LogDebug("Spawned " + spawnedInstance.gameObject.GetComponent<CharacterBody>() + " at position " + (i+1));
                        }
                    };
                }

                
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + " - " + e.StackTrace);
            }
        }
    }
}
