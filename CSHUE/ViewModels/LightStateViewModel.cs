using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using CSHUE.Views;
using Q42.HueApi;

namespace CSHUE.ViewModels
{
    public class LightStateViewModel : BaseViewModel
    {
        public LightStateCell Content { get; set; }
    }
}
