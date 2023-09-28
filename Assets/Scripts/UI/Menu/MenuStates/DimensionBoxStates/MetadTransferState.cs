using CryptoQuest.UI.Menu.Panels.DimensionBox;
using UnityEngine;

namespace CryptoQuest.UI.Menu.MenuStates.DimensionBoxStates
{
    public class MetadTransferState : DimensionBoxStateBase
    {
        public MetadTransferState(UIDimensionBoxMenu panel) : base(panel) { }

        public override void OnEnter()
        {
            base.OnEnter();
            NavigationBar.SetActive(false);
            NavigationBar.HighlightHeader(DimensionBoxPanel.TypeSO);
            DimensionBoxPanel.MetadTransferSection.EnterTransferSection();
            DimensionBoxPanel.MetadTransferSection.InitMetadTransferSection();
        }

        public override void HandleCancel()
        {
            base.HandleCancel();
            NavigationBar.SetActive(true);
            NavigationBar.HighlightHeader(DimensionBoxPanel.TypeSO, true);
            MenuStateMachine.RequestStateChange(DimensionBoxMenuStateMachine.TransferTypeSelection);
        }

        public override void OnExit()
        {
            base.OnExit();
            DimensionBoxPanel.MetadTransferSection.ExitTransferSection();
        }
        
        public override void Reset()
        {
            base.Reset();
            DimensionBoxPanel.MetadTransferSection.ResetTransfer();
        }

        public override void Execute()
        {
            base.Execute();
            DimensionBoxPanel.MetadTransferSection.SendItems();
        }
    }
}
