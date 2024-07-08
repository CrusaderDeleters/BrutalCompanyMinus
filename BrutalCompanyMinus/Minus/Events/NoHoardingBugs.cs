using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoHoardingBugs : MEvent
    {
        public override string Name() => nameof(NoHoardingBugs);

        public static NoHoardingBugs Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 1;
            descriptions = new List<string>() { "No critters", "No cuties", "Why are they gone :(" };
            colorHex = "#008000";
            type = EventType.Remove;

            eventsToRemove = new List<string>() { nameof(HoardingBugs), nameof(BugHorde), nameof(KamikazieBugs), nameof(Hell), nameof(HolidaySeason) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.HoardingBug);

        public override void Execute() => Manager.RemoveSpawn(Assets.EnemyName.HoardingBug);
    }
}
