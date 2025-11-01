using System;
using System.Linq;

namespace DialogueSystem
{
    public class DialogueController
    {
        private readonly IGameActions _actions;
        private readonly IGameConditions _conditions;
        private readonly IDialogueView _view;

        private Dialog _currentDialog;
        private DialogueBlock _currentBlock;

        private Action _onComplite;

        public DialogueController(IGameActions actions, IGameConditions conditions, 
                                  IDialogueView view)
        {
            _actions    = actions;
            _conditions = conditions;
            _view       = view;
        }

        public void StartDialog(Dialog dialog, Action onComplite)
        {
            _currentDialog  = dialog;
            _onComplite     = onComplite;

            SetBlock(dialog.First);
        }

        private void SetBlock(DialogueBlock block)
        {
            _currentBlock = block;

            _view.ShowText(_currentBlock.Text);
            _view.ShowChoices(_currentBlock.Choices
                .Where(choice => choice.Condition.Check(_conditions))
                .Select(choice => new ViewData(choice.Text, choice.Markers)));
        }

        private void SetBlock(int id) => SetBlock(_currentDialog.GetNext(id));

        public void MakeChoice(int index)
        {
            var choice = _currentBlock.Choices.ElementAt(index);
            int nextId = choice.NextId;

            choice.Action.Invoke(_actions);

            if (nextId == -1)
            {
                _onComplite?.Invoke();

                return;
            }

            SetBlock(nextId);
        }
    }
}