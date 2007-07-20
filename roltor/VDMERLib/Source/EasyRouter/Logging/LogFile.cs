using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;

namespace VDMERLib.EasyRouter.General
{
    public class LogFile
    {
        // Defines the Schema when backing up files.
        // The enum is in two parts;
        // - Backup initiation style
        // - File naming Schema
        public enum EFileBackupSchema
        {
            // The file just grows.
            TriggerNone = 0,

            // Initiate backup based on file size, or date/time comparison.
            TriggerFileSize = 0x01,
            TriggerDateTime = 0x02,

            // If "delete existing" is enabled, deletes existing
            // backup file "filename-BAK.log".  Otherwise, creates new backup
            // files "filename-BAK-001.log", "filename-BAK-002.log" etc.
            DeleteExisting = 0x04,

            // If not specified, the backup is not created.
            RenameAppendBackup = 0x08, // Create new file with "-BAK"

            // These two are a little odd, in that if specified the initial
            // filename will have the hour/minute appended as well.
            RenameAppendDate = 0x10, // Create new file with "-YYYYMMDD"
            RenameAppendTime = 0x20, // Create new file with "-HHMMSS"
        };

        public enum ELogFileType
        {
            Log = 0,
            Audit = 1,
            Wire = 2
        }

        public const EFileBackupSchema DefaultFileBackupSchemaSize = EFileBackupSchema.TriggerFileSize | EFileBackupSchema.DeleteExisting | EFileBackupSchema.RenameAppendBackup;
        public const EFileBackupSchema DefaultFileBackupSchemaDate = EFileBackupSchema.TriggerDateTime | EFileBackupSchema.RenameAppendDate;
        public const EFileBackupSchema DefaultFileBackupSchemaDateTime = EFileBackupSchema.TriggerDateTime | EFileBackupSchema.RenameAppendDate | EFileBackupSchema.RenameAppendTime;
        
        
        StreamWriter m_File;
        string m_strLogFileName;
        string m_strFilePath;
        string m_strFileBackup;
        int m_iLogRules = 0;
        int m_nAppendUnique = 0;
        bool m_bIsBackingUp = false;
        bool m_bIsSameFile = false;
        int m_iMaxFileSize = 0;
        int m_iFlushPeriod = 0;
        DateTime m_NextRenameTime;
        DateTime m_LastFlushTime;
        Thread m_FlushThread;
        ELogFileType m_eFileType;
        ~LogFile()
        {
            Close();
            m_FlushThread.Abort();
        }
        
        public bool Close()
        {
            if (m_File != null && m_File.BaseStream != null)
            {
                lock (m_File)
                {
                    m_File.Flush(); 
                    m_File.Close();
                    m_File = null;
                }
                return true;
            }
            else
                return false;
        }

        public bool Open(string strLogFileName, ELogFileType eFileType)
        {
            m_strLogFileName = strLogFileName;
            // Default log directory is %TEMP%
            string strTempDir = Path.GetTempPath();
            // Try to access the root registry key and, if possible, the key for this executable
            string strRootKeyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\EasyScreen\\Debug\\";
            m_eFileType = eFileType;
            string strFileTypeLookup;
            switch (eFileType)
            {
                case ELogFileType.Audit: strFileTypeLookup = "Audit"; break;
                case ELogFileType.Wire: strFileTypeLookup = "Wire"; break;
                default: strFileTypeLookup = "Log"; break;
            }

            string strLogDirectory = Registry.GetValue(strRootKeyName, "LogDirectory", strTempDir).ToString();
            string strLogExtension = Registry.GetValue(strRootKeyName, strFileTypeLookup + "Extension", "log").ToString();
            m_iMaxFileSize = (int)Registry.GetValue(strRootKeyName, strFileTypeLookup + "Filesize", 1048576);
            m_iFlushPeriod = (int)Registry.GetValue(strRootKeyName, strFileTypeLookup + "FlushPeriod", 1000);
            m_iLogRules = (int)Registry.GetValue(strRootKeyName, strFileTypeLookup + "Rules", LogFile.DefaultFileBackupSchemaSize);
            m_LastFlushTime = DateTime.Now;

            m_FlushThread = new Thread(delegate()
            {
                while (true)
                {
                    if (m_File != null && m_LastFlushTime.AddMilliseconds(m_iFlushPeriod) < DateTime.Now)
                    {
                        lock (m_File)
                        {
                            m_File.Flush();
                            m_LastFlushTime = DateTime.Now;
                        }
                    }
                    Thread.Sleep(m_iFlushPeriod);
                }
            });

            m_FlushThread.Name = "Log Flusher";
            m_FlushThread.Start();

            // Add a trailing backslash to the directory name if there isn't already one in there
            if ((strLogDirectory.Length > 0) &&
                (strLogDirectory[strLogDirectory.Length - 1] != '\\') &&
                (strLogDirectory[strLogDirectory.Length - 1] != '/'))
                strLogDirectory += '\\';
            m_strFilePath = strLogDirectory + strLogFileName + "." + strLogExtension;
            return ReOpen();
        }
        
        public bool ReOpen()
        {
            GenerateFileNames();
            GenerateBackupTime();
            // Open the text file.
            try
            {
                // Always close and re-open, incase the time has changed
                Close();

                m_File = new StreamWriter(m_strFilePath);
                lock (m_File)
                {
                    //m_File = (StreamWriter)TextWriter.Synchronized((TextWriter)m_File);
                    FileInfo objFileInfo = new FileInfo(m_strFilePath);
                    if (!m_bIsSameFile && objFileInfo.Length>0)
                        m_File.WriteLine("-------------------------------");
                    m_File.Flush();
                }
                return true;
            }
            catch (Exception ex)
            {
                //where do we write this?
                return false;
            }
        }

        public bool Write(string strMessage, TraceEventCache eventCache, params Object[] parameters)
        {
            StackTrace st = new StackTrace(true);
            string strLocation = "Stack Frame Error";
            if (st != null)
            {
                StackFrame sf = st.GetFrame(5);
                if (sf!=null)
                    strLocation = String.Format("{0}({1})", sf.GetFileName(), sf.GetFileLineNumber());
            }

            string strEventData = "Event Cache Error";
            if (eventCache != null)
            {
                strEventData = String.Format("{0}.{1} : {2}[{3}]",eventCache.DateTime.ToString(),eventCache.DateTime.Millisecond, eventCache.ProcessId, eventCache.ThreadId);
            }

            if (parameters!=null)
                strMessage = string.Format(strMessage, parameters);

            string strFormattedMessage;
            if (m_eFileType == ELogFileType.Log)
                strFormattedMessage = String.Format("{0} : {1} : {2}", strEventData, strLocation, strMessage);
            else
                strFormattedMessage = strMessage;

            try
            {
                lock (m_File)
                {
                    m_File.WriteLine(strFormattedMessage);
                }
            }
            catch (Exception ex)
            {
                //where do we write this?
                return false;
            }
            // If the log file has gone over the maximum file size, make a backup.
            BackupLogFile();
            return true;
        }

        bool BackupLogFile()
        {
           // Check we're not calling this function recursively, and the log
           // file is actually open.
           if (!m_bIsBackingUp && (m_File != null))
           {
              // [MT 03/10/01] Added time based backups.
              bool bPerformBackup = false;
               
              // Check to see if size based backup is enabled.
              if ((m_iLogRules & (int)EFileBackupSchema.TriggerFileSize) == (int)EFileBackupSchema.TriggerFileSize)
              {
                 // Check if the current log file size is over the limit.  If the
                 // limit is zero, let it grow indefinitely.
                  FileInfo objFileInfo = new FileInfo(m_strFilePath);
                  if (objFileInfo.Length > m_iMaxFileSize)
                 {
                    bPerformBackup = true;
                 }
              }

              // Check to see if time based backup is enabled.
              if ((m_iLogRules & (int)EFileBackupSchema.TriggerDateTime) == (int)EFileBackupSchema.TriggerDateTime)
              {
                 if (DateTime.Now > m_NextRenameTime)
                 {
                    bPerformBackup = true;
                 }
              }

              // If we have hit one of the backup conditions.
              if (bPerformBackup == true)
              {
                 // Set this flag, so we don't get called recursively.
                 m_bIsBackingUp = true;

                 // Close the existing file.
                 Close();
                 
                 // Delete the backup file if it exists.
                 if ((m_iLogRules & (int)EFileBackupSchema.DeleteExisting) == (int)EFileBackupSchema.DeleteExisting)
                 {
                    // Rename the live file as the backup file;
                    // delete if it already exists.
                    File.Delete(m_strFileBackup);
                    File.Move(m_strFilePath, m_strFileBackup);
                 }
                 else
                 {
                    FileInfo newBackupFile = new FileInfo(m_strFileBackup);
                    string strBackUpName = newBackupFile.Name; 
                    while (newBackupFile.Exists)
                    {
                       // Append "-001", "-002" etc to the filename component.
                        strBackUpName = String.Format("{0}-{1:000}", m_strFileBackup, m_nAppendUnique);
                    }
                    try
                    {
                        m_strFileBackup = Path.GetDirectoryName(m_strFileBackup) + strBackUpName + Path.GetExtension(m_strFileBackup);
                        // Rename the live file as the backup file;
                        File.Move(m_strFilePath, m_strFileBackup);
                    }
                    catch (Exception ex)
                    {
                    }

                 }
                 
                 // Recreate the log file, which generates new filenames if required.
                 ReOpen();

                 // Clear this flag, so we can get called again.
                 m_bIsBackingUp = false;
              }
           }

           // Success.
           return true;
        }

        void GenerateFileNames()
        {
            string strNewFileName = m_strLogFileName;
            // Apply the rules
           if ((m_iLogRules & (int)EFileBackupSchema.RenameAppendDate) == (int)EFileBackupSchema.RenameAppendDate)
               strNewFileName += String.Format("-{0:0000}{1:00}{2:00}", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

           if ((m_iLogRules & (int)EFileBackupSchema.RenameAppendTime) == (int)EFileBackupSchema.RenameAppendTime)
           {
              int iMinutes = DateTime.Today.Minute;
              iMinutes -= iMinutes % 5;
              strNewFileName += String.Format("-{0}:00{1}:00", DateTime.Today.Hour, iMinutes);
           }

           string strNewFilePath = Path.GetDirectoryName(m_strFilePath) + "\\" + strNewFileName + Path.GetExtension(m_strFilePath);
           
            // Need to work out whether we're reopening the same file
           // in this session; if so, we must not print a blank line.
           m_bIsSameFile = (strNewFilePath == m_strFilePath);

           // Create the filename.
           m_strFilePath = strNewFilePath;

           // This isn't quite as crazy as it looks.  If we are using time based
           // filenames, then you get a unique identifier without the -BAK... but
           // it'll all blow up if you don't specify backup and don't specify times :)
           if ((m_iLogRules & (int)EFileBackupSchema.RenameAppendBackup) == (int)EFileBackupSchema.RenameAppendBackup)
           {
               m_strFileBackup = Path.GetDirectoryName(m_strFilePath) + strNewFileName + "-BAK." + Path.GetExtension(m_strFilePath);
           }
           else
           {
               m_strFileBackup = m_strFilePath;
           }

           // If the live and backup filenames have changed, reset
           // the unique integer for appending.
           if (!m_bIsSameFile)
           {
              m_nAppendUnique = 0;
           }
        }

        void GenerateBackupTime()
        {
            // Nothing to do here...
            if ((m_iLogRules & (int)EFileBackupSchema.TriggerDateTime) != (int)EFileBackupSchema.TriggerDateTime)
            {
                return;
            }

            int nNowSeconds = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;

            // If we don't have any time objects, calculate until midnight tonight
            int nSecondsUntilNextRename = 86400 - nNowSeconds;
            // Better just check...
            if (nSecondsUntilNextRename == 0)
                nSecondsUntilNextRename = 86400;

            // Set the next rename time
            m_NextRenameTime = DateTime.Now.AddSeconds(nSecondsUntilNextRename);
        }           
    }
}
