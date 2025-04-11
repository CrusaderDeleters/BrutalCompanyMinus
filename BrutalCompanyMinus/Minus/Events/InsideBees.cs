﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace BrutalCompanyMinus.Minus.Events
{
    internal class InsideBees : MEvent
    {
        public override string Name() => nameof(InsideBees);

        public static InsideBees Instance;

        public override void Initalize()
        {
            Instance = this;

            weight = 1;
            descriptions = new List<string>() { "BEES!! wait...", "The facility is abuzz!", "Bee careful", "The inside is sweet", "Why was the bee fired from the barbershop? He only knew how to give a buzz cut." };
            colorHex = "#800000";
            type = EventType.VeryBad;

            eventsToRemove = new List<string>() { nameof(Bees) };

            monsterEvents = new List<MonsterEvent>() { new MonsterEvent(
                Assets.EnemyName.CircuitBee,
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(0.0f, 0.0f, 0.0f, 0.0f),
                new Scale(3.0f, 0.05f, 2.0f, 9.0f),
                new Scale(4.0f, 0.084f, 3.0f, 12.0f),
                new Scale(3.0f, 0.06f, 3.0f, 9.0f),
                new Scale(5.0f, 0.1f, 5.0f, 15.0f))
            };

            scaleList.Add(ScaleType.DaytimeEnemyRarity, new Scale(20.0f, 0.8f, 20.0f, 100.0f));
        }

        public override void Execute()
        {
            EnemyType bee = Assets.GetEnemy(Assets.EnemyName.CircuitBee);

            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, bee, monsterEvents[0].insideSpawnRarity.Compute(type));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, bee, monsterEvents[0].outsideSpawnRarity.Compute(type));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.DaytimeEnemies, bee, Get(ScaleType.DaytimeEnemyRarity));

            Manager.Spawn.InsideEnemies(bee, UnityEngine.Random.Range(monsterEvents[0].minInside.Compute(type), monsterEvents[0].maxInside.Compute(type) + 1), 30.0f);
            Manager.Spawn.OutsideEnemies(bee, UnityEngine.Random.Range(monsterEvents[0].minOutside.Compute(type), monsterEvents[0].maxOutside.Compute(type) + 1));
        }
    }
}
