using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TelegramBotTest
{
  /// <summary>
  /// Main logic for MainWindow.xaml. Entrance to the program.
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();  
      FilmBot filmBot = new FilmBot();
    }
  }
}
