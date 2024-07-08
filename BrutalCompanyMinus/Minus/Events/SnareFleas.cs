using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class SnareFleas : MEvent
    {
        public override string Name() => nameof(SnareFleas);

        public static SnareFleas Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 3;
            descriptions = new List<string>() { "Ceiling campers!", "A delicacy", "The finest of creatures", "Look up", "Look down" };
            colorHex = "#FF0000";
            type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.SnareFlea,
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(2.0f, 0.08f, 2.0f, 10.0f),
                new Scale(1.0f, 0.05f, 1.0f, 6.0f),
                new Scale(2.0f, 0.08f, 2.0f, 10.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
