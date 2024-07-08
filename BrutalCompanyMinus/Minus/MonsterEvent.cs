using BepInEx.Configuration;
using LethalConfig.ConfigItems;
using System;
using System.Collections.Generic;
using System.Text;
using static BrutalCompanyMinus.Minus.MEvent;
using static BrutalCompanyMinus.Helper;
using static BrutalCompanyMinus.Configuration;
using MonoMod.ModInterop;

namespace BrutalCompanyMinus.Minus
{
    public class MonsterEvent
    {
        internal ConfigEntry<string> enemyConfigEntry;
        public EnemyType enemy;

        internal ConfigEntry<string> insideSpawnRarityConfigEntry, outsideSpawnRarityConfigEntry, minInsideConfigEntry, maxInsideConfigEntry, minOutisdeConfigEntry, maxOutsideConfigEntry;
        public Scale insideSpawnRarity, outsideSpawnRarity, minInside, maxInside, minOutside, maxOutside;

        public EventType eventType;

        public MonsterEvent(EnemyType enemy, Scale insideSpawnRarity, Scale outsideSpawnRarity, Scale minInside, Scale maxInside, Scale minOutside, Scale maxOutside)
        {
            this.enemy = enemy;
            assignRarities(insideSpawnRarity, outsideSpawnRarity, minInside, maxInside, minOutside, maxOutside);
        }

        public MonsterEvent(Assets.EnemyName enemyName, Scale insideSpawnRarity, Scale outsideSpawnRarity, Scale minInside, Scale maxInside, Scale minOutside, Scale maxOutside)
        {
            this.enemy = Assets.GetEnemy(enemyName);
            assignRarities(insideSpawnRarity, outsideSpawnRarity, minInside, maxInside, minOutside, maxOutside);
        }

        public MonsterEvent(string enemyName, Scale insideSpawnRarity, Scale outsideSpawnRarity, Scale minInside, Scale maxInside, Scale minOutside, Scale maxOutside)
        {
            this.enemy = Assets.GetEnemy(enemyName);
            assignRarities(insideSpawnRarity, outsideSpawnRarity, minInside, maxInside, minOutside, maxOutside);
        }

        private void assignRarities(Scale insideSpawnRarity, Scale outsideSpawnRarity, Scale minInside, Scale maxInside, Scale minOutside, Scale maxOutside)
        {
            this.insideSpawnRarity = insideSpawnRarity;
            this.outsideSpawnRarity = outsideSpawnRarity;
            this.minInside = minInside;
            this.maxInside = maxInside;
            this.minOutside = minOutside;
            this.maxOutside = maxOutside;
        }

        public void Execute()
        {
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.Enemies, enemy, insideSpawnRarity.Compute(eventType));
            Manager.AddEnemyToPoolWithRarity(ref RoundManager.Instance.currentLevel.OutsideEnemies, enemy, outsideSpawnRarity.Compute(eventType));
            Manager.Spawn.InsideEnemies(enemy, UnityEngine.Random.Range(minInside.Compute(eventType), maxInside.Compute(eventType) + 1));
            Manager.Spawn.OutsideEnemies(enemy, UnityEngine.Random.Range(minOutside.Compute(eventType), maxOutside.Compute(eventType) + 1));
        }

        internal void IniatlizeConfig(string eventName, ConfigFile to, ModInfo info)
        {
            insideSpawnRarityConfigEntry = to.Bind(eventName, $"{enemy.name} {ScaleType.InsideEnemyRarity}", GetStringFromScale(insideSpawnRarity), $"{ScaleInfoList[ScaleType.InsideEnemyRarity]}   {scaleDescription}");
            insideSpawnRarity = GetScale(insideSpawnRarityConfigEntry.Value);
            insideSpawnRarityConfigEntry.SettingChanged += (o, e) => insideSpawnRarity = GetScale(insideSpawnRarityConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(insideSpawnRarityConfigEntry, false), info);

            outsideSpawnRarityConfigEntry = to.Bind(eventName, $"{enemy.name} {ScaleType.OutsideEnemyRarity}", GetStringFromScale(outsideSpawnRarity), $"{ScaleInfoList[ScaleType.OutsideEnemyRarity]}   {scaleDescription}");
            outsideSpawnRarity = GetScale(outsideSpawnRarityConfigEntry.Value);
            outsideSpawnRarityConfigEntry.SettingChanged += (o, e) => outsideSpawnRarity = GetScale(outsideSpawnRarityConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(outsideSpawnRarityConfigEntry, false), info);

            minInsideConfigEntry = to.Bind(eventName, $"{enemy.name} {ScaleType.MinInsideEnemy}", GetStringFromScale(minInside), $"{ScaleInfoList[ScaleType.MinInsideEnemy]}   {scaleDescription}");
            minInside = GetScale(minInsideConfigEntry.Value);
            minInsideConfigEntry.SettingChanged += (o, e) => minInside = GetScale(minInsideConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(minInsideConfigEntry, false), info);

            maxInsideConfigEntry = to.Bind(eventName, $"{enemy.name} {ScaleType.MaxInsideEnemy}", GetStringFromScale(maxInside), $"{ScaleInfoList[ScaleType.MaxInsideEnemy]}   {scaleDescription}");
            maxInside = GetScale(maxInsideConfigEntry.Value);
            maxInsideConfigEntry.SettingChanged += (o, e) => maxInside = GetScale(maxInsideConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(maxInsideConfigEntry, false), info);

            minOutisdeConfigEntry = to.Bind(eventName, $"{enemy.name} {ScaleType.MinOutsideEnemy}", GetStringFromScale(minOutside), $"{ScaleInfoList[ScaleType.MinOutsideEnemy]}   {scaleDescription}");
            minOutside = GetScale(minOutisdeConfigEntry.Value);
            minOutisdeConfigEntry.SettingChanged += (o, e) => minOutside = GetScale(minOutisdeConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(minOutisdeConfigEntry, false), info);

            maxOutsideConfigEntry = to.Bind(eventName, $"{enemy.name} {ScaleType.MaxOutsideEnemy}", GetStringFromScale(maxOutside), $"{ScaleInfoList[ScaleType.MaxOutsideEnemy]}   {scaleDescription}");
            maxOutside = GetScale(maxOutsideConfigEntry.Value);
            maxOutsideConfigEntry.SettingChanged += (o, e) => maxOutside = GetScale(maxOutsideConfigEntry.Value);
            AddConfigForLethalConfig(new TextInputFieldConfigItem(maxOutsideConfigEntry, false), info);
        }

        internal void InitalizeConfigWithEnemyName(string eventName, ConfigFile to, ModInfo info)
        {
            enemyConfigEntry = to.Bind(eventName, "Enemy Name", enemy.name, "To get enemy names type 'menemies' into the terminal.");
            enemyConfigEntry.Value = Assets.GetEnemy(enemyConfigEntry.Value).name;
            enemyConfigEntry.SettingChanged += (o, e) => enemyConfigEntry.Value = Assets.GetEnemy(enemyConfigEntry.Value).name; // If cant find enemy then GetEnemy() will return a hoardingbug.
            AddConfigForLethalConfig(new TextInputFieldConfigItem(enemyConfigEntry, false), info);

            IniatlizeConfig(eventName, to, info);
        }
    }
}
