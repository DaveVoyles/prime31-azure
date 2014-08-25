/*
 * A container for the things you insert into your leaderboard. 
 * It simply holds a name, score, and unique ID for each object you insert into the board.
 * Tutorial: http://davevoyles.azurewebsites.net/prime31-azure-plugin-win8-wp8-unity-games/
 * 
 * @Author: Dave Voyles, Microsoft Corporation 2014
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Assets.Plugins.MetroAzure
{
    public class LeaderBoard
    {
        public string Id       { get; set; }
        public string username { get; set; }
        public int    score    { get; set; }
    }
}
