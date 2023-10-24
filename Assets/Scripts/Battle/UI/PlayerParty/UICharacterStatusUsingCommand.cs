using System.Collections;
using CryptoQuest.Battle.Events;
using CryptoQuest.Battle.Presenter.Commands;
using IndiGames.GameplayAbilitySystem.TagSystem.ScriptableObjects;

namespace CryptoQuest.Battle.UI.PlayerParty
{
    public class CharacterStatusIconCommand : IPresentCommand
    {
        private UICharacterStatusUsingCommand _uiCharacterStatus;
        private TagScriptableObject[] _tags;
        private bool _isShow;

        public CharacterStatusIconCommand(UICharacterStatusUsingCommand ui,
            TagScriptableObject[] tags, bool isShow = true)
        {
            _uiCharacterStatus = ui;
            _tags = tags;
            _isShow = isShow;
        }

        public IEnumerator Present()
        {
            if (_isShow)
            {
                _uiCharacterStatus.ShowTags(_tags);
                yield break;
            }
            _uiCharacterStatus.HideTags(_tags);
        }
    }

    public class UICharacterStatusUsingCommand : UICharacterStatus
    {
        public void ShowTags(TagScriptableObject[] tags)
        {
            base.TagAdded(tags);
        }

        public void HideTags(TagScriptableObject[] tags)
        {
            base.TagRemoved(tags);
        }

        protected override void TagAdded(params TagScriptableObject[] baseTags)
        {
            var command = new CharacterStatusIconCommand(this, baseTags);
            BattleEventBus.RaiseEvent<EnqueuePresentCommandEvent>(
                new EnqueuePresentCommandEvent(command));
        }

        protected override void TagRemoved(params TagScriptableObject[] baseTags)
        {
            var command = new CharacterStatusIconCommand(this, baseTags, true);
            BattleEventBus.RaiseEvent<EnqueuePresentCommandEvent>(
                new EnqueuePresentCommandEvent(command));
        }
    }
}