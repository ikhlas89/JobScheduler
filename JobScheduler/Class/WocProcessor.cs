using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobScheduler.Datatype;
using JobScheduler.Interface;

namespace JobScheduler.Class
{
    public class WocProcessor : IWocProcessor
    {
        public int[,] PreferabilityMatrix { get; set; }

        private List<Job> _initialSched;

        #region Constructors

        public WocProcessor()
        {
            InitializePreferabilityMatrix(0);
            _initialSched = new List<Job>();
        }

        public WocProcessor(List<Job> initialSchedule)
        {
            InitializePreferabilityMatrix(initialSchedule.Count);
            _initialSched = initialSchedule;
        }

        #endregion

        #region Methods

        public void InitializePreferabilityMatrix(int size)
        {
            PreferabilityMatrix = new int[size, size];
            PreferabilityMatrix.Initialize();
        }

        public void PopulatePreferabilityMatrix(List<List<Job>> opGen)
        {
            foreach (var schedule in opGen)
            {
                //i = job; j = index of job
                int i = 0, j = 0;

                foreach (var job in schedule)
                {
                    i = _initialSched.IndexOf(job);

                    PreferabilityMatrix[i, j] += 1;

                    j++;
                }
            }
        }

        public List<Job> ReadPreferabilityMatrix()
        {
            List<Job> opSched = new List<Job>();
            List<Job> unreadJobs = new List<Job>(_initialSched);

            //i = row index
            for (int i = 0; i < _initialSched.Count; i++)
            {
                var rowMax = -1;
                int jMax = -1;

                for (int j = 0; j < _initialSched.Count; j++)
                {
                    bool isNewMax = (PreferabilityMatrix[i, j] > rowMax);

                    if (isNewMax)
                    {
                        if (unreadJobs.Contains(_initialSched[j]))
                        {
                            jMax = j;
                            rowMax = PreferabilityMatrix[i, j];
                        }
                    }
                }

                unreadJobs.Remove(_initialSched[jMax]);
                opSched.Add(_initialSched[jMax]);
            }

            return opSched;
        }

        #endregion



    }
}
