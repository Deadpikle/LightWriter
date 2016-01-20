using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Helpers;

using LightWriter.App_Code;

/***
 * AjaxServices.asmx.cs
 * Created on 10/27/2013
 * Last Updated 11/24/2013
 * A web service file for the LightWriter application that allows
 * for Ajax calls from the JavaScript to C# code.
 * Generally used for log out/in services and saving and loading
 * of algorithms.
 * Only 1 file is used since we won't have all that many service
 * calls.
 * Most functions need to see whether the user is logged in before
 * performing actions.
 ***/
namespace LightWriter
{
    [WebService(Namespace = "")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService] // allows for Ajax calls
    public class AjaxServices : System.Web.Services.WebService
    {
        /// <summary>
        /// RegisterUser registers a client with the given username and
        /// password. Does NOT log them into the system.
        /// </summary>
        /// <param name="username">The username they wish to register with.</param>
        /// <param name="password">The password that the user would like to use.</param>
        /// <returns>"Success" if registration was successful, error message if not.</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RegisterUser(string username, string password)
        {
            bool wasUserCreationSuccessful = UserLogin.CreateUserLogin(username, password);
            if (wasUserCreationSuccessful)
                return "Success";
            else return "Couldn't create user: Username already exists.";
        }

        /// <summary>
        /// LoginUser logs a user into the system with a given username and password.
        /// </summary>
        /// <param name="username">The client's username.</param>
        /// <param name="password">The client's password.</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LoginUser(string username, string password)
        {
            bool wasLoginSuccessful = UserLogin.Login(username, password);
            if (wasLoginSuccessful)
                return "Success";
            else return "Failure";
        }

        /// <summary>
        /// LogoutUser logs a user out of the LightWriter system.
        /// </summary>
        /// <returns>"Success (it should never fail!)"</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LogoutUser()
        {
            UserLogin.Logout();
            return "Success";
        }

        /// <summary>
        /// CheckUserSession() checks to see if a user is currently logged
        /// into the system.
        /// </summary>
        /// <returns>If a user is logged in, returns "Inactive", else returns "Active"</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CheckUserSession()
        {
            string username = (string)HttpContext.Current.Session["Username"];
            if (username == null)
                return "Inactive";
            else return "Active";
        }

        /// <summary>
        /// Saves a user algorithm from a JSON string sent to the server
        /// by a logged-in user.
        /// </summary>
        /// <param name="blockListJson">The JSON string for the block list algorithm</param>
        /// <param name="algorithmName">Name to save algorithm under</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Dictionary<string, object> SaveUserAlgorithm(string blockListJson, string algorithmName, string rulesJson)
        {
            // first verify logged in user, then if logged in save their algorithm
            if (UserLogin.IsAuthenticated)
                return BlockListSaverLoader.SaveUserAlgorithm(UserLogin.UserID, algorithmName, blockListJson, rulesJson);
            else return 
                    new Dictionary<string, object>() {
                        { "LoginError", "You have to log in before saving an algorithm." }
                    };
        }

        /// <summary>
        /// Loads all of the algorithm names and IDs for a logged in user.
        /// </summary>
        /// <returns>Returns a list of algorithm names with IDs if the user is logged in, or
        /// returns an empty string array if the user isn't logged in at all (or no algorithm names
        /// exist)</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Array LoadUserAlgorithmNames()
        {
            if (UserLogin.IsAuthenticated)
                return BlockListSaverLoader.LoadUserAlgorithmNames(UserLogin.UserID);
            else return new string[0];
        }

        /// <summary>
        /// Deletes a user algorithm with a specific id.
        /// </summary>
        /// <param name="id">ID of the algorithm to delete.</param>
        /// <returns>"Success" if it successfully deleted the algorithm, and an error message otherwise
        /// </returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteUserAlgorithm(int id)
        {
            if (UserLogin.IsAuthenticated)
                return BlockListSaverLoader.DeleteAlgorithmByName(id);
            else return "Failure: User not logged in.";
        }

        /// <summary>
        /// Loads a user algorithm into a string, object dictionary (the
        /// javascript just sees it as an object essentially)
        /// </summary>
        /// <param name="algorithmID">The ID of the algorithm to load</param>
        /// <returns>A dictionary full of blocks of the loaded algorithm if sent
        /// a valid ID and a user is logged in, or an empty dictionary if
        /// not successful</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Dictionary<string, object> LoadUserAlgorithm(int algorithmID)
        {
            if (UserLogin.IsAuthenticated)
                return BlockListSaverLoader.LoadUserAlgorithm(algorithmID);
            else return new Dictionary<string,object>();
        }

        /// <summary>
        /// Loads a user algorithm into a string, object dictionary (the
        /// javascript just sees it as an object essentially).
        /// No registration required; used for sharing.
        /// URL must contain sharing parameter (see code) in order to 'prove' that this service
        /// is being used for sharing.
        /// </summary>
        /// <param name="algorithmID">The ID of the algorithm to load</param>
        /// <returns>A dictionary full of blocks of the loaded algorithm if sent
        /// a valid ID and a user is logged in, or an empty dictionary if
        /// not successful OR the URL isn't meant for sharing</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Dictionary<string, object> LoadUserAlgorithmForSharing(int algorithmID, string URL)
        {
            if (URL.Contains("?shareID="))
                return BlockListSaverLoader.LoadUserAlgorithm(algorithmID);
            else
            {
                Dictionary<string, object> errorDictionary = new Dictionary<string, object>() {
                    { "Error", "URL is invalid for sharing."}
                };
                return errorDictionary;
            }
        }
        /* Alternate method that receives the full blocklist (not fully working, but gives another idea)
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveUserAlgorithmy(Dictionary<string, object> algorithm)
        {
            Dictionary<string, object> blocklist = (Dictionary<string, object>)algorithm["blockList"];
            //var blocky = blocklist[0];
            foreach (KeyValuePair<string, object> kvp in blocklist)
            {
                Console.WriteLine("Key = {0}, Value = {1}",
                    kvp.Key, kvp.Value);
            }
            return "";
        }
        */

    }
}
