using BrutalCompanyMinus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class OutsideLandmines : MEvent
    {
        public override string Name() => nameof(OutsideLandmines);

        public static OutsideLandmines Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 3;
            descriptions = new List<string>() { "There are landmines, Outside.", "This facility also has setup it's own booby traps outside", "Watch your step... but outside", "Iraq" };
            colorHex = "#FF0000";
            type = EventType.Bad;

            scaleList.Add(ScaleType.MinDensity, new Scale(0.003f, 0.00012f, 0.003f, 0.015f));
            scaleList.Add(ScaleType.MaxDensity, new Scale(0.0042f, 0.000168f, 0.0042f, 0.021f));
        }

        public override void Execute()
        {
            Manager.insideObjectsToSpawnOutside.Add(new Manager.ObjectInfo(Assets.GetObject(Assets.ObjectName.Landmine), UnityEngine.Random.Range(Getf(ScaleType.MinDensity), Getf(ScaleType.MaxDensity))));
        }
    }
}
