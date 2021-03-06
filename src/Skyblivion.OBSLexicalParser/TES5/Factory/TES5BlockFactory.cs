using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5BlockFactory
    {
        private readonly TES5ChainedCodeChunkFactory codeChunkFactory;
        private readonly TES5AdditionalBlockChangesPass changesPass;
        private readonly TES5InitialBlockCodeFactory initialBlockCodeFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public TES5BlockFactory(TES5ChainedCodeChunkFactory chainedCodeChunkFactory, TES5AdditionalBlockChangesPass changesPass, TES5InitialBlockCodeFactory initialBlockCodeFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.codeChunkFactory = chainedCodeChunkFactory;
            this.changesPass = changesPass;
            this.initialBlockCodeFactory = initialBlockCodeFactory;
            this.objectCallFactory = objectCallFactory;
        }

        private static string MapBlockType(string blockType)
        {
            string newBlockType;
            switch (blockType.ToLower())
            {
                case "gamemode":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                case "onactivate":
                    {
                        newBlockType = "OnActivate";
                        break;
                    }

                case "oninit":
                    {
                        newBlockType = "OnInit";
                        break;
                    }

                case "onsell":
                    {
                        newBlockType = "OnSell";
                        break;
                    }

                case "ondeath":
                    {
                        newBlockType = "OnDeath";
                        break;
                    }

                case "onload":
                    {
                        newBlockType = "OnLoad";
                        break;
                    }

                case "onactorequip":
                    {
                        newBlockType = "OnObjectEquipped";
                        break;
                    }

                case "ontriggeractor":
                    {
                        newBlockType = "OnTriggerEnter";
                        break;
                    }

                case "onadd":
                    {
                        newBlockType = "OnContainerChanged";
                        break;
                    }

                case "onequip":
                    {
                        newBlockType = "OnEquipped";
                        break;
                    }

                case "onunequip":
                    {
                        newBlockType = "OnUnequipped";
                        break;
                    }

                case "ondrop":
                    {
                        newBlockType = "OnContainerChanged";
                        break;
                    }

                case "ontriggermob":
                    {
                        newBlockType = "OnTriggerEnter";
                        break;
                    }

                case "ontrigger":
                    {
                        newBlockType = "OnTrigger";
                        break;
                    }

                case "onhitwith":
                    {
                        newBlockType = "OnHit";
                        break;
                    }

                case "onhit":
                    {
                        newBlockType = "OnHit";
                        break;
                    }

                case "onalarm":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                case "onstartcombat":
                    {
                        newBlockType = "OnCombatStateChanged";
                        break;
                    }

                case "onpackagestart":
                    {
                        newBlockType = "OnPackageStart";
                        break;
                    }

                case "onpackagedone":
                    {
                        newBlockType = "OnPackageEnd";
                        break;
                    }

                case "onpackageend":
                    {
                        newBlockType = "OnPackageEnd";
                        break;
                    }

                case "onpackagechange":
                    {
                        newBlockType = "OnPackageChange";
                        break;
                    }

                case "onmagiceffecthit":
                    {
                        newBlockType = "OnMagicEffectApply";
                        break;
                    }

                case "onreset":
                    {
                        newBlockType = "OnReset";
                        break;
                    }

                case "scripteffectstart":
                    {
                        newBlockType = "OnEffectStart";
                        break;
                    }

                case "scripteffectupdate":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                case "scripteffectfinish":
                    {
                        newBlockType = "OnEffectFinish";
                        break;
                    }

                default:
                    {
                        throw new ConversionException("Cannot find new block type out of " + blockType);
                    }
            }

            return newBlockType;
        }

        public static TES5EventCodeBlock CreateEventCodeBlock(string blockType, TES5FunctionScope functionScope = null)
        {
            if (functionScope == null)
            {
                functionScope = new TES5FunctionScope(blockType);
            }
            TES5CodeScope codeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(functionScope);
            TES5EventCodeBlock newBlock = new TES5EventCodeBlock(functionScope, codeScope);
            return newBlock;
        }

        public static TES5State CreateState(string name, bool auto)
        {
            TES5FunctionScope functionScope = new TES5FunctionScope(name);
            TES5CodeScope codeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(functionScope);
            TES5State state = new TES5State(name, auto, functionScope, codeScope);
            return state;
        }

        public TES5BlockList CreateBlock(TES4CodeBlock block, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5BlockList blockList = new TES5BlockList();
            string blockType = block.BlockType;
            if (blockType.Equals("menumode", StringComparison.OrdinalIgnoreCase))
            {
                return blockList;
            }
            string newBlockType = MapBlockType(blockType);
            TES4CodeChunks chunks = block.Chunks;
            if (chunks != null && chunks.Any())
            {
                TES5EventCodeBlock newBlock;
                bool onUpdateOfNonQuest = newBlockType == "OnUpdate" && globalScope.ScriptHeader.BasicScriptType != TES5BasicType.T_QUEST;
                TES5BlockList onUpdateOfNonQuestAdditionalBlocks = null;
                if (onUpdateOfNonQuest)
                {
                    CreateActivationStates(globalScope, multipleScriptsScope, out onUpdateOfNonQuestAdditionalBlocks, out newBlock);
                }
                else
                {
                    TES5FunctionScope blockFunctionScope = TES5FunctionScopeFactory.CreateFromBlockType(newBlockType);
                    newBlock = CreateEventCodeBlock(newBlockType, blockFunctionScope);
                }
                this.ConvertTES4CodeChunksToTES5EventCodeBlock(chunks, newBlock, globalScope, multipleScriptsScope);
                this.changesPass.Modify(block, blockList, newBlock, globalScope, multipleScriptsScope);
                blockList.Add(newBlock);
                if (onUpdateOfNonQuest)
                {
                    return onUpdateOfNonQuestAdditionalBlocks;
                }
            }
            return blockList;
        }

        private void ConvertTES4CodeChunksToTES5EventCodeBlock(TES4CodeChunks chunks, TES5EventCodeBlock newBlock, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5CodeScope conversionScope = this.initialBlockCodeFactory.AddInitialCode(multipleScriptsScope, globalScope, newBlock);
            foreach (ITES4CodeChunk codeChunk in chunks)
            {
                TES5CodeChunkCollection codeChunks = this.codeChunkFactory.createCodeChunk(codeChunk, newBlock.CodeScope, globalScope, multipleScriptsScope);
                if (codeChunks != null)
                {
                    foreach (ITES5CodeChunk newCodeChunk in codeChunks)
                    {
                        conversionScope.AddChunk(newCodeChunk);
                    }
                }
            }
        }

        public static TES5EventCodeBlock CreateOnInit()
        {
            TES5FunctionScope onInitFunctionScope = TES5FunctionScopeFactory.CreateFromBlockType("OnInit");
            TES5EventCodeBlock newInitBlock = new TES5EventCodeBlock(onInitFunctionScope, TES5CodeScopeFactory.CreateCodeScopeRoot(onInitFunctionScope));
            return newInitBlock;
        }

        private void CreateActiveStateBlock(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, out TES5State state, out TES5EventCodeBlock onUpdate)
        {
            state = CreateState("ActiveState", false);
            TES5EventCodeBlock onBeginState = CreateEventCodeBlock("OnBeginState");
            onBeginState.CodeScope.CodeChunks.Add(this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "OnUpdate", multipleScriptsScope));
            state.AddBlock(onBeginState);
            onUpdate = CreateEventCodeBlock("OnUpdate");
            state.AddBlock(onUpdate);
        }

        private static TES5State CreateInactiveStateBlock()
        {
            TES5State state = CreateState("InactiveState", true);
            return state;
        }

        private void CreateActivationStates(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, out TES5BlockList blocks, out TES5EventCodeBlock onUpdate)
        {
            TES5State activeState;
            CreateActiveStateBlock(globalScope, multipleScriptsScope, out activeState, out onUpdate);
            TES5State inactiveState=CreateInactiveStateBlock();
            TES5EventCodeBlock onCellAttach = CreateEventCodeBlock("OnCellAttach");
            onCellAttach.CodeScope.AddChunk(objectCallFactory.CreateGotoState("ActiveState", globalScope, multipleScriptsScope));
            TES5EventCodeBlock onCellDetach = CreateEventCodeBlock("OnCellDetach");
            onCellDetach.CodeScope.AddChunk(objectCallFactory.CreateGotoState("InactiveState", globalScope, multipleScriptsScope));
            blocks = new TES5BlockList() { activeState, inactiveState, onCellAttach, onCellDetach };
        }
    }
}