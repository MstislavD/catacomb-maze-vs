using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFormControls
{
    public class ThreeIntsPanel : ParameterPanel
    {
        public int Int1 = 2;
        public int Int2 = 3;
        public int Int3 = 5;

        public ThreeIntsPanel(string int1, string int2, string int3)
        {
            TextPanel aPanel = InitializeTextPanel($"{int1} =");
            aPanel.TextBox.Text = Int1.ToString();
            aPanel.TextBox.TextChanged += (s, e) => ParameterChanged(ref Int1, aPanel.TextBox);

            TextPanel bPanel = InitializeTextPanel($"{int2} =");
            bPanel.TextBox.Text = Int2.ToString();
            bPanel.TextBox.TextChanged += (s, e) => ParameterChanged(ref Int2, bPanel.TextBox);

            TextPanel cPanel = InitializeTextPanel($"{int3} =");
            cPanel.TextBox.Text = Int3.ToString();
            cPanel.TextBox.TextChanged += (s, e) => ParameterChanged(ref Int3, cPanel.TextBox);
        }
    }
}
