using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using IndiGames.Core.SceneManagementSystem.ScriptableObjects;

namespace CryptoQuest.Gameplay.Battle.Core.ScriptableObjects.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Battle/Battle Data")]
    public class BattleDataSO : ScriptableObject
    {
        [field: SerializeField]
        public CharacterGroup[] EnemyGroups { get; private set; }

        [NonSerialized]
        private List<CharacterDataSO> _allEnemies = new();

        public List<CharacterDataSO> Enemies
        {
            get
            {
                InitAllEnemies();
                return _allEnemies;
            }
        }

        private void InitAllEnemies()
        {
            if (_allEnemies.Count > 0) return;
            foreach (var enemyGroup in EnemyGroups)
            {
                _allEnemies.AddRange(enemyGroup.Characters);
            }
        }

        private void OnValidate()
        {
            if (EnemyGroups == null) return;

            foreach (var group in EnemyGroups)
            {
                group.ValidateSameCharacterInGroup();
            }

            while (TotalEnemyCount() > 4)
            {
                RemoveLastGroup();
            }
        }

        private int TotalEnemyCount() => EnemyGroups.Sum(group => group.Characters.Length);

        private void RemoveLastGroup() => EnemyGroups = EnemyGroups.Take(EnemyGroups.Length - 1).ToArray();
    }

    [Serializable]
    public class CharacterGroup
    {
        [field: SerializeField]
        public CharacterDataSO[] Characters { get; private set; }

        public void ValidateSameCharacterInGroup()
        {
            if (Characters.Length <= 0) return;
            var firstValidChara = Characters.First<CharacterDataSO>(x => x != null);

            for (int i = 0; i < Characters.Length; i++)
            {
                var chara = Characters[i];
                if (chara == firstValidChara) continue;
                Characters[i] = firstValidChara;
            }
        }
    }
}