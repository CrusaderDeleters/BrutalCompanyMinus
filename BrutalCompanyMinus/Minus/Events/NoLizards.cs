using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoLizards : MEvent
    {
        public override string Name() => nameof(NoLizards);

        public static NoLizards Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 1;
            descriptions = new List<string>() { "No lizards", "No immortal things for whatever reason", "No gas gas gas" };
            colorHex = "#008000";
            type = EventType.Remove;
            eventsToRemove = new List<string>() { nameof(Lizard) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.SporeLizard);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.SporeLizard);
    }
}
