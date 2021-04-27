using System;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace Hp.Omen.OmenCommonLib.Utilities
{
    [EventSource]
    public class OMENEventSource : EventSource
    {
        private static Lazy<OMENEventSource> log = new Lazy<OMENEventSource>(() => new OMENEventSource());
        private const int NUMBER_OF_MAX_CHARS_TO_LOG_FOR_CONTENT = 1000;

        protected OMENEventSource()
        {
        }

        public static OMENEventSource Log => log.Value;

        [Event(1, Level = EventLevel.Informational, Message = "{0}")]
        public void Info(string message)
        {
            if (IsEnabled()) WriteEvent(1, TruncateContentIfNeeded(message));
        }

        [Event(2, Level = EventLevel.Warning, Message = "{0}")]
        public void Warn(string message)
        {
            if (IsEnabled()) WriteEvent(2, TruncateContentIfNeeded(message));
        }

        [Event(3, Level = EventLevel.Error, Message = "{0}")]
        public void Error(string message)
        {
            if (IsEnabled()) WriteEvent(3, TruncateContentIfNeeded(message));
        }

        [Event(4, Level = EventLevel.Informational, Message = "Command: {0} / Message: {1}")]
        public void CommandInfo(string commandName, string message)
        {
            if (IsEnabled()) WriteEvent(4, commandName, TruncateContentIfNeeded(message));
        }

        [Event(5, Level = EventLevel.Warning, Message = "Warning. Command: {0} / Message: {1}")]
        public void CommandWarn(string commandName, string message)
        {
            if (IsEnabled()) WriteEvent(5, commandName, TruncateContentIfNeeded(message));
        }

        [Event(6, Level = EventLevel.Error, Message = "Error. Command: {0} / Message: {1}")]
        public void CommandError(string commandName, string message)
        {
            if (IsEnabled()) WriteEvent(6, commandName, TruncateContentIfNeeded(message));
        }

        [Event(7, Level = EventLevel.Error, Message = "Error. Command: {0} / Message: {1}")]
        public void CommandErrorFunctionName(string message, [CallerMemberName] string memberName = "")
        {
            if (IsEnabled()) WriteEvent(7, memberName, TruncateContentIfNeeded(message));
        }

        [NonEvent]
        public void CommandExecutionStarts(string commandName)
        {
            CommandExecution(commandName, "start");
        }

        [NonEvent]
        public void CommandExecutionEnds(string commandName)
        {
            CommandExecution(commandName, "end");
        }

        [Event(8, Message = "{0}: {1}")]
        public void CommandExecution(string commandName, string startOrEnd)
        {
            if (IsEnabled()) WriteEvent(8, commandName, startOrEnd);
        }

        [Event(9, Level = EventLevel.Informational, Message = "{0}")]
        public void CommunicationWithExternalAgentInfo(string message)
        {
            if (IsEnabled()) WriteEvent(9, TruncateContentIfNeeded(message));
        }

        [Event(10, Level = EventLevel.Verbose)]
        public void Environment(string source, string key, string value)
        {
            if (IsEnabled()) WriteEvent(10, source, key, value);
        }

        private string TruncateContentIfNeeded(string content)
        {
            if (content != null && content.Length >= NUMBER_OF_MAX_CHARS_TO_LOG_FOR_CONTENT)
                return content.Substring(0, NUMBER_OF_MAX_CHARS_TO_LOG_FOR_CONTENT);

            return content;
        }
    }
}