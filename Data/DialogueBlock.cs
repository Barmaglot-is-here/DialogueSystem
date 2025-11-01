using System.Collections.Generic;

namespace DialogueSystem
{
    internal class DialogueBlock
    {
        public string Text { get; }
        public IEnumerable<Choice> Choices { get; }

        public DialogueBlock(string text, IEnumerable<Choice> choices)
        {
            Text    = text;
            Choices = choices;
        }
    }
}