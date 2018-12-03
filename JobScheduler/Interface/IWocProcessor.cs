using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobScheduler.Datatype;

namespace JobScheduler.Interface
{
    public interface IWocProcessor
    {
        int[,] PreferabilityMatrix { get; set; }

        void PopulatePreferabilityMatrix(List<List<Job>> opGen);

        List<Job> ReadPreferabilityMatrix();

        void InitializePreferabilityMatrix(int size);
    }
}
