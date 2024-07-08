using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static BrutalCompanyMinus.Helper;
using static BrutalCompanyMinus.Configuration;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using System.Reflection;
using System.Linq;
using System.ComponentModel;

namespace BrutalCompanyMinus.Minus
{
    public class MEvent
    {

        internal ConfigEntry<string> descriptionsConfigEntry;
        /// <summary>
        /// This is the text displayed in the UI.
        /// </summary>
        public List<string> descriptions = new List<string>() { "" };

        internal ConfigEntry<string> colorHexConfigEntry;
        /// <summary>
        /// The color the event will be displayed in the UI in hex.
        /// </summary>
        public string colorHex = "#FFFFFF";

        internal ConfigEntry<int> weightConfigEntry;
        /// <summary>
        /// This can be ignored, this value is only used when Use_Custom_Weights in the config is set to true.
        /// </summary>
        public int weight = 1;

        internal ConfigEntry<EventType> typeConfigEntry;
        /// <summary>
        /// Set in what your opinion the severity of this event.
        /// </summary>
        public EventType type = EventType.Neutral;

        internal ConfigEntry<bool> enabledConfigEntry;
        /// <summary>
        /// If true event will appear, if false event will not appear.
        /// </summary>
        public bool enabled = true;

        internal Dictionary<ScaleType, ConfigEntry<string>> scaleListConfigEntry = new Dictionary<ScaleType, ConfigEntry<string>>();
        /// <summary>
        /// Set scales in Initalize() and then use Getf(ScaleType) or Get(ScaleType) to compute scale in Execute(), this will also generate automatically generate in the config.
        /// </summary>
        public Dictionary<ScaleType, Scale> scaleList = new Dictionary<ScaleType, Scale>();

        internal ConfigEntry<string> eventsToRemoveConfigEntry;
        /// <summary>
        /// Set this in Initalize() to specify events to prevent from occuring.
        /// </summary>
        public List<string> eventsToRemove = new List<string>();

        internal ConfigEntry<string> eventsToSpawnWithConfigEntry;
        /// <summary>
        /// Set this in Initalize() to specify events to spawn with, these wont be shown in the UI.
        /// </summary>
        public List<string> eventsToSpawnWith = new List<string>();

        /// <summary>
        /// Set this in Initalize() to make monster event(s).
        /// </summary>
        public List<MonsterEvent> monsterEvents = new List<MonsterEvent>();

        /// <summary>
        /// Set this in Initalize() to make a transmutation event.
        /// </summary>
        public ScrapTransmutationEvent scrapTransmutationEvent = new ScrapTransmutationEvent(new Scale(0.0f, 0.0f, 0.0f, 0.0f));

        internal bool executed = false;

        /// <summary>
        /// This is the event type.
        /// </summary>
        public enum EventType
        {
            VeryBad, Bad, Neutral, Good, VeryGood, Remove
        }

        /// <summary>
        /// Use the right one to generate the right config name and description.
        /// </summary>
        public enum ScaleType
        {
            InsideEnemyRarity, OutsideEnemyRarity, DaytimeEnemyRarity, MinOutsideEnemy, MinInsideEnemy, MaxOutsideEnemy, MaxInsideEnemy,
            ScrapValue, ScrapAmount, FactorySize, MinDensity, MaxDensity, MinCash, MaxCash, MinItemAmount, MaxItemAmount, MinValue, MaxValue, Rarity, MinRarity, MaxRarity,
            MinCut, MaxCut, MinHp, MaxHp, SpawnMultiplier, MaxInsideEnemyCount, MaxOutsideEnemyCount, SpawnCapMultiplier, MinPercentageCut, MaxPercentageCut, MinAmount, MaxAmount, Percentage
        }

        internal static Dictionary<ScaleType, string> ScaleInfoList = new Dictionary<ScaleType, string>() {
            { ScaleType.InsideEnemyRarity, "Enemy is added to Inside enemy list with rarity." }, { ScaleType.OutsideEnemyRarity, "Enemy is added to Outside enemy list with rarity." }, { ScaleType.DaytimeEnemyRarity, "Enemy is added to Daytime enemy list with rarity." },
            { ScaleType.MinOutsideEnemy, "Minimum amount of enemies garunteed to spawn outside." }, { ScaleType.MaxOutsideEnemy, "Maximum amount of enemies garunteed to spawn outside." }, { ScaleType.MinInsideEnemy, "Minimum amount of enemies garunteed to spawn inside." }, { ScaleType.MaxInsideEnemy, "Maximum amount of enemies garunteed to spawn inside." },
            { ScaleType.ScrapValue, "The amount that scrap value is multiplied by." }, { ScaleType.ScrapAmount, "The amount that scrap amount is multiplied by." }, { ScaleType.FactorySize, "The amount that factory size is multiplied by." },
            { ScaleType.MinDensity, "Minimum density value chosen." }, { ScaleType.MaxDensity, "Maximum density value chosen." }, { ScaleType.MinCash, "Minumum amount of cash given." }, { ScaleType.MaxCash, "Maximum amount of cash given." },
            { ScaleType.MinItemAmount, "Minimum amount of items to spawn." }, { ScaleType.MaxItemAmount, "Maximum amount of items to spawn." }, { ScaleType.MinValue, "The minimum value of something." }, { ScaleType.MaxValue, "The maximum value of something." },
            { ScaleType.Rarity, "The general chance of something." }, { ScaleType.MinRarity, "Minimum chance of something." }, { ScaleType.MaxRarity, "Maximum chance of something." }, { ScaleType.MinCut, "Minimum cut taken." }, { ScaleType.MaxCut, "Maximum cut taken." },
            { ScaleType.MinHp, "Minimum possible to be chosen." }, { ScaleType.MaxHp, "Maxmimum possible hp to be chosen." }, { ScaleType.SpawnMultiplier, "Will multiply the spawn chance." }, { ScaleType.SpawnCapMultiplier, "Will multiply the spawn cap." },
            { ScaleType.MaxInsideEnemyCount, "Changes max amount of inside enemies spawnable. " }, { ScaleType.MaxOutsideEnemyCount, "Changes max amount of outside enemies spawnable. " }, { ScaleType.MinPercentageCut, "Minimum possible percentage cut." }, { ScaleType.MaxPercentageCut, "Maximum possible percentage cut." },
            { ScaleType.MinAmount, "Minimum amount of something to be chosen." }, { ScaleType.MaxAmount, "Maximum amount of something to be chosen." }, { ScaleType.Percentage, "This value goes between 0.0 to 1.0." }
        };

        /// <summary>
        /// This is used to identify said event, preferably use nameof(thisClass) for name.
        /// </summary>
        /// <returns>Returns the given name.</returns>
        public virtual string Name() => "";

        /// <summary>
        /// This is called just right before config is generated.
        /// </summary>
        public virtual void Initalize() { }

        /// <summary>
        /// Event algorithm will only pick this event if this returns true.
        /// </summary>
        /// <returns>Always true if not overriden.</returns>
        public virtual bool AddEventIfOnly() { return true; }

        /// <summary>
        /// This is called after lever is pulled.
        /// </summary>
        public virtual void Execute() { }

        /// <summary>
        /// This is called when ship leaves.
        /// </summary>
        public virtual void OnShipLeave() { }

        /// <summary>
        /// This will only be called once when the game starts.
        /// </summary>
        public virtual void OnGameStart() { }

        /// <summary>
        /// This is used to compute scales, can be overriden.
        /// </summary>
        /// <param name="scaleType">A scale from Scale List, if dosen't exist it will return 0.0f.</param>
        /// <returns>Returns computed float value of given ScaleType if found.</returns>
        public virtual float Getf(ScaleType scaleType)
        {
            try
            {
                return Scale.Compute(scaleList[scaleType], type);
            } catch
            {
                Log.LogError(string.Format("Scalar '{0}' for '{1}' not found, returning 0.", scaleType.ToString(), Name()));
            }
            return 0.0f;
        }

        /// <summary>
        /// This computes Getf() and then converts to int.
        /// </summary>
        /// <param name="scaleType">A scale from Scale List, if dosen't exist it will return 0.</param>
        /// <returns>Returns computed int value of given ScaleType if found.</returns>
        public int Get(ScaleType scaleType)
        {
            return (int)Getf(scaleType);
        }

        /// <summary>
        /// Will execute every monster event inside of monsterEvents.
        /// </summary>
        public void ExecuteAllMonsterEvents()
        {
            foreach(MonsterEvent monsterEvent in monsterEvents)
            {
                monsterEvent.Execute();
            }
        }

        /// <summary>
        /// Will return an event from name.
        /// </summary>
        /// <param name="name">Name of event to find.</param>
        /// <returns>Will return said event if found, otherwise it will return the Nothing event.</returns>
        public static MEvent GetEvent(string name)
        {
            int index = EventManager.events.FindIndex(x => x.Name() == name);
            if (index != -1) return EventManager.events[index];

            Log.LogError(string.Format("Event '{0}' dosen't exist, returning nothing event", name));
            return new Events.Nothing();
        }

        internal void InitalizeBasicEntries(ConfigFile to, ModInfo info)
        {
            descriptionsConfigEntry = to.Bind(Name(), "Descriptions", ListToString(descriptions, "|"), "Seperated by |");
            descriptions = StringToList(descriptionsConfigEntry.Value);
            descriptionsConfigEntry.SettingChanged += (o, e) => descriptions = StringToList(descriptionsConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(descriptionsConfigEntry, false), info);

            colorHexConfigEntry = to.Bind(Name(), "Color Hex", colorHex);
            colorHex = colorHexConfigEntry.Value;
            colorHexConfigEntry.SettingChanged += (o, e) => colorHex = colorHexConfigEntry.Value;
            AddConfigForLethalConfig(new TextInputFieldConfigItem(colorHexConfigEntry, false), info);

            weightConfigEntry = to.Bind(Name(), "Custom Weight", weight, "If you want to use custom weights change 'Use custom weights'? setting in '__Event Settings' to true.");
            weight = weightConfigEntry.Value;
            weightConfigEntry.SettingChanged += (o, e) => weight = weightConfigEntry.Value;
            AddConfigForLethalConfig(new IntInputFieldConfigItem(weightConfigEntry, false), info);

            typeConfigEntry = to.Bind(Name(), "Event Type", type);
            type = typeConfigEntry.Value;
            typeConfigEntry.SettingChanged += (o, e) => type = typeConfigEntry.Value;
            AddConfigForLethalConfig(new EnumDropDownConfigItem<EventType>(typeConfigEntry, false), info);

            enabledConfigEntry = to.Bind(Name(), "Event Enabled?", enabled, "Setting this to false will stop the event from occuring.");
            enabled = enabledConfigEntry.Value;
            enabledConfigEntry.SettingChanged += (o, e) => enabled = enabledConfigEntry.Value;
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(enabledConfigEntry, false), info);

            List<KeyValuePair<ScaleType, Scale>> scaleListed = scaleList.ToList();
            for (int i = 0; i < scaleList.Count; i++)
            {
                KeyValuePair<ScaleType, Scale> scale = scaleListed[i];
                scaleListConfigEntry.Add(scale.Key, to.Bind(Name(), scale.Key.ToString(), GetStringFromScale(scale.Value), ScaleInfoList[scale.Key] + "   " + scaleDescription));
                scaleList[scale.Key] = GetScale(scaleListConfigEntry[scale.Key].Value);
                scaleListConfigEntry[scale.Key].SettingChanged += (o, e) => scaleList[scale.Key] = GetScale(scaleListConfigEntry[scale.Key].Value);
                AddConfigForLethalConfig(new TextInputFieldConfigItem(scaleListConfigEntry[scale.Key], false), info);
            }

            eventsToRemoveConfigEntry = to.Bind(Name(), "Events To Remove", ListToString(eventsToRemove, ", "), "Will prevent said event(s) from occuring.");
            eventsToRemove = ListToStrings(eventsToRemoveConfigEntry.Value);
            eventsToRemoveConfigEntry.SettingChanged += (o, e) => eventsToRemove = ListToStrings(eventsToRemoveConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(eventsToRemoveConfigEntry, false), info);

            eventsToSpawnWithConfigEntry = to.Bind(Name(), "Events To Spawn With", ListToString(eventsToSpawnWith, ", "), "Will spawn said events(s).");
            eventsToSpawnWith = ListToStrings(eventsToSpawnWithConfigEntry.Value);
            eventsToSpawnWithConfigEntry.SettingChanged += (o, e) => eventsToSpawnWith = ListToStrings(eventsToSpawnWithConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(eventsToSpawnWithConfigEntry, false), info);

        }

        public virtual void InitalizeConfigEntries(ConfigFile to, ModInfo info)
        {
            InitalizeBasicEntries(to, info);

            foreach(MonsterEvent monsterEvent in monsterEvents)
            {
                monsterEvent.IniatlizeConfig(Name(), to, info);
            }

            if (scrapTransmutationEvent.items.Length > 0)
            {
                scrapTransmutationEvent.amountConfigEntry = to.Bind(Name(), "Percentage", GetStringFromScale(scrapTransmutationEvent.amount), $"{ScaleInfoList[ScaleType.Percentage]}   {scaleDescription}");
                scrapTransmutationEvent.amount = GetScale(scrapTransmutationEvent.amountConfigEntry.Value);
                scrapTransmutationEvent.amountConfigEntry.SettingChanged += (o, e) => scrapTransmutationEvent.amount = GetScale(scrapTransmutationEvent.amountConfigEntry.Value);
                AddConfigForLethalConfig(new TextInputFieldConfigItem(scrapTransmutationEvent.amountConfigEntry, false), info);

                for (int i = 0; i < scrapTransmutationEvent.items.Length; i++)
                {
                    int k = i;
                    scrapTransmutationEvent.itemsConfigEntry.Add(to.Bind(Name(), $"Item {k}", SpawnableItemToString(scrapTransmutationEvent.items[k]), $"{ScaleInfoList[ScaleType.Percentage]}   {scaleDescription}"));
                    scrapTransmutationEvent.items[k] = StringToSpawnableItem(scrapTransmutationEvent.itemsConfigEntry[k].Value);
                    scrapTransmutationEvent.itemsConfigEntry[k].SettingChanged += (o, e) => scrapTransmutationEvent.items[k] = StringToSpawnableItem(scrapTransmutationEvent.itemsConfigEntry[k].Value);
                    AddConfigForLethalConfig(new TextInputFieldConfigItem(scrapTransmutationEvent.itemsConfigEntry[k], false), info);
                }
            }
        }
    }
}
