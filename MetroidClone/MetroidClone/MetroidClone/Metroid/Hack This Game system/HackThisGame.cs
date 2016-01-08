using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using System.IO;
using System.Collections;

namespace MetroidClone.Metroid
{
    // The hackThisGame system makes it possible for people who watch the game to change things about the game.
    // This should make watching the game of another player more interesting.

    class HackThisGame : GameObject
    {
        const string baseURI = "http://wagtailgames.com/HiddenHorseEntertainment/";
        const string connectURI = baseURI + "connect.php"; //The URI to register the game to the system.
        const string setOptionsURI = baseURI + "set_options.php"; //The URI to set the current options of the game.
        const string getResultURI = baseURI + "get_result.php"; //The URI to get the result of options.
        const int chooseTime = 30; //How long the user has to make a choice.
        const float timeToDrawChoices = 10; //How long hack results should be drawn.

        bool isConnected; //Whether we are connected to the web service.
        bool optionsAvailable; //Whether there are options available at this moment.
        string gameID; //The game id of the current game.
        string gameToken; //The game token of the current game. This is used for commands that only the game should be able to use.

        DateTime? nextActionTime; //The time when we have to perform some action.

        List<IHackingOpportunity> hackingOpportunities;
        
        //Information about the current choice status.
        IHackingOpportunity currentFirstChoice, currentSecondChoice, winningChoice;
        int option1Votes, option2Votes;
        bool hasWinner;
        float timeUntilChoiceEnd = 0, timeLeftToDrawChoice = 0;

        public override bool ShouldDrawGUI => true;

        public HackThisGame()
        {
            //Set variables
            isConnected = false;
            optionsAvailable = false;
            nextActionTime = null;

            gameID = "";
            gameToken = "";

            hackingOpportunities = GetHackingOpportunities();

            hasWinner = false;

            Connect(); //Connect
        }

        //Draw the GUI of this system.
        public override void DrawGUI()
        {
            Color guiColor = new Color(50, 50, 50, 200);
            if (isConnected)
            {
                //Draw the game ID.
                const string basicInfo = "Game ID:";
                Drawing.DrawRectangleUnscaled(new Rectangle(10, (int)Drawing.GUISize.Y - 80, (int)Drawing.MeasureText("font14", basicInfo).X + 20, 70),
                    guiColor);
                Drawing.DrawText("font14", basicInfo, new Vector2(20, (int)Drawing.GUISize.Y - 75), Color.White);
                Drawing.DrawText("font22", gameID, new Vector2(20, (int)Drawing.GUISize.Y - 50), Color.White);
                
                if (hasWinner && timeLeftToDrawChoice > 0) //Draw the winning option.
                {
                    timeLeftToDrawChoice -= 1 / 60f;
                    //Add a simple animation.
                    float anim = 0;
                    if (timeLeftToDrawChoice > Math.Max(timeToDrawChoices, winningChoice.Time) - 0.5f)
                        anim = (0.5f - (Math.Max(timeToDrawChoices, winningChoice.Time) - timeLeftToDrawChoice)) * 2f;
                    else if (timeLeftToDrawChoice < 0.5f)
                        anim = (0.5f - timeLeftToDrawChoice) * 2f;

                    const string youHaveBeenHackedText = "This game has been hacked!";
                    string hackText = winningChoice.Text;
                    int centerX = (int)Drawing.GUISize.X / 2;
                    int width = Math.Max((int) Drawing.MeasureText("font14", youHaveBeenHackedText).X, (int) Drawing.MeasureText("font18", hackText).X);
                    Drawing.DrawRectangleUnscaled(new Rectangle(centerX - width / 2 - 10, (int)Drawing.GUISize.Y - 75 + (int) (75 * anim),
                        width + 20, 65), guiColor);
                    Drawing.DrawText("font14", youHaveBeenHackedText, new Vector2(centerX, Drawing.GUISize.Y - 70 + 75 * anim), Color.White,
                        alignment: Engine.Asset.Font.Alignment.TopCenter);
                    Drawing.DrawText("font18", hackText, new Vector2(centerX, Drawing.GUISize.Y - 45 + 75 * anim), Color.White,
                        alignment: Engine.Asset.Font.Alignment.TopCenter);
                }
            }
        }

        //Update the system.
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isConnected && nextActionTime?.CompareTo(DateTime.Now) <= 0)
            {
                nextActionTime = null;
                if (optionsAvailable)
                    GetOptionResult();
                else
                {
                    //Get the new hacking opportunities.
                    IEnumerable possibleHackingOpportunities = hackingOpportunities.Where(h => h.CanUse(World)); //Get the possible opportunities first.
                    currentFirstChoice = hackingOpportunities.GetRandomItem();
                    currentSecondChoice = hackingOpportunities.Where(h => h != currentFirstChoice).ToList().GetRandomItem();
                    SetOptions(currentFirstChoice.Text, currentSecondChoice.Text);
                }
            }

            //Stop any current hacks if needed.
            if (isConnected && hasWinner && timeUntilChoiceEnd > 0)
            {
                timeUntilChoiceEnd -= 1 / 60f;

                if (timeUntilChoiceEnd <= 0 && winningChoice.IsTimeBased)
                    winningChoice.Stop(World);
            }
        }

        //Connect the game to the web services.
        void Connect()
        {
            HTTPGet(connectURI, OnConnectDone);
        }

        //Set new options
        void SetOptions(string option1, string option2)
        {
            if (isConnected)
            {
                HTTPGet(setOptionsURI + $"?gameid={gameID}&token={gameToken}&option1={option1}&option2={option2}&time={chooseTime}", OnSetOptionsDone);
            }
        }

        //Get option results
        void GetOptionResult()
        {
            if (isConnected && optionsAvailable)
            {
                HTTPGet(getResultURI + $"?gameid={gameID}", OnGetOptionsDone);
            }
        }

        //Get data from an URI. Call the callback when we're done.
        void HTTPGet(string uri, OpenReadCompletedEventHandler callback)
        {
            WebClient downloader = new WebClient();
            downloader.OpenReadCompleted += callback;
            downloader.OpenReadAsync(new Uri(uri));
        }

        //A callback event for when we're connected.
        void OnConnectDone(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //Okay, we've set new options! Get the response.
                Dictionary<string, string> response = ReadKeyPair(new StreamReader(e.Result).ReadToEnd());

                if (response["SUCCESS"] == "true")
                {
                    isConnected = true;

                    gameID = response["GameID"];
                    gameToken = response["Token"];

                    nextActionTime = DateTime.Now.AddSeconds(4);
                }
            }
        }

        //A callback event for when we've set options.
        void OnSetOptionsDone(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //Okay, we're connected to the server! Get the response.
                Dictionary<string, string> response = ReadKeyPair(new StreamReader(e.Result).ReadToEnd());

                if (response["SUCCESS"] == "true")
                {
                    //Success!
                    nextActionTime = DateTime.Now.AddSeconds(chooseTime); //We should take the next action in 60 seconds.
                    optionsAvailable = true;
                }
            }
        }

        //A callback event for when we've got options.
        void OnGetOptionsDone(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //Okay, we're connected to the server! Get the response.
                Dictionary<string, string> response = ReadKeyPair(new StreamReader(e.Result).ReadToEnd());

                if (response["SUCCESS"] == "true")
                {
                    //Success!

                    //Now determine the winner, if there were any votes.
                    option1Votes = int.Parse(response["option1votes"]);
                    option2Votes = int.Parse(response["option2votes"]);

                    if (option1Votes > 0 || option2Votes > 0 && timeUntilChoiceEnd <= 0)
                    {
                        //The winner is the option that has been hacked the most.
                        if (option1Votes > option2Votes)
                            winningChoice = currentFirstChoice;
                        else if (option2Votes > option1Votes)
                            winningChoice = currentSecondChoice;
                        else //Choose a random winner.
                            winningChoice = new List<IHackingOpportunity> { currentFirstChoice, currentSecondChoice }.GetRandomItem();
                        hasWinner = true;
                        winningChoice.Use(World);
                        timeUntilChoiceEnd = winningChoice.Time;
                        timeLeftToDrawChoice = Math.Max(timeToDrawChoices, winningChoice.Time);
                    }

                    nextActionTime = DateTime.Now.AddSeconds(2); //We should take the next action in 2 seconds.
                    optionsAvailable = false;
                }
            }
        }

        //Helper function to create a dictionary from the keypair format.
        Dictionary<string, string> ReadKeyPair(string keyPairString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] keyPairs = keyPairString.Split('\n');
            if (keyPairs[0] == "SUCCESS")
            {
                result["SUCCESS"] = "true";
                //The rest of the lines contain key pair data.
                for (int i = 1; i < keyPairs.Length; i++)
                {
                    string keyPair = keyPairs[i];
                    int equalsSignIndex = keyPair.IndexOf("=");
                    if (equalsSignIndex != -1)
                    {
                        result[keyPair.Substring(0, equalsSignIndex)] = keyPair.Substring(equalsSignIndex + 1);
                    }
                }
            }
            else
            {
                //Something went wrong!
                if (keyPairs.Length > 1)
                    Console.WriteLine(keyPairs[1]);
                result["SUCCESS"] = "false";
            }
            return result;
        }

        //Get all hacking opportunities
        List<IHackingOpportunity> GetHackingOpportunities()
        {
            Type hackingOpportunityType = typeof(IHackingOpportunity);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => hackingOpportunityType.IsAssignableFrom(p) && p != hackingOpportunityType)
                .Select(t => (IHackingOpportunity) Activator.CreateInstance(t))
                .ToList();
        }
    }
}
