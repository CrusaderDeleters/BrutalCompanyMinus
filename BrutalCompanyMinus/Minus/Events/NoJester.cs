using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoJester : MEvent
    {
        public override string Name() => nameof(NoJester);

        public static NoJester Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 1;
            descriptions = new List<string>() { "No more jackbox's", "No cranking", "You dont need to go home today" };
            colorHex = "#008000";
            type = EventType.Remove;

            eventsToRemove = new List<string>() { nameof(Jester), nameof(Hell) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.Jester);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.Jester);
    }
}
