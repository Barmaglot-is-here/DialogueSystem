using System.Collections.Generic;

namespace DialogueSystem
{
    internal class Choice
    {
        public string Text { get; }
        public int NextId { get; }

        public ICollection<string> Markers { get; }

        public DialogueAction Action { get; set; }
        public DialogueCondition Condition { get; set; }

        public Choice(string text, int nextId, ICollection<string> markers, 
                      DialogueAction action, DialogueCondition condition)
        {
            Text    = text;
            NextId  = nextId;
            Markers = markers;

            Action      = action;
            Condition   = condition;
        }
    }
}