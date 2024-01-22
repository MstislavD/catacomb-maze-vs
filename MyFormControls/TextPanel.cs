using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFormControls
{
    public class TextPanel : FlowLayoutPanel
    {
        public TextBox TextBox;

        public TextPanel(String text)
        {
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoSize = true;
            FlowDirection = FlowDirection.LeftToRight;
            Margin = new Padding(0);

            Label label = new Label();
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.Margin = new Padding(0);
            label.AutoSize = true;
            Controls.Add(label);

            TextBox = new TextBox();
            TextBox.Margin = new Padding(0);
            TextBox.Width = 50;
            Controls.Add(TextBox);
        }
    }
}
