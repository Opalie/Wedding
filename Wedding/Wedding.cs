using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile;

namespace WeddingMod
{
    /// <summary>The main entry point for the mod.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The path to the wedding map file, relative to the mod folder.</summary>
        private string MapPath = "assets/wedding.tbin";

        /// <summary>The player's tile position in the wedding map.</summary>
        private readonly Vector2 PlayerPixels = new Vector2(1732, 4052);

        /// <summary>The question before the wedding.</summary>

        public void setup(object sender, DayStartedEventArgs e)
        {
            if (Game1.weddingToday)
            {
                Game1.player.currentLocation.createQuestionDialogue(
                            this.Helper.Translation.Get("Wedding_Question"),
                new[]
                {
                    new Response("On the Beach.",this.Helper.Translation.Get("Wedding_Beach")),
                    new Response("In the Forest, near the river.",this.Helper.Translation.Get("Wedding_Forest")),
                    new Response("In the Deep Woods.",this.Helper.Translation.Get("Wedding_Wood")),
                    new Response("Where the Flower Dance takes place!",this.Helper.Translation.Get("Wedding_Flower"))
                }, (farmer, answerKey) =>
                {
                    switch (answerKey)
                    {
                        case "On the Beach.":
                            MapPath = "assets/WeddingBeach.tbin";
                            // Load Beach
                            break;
                        case "In the Forest, near the river.":
                            MapPath = "assets/WeddingForest.tbin";
                            // Load Forest
                            break;
                        case "In the Deep Woods.":
                            MapPath = "assets/WeddingWoods.tbin";
                            // Load Wood
                            break;
                        case "Where the Flower Dance takes place!":
                            MapPath = "assets/WeddingFlower.tbin";

                            // Load Flower Dance
                            break;
                    }
                }
                );
            }
        }


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += Player_Warped;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (Game1.weddingToday && e.NewLocation.Name == "Town" && e.NewLocation.currentEvent != null)
            {
                this.Monitor.Log("The wedding event has started!");

                // create temporary location
                this.Helper.Content.Load<Map>(this.MapPath); // make sure map is loaded before game accesses it
                string mapKey = this.Helper.Content.GetActualAssetKey(this.MapPath);
                var newLocation = new GameLocation(mapKey, "Temp");

                // move everything to new location
                var oldLocation = Game1.currentLocation;
                Game1.player.currentLocation = Game1.currentLocation = newLocation;
                newLocation.resetForPlayerEntry();
                newLocation.currentEvent = oldLocation.currentEvent;
                newLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);

                // move player position within map (if needed)
                Game1.player.Position = this.PlayerPixels;
            }
        }
    }
}

