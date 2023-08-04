﻿using UnityEngine;
using UnityEngine.Localization;

namespace CryptoQuest.Data.Item
{
    public class RaritySO : ScriptableObject
    {
        public int ID;

        [field: SerializeField] public LocalizedString DisplayName { get; private set; }
        public LocalizedString Description;
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}