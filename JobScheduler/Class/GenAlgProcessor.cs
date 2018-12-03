using System;
using System.Collections.Generic;
using System.Linq;
using JobScheduler.Datatype;
using JobScheduler.Interface;

namespace JobScheduler.Class
{
    public class GenAlgProcessor : IGenAlgProcessor
    {
        #region Properties 

        public List<List<Job>> OptimizedGeneration { get; set; }

        public int NumIterations { get; set; }

        public int GenerationSize { get; set; }

        private const double CrossoverProb = .6;

        private const double MutationProb = .01;

        private Random _rng;

        #endregion

        #region Constructors

        public GenAlgProcessor()
        {
            _rng = new Random();
            GenerationSize = 0;
            NumIterations = 0;
        }

        public GenAlgProcessor(int generationWidth, int iterations)
        {
            _rng = new Random();
            GenerationSize = generationWidth;
            NumIterations = iterations;

        }

        #endregion

        public void RunGeneticAlgorithm(List<Job> jobList)
        {

            //generate randomized parent generation
            List<List<Job>> parentGen = GenerateRandomizedGeneration(jobList);
            List<List<Job>> childGen = new List<List<Job>>();

            for (int i = 0; i < NumIterations; i++)
            {
                //calculate fitness of parent generation
                var fitnessList = CalculateGenerationFitness(parentGen);

                //generate child generation
                childGen.Clear();
                while (childGen.Count < GenerationSize)
                {
                    List<List<Job>> parentPair = RouletteWheelSelection(fitnessList, parentGen);

                    //2-point crossover
                    List<List<Job>> childPair = Crossover2Pt(parentPair);

                    //mutation
                    childPair[0] = MutationReverseSeq(childPair[0]);
                    childPair[1] = MutationReverseSeq(childPair[1]);

                    childGen.AddRange(childPair);
                }

                //set parent generation = child generation
                parentGen.Clear();
                parentGen.AddRange(childGen);
            }

            OptimizedGeneration = parentGen;
        }

        private List<List<Job>> GenerateRandomizedGeneration(List<Job> jobList)
        {
            List<List<Job>> genList = new List<List<Job>>();
            
            //populate genList with n = GenerationSize number of copies of jobList
            for (int i = 0; i < GenerationSize; i++)
            {
                genList.Add(jobList);
            }

            //randomize each schedule within genList
            for (int i = 0; i < GenerationSize; i++)
            {
                genList[i] = RandomizeList(genList[i]);
            }

            return genList;

        }

        private List<Job> RandomizeList(List<Job> inputList)
        {
            List<Job> schedule = new List<Job>(inputList);

            for (int i = 0; i < schedule.Count ; i++)
            {
                var swapIndex = _rng.Next(0, schedule.Count - 1);

                Job tempJob = schedule[i];
                schedule[i] = schedule[swapIndex];
                schedule[swapIndex] = tempJob;
            }

            return schedule;
        }

        private List<double> CalculateGenerationFitness(List<List<Job>> generationList)
        {
            //calculate generation completion list
            List<List<CompletionNode>> generationCompletionList = new List<List<CompletionNode>>();

            for (int i = 0; i < generationList.Count; i++)
            {
                var tempCompletionList = CalculateCompletionList(generationList[i]);

                generationCompletionList.Add(tempCompletionList);
            }

            //for each generation completion list member, calculate total delay
            List<double> fitnessList = new List<double>();

            foreach (var completionList in generationCompletionList)
            {
                //fitness heuristic = inverse of total delay -> ( small delay = high fitness = good / large delay = low fitness = bad )
                var fitness = (1 / (double)CalculateTotalDelay(completionList));
                fitnessList.Add(fitness);
            }

            return fitnessList;
        }

        public int CalculateTotalDelay(List<CompletionNode> completionList)
        {
            var delay = 0;

            foreach (var completionNode in completionList)
            {
                delay += completionNode.Delay;
            }

            return delay;
        }

        public List<CompletionNode> CalculateCompletionList(List<Job> schedule)
        {
            List<CompletionNode> completionList = new List<CompletionNode>();
            var scheduleClock = 0;
            
            foreach (var job in schedule)
            {
                scheduleClock += job.execTime;

                var delay = (scheduleClock - job.dueTime)<0 ? 0 : (scheduleClock - job.dueTime);

                var tempCompletionNode = new CompletionNode()
                {
                    Job = job.jobID,
                    Completion = scheduleClock,
                    Delay = delay
                };

                completionList.Add(tempCompletionNode);
            }

            return completionList;
        }

        private List<List<Job>> RouletteWheelSelection(List<double> fitnessList, List<List<Job>> generation)
        {
            var rangeMax = 0.0;
            List<Job> schedA = new List<Job>(), schedB = new List<Job>();

            foreach (var i in fitnessList)
            {
                rangeMax += i;
            }

            do
            {
                //find schedA
                do
                {
                    double selectedA = _rng.NextDouble() * rangeMax;

                    for (int i = 0; i < fitnessList.Count; i++)
                    {
                        if (selectedA - fitnessList[i] > 0)
                        {
                            selectedA -= fitnessList[i];
                        }
                        else
                        {
                            schedA = generation[i];
                            break;
                        }
                    }
                } while (schedA.Count == 0);

                //find pathB
                do
                {
                    double selectedB = _rng.NextDouble() * rangeMax;

                    for (int i = 0; i < fitnessList.Count; i++)
                    {
                        if (selectedB - fitnessList[i] > 0)
                        {
                            selectedB -= fitnessList[i];
                        }
                        else
                        {
                            schedB = generation[i];
                            break;
                        }
                    }
                } while (schedB.Count == 0);

                //loop until schedA is not in the same order as schedB
            } while (schedA.SequenceEqual(schedB));

            List<List<Job>> parentPair = new List<List<Job>>();
            parentPair.Add(schedA);
            parentPair.Add(schedB);

            return parentPair;
        }

        private List<List<Job>> Crossover2Pt(List<List<Job>> parents)
        {
            List<Job> childA = new List<Job>(), childB = new List<Job>();

            if (_rng.NextDouble() <= CrossoverProb)
            {
                int i, j;

                do
                {
                    i = _rng.Next(0, parents[0].Count - 1);
                    j = _rng.Next(0, parents[0].Count - 1);
                } while (i >= j);

                //get the jobs that will be swapped
                List<Job> swapA = parents[0].GetRange(i, (j - i));
                List<Job> swapB = parents[1].GetRange(i, (j - i));

                childA.AddRange(parents[0].GetRange(0, i));
                childA.AddRange(parents[1].GetRange(i, j - i));
                childA.AddRange(parents[0].GetRange(j, parents[0].Count - j));

                childB.AddRange(parents[1].GetRange(0, i));
                childB.AddRange(parents[0].GetRange(i, j - i));
                childB.AddRange(parents[1].GetRange(j, parents[0].Count - j));

                //compare to find unique jobs that are swapped
                for (int k = 0; k < swapB.Count; k++)
                {
                    if (swapA.Contains(swapB[k]))
                    {
                        //remove element from swap A & B, and reset k = 0
                        swapA.Remove(swapB[k]);
                        swapB.Remove(swapB[k]);

                        k = 0;
                    }
                }

                //childA re-swap
                //reswap variables within the substring that was swapped
                //swapA - variables to insert
                //swapB - variables to replace
                var itr = 0;
                foreach (var job in swapB)
                {
                    var index = childA.FindIndex(x => x.Equals(job));

                    if (index < i || index > j)
                    {
                        index = childA.FindLastIndex(x => x.Equals(job));
                    }

                    childA[index] = swapA[itr];
                    itr++;
                }

                //childB re-swap
                //reswap variables within the substring that was swapped
                //swapB - variables to insert
                //swapA - variables to replace
                itr = 0;
                foreach (var job in swapA)
                {
                    var index = childB.FindIndex(x => x.Equals(job));

                    if (index < i || index > j)
                    {
                        index = childB.FindLastIndex(x => x.Equals(job));
                    }

                    childB[index] = swapB[itr];
                    itr++;
                }
            }
            else
            {
                //no crossover
                childA = parents[0];
                childB = parents[1];
            }

            List<List<Job>> childPair = new List<List<Job>>();
            childPair.Add(childA);
            childPair.Add(childB);

            return childPair;
        }

        private List<Job> MutationReverseSeq(List<Job> child)
        {
            if (_rng.NextDouble() <= MutationProb)
            {
                int i = 0;//also acts as index for flipping
                int j = 0;

                while (i >= j)
                {
                    i = _rng.Next(0, child.Count - 1);
                    j = _rng.Next(0, child.Count - 1);
                }

                List<Job> subList = child.GetRange(i, j - i);
                subList.Reverse();

                foreach (var vertex in subList)
                {
                    child[i] = vertex;
                    i++;
                }
            }

            return child;
        }
    
    }
}
