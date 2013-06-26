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
using System.Runtime.InteropServices;

namespace RsiAlarm
{
    public abstract class GlobalHook<E> where E : EventArgs
    {
        private IntPtr HookID = IntPtr.Zero;

        private User32.WindowsHookProc HookProc;

        public void Set()
        {
            if (HookID != IntPtr.Zero)
            {
                throw new InvalidOperationException("Global hook is already set");
            }

            HookProc = new User32.WindowsHookProc(HookCallback);
            HookID = SetHook(HookProc);
        }

        public void Unset()
        {
            if (HookID == IntPtr.Zero)
            {
                throw new InvalidOperationException("Global hook is not set");
            }

            User32.UnhookWindowsHookEx(HookID);
            HookID = IntPtr.Zero;
            HookProc = null;
        }

        public event EventHandler<E> EventTriggered;

        private IntPtr SetHook(User32.WindowsHookProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return User32.SetWindowsHookEx(HookTypeId, proc, Kernel32.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (ShouldTriggerEvent(nCode, wParam))
            {
                if (EventTriggered != null)
                {
                    E e = CreateEventArgs(nCode, wParam, lParam);

                    EventTriggered(this, e);
                }
            }
            return User32.CallNextHookEx(HookID, nCode, wParam, lParam);
        }

        protected abstract bool ShouldTriggerEvent(int nCode, IntPtr wParam);

        protected abstract E CreateEventArgs(int nCode, IntPtr wParam, IntPtr lParam);

        public abstract int HookTypeId { get; }
    }
}
