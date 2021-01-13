using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualClass.Models.Result
{
    public class ResultErrorDTO : ResultDTO
    {
        public List<string> Errors { get; set; }
    }
}
