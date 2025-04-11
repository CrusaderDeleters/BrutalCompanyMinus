using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutalCompanyMinus.Minus
{
    public class WeatherConfig
    {
        public Weather weather;

        public ConfigEntry<float> scrapValueMultiplierConfigEntry;

        public ConfigEntry<float> scrapAmountMultiplierConfigEntry;

        public WeatherConfig(Weather weather, ConfigEntry<float> scrapValueMultiplierConfigEntry, ConfigEntry<float> scrapAmountMultiplierConfigEntry)
        {
            this.weather = weather;

            this.scrapValueMultiplierConfigEntry = scrapValueMultiplierConfigEntry;
            this.weather.scrapValueMultiplier = this.scrapValueMultiplierConfigEntry.Value;
            this.scrapValueMultiplierConfigEntry.SettingChanged += (o, e) => this.weather.scrapValueMultiplier = this.scrapValueMultiplierConfigEntry.Value;

            this.scrapAmountMultiplierConfigEntry = scrapAmountMultiplierConfigEntry;
            this.weather.scrapAmountMultiplier = this.scrapAmountMultiplierConfigEntry.Value;
            this.scrapAmountMultiplierConfigEntry.SettingChanged += (o, e) => this.weather.scrapAmountMultiplier = this.scrapAmountMultiplierConfigEntry.Value;
        }
    }
}
