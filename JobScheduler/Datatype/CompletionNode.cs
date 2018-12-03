using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Datatype
{
    public struct CompletionNode
    {
        public string Job;

        public int Completion;

        public int Delay;
    }
}
