using System.IO;
using System.Net;

namespace Utilities.Trovit
{
    public class HTTPClient
    {
        public static string Get(string url)
        {
            try
            {
                var response = WebRequest.Create(url).GetResponse() as HttpWebResponse;

                if (response.StatusCode != HttpStatusCode.OK) return null;

                return new StreamReader(response.GetResponseStream()).ReadToEnd();

            }
            catch (WebException)
            {
                return null;
            }
        }
    }
}
