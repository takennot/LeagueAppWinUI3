using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueAppWinUI.Models;
public class SummonerProfile
{
    public long? summonerId { get; set; }
    public long? accountId { get; set; }
    public string? displayName { get; set; }
    public string? internalName { get; set; }
    public long? profileIconId { get; set; }
    public long? summonerLevel { get; set; }
    public long? percentCompleteForNextLevel { get; set; }
    public string? puuid { get; set; }
    public string? gameName { get; set; }
    public string? tagLine { get; set; }

    public SummonerProfile() 
    {
    
    }
}
