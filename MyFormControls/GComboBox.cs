using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFormControls
{
    public class GComboBox<T> : ComboBox
    {
        public GComboBox(T defaultMode) : base()
        {
            if (typeof(T).BaseType != typeof(Enum))
            {
                throw new InvalidCastException();
            }

            T[] modes = (T[])Enum.GetValues(typeof(T));
            object[] objects = modes.Select(m => (object)m).ToArray();

            Items.AddRange(objects);
            base.SelectedItem = defaultMode;
        }

        public GComboBox(T[] modes, T defaultMode) : base()
        {
            object[] objects = modes.Select(m => (object)m).ToArray();

            Items.AddRange(objects);
            base.SelectedItem = defaultMode;
        }

        public new T SelectedItem => (T)base.SelectedItem;
    }
}
