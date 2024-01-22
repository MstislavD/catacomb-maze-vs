using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFormControls
{
    public class OneIntPanel : ParameterPanel
    {
        public int Int1 = 2;

        public OneIntPanel(string int1)
        {
            TextPanel aPanel = InitializeTextPanel($"{int1} =");
            aPanel.TextBox.Text = Int1.ToString();
            aPanel.TextBox.TextChanged += (s, e) => ParameterChanged(ref Int1, aPanel.TextBox);
        }
    }
}
