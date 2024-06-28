using LeagueAppWinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;
using dotenv.net;
using MingweiSamuel.Camille;
using PoniLCU;
using static PoniLCU.LeagueClient;
using System.Security.Principal;
using Microsoft.UI.Xaml;

namespace LeagueAppWinUI.Views;

public sealed partial class MainPage : Page
{
    string? apiKey;
    static LeagueClient? leagueClient;

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        AdministratorStatusTextBlock.Text = IsAdmin() is true
            ? "Running as admin."
            : "NOT running as admin.";
        myButton.IsEnabled = false;
    }

    public static bool IsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
    private async void myButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO: Update API Key every 24 hours
        // Debug -> LeagueAppWinUI Debug Properties -> LeagueAppWinUI (Unpackaged)
        // -> Environment Variables
        apiKey = Environment.GetEnvironmentVariable("RIOT_API");
        if (apiKey != null)
        {
            // Riot API
            //var riotApi = MingweiSamuel.Camille.RiotApi.NewInstance(apiKey);
            //var accData = riotApi.SummonerV4.GetByPUUID(MingweiSamuel.Camille.Enums.Region.EUNE, "pNngUigS14BX3Ghd53rhWivGS54zVxi87l8-MPMf-chcoWUydRmU5gdICco6I4xxaSt8QDzae8W4-g");
            //myText.Text = accData.SummonerLevel.ToString();

            // LCU API
            if (leagueClient != null)
            {
                var accData = await leagueClient.Request(requestMethod.GET, "/lol-summoner/v1/current-summoner");
                if (accData != null)
                {
                    //myText.Text = accData.SummonerLevel.ToString();
                    myText.Text = accData.ToString();
                }
            }
        }
    }
    public async void ShowErrorNoClient()
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Title = "Error";
        dialog.PrimaryButtonText = "Exit";
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.Content = "Could not find League of Legends client.\nPlease start the client first and then Project Tomato.";
       

        ContentDialogResult result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            Environment.Exit(0);
        }
    }

    private void getClientButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO: shit doesnt work, find a way to check if client is actually running
        leagueClient = new LeagueClient(credentials.lockfile);
        if (leagueClient == null)
        {
            ShowErrorNoClient();
        }
        else
        {
            getClientButton.IsEnabled = false;
            myButton.IsEnabled = true;
        }
    }
}
