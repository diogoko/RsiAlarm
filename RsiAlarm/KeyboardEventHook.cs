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
using System.Runtime.InteropServices;

namespace RsiAlarm
{
    public class KeyboardEventHook : GlobalHook<KeyboardEventArgs>
    {
        private const int WH_KEYBOARD = 2;

        public override int HookTypeId
        {
            get
            {
                return WH_KEYBOARD;
            }
        }

        protected override KeyboardEventArgs CreateEventArgs(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KeyboardEventArgs e = new KeyboardEventArgs();

            e.VkCode = Marshal.ReadInt32(wParam);
            int flags = Marshal.ReadInt32(lParam);

            e.RepeatCount = flags & 0xFFFF;
            e.ScanCode = (flags >> 16) & 0xFF;
            e.IsExtended = (flags & 0x1000000) != 0;
            e.IsAltDown = (flags & 0x20000000) != 0;
            e.IsUp = (flags & 0x40000000) == 0;
            e.IsReleasing = (flags & 0x80000000) != 0;
            return e;
        }

        protected override bool ShouldTriggerEvent(int nCode, IntPtr wParam)
        {
            return nCode >= 0;
        }
    }
}
