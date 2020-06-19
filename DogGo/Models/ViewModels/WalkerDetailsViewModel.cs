using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models.ViewModels
{
    public class WalkerDetailsViewModel
    {
        public Walker Walker { get; set; }
        public List<Walk> Walks { get; set; }
        public Neighborhood Neighborhood { get; set; }
        public int TotalWalkTime()
        {
            int totalTime = 0;
            Walks.ForEach(w => totalTime += w.Duration);
            return totalTime / 60;
        }

    }
}
