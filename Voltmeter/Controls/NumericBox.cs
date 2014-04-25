using System.ComponentModel;

namespace Voltmeter.Controls
{
    public sealed class NumericBox : System.Windows.Forms.TextBox
    {
        double _value;

        [DefaultValue(0.0d)]
        public double Value
        {
            get { return _value; }
            set { _value = value; UpdateText(); }
        }

        byte _decimalPlaces;

        [DefaultValue(0)]
        public byte DecimalPlaces
        {
            get { return _decimalPlaces; }
            set { _decimalPlaces = value; UpdateText(); }
        }

        void UpdateText()
        {           
            Text = string.Format("{0:F" + DecimalPlaces + "}", Value);
        }
    }
}
