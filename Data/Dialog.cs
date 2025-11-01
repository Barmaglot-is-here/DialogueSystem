using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DialogueSystem
{
    public class Dialog
    {
        private readonly IReadOnlyDictionary<int, DialogueBlock> _blocks;

        internal DialogueBlock First => _blocks.Values.First();

        internal Dialog(IReadOnlyDictionary<int, DialogueBlock> blocks)
        {
            _blocks = blocks;    
        }

        public static Dialog Load(string path) => Load(XDocument.Load(path));
        public static Dialog Load(Stream stream) => Load(XDocument.Load(stream));
        public static Dialog Load(XDocument doc)
        {
            DialogueParser parser = new();

            return parser.Parse(doc);
        }

        internal DialogueBlock GetNext(int id) => _blocks[id];
    }
}