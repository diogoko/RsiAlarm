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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Configuration;
using System.Reflection;

namespace RsiAlarm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KeyboardLowLevelHook KeyboardHook;

        private long KeyboardPoints = 0;

        private Stopwatch KeyboardStopwatch;

        private long KeyboardPointsWarningLimit;

        private long KeyboardPointsDecreaseRate;

        private int FadeInDuration;

        private int FadeOutDuration;

        private int LoadIntSetting(string key, int defaultValue)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return int.Parse(ConfigurationManager.AppSettings[key]);
            }
            else
            {
                return defaultValue;
            }
        }

        public MainWindow()
        {
            KeyboardPointsWarningLimit = LoadIntSetting("KeyboardPointsWarningLimit", 15);
            KeyboardPointsDecreaseRate = LoadIntSetting("KeyboardPointsDecreaseRate", 5);
            FadeInDuration = LoadIntSetting("FadeInDuration", 50);
            FadeOutDuration = LoadIntSetting("FadeOutDuration", 300);

            InitializeComponent();

            KeyboardHook = new KeyboardLowLevelHook();
            KeyboardHook.EventTriggered += KeyboardHook_EventTriggered;
            KeyboardHook.Set();

            KeyboardStopwatch = Stopwatch.StartNew();

            DoubleAnimation fadeIn = new DoubleAnimation();
            fadeIn.From = 0.0;
            fadeIn.To = 1.0;
            fadeIn.Duration = new Duration(TimeSpan.FromMilliseconds(FadeInDuration));

            DoubleAnimation fadeOut = new DoubleAnimation();
            fadeOut.From = 1.0;
            fadeOut.To = 0.0;
            fadeOut.Duration = new Duration(TimeSpan.FromMilliseconds(FadeOutDuration));

            WarningStoryboard = new Storyboard();
            WarningStoryboard.Children.Add(fadeIn);
            WarningStoryboard.Children.Add(fadeOut);
            WarningStoryboard.Completed += WarningStoryboard_Completed;

            Storyboard.SetTarget(fadeIn, WarningRectangle);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(Rectangle.OpacityProperty));

            Storyboard.SetTarget(fadeOut, WarningRectangle);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(Rectangle.OpacityProperty));
        }

        private void WarningStoryboard_Completed(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void KeyboardHook_EventTriggered(object sender, EventArgs e)
        {
            KeyboardPoints++;

            long elapsed = KeyboardStopwatch.ElapsedMilliseconds;
            if (elapsed >= 1000)
            {
                long pointsToRemove = elapsed * KeyboardPointsDecreaseRate / 1000;
                KeyboardPoints = Math.Max(KeyboardPoints - pointsToRemove, 0);

                KeyboardStopwatch.Restart();
            }

            if (KeyboardPoints >= KeyboardPointsWarningLimit)
            {
                Visibility = Visibility.Visible;

                WarningStoryboard.Begin();
                KeyboardPoints = 0;
            }
        }

        private Storyboard WarningStoryboard;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.CaptionHeight;
        }
    }
}
