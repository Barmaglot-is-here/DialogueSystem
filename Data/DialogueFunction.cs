namespace DialogueSystem
{
    internal class DialogueFunction
    {
        public string Name { get; }
        public string[] Args { get; }

        public DialogueFunction(string name, string[] args)
        {
            Name = name;
            Args = args;
        }
    }
}