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

        private int FadeInDuration;

        private int FadeOutDuration;

        private WarningController Controller;

        private DoubleAnimation FadeIn;

        private DoubleAnimation FadeOut;

        private Storyboard WarningStoryboard;

        private WarningState CurrentWarning = WarningState.None;

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
            Controller = new WarningController();
            Controller.KeyboardWarningSoftLimit = LoadIntSetting("KeyboardWarningSoftLimit", 10);
            Controller.KeyboardWarningHardLimit = LoadIntSetting("KeyboardWarningHardLimit", 15);
            Controller.KeyboardIncreaseRate = LoadIntSetting("KeyboardIncreaseRate", 1);
            Controller.KeyboardDecreaseRate = LoadIntSetting("KeyboardDecreaseRate", 5);
            Controller.SoftLimitWarningStart += Controller_SoftLimitWarningStart;
            Controller.HardLimitWarning += Controller_HardLimitWarning;

            FadeInDuration = LoadIntSetting("FadeInDuration", 50);
            FadeOutDuration = LoadIntSetting("FadeOutDuration", 300);

            InitializeComponent();

            KeyboardHook = new KeyboardLowLevelHook();
            KeyboardHook.EventTriggered += KeyboardHook_EventTriggered;
            KeyboardHook.EventFilter = KeyboardLowLevelHook.KeyboardEvents.KeyUp;
            KeyboardHook.Set();

            CreateWarningEffects();
        }

        private void CreateWarningEffects()
        {
            FadeIn = new DoubleAnimation();
            FadeIn.Duration = new Duration(TimeSpan.FromMilliseconds(FadeInDuration));
            FadeIn.FillBehavior = FillBehavior.Stop;

            FadeOut = new DoubleAnimation();
            FadeOut.Duration = new Duration(TimeSpan.FromMilliseconds(FadeOutDuration));
            FadeOut.FillBehavior = FillBehavior.Stop;

            WarningStoryboard = new Storyboard();
            WarningStoryboard.Children.Add(FadeIn);
            WarningStoryboard.Children.Add(FadeOut);
            WarningStoryboard.Completed += WarningStoryboard_Completed;

            Storyboard.SetTarget(FadeIn, WarningRectangle);
            Storyboard.SetTargetProperty(FadeIn, new PropertyPath(Rectangle.OpacityProperty));

            Storyboard.SetTarget(FadeOut, WarningRectangle);
            Storyboard.SetTargetProperty(FadeOut, new PropertyPath(Rectangle.OpacityProperty));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.CaptionHeight;
        }

        private void KeyboardHook_EventTriggered(object sender, KeyboardLowLevelEventArgs e)
        {
            Controller.HandleKey();
        }

        private void Controller_SoftLimitWarningStart(object sender, SoftLimitEventArgs e)
        {
            AnimateTo(e.Level, WarningState.SoftLimit);
        }

        private void Controller_HardLimitWarning(object sender, EventArgs e)
        {
            AnimateTo(1, WarningState.HardLimit);
        }

        private void AnimateTo(double opacity, WarningState state)
        {
            if (CurrentWarning == WarningState.None || CurrentWarning == WarningState.SoftLimit)
            {
                Visibility = Visibility.Visible;

                WarningStoryboard.Stop();
                FadeIn.From = WarningRectangle.Opacity;
                FadeIn.To = opacity;
                FadeOut.From = opacity;
                FadeOut.To = 0;
                WarningStoryboard.Begin();

                Color newColor = ColorMixer.Mix(Colors.Red, Colors.Yellow, opacity);
                WarningRectangle.Fill = new SolidColorBrush(newColor);

                CurrentWarning = state;
            }
        }

        private void WarningStoryboard_Completed(object sender, EventArgs e)
        {
            if (CurrentWarning == WarningState.HardLimit)
            {
                Visibility = Visibility.Hidden;
                CurrentWarning = WarningState.None;
            }
        }
    }
}
