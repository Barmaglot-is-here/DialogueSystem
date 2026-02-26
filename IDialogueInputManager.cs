using System;

namespace DialogueSystem
{
    public interface IDialogueInputManager
    {
        event Action<int> MakeChoiceCallback;
    }
}
