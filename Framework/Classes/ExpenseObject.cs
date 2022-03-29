using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class ExpenseObject
    {
        public string Name {get; set;}
        public ExpenseType Type { get; set; }
        public bool Recurring { get; set; }
        public int Amount { get; set; }
    }
}
