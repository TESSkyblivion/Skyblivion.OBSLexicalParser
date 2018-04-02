using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5EventBlockList
    {
        private List<TES5EventCodeBlock> blocks = new List<TES5EventCodeBlock>();
        public List<TES5EventCodeBlock> getBlocks()
        {
            return this.blocks;
        }

        public void add(TES5EventCodeBlock block)
        {
            this.blocks.Add(block);
        }
    }
}