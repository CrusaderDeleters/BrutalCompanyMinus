using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoGhosts : MEvent
    {
        public override string Name() => nameof(NoGhosts);

        public static NoGhosts Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 1;
            descriptions = new List<string>() { "No ghosts", "No more paranormal activity", "The ghost busters have cleared this facility of ghosts." };
            colorHex = "#008000";
            type = EventType.Remove;

            eventsToRemove = new List<string>() { nameof(LittleGirl), nameof(FacilityGhost), nameof(Hell), nameof(Walkers), nameof(Herobrine), nameof(SlenderMan) };
        }

        public override bool AddEventIfOnly() => Manager.SpawnExists(Assets.EnemyName.GhostGirl) || Manager.SpawnExists("WalkerType") || Manager.SpawnExists("Herobrine") || Manager.SpawnExists("Football");

        public override void Execute()
        {
            Manager.RemoveSpawn(Assets.EnemyName.GhostGirl);
            Manager.RemoveSpawn("WalkerType");
            Manager.RemoveSpawn("Herobrine");
            Manager.RemoveSpawn("Football");
        }
    }
}
