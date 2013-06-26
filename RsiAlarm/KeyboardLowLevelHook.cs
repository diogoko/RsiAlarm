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
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace RsiAlarm
{
    public class KeyboardLowLevelHook : GlobalHook<KeyboardLowLevelEventArgs>
    {
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;

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

            BitVector32 flagsBits = new BitVector32(flags);
            e.IsExtended = flagsBits[0];
            e.IsInjected = flagsBits[4];
            e.IsAltDown = flagsBits[5];
            e.IsUp = flagsBits[7];
            return e;
        }

        protected override bool ShouldTriggerEvent(int nCode, IntPtr wParam)
        {
            return nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN;
        }
    }
}
