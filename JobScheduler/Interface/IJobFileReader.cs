using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobScheduler.Datatype;

namespace JobScheduler.Interface
{
    public interface IJobFileReader
    {
        List<Job> Jobs { get; set; }

        void ReadFile(string fileDir);

    }
}
