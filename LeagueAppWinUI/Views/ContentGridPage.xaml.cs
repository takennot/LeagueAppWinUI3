using LeagueAppWinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace LeagueAppWinUI.Views;

public sealed partial class ContentGridPage : Page
{
    public ContentGridViewModel ViewModel
    {
        get;
    }

    public ContentGridPage()
    {
        ViewModel = App.GetService<ContentGridViewModel>();
        InitializeComponent();
    }
}
