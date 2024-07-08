using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using static BrutalCompanyMinus.Minus.MEvent;

namespace BrutalCompanyMinus.Minus
{
    public class ScrapTransmutationEvent
    {
        internal ConfigEntry<string> amountConfigEntry;
        public Scale amount; // Between 0.0 to 1.0

        internal List<ConfigEntry<string>> itemsConfigEntry = new List<ConfigEntry<string>>();
        public SpawnableItemWithRarity[] items;

        public ScrapTransmutationEvent(Scale amount, params SpawnableItemWithRarity[] items)
        {
            this.items = items;
            this.amount = amount;
        }

        public void Execute() => Manager.TransmuteScrap(amount.Computef(EventType.Neutral), items);
    }
}
