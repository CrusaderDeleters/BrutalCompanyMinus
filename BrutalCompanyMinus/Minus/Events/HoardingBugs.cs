using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class HoardingBugs : MEvent
    {
        public override string Name() => nameof(HoardingBugs);

        public static HoardingBugs Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 3;
            descriptions = new List<string>() { "They look cute.", "Best served bonked", "Pretty innocent" };
            colorHex = "#FF0000";
            type = EventType.Bad;

            eventsToSpawnWith = new List<string>() { nameof(ScarceOutsideScrap) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.HoardingBug,
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(4.0f, 0.08f, 4.0f, 12.0f),
                new Scale(2.0f, 0.08f, 2.0f, 10.0f),
                new Scale(3.0f, 0.12f, 3.0f, 15.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f)),
            };
        }

        public override bool AddEventIfOnly()
        {
            if (!Manager.transmuteScrap)
            {
                Manager.transmuteScrap = true;
                return true;
            }
            return false;
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
