using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoWorm : MEvent
    {
        public override string Name() => nameof(NoWorm);

        public static NoWorm Instance;

        public override void Initalize()
        {
            Instance = this;

            eventsToRemove = new List<string>() { nameof(Hell), nameof(Worms) };

            weight = 1;
            descriptions = new List<string>() { "No worms", "No leviathans", "The earth is safe", "No delicacy" };
            colorHex = "#008000";
            type = EventType.Remove;
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.EarthLeviathan);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.EarthLeviathan);
    }
}
