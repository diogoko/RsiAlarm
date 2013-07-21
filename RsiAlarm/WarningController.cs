/*
 * Copyright 2013 Diogo Kollross
 *
 * This file is part of RsiAlarm.
 *
 * RsiAlarm is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * RsiAlarm is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with RsiAlarm. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Diagnostics;

namespace RsiAlarm
{
    public class WarningController
    {
        public long KeyboardWarningSoftLimit;

        public long KeyboardWarningHardLimit;

        public long KeyboardDecreaseRate;

        public event EventHandler<SoftLimitEventArgs> SoftLimitWarning;

        public event EventHandler HardLimitWarning;

        private long KeyboardPoints = 0;

        private Stopwatch KeyboardStopwatch;

        public WarningController()
        {
            KeyboardStopwatch = Stopwatch.StartNew();
        }

        public void HandleKey()
        {
            KeyboardPoints++;

            long elapsed = KeyboardStopwatch.ElapsedMilliseconds;
            if (elapsed >= 1000)
            {
                long pointsToRemove = elapsed * KeyboardDecreaseRate / 1000;
                KeyboardPoints = Math.Max(KeyboardPoints - pointsToRemove, 0);

                KeyboardStopwatch.Restart();
            }

            if (KeyboardPoints >= KeyboardWarningHardLimit)
            {
                if (HardLimitWarning != null)
                {
                    HardLimitWarning(this, new EventArgs());
                }

                KeyboardPoints = 0;
            }
            else if (KeyboardPoints >= KeyboardWarningSoftLimit)
            {
                if (SoftLimitWarning != null)
                {
                    long level = (KeyboardPoints - KeyboardWarningSoftLimit) * 100 / (KeyboardWarningHardLimit - KeyboardWarningSoftLimit);
                    SoftLimitWarning(this, new SoftLimitEventArgs((int) level));
                }
            }
        }
    }
}
