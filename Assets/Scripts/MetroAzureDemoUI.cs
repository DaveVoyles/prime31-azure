/* You can view this layout inside of the Unity Editor, but you 
 * CANNOT connect to azure unless you BUILD, then run the application from Visual Studio.
 * Tutorial: http://davevoyles.azurewebsites.net/prime31-azure-plugin-win8-wp8-unity-games/
 * 
 * @Author: Dave Voyles, Microsoft Corporation 2014
 */

using Assets.Plugins.MetroAzure;
using UnityEngine;
using System.Collections.Generic;
using Prime31.MetroAzure;
using Prime31;

public class MetroAzureDemoUI : MonoBehaviourGUI
{
    private Vector2               _scrollPosition;
    private LeaderBoard           _leaderBoardItem    = new LeaderBoard() {username = "", Id = null, score = 0};
    private List<LeaderBoard>     _leadersList        = new List<LeaderBoard>();
    private int                   _minScoreToReturn   = 100;
    private int                   _columnWidth        = 300;


    /* 
     * You can find both of these properties in your Azure Portal: https://manage.windowsazure.com
     * I serialized them, to expose them to the editor.
     * I chose not to hard code them, so that I can edit them within the Unity editor.
     */
    [SerializeField]
    private string _azureEndPoint  = "<your leaderboard service>";
    [SerializeField]
    private string _applicationKey = "<your application key>";


    /// <summary>
    /// All of our GUI drawing is handled here
    /// </summary>
    private void OnGUI()
    {
        // Container to hold the UI
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

            // First column
            DrawEndPointAndAppKeyBtns();

            // Second column
            DrawInsertBtn();

            // Third column
            DrawListAllItemsBtn();
            ColumnForLeaderboardList();

            // Fourth column
            DrawQueryScoresBtn();

        // End UI Container
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }



    // =========================================================================================================
    //                                          Buttons for layout
    // =========================================================================================================


    /// <summary>
    /// Enter your endpoint web address and the application key 
    /// </summary>
    private void DrawEndPointAndAppKeyBtns()
    {
        GUILayout.BeginVertical(GUILayout.Width(_columnWidth));

            // Drawn Azure end point Input field
            GUILayout.Label("AZURE END POINT");
            _azureEndPoint  = GUILayout.TextField(_azureEndPoint, GUILayout.Width(_columnWidth));

            // Draw Application key Input field
            GUILayout.Label("APPLICATION KEY");
            _applicationKey = GUILayout.TextField(_applicationKey, GUILayout.Width(_columnWidth));
        
            ConnectToAzureMobileServiceBtn();

        GUILayout.EndVertical();
    }


    /// <summary>
    /// Uses Azure.Connect from prime[31] to and passes in endPoint & AppKey to connect to AMS
    /// </summary>
    private void ConnectToAzureMobileServiceBtn()
    {
        if (!GUILayout.Button("CONNECT TO AZURE SERVICE")) return;

        Azure.connect(_azureEndPoint, _applicationKey);
        Debug.Log("...Connecting to Azure Mobile Service. Endpoint:" + " " + _azureEndPoint + " " + "_appKey:" + "" + _applicationKey);
    }


    /// <summary>
    /// Draws the inputs for username and Score, as well as the Insert LeaderBoard Button
    /// </summary>
    private void DrawInsertBtn()
    {
        GUILayout.BeginVertical(GUILayout.Width(_columnWidth));
            
            // Draw "USER NAME button
            GUILayout.Label("USER NAME");
            _leaderBoardItem.username = GUILayout.TextField(_leaderBoardItem.username);
            
            // Draw "SCORE" button
            GUILayout.Label("SCORE");
            _leaderBoardItem.score    = int.Parse(GUILayout.TextField(_leaderBoardItem.score.ToString()));

            if (GUILayout.Button("INSERT TO LEADERBOARD"))
            {
                Azure.insert(_leaderBoardItem, () => Debug.Log("inserted" + " " + _leaderBoardItem.username + " " + "to leaderboard"));
            }

        GUILayout.EndVertical();
    }


    /// <summary>
    /// Draws all of the items in the leaderboard
    /// </summary>
    private void DrawListAllItemsBtn()
    {
        GUILayout.BeginVertical(GUILayout.Width(_columnWidth));

            GUILayout.Label("Connect to Azure, hit these buttons, then WAIT a few moments for results to return");

            // Draw the button to get ALL of the items in the leaderboard
            if (!GUILayout.Button("List ALL Items In Leaderboard")) return;

            // Remove everything currently in the list
            _leadersList.Clear();

            // Get all of the items currently stored on the leaderboard
            Azure.where<LeaderBoard>(i => i.username != null, itemsInTheLeaderboard =>
            {
                Debug.Log("queried ALL scores, completed with _leadersList count:" + " " + itemsInTheLeaderboard.Count);
                _leadersList = itemsInTheLeaderboard;

                // Loop through each item in the leaderboard list, and draw it to the log
                foreach (var item in itemsInTheLeaderboard)
                    Debug.Log("Item in the leaderboard:" + " " + item);
            });

            // All items returned from the leaderboard are drawn in this list container
            GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Height(_columnWidth));
                ColumnForLeaderboardList();
            GUILayout.EndScrollView();

        GUILayout.EndVertical();
    }


    /// <summary>
    /// All items returned from the leaderboard are drawn in this list. Called by DrawColumnList()
    /// </summary>
    private void ColumnForLeaderboardList()
    {
        GUILayout.BeginVertical();

            // Loop through each item in the leaderboard
            foreach (var item in _leadersList)
            {
                // BUG: Sometimes this throws a random error. You only need to re-launch the project 
                GUILayout.BeginHorizontal();

                  // Use GUILayout.Label to draw text to the screen in Unity, rather than to the VS console
                  GUILayout.Label(string.Format("Name: {0} Score: {1}", item.username, item.score));

                GUILayout.EndHorizontal();
            }

            // Only appear if we have something in our leaderboard
            UpdateAndDeleteBtns();

        GUILayout.EndVertical();
    }


    /// <summary>
    /// NOTE: We need to query the database FIRST and pull in results, then we can make changes to it 
    /// </summary>
    private void UpdateAndDeleteBtns()
    {
        // Have you tried to connect to Azure first? Is there anything in our leaderboard?
        //if (_leadersList != null || _leadersList.Count > 0)
        if (_leadersList.Count > 0) 

        {
            // REMOVE the first (latest) thing in the leaderboard (then delete it later)
            if (GUILayout.Button("DELETE LATEST ITEM"))
            {
                // Grab the first item in the list
                var leaderToRemove = _leadersList[0];

                // Removes item at the specified index.
                _leadersList.RemoveAt(0);

                // DELETE it from the leaderboard
                Azure.delete(leaderToRemove,
                    () => Debug.Log("Deleted latest item from leaderboard:" + " " + leaderToRemove.username));
            }

            // UPDATE the first (latest) thing in the leaderboard
            if (GUILayout.Button("Update latest Item"))
            {
                // Grab the first item in the leaderboard list
                var leaderToUpdate      = _leadersList[0];

                // Set the item's username to what we entered in the username input field
                leaderToUpdate.username = GUILayout.TextField(_leaderBoardItem.username);

                // Update the item in the leaderboard
                Azure.update(leaderToUpdate, () => Debug.Log("Updated leaderboard item:" + " " + leaderToUpdate.username));
            }
       }
    }


    /// <summary>
    /// Grabs all of the scores in the database, based on the search results you enter
    /// </summary>
    private void DrawQueryScoresBtn()
    {
        GUILayout.BeginVertical(GUILayout.Width(_columnWidth));
            if (GUILayout.Button("Query All Scores <= 200"))
            {
                // Remove everything from the current log
                _leadersList.Clear();

                // Grab all scores in our leaderboard which are <= _minScoreToReturn
                Azure.where<LeaderBoard>(i => i.score <= _minScoreToReturn, itemsInTheLeaderboard =>
                {
                    Debug.Log("queried all scores <= 100 has completed with _leadersList count: " + " " + itemsInTheLeaderboard.Count);
                    _leadersList = itemsInTheLeaderboard;

                    // Loop through each item in the leaderboard list, and draw it to the log
                    foreach (var item in itemsInTheLeaderboard)
                    {
                        GUILayout.Label(string.Format("Name: {0} Score: {1}", item.username, item.score));
                    }
                });
            }
            GUILayout.EndVertical();
    }
}
