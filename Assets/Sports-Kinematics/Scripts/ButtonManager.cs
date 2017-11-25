using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SportsKinematics.UI
{
    /// <summary>
    /// Manages all buttons in GUI
    /// </summary>
    public class ButtonManager : MonoBehaviour
    {
        /// <summary>
        /// Name of level to load
        /// </summary>
        public string m_level;

        /// <summary>
        /// Popup that has been drawn
        /// </summary>
        private GameObject m_Popup;

        /// <summary>
        /// Function to be called on popup end
        /// </summary>
        private Func<int> m_func;

        /// <summary>
        /// canvas to be enabled or disabled
        /// </summary>
        private string m_canvas;

        /// <summary>
        /// thread to create folders on login or user creation
        /// </summary>
        private Thread t_folders;

        /// <summary>
        /// start GameObject. Initialise m_func function as NoFunc
        /// </summary>
        void Start()
        {
            m_func = NoFunc;
        }

        /// <summary>
        /// Load level that has been selected
        /// </summary>
        public void LoadLevel()
        {
            SceneManager.LoadSceneAsync(m_level);
        }

        /// <summary>
        /// Delete Player Pref key at [field]
        /// </summary>
        /// <param name="field">key to delete</param>
        public void ClearPlayerPrefs(string field)
        {
            PlayerPrefs.DeleteKey(field);
        }

        /// <summary>
        /// Create popup and track its function
        /// </summary>
        /// <param name="popup">popup drawn</param>
        /// <param name="myFunc">function to call after destroying popup</param>
        private void CreatePopup(GameObject popup, Func<int> myFunc)
        {
            GameObject.Find(m_canvas).GetComponent<CanvasGroup>().blocksRaycasts = false;

            m_func = myFunc;

            m_Popup = Instantiate(popup) as GameObject;
            Button b = GetComponent<Button>();

            m_Popup.transform.Find("Popup").GetComponent<Popup>().CallingButton = b;
        }

        /// <summary>
        /// Create popup with no function
        /// </summary>
        /// <param name="popup">popup to create</param>
        private void CreatePopup(GameObject popup)
        {
            m_func = NoFunc;

            GameObject.Find(m_canvas).GetComponent<CanvasGroup>().blocksRaycasts = false;

            m_Popup = Instantiate(popup) as GameObject;
            Button b = GetComponent<Button>();

            m_Popup.transform.Find("Popup").GetComponent<Popup>().CallingButton = b;
        }

        /// <summary>
        /// Destroy popup and call its function
        /// </summary>
        /// <param name="result">result of popup interactions</param>
        public void DestroyPopup(Popup.DialogResults result)
        {
            Destroy(m_Popup);

            GameObject.Find(m_canvas).GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (result == Popup.DialogResults.Yes)
            {
                if (m_func != NoFunc)
                    m_func();
            }
        }

        /// <summary>
        /// Confirm close function for popup
        /// </summary>
        /// <returns>0 (unused)</returns>
        public int ConfirmClose()
        {
            GameObject.Find("Simulation/PlaylistBuilder").GetComponent<PlaylistBuilder>().PurgePlaylistBuilder();
            GetComponent<SwitchCanvasOnClick>().Switch();

            return 0;
        }

        /// <summary>
        /// Attenot to close playlist builder 
        /// </summary>
        /// <param name="popup">0 (unused)</param>
        public void AttemptClose(GameObject popup)
        {
            m_canvas = "Simulation/PlaylistBuilderCanvas";

            CreatePopup(popup, ConfirmClose);
        }

        /// <summary>
        /// Load Main Menu
        /// </summary>
        /// <returns>0 (unused)</returns>
        private int LoadMainMenu()
        {
            m_level = "MainMenu";
            LoadLevel();

            return 0;
        }

        /// <summary>
        /// Load Main Menu from the Record Scene. Discard logged data
        /// </summary>
        /// <returns>0 (unused)</returns>
        private int LoadMainMenuFromRecord()
        {
            GameObject.Find("GUI/DiscardButton").GetComponent<GUIRecordScript>().DiscardTaskOnClick();
            LoadMainMenu();

            return 0;
        }

        /// <summary>
        /// Clikc handler for Main Menu button. Creates popup
        /// </summary>
        /// <param name="popup">popup to generate</param>
        public void MainMenuOnClick(GameObject popup)
        {
            m_canvas = "GUI";

            string currentScene = SceneManager.GetActiveScene().name;
            switch (name)
            {
                case "RecordMainMenuButton":
                    if (PlayerPrefs.GetInt("RecPopup") != 0)
                    {
                        popup.GetComponentInChildren<Text>().text = "Any unsaved data will be lost. Are you sure?";
                        CreatePopup(popup, LoadMainMenuFromRecord);
                    }
                    else
                        LoadMainMenuFromRecord();
                    break;
                case "EditingMainMenuButton":
                    if (PlayerPrefs.GetInt("EditPopup") != 0)
                    {
                        popup.GetComponentInChildren<Text>().text = "Are you sure?";
                        CreatePopup(popup, LoadMainMenu);
                    }
                    LoadMainMenu();
                    break;
                case "SimulationMainMenuButton":
                    if (PlayerPrefs.GetInt("SimPopup") != 0)
                    {
                        popup.GetComponentInChildren<Text>().text = "Are you sure?";
                        CreatePopup(popup, LoadMainMenu);
                    }
                    LoadMainMenu();
                    break;
            }
        }

        /// <summary>
        /// finalise quit
        /// </summary>
        /// <returns>0 (unused)</returns>
        public int ConfirmQuit()
        {
            Application.Quit();

            return 0;
        }

        /// <summary>
        /// Allow attempt to quit
        /// </summary>
        /// <param name="popup">popup to create</param>
        public void AttemptQuit(GameObject popup)
        {
            m_canvas = "Login/LoginSelectionCanvas";
            popup.GetComponentInChildren<Text>().text = "Are you sure?";
            CreatePopup(popup, ConfirmQuit);
        }

        /// <summary>
        /// Determine if enterred value is an email address
        /// </summary>
        /// <param name="attempt">attempt that has been made</param>
        /// <returns>true if email address</returns>
        private bool isEmail(string attempt)
        {
            return (attempt.Contains("@") && attempt.Contains("."));
        }

        /// <summary>
        /// determine if value is not empty
        /// </summary>
        /// <param name="attempt">string input</param>
        /// <returns>true if valid</returns>
        private bool Valid(string attempt)
        {
            return !(attempt.Equals(""));
        }

        /// <summary>
        /// Determines if string input matches actual information
        /// </summary>
        /// <param name="actual">recorded string</param>
        /// <param name="attempt">attempt to match</param>
        /// <returns>true if matching</returns>
        private bool Match(string actual, string attempt)
        {
            return actual.Equals(attempt);
        }

        /// <summary>
        /// Empty function used as default popup call back
        /// </summary>
        /// <returns>-1</returns>
        private int NoFunc()
        {
            return -1;
        }

        /// <summary>
        /// Determine is Directory input exists
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <returns>true if existing</returns>
        private bool DirExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// If Directory does not exist, create
        /// </summary>
        /// <param name="path">path to directory</param>
        private void Create(string path)
        {
            if (!DirExists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Creates a random password
        /// </summary>
        /// <returns>randomly generated password</returns>
        private string GeneratePassword()
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string new_password = "";

            for (int i = 0; i < 8; i++)
            {
                new_password += chars[UnityEngine.Random.Range(0, chars.Length)];
            }

            return new_password;
        }

        /// <summary>
        /// Creates all missing subdirectories
        /// </summary>
        /// <param name="path">path to base folder layer</param>
        /// <param name="user">user to attach folders to</param>
        public void CreateSubDirectories(string path, string user)
        {
            string head = path + "/../Users/" + user;
            string actions = head + "/Actions/";
            string actionData = head + "/ActionData/";
            string playlists = head + "/Playlists/";
            string newConfigurations = head + "/Configurations/newConfigurations/";

            Create(head);
            Create(actions + "/Edited/");
            Create(actions + "/Unedited/");
            Create(actionData + "/Edited/");
            Create(actionData + "/Unedited/");
            Create(playlists);
            Create(head + "/Configurations/");
            Create(head + "/Reports/");

            if (Directory.Exists(newConfigurations))
                Directory.Delete(newConfigurations, true);
        }

        /// <summary>
        /// Allow user ot login to scene
        /// </summary>
        /// <param name="u"></param>
        public void Login(User u)
        {
            PlayerPrefs.SetString("Username", u.Username);

            SceneManager.LoadSceneAsync("MainMenu");
        }

        /// <summary>
        /// Make a user in scene
        /// </summary>
        /// <param name="Popup">Popup to generate</param>
        public void TaskOnClickMakeUser(GameObject Popup)
        {
            m_canvas = "Login/NewUserCanvas/";
            string details = m_canvas + "UserDetails/";
            string user = GameObject.Find(details + "Username/UsernameInput").GetComponent<InputField>().text;
            string email = GameObject.Find(details + "EmailAddress/EmailInput/").GetComponent<InputField>().text;
            string pass = GameObject.Find(details + "Password/PasswordInput").GetComponent<InputField>().text;

            string path = Application.dataPath;

            bool userValid = Valid(user);
            bool emailValid = (Valid(email) && isEmail(email));
            bool passValid = Valid(pass);

            if (userValid && passValid && emailValid)
            {
                //if (!(File.Exists(path + "/../Users/" + user + "/" + user + ".shri")))
                if (!Server.Database.UserExist(user))
                {
                    t_folders = new Thread(() => CreateSubDirectories(path, user));
                    if (!t_folders.IsAlive)
                        t_folders.Start();

                    User u = new User(user, email, pass);

                    GameObject.Find("UserManager").GetComponent<UserManager>().Save(u);

                    Login(u);
                }
                else
                {
                    Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! User already exists!";
                    CreatePopup(Popup);
                    return;
                }
            }
            else
            {
                if (!userValid)
                {
                    Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! Username is empty!";
                    CreatePopup(Popup);
                    return;
                }

                if (!passValid)
                {
                    Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! Password is empty!";
                    CreatePopup(Popup);
                    return;
                }
                if (!emailValid)
                {
                    Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! Email is empty or invalid!";
                    CreatePopup(Popup);
                    return;
                }
            }
        }

        /// <summary>
        /// Allow attempt at login
        /// </summary>
        /// <param name="Popup">Popup to generate</param>
        public void TaskOnClickLogin(GameObject Popup)
        {
            m_canvas = "Login/ExistingUserCanvas/";

            InputField userInp = GameObject.Find("LoginDetails/EmailAddress/EmailInput").GetComponent<InputField>();
            InputField passInp = GameObject.Find("LoginDetails/Password/PasswordInput").GetComponent<InputField>();

            string username = userInp.text.ToLower();
            string pass = passInp.text;

            string path = Application.dataPath;

            //string fn = Application.dataPath + "/../Users/" + username + "/" + username + ".shri";

            bool remember = GameObject.Find(m_canvas + "RememberToggle").GetComponent<Toggle>().isOn;

            bool userValid = Valid(username);
            bool passValid = Valid(pass);
            
            if (userValid && passValid)
            {
                //if (File.Exists(fn))
                if (Server.Database.UserExist(username))
                {
                    //User user = GameObject.Find("UserManager").GetComponent<UserManager>().Load(username);
                    User user = Server.Database.SelectUser(username);

                    if (Match(pass.GetHashCode().ToString(), user.Password))
                    {
                        t_folders = new Thread(() => CreateSubDirectories(path, username));
                        if (!t_folders.IsAlive)
                            t_folders.Start();

                        passInp.text = "";

                        if (remember)
                        {
                            PlayerPrefs.SetString("RememberEmailAddress", username);
                        }
                        else
                        {
                            userInp.text = "";
                        }

                        PlayerPrefs.SetString("EmailAddress", user.Email);
                        PlayerPrefs.SetString("ReportEmailAddress", user.Email);
                        Login(user);
                    }
                    else
                        if (!Match(pass, user.Password))
                    {
                        Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! Password incorrect!";
                        CreatePopup(Popup);
                        passInp.text = "";
                    }
                }
                else
                {
                    Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! User does not exist!";
                    CreatePopup(Popup);
                    userInp.text = "";
                    passInp.text = "";
                }
            }
            else
            {
                if (!userValid)
                {
                    Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! Username is empty!";
                    CreatePopup(Popup);
                    return;
                }
                if (!passValid)
                {
                    Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! Password is empty!";
                    CreatePopup(Popup);
                    return;
                }
            }
        }

        /// <summary>
        /// Go to main menu
        /// </summary>
        /// <returns>0 (unused)</returns>
        private int ToMain()
        {
            GetComponent<SwitchCanvasOnClick>().Switch();

            return 0;
        }

        /// <summary>
        /// Allow user to find their password
        /// </summary>
        /// <param name="Popup"></param>
        public void TaskOnClickFindPassword(GameObject Popup)
        {
            m_canvas = "Login/LostPasswordCanvas/";

            MailMessage mail = new MailMessage();

            string u_newPassword = GeneratePassword();

            string user = GameObject.Find(m_canvas + "Email address/EmailField").GetComponent<InputField>().text.ToLower();
            //print("User: " + user + " New password: " + u_newPassword);

            bool valid = Valid(user);

            if (!valid)
            {
                Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! Username is empty!";
                CreatePopup(Popup);
                return;
            }

            if (!Connectivity.CheckConnection())
            {
                Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Error! Internet connection is needed!";
                CreatePopup(Popup);
                return;
            }

           
            mail.From = new MailAddress("creative.solutions.murdoch@outlook.com");
            mail.Body = "A Point Light user tried to use our \"Forgotten Password\" feature with your email address.\n" +
                        "As we could not find your email address in our system, this could not be completed.\n" +
                        "Thank you for your patience.";
            mail.Subject = "Automated Email - Forgotten Password";

            SmtpClient smtpServer = new SmtpClient("smtp.live.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential("creative.solutions.murdoch@outlook.com", "kinetic2017S2") as ICredentialsByHost;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            //if (File.Exists(fn))
            if (Server.Database.UserExist(user))
            {
                User u = Server.Database.SelectUser(user);//GameObject.Find("UserManager").GetComponent<UserManager>().Load(user);

                //body = "Hi. Your password is: " + u.Password + "\nThank you for using Point Light.";
                mail.Body = "Hi. Your password has been reset to: " + u_newPassword + "\nThank you for using Point Light.";

                mail.To.Add(u.Email);

                int p = Server.Database.UpdateUser(u.Username, u.Email, u_newPassword);
            }

            //EmailDetails em = new EmailDetails(recipient, senderEmail, pass, senderName, host);

            Popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "An email has been sent to your account.";

            //em.Subject = subject;
            //em.Body = body;
            //em.AddCC(senderEmail);


            smtpServer.Send(mail);


            //EmailingSystem.SendEmail(em);

            CreatePopup(Popup, ToMain);
        }

        /// <summary>
        /// Allow user to logout of system
        /// </summary>
        public void TaskOnClickLogout()
        {
            PlayerPrefs.DeleteKey("EmailAddress");

            LoadLevel();
        }

        /// <summary>
        /// Allow user to change or update their settings, including details
        /// </summary>
        /// <param name="popup">popup to generate</param>
        public void TaskOnClickSettingsSave(GameObject popup)
        {
            m_canvas = "Settings/SettingsCanvas/";

            string pass = GameObject.Find(m_canvas + "PassInput").GetComponent<InputField>().text;
            string email = GameObject.Find(m_canvas + "EmailInput").GetComponent<InputField>().text;

            Toggle emailEnT = GameObject.Find(m_canvas + "EmailingToggle").GetComponent<Toggle>();
            Toggle recPopupT = GameObject.Find(m_canvas + "RecPopupToggle").GetComponent<Toggle>();
            Toggle editPopupT = GameObject.Find(m_canvas + "EditPopupToggle").GetComponent<Toggle>();
            Toggle simPopupT = GameObject.Find(m_canvas + "SimPopupToggle").GetComponent<Toggle>();

            bool emailEn = emailEnT.isOn;
            bool recPopup = recPopupT.isOn;
            bool editPopup = editPopupT.isOn;
            bool simPopup = simPopupT.isOn;

            GameObject userManager = GameObject.Find("UserManager");

            User u = userManager.GetComponent<UserManager>().MyUser;

            if (Valid(pass))
            {
                u.Password = pass;
            }

            if (Valid(email))
            {
                u.Email = email;
                PlayerPrefs.SetString("EmailAddress", u.Email);
            }

            PlayerPrefs.SetInt("EnableEmail", 0);

            if (emailEn)
            {
                PlayerPrefs.SetInt("EnableEmail", 1);
            }

            PlayerPrefs.SetInt("RecPopup", 0);

            if (recPopup)
            {
                PlayerPrefs.SetInt("RecPopup", 1);
            }

            PlayerPrefs.SetInt("EditPopup", 0);

            if (editPopup)
            {
                PlayerPrefs.SetInt("EditPopup", 1);
            }

            PlayerPrefs.SetInt("SimPopup", 0);

            if (simPopup)
            {
                PlayerPrefs.SetInt("SimPopup", 1);
            }

            userManager.GetComponent<UserManager>().Save(u);

            popup.transform.Find("Popup/ErrorLabel").GetComponent<Text>().text = "Successfully edited your account details.";
            CreatePopup(popup, NoFunc);
        }
    }
}