using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Bwl.Framework;

namespace Bwl.Framework.Test.CLI
{

    internal class BwlConsoleTestLoggerWriter : ILogWriter
    {
        private readonly int _logMessageCapacity;
        private readonly Queue<LogRecord> _logMessagesQueue;

        public bool LogEnabled { get; set; }

        /// <summary>
        /// Writer для получения записей из лога и передачи в WebUI
        /// </summary>
        /// <param name="logEnabled">Использовать лог?</param>
        /// <param name="capacity">Количество записей в логе</param>
        public BwlConsoleTestLoggerWriter(bool logEnabled, int capacity = 5000)
        {
            _logMessageCapacity = capacity;
            _logMessagesQueue = new Queue<LogRecord>(_logMessageCapacity);
            this.LogEnabled = logEnabled;
        }

        public void WriteEvent(DateTime datetime, string[] path, LogEventType type, string text, params object[] @params)
        {
            if (_logMessagesQueue.Count == _logMessageCapacity)
                _logMessagesQueue.Dequeue();
            _logMessagesQueue.Enqueue(new LogRecord(datetime, path, type, text, @params));
        }

        public LogRecord[] GetLogRecords(DateTime olderThan, LogEventType[] types, string filter, int maxRecordCount)
        {
            var records = _logMessagesQueue.Where(f => f.DateTime > olderThan && types.Contains(f.Type) && (string.IsNullOrWhiteSpace(filter) || f.Text.Contains(filter)));
            return maxRecordCount >= 0 ? records.OrderByDescending(f => f.DateTime).Take(maxRecordCount).OrderBy(f => f.DateTime).ToArray() : records.ToArray();
        }

        public void CategoryListChanged()
        {
        }

        public void ConnectedToLogger(Logger logger)
        {
        }
    }

    [DataContract()]
    public class LogRecord
    {
        [DataMember]
        public DateTime DateTime { get; set; }
        [DataMember]
        public string[] Path { get; set; }
        [DataMember]
        public LogEventType Type { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public object[] Params { get; set; }

        public LogRecord(DateTime datetime, string[] path, LogEventType type, string text, params object[] @params)
        {
            this.DateTime = datetime;
            this.Path = path;
            this.Type = type;
            this.Text = text;
            this.Params = @params;
        }
    }
}
