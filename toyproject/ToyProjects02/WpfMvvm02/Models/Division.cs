using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfMvvm02.Models
{
    public partial class Division : ObservableObject 
    {
        [ObservableProperty]
        private string divCode = string.Empty;

        [ObservableProperty]
        private string divName = string.Empty;
    }
}
