using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobScheduler.Datatype;

namespace JobScheduler.Interface
{
    public interface IHybridAlgProcessor
    {
        IWocProcessor Woc { get; set; }

        IGenAlgProcessor GA { get; set; }

        IJobFileReader FileReader { get; set; }

        List<Job> OpSched { get; set; }

        int OpSchedTotalDelay { get; set; }

        void RunHybridAlg(string fileDir);
    }
}
