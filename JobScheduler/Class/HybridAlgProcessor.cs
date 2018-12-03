using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobScheduler.Datatype;
using JobScheduler.Interface;

namespace JobScheduler.Class
{
    public class HybridAlgProcessor : IHybridAlgProcessor
    {

        #region Properties

        public IWocProcessor Woc { get; set; }

        public IGenAlgProcessor GA { get; set; }

        public IJobFileReader FileReader { get; set; }

        public List<Job> OpSched { get; set; }

        public int OpSchedTotalDelay { get; set; }

        #endregion

        #region Constructors

        public HybridAlgProcessor()
        {
            GA = new GenAlgProcessor(10, 10);
            FileReader = new JobFileReader();
            Woc = new WocProcessor();
        }

        #endregion

        public void RunHybridAlg(string fileDir)
        {
            FileReader.ReadFile(fileDir);

            var jobs = new List<Job>();
            jobs = FileReader.Jobs;

            GA.RunGeneticAlgorithm(jobs);
            List<List<Job>> opGen = GA.OptimizedGeneration;

            IWocProcessor WOC = new WocProcessor(jobs);
            WOC.PopulatePreferabilityMatrix(opGen);
            OpSched = WOC.ReadPreferabilityMatrix();

            List<CompletionNode> opSchedCompletionList = GA.CalculateCompletionList(OpSched);

            OpSchedTotalDelay = GA.CalculateTotalDelay(opSchedCompletionList);
        }
    }
}
