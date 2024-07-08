using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoBaboons : MEvent
    {
        public override string Name() => nameof(NoBaboons);

        public static NoBaboons Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 1;
            descriptions = new List<string>() { "No gangs", "No stealing", "It's free real estate" };
            colorHex = "#008000";
            type = EventType.Remove;

            eventsToRemove = new List<string>() { nameof(BaboonHorde), nameof(Hell) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.BaboonHawk);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.BaboonHawk);
    }
}
