using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для MultistateButton.xaml
    /// </summary>
    public partial class MultistateButton : UserControl
    {
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(object), typeof(MultistateButton));
        

        public event RoutedEventHandler Click
        {
            add
            {
                _button.Click += value;
            }
            remove
            {
                _button.Click -= value;
            }
        }


        public MultistateButton()
        {
            InitializeComponent();
        }

        object _state = 0;

        public Dictionary<object, string> Statuses { get; set; } = new Dictionary<object, string>()
        {
            [0] = "MultistateButton0.png",
            [1] = "MultistateButton1.png",
            [2] = "MultistateButton2.png",
        };

        public string Text
        {
            get => _button.Content.ToString();
            set => _button.Content = value;
        }


        public object State
        {
            get => GetValue(StateProperty);
            set
            {
                SetValue(StateProperty, value);
                if (_state != value)
                {
                    if (Statuses.ContainsKey(value))
                    {
                        var image = Statuses[value];
                        var sr = Application.GetResourceStream(new Uri(@"Images\" + image, UriKind.Relative));
                        BitmapImage btpImg = new BitmapImage();
                        btpImg.BeginInit();
                        btpImg.StreamSource = sr.Stream;
                        btpImg.EndInit();
                        _button.Background = new ImageBrush(btpImg) { Stretch = Stretch.Uniform };
                    }
                    _state = value;
                }
            }
        }
    }
}
