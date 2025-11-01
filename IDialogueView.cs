using System.Collections.Generic;

namespace DialogueSystem
{
    public interface IDialogueView
    {
        void ShowText(string text);
        void ShowChoices(IEnumerable<ViewData> choices);
    }
}