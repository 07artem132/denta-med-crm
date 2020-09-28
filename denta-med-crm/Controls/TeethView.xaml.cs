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

namespace denta_med_crm.Controls
{
    /// <summary>
    /// Логика взаимодействия для TeethView.xaml
    /// </summary>
    public partial class TeethView : UserControl
    {
        private List<int> availableTags;

        public bool AllowPickAnyTooth { get; set; }
        public event Action<int> ToothPicked;

        public TeethView()
        {
            InitializeComponent();
        }

        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            foreach (var item in _ts1.Children.OfType<T>()) yield return item;
            foreach (var item in _ts2.Children.OfType<T>()) yield return item;
            foreach (var item in _ts3.Children.OfType<T>()) yield return item;
            foreach (var item in _ts4.Children.OfType<T>()) yield return item;

            /*if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }*/
        }

        private MultistateButton SearchByTag(string tag)
        {
            return FindVisualChildren<MultistateButton>(this).Where(x => x.Tag != null && x.Tag.ToString() == tag).First();
        }


        public void SetAvailableTeeth(params int[] tags)
        {
            availableTags = tags.ToList();

            //Reset
            foreach (var item in FindVisualChildren<MultistateButton>(this))
            {
                item.State = 0;
            }

            foreach (var item in availableTags)
            {
                SearchByTag(item.ToString()).State = 1;
            }
        }

        private void MultistateButton_Click(object sender, RoutedEventArgs e)
        {
            var src = (MultistateButton)(((Grid)((Button)sender).Parent).Parent);
            var tag = src.Tag.ToString();
            var tagInt = int.Parse(tag);

            if (availableTags.Contains(tagInt))
            {
                foreach (var item in FindVisualChildren<MultistateButton>(this))
                {
                    if ((int)item.State == 2)
                        item.State = availableTags.IndexOf(int.Parse(item.Tag.ToString())) == -1 ? 0 : 1;
                }

                if ((int)src.State == 1 || AllowPickAnyTooth)
                {
                    src.State = 2;
                    ToothPicked?.Invoke(tagInt);
                }
            }

        }
    }
}
