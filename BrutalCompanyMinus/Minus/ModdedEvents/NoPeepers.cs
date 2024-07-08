using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoPeepers : MEvent
    {
        public override string Name() => nameof(NoPeepers);

        public static NoPeepers Instance;

        public static float oldSpawnChance = -1.0f;

        public override void Initalize()
        {
            Instance = this;

            eventsToRemove = new List<string>() { nameof(Hell), nameof(Peepers) };

            weight = 1;
            descriptions = new List<string>() { "No weights", "The air feels light." };
            colorHex = "#008000";
            type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Compatibility.peepersPresent;

        public override void Execute()
        {
            oldSpawnChance = (float)Compatibility.peeperSpawnChance.GetValue(null);
            Compatibility.peeperSpawnChance.SetValue(null, -1.0f);
        }

        public override void OnGameStart()
        {
            if (!Compatibility.peepersPresent) return;
            Compatibility.peeperSpawnChance.SetValue(null, oldSpawnChance);
        }

        public override void OnShipLeave()
        {
            if (!Compatibility.peepersPresent) return;
            Compatibility.peeperSpawnChance.SetValue(null, oldSpawnChance);
        }
    }
}
