using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoMasks : MEvent
    {
        public override string Name() => nameof(NoMasks);

        public static NoMasks Instance;

        public override void Initalize()
        {
            Instance = this;

            eventsToRemove = new List<string>() { nameof(Hell), nameof(Masked), nameof(NutSlayer) };

            weight = 1;
            descriptions = new List<string>() { "No friends :(", "No more hugs", "No more trust issues" };
            colorHex = "#008000";
            type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.Masked);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.Masked);
    }
}
