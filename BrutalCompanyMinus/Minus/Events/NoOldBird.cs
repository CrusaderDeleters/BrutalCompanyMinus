using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoOldBird : MEvent
    {
        public override string Name() => nameof(NoOldBird);

        public static NoOldBird Instance;

        public override void Initalize()
        {
            Instance = this;

            eventsToRemove = new List<string>() { nameof(OldBirds), nameof(Hell) };

            weight = 1;
            descriptions = new List<string>() { "No robots", "No deranged children", "No more giant killers" };
            colorHex = "#008000";
            type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.OldBird);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.OldBird);
    }
}
