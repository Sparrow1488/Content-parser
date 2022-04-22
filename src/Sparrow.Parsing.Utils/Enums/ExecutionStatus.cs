namespace Sparrow.Parsing.Utils.Enums
{
    public class ExecutionStatus
    {
        private ExecutionStatus(string status)
        {
            _status = status;
        }

        private readonly string _status = string.Empty;

        public static readonly ExecutionStatus Ok = new ExecutionStatus(nameof(Ok));
        public static readonly ExecutionStatus HandleError = new ExecutionStatus(nameof(HandleError));
        public static readonly ExecutionStatus NotHandleError = new ExecutionStatus(nameof(NotHandleError));

        public override string ToString() => _status;

        public static bool operator ==(ExecutionStatus a, ExecutionStatus b) =>
            a.ToString() == b.ToString();

        public static bool operator !=(ExecutionStatus a, ExecutionStatus b) =>
            a.ToString() != b.ToString();
    }
}
