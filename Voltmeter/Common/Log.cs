using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using System.Windows.Forms;
using Voltmeter.Resources;

namespace Voltmeter.Common
{
    public static class Log
    {
        public static readonly string Source = Application.ProductName;
        public static void Info(string message)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(message));
            WriteLogEntry(message, EventLogEntryType.Information);
        }

        public static void InfoFormat(string message, params object[] args)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(message));
            Contract.Requires<ArgumentException>(args != null);
            WriteLogEntry(string.Format(message, args), EventLogEntryType.Information);
        }

        public static void Error(string message)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(message));
            WriteLogEntry(message, EventLogEntryType.Error);
        }

        public static void ErrorFormat(string message, params object[] args)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(message));
            Contract.Requires<ArgumentException>(args != null);
            WriteLogEntry(string.Format(message, args), EventLogEntryType.Error);
        }

        public static void ExceptionError(Exception exception, string message = null)
        {
            Contract.Requires<ArgumentNullException>(exception != null);
            WriteExceptionEntry(exception, message, EventLogEntryType.Error);
        }

        public static void ExceptionErrorFormat(Exception exception, string message, params object[] args)
        {
            Contract.Requires<ArgumentNullException>(exception != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(message));
            Contract.Requires<ArgumentException>(args != null);

            ExceptionError(exception, string.Format(message, args));
        }

        public static void ExceptionWarning(Exception exception, string message = null)
        {
            Contract.Requires<ArgumentNullException>(exception != null);
            WriteExceptionEntry(exception, message, EventLogEntryType.Warning);
        }

        public static void ExceptionWarningFormat(Exception exception, string message, params object[] args)
        {
            Contract.Requires<ArgumentNullException>(exception != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(message));
            Contract.Requires<ArgumentException>(args != null);

            WriteExceptionEntry(exception, string.Format(message, args), EventLogEntryType.Warning);
        }
        
        public static void Warning(string message)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(message));
            WriteLogEntry(message, EventLogEntryType.Warning);
        }

        public static void WarningFormat(string message, params object[] args)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(message));
            Contract.Requires<ArgumentException>(args != null);

            WriteLogEntry(string.Format(message, args), EventLogEntryType.Warning);
        }

        static void WriteExceptionEntry(Exception exception, string message, EventLogEntryType entryType)
        {
            WriteLogEntry(
                string.IsNullOrWhiteSpace(message)
                    ? DumpException(exception)
                    : string.Concat(message, Environment.NewLine, DumpException(exception)), entryType);
        }

        static void WriteLogEntry(string message, EventLogEntryType entryType, int eventId = 0)
        {
            EnsureSourceExists();
            var elog = new EventLog
            {
                Source = Source,
                EnableRaisingEvents = true
            };
            elog.WriteEntry(message, entryType, eventId);
            Debug.WriteLine(message);
        }

        static void EnsureSourceExists()
        {
            if (!EventLog.SourceExists(Source))
            {
                EventLog.CreateEventSource(Source, Source);
            }
        }

        static string DumpException(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(Strings.Log_Exception_0, exception.GetType().Name);
            sb.AppendLine();
            sb.AppendFormat(Strings.Log_Message_0, exception.Message);
            sb.AppendLine();
            sb.AppendLine(Strings.Log_StackTrace);
            sb.AppendLine(exception.StackTrace);
            if (exception.InnerException != null)
            {
                sb.AppendLine(Strings.Log_NestedException);
                sb.AppendLine(DumpException(exception.InnerException));
            }
            return sb.ToString();
        }
    }
}
