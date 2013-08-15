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
using System.Windows.Media;

namespace RsiAlarm
{
    public class ColorMixer
    {
        public static Color Mix(Color c1, Color c2, double amount)
        {
            Color mixed = new Color();

            mixed.A = (byte)((c1.A * amount) + (c2.A * (1.0 - amount)));
            mixed.R = (byte)((c1.R * amount) + (c2.R * (1.0 - amount)));
            mixed.G = (byte)((c1.G * amount) + (c2.G * (1.0 - amount)));
            mixed.B = (byte)((c1.B * amount) + (c2.B * (1.0 - amount)));

            return mixed;
        }
    }
}
