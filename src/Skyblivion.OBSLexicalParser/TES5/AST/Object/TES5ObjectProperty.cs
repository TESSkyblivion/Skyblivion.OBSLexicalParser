using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectProperty : TES5Castable, ITES5Referencer, ITES5ObjectAccess
    {
        public ITES5Referencer AccessedObject { get; private set; }
        private TES5Property property;
        public TES5ObjectProperty(ITES5Referencer objectReference, TES5Property property)
        {
            this.AccessedObject = objectReference;
            this.property = property;
        }

        public IEnumerable<string> Output
        {
            get
            {
                string referenceOutput = this.AccessedObject.Output.Single();
                return new string[] { referenceOutput + "." + this.property.PropertyNameWithSuffix+ ManualCastToOutput };
            }
        }

        public ITES5Type TES5Type => this.property.PropertyType;

        public ITES5Variable ReferencesTo => this.property;

        public string Name => this.property.PropertyNameWithSuffix;
    }
}
