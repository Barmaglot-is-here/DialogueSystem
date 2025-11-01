namespace DialogueSystem
{
    public interface IGameConditions
    {
        bool Check(string key, string[] args);
    }
}