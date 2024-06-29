using LeagueAppWinUI.ViewModels;
using Microsoft.UI.Xaml.Controls;
using MingweiSamuel.Camille;
using PoniLCU;
using static PoniLCU.LeagueClient;
using System.Security.Principal;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using LeagueAppWinUI.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using System.Xml.Linq;

namespace LeagueAppWinUI.Views;

public sealed partial class MainPage : Page
{
    string? apiKey = Environment.GetEnvironmentVariable("RIOT_API");
    static LeagueClient? leagueClient;

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    public static bool IsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
    private async void getInfoButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        double left = (profilePictureCanvas.ActualWidth - progressBarNextLevel.ActualWidth) / 2;
        Canvas.SetLeft(progressBarNextLevel, left);
        double left2 = (profilePictureCanvas.ActualWidth - profilePic.ActualWidth) / 2;
        Canvas.SetLeft(profilePic, left2);
        //double left3 = (profilePictureCanvas.ActualWidth - profileName.ActualWidth) / 2;
        //Canvas.SetLeft(profileName, left3);

        leagueClient = new LeagueClient(credentials.lockfile);
        // TODO: Update API Key every 24 hours
        // Debug -> LeagueAppWinUI Debug Properties -> LeagueAppWinUI (Unpackaged)
        // -> Environment Variables
        //if (apiKey != null)
        //{
        //    // Riot API
        //    //var riotApi = MingweiSamuel.Camille.RiotApi.NewInstance(apiKey);
        //    //var accData = riotApi.SummonerV4.GetByPUUID(MingweiSamuel.Camille.Enums.Region.EUNE, "pNngUigS14BX3Ghd53rhWivGS54zVxi87l8-MPMf-chcoWUydRmU5gdICco6I4xxaSt8QDzae8W4-g");
        //    //myText.Text = accData.SummonerLevel.ToString();

            

        //}
        // LCU API
        if (leagueClient != null)
        {
            try
            {
                string accData = await leagueClient.Request(requestMethod.GET, "/lol-summoner/v1/current-summoner");
                if (accData != null)
                {
                    SummonerProfile? summonerProfile = new SummonerProfile();
                    summonerProfile = JsonConvert.DeserializeObject<SummonerProfile>(accData);

                    BitmapImage bi3 = new BitmapImage();
                    bi3.UriSource = new Uri("https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/profile-icons/" + summonerProfile.profileIconId + ".jpg", UriKind.Absolute);
                    profilePic.ProfilePicture = bi3;

                    profileName.Text = summonerProfile.gameName + "#" + summonerProfile.tagLine;

                    progressBarNextLevel.Value = (double)summonerProfile.percentCompleteForNextLevel;
                }
            }
            catch (Exception)
            {
                ShowErrorNoClient();
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
}
