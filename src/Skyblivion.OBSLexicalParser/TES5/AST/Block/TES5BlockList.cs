using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5BlockList : ITES5Outputtable
    {
        private List<ITES5CodeBlock> blocks = new List<ITES5CodeBlock>();
        public List<ITES5CodeBlock> getBlocks()
        {
            return this.blocks;
        }

        public IEnumerable<string> Output => blocks.SelectMany(b => b.Output);

        public void add(ITES5CodeBlock block)
        {
            blocks.Add(block);
        }
    }
}