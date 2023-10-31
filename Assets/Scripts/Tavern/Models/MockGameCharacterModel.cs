﻿using System.Collections;
using System.Collections.Generic;
using CryptoQuest.Tavern.Data;
using CryptoQuest.Tavern.Interfaces;
using UnityEngine;
using UnityEngine.Localization;
using Random = System.Random;

namespace CryptoQuest.Tavern.Models
{
    public class MockGameCharacterModel : MonoBehaviour, IGameCharacterModel
    {
        [SerializeField] private int _dataLength;
        [SerializeField] private Sprite _classIcon;
        [SerializeField] private LocalizedString[] _localizedNames = new LocalizedString[4];

        private List<IGameCharacterData> _gameData;
        public List<IGameCharacterData> Data => _gameData;

        public IEnumerator CoGetData()
        {
            yield return new WaitForSeconds(1f);
            _gameData = new List<IGameCharacterData>();
            InitMockData();
        }

        private void InitMockData()
        {
            for (var i = 0; i < _dataLength; i++)
            {
                Random rand = new Random();
                LocalizedString name = _localizedNames[rand.Next(_localizedNames.Length - 1)];
                int level = rand.Next(1, 99);

                var obj = new GameCharacterData(_classIcon, name, level);

                _gameData.Add(obj);
            }
        }
    }
}