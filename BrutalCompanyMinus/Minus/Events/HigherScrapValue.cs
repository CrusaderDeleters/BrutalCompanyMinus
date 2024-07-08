using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class HigherScrapValue : MEvent
    {
        public override string Name() => nameof(HigherScrapValue);

        public static HigherScrapValue Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 2;
            descriptions = new List<string>() { "Everything is worth slightly more!", "Premium scrap", "Gucci scrap" };
            colorHex = "#008000";
            type = EventType.Good;

            scaleList.Add(ScaleType.ScrapValue, new Scale(1.1f, 0.007f, 1.1f, 1.8f));
        }

        public override void Execute() => Manager.scrapValueMultiplier *= Getf(ScaleType.ScrapValue);
    }
}
