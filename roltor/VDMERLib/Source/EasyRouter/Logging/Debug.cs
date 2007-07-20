using System;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.IO;


namespace VDMERLib.EasyRouter.General
{
    public abstract class TraceListener2 : TraceListener{
    protected TraceListener2(string name): base(name) {}

    protected abstract void TraceEventCore(
        TraceEventCache eventCache, string source,
        TraceEventType eventType, int id, string message, params Object[] parameters);

    protected virtual string FormatData(object[] data) {
        StringBuilder strData = new StringBuilder();
        for (int i = 0; i < data.Length; i++) {
            if (i >= 1) strData.Append("|");
            strData.Append(data[i].ToString());
        }
        return strData.ToString();
    }

    protected void TraceDataCore(TraceEventCache eventCache, 
            string source, TraceEventType eventType, 
            int id, params object[] data) {
        if (Filter != null && 
            !Filter.ShouldTrace(eventCache, source, eventType, 
                id, null, null, null, data)) return;
        TraceEventCore(eventCache, source, eventType, 
            id, FormatData(data));
    }

    public sealed override void TraceEvent(TraceEventCache eventCache, 
            string source, TraceEventType eventType, 
            int id, string message) {
        if (Filter != null && 
            !Filter.ShouldTrace(eventCache, source, eventType, id, 
            message,null,null,null)) return;
        TraceEventCore(eventCache, source, eventType, id, message);
    }

    public sealed override void TraceEvent(TraceEventCache eventCache,
            string source, TraceEventType eventType,
            int id, string message, params Object[] parameters)
    {
        if (Filter != null &&
            !Filter.ShouldTrace(eventCache, source, eventType, id,
            message, null, null, null)) return;
        TraceEventCore(eventCache, source, eventType, id, message, parameters);
    }

    public sealed override void Write(string message) {
        if (Filter != null && 
            !Filter.ShouldTrace(null, "Trace", TraceEventType.Information, 
            0, message, null, null, null)) return;
        TraceEventCore(null, "Trace", TraceEventType.Information, 
                       0, message);
    }

    public sealed override void WriteLine(string message)
    {
        if (Filter != null &&
            !Filter.ShouldTrace(null, "Trace", TraceEventType.Information,
            0, message, null, null, null)) return;
        TraceEventCore(null, "Trace", TraceEventType.Information,
                       0, message);
    }
    
    public sealed override int GetHashCode() {
        return base.GetHashCode();
    }

    public sealed override string Name {
        get { return base.Name; }
        set { base.Name = value; }
    }

    }

    

    
    public class LogFileWriter : TraceListener2 
    {
        static LogFileWriter g_LogFile = null;
        static public void Initialise(string sfileName)
        {
            if (g_LogFile == null)
            {
                g_LogFile = new LogFileWriter(sfileName);
            }
        }
        public static void Initialise()
        {
            Initialise(AppDomain.CurrentDomain.FriendlyName);
        }

        static public LogFileWriter Logger
        {
            get 
            {
                Initialise();
                return g_LogFile;
            }
        }

        static public void TraceEvent(TraceEventType EventType, String strMessage)
        {
            Initialise();
            myTrace.TraceEvent(EventType,0,strMessage);
        }

        static public void TraceEvent(TraceEventType EventType, String strMessage, params Object[] parameters)
        {
            Initialise();
            myTrace.TraceEvent(EventType, 0, strMessage, parameters);
        }
        
        static TraceSource myTrace;
        LogFile m_LogFile;
        
        public LogFileWriter(string strLogFileName)
            : base(strLogFileName)
        {
            myTrace = new TraceSource(strLogFileName);
            myTrace.Listeners.Add(this);
            myTrace.Switch.Level = SourceLevels.All;
            m_LogFile = new LogFile();
            
            m_LogFile.Open(strLogFileName, LogFile.ELogFileType.Log);
        }

        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message, params Object[] parameters)
        {
            m_LogFile.Write(message, eventCache, parameters);
        }
    }
    public class AuditFileWriter : TraceListener2
    {
        static AuditFileWriter g_AuditFile = null;
        static public void Initialise(string sfileName)
        {
            if (g_AuditFile == null)
            {
                g_AuditFile = new AuditFileWriter(sfileName);
            }
        }
        public static void Initialise()
        {
            Initialise(AppDomain.CurrentDomain.FriendlyName);
        }

        static public AuditFileWriter Logger
        {
            get
            {
                Initialise();
                return g_AuditFile;
            }
        }

        static public void Log(String strMessage)
        {
            Initialise();
            myTrace.TraceEvent(TraceEventType.Information, 0, strMessage);
        }

        static public void TraceEvent(String strMessage, params Object[] parameters)
        {
            Initialise();
            myTrace.TraceEvent(TraceEventType.Information, 0, strMessage, parameters);
        }

        static TraceSource myTrace;
        LogFile m_LogFile;

        public AuditFileWriter(string strLogFileName)
            : base(strLogFileName)
        {
            myTrace = new TraceSource(strLogFileName);
            myTrace.Listeners.Add(this);
            myTrace.Switch.Level = SourceLevels.All;
            m_LogFile = new LogFile();

            m_LogFile.Open(strLogFileName, LogFile.ELogFileType.Audit);
        }

        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message, params Object[] parameters)
        {
            m_LogFile.Write(message, eventCache, parameters);
        }
    }
}
