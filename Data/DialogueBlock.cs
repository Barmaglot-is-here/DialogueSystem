using System.Collections.Generic;

namespace DialogueSystem
{
    internal class DialogueBlock
    {
        public string Text { get; }
        public IEnumerable<Choice> Choices { get; }
        public DialogueAction Action { get; }

        public DialogueBlock(string text, IEnumerable<Choice> choices, DialogueAction action)
        {
            Text    = text;
            Choices = choices;
            Action  = action;
        }
    }
}