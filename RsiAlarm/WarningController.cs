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

        public long KeyboardIncreaseRate;

        public long KeyboardDecreaseRate;

        /// <summary>
        /// Occurs when keyboard points go above the soft limit but still under the hard limit.
        /// </summary>
        public event EventHandler<SoftLimitEventArgs> SoftLimitWarningStart;

        /// <summary>
        /// Occurs when keyboard points go above the hard limit.
        /// </summary>
        public event EventHandler HardLimitWarning;

        /// <summary>
        /// Occurs when the soft limit warning finishes (keyboard points go below soft limit).
        /// </summary>
        public event EventHandler SoftLimitWarningEnd;

        private long KeyboardPoints = 0;

        private WarningState CurrentWarning = WarningState.None;

        private Stopwatch KeyboardStopwatch;

        public WarningController()
        {
            KeyboardStopwatch = Stopwatch.StartNew();
        }

        public void HandleKey()
        {
            KeyboardPoints += KeyboardIncreaseRate;

            long elapsed = KeyboardStopwatch.ElapsedMilliseconds;
            if (elapsed >= 100)
            {
                long pointsToRemove = elapsed * KeyboardDecreaseRate / 1000;
                KeyboardPoints = Math.Max(KeyboardPoints - pointsToRemove, 0);

                KeyboardStopwatch.Restart();
            }

            if (KeyboardPoints >= KeyboardWarningHardLimit)
            {
                CurrentWarning = WarningState.None;

                if (HardLimitWarning != null)
                {
                    HardLimitWarning(this, new EventArgs());
                }

                KeyboardPoints = 0;
            }
            else if (KeyboardPoints >= KeyboardWarningSoftLimit)
            {
                CurrentWarning = WarningState.SoftLimit;

                if (SoftLimitWarningStart != null)
                {
                    double level = ((double)KeyboardPoints - (double)KeyboardWarningSoftLimit) / ((double)KeyboardWarningHardLimit - (double)KeyboardWarningSoftLimit);
                    SoftLimitWarningStart(this, new SoftLimitEventArgs(level));
                }
            }
            else if (KeyboardPoints < KeyboardWarningSoftLimit && CurrentWarning == WarningState.SoftLimit)
            {
                CurrentWarning = WarningState.None;

                if (SoftLimitWarningEnd != null)
                {
                    SoftLimitWarningEnd(this, new EventArgs());
                }
            }
        }
    }
}
