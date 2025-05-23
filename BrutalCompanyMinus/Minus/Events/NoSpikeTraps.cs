﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class NoSpikeTraps : MEvent
    {
        public override string Name() => nameof(NoSpikeTraps);

        public static NoSpikeTraps Instance;

        public override void Initalize()
        {
            Instance = this;

            Weight = 1;
            Descriptions = new List<string>() { "No spikes!", "No roof traps", "No hydraulic press" };
            ColorHex = "#008000";
            Type = EventType.Remove;

            eventsToRemove = new List<string>() { nameof(SpikeTraps), nameof(Hell) };
        }

        public override bool AddEventIfOnly() => RoundManager.Instance.currentLevel.spawnableMapObjects.ToList().Exists(x => x.prefabToSpawn.name == Assets.ObjectNameList[Assets.ObjectName.SpikeRoofTrap]);

        public override void Execute()
        {
            AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f));

            foreach (SpawnableMapObject obj in RoundManager.Instance.currentLevel.spawnableMapObjects)
            {
                if (obj.prefabToSpawn.name == Assets.ObjectNameList[Assets.ObjectName.SpikeRoofTrap])
                {
                    obj.numberToSpawn = curve;
                }
            }
        }

    }
}
