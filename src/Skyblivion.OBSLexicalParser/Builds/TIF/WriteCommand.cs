using Skyblivion.OBSLexicalParser.Commands;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class WriteCommand : WriteCommandBase, IWriteCommand
    {
        public void write(BuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            Write(buildTracker.getBuiltScripts(target.GetTargetName()).Values, progressWriter);
        }
    }
}