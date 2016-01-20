using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.SessionState;

/***
 * UserLogin.cs
 * Created on 10/24/2013
 * Last Updated 11/26/2013
 * A simple class that allows for users logging in and out
 * of the LightWriter system. Most methods are static,
 * since this is really just a helper class.
 * Uses sessions instead of cookies, so the login will 
 * "die"/end once the user closes their browser.
 ***/
namespace LightWriter.App_Code
{
    /// <summary>
    /// UserLogin functions are used/called when the client
    /// initializes a web service call via the registration, log in, or
    /// log out methods in LightWriter.
    /// </summary>
    public class UserLogin
    {
        /// <summary>
        /// IsAuthenticated is a variable that checks the current session of the user
        /// and returns true if a user is logged in and false otherwise.
        /// Modified from http://stackoverflow.com/questions/18444215/custom-user-login-with-asp-net-using-sql-and-c-sharp
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                var session = HttpContext.Current.Session;
                string username = (string)session["Username"];
                if (username == null || username.Equals(""))
                    return false;
                else return true;
            }
        }

        /// <summary>
        /// Returns UserID if a user is logged in and -1 otherwise.
        /// </summary>
        public static int UserID
        {
            get
            {
                if (IsAuthenticated)
                    return (int)HttpContext.Current.Session["UserID"];
                else return -1;
            }
        }

        /// <summary>
        /// Attempts to log a user into LightWriter. Sets the Session
        /// up with their username so that we can verify later whether
        /// any user is logged in or not.
        /// </summary>
        /// <param name="username">The username to log in with.</param>
        /// <param name="password">The password to log in with.</param>
        /// <returns>True if the username/password was valid, false otherwise.</returns>
        public static bool Login(string username, string password)
        {
            int userID = verifyUsernamePassword(username, password);
            if (userID >= 0)
            {
                HttpContext.Current.Session["Username"] = username;
                HttpContext.Current.Session["UserID"] = userID;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Logs a user out of the system.
        /// </summary>
        public static void Logout()
        {
            if (IsAuthenticated)
                HttpContext.Current.Session.Abandon();
        }

        /// <summary>
        /// This function generates a rather weak MD5 hash [key] of the user's password for
        /// storage in our Users table of the database. Borrowed from and old
        /// DES undergrad assignment.
        /// NOT CONSIDERED SECURE, although it does put it in a UInt64, which might
        /// throw crackers off a tiny tiny bit.
        /// </summary>
        /// <param name="password">The password to generate a key for.</param>
        /// <returns></returns>
        private static ulong generatePasswordKey(string password) // NOT REALLY THAT SECURE!
        {   // my algorithm based on: http://msdn.microsoft.com/en-us/library/ez5bche8.aspx and used
            // http://stackoverflow.com/questions/1139957/c-sharp-convert-integer-to-hex-and-back-again for help
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            ulong key = BitConverter.ToUInt64(data, 0);
            return key;
        }

        /// <summary>
        /// Creates a user login after checking to see whether the username
        /// exists in the database. Adds the user login to the database .
        /// </summary>
        /// <param name="username">The username to register with the system.</param>
        /// <param name="password">The password to use in the system.</param>
        /// <returns>True if successful, false otherwise (usually because the username exists).</returns>
        public static bool CreateUserLogin(string username, string password)
        {
            if (!checkUserExists(username))
            {
                LightWriterDataContext dataContext = new LightWriterDataContext();
                ulong passHash = generatePasswordKey(password);
                User user = new User();
                user.Username = username;
                user.Password = passHash.ToString();
                dataContext.Users.InsertOnSubmit(user);
                dataContext.SubmitChanges();
                Login(username, password); // Log the user in now that they're registered
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see whether a given username exists in the Users table
        /// of the database already.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>True if the username already exists, false otherwise.</returns>
        private static bool checkUserExists(string username)
        {
            LightWriterDataContext dataContext = new LightWriterDataContext();
            string usernameIfExists = dataContext.Users.Where(c => c.Username == username).Select(c => c.Username).FirstOrDefault();
            if (String.IsNullOrEmpty(usernameIfExists))
                return false;
            return true;
        }

        /// <summary>
        /// Verifies that a given username and password match what has been previously
        /// registered for the system. Does not use the given password, but hashes
        /// the password first before checking against the database.
        /// </summary>
        /// <param name="username">The username to attempt to verify.</param>
        /// <param name="password">The password for said username.</param>
        /// <returns>Some positive int (user id) if the username/password pair are valid, -1 otherwise.</returns>
        private static int verifyUsernamePassword(string username, string password)
        {
            LightWriterDataContext dataContext = new LightWriterDataContext();
            string passHash = generatePasswordKey(password).ToString();
            User userIfExists = dataContext.Users.Where(c => c.Username == username && c.Password == passHash).FirstOrDefault();
            if (userIfExists == null) // ????? not sure if this line is correct (TODO)
                return -1;            
            return userIfExists.ID;
        }
        /*
        void System.Web.IHttpHandler.ProcessRequest(System.Web.HttpContext context) {
        }
        public bool IsReusable
        {
            get { return true; }
        }*/
    }
}