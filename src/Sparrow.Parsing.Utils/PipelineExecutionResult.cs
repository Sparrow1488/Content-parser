using Sparrow.Parsing.Utils.Enums;

namespace Sparrow.Parsing.Utils
{
    public class PipelineExecutionResult<TResult>
    {
        internal PipelineExecutionResult() { }

        public TResult Content { get; internal set; }
        public ExecutionStatus Status { get; internal set; }
    }
}
