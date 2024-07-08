using BepInEx.Configuration;
using LethalConfig.ConfigItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.CustomEvents
{
    internal class CustomMonsterEvent : MEvent
    {
        public ConfigEntry<string> name;

        private int previousMonsterEventCount = 0;
        private int highestMonsterCount = 0;
        public ConfigEntry<int> monsterEventAmount;

        public override string Name() => name.Value;

        public override void Initalize()
        {
            monsterEventAmount = Configuration.customEventConfig.Bind("Custom Events", "Monster Event count", 3, "How many monster events to generate in the config?");
            previousMonsterEventCount = monsterEventAmount.Value;
            highestMonsterCount = monsterEventAmount.Value;
            monsterEventAmount.SettingChanged += (o, e) =>
            {
                monsterEventAmount.Value = Mathf.Clamp(monsterEventAmount.Value, 0, 50);

                if(monsterEventAmount.Value > previousMonsterEventCount)
                {
                    if(monsterEventAmount.Value <= highestMonsterCount)
                    {

                    } else
                    {
                        List<MonsterEvent> newMonsterEventList = new List<MonsterEvent>();
                        foreach (MonsterEvent monsterEvent in monsterEvents)
                        {
                            newMonsterEventList.Add(monsterEvent);
                        }

                        for (int i = 0; i < monsterEventAmount.Value - previousMonsterEventCount; i++)
                        {
                            MonsterEvent newMonsterEvent = new MonsterEvent(Assets.GetEnemy(Assets.EnemyName.HoardingBug), new Scale(), new Scale(), new Scale(), new Scale(), new Scale(), new Scale());
                            newMonsterEvent.InitalizeConfigWithEnemyName(Name(), Configuration.customEventConfig, Configuration.monsterEventsModInfo);
                            newMonsterEventList.Add(newMonsterEvent);
                        }

                        monsterEvents.Clear();
                        foreach (MonsterEvent monsterEvent in newMonsterEventList)
                        {
                            monsterEvents.Add(monsterEvent);
                        }
                    }
                } else if(monsterEventAmount.Value < previousMonsterEventCount)
                {
                    monsterEvents.RemoveRange(monsterEventAmount.Value - 1, previousMonsterEventCount - monsterEventAmount.Value);
                }

                if(monsterEventAmount.Value > highestMonsterCount)
                {
                    highestMonsterCount = monsterEventAmount.Value;
                }
                previousMonsterEventCount = monsterEventAmount.Value;
            };
            Configuration.AddConfigForLethalConfig(new IntInputFieldConfigItem(monsterEventAmount, false), Configuration.monsterEventsModInfo);

            enabled = false;
            weight = 0;
            descriptions = new List<string>() { "Descriptions..." };
            colorHex = "#FF0000";
            type = EventType.Neutral;

        }

        public override void Execute() => ExecuteAllMonsterEvents();

        public override void InitalizeConfigEntries(ConfigFile to, Configuration.ModInfo info)
        {
            InitalizeBasicEntries(to, info);

            foreach(MonsterEvent e in monsterEvents)
            {
                e.InitalizeConfigWithEnemyName(Name(), to, info);
            }
        }
    }
}
