using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueAppWinUI.Models;
internal class SummonerBoosts
{
    public string? xpBoostEndDate { get; set; }
    public int? xpBoostPerWinCount { get; set; }
    public int? xpLoyaltyBoost { get; set; }
    public string? firstWinOfTheDayStartTime { get; set; }

    public SummonerBoosts() { }
}
