using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class TheFiend : MEvent
    {
        public override string Name() => nameof(TheFiend);

        public static TheFiend Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 3;
            descriptions = new List<string>() { "The fiend is inside the facility", "Comes with jumpscares", "Dont flash it..." };
            colorHex = "#800000";
            type = EventType.VeryBad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                "TheFiend",
                new Scale(8.0f, 0.4f, 8.0f, 32.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(1.0f, 0.034f, 1.0f, 3.0f),
                new Scale(0.0f, 0.022f, 0.0f, 1.0f),
                new Scale(0.0f, 0.034f, 0.0f, 2.0f))
            };
        }

        public override bool AddEventIfOnly() => Compatibility.scopophobiaPresent;

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
