using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Animations;
using static BrutalCompanyMinus.Minus.MEvent;
using static BrutalCompanyMinus.Configuration;
using static BrutalCompanyMinus.Helper;
using LethalConfig.ConfigItems;

namespace BrutalCompanyMinus.Minus
{
    public class LevelProperties
    {
        public int levelID;

        public ConfigEntry<string> minScrapAmountConfigEntry, maxScrapAmountConfigEntry;

        public ConfigEntry<string> minScrapValueConfigEntry, maxScrapValueConfigEntry;

        public Scale minScrapAmount, maxScrapAmount;

        public Scale minScrapValue, maxScrapValue;

        public LevelProperties(int levelID, Scale minScrapAmount, Scale maxScrapAmount, Scale minScrapValue, Scale maxScrapValue)
        {
            this.levelID = levelID;
            this.minScrapAmount = minScrapAmount;
            this.maxScrapAmount = maxScrapAmount;
            this.minScrapValue = minScrapValue;
            this.maxScrapValue = maxScrapValue;
        }

        public LevelProperties(int levelID)
        {
            this.levelID = levelID;
        }

        public void InitalizeConfigEntries(string header, ConfigFile to, ModInfo info)
        {
            
            minScrapAmountConfigEntry = to.Bind(header, "Min scrap amount scale", "1.0, 0.0, 1.0, 1.0", scaleDescription);
            minScrapAmount = GetScale(minScrapAmountConfigEntry.Value);
            minScrapAmountConfigEntry.SettingChanged += (o, e) => minScrapAmount = GetScale(minScrapAmountConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(minScrapValueConfigEntry, false), info);

            maxScrapAmountConfigEntry = to.Bind(header, "Max scrap amount scale", "1.0, 0.0, 1.0, 1.0", scaleDescription);
            maxScrapAmount = GetScale(maxScrapAmountConfigEntry.Value);
            maxScrapAmountConfigEntry.SettingChanged += (o, e) => maxScrapAmount = GetScale(maxScrapAmountConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(maxScrapAmountConfigEntry, false), info);

            minScrapValueConfigEntry = to.Bind(header, "Min scrap value scale", "1.0, 0.0, 1.0, 1.0", scaleDescription);
            minScrapValue = GetScale(minScrapValueConfigEntry.Value);
            minScrapValueConfigEntry.SettingChanged += (o, e) => minScrapValue = GetScale(minScrapValueConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(minScrapValueConfigEntry, false), info);

            maxScrapValueConfigEntry = to.Bind(header, "Max scrap value scale", "1.0, 0.0, 1.0, 1.0", scaleDescription);
            maxScrapValue = GetScale(maxScrapValueConfigEntry.Value);
            maxScrapValueConfigEntry.SettingChanged += (o, e) => maxScrapValue = GetScale(maxScrapValueConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(maxScrapValueConfigEntry, false), info);
        }

        public float GetScrapAmountMultiplier() => UnityEngine.Random.Range(minScrapAmount.Computef(EventType.Neutral), maxScrapAmount.Computef(EventType.Neutral));

        public float GetScrapValueMultiplier() => UnityEngine.Random.Range(minScrapValue.Computef(EventType.Neutral), maxScrapValue.Computef(EventType.Neutral));
    }
}
