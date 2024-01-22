using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFormControls
{
    public class NumericUpDownAdvanced :NumericUpDown
    {
        const decimal _controlMultiplier = 10;
        const decimal _shiftMultiplier = 100;
        const decimal _altMultiplier = 1000;

        public override void UpButton()
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                _modify(_controlMultiplier);
            }
            else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                _modify(_shiftMultiplier);
            }
            else if ((ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                _modify(_altMultiplier);
            }
            else
            {
                _modify(1);
            }
        }

        public override void DownButton()
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                _modify(-_controlMultiplier);
            }
            else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                _modify(-_shiftMultiplier);
            }
            else if ((ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                _modify(-_altMultiplier);
            }
            else
            {
                _modify(-1);
            }
        }

        void _modify(decimal delta)
        {
            delta = delta * Increment;
            if (Value + delta > Maximum)
            {
                Value = Maximum;
            }
            else if (Value + delta < Minimum)
            {
                Value = Minimum;
            }
            else
            {
                Value += delta;
            }
        }
    }
}
