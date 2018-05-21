using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace task5
{
    static class ExtensionMethods
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void OpacityAnimation(this Button b, double from, double to, double miliseconds)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(miliseconds));
            b.BeginAnimation(Button.OpacityProperty, da);
        }
    }
}
