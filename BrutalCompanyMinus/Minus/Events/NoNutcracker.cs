﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoNutcracker : MEvent
    {
        public override string Name() => nameof(NoNutcracker);

        public static NoNutcracker Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "No Nutcrackers", "You are allowed to move.", "You won't need to bring your metal codpiece here."};
            ColorHex = "#008000";
            Type = EventType.Remove;

            eventsToRemove = new List<string>() { nameof(Nutcracker), nameof(NutSlayer), nameof(Hell), nameof(HolidaySeason) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.NutCracker);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.NutCracker);
    }
}
