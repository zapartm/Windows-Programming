using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace task5
{
    
    public partial class MainWindow : Window
    {
        public List<Button> buttons = new List<Button>();
        public List<Button> buttonsOriginalOrder = new List<Button>();
        public ObservableCollection<ExpanderElement> collection = new ObservableCollection<ExpanderElement>();

        public Button PickedButton1, PickedButton2;
        public BitmapImage[] images = new BitmapImage[8];
        private double FlipBackTime = 1000;
        public bool SettingsActive { get; set; } = true;
        private bool gameActive = new bool();
        public int TimeLeft = 20, buttonsLeft = 16;
        DispatcherTimer timer;
        int imagesShown = 0;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            Button[] T = { B1, B2, B3, B4, B5, B6, B7, B8, B9, B10, B11, B12, B13, B14, B15, B16 };
            buttons.AddRange(T);
            buttonsOriginalOrder = buttons.ToList();
            buttons.Shuffle();

            for (int i = 0; i < 8; i++)
            {
                string str = "C:/Users/Michał/Documents/Visual Studio 2015/Projects/sem4/PWSG/task5/resources/" + (i + 1).ToString() + ".jpg";
                images[i] = new BitmapImage();
                images[i].BeginInit();
                images[i].UriSource = new Uri(str);
                images[i].EndInit();

                collection.Add(new ExpanderElement(str));
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            
        }
      
        private void timer_Tick(object sender, EventArgs e)
        {
            TimeLeft--;
            textBlock.Text = "Time: " + TimeLeft.ToString();
            if(TimeLeft == 0)
            {
                ShowMessageBox();
            }
        }

        bool[] IsClicked = new bool[16];

        // jeśli został wybrany pierwszy z pary to jest on przypisany do lastClicked i firstClicked = true
        private Button lastClicked = new Button();
        private int firstClickedInd = -1;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!gameActive) return;

            var b = sender as Button;
            int i = buttons.FindIndex((Button b1) => { return b1.Name == b.Name; });

            if (!IsClicked[i] && firstClickedInd < 0)
            {
                var brush = new ImageBrush();
                brush.ImageSource = images[i % 8];
                b.Background = brush;
                firstClickedInd = i;
                lastClicked = b;
                IsClicked[i] = true;

                b.OpacityAnimation(0, 1, 1000);
            }
            else if (!IsClicked[i] && firstClickedInd > -1)
            {
                gameActive = false;
                var brush = new ImageBrush();
                brush.ImageSource = images[i % 8];
                b.Background = brush;

                b.OpacityAnimation(0, 1, 1000);

                TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)FlipBackTime);
                await Task.Delay(ts);
                if (i % 8 != firstClickedInd % 8)
                {
                    b.ClearValue(BackgroundProperty);
                    lastClicked.ClearValue(BackgroundProperty);
                    IsClicked[firstClickedInd] = false;
                }
                else
                {
                    b.ClearValue(BackgroundProperty);
                    lastClicked.ClearValue(BackgroundProperty);
                    b.OpacityAnimation(1, 0, 1000);
                    lastClicked.OpacityAnimation(1, 0, 1000);
                    buttonsLeft -= 2;
                }

                imagesShown += 2;
                gameActive = true;
                IsClicked[i] = IsClicked[firstClickedInd] = false;
                firstClickedInd = -1;

                if(imagesShown == 16)
                {
                    ShowMessageBox();
                    return;
                }
            }

        }

        private async void ItsTheFinalCountdownTutuuuutu()
        {
            button.IsEnabled = false;
            button1.IsEnabled = false;
            ResetButton_Click(null, null);

            foreach (var b in buttons)
            {
                b.Visibility = Visibility.Hidden;
                b.Opacity = 0;
            }

            for (int i = 0; i < 16; i++)
            {
                buttonsOriginalOrder[i].Visibility = Visibility.Visible;
                var brush = new ImageBrush();
                brush.ImageSource = images[i % 8];
                buttonsOriginalOrder[i].Background = brush;
                buttonsOriginalOrder[i].OpacityAnimation(0, 1, 800);

                TimeSpan ts = new TimeSpan(0, 0, 0, 0, 800);
                await Task.Delay(ts);
            }

            button.IsEnabled = true;
            button1.IsEnabled = true;
        }

        private void ShowMessageBox()
        {
            string str = "";
            if (buttonsLeft == 0)
                str = "You won! Wanna play again?";
            else
                str = "You lost! Wanna play again?";

            MessageBoxResult result = MessageBox.Show(str, "Game result:", MessageBoxButton.YesNo, MessageBoxImage.None);
            
            if (result == MessageBoxResult.Yes)
            {
                ResetButton_Click(null, null);
            }
            else if(result == MessageBoxResult.No)
            {
                ItsTheFinalCountdownTutuuuutu();
            }
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FlipBackTime = e.NewValue + 250;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            if((string)b.Content=="Play")
            {
                timer.Start();
                gameActive = true;
                b.Content = "Pause";
            }
            else
            {
                timer.Stop();
                gameActive = false;
                b.Content = "Play";
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            buttons.Shuffle();
            imagesShown = 0;
            buttonsLeft = 16;
            foreach (var b in buttons)
            {
                
                b.ClearValue(BackgroundProperty);
                //b.Opacity = 1;
            }
            firstClickedInd = -1;
            for (int i = 0; i < 16; i++)
                IsClicked[i] = false;
            timer.Stop();
            TimeLeft = 20;
            textBlock.Text = "Time: " + TimeLeft.ToString();
            gameActive = false;
            button.Content = "Play";
        }

        private void SettingsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            var len = new System.Windows.GridLength(250);
            SettingsColumn.Width = len;
        }

        private void SettingsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            var len = new System.Windows.GridLength(0);
            SettingsColumn.Width = len;
        }
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {

        }

    }
}
