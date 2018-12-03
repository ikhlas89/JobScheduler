using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Datatype
{
    public struct Job
    {
        public string jobID;
        public int execTime;
        public int dueTime;
    }
}
