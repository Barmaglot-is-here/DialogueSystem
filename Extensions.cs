namespace DialogueSystem
{
    internal static class Extensions
    {
        public static bool Check(this DialogueCondition condition, IGameConditions conditions)
        {
            if (condition == null)
                return true;

            return conditions.Check(condition.Name, condition.Args);
        }

        public static void Invoke(this DialogueAction action, IGameActions actions)
        {
            if (action == null)
                return;

            actions.Invoke(action.Name, action.Args);
        }
    }
}