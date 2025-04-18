﻿using BepInEx.Configuration;
using BrutalCompanyMinus.Minus.Handlers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace BrutalCompanyMinus.Minus
{
    [HarmonyPatch]
    public class EventManager
    {
        internal static List<MEvent> vanillaEvents = new List<MEvent>() {
            // Very Good
            new Events.BigBonus(),
            new Events.ScrapGalore(),
            new Events.GoldenBars(),
            new Events.BigDelivery(),
            new Events.PlentyOutsideScrap(),
            new Events.BlackFriday(),
            new Events.SafeOutside(),
            // Good
            new Events.Bounty(),
            new Events.Bonus(),
            new Events.SmallerMap(),
            new Events.MoreScrap(),
            new Events.HigherScrapValue(),
            new Events.GoldenFacility(),
            new Events.Dentures(),
            new Events.Pickles(),
            new Events.Honk(),
            new Events.TransmuteScrapSmall(),
            new Events.SmallDelivery(),
            new Events.ScarceOutsideScrap(),
            new Events.FragileEnemies(),
            new Events.FullAccess(),
            new Events.EarlyShip(),
            new Events.MoreExits(),
            // Neutral
            new Events.Nothing(),
            new Events.Locusts(),
            new Events.Birds(),
            new Events.Trees(),
            new Events.LeaflessBrownTrees(),
            new Events.LeaflessTrees(),
            new Events.Raining(),
            new Events.Gloomy(),
            new Events.HeavyRain(),
            // Bad
            new Events.HoardingBugs(),
            new Events.Bees(),
            new Events.Landmines(),
            new Events.Lizard(),
            new Events.Slimes(),
            new Events.Thumpers(),
            new Events.Turrets(),
            new Events.Spiders(),
            new Events.SnareFleas(),
            new Events.FacilityGhost(),
            new Events.OutsideTurrets(),
            new Events.OutsideLandmines(),
            new Events.ShipmentFees(),
            new Events.GrabbableLandmines(),
            new Events.GrabbableTurrets(),
            new Events.StrongEnemies(),
            new Events.KamikazieBugs(),
            new Events.RealityShift(),
            new Events.Masked(),
            new Events.Butlers(),
            new Events.SpikeTraps(),
            new Events.FlowerSnake(),
            new Events.LateShip(),
            new Events.HolidaySeason(),
            // Very Bad
            new Events.Nutcracker(),
            new Events.Arachnophobia(),
            new Events.Bracken(),
            new Events.Coilhead(),
            new Events.BaboonHorde(),
            new Events.Dogs(),
            new Events.Jester(),
            new Events.LittleGirl(),
            new Events.AntiCoilhead(),
            new Events.ChineseProduce(),
            new Events.TransmuteScrapBig(),
            new Events.Warzone(),
            new Events.BugHorde(),
            new Events.ForestGiant(),
            new Events.InsideBees(),
            new Events.NutSlayer(),
            new Events.Hell(),
            new Events.AllWeather(),
            new Events.Worms(),
            new Events.OldBirds(),
            // No Enemy
            new Events.NoBaboons(),
            new Events.NoBracken(),
            new Events.NoCoilhead(),
            new Events.NoDogs(),
            new Events.NoGiants(),
            new Events.NoHoardingBugs(),
            new Events.NoJester(),
            new Events.NoGhosts(),
            new Events.NoLizards(),
            new Events.NoNutcracker(),
            new Events.NoSpiders(),
            new Events.NoThumpers(),
            new Events.NoSnareFleas(),
            new Events.NoWorm(),
            new Events.NoSlimes(),
            new Events.NoMasks(),
            new Events.NoTurrets(),
            new Events.NoLandmines(),
            new Events.NoOldBird(),
            new Events.NoButlers(),
            new Events.NoSpikeTraps()
        };

        internal static List<MEvent> moddedEvents = new List<MEvent>();

        internal static List<MEvent> customEvents = new List<MEvent>();

        internal static List<MEvent> events = new List<MEvent>();

        internal static List<MEvent> disabledEvents = new List<MEvent>();

        internal static List<MEvent> currentEvents = new List<MEvent>();

        internal static List<MEvent> forcedEvents = new List<MEvent>();

        internal static List<MEvent> allVeryGood = new List<MEvent>(), allGood = new List<MEvent>(), allNeutral = new List<MEvent>(), allBad = new List<MEvent>(), allVeryBad = new List<MEvent>(), allRemove = new List<MEvent>();

        internal static List<CustomEvents> customEventsList = new List<CustomEvents>();

        internal static List<string> currentEventDescriptions = new List<string>();

        internal static float eventTypeSum = 0;
        internal static float[] eventTypeCount = new float[] { };

        internal static float[] eventTypeRarities = new float[] { };

        /// <summary>
        /// This must be called before save load, will generate the config in Custom_Events.cfg
        /// </summary>
        /// <param name="mEvents">MEvents.</param>
        public static void AddEvents(params MEvent[] mEvents) => customEventsList.Add(new CustomEvents(Configuration.customEventConfig, mEvents.ToList()));

        /// <summary>
        /// This must be called before save load, will generate the config in specified config file.
        /// </summary>
        /// <param name="toConfig">Config to generate to.</param>
        /// <param name="mEvents">MEvents.</param>
        public static void AddEvents(ConfigFile toConfig, params MEvent[] mEvents) => customEventsList.Add(new CustomEvents(toConfig, mEvents.ToList()));

        internal static MEvent RandomWeightedEvent(List<MEvent> _events, System.Random rng)
        {
            if (_events.Count == 0) return new Events.Nothing();

            int WeightedSum = 0;
            foreach (MEvent e in _events) WeightedSum += e.Weight;

            foreach (MEvent e in _events)
            {
                if (rng.Next(0, WeightedSum) < e.Weight)
                {
                    return e;
                }
                WeightedSum -= e.Weight;
            }

            return _events[_events.Count - 1];
        }

        internal static List<MEvent> ChooseEvents(out List<MEvent> additionalEvents)
        {
            currentEvents.Clear();

            events.RemoveAll(e => e.enabled == false);

            List<MEvent> chosenEvents = new List<MEvent>();
            List<MEvent> eventsToChooseForm = new List<MEvent>();
            foreach (MEvent e in events) eventsToChooseForm.Add(e);

            // Decide how many events to spawn
            System.Random rng = new System.Random(StartOfRound.Instance.randomMapSeed + 32345 + Environment.TickCount);
            int eventsToSpawn = (int)Scale.Compute(Configuration.eventsToSpawn, MEvent.EventType.Neutral) + RoundManager.Instance.GetRandomWeightedIndex(Configuration.WeightsForExtraEvents.IntArray(), rng);
            
            foreach(MEvent forcedEvent in forcedEvents)
            {
                eventsToChooseForm.RemoveAll(x => x.Name() == forcedEvent.Name());
                foreach(string eventToRemove in forcedEvent.eventsToRemove)
                {
                    eventsToChooseForm.RemoveAll(x => x.Name() == forcedEvent.Name());
                }
            }

            // Spawn events
            for (int i = 0; i < eventsToSpawn; i++)
            {
                MEvent newEvent = RandomWeightedEvent(eventsToChooseForm, rng);

                if (!newEvent.AddEventIfOnly()) // If event condition is false, remove event from eventsToChoosefrom and iterate again
                {
                    i--;
                    eventsToChooseForm.RemoveAll(x => x.Name() == newEvent.Name());
                    continue;
                }

                chosenEvents.Add(newEvent);

                // Remove so no further accurrences
                eventsToChooseForm.RemoveAll(x => x.Name() == newEvent.Name());

                // Remove incompatible events from toChooseList
                int AmountRemoved = 0;

                foreach (string eventToRemove in newEvent.eventsToRemove)
                {
                    eventsToChooseForm.RemoveAll(x => x.Name() == eventToRemove);
                    AmountRemoved += chosenEvents.RemoveAll(x => x.Name() == eventToRemove);
                }

                foreach (string eventToSpawnWith in newEvent.eventsToSpawnWith)
                {
                    eventsToChooseForm.RemoveAll(x => x.Name() == eventToSpawnWith);
                    AmountRemoved += chosenEvents.RemoveAll(x => x.Name() == eventToSpawnWith);
                }

                i -= AmountRemoved; // Decrement each time an event is removed from chosenEvents list
            }

            // Generate eventsToSpawnWith list with no copies
            List<MEvent> eventsToSpawnWith = new List<MEvent>();
            for (int i = 0; i < chosenEvents.Count; i++)
            {
                foreach (string eventToSpawnWith in chosenEvents[i].eventsToSpawnWith)
                {
                    int index = eventsToSpawnWith.FindIndex(x => x.Name() == eventToSpawnWith);
                    if (index == -1) eventsToSpawnWith.Add(MEvent.GetEvent(eventToSpawnWith)); // If dosen't exist in list, add.
                }
            }

            // Remove disabledEvents from EventsToSpawnWith List
            foreach (MEvent e in disabledEvents)
            {
                int index = eventsToSpawnWith.FindIndex(x => x.Name() == e.Name());
                if (index != -1) eventsToSpawnWith.RemoveAt(index);
            }

            additionalEvents = eventsToSpawnWith;
            currentEvents = chosenEvents;
            return chosenEvents;
        }

        internal static void ApplyEvents(List<MEvent> currentEvents)
        {
            foreach (MEvent e in currentEvents)
            {
                if(!e.executed)
                {
                    e.executed = true;
                    e.Execute();
                }
            }
        }

        internal static void ExecuteOnShipLeave()
        {
            Log.LogInfo("Executing OnShipLeave for all events()");
            foreach(MEvent e in events)
            {
                e.OnShipLeave();
            }

            foreach(MEvent e in vanillaEvents)
            {
                e.OnShipLeave();
            }

            foreach(MEvent e in moddedEvents)
            {
                e.OnShipLeave();
            }

            foreach(MEvent e in customEvents)
            {
                e.OnShipLeave();
            }
        }

        internal static void ExecuteOnGameStart()
        {
            Log.LogInfo("Executing OnGameStart for all events()");
            foreach (MEvent e in events)
            {
                e.OnGameStart();
            }

            foreach (MEvent e in vanillaEvents)
            {
                e.OnGameStart();
            }

            foreach (MEvent e in moddedEvents)
            {
                e.OnGameStart();
            }

            foreach (MEvent e in customEvents)
            {
                e.OnGameStart();
            }
        }

        internal static void UpdateAllEventWeights()
        {
            if (Configuration.useCustomWeights.Value) return;

            float fix(float value) // This is to avoid division by zero
            {
                if (value < 1) return 1;
                return value;
            }

            int eventTypeAmount = Configuration.eventTypeScales.Length;

            float[] computedScales = new float[eventTypeAmount];
            for (int i = 0; i < eventTypeAmount; i++) computedScales[i] = Scale.Compute(Configuration.eventTypeScales[i]);

            float eventTypeWeightSum = 0;
            for (int i = 0; i < eventTypeAmount; i++) eventTypeWeightSum += computedScales[i];
            eventTypeWeightSum = fix(eventTypeWeightSum);

            float[] eventTypeProbabilities = new float[eventTypeAmount];
            for(int i = 0; i < eventTypeAmount; i++) eventTypeProbabilities[i] = computedScales[i] / eventTypeWeightSum;
            eventTypeRarities = eventTypeProbabilities;

            int[] newEventWeights = new int[eventTypeAmount];
            for (int i = 0; i < eventTypeAmount; i++)
            {
                newEventWeights[i] = (int)((eventTypeSum / fix(eventTypeCount[i])) * eventTypeProbabilities[i] * 1000.0f);
                Log.LogInfo($"Set eventType Weight for {((MEvent.EventType)Enum.ToObject(typeof(MEvent.EventType), i)).ToString()} to {newEventWeights[i]}");
            }

            foreach(MEvent e in events) e.Weight = newEventWeights[(int)e.Type];
        }

        internal static void UpdateEventTypeCounts()
        {
            int eventTypeAmount = Configuration.eventTypeScales.Length;

            eventTypeCount = new float[eventTypeAmount];
            for (int i = 0; i < eventTypeAmount; i++) eventTypeCount[i] = 0.0f;
            foreach (MEvent e in events) eventTypeCount[(int)e.Type]++;

            eventTypeSum = 0.0f;
            for (int i = 0; i < eventTypeAmount; i++) eventTypeSum += eventTypeCount[i];
        }

        internal static void UpdateEventDescriptions(List<MEvent> events)
        {
            if (!Configuration.displayEvents.Value) return;
            currentEventDescriptions.Clear();
            foreach(MEvent e in events)
            {
                currentEventDescriptions.Add($"<color={e.ColorHex}>{e.Descriptions[UnityEngine.Random.Range(0, e.Descriptions.Count)]}</color>");
            }
        }

        /// <summary>
        /// This is used to describe difficulty name and color transitioning in the UI.
        /// </summary>
        public struct DifficultyTransition : IComparable<DifficultyTransition>
        {
            internal const uint byteMask = 0b_00000000_00000000_00000000_11111111;

            public string name;
            public string hex;
            public float above;

            public uint[] rgb;

            public DifficultyTransition(string name, string hex, float above)
            {
                this.name = name;
                this.hex = hex;
                this.above = above;

                rgb = new uint[3];
                uint parsedValue = 0;
                try
                {
                    parsedValue = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                } catch
                {
                    Log.LogError("Failed to parse hex.");
                }

                rgb[0] = (parsedValue >> 16) & byteMask; // r
                rgb[1] = (parsedValue >> 8) & byteMask;  // g
                rgb[2] = parsedValue & byteMask;         // b
            }

            public string GetTransitionHex(DifficultyTransition next)
            {
                float at = Mathf.Clamp((next.above - Manager.difficulty) / (next.above - above), 0.0f, 1.0f);

                uint newR = InBetween(rgb[0], next.rgb[0], at);
                uint newG = InBetween(rgb[1], next.rgb[1], at);
                uint newB = InBetween(rgb[2], next.rgb[2], at);

                return newR.ToString("X2") + newG.ToString("X2") + newB.ToString("X2");
            }

            private uint InBetween(uint min, uint max, float at) => (uint)Mathf.Clamp((at * (max - min)) + min, 0.0f, 255.0f);

            public int CompareTo(DifficultyTransition other)
            {
                return above.CompareTo(other.above);
            }
        }

        internal class 
            CustomEvents
        {
            public ConfigFile configFile;

            public List<MEvent> events;

            public CustomEvents(ConfigFile configFile, List<MEvent> events)
            {
                this.configFile = configFile;
                this.events = events;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        private static void ModifyLevel(ref SelectableLevel newLevel)
        {
            ExecuteOnShipLeave();
            UI.canClearText = false;
            Manager.ComputeDifficultyValues();

            Manager.currentLevel = newLevel;
            Manager.currentTerminal = GameObject.FindObjectOfType<Terminal>();

            Assets.generateOriginalValuesLists();
            Net.Instance.ClearGameObjectsClientRpc(); // Clear all previously placed objects on all clients
            if (!RoundManager.Instance.IsHost || newLevel.levelID == 3) return;

            LevelModifications.ResetValues(StartOfRound.Instance);
            
            // Apply weather multipliers
            foreach (Weather e in Net.Instance.currentWeatherMultipliers)
            {
                if (newLevel.currentWeather == e.weatherType)
                {
                    Manager.scrapValueMultiplier *= e.scrapValueMultiplier;
                    Manager.scrapAmountMultiplier *= e.scrapAmountMultiplier;
                }
            }

            // Apply level properties
            LevelProperties properties = Configuration.levelProperties.GetValueOrDefault(newLevel.levelID);
            if(properties != null)
            {
                Manager.scrapValueMultiplier *= properties.GetScrapValueMultiplier();
                Manager.scrapAmountMultiplier *= properties.GetScrapAmountMultiplier();
            }
            
            // Difficulty modifications
            Manager.AddEnemyHp((int)Scale.Compute(Configuration.enemyBonusHpScaling));
            Manager.AddInsideSpawnChance(newLevel, Scale.Compute(Configuration.insideSpawnChanceAdditive));
            Manager.AddOutsideSpawnChance(newLevel, Scale.Compute(Configuration.outsideSpawnChanceAdditive));
            Manager.MultiplySpawnChance(newLevel, Scale.Compute(Configuration.spawnChanceMultiplierScaling));
            Manager.MultiplySpawnCap(Scale.Compute(Configuration.spawnCapMultiplier));
            Manager.AddInsidePower((int)Scale.Compute(Configuration.insideEnemyMaxPowerCountScaling));
            Manager.AddOutsidePower((int)Scale.Compute(Configuration.outsideEnemyPowerCountScaling));
            Manager.scrapValueMultiplier *= Scale.Compute(Configuration.scrapValueMultiplier);
            Manager.scrapAmountMultiplier *= Scale.Compute(Configuration.scrapAmountMultiplier);

            // Choose and apply events
            if (!Configuration.useCustomWeights.Value) UpdateAllEventWeights();

            List<MEvent> additionalEvents = new List<MEvent>();
            List<MEvent> currentEvents = ChooseEvents(out additionalEvents);

            foreach (MEvent e in currentEvents) Log.LogInfo("Event chosen: " + e.Name()); // Log Chosen events
            foreach (MEvent e in additionalEvents) Log.LogInfo("Additional events: " + e.Name());

            ApplyEvents(currentEvents);
            ApplyEvents(additionalEvents);

            foreach(MEvent forcedEvent in forcedEvents)
            {
                forcedEvent.Execute();

                foreach(string additionalEvent in forcedEvent.eventsToSpawnWith)
                {
                    MEvent.GetEvent(additionalEvent).Execute();
                }
            }

            currentEvents.AddRange(forcedEvents);
            forcedEvents.Clear();

            UpdateEventDescriptions(currentEvents);

            if (Configuration.showEventsInChat.Value && !Configuration.DisplayUIAfterShipLeaves.Value)
            {
                HUDManager.Instance.AddTextToChatOnServer("<color=#FFFFFF>Events:</color>");
                foreach(string eventDescription in currentEventDescriptions)
                {
                    HUDManager.Instance.AddTextToChatOnServer(eventDescription);
                }
            }

            // Apply maxPower counts
            RoundManager.Instance.currentLevel.maxEnemyPowerCount = (int)((RoundManager.Instance.currentLevel.maxEnemyPowerCount + Manager.bonusMaxInsidePowerCount) * Manager.spawncapMultipler);
            RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount = (int)((RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount + Manager.bonusMaxOutsidePowerCount) * Manager.spawncapMultipler);

            // Sync values to all clients
            Net.Instance.SyncValuesClientRpc(Manager.currentLevel.factorySizeMultiplier, Manager.scrapValueMultiplier, Manager.scrapAmountMultiplier, Manager.bonusEnemyHp);

            // Apply UI
            if (!Configuration.DisplayUIAfterShipLeaves.Value)
            {
                UI.GenerateText(currentEvents);
            }
            else
            {
                UI.ClearText();
            }

            // Logging
            Log.LogInfo("MapMultipliers = [scrapValueMultiplier: " + Manager.scrapValueMultiplier + ",     scrapAmountMultiplier: " + Manager.scrapAmountMultiplier + ",     currentLevel.factorySizeMultiplier:" + Manager.currentLevel.factorySizeMultiplier + "]");
            Log.LogInfo("Inside Spawn Curve");
            foreach (Keyframe key in newLevel.enemySpawnChanceThroughoutDay.keys) Log.LogInfo($"Time:{key.time} + $Value:{key.value}");
            Log.LogInfo("Outside Spawn Curve");
            foreach (Keyframe key in newLevel.outsideEnemySpawnChanceThroughDay.keys) Log.LogInfo($"Time:{key.time} + $Value:{key.value}");
            Log.LogInfo("Daytime Spawn Curve");
            foreach (Keyframe key in newLevel.daytimeEnemySpawnChanceThroughDay.keys) Log.LogInfo($"Time:{key.time} + $Value:{key.value}");
        }
    }
}
