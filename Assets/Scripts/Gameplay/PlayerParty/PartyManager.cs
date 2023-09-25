﻿using System;
using CryptoQuest.Battle.Components;
using CryptoQuest.Character.Attributes;
using CryptoQuest.Gameplay.Character;
using CryptoQuest.System;
using UnityEngine;

namespace CryptoQuest.Gameplay.PlayerParty
{
    public interface IPartyController
    {
        bool TryGetMemberAtIndex(int charIndexInParty, out IHero character);
        IParty Party { get; }
    }

    public class PartyManager : MonoBehaviour, IPartyController
    {
        [SerializeField] private AttributeSets _attributeSets; // Just for the asset to load

        [field: SerializeField, Header("Party Config")]
        private PartySO _party;

        public IParty Party => _party;

        [SerializeField, Space] private PartySlot[] _partySlots = new PartySlot[PartyConstants.MAX_PARTY_SIZE];

        private void OnValidate()
        {
            if (_partySlots.Length != PartyConstants.MAX_PARTY_SIZE)
            {
                Array.Resize(ref _partySlots, PartyConstants.MAX_PARTY_SIZE);
            }

            _partySlots = GetComponentsInChildren<PartySlot>();
        }

        private void Awake()
        {
            ServiceProvider.Provide<IPartyController>(this);
            InitParty();
        }

        /// <summary>
        /// Init party members stats at run time
        /// and bind the mono behaviour to the <see cref="CharacterSpec._characterComponent"/>
        /// </summary>
        private void InitParty()
        {
            for (int i = 0; i < _party.Members.Length; i++)
            {
                var character = _party.Members[i];
                if (!character.IsValid()) continue;
                _partySlots[i].Init(character);
            }
        }

        public bool TryGetMemberAtIndex(int charIndexInParty, out IHero character)
        {
            character = _partySlots[0].CharacterComponent; // first slot suppose to never be null

            if (charIndexInParty < 0 || charIndexInParty >= _partySlots.Length) return false;
            var partySlot = _partySlots[charIndexInParty];
            if (partySlot.IsValid() == false) return false;

            character = partySlot.CharacterComponent;

            return true;
        }
    }
}