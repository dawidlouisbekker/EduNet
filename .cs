using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.app1
{
    internal class GridSpanChanger : INotifyPropertyChanged
    {
        private int spanValue;
        public int SpanValue
        {
            get => spanValue;
            set
            {
                if (spanValue != value)
                {
                    spanValue = value;
                    OnPropertyChanged(nameof(SpanValue));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
