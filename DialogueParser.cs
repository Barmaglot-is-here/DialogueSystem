using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.Android.Gradle;

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
            var strId   = element.Attribute("id").Value;
            string text = element.Element("text").Value;

            id          = int.Parse(strId);
            var choices = ParseChoices(element.Elements()
                                              .Where(element => element.Name == "choice" 
                                                             || element.Name == "union"));

            return new(text, choices);
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
            string actionStr    = rootElement.Element("action")?.Value;
            string conditionStr = rootElement.Element("condition")?.Value;

            action      = ParseAction(actionStr);
            condition   = ParseCondition(conditionStr);
        }

        private DialogueAction ParseAction(string str)
        {
            if (str == null)
                return null;

            ParseFunction(str, out var name, out var args);

            return new(name, args);
        }

        private DialogueCondition ParseCondition(string str)
        {
            if (str == null)
                return null;

            ParseFunction(str, out var name, out var args);

            return new(name, args);
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