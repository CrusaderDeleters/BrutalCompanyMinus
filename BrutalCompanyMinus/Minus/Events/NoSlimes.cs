using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoSlimes : MEvent
    {
        public override string Name() => nameof(NoSlimes);

        public static NoSlimes Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 1;
            descriptions = new List<string>() { "No goo", "No slimes", "A mysterious force repels slimes", "The absence of slimes bring a calm to this planet." };
            colorHex = "#008000";
            type = EventType.Remove;

            eventsToRemove = new List<string>() { nameof(Slimes), nameof(Hell) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.Hygrodere);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.Hygrodere);
    }
}
