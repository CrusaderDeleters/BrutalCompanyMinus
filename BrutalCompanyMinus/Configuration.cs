using BepInEx;
using BepInEx.Configuration;
using BrutalCompanyMinus.Minus;
using System.Collections.Generic;
using static BrutalCompanyMinus.Minus.MEvent;
using HarmonyLib;
using UnityEngine;
using System.Globalization;
using BrutalCompanyMinus.Minus.Events;
using static BrutalCompanyMinus.Minus.MonoBehaviours.EnemySpawnCycle;
using static BrutalCompanyMinus.Assets;
using static BrutalCompanyMinus.Helper;
using BrutalCompanyMinus.Minus.MonoBehaviours;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using UnityEngine.InputSystem.XR;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    public class Configuration
    {
        // Config files
        public static ConfigFile uiConfig, eventConfig, weatherConfig, customAssetsConfig, difficultyConfig, moddedEventConfig, customEventConfig, allEnemiesConfig, levelPropertiesConfig;
        public static bool saveUIConfig = false, saveEventConfig = false, saveWeatherConfig = false, saveCustomAssetsConfig = false, saveDifficultyConfig = false, saveModdedEventConfig = false, saveCustomEventConfig = false, saveAllEnemiesConfig = false, saveLevelPropertiesConfig = false;

        // Difficulty Settings
        public static ConfigEntry<bool> useCustomWeights, showEventsInChat;
        public static Scale eventsToSpawn;
        public static ConfigEntry<float> goodEventIncrementMultiplier, badEventIncrementMultiplier;
        public static float[] weightsForExtraEvents;
        public static Scale[] eventTypeScales = new Scale[6];
        public static EventManager.DifficultyTransition[] difficultyTransitions;
        public static ConfigEntry<bool> enableQuotaChanges;
        public static ConfigEntry<int> deadLineDaysAmount, startingCredits, startingQuota, baseIncrease, increaseSteepness;
        public static Scale
            spawnChanceMultiplierScaling = new Scale(), insideEnemyMaxPowerCountScaling = new Scale(), outsideEnemyPowerCountScaling = new Scale(), enemyBonusHpScaling = new Scale(), spawnCapMultiplier = new Scale(),
            scrapAmountMultiplier = new Scale(), scrapValueMultiplier = new Scale(), insideSpawnChanceAdditive = new Scale(), outsideSpawnChanceAdditive = new Scale();
        public static ConfigEntry<bool> ignoreMaxCap;
        public static ConfigEntry<float> difficultyMaxCap;
        public static ConfigEntry<bool> scaleByDaysPassed, scaleByScrapInShip, scaleByMoonGrade, scaleByWeather, scaleByQuota;
        public static ConfigEntry<float> daysPassedDifficultyMultiplier, daysPassedDifficultyCap, scrapInShipDifficultyMultiplier, scrapInShipDifficultyCap, quotaDifficultyMultiplier, quotaDifficultyCap;
        public static Dictionary<string, float> gradeAdditives = new Dictionary<string, float>();
        public static Dictionary<LevelWeatherType, float> weatherAdditives = new Dictionary<LevelWeatherType, float>();

        // Weather settings
        public static ConfigEntry<bool> useWeatherMultipliers, randomizeWeatherMultipliers, enableTerminalText;
        public static ConfigEntry<float> weatherRandomRandomMinInclusive, weatherRandomRandomMaxInclusive;
        public static WeatherConfig noneMultiplier, dustCloudMultiplier, rainyMultiplier, stormyMultiplier, foggyMultiplier, floodedMultiplier, eclipsedMultiplier;

        // UI settings
        public static ConfigEntry<string> UIKey;
        public static ConfigEntry<bool> NormaliseScrapValueDisplay, EnableUI, ShowUILetterBox, ShowExtraProperties, PopUpUI, DisplayUIAfterShipLeaves, DisplayExtraPropertiesAfterShipLeaves, displayEvents;
        public static ConfigEntry<float> UITimeSeconds, scrollSpeed;

        // Custom assets settings
        public static ConfigEntry<int> nutSlayerLives, nutSlayerHp;
        public static ConfigEntry<float> nutSlayerMovementSpeed;
        public static ConfigEntry<bool> nutSlayerImmortal;
        public static ConfigEntry<int> slayerShotgunMinValue, slayerShotgunMaxValue;

        // All enemies settings
        public static ConfigEntry<bool> enableAllEnemies, enableAllAllEnemies;

        // Level properties settings
        public static Dictionary<int, LevelProperties> levelProperties = new Dictionary<int, LevelProperties>();

        // Other
        public static CultureInfo en = new CultureInfo("en-US"); // This is important, no touchy
        public static string scaleDescription = "Format: BaseScale, IncrementScale, MinCap, MaxCap,   Forumla: BaseScale + (IncrementScale * Difficulty),   By default difficulty goes between 0 to 100 depending on certain factors";

        internal static Assembly lethalConfigAssembly;
        internal static Type modInfo;
        internal static PropertyInfo modInfo_Name, modInfo_Guid, modInfo_Version, modInfo_Description;

        internal static Type mod;
        internal static MethodInfo mod_AddConfigItem;

        internal static Type lethalConfigManager;
        internal static PropertyInfo lethalConfigManager_Mods, lethalConfigManager_ModToAssemblyMap;
        internal static MethodInfo dictionaryTryGetValue;

        internal static Type baseConfigItem;
        internal static PropertyInfo baseConfigItem_Owner;

        internal const string uiModInfoGUID = "Drinkable.UISettings", weatherModInfoGUID = "Drinkable.WeatherSettings", customAssetsModInfoGUID = "Drinkable.CustomAssets", difficultyModInfoGUID = "Drinkable.DifficultySettings",
            vanillaEventsModInfoGUID = "Drinkable.VanillaEvents", moddedEventsModInfoGUID = "Drinkable.ModdedEvents", spawnCyclesModInfoGUID = "Drinkable.SpawnCycles", levelPropertiesModInfoGUID = "Drinkable.LevelProperties",
            monsterEventsModInfoGUID = "Drinkable.MonsterEvents", scrapEventModInfoGUID = "Drinkable.ScrapEvents";

        public static ModInfo uiModInfo = new ModInfo(uiModInfoGUID, "BCM UI settings", Plugin.VERSION, "");
        public static ModInfo weatherModInfo = new ModInfo(weatherModInfoGUID, "BCM Weather settings", Plugin.VERSION, "");
        public static ModInfo customAssetsModInfo = new ModInfo(customAssetsModInfoGUID, "BCM Custom assets", Plugin.VERSION, "");
        public static ModInfo difficultyModInfo = new ModInfo(difficultyModInfoGUID, "BCM Difficulty Settings", Plugin.VERSION, "");
        public static ModInfo vanillaEventsModInfo = new ModInfo(vanillaEventsModInfoGUID, "BCM Vanilla Events", Plugin.VERSION, "asdasd");
        public static ModInfo moddedEventsModInfo = new ModInfo(moddedEventsModInfoGUID, "BCM Modded Events", Plugin.VERSION, "asdasdeeeeeee");
        public static ModInfo spawnCyclesModInfo = new ModInfo(spawnCyclesModInfoGUID, "BCM All enemies", Plugin.VERSION, "");
        public static ModInfo levelPropertiesModInfo = new ModInfo(levelPropertiesModInfoGUID, "BCM Level properties", Plugin.VERSION, "");
        public static ModInfo monsterEventsModInfo = new ModInfo(monsterEventsModInfoGUID, "BCM Custom Monster events", Plugin.VERSION, "");
        public static ModInfo scrapEventModInfo = new ModInfo(scrapEventModInfoGUID, "BCM Custom scrap events", Plugin.VERSION, "");

        private static void Initalize()
        {
            EventManager.vanillaEvents.OrderBy(e => e.Name());
            EventManager.moddedEvents.OrderBy(e => e.Name());

            // I should just use a publicizer, but here I am...
            Plugin.harmony.PatchAll(typeof(ConfigMenuPatch));

            lethalConfigAssembly = Assembly.GetAssembly(typeof(BaseConfigItem));

            modInfo = lethalConfigAssembly?.GetType("LethalConfig.Mods.ModInfo");
            modInfo_Name = modInfo?.GetProperty("Name", BindingFlags.Instance | BindingFlags.NonPublic);
            modInfo_Guid = modInfo?.GetProperty("Guid", BindingFlags.Instance | BindingFlags.NonPublic);
            modInfo_Version = modInfo?.GetProperty("Version", BindingFlags.Instance | BindingFlags.NonPublic);
            modInfo_Description = modInfo?.GetProperty("Description", BindingFlags.Instance | BindingFlags.NonPublic);

            mod = lethalConfigAssembly?.GetType("LethalConfig.Mods.Mod");
            mod_AddConfigItem = mod?.GetMethod("AddConfigItem", BindingFlags.Instance | BindingFlags.NonPublic);

            lethalConfigManager = lethalConfigAssembly?.GetType("LethalConfig.LethalConfigManager");
            lethalConfigManager_Mods = lethalConfigManager?.GetProperty("Mods", BindingFlags.Static | BindingFlags.NonPublic);
            lethalConfigManager_ModToAssemblyMap = lethalConfigManager?.GetProperty("ModToAssemblyMap", BindingFlags.Static | BindingFlags.NonPublic);

            baseConfigItem = lethalConfigAssembly?.GetType("LethalConfig.ConfigItems.BaseConfigItem");
            baseConfigItem_Owner = baseConfigItem?.GetProperty("Owner", BindingFlags.Instance | BindingFlags.NonPublic);

            

            // Difficulty Settings
            useCustomWeights = difficultyConfig.Bind("_Event Settings", "Use custom weights?", false, "'false'= Use eventType weights to set all the weights     'true'= Use custom set weights");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(useCustomWeights, false), difficultyModInfo);

            ConfigEntry<string> eventsToSpawnConfigEntry = difficultyConfig.Bind("_Event Settings", "Event scale amount", "2, 0.03, 2.0, 5.0", "The base amount of events   Format: BaseScale, IncrementScale, MinCap, MaxCap,   " + scaleDescription);
            eventsToSpawn = GetScale(eventsToSpawnConfigEntry.Value);
            eventsToSpawnConfigEntry.SettingChanged += (o, e) => eventsToSpawn = GetScale(eventsToSpawnConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(eventsToSpawnConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> weightsForExtraEventsConfigEntry = difficultyConfig.Bind("_Event Settings", "Weights for bonus events", "40, 39, 15, 5, 1", "Weights for bonus events, can be expanded. (40, 39, 15, 5, 1) is equivalent to (+0, +1, +2, +3, +4) events");
            weightsForExtraEvents = ParseValuesFromString(weightsForExtraEventsConfigEntry.Value);
            weightsForExtraEventsConfigEntry.SettingChanged += (o, e) => weightsForExtraEvents = ParseValuesFromString(weightsForExtraEventsConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(weightsForExtraEventsConfigEntry, false), difficultyModInfo);

            showEventsInChat = difficultyConfig.Bind("_Event Settings", "Will Minus display events in chat?", false);
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(showEventsInChat, false), difficultyModInfo);

            

            eventTypeScales = new Scale[6];

            ConfigEntry<string> veryBadScaleConfigEntry = difficultyConfig.Bind("_EventType Weights", "VeryBad event scale", "5, 0.25, 5, 30", scaleDescription);
            eventTypeScales[0] = GetScale(veryBadScaleConfigEntry.Value);
            veryBadScaleConfigEntry.SettingChanged += (o, e) => eventTypeScales[0] = GetScale(veryBadScaleConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(veryBadScaleConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> badScaleConfigEntry = difficultyConfig.Bind("_EventType Weights", "Bad event scale", "40, -0.15, 25, 40", scaleDescription);
            eventTypeScales[1] = GetScale(badScaleConfigEntry.Value);
            badScaleConfigEntry.SettingChanged += (o, e) => eventTypeScales[1] = GetScale(badScaleConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(badScaleConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> neutralScaleConfigEntry = difficultyConfig.Bind("_EventType Weights", "Neutral event scale", "10, -0.05, 5, 10", scaleDescription);
            eventTypeScales[2] = GetScale(neutralScaleConfigEntry.Value);
            neutralScaleConfigEntry.SettingChanged += (o, e) => eventTypeScales[2] = GetScale(neutralScaleConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(neutralScaleConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> goodScaleConfigEntry = difficultyConfig.Bind("_EventType Weights", "Good event scale", "23, -0.1, 13, 23", scaleDescription);
            eventTypeScales[3] = GetScale(goodScaleConfigEntry.Value);
            goodScaleConfigEntry.SettingChanged += (o, e) => eventTypeScales[3] = GetScale(goodScaleConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(goodScaleConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> veryGoodScaleConfigEntry = difficultyConfig.Bind("_EventType Weights", "VeryGood event scale", "3, 0.14, 3, 17", scaleDescription);
            eventTypeScales[4] = GetScale(veryGoodScaleConfigEntry.Value);
            veryGoodScaleConfigEntry.SettingChanged += (o, e) => eventTypeScales[4] = GetScale(veryGoodScaleConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(veryGoodScaleConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> removeScaleConfigEntry = difficultyConfig.Bind("_EventType Weights", "Remove event scale", "15, -0.05, 10, 15", "These events remove something   " + scaleDescription);
            eventTypeScales[5] = GetScale(removeScaleConfigEntry.Value);
            removeScaleConfigEntry.SettingChanged += (o, e) => eventTypeScales[5] = GetScale(removeScaleConfigEntry.Value);            AddConfigForLethalConfig(new TextInputFieldConfigItem(removeScaleConfigEntry, false), difficultyModInfo);

            

            ConfigEntry<string> difficultyTransitionsConfigEntry = difficultyConfig.Bind("Difficulty Scaling", "Difficulty Transitions", "Easy,00FF00,0|Medium,008000,15|Hard,FF0000,30|Very Hard,800000,50|Insane,140000,75", "Format: NAME,HEX,ABOVE, above is the value the name will be shown at.");
            difficultyTransitions = GetDifficultyTransitionsFromString(difficultyTransitionsConfigEntry.Value);
            difficultyTransitionsConfigEntry.SettingChanged += (o, e) => difficultyTransitions = GetDifficultyTransitionsFromString(difficultyTransitionsConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(difficultyTransitionsConfigEntry, false), difficultyModInfo);

            ignoreMaxCap = difficultyConfig.Bind("Difficulty Scaling", "Ignore max cap?", false, "Will ignore max cap if true, upperlimit is dictated by difficulty max cap setting as well.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(ignoreMaxCap, false), difficultyModInfo);

            difficultyMaxCap = difficultyConfig.Bind("Difficulty Scaling", "Difficulty max cap", 100.0f, "The difficulty value wont go beyond this.");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(difficultyMaxCap, false), difficultyModInfo);

            scaleByDaysPassed = difficultyConfig.Bind("Difficulty Scaling", "Scale by days passed?", true, "Will add to difficulty depending on how many days have passed.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(scaleByDaysPassed, false), difficultyModInfo);

            daysPassedDifficultyMultiplier = difficultyConfig.Bind("Difficulty Scaling", "Difficulty per days passed?", 1.0f, "");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(daysPassedDifficultyMultiplier, false), difficultyModInfo);

            daysPassedDifficultyCap = difficultyConfig.Bind("Difficulty Scaling", "Days passed difficulty cap", 60.0f, "Days passed difficulty scaling wont add beyond this.");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(daysPassedDifficultyCap, false), difficultyModInfo);

            scaleByScrapInShip = difficultyConfig.Bind("Difficulty Scaling", "Scale by scrap in ship?", true, "Will add to difficulty depending on how much scrap is inside the ship.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(scaleByScrapInShip, false), difficultyModInfo);

            scrapInShipDifficultyMultiplier = difficultyConfig.Bind("Difficulty Scaling", "Difficulty per scrap value in ship?", 0.0025f, "By default +1.0 per 400 scrap in ship");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(scrapInShipDifficultyMultiplier, false), difficultyModInfo);

            scrapInShipDifficultyCap = difficultyConfig.Bind("Difficulty Scaling", "Scrap in ship difficulty cap", 30.0f, "Scrap in ship difficulty scaling wont add beyond this.");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(scrapInShipDifficultyCap, false), difficultyModInfo);

            scaleByQuota = difficultyConfig.Bind("Difficulty Scaling", "Scale by quota?", false, "Will add to difficulty depending on how high the quota is.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(scaleByQuota, false), difficultyModInfo);

            quotaDifficultyMultiplier = difficultyConfig.Bind("Difficulty Scaling", "Difficulty per quota value?", 0.005f, "By default +1.0 per 200 quota");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(quotaDifficultyMultiplier, false), difficultyModInfo);

            quotaDifficultyCap = difficultyConfig.Bind("Difficulty Scaling", "Quota difficulty cap", 100.0f, "Quota scaling wont add difficulty beyond this");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(quotaDifficultyCap, false), difficultyModInfo);

            scaleByMoonGrade = difficultyConfig.Bind("Difficulty Scaling", "Scale by moon grade?", true, "Will add to difficulty depending on grade of moon you land on.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(scaleByMoonGrade, false), difficultyModInfo);

            ConfigEntry<string> gradeAdditivesConfigEntry = difficultyConfig.Bind("Difficulty Scaling", "Grade difficulty scaling", "D,-8|C,-8|B,-4|A,5|S,10|S+,15|S++,20|S+++,30|Other,10", "Format: GRADE,DIFFICULTY, Do not remove 'Other'");
            gradeAdditives = GetMoonRiskFromString(gradeAdditivesConfigEntry.Value);
            gradeAdditivesConfigEntry.SettingChanged += (o, e) => gradeAdditives = GetMoonRiskFromString(gradeAdditivesConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(gradeAdditivesConfigEntry, false), difficultyModInfo);

            scaleByWeather = difficultyConfig.Bind("Difficulty Scaling", "Scale by weather type?", false, "Will add to difficulty depending on weather of moon you land on.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(scaleByWeather, false), difficultyModInfo);

            weatherAdditives = new Dictionary<LevelWeatherType, float>()
            {
                { LevelWeatherType.None, 0.0f },
                { LevelWeatherType.Rainy, 2.0f },
                { LevelWeatherType.DustClouds, 2.0f },
                { LevelWeatherType.Flooded, 4.0f },
                { LevelWeatherType.Foggy, 4.0f },
                { LevelWeatherType.Stormy, 7.0f },
                { LevelWeatherType.Eclipsed, 7.0f },
            };

            ConfigEntry<float> noneWeatherDifficultyConfigEntry = difficultyConfig.Bind("Difficulty Scaling", "None weather difficulty", 0.0f, "Difficulty added for playing on None weather");
            weatherAdditives[LevelWeatherType.None] = noneWeatherDifficultyConfigEntry.Value;
            noneWeatherDifficultyConfigEntry.SettingChanged += (o, e) => weatherAdditives[LevelWeatherType.None] = noneWeatherDifficultyConfigEntry.Value;
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(noneWeatherDifficultyConfigEntry, false), difficultyModInfo);

            ConfigEntry<float> rainyWeatherDifficultyConfigEntry = difficultyConfig.Bind("Difficulty Scaling", "Rainy weather difficulty", 2.0f, "Difficulty added for playing on Rainy weather");
            weatherAdditives[LevelWeatherType.Rainy] = rainyWeatherDifficultyConfigEntry.Value;
            rainyWeatherDifficultyConfigEntry.SettingChanged += (o, e) => weatherAdditives[LevelWeatherType.Rainy] = rainyWeatherDifficultyConfigEntry.Value;
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(rainyWeatherDifficultyConfigEntry, false), difficultyModInfo);

            ConfigEntry<float> dustCloudsWeatherDifficultyConfigEntry = difficultyConfig.Bind("Difficulty Scaling", "DustClouds weather difficulty", 2.0f, "Difficulty added for playing on DustClouds weather");
            weatherAdditives[LevelWeatherType.DustClouds] = dustCloudsWeatherDifficultyConfigEntry.Value;
            dustCloudsWeatherDifficultyConfigEntry.SettingChanged += (o, e) => weatherAdditives[LevelWeatherType.DustClouds] = dustCloudsWeatherDifficultyConfigEntry.Value;
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(dustCloudsWeatherDifficultyConfigEntry, false), difficultyModInfo);

            ConfigEntry<float> floodedWeatherDifficultyConfigEntry = difficultyConfig.Bind("Difficulty Scaling", "Flooded weather difficulty", 4.0f, "Difficulty added for playing on Flooded weather");
            weatherAdditives[LevelWeatherType.Flooded] = floodedWeatherDifficultyConfigEntry.Value;
            floodedWeatherDifficultyConfigEntry.SettingChanged += (o, e) => weatherAdditives[LevelWeatherType.Flooded] = floodedWeatherDifficultyConfigEntry.Value;
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(floodedWeatherDifficultyConfigEntry, false), difficultyModInfo);

            ConfigEntry<float> foggyWeatherDifficultyConfigEntry = difficultyConfig.Bind("Difficulty Scaling", "Foggy weather difficulty", 4.0f, "Difficulty added for playing on Foggy weather");
            weatherAdditives[LevelWeatherType.Foggy] = foggyWeatherDifficultyConfigEntry.Value;
            foggyWeatherDifficultyConfigEntry.SettingChanged += (o, e) => weatherAdditives[LevelWeatherType.Foggy] = foggyWeatherDifficultyConfigEntry.Value;
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(foggyWeatherDifficultyConfigEntry, false), difficultyModInfo);

            ConfigEntry<float> stormyWeatherDifficultyConfigEntry = difficultyConfig.Bind("Difficulty Scaling", "Stormy weather difficulty", 7.0f, "Difficulty added for playing on Stormy weather");
            weatherAdditives[LevelWeatherType.Stormy] = stormyWeatherDifficultyConfigEntry.Value;
            stormyWeatherDifficultyConfigEntry.SettingChanged += (o, e) => weatherAdditives[LevelWeatherType.Stormy] = stormyWeatherDifficultyConfigEntry.Value;
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(stormyWeatherDifficultyConfigEntry, false), difficultyModInfo);

            ConfigEntry<float> eclipsedWeatherDifficultyConfigEntry = difficultyConfig.Bind("Difficulty Scaling", "Stormy weather difficulty", 7.0f, "Difficulty added for playing on Stormy weather");
            weatherAdditives[LevelWeatherType.Eclipsed] = eclipsedWeatherDifficultyConfigEntry.Value;
            eclipsedWeatherDifficultyConfigEntry.SettingChanged += (o, e) => weatherAdditives[LevelWeatherType.Eclipsed] = eclipsedWeatherDifficultyConfigEntry.Value;
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(eclipsedWeatherDifficultyConfigEntry, false), difficultyModInfo);

            

            ConfigEntry<string> spawnChanceMultiplierScalingConfigEntry = difficultyConfig.Bind("Difficulty", "Spawn chance multiplier scale", "1.0, 0.017, 1.0, 2.0", "This will multiply the spawn chance by this,   " + scaleDescription);
            spawnChanceMultiplierScaling = GetScale(spawnChanceMultiplierScalingConfigEntry.Value);
            spawnChanceMultiplierScalingConfigEntry.SettingChanged += (o, e) => spawnChanceMultiplierScaling = GetScale(spawnChanceMultiplierScalingConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(spawnChanceMultiplierScalingConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> insideSpawnChanceAdditiveConfigEntry = difficultyConfig.Bind("Difficulty", "Inside spawn chance additive", "0.0, 0.0, 0.0, 0.0", "This will add to all keyframes for insideSpawns on the animationCurve,   " + scaleDescription);
            insideSpawnChanceAdditive = GetScale(insideSpawnChanceAdditiveConfigEntry.Value);
            insideSpawnChanceAdditiveConfigEntry.SettingChanged += (o, e) => insideSpawnChanceAdditive = GetScale(insideSpawnChanceAdditiveConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(insideSpawnChanceAdditiveConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> outsideSpawnChanceAdditiveConfigEntry = difficultyConfig.Bind("Difficulty", "Outside spawn chance additive", "0.0, 0.0, 0.0, 0.0", "This will add to all keyframes for outsideSpawns on the animationCurve,   " + scaleDescription);
            outsideSpawnChanceAdditive = GetScale(outsideSpawnChanceAdditiveConfigEntry.Value);
            outsideSpawnChanceAdditiveConfigEntry.SettingChanged += (o, e) => outsideSpawnChanceAdditive = GetScale(outsideSpawnChanceAdditiveConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(outsideSpawnChanceAdditiveConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> spawnCapMultiplierConfigEntry = difficultyConfig.Bind("Difficulty", "Spawn cap multipler scale", "1.0, 0.017, 1.0, 2.0", "This will multiply outside and inside power counts by this,   " + scaleDescription);
            spawnCapMultiplier = GetScale(spawnCapMultiplierConfigEntry.Value);
            spawnCapMultiplierConfigEntry.SettingChanged += (o, e) => spawnCapMultiplier = GetScale(spawnCapMultiplierConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(spawnCapMultiplierConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> insideEnemyMaxPowerCountScalingConfigEntry = difficultyConfig.Bind("Difficulty", "Additional Inside Max Enemy Power Count", "0, 0, 0, 0", "Added max enemy power count for inside enemies.,   " + scaleDescription);
            insideEnemyMaxPowerCountScaling = GetScale(insideEnemyMaxPowerCountScalingConfigEntry.Value);
            insideEnemyMaxPowerCountScalingConfigEntry.SettingChanged += (o, e) => insideEnemyMaxPowerCountScaling = GetScale(insideEnemyMaxPowerCountScalingConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(insideEnemyMaxPowerCountScalingConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> outsideEnemyPowerCountScalingConfigEntry = difficultyConfig.Bind("Difficulty", "Additional Outside Max Enemy Power Count", "0, 0, 0, 0", "Added max enemy power count for outside enemies.,   " + scaleDescription);
            outsideEnemyPowerCountScaling = GetScale(outsideEnemyPowerCountScalingConfigEntry.Value);
            outsideEnemyPowerCountScalingConfigEntry.SettingChanged += (o, e) => outsideEnemyPowerCountScaling = GetScale(outsideEnemyPowerCountScalingConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(outsideEnemyPowerCountScalingConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> enemyBonusHpScalingConfigEntry = difficultyConfig.Bind("Difficulty", "Additional hp scale", "0, 0, 0, 0", "Added hp to all enemies,   " + scaleDescription);
            enemyBonusHpScaling = GetScale(enemyBonusHpScalingConfigEntry.Value);
            enemyBonusHpScalingConfigEntry.SettingChanged += (o, e) => enemyBonusHpScaling = GetScale(enemyBonusHpScalingConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(enemyBonusHpScalingConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> scrapValueMultiplierConfigEntry = difficultyConfig.Bind("Difficulty", "Scrap value multiplier scale", "1.0, 0.003, 1.0, 1.3", "Global scrap value multiplier,   " + scaleDescription);
            scrapValueMultiplier = GetScale(scrapValueMultiplierConfigEntry.Value);
            scrapValueMultiplierConfigEntry.SettingChanged += (o, e) => scrapValueMultiplier = GetScale(scrapValueMultiplierConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(scrapValueMultiplierConfigEntry, false), difficultyModInfo);

            ConfigEntry<string> scrapAmountMultiplierConfigEntry = difficultyConfig.Bind("Difficulty", "Scrap amount multiplier scale", "1.0, 0.003, 1.0, 1.3", "Global scrap amount multiplier,   " + scaleDescription);
            scrapAmountMultiplier = GetScale(scrapAmountMultiplierConfigEntry.Value);
            scrapAmountMultiplierConfigEntry.SettingChanged += (o, e) => scrapAmountMultiplier = GetScale(scrapAmountMultiplierConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(scrapAmountMultiplierConfigEntry, false), difficultyModInfo);

            goodEventIncrementMultiplier = difficultyConfig.Bind("Difficulty", "Global multiplier for increment value on good and veryGood eventTypes.", 1.0f);
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(goodEventIncrementMultiplier, false), difficultyModInfo);

            badEventIncrementMultiplier = difficultyConfig.Bind("Difficulty", "Global multiplier for increment value on bad and veryBad eventTypes.", 1.0f);
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(badEventIncrementMultiplier, false), difficultyModInfo);

            

            // Custom scrap settings
            nutSlayerLives = customAssetsConfig.Bind("NutSlayer", "Lives", 5, "If hp reaches zero or below, decrement lives and reset hp until 0 lives.");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(nutSlayerLives, false), customAssetsModInfo);

            nutSlayerHp = customAssetsConfig.Bind("NutSlayer", "Hp", 6);
            AddConfigForLethalConfig(new IntInputFieldConfigItem(nutSlayerHp, false), customAssetsModInfo);

            nutSlayerMovementSpeed = customAssetsConfig.Bind("NutSlayer", "Speed?", 9.5f);
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(nutSlayerMovementSpeed, false), customAssetsModInfo);

            nutSlayerImmortal = customAssetsConfig.Bind("NutSlayer", "Immortal?", true);
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(nutSlayerImmortal, false), customAssetsModInfo);

            ConfigEntry<int> grabbableTurretMinValueConfigEntry = customAssetsConfig.Bind("Grabbable Turret", "Min value", 125);
            grabbableTurret.minValue = grabbableTurretMinValueConfigEntry.Value;
            grabbableTurretMinValueConfigEntry.SettingChanged += (o, e) => grabbableTurret.minValue = grabbableTurretMinValueConfigEntry.Value;
            AddConfigForLethalConfig(new IntInputFieldConfigItem(grabbableTurretMinValueConfigEntry, false), customAssetsModInfo);

            ConfigEntry<int> grabbableTurretMaxValueConfigEntry = customAssetsConfig.Bind("Grabbable Turret", "Max value", 175);
            grabbableTurret.maxValue = grabbableTurretMaxValueConfigEntry.Value;
            grabbableTurretMaxValueConfigEntry.SettingChanged += (o, e) => grabbableTurret.maxValue = grabbableTurretMaxValueConfigEntry.Value;
            AddConfigForLethalConfig(new IntInputFieldConfigItem(grabbableTurretMaxValueConfigEntry, false), customAssetsModInfo);

            ConfigEntry<int> grabbableLandmineMinValueConfigEntry = customAssetsConfig.Bind("Grabbable Landmine", "Min value", 125);
            grabbableLandmine.minValue = grabbableLandmineMinValueConfigEntry.Value;
            grabbableLandmineMinValueConfigEntry.SettingChanged += (o, e) => grabbableLandmine.minValue = grabbableLandmineMinValueConfigEntry.Value;
            AddConfigForLethalConfig(new IntInputFieldConfigItem(grabbableLandmineMinValueConfigEntry, false), customAssetsModInfo);

            ConfigEntry<int> grabbableLandmineMaxValueConfigEntry = customAssetsConfig.Bind("Grabbable Landmine", "Max value", 175);
            grabbableLandmine.maxValue = grabbableLandmineMaxValueConfigEntry.Value;
            grabbableLandmineMaxValueConfigEntry.SettingChanged += (o, e) => grabbableLandmine.maxValue = grabbableLandmineMaxValueConfigEntry.Value;
            AddConfigForLethalConfig(new IntInputFieldConfigItem(grabbableLandmineMaxValueConfigEntry, false), customAssetsModInfo);

            slayerShotgunMinValue = customAssetsConfig.Bind("Slayer Shotgun", "Min value", 200);
            AddConfigForLethalConfig(new IntInputFieldConfigItem(slayerShotgunMinValue, false), customAssetsModInfo);

            slayerShotgunMaxValue = customAssetsConfig.Bind("Slayer Shotgun", "Max value", 300);
            AddConfigForLethalConfig(new IntInputFieldConfigItem(slayerShotgunMaxValue, false), customAssetsModInfo);

            

            // Weather settings
            useWeatherMultipliers = weatherConfig.Bind("_Weather Settings", "Enable weather multipliers?", true, "'false'= Disable all weather multipliers     'true'= Enable weather multipliers");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(useWeatherMultipliers, false), weatherModInfo);

            randomizeWeatherMultipliers = weatherConfig.Bind("_Weather Settings", "Weather multiplier randomness?", false, "'false'= disable     'true'= enable");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(randomizeWeatherMultipliers, false), weatherModInfo);

            enableTerminalText = weatherConfig.Bind("_Weather Settings", "Enable terminal text?", true);
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(enableTerminalText, false), weatherModInfo);

            weatherRandomRandomMinInclusive = weatherConfig.Bind("_Weather Random Multipliers", "Min Inclusive", 0.9f, "Lower bound of random value");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(weatherRandomRandomMinInclusive, false), weatherModInfo);

            weatherRandomRandomMaxInclusive = weatherConfig.Bind("_Weather Random Multipliers", "Max Inclusive", 1.2f, "Upper bound of random value");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(weatherRandomRandomMaxInclusive, false), weatherModInfo);

            WeatherConfig CreateWeatherSettings(Weather weather)
            {
                ConfigEntry<float> scrapValueMultiplierConfigEntry = weatherConfig.Bind($"({weather.weatherType}) Weather Multipliers", "Scrap Value Multiplier", weather.scrapValueMultiplier, $"Multiply Scrap value for {weather.weatherType}");
                ConfigEntry<float> scrapAmountMultiplierConfigEntry = weatherConfig.Bind($"({weather.weatherType}) Weather Multipliers", "Scrap Amount Multiplier", weather.scrapAmountMultiplier, $"Multiply Scrap amount for {weather.weatherType}");

                AddConfigForLethalConfig(new FloatInputFieldConfigItem(scrapAmountMultiplierConfigEntry, false), weatherModInfo);
                AddConfigForLethalConfig(new FloatInputFieldConfigItem(scrapAmountMultiplierConfigEntry, false), weatherModInfo);

                return new WeatherConfig(weather, scrapValueMultiplierConfigEntry, scrapAmountMultiplierConfigEntry);
            }

            noneMultiplier = CreateWeatherSettings(new Weather(LevelWeatherType.None, 1.00f, 1.00f));
            dustCloudMultiplier = CreateWeatherSettings(new Weather(LevelWeatherType.DustClouds, 1.05f, 1.00f));
            rainyMultiplier = CreateWeatherSettings(new Weather(LevelWeatherType.Rainy, 1.05f, 1.00f));
            stormyMultiplier = CreateWeatherSettings(new Weather(LevelWeatherType.Stormy, 1.35f, 1.20f));
            foggyMultiplier = CreateWeatherSettings(new Weather(LevelWeatherType.Foggy, 1.15f, 1.10f));
            floodedMultiplier = CreateWeatherSettings(new Weather(LevelWeatherType.Flooded, 1.25f, 1.15f));
            eclipsedMultiplier = CreateWeatherSettings(new Weather(LevelWeatherType.Eclipsed, 1.35f, 1.20f));

            

            // UI Settings
            UIKey = uiConfig.Bind("UI Options", "Toggle UI Key", "K");
            UIKey.SettingChanged += (o, e) =>
            {
                if(UI.Instance == null || UI.eventUIObject == null) return;

                UI.Instance.key = UIKey.Value.ToUpper();
                if(UI.Instance.keyboard != null && EnableUI.Value)
                {
                    UI.Instance.keyControl = UI.Instance.keyboard.FindKeyOnCurrentKeyboardLayout(UI.Instance.key);
                }
            };
            AddConfigForLethalConfig(new TextInputFieldConfigItem(UIKey, false), uiModInfo);

            NormaliseScrapValueDisplay = uiConfig.Bind("UI Options", "Normlize scrap value display number?", true, "In game default value is 0.4, having this set to true will multiply the 'displayed value' by 2.5 so it looks normal.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(NormaliseScrapValueDisplay, false), uiModInfo);

            EnableUI = uiConfig.Bind("UI Options", "Enable UI?", true);
            EnableUI.SettingChanged += (o, e) =>
            {
                if (UI.Instance == null || UI.Instance.keyboard == null || UI.eventUIObject == null) return;

                UI.eventUIObject.SetActive(EnableUI.Value);
                if(EnableUI.Value)
                {
                    UI.Instance.keyboard.onTextInput += UI.Instance.OnKeyboardInput;
                } else
                {
                    UI.Instance.keyboard.onTextInput -= UI.Instance.OnKeyboardInput;
                }

            };
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(EnableUI, false), uiModInfo);

            ShowUILetterBox = uiConfig.Bind("UI Options", "Display UI Letter Box?", true);
            ShowUILetterBox.SettingChanged += (o, e) =>
            {
                if (UI.Instance == null || UI.eventUIObject == null) return;

                UI.Instance.letterPanel.SetActive(ShowUILetterBox.Value);
            };
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(ShowUILetterBox, false), uiModInfo);

            ShowExtraProperties = uiConfig.Bind("UI Options", "Display extra properties", true, "Display extra properties on UI such as scrap value and amount multipliers.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(ShowExtraProperties, false), uiModInfo);

            PopUpUI = uiConfig.Bind("UI Options", "PopUp UI?", true, "Will the UI popup whenever you start the day?");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(PopUpUI, false), uiModInfo);

            UITimeSeconds = uiConfig.Bind("UI Options", "PopUp UI time.", 45.0f, "UI popup time in seconds.");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(UITimeSeconds, false), uiModInfo);

            scrollSpeed = uiConfig.Bind("UI Options", "Scroll speed", 1.0f, "Multiplier speed on scrolling with arrows.");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(scrollSpeed, false), uiModInfo);

            DisplayUIAfterShipLeaves = uiConfig.Bind("UI Options", "Display UI after ship leaves?", false, "Will only display event's after ship has left.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(DisplayUIAfterShipLeaves, false), uiModInfo);

            DisplayExtraPropertiesAfterShipLeaves = uiConfig.Bind("UI Options", "Display extra properties on UI after ship Leaves?", true, "This will show Event Type raritys for next day and difficulty info.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(DisplayExtraPropertiesAfterShipLeaves, false), uiModInfo);

            displayEvents = uiConfig.Bind("UI Options", "Display events?", true, "Having this set to false wont show events in the UI.");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(displayEvents, false), uiModInfo);

            /* TODOOOOOOOOOOOOOOOOOOOOOOOOOOOOO

            // Custom enemy events
            int customEventCount = customEventConfig.Bind("Custom Monster Event Count", "How many events to generate in config?", 3, "").Value;
            for (int i = 0; i < customEventCount; i++) // TODO: Custom monster events and scrap events.. later
            {
                MEvent e = new Minus.CustomEvents.CustomMonsterEvent();
                e.Initalize();
                e.InitalizeConfigEntries(customEventConfig, monsterEventsModInfo);
                EventManager.customEvents.Add(e);
            }


            foreach (EventManager.CustomEvents customevent in EventManager.customEventsList)
            {
                foreach(MEvent e in customevent.events)
                {
                    e.Initalize();
                    e.InitalizeConfigEntries(customevent.configFile, moddedEventsModInfo);
                }

                EventManager.customEvents.AddRange(customevent.events);
            }
            EventManager.customEventsList.Clear();

            */
            EventManager.events.AddRange(EventManager.vanillaEvents);
            EventManager.events.AddRange(EventManager.moddedEvents);
            EventManager.events.AddRange(EventManager.customEvents);



            // Specific event settings
            Minus.Handlers.FacilityGhost.actionTimeCooldown = eventConfig.Bind(nameof(FacilityGhost), "Normal Action Time Interval", 15.0f, "How often does it take for the ghost to make a decision?");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(Minus.Handlers.FacilityGhost.actionTimeCooldown, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.ghostCrazyActionInterval = eventConfig.Bind(nameof(FacilityGhost), "Crazy Action Time Interval", 0.1f, "How often does it take for the ghost to make a decision while going crazy?");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(Minus.Handlers.FacilityGhost.ghostCrazyActionInterval, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.ghostCrazyPeriod = eventConfig.Bind(nameof(FacilityGhost), "Crazy Period", 5.0f, "How long will the ghost go crazy for?");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(Minus.Handlers.FacilityGhost.ghostCrazyPeriod, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.crazyGhostChance = eventConfig.Bind(nameof(FacilityGhost), "Crazy Chance", 0.1f, "Whenever the ghost makes a decision, what is the chance that it will go crazy?");
            AddConfigForLethalConfig(new FloatSliderConfigItem(Minus.Handlers.FacilityGhost.crazyGhostChance, new FloatSliderOptions() { RequiresRestart = false, Min = 0.0f, Max = 1.0f }), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.DoNothingWeight = eventConfig.Bind(nameof(FacilityGhost), "Do Nothing Weight?", 25, "Whenever the ghost makes a decision, what is the weight to do nothing?");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.FacilityGhost.DoNothingWeight, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.OpenCloseBigDoorsWeight = eventConfig.Bind(nameof(FacilityGhost), "Open and close big doors weight", 20, "Whenever the ghost makes a decision, what is the weight for ghost to open and close big doors?");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.FacilityGhost.OpenCloseBigDoorsWeight, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.MessWithLightsWeight = eventConfig.Bind(nameof(FacilityGhost), "Mess with lights weight", 16, "Whenever the ghost makes a decision, what is the weight to mess with lights?");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.FacilityGhost.MessWithLightsWeight, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.MessWithBreakerWeight = eventConfig.Bind(nameof(FacilityGhost), "Mess with breaker weight", 4, "Whenever the ghost makes a decision, what is the weight to mess with the breaker?");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.FacilityGhost.MessWithBreakerWeight, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.disableTurretsWeight = eventConfig.Bind(nameof(FacilityGhost), "Disable turrets weight?", 5, "Whenever the ghost makes a decision, what is the weight to attempt to disable the turrets?");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.FacilityGhost.disableTurretsWeight, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.disableLandminesWeight = eventConfig.Bind(nameof(FacilityGhost), "Disable landmines weight?", 5, "Whenever the ghost makes a decision, what is the weight to attempt to disable the landmines?");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.FacilityGhost.disableLandminesWeight, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.turretRageWeight = eventConfig.Bind(nameof(FacilityGhost), "Turret rage weight?", 5, "Whenever the ghost makes a decision, what is the weight to attempt to make turrets rage?");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.FacilityGhost.turretRageWeight, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.OpenCloseDoorsWeight = eventConfig.Bind(nameof(FacilityGhost), "Open and close normal doors weight", 9, "Whenever the ghost makes a decision, what is the weight to attempt to open and close normal doors.");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.FacilityGhost.OpenCloseDoorsWeight, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.lockUnlockDoorsWeight = eventConfig.Bind(nameof(FacilityGhost), "Lock and unlock normal doors weight", 3, "Whenever the ghost makes a decision, what is the weight to attempt to lock and unlock normal doors.");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.FacilityGhost.lockUnlockDoorsWeight, false), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.chanceToOpenCloseDoor = eventConfig.Bind(nameof(FacilityGhost), "Chance to open and close normal doors", 0.3f, "Whenever the ghosts decides to open and close doors, what is the chance for each individual door that it will do that.");
            AddConfigForLethalConfig(new FloatSliderConfigItem(Minus.Handlers.FacilityGhost.chanceToOpenCloseDoor, new FloatSliderOptions() { RequiresRestart = false, Min = 0.0f, Max = 1.0f }), vanillaEventsModInfo);

            Minus.Handlers.FacilityGhost.rageTurretsChance = eventConfig.Bind(nameof(FacilityGhost), "Chance to rage a turret", 0.3f, "Whenever the ghosts decides to rage a turret, what is the chance for each individual turret that it will do that.");
            AddConfigForLethalConfig(new FloatSliderConfigItem(Minus.Handlers.FacilityGhost.rageTurretsChance, new FloatSliderOptions() { RequiresRestart = false, Min = 0.0f, Max = 1.0f }), vanillaEventsModInfo);



            Minus.Handlers.RealityShift.normalScrapWeight = eventConfig.Bind(nameof(RealityShift), "Normal shift weight", 85, "Weight for transforming scrap into some other scrap?");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.RealityShift.normalScrapWeight, false), vanillaEventsModInfo);

            Minus.Handlers.RealityShift.grabbableLandmineWeight = eventConfig.Bind(nameof(RealityShift), "Grabbable landmine shift weight", 15, "Weight for transforming scrap into a grabbable landmine?");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(Minus.Handlers.RealityShift.grabbableLandmineWeight, false), vanillaEventsModInfo);

            Minus.Handlers.RealityShift.transmuteChance = eventConfig.Bind(nameof(RealityShift), "Chance to transmute", 0.5f, "Chance for transmutation to occur.");
            AddConfigForLethalConfig(new FloatSliderConfigItem(Minus.Handlers.RealityShift.transmuteChance, new FloatSliderOptions() { RequiresRestart = false, Min = 0.0f, Max = 1.0f }), vanillaEventsModInfo);

            Minus.Handlers.RealityShift.enemyTeleportChance = eventConfig.Bind(nameof(RealityShift), "Enemy teleport chance", 0.2f, "Chance enemy teleportation to occur when hit.");
            AddConfigForLethalConfig(new FloatSliderConfigItem(Minus.Handlers.RealityShift.enemyTeleportChance, new FloatSliderOptions() { RequiresRestart = false, Min = 0.0f, Max = 1.0f }), vanillaEventsModInfo);



            DDay.bombardmentInterval = eventConfig.Bind(nameof(Warzone), "Bombardment interval", 100.0f, "The time it takes before each bombardment event.");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(DDay.bombardmentInterval, false), vanillaEventsModInfo);

            DDay.bombardmentTime = eventConfig.Bind(nameof(Warzone), "Bombardment time", 15.0f, "When a bombardment event occurs, how long will it last?");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(DDay.bombardmentTime, false), vanillaEventsModInfo);

            DDay.fireInterval = eventConfig.Bind(nameof(Warzone), "Fire interval", 1.0f, "During a bombardment event how often will it fire?");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(DDay.fireInterval, false), vanillaEventsModInfo);

            DDay.fireAmount = eventConfig.Bind(nameof(Warzone), "Fire amount", 8, "For every fire interval, how many shot's will it take? This will get scaled higher on bigger maps.");
            AddConfigForLethalConfig(new IntInputFieldConfigItem(DDay.fireAmount, false), vanillaEventsModInfo);

            DDay.displayWarning = eventConfig.Bind(nameof(Warzone), "Display warning?", true, "Display warning message before bombardment?");
            AddConfigForLethalConfig(new BoolCheckBoxConfigItem(DDay.displayWarning, false), vanillaEventsModInfo);

            DDay.volume = eventConfig.Bind(nameof(Warzone), "Siren Volume?", 0.3f, "Volume of the siren? between 0.0 and 1.0");
            AddConfigForLethalConfig(new FloatSliderConfigItem(DDay.volume, new FloatSliderOptions() { RequiresRestart = false, Min = 0.0f, Max = 1.0f }), vanillaEventsModInfo);

            ArtilleryShell.speed = eventConfig.Bind(nameof(Warzone), "Artillery shell speed", 100.0f, "How fast does the artillery shell travel?");
            AddConfigForLethalConfig(new FloatInputFieldConfigItem(ArtilleryShell.speed, false), vanillaEventsModInfo);



            if(Compatibility.mimicsPresent)
            {
                ConfigEntry<string> spawnRateScales0ConfigEntry = moddedEventConfig.Bind(nameof(Mimics), "Zero Mimics Scale", "0, 0, 0, 0", "Weight Scale of zero mimics spawning   " + scaleDescription);
                Minus.Handlers.Mimics.spawnRateScales[0] = GetScale(spawnRateScales0ConfigEntry.Value);
                spawnRateScales0ConfigEntry.SettingChanged += (o, e) => Minus.Handlers.Mimics.spawnRateScales[0] = GetScale(spawnRateScales0ConfigEntry.Value);
                AddConfigForLethalConfig(new TextInputFieldConfigItem(spawnRateScales0ConfigEntry, false), moddedEventsModInfo);

                ConfigEntry<string> spawnRateScales1ConfigEntry = moddedEventConfig.Bind(nameof(Mimics), "One Mimic Scale", "0, 0, 0, 0", "Weight Scale of one mimic spawning   " + scaleDescription);
                Minus.Handlers.Mimics.spawnRateScales[1] = GetScale(spawnRateScales1ConfigEntry.Value);
                spawnRateScales1ConfigEntry.SettingChanged += (o, e) => Minus.Handlers.Mimics.spawnRateScales[1] = GetScale(spawnRateScales1ConfigEntry.Value);
                AddConfigForLethalConfig(new TextInputFieldConfigItem(spawnRateScales1ConfigEntry, false), moddedEventsModInfo);

                ConfigEntry<string> spawnRateScales2ConfigEntry = moddedEventConfig.Bind(nameof(Mimics), "Two Mimics Scale", "0, 0, 0, 0", "Weight Scale of two mimics spawning   " + scaleDescription);
                Minus.Handlers.Mimics.spawnRateScales[2] = GetScale(spawnRateScales2ConfigEntry.Value);
                spawnRateScales2ConfigEntry.SettingChanged += (o, e) => Minus.Handlers.Mimics.spawnRateScales[2] = GetScale(spawnRateScales2ConfigEntry.Value);
                AddConfigForLethalConfig(new TextInputFieldConfigItem(spawnRateScales2ConfigEntry, false), moddedEventsModInfo);

                ConfigEntry<string> spawnRateScales3ConfigEntry = moddedEventConfig.Bind(nameof(Mimics), "Three Mimics Scale", "80.0, -1.25, 5.0, 80.0", "Weight Scale of three mimics spawning   " + scaleDescription);
                Minus.Handlers.Mimics.spawnRateScales[3] = GetScale(spawnRateScales3ConfigEntry.Value);
                spawnRateScales3ConfigEntry.SettingChanged += (o, e) => Minus.Handlers.Mimics.spawnRateScales[3] = GetScale(spawnRateScales3ConfigEntry.Value);
                AddConfigForLethalConfig(new TextInputFieldConfigItem(spawnRateScales3ConfigEntry, false), moddedEventsModInfo);

                ConfigEntry<string> spawnRateScales4ConfigEntry = moddedEventConfig.Bind(nameof(Mimics), "Four Mimics Scale", "40.0, -0.5, 10.0, 40.0", "Weight Scale of four mimics spawning   " + scaleDescription);
                Minus.Handlers.Mimics.spawnRateScales[4] = GetScale(spawnRateScales4ConfigEntry.Value);
                spawnRateScales4ConfigEntry.SettingChanged += (o, e) => Minus.Handlers.Mimics.spawnRateScales[4] = GetScale(spawnRateScales4ConfigEntry.Value);
                AddConfigForLethalConfig(new TextInputFieldConfigItem(spawnRateScales4ConfigEntry, false), moddedEventsModInfo);

                ConfigEntry<string> spawnRateScales5ConfigEntry = moddedEventConfig.Bind(nameof(Mimics), "Maximum Mimics Scale", "10.0, 0.84, 10.0, 60.0", "Weight Scale of maximum mimics spawning   " + scaleDescription);
                Minus.Handlers.Mimics.spawnRateScales[5] = GetScale(spawnRateScales5ConfigEntry.Value);
                spawnRateScales5ConfigEntry.SettingChanged += (o, e) => Minus.Handlers.Mimics.spawnRateScales[5] = GetScale(spawnRateScales5ConfigEntry.Value);
                AddConfigForLethalConfig(new TextInputFieldConfigItem(spawnRateScales5ConfigEntry, false), moddedEventsModInfo);
            }


            /*
            // Level properties config
            foreach(SelectableLevel level in StartOfRound.Instance.levels)
            {
                if (level == null) continue;

                LevelProperties levelProperty = new LevelProperties(level.levelID);
                levelProperty.InitalizeConfigEntries($"{level.levelID}:{level.name}", levelPropertiesConfig, levelPropertiesModInfo);
                levelProperties.TryAdd(level.levelID, levelProperty);
            }
            */
        }

        internal static bool Initalized = false;
        public static void CreateConfig()
        {
            if (Initalized) return;

            // Config
            Initalize();

            // Initalize Events
            foreach (MEvent e in EventManager.vanillaEvents)
            {
                e.Initalize();
                e.InitalizeConfigEntries(eventConfig, vanillaEventsModInfo);
            }
            foreach (MEvent e in EventManager.moddedEvents)
            {
                e.Initalize();
                e.InitalizeConfigEntries(moddedEventConfig, vanillaEventsModInfo);
            }
            

            // Create disabled events list and update
            List<MEvent> newEvents = new List<MEvent>();
            foreach (MEvent e in EventManager.events)
            {
                if (!e.enabled)
                {
                    EventManager.disabledEvents.Add(e);
                }
                else
                {
                    newEvents.Add(e);
                    switch(e.type)
                    {
                        case EventType.VeryBad:
                            EventManager.allVeryBad.Add(e);
                            break;
                        case EventType.Bad:
                            EventManager.allBad.Add(e);
                            break;
                        case EventType.Neutral:
                            EventManager.allNeutral.Add(e);
                            break;
                        case EventType.Good:
                            EventManager.allGood.Add(e);
                            break;
                        case EventType.VeryGood:
                            EventManager.allVeryGood.Add(e);
                            break;
                        case EventType.Remove:
                            EventManager.allRemove.Add(e);
                            break;
                    }
                }
            }
            
            EventManager.events = newEvents;

            EventManager.UpdateEventTypeCounts();
            EventManager.UpdateAllEventWeights();

            Log.LogInfo(
                $"\n\nTotal Events:{EventManager.events.Count},   Disabled Events:{EventManager.disabledEvents.Count},   Total Events - Remove Count:{EventManager.events.Count - EventManager.eventTypeCount[5]}\n" +
                $"Very Bad:{EventManager.eventTypeCount[0]}\n" +
                $"Bad:{EventManager.eventTypeCount[1]}\n" +
                $"Neutral:{EventManager.eventTypeCount[2]}\n" +
                $"Good:{EventManager.eventTypeCount[3]}\n" +
                $"Very Good:{EventManager.eventTypeCount[4]}\n" +
                $"Remove:{EventManager.eventTypeCount[5]}\n");

            CreateAllEnemiesConfig();
            
            uiConfig.Save();
            difficultyConfig.Save();
            eventConfig.Save();
            weatherConfig.Save();
            customAssetsConfig.Save();
            moddedEventConfig.Save();
            customEventConfig.Save();
            allEnemiesConfig.Save();
            levelPropertiesConfig.Save();
            

            BoolCheckBoxConfigItem checkBox = new BoolCheckBoxConfigItem(EnableUI);
            LethalConfigManager.AddConfigItem(checkBox);

            Initalized = true;
        }

        private static void CreateAllEnemiesConfig()
        {
            enableAllEnemies = allEnemiesConfig.Bind("_All Enemies", "Enable?", false, "This will make all enemies capable of spawning on all moons, this will make the game harder.");
            enableAllAllEnemies = allEnemiesConfig.Bind("_All All Enemies", "Enable?", false, "This will make all inside enemies spawn inside and outside and all outside enemies spawn inside and outside, so giants and worms can spawn inside, enable both 'All' and 'All All' if you are a sadist. This will make the game harder.");

            List<EnemySpawnInfo> allSpawnInfos = new List<EnemySpawnInfo>()
            {
                // Inside
                CreateEnemyEntry(EnemyName.Bracken, 8, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.HoardingBug, 60, 10, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.CoilHead, 20, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Thumper, 25, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.BunkerSpider, 35, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Jester, 7, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.SnareFlea, 45, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Hygrodere, 10, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.GhostGirl, 5, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.SporeLizard, 15, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.NutCracker, 15, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Masked, 10, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Butler, 20, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Lasso, 5, 1, SpawnLocation.Inside),
                CreateEnemyEntry(kamikazieBug.name, 30, 5, SpawnLocation.Inside),
                CreateEnemyEntry(antiCoilHead.name, 10, 2, SpawnLocation.Inside),
                CreateEnemyEntry(nutSlayer.name, 3, 1, SpawnLocation.Inside),
                // Outside
                CreateEnemyEntry(EnemyName.EyelessDog, 25, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.ForestKeeper, 10, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.EarthLeviathan, 8, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.BaboonHawk, 35, 10, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.OldBird, 6, 3, SpawnLocation.Outside)
            };

            foreach(EnemyType enemy in EnemyList.Values)
            {
                if (enemy == null || enemy.enemyPrefab == null || enemy.isDaytimeEnemy || allSpawnInfos.Exists(x => x.enemy.name == enemy.name)) continue;

                if(enemy.isOutsideEnemy)
                {
                    CreateEnemyEntry(enemy.name, 5, 1, SpawnLocation.Outside);
                } else
                {
                    CreateEnemyEntry(enemy.name, 5, 1, SpawnLocation.Inside);
                }
            }

            allEnemiesCycle = new SpawnCycle()
            {
                enemies = allSpawnInfos,
                nothingWeight = allEnemiesConfig.Bind("_All Enemies", "All enemies nothing weight", 400.0f, "This is the weight chance for a spawn to not occur.").Value,
                spawnAttemptInterval = allEnemiesConfig.Bind("_All Enemies", "Spawn interval", 86.0f, "How often will this cycle attempt to spawn an enemy? in seconds").Value,
                spawnCycleDuration = 0.0f
            };

            header = "All All Enemies";
            List<EnemySpawnInfo> allAllSpawnInfos = new List<EnemySpawnInfo>()
            {
                // Inside
                CreateEnemyEntry(EnemyName.Bracken, 8, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.HoardingBug, 60, 10, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.CoilHead, 20, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Thumper, 25, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.BunkerSpider, 35, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Jester, 7, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.SnareFlea, 45, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Hygrodere, 10, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.GhostGirl, 5, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.SporeLizard, 15, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.NutCracker, 15, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Masked, 10, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Butler, 20, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.Lasso, 5, 1, SpawnLocation.Inside),
                CreateEnemyEntry(kamikazieBug.name, 30, 5, SpawnLocation.Inside),
                CreateEnemyEntry(antiCoilHead.name, 10, 2, SpawnLocation.Inside),
                CreateEnemyEntry(nutSlayer.name, 3, 1, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.EyelessDog, 10, 5, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.ForestKeeper, 6, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.EarthLeviathan, 8, 3, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.BaboonHawk, 20, 10, SpawnLocation.Inside),
                CreateEnemyEntry(EnemyName.OldBird, 6, 3, SpawnLocation.Inside),
                // Outside
                CreateEnemyEntry(EnemyName.Bracken, 4, 1, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.HoardingBug, 30, 10, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.CoilHead, 10, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Thumper, 13, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.BunkerSpider, 18, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Jester, 3, 1, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.SnareFlea, 10, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Hygrodere, 5, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.GhostGirl, 3, 1, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.SporeLizard, 7, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.NutCracker, 8, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Masked, 5, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Butler, 10, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.Lasso, 3, 1, SpawnLocation.Outside),
                CreateEnemyEntry(kamikazieBug.name, 15, 5, SpawnLocation.Outside),
                CreateEnemyEntry(antiCoilHead.name, 5, 2, SpawnLocation.Outside),
                CreateEnemyEntry(nutSlayer.name, 2, 1, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.EyelessDog, 15, 5, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.ForestKeeper, 10, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.EarthLeviathan, 12, 3, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.BaboonHawk, 35, 10, SpawnLocation.Outside),
                CreateEnemyEntry(EnemyName.OldBird, 10, 3, SpawnLocation.Outside)
            };

            foreach (EnemyType enemy in EnemyList.Values)
            {
                if (enemy == null || enemy.enemyPrefab == null || allSpawnInfos.Exists(x => x.enemy.name == enemy.name)) continue;

                CreateEnemyEntry(enemy.name, 5, 1, SpawnLocation.Inside);
                CreateEnemyEntry(enemy.name, 5, 1, SpawnLocation.Outside);
            }

            allAllEnemiesCycle = new SpawnCycle()
            {
                enemies = allAllSpawnInfos,
                nothingWeight = allEnemiesConfig.Bind("_All All Enemies", "All enemies nothing weight", 400.0f, "This is the weight chance for a spawn to not occur.").Value,
                spawnAttemptInterval = allEnemiesConfig.Bind("_All All Enemies", "Spawn interval", 86.0f, "How often will this cycle attempt to spawn enemies? in seconds").Value,
                spawnCycleDuration = 0.0f
            };
        }

        private static string header = "All Enemies";
        private static EnemySpawnInfo CreateEnemyEntry(string enemy, float defaultWeight, int spawnCap, SpawnLocation spawnLocation)
        {
            return new EnemySpawnInfo()
            {
                enemy = GetEnemyOrDefault(enemy).enemyPrefab,
                enemyWeight = allEnemiesConfig.Bind(header, $"{spawnLocation} {enemy} Weight", defaultWeight, "weight").Value,
                spawnCap = allEnemiesConfig.Bind(header, $"{spawnLocation} {enemy} Spawn Cap", spawnCap, "weight").Value,
                spawnLocation = spawnLocation
            };
        }

        private static EnemySpawnInfo CreateEnemyEntry(EnemyName name, float defaultWeight, int spawnCap, SpawnLocation spawnLocation) => CreateEnemyEntry(EnemyNameList[name], defaultWeight, spawnCap, spawnLocation);

        public static void AddConfigForLethalConfig(BaseConfigItem configItem, ModInfo mInfo)
        {
            object _mod = ModForAssembly(mInfo);

            if (_mod == null)
            {
                Log.LogError("Failed to add config entry for lethal config");
                return;
            }

            switch (mInfo.GUID)
            {
                case uiModInfoGUID:
                    saveUIConfig = true;
                    break;
                case vanillaEventsModInfoGUID:
                    saveEventConfig = true;
                    break;
                case weatherModInfoGUID:
                    saveWeatherConfig = true;
                    break;
                case customAssetsModInfoGUID:
                    saveCustomAssetsConfig = true;
                    break;
                case difficultyModInfoGUID:
                    saveDifficultyConfig = true;
                    break;
                case moddedEventsModInfoGUID:
                    saveModdedEventConfig = true;
                    break;
                case monsterEventsModInfoGUID:
                case scrapEventModInfoGUID:
                    saveCustomEventConfig = true;
                    break;
                case spawnCyclesModInfoGUID:
                    saveAllEnemiesConfig = true;
                    break;
                case levelPropertiesModInfoGUID:
                    saveLevelPropertiesConfig = true;
                    break;
            }

            baseConfigItem_Owner.SetValue(configItem, _mod);

            mod_AddConfigItem.Invoke(_mod, new object[] { configItem });
        }

        private static object ModForAssembly(ModInfo mInfo)
        {
            IDictionary modsDictionary = lethalConfigManager_Mods.GetValue(null) as IDictionary;
            object _mod;
            try
            {
                _mod = modsDictionary[mInfo.GUID];

                if(_mod != null)
                {
                    return _mod;
                }
            } catch
            {

            }

            object _modInfo = Activator.CreateInstance(modInfo, true);

            modInfo_Name.SetValue(_modInfo, mInfo.NAME);
            modInfo_Guid.SetValue(_modInfo, mInfo.GUID);
            modInfo_Version.SetValue(_modInfo, mInfo.VERSION);
            modInfo_Description.SetValue(_modInfo, mInfo.DESCRIPTION);

            _mod = Activator.CreateInstance(mod, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { _modInfo }, null, null);

            IDictionary modsToAssemblyMapDictionary = lethalConfigManager_ModToAssemblyMap.GetValue(null) as IDictionary;

            try
            {
                modsDictionary.Add(mInfo.GUID, _mod);
            } catch
            {
                return null;
            }

            try
            {
                modsToAssemblyMapDictionary.Add(_mod, Assembly.GetExecutingAssembly());
            } catch
            {
                return null;
            }


            return _mod;
        }

        public class ConfigMenuPatch
        {
            public static MethodBase TargetMethod()
            {
                return Assembly.GetAssembly(typeof(BaseConfigItem)).GetType("LethalConfig.MonoBehaviours.ConfigMenu").GetMethod("OnApplyButtonClicked", BindingFlags.Instance | BindingFlags.Public);
            }

            public static void Postfix()
            {
                if(saveUIConfig)
                {
                    uiConfig.Save();
                    saveUIConfig = false;
                }
                if(saveEventConfig)
                {
                    eventConfig.Save();
                    saveEventConfig = false;
                }
                if(saveWeatherConfig)
                {
                    weatherConfig.Save();
                    saveWeatherConfig = false;
                }
                if(saveCustomAssetsConfig)
                {
                    customAssetsConfig.Save();
                    saveCustomAssetsConfig = false;
                }
                if(saveDifficultyConfig)
                {
                    difficultyConfig.Save();
                    saveDifficultyConfig = false;
                }
                if(saveModdedEventConfig)
                {
                    moddedEventConfig.Save();
                    saveModdedEventConfig = false;
                }
                if(saveCustomEventConfig)
                {
                    customEventConfig.Save();
                    saveCustomEventConfig = false;
                }
                if(saveAllEnemiesConfig)
                {
                    allEnemiesConfig.Save();
                    saveAllEnemiesConfig = false;
                }
                if(saveLevelPropertiesConfig)
                {
                    levelPropertiesConfig.Save();
                    saveLevelPropertiesConfig = false;
                }
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TimeOfDay), "Awake")] 
        private static void OnTimeDayStart(ref TimeOfDay __instance)
        {
            enableQuotaChanges = difficultyConfig.Bind("Quota Settings", "_Enable Quota Changes", false, "Once set to true, load up a save to generate the rest of this config, this will take values from the game as default.");
            if(enableQuotaChanges.Value)
            {
                __instance.quotaVariables.deadlineDaysAmount = difficultyConfig.Bind("Quota Settings", "Deadline Days Amount", __instance.quotaVariables.deadlineDaysAmount).Value;
                __instance.quotaVariables.startingCredits = difficultyConfig.Bind("Quota Settings", "Starting Credits", __instance.quotaVariables.startingCredits).Value;
                __instance.quotaVariables.startingQuota = difficultyConfig.Bind("Quota Settings", "Starting Quota", __instance.quotaVariables.startingQuota).Value;
                __instance.quotaVariables.baseIncrease = difficultyConfig.Bind("Quota Settings", "Base Increase", __instance.quotaVariables.baseIncrease).Value;
                __instance.quotaVariables.increaseSteepness = difficultyConfig.Bind("Quota Settings", "Increase Steepness", __instance.quotaVariables.increaseSteepness).Value;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Terminal), "Awake")]
        private static void OnTerminalAwake()
        {
            Manager.currentTerminal = GameObject.FindObjectOfType<Terminal>();
        }

        public class ModInfo : IEquatable<ModInfo>
        {
            public string GUID;
            public string NAME;
            public string VERSION;
            public string DESCRIPTION;

            public ModInfo(string GUID, string NAME, string VERSION, string DESCRIPTION)
            {
                this.GUID = GUID;
                this.NAME = NAME;
                this.VERSION = VERSION;
                this.DESCRIPTION = DESCRIPTION;
            }

            public bool Equals(ModInfo other)
            {
                return GUID == other.GUID;
            }
        }
    }
}
