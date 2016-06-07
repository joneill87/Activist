using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Models
{
    public class JobIteration
    {
        public Guid IterationID { get; set; }

        public Guid JobID { get; set; }

        public Guid PreviousIterationID { get; set; }

        public string[] QueryIDs { get; set; }
        public string[] QueryLabels { get; set; }
    }
}
