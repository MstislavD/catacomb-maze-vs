using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathematics
{
    public class IntegerParity
    {
        public static IntegerParity ODD = new IntegerParity();
        public static IntegerParity EVEN = new IntegerParity();

        public static IntegerParity GetParity(int i) => i % 2 == 0 ? EVEN : ODD;

        IntegerParity() { }

        public override string ToString()
        {
            return this == ODD ? "Odd" : "Even";
        }
    }
}
