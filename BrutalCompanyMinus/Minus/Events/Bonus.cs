using BrutalCompanyMinus.Minus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Bonus : MEvent
    {
        public override string Name() => nameof(Bonus);

        public static Bonus Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 2;
            descriptions = new List<string>() { "Corporate is feeling good today.", "The company is giving you credits for existing", "■ ■ ■", "It's never enough." };
            colorHex = "#008000";
            type = EventType.Good;

            scaleList.Add(ScaleType.MinCash, new Scale(75.0f, 2.25f, 75.0f, 300.0f));
            scaleList.Add(ScaleType.MaxCash, new Scale(125.0f, 3.75f, 125.0f, 400.0f));
        }

        public override void Execute() => Manager.PayCredits(UnityEngine.Random.Range(Get(ScaleType.MinCash), Get(ScaleType.MaxCash) + 1));
    }
}
