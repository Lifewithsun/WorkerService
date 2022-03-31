using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService1
{
    
    public class Configuration
    {
        public string MSSqlConnection { get; set; }
        public string FilePath { get; set; }
        public int LogProcessMode { get; set; }
        public int IsInformatioLogRequired { get; set; }
        public int IntervalSeconds { get; set; }
        public int T1DayName { get; set; }
        public int T1Day { get; set; }
        public int T1Hour { get; set; }
        public int T1Minutes { get; set; }

       
    }
}
