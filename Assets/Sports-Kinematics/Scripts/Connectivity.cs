using System.Net;

public static class Connectivity
{
    /// <summary>
    /// WebClient used to check internet connectivity.
    /// </summary>
    static WebClient m_client;

    /// <summary>
    /// Checks if there is an active internet connection by checking
    /// if there is a connection to the website specified by the site argument.
    /// </summary>
    /// <param name="site">Site accessed to check connectivity.</param>
    /// <returns>True if you can connect to the site.</returns>
    public static bool CheckConnection(string site)
    {
        try
        {
            m_client = new WebClient();

            using (var stream = m_client.OpenRead(site))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if there is an active internet connection by checking
    /// if there is a connection to the Google website.
    /// </summary>
    /// <returns>True if you can connect to Google.</returns>
    public static bool CheckConnection()
    {
        try
        {
            m_client = new WebClient();

            using (var stream = m_client.OpenRead("http://www.google.com"))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}