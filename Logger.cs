using System.Reflection;
using System.Text;

namespace WorkerService1
{

    public class Logger
    {       
        public static void WriteToFile(string Message)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        public static void Log(string Message, Exception exception, string classname, LoggerMsgType loggerMsgType = (LoggerMsgType)1)
        {
            try
            {
                if (Convert.ToInt32(1) == (int)LoggerMode.File)
                {
                    Logtofile(Message, exception, classname, loggerMsgType);
                }
                else
                {
                    string CurrentTimeDate = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

                    Hl7LogInfo hl7LogInfo = new Hl7LogInfo();
                    hl7LogInfo.CreatedBy = 99999997;
                    hl7LogInfo.CreatedOn = DateTime.Now;
                    hl7LogInfo.MsgType = (int)loggerMsgType;
                    hl7LogInfo.Message = Message.ToString().Replace("'", "").Trim();
                    hl7LogInfo.IpAddress = "";
                    hl7LogInfo.HostName = "";
                    hl7LogInfo.AddInfo = "";
                    if (exception != null)
                    {
                        hl7LogInfo.Source = exception.Source;
                        hl7LogInfo.StackTrace = exception.StackTrace + "-" + exception.InnerException;// exception.StackTrace;
                        hl7LogInfo.TargetSite = exception.TargetSite.ToString();
                    }
                    hl7LogInfo.AppName = Assembly.GetCallingAssembly().GetName().Name;


                    Int64 logid = 0;

                    try
                    {
                        WareedBL wareedBL = new WareedBL();
                        //SenderType 1:Referral ,2:Health Status
                        logid = wareedBL.InsertSenderHl7LogInfo(hl7LogInfo, 2);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                string messgae = ex.Message;
                Logtofile(messgae, ex, classname, loggerMsgType);
            }
        }

        private static void Logtofile(string Message, Exception ex, string classname, LoggerMsgType loggerMsgType = (LoggerMsgType)1)
        {
            try
            {
                string ErrorOrInfo = "Information";
                if (ex != null)
                {
                    ErrorOrInfo = "Error";
                }
                //IsInformatioLogRequired is required & logger Msg  is of type information  or
                //Error Message then log the information to DB or File
                if (((int)loggerMsgType == 1 && IsInformatioLogRequired == 1) || ((int)loggerMsgType == 2))
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
                    if (!File.Exists(filepath))
                    {
                        // Create a file to write to.   
                        using (StreamWriter sw = File.CreateText(filepath))
                        {
                            WriteLogToFile(Message, ex, ErrorOrInfo, sw);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = File.AppendText(filepath))
                        {
                            WriteLogToFile(Message, ex, ErrorOrInfo, sw);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private static void WriteLogToFile(string Message, Exception ex, string ErrorOrInfo, StreamWriter sw)
        {
            string logdate = DateTime.Now.ToString();
            //sw.WriteLine(Message);
            sw.WriteLine();
            sw.WriteLine($"============={ErrorOrInfo} Logging ===========");
            sw.WriteLine($"======================Start============= {logdate}");
            sw.WriteLine($"Message:{Message}");

            //messgae += exception.StackTrace + "-" + exception.InnerException;// exception.StackTrace;

            while (ex != null)
            {
                sw.WriteLine(ex.GetType().FullName);
                if (ex.Message != null)
                    sw.WriteLine($"Error Message: { ex.Message}");
                if (ex.Source != null)
                    sw.WriteLine($"Error Source: { ex.Source}");
                if (ex.TargetSite != null)
                    sw.WriteLine($"Error TargetSite: { ex.TargetSite.ToString()}");
                if (ex.StackTrace != null)
                    sw.WriteLine($"Stack Trace: {ex.StackTrace}");
                ex = ex.InnerException;
            }
            sw.WriteLine($"======================END============= {logdate}");

        }
    }
    public enum LoggerMode
    {
        File = 1,
        DB = 2,
    }

    
    public enum LoggerMsgType
    {
        Information = 1,
        Error = 2,
        Warning,
    }
   
    public class Hl7LogInfo
    {
        public long Id { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }

        public string StackTrace { get; set; }

        public string TargetSite { get; set; }

        public string AddInfo { get; set; }

        public string AppName { get; set; }

        public string HostName { get; set; }

        public string IpAddress { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? MsgType { get; set; }

        public int? DataFlag { get; set; }

    }
}
