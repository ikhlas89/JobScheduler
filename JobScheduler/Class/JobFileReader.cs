using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JobScheduler.Datatype;
using JobScheduler.Interface;

namespace JobScheduler.Class 
{
    public class JobFileReader : IJobFileReader
    {
        public List<Job> Jobs { get; set; }

        public JobFileReader()
        {
            Jobs = new List<Job>();
        }

        public void ReadFile(string fileDir)
        {
            Jobs.Clear();

            var assembly = Assembly.GetExecutingAssembly();
            var resource = "TSPConsole." + fileDir;

            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                using (var reader = new StreamReader(fileDir))
                {
                    //read jobs until end of file
                    do
                    {
                        var line = reader.ReadLine();

                        //get the substring of coordinates
                        var job = line.Substring(0, line.IndexOf(" "));

                        line = line.Substring(line.IndexOf(" ") + 1);
                        var exec = line.Substring(0, line.IndexOf(" "));
                        var due = line.Substring(line.IndexOf(" ") + 1);

                        //turn to a vertex
                        var tempJob = new Job()
                        {
                            jobID = job,
                            dueTime = Convert.ToInt32(due),
                            execTime = Convert.ToInt32(exec)
                        };

                        //add to list of vertices
                        this.Jobs.Add(tempJob);

                    } while (!reader.EndOfStream);
                }
            }
        }

    }
}
