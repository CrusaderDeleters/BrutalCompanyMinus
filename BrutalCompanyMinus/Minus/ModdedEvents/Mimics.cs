using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Mimics : MEvent
    {
        public override string Name() => nameof(Mimics);

        public static Mimics Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 3;
            descriptions = new List<string>() { "Mimics!", "The fire exit's are suspicous", "Dont be hasty!" };
            colorHex = "#FF0000";
            type = EventType.Bad;
        }

        public override bool AddEventIfOnly() => Compatibility.mimicsPresent;

        public override void Execute() => Handlers.Mimics.MoreMimics();

        public override void OnShipLeave() => Handlers.Mimics.Reset();

        public override void OnGameStart() => Handlers.Mimics.Reset();
    }
}
