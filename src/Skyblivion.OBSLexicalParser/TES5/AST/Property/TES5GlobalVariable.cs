using System;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5GlobalVariable
    {
        public string Name { get; private set; }

        public TES5GlobalVariable(string name)
        {
            Name = name;
        }
    }
}