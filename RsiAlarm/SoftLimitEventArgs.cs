using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RsiAlarm
{
    public class SoftLimitEventArgs : EventArgs
    {
        public int Level;

        public SoftLimitEventArgs(int level)
        {
            this.Level = level;
        }
    }
}
