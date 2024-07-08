using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class BadDice : MEvent
    {
        public override string Name() => nameof(BadDice);

        public static BadDice Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 2;
            descriptions = new List<string>() { "Bad Dice!!!!", "You wont make it big today", "Horrible luck is bestowed upon you!" };
            colorHex = "#800000";
            type = EventType.VeryBad;

            scrapTransmutationEvent = new ScrapTransmutationEvent(
                new Scale(0.5f, 0.008f, 0.5f, 0.9f),
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("MysteryDiceItem"), rarity = 80 },
                new SpawnableItemWithRarity() { spawnableItem = Assets.GetItem("Sacrificer"), rarity = 20 }
            );

            eventsToRemove = new List<string>() { nameof(RealityShift) };

            scaleList.Add(ScaleType.ScrapAmount, new Scale(1.0f, 0.005f, 1.0f, 1.5f));
        }

        public override bool AddEventIfOnly()
        {
            if (!Compatibility.emergencyDicePresent) return false;
            if (!Manager.transmuteScrap)
            {
                Manager.transmuteScrap = true;
                return true;
            }
            return false;
        }
        
        public override void Execute()
        {
            Manager.scrapAmountMultiplier *= Getf(ScaleType.ScrapAmount);
            scrapTransmutationEvent.Execute();
        }
    }
}
