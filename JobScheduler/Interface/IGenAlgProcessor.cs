using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobScheduler.Datatype;

namespace JobScheduler.Interface
{
    public interface IGenAlgProcessor
    {
        #region Properties

        List<List<Job>> OptimizedGeneration { get; set; }

        int NumIterations { get; set; }

        int GenerationSize { get; set; }

        #endregion

        #region Methods

        void RunGeneticAlgorithm(List<Job> jobList);

        int CalculateTotalDelay(List<CompletionNode> completionList);

        List<CompletionNode> CalculateCompletionList(List<Job> schedule);

        #endregion
    }
}
