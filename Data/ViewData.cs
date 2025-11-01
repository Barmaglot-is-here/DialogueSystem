using System.Collections.Generic;

namespace DialogueSystem
{
    public readonly struct ViewData
    {
        public readonly string Text;
        public readonly ICollection<string> Markers;

        public ViewData(string text, ICollection<string> markers)
        {
            Text    = text;
            Markers = markers;
        }
    }
}