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
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace LeagueAppWinUI.Views;

public sealed partial class MainPage : Page
{
    string? apiKey = Environment.GetEnvironmentVariable("RIOT_API");
    static LeagueClient? leagueClient;
    public static Dictionary<string, dynamic> championsJson = new Dictionary<string, dynamic>();


    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
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
        // this check is useless btw, because even if there is no client, it still returns LeagueClient object
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

                    summonerLevel.Text = "Level: " + summonerProfile.summonerLevel;
                }

                try
                {
                    string boosts = await leagueClient.Request(requestMethod.GET, "/lol-active-boosts/v1/active-boosts");
                    if (boosts != null)
                    {
                        SummonerBoosts summonerBoosts = new SummonerBoosts();
                        summonerBoosts = JsonConvert.DeserializeObject<SummonerBoosts>(boosts);

                        DateTime currentDate = DateTime.Now;

                        DateTime dailyWin = DateTime.Parse(summonerBoosts.firstWinOfTheDayStartTime);
                        DateTime xpBoostTime = DateTime.Parse(summonerBoosts.xpBoostEndDate);

                        string dailyWinStatus = dailyWin <= currentDate ? "Available!" : "Unavailable!";
                        string xpBoostStatus = xpBoostTime <= currentDate ? "No time boosts active!" : xpBoostTime.ToString();

                        boostsInfo.Text = "First win of the day: " + dailyWinStatus + "\n" +
                                           "XP boost end date: " + xpBoostStatus + "\n" +
                                           "Win game XP boosts: " + summonerBoosts.xpBoostPerWinCount;
                    }

                    try
                    {
                        var masteryData = await leagueClient.Request(requestMethod.GET, "/lol-champion-mastery/v1/local-player/champion-mastery");
                        if (masteryData != null)
                        {
                            //list of champions sorted by highest mastery
                            List<ChampionMasteryV1> championMasteries = JsonConvert.DeserializeObject<List<ChampionMasteryV1>>(masteryData);
                            ChampionMasteryV1 mainChampionMastery = championMasteries.First();
                            Champion mainChampion = new Champion();
                            mainChampion = JsonConvert.DeserializeObject<Champion>(GetChampionByKey(mainChampionMastery.championId).ToString());
                            string name = mainChampion.name;
                            string title = mainChampion.title;
                            string level = mainChampionMastery.championLevel.ToString();
                            string points = mainChampionMastery.championPoints.ToString();
                            string highestGrade = mainChampionMastery.highestGrade;
                            string mainChampionString = name + ", " + title + "\n" +
                                                    "Mastery Level: " + level + "\n" +
                                                    "Mastery Points: " + points + "\n" +
                                                    "Season's highest grade: " + highestGrade;

                            championMasteryInfo.Text = mainChampionString;


                            BitmapImage bi3 = new BitmapImage();
                            bi3.UriSource = new Uri("https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/assets/characters/" + mainChampion.id.ToLower() + "/skins/base/images/" + mainChampion.id.ToLower() + "_splash_centered_0.jpg", UriKind.Absolute);
                            mainChampionArt.Source = bi3;
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
                catch (Exception)
                {

                    throw;
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

    public static Dictionary<string, dynamic> GetLatestDDragon()
    {
        if (championsJson.Count > 0) return championsJson;

        using (HttpClient client = new HttpClient())
        {
            var versionsRequest = new HttpRequestMessage(HttpMethod.Get, "https://ddragon.leagueoflegends.com/api/versions.json");
            var versionsResponse = client.Send(versionsRequest);
            var versionsResponseString = versionsResponse.Content.ReadAsStringAsync().Result;
            //var versionsResponse = await client.GetStringAsync("https://ddragon.leagueoflegends.com/api/versions.json");
            var versions = JArray.Parse(versionsResponseString);
            var latest = versions[0].ToString();

            var ddragonRequest = new HttpRequestMessage(HttpMethod.Get, $"https://ddragon.leagueoflegends.com/cdn/{latest}/data/en_US/champion.json");
            var ddragonResponse = client.Send(ddragonRequest);
            var ddragonResponseString = ddragonResponse.Content.ReadAsStringAsync().Result;
            //var ddragonResponse = await client.GetStringAsync($"https://ddragon.leagueoflegends.com/cdn/{latest}/data/en_US/champion.json");
            var ddragon = JObject.Parse(ddragonResponseString);
            var champions = ddragon["data"].ToObject<Dictionary<string, dynamic>>();

            championsJson = champions;
            return champions;
        }
    }

    public static JObject GetChampionByKey(int key)
    {
        var champions = GetLatestDDragon();

        foreach (var champion in champions)
        {
            if (champion.Value["key"] == key)
            {
                return champion.Value;
            }
        }

        return null;
    }
}
