using System;
using CryptoQuest.Gameplay.Character;
using CryptoQuest.Gameplay.PlayerParty;
using CryptoQuest.Item.Equipment;
using CryptoQuest.System;
using CryptoQuest.UI.Character;
using CryptoQuest.UI.Menu.MenuStates.StatusStates;
using CryptoQuest.UI.Menu.Panels.Status.Equipment;
using FSM;
using UnityEngine;

namespace CryptoQuest.UI.Menu.Panels.Status
{
    /// <summary>
    /// The context that hopefully holds all the UI information for the Status Menu. This is a mono behaviour class that
    /// can controls all the UI element or at least delegate back the reference to the correct state when needed.
    /// </summary>
    public class UIStatusMenu : UIMenuPanel
    {
        public event Action Show;
        public event Action Hide;

        [field: SerializeField, Header("State Context")]
        public UICharacterEquipmentsPanel CharacterEquipmentsPanel { get; private set; }

        [SerializeField] private UIEquipmentsInventory _equipmentsInventoryPanel;
        public UIEquipmentsInventory EquipmentsInventoryPanel => _equipmentsInventoryPanel;
        [field: SerializeField] public UIStatusCharacter CharacterPanel { get; private set; }
        [SerializeField] private AttributeChangeEvent _attributeChangeEvent;
        private CharacterSpec _inspectingCharacter;
        public CharacterSpec InspectingCharacter => _inspectingCharacter; // Should have using ICharacter
        private IParty _party;

        private void Awake()
        {
            CharacterPanel.InspectingCharacter += InspectCharacter;
            // This is event could not be fired because the scene contains this component is not loaded yet
            CharacterEquipmentsPanel.UnequipEquipmentAtSlot += UnequipEquipmentAtSlot;
        }

        private void Start()
        {
            _party = ServiceProvider.GetService<IPartyController>().Party;
        }

        private void OnDestroy()
        {
            CharacterPanel.InspectingCharacter -= InspectCharacter;
            CharacterEquipmentsPanel.UnequipEquipmentAtSlot -= UnequipEquipmentAtSlot;
        }

        protected override void OnShow()
        {
            base.OnShow();
            Show?.Invoke();
        }

        protected override void OnHide()
        {
            base.OnHide();
            Hide?.Invoke();
        }

        private void UnequipEquipmentAtSlot(EquipmentSlot.EType slot)
        {
            if (!_inspectingCharacter.IsValid())
            {
                Debug.Log($"UIStatusMenu::UnequipEquipmentAtSlot: No character is inspecting");
                return;
            }

            // TODO: REFACTOR PARTY
            // _provider.UnequipEquipmentAtSlot(_inspectingCharacter, slot);
        }

        public void EquipItem(EquipmentInfo equipment)
        {
            // TODO: REFACTOR PARTY
            // _provider.EquipEquipment(_inspectingCharacter, equipment);
        }

        private void InspectCharacter(CharacterSpec characterSpec)
        {
            if (characterSpec.IsValid() == false) return;
            _inspectingCharacter = characterSpec;

            var charAttributeSystem = characterSpec.CharacterComponent.AttributeSystem;
            _attributeChangeEvent.AttributeSystemReference = charAttributeSystem;
            CharacterEquipmentsPanel.SetEquipmentsUI(_inspectingCharacter.Equipments);
        }


        private StatusMenuStateMachine _state;
        public StatusMenuStateMachine State => _state;

        /// <summary>
        /// Return the specific state machine for this panel.
        /// </summary>
        /// <param name="menuManager"></param>
        /// <returns>The <see cref="StatusMenuStateMachine"/> which derived
        /// <see cref="CryptoQuest.UI.Menu.MenuStates.MenuStateMachine"/> derived
        /// from <see cref="StateMachine"/> which also derived from <see cref="StateBase"/></returns>
        public override StateBase<string> GetPanelState(MenuManager menuManager)
        {
            return _state ??= new StatusMenuStateMachine(this);
        }
    }
}