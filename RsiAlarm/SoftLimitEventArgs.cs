using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RsiAlarm
{
    public class SoftLimitEventArgs : EventArgs
    {
        public double Level;

        public SoftLimitEventArgs(double level)
        {
            this.Level = level;
        }
    }
}
