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
    internal class Raining : MEvent
    {
        public override string Name() => nameof(Raining);

        public static Raining Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 8;
            descriptions = new List<string>() { "It's raining out here", "Rain...", "The only thing you will see in Ireland" };
            colorHex = "#FFFFFF";
            type = EventType.Neutral;
        }

        public override bool AddEventIfOnly()
        {
            switch(RoundManager.Instance.currentLevel.currentWeather)
            {
                case LevelWeatherType.Rainy: return false;
                case LevelWeatherType.Stormy: return false;
                case LevelWeatherType.Flooded: return false;
            }
            return true;
        }

        public override void Execute() => Manager.SetAtmosphere(Assets.AtmosphereName.Rainy, true);
    }
}
