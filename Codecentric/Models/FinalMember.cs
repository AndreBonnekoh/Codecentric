using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecentric
{
    public class FinalMember
    {
        public MemberInformation Member { get; set; }
        public Repository[] Repo { get; set; }
        public List<string> Languages { get; set; }
    }
}
