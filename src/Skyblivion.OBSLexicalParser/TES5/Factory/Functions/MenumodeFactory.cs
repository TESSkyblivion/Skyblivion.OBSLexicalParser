using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class MenumodeFactory : IFunctionFactory
    {
        public ITES5CodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return new TES5Integer(0);
        }
    }
}