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
    public class KeyboardLowLevelHook : GlobalHook<KeyboardLowLevelEventArgs>
    {
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;

        private const int WM_KEYUP = 0x0101;

        private const int WM_SYSKEYDOWN = 0x0104;

        private const int WM_SYSKEYUP = 0x0105;
        
        [Flags]
        public enum KeyboardEvents
        {
            Any        = 0,
            KeyDown    = 1,
            KeyUp      = 2,
            SysKeyDown = 4,
            SysKeyUp   = 8
        }

        public KeyboardEvents EventFilter;

        public override int HookTypeId
        {
            get
            {
                return WH_KEYBOARD_LL;
            }
        }

        protected override KeyboardLowLevelEventArgs CreateEventArgs(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KeyboardLowLevelEventArgs e = new KeyboardLowLevelEventArgs();

            e.VkCode = Marshal.ReadInt32(lParam);
            e.ScanCode = Marshal.ReadInt32(lParam + 4);
            int flags = Marshal.ReadInt32(lParam + 8);

            e.IsExtended = (flags & (1)) != 0;
            e.IsInjected = (flags & (1 << 4)) != 0;
            e.IsAltDown  = (flags & (1 << 5)) != 0;
            e.IsUp       = (flags & (1 << 7)) != 0;
            return e;
        }

        protected override bool ShouldTriggerEvent(int nCode, IntPtr wParam)
        {
            return (nCode >= 0)
                && (EventFilter == KeyboardEvents.Any
                    || (wParam == (IntPtr)WM_KEYDOWN && EventFilter.HasFlag(KeyboardEvents.KeyDown))
                    || (wParam == (IntPtr)WM_KEYUP && EventFilter.HasFlag(KeyboardEvents.KeyUp))
                    || (wParam == (IntPtr)WM_SYSKEYDOWN && EventFilter.HasFlag(KeyboardEvents.SysKeyDown))
                    || (wParam == (IntPtr)WM_SYSKEYUP && EventFilter.HasFlag(KeyboardEvents.SysKeyUp)));
        }
    }
}
