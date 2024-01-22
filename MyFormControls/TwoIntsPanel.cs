using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFormControls
{
    public class TwoIntsPanel : ParameterPanel
    {
        public int Int1 = 2;
        public int Int2 = 3;

        public TwoIntsPanel(string int1, string int2)
        {
            TextPanel aPanel = InitializeTextPanel($"{int1} =");
            aPanel.TextBox.Text = Int1.ToString();
            aPanel.TextBox.TextChanged += (s, e) => ParameterChanged(ref Int1, aPanel.TextBox);

            TextPanel bPanel = InitializeTextPanel($"{int2} =");
            bPanel.TextBox.Text = Int2.ToString();
            bPanel.TextBox.TextChanged += (s, e) => ParameterChanged(ref Int2, bPanel.TextBox);
        }
    }
}
