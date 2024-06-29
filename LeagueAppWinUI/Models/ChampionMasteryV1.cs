using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MingweiSamuel.Camille.Enums;

namespace LeagueAppWinUI.Models;
internal class ChampionMasteryV1
{
    public int championId { get; set; }
    public int championLevel { get; set; }
    public int championPoints { get; set; }
    public long lastPlayTime { get; set; }
    public string? highestGrade { get; set; }

    public ChampionMasteryV1()
    {
    
    }
}
