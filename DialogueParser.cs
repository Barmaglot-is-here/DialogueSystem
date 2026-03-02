using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DialogueSystem
{
    public class DialogueParser
    {
        public Dialog Parse(XDocument doc)
        {
            var blocks = Parse(doc.Root.Elements());

            return new(blocks);
        }

        private IReadOnlyDictionary<int, DialogueBlock> Parse(IEnumerable<XElement> elements)
        {
            Dictionary<int, DialogueBlock> blocks = new();

            foreach (var element in elements)
            {
                var block = ParseBlock(element, out int id);

                blocks.Add(id, block);
            }

            return blocks;
        }

        private DialogueBlock ParseBlock(XElement element, out int id)
        {
            var strId       = element.Attribute("id").Value;
            var text        = element.Element("text")?.Value;

            text = text == null ? "No data" : text;

            id          = int.Parse(strId);
            var choices = ParseChoices(element.Elements()
                                              .Where(element => element.Name == "choice" 
                                                             || element.Name == "union"));

            var action = ParseAction(element);

            return new(text, choices, action);
        }

        private IEnumerable<Choice> ParseChoices(IEnumerable<XElement> elements)
        {
            IEnumerable<Choice> choices = Array.Empty<Choice>();

            foreach (var element in elements)
            {
                if (element.Name == "choice")
                {
                    var choice = ParseChoice(element);

                    choices = choices.Append(choice);
                }
                else if (element.Name == "union")
                {
                    var union = ParseUnion(element);

                    choices = choices.Union(union);
                }
                else
                    throw new ArgumentException($"Wrong node type: {element.Name}");
            }

            return choices;
        }

        private Choice ParseChoice(XElement element)
        {
            string text         = element.Element("text").Value;
            string strNext      = element.Element("next")?.Value;
            int next            = strNext != null ? int.Parse(strNext) : -1;
            var markers         = ParseMarkers(element);

            ParseFunctions(element, out var action, out var condition);

            return new(text, next, markers, action, condition);
        }

        private ICollection<string> ParseMarkers(XElement root)
            => root.Elements("marker")
                   .Select(marker => marker.Value)
                   .ToHashSet();

        private IEnumerable<Choice> ParseUnion(XElement element)
        {
            ParseFunctions(element, out var action, out var condition);

            var choices = ParseChoices(element.Elements("choice"));

            foreach (var choice in choices)
            {
                choice.Action       = action;
                choice.Condition    = condition;
            }

            return choices;
        }

        private void ParseFunctions(XElement rootElement, out DialogueAction action, 
                                                          out DialogueCondition condition)
        {
            action      = ParseAction(rootElement);
            condition   = ParseCondition(rootElement);
        }

        private DialogueAction ParseAction(XElement rootElement) 
            => ParseFunctionSafe(rootElement, "action", (name, args) => new DialogueAction(name, args));

        private DialogueCondition ParseCondition(XElement rootElement) 
            => ParseFunctionSafe(rootElement, "condition", (name, args) => new DialogueCondition(name, args));

        private T ParseFunctionSafe<T>(XElement element, string elementKey, 
                                       Func<string, string[], T> createFunc) where T : DialogueFunction
        {
            string str = element.Element(elementKey)?.Value;

            if (str == null)
                return null;

            ParseFunction(str, out var name, out var args);

            return createFunc(name, args);
        }

        private void ParseFunction(string rawString, out string name, out string[] args)
        {
            int argsBegin = rawString.IndexOf('(');

            if (argsBegin == -1)
            {
                name = rawString;
                args = Array.Empty<string>();

                return;
            }

            int argsEnd     = rawString.IndexOf(')');
            string rawArgs  = rawString[argsBegin..argsEnd];

            name = rawString[..argsBegin];
            args = rawArgs.TrimStart('(').Split(',');
        }
    }
}