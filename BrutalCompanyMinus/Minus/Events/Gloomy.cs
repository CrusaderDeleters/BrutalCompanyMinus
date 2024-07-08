using BrutalCompanyMinus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Gloomy : MEvent
    {
        public override string Name() => nameof(Gloomy);

        public static Gloomy Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 8;
            descriptions = new List<string>() { "It's gloomy out here", "Misty", "Who turned on the fog machine?" };
            colorHex = "#FFFFFF";
            type = EventType.Neutral;
        }

        public override bool AddEventIfOnly()
        {
            switch(RoundManager.Instance.currentLevel.currentWeather)
            {
                case LevelWeatherType.Eclipsed: return false;
                case LevelWeatherType.Stormy: return false;
                case LevelWeatherType.Flooded: return false;
                case LevelWeatherType.Foggy: return false;
            }
            return true;
        }

        public override void Execute() => Manager.SetAtmosphere(Assets.AtmosphereName.Foggy, true);
    }
}
