﻿using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace ControlCasaLuces.CustomControls
{
    public sealed partial class Light : UserControl, INotifyPropertyChanged
    {
        private string lightName;
        private bool isLightOn;

        public string LightName
        {
            get { return lightName; }
            set { lightName = value; txtLight.Text = lightName; }
        }

        public bool IsLightOn
        {
            get { return isLightOn; }
            set
            {
                isLightOn = value;
                RaisePropertyChanged("IsLightOn");
                ChangeBackground(isLightOn);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ChangeBackground(bool receivedValue)
        {
            if (receivedValue)
            {
                borBackground.Background = new SolidColorBrush(Color.FromArgb(255, 15, 106, 158));
            }
            else
            {
                borBackground.Background = new SolidColorBrush(Color.FromArgb(51, 15, 106, 158));
            }
        }

        public Light()
        {
            InitializeComponent();
        }
    }
}
