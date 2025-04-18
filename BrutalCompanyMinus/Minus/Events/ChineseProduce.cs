﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class ChineseProduce : MEvent
    {
        public override string Name() => nameof(ChineseProduce);

        public static ChineseProduce Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "Everything here is made cheaply...", "Who produced this crap...?", "Budget scrap...", "Quantity over quality." };
            ColorHex = "#800000";
            Type = EventType.VeryBad;

            scaleList.Add(ScaleType.ScrapValue, new Scale(0.6f, 0.0f, 0.6f, 0.6f));
            scaleList.Add(ScaleType.ScrapAmount, new Scale(2.0f, 0.0f, 2.0f, 2.0f));
        }

        public override void Execute()
        {
            Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
        }
    }
}
