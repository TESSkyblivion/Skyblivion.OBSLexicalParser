using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory
{
    class ObjectiveHandlingFactory
    {
        private TES5ReferenceFactory referenceFactory;
        private TES5VariableAssignationFactory variableAssignationFactory;
        private TES5BranchFactory branchFactory;
        public ObjectiveHandlingFactory(TES5BranchFactory branchFactory, TES5VariableAssignationFactory variableAssignationFactory, TES5ReferenceFactory referenceFactory)
        {
            this.variableAssignationFactory = variableAssignationFactory;
            this.referenceFactory = referenceFactory;
            this.branchFactory = branchFactory;
        }

        /*
             *  The stage ID
         *  List of integers describing targets being enabled or disabled for given stage
        */
        public TES5FunctionCodeBlock createEnclosedFragment(TES5GlobalScope globalScope, int stageId, List<int> stageMap)
        {
            string fragmentName = "Fragment_"+stageId.ToString();
            TES5FunctionScope functionScope = TES5FragmentFunctionScopeFactory.createFromFragmentType(fragmentName, TES5FragmentType.T_QF);
            TES5CodeScope codeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(functionScope));
            TES5FunctionCodeBlock codeBlock = new TES5FunctionCodeBlock(functionScope, codeScope, new TES5VoidType());
            List<ITES5CodeChunk> chunks = this.generateObjectiveHandling(codeBlock, globalScope, stageMap);
            foreach (var chunk in chunks)
            {
                codeBlock.addChunk(chunk);
            }
            return codeBlock;
        }

        public List<ITES5CodeChunk> generateObjectiveHandling(ITES5CodeBlock codeBlock, TES5GlobalScope globalScope, List<int> stageMap)
        {
            List<ITES5CodeChunk> result = new List<ITES5CodeChunk>();
            TES5LocalVariable castedToQuest = new TES5LocalVariable("__temp", TES5BasicType.T_QUEST);
            TES5Reference referenceToTemp = TES5ReferenceFactory.CreateReferenceToVariable(castedToQuest);
            result.Add(variableAssignationFactory.createAssignation(referenceToTemp, TES5ReferenceFactory.CreateReferenceToSelf(globalScope)));
            TES5LocalScope localScope = codeBlock.getCodeScope().LocalScope;
            localScope.addVariable(castedToQuest);
            int i = 0;
            foreach (var stageTargetState in stageMap)
            {
                TES5Integer targetIndex = new TES5Integer(i);
                if (stageTargetState != 0)
                {
                    //Should be visible
                    TES5ObjectCallArguments displayedArguments = new TES5ObjectCallArguments();
                    displayedArguments.Add(targetIndex);
                    TES5ObjectCall isObjectiveDisplayed = new TES5ObjectCall(referenceToTemp, "IsObjectiveDisplayed", displayedArguments);
                    TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(isObjectiveDisplayed, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, new TES5Integer(0));
                    TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
                    arguments.Add(targetIndex);
                    arguments.Add(new TES5Integer(1));
                    TES5ObjectCall showTheObjective = new TES5ObjectCall(referenceToTemp, "SetObjectiveDisplayed", arguments);
                    TES5Branch branch = this.branchFactory.createSimpleBranch(expression, localScope);
                    branch.getMainBranch().getCodeScope().Add(showTheObjective);
                    result.Add(branch);
                }
                else
                {
                    TES5ObjectCallArguments displayedArguments = new TES5ObjectCallArguments();
                    displayedArguments.Add(targetIndex);
                    TES5ObjectCall isObjectiveDisplayed = new TES5ObjectCall(referenceToTemp, "IsObjectiveDisplayed", displayedArguments);
                    TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(isObjectiveDisplayed, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, new TES5Integer(1));
                    TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
                    arguments.Add(targetIndex);
                    arguments.Add(new TES5Integer(1));
                    TES5ObjectCall completeTheObjective = new TES5ObjectCall(referenceToTemp, "SetObjectiveCompleted", arguments);
                    TES5Branch branch = this.branchFactory.createSimpleBranch(expression, localScope);
                    branch.getMainBranch().getCodeScope().Add(completeTheObjective);
                    result.Add(branch);
                }

                ++i;
            }

            return result;
        }
    }
}