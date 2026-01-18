using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resurrection.Core.Core.Domain.Entities
{
    public class LineColor
    {
        public LineColor(string text, ConsoleColor color = ConsoleColor.White, bool pulaLinha = false)
        {
            Text = text;
            Color = color;
            PulaLinha = pulaLinha;
        }

        public string Text { get; private set; }
        public ConsoleColor Color { get; private set; }
        public bool PulaLinha { get; private set; }
    }
}
