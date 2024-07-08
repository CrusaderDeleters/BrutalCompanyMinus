using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class Lizard : MEvent
    {
        public override string Name() => nameof(Lizard);

        public static Lizard Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 3;

            descriptions = new List<string>() { "They dont bite... i swear", "Annoying ones...", "MOVE!!!!!!!!", "These will fart on you, And it isn't pleasent." };
            colorHex = "#FF0000";
            type = EventType.Bad;

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.SporeLizard,
                new Scale(10.0f, 0.4f, 10.0f, 50.0f),
                new Scale(5.0f, 0.1f, 5.0f, 15.0f),
                new Scale(1.0f, 0.02f, 1.0f, 3.0f),
                new Scale(2.0f, 0.04f, 2.0f, 6.0f),
                new Scale(0.0f, 0.02f, 0.0f, 1.0f),
                new Scale(0.0f, 0.03f, 0.0f, 3.0f))
            };
        }

        public override void Execute() => ExecuteAllMonsterEvents();
    }
}
