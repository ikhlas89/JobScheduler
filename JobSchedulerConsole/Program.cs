using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobScheduler.Interface;
using JobScheduler.Class;
using JobScheduler.Datatype;

namespace JobSchedulerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string jobString = "JobFiles/jobs10.txt";

            Stopwatch sw = new Stopwatch();

            IHybridAlgProcessor hybridAlg = new HybridAlgProcessor();
            IJobFileReader reader = new JobFileReader();
            IGenAlgProcessor GA = new GenAlgProcessor(10, 10); 

            reader.ReadFile(jobString);
            List<Job> originalJobList = reader.Jobs;
            List<CompletionNode> originalCompletionList = GA.CalculateCompletionList(originalJobList);
            
            Console.WriteLine("Beginning Schedule");
            Console.WriteLine("\tTotal Delay: " + GA.CalculateTotalDelay(originalCompletionList));
            Console.WriteLine("\tSchedule:");
            Console.WriteLine("\t\tjobID\t\texecTime\tdueTime");

            foreach (var job in originalJobList)
            {
                Console.WriteLine("\t\t" + job.jobID + "\t\t" + job.execTime + "\t\t" + job.dueTime);
            }

            Console.WriteLine("\nRunning Hybrid Algorithm...\n");

            sw.Start();
            hybridAlg.RunHybridAlg(jobString);
            sw.Stop();
            
            Console.WriteLine("Optimal Schedule");
            Console.WriteLine("\tElapsed Time (ms): " + sw.ElapsedMilliseconds);
            Console.WriteLine("\tTotal Delay: " + hybridAlg.OpSchedTotalDelay);
            Console.WriteLine("\tCalculated Schedule:");
            Console.WriteLine("\t\tjobID\t\texecTime\tdueTime");

            foreach (var job in hybridAlg.OpSched)
            {
                Console.WriteLine("\t\t"+job.jobID+"\t\t"+job.execTime+"\t\t"+job.dueTime);
            }

            Console.ReadLine();

        }
    }
}
