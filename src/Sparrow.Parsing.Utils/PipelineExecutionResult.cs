using Sparrow.Parsing.Utils.Enums;
using System;

namespace Sparrow.Parsing.Utils
{
    public class PipelineExecutionResult<TResult>
    {
        internal PipelineExecutionResult() { }

        public TResult Content { get; internal set; }
        public ExecutionStatus Status { get; internal set; }
        public Exception Exception { get; set; }
    }
}
