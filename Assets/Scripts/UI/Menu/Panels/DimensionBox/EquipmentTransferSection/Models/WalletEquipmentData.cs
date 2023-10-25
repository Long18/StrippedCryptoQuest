using CryptoQuest.Item.Equipment;
using CryptoQuest.UI.Menu.Panels.DimensionBox.Interfaces;
using UnityEngine;
using UnityEngine.Localization;

namespace CryptoQuest.UI.Menu.Panels.DimensionBox.EquipmentTransferSection.Models
{
    public class WalletEquipmentData : INFT
    {
        private uint _id;
        private Sprite _icon;
        private LocalizedString _name;

        public WalletEquipmentData(EquipmentInfo equipmentInfo)
        {
            _id = equipmentInfo.Id;
            _icon = equipmentInfo.Data.EquipmentType.Icon;
            _name = equipmentInfo.Data.DisplayName;
        }

        public uint GetId() { return _id; }
        public Sprite GetIcon() { return _icon; }
        public LocalizedString GetLocalizedName() { return _name; }
    }
}
