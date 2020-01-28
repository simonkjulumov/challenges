using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Challenge3
{
    /// <summary>
    /// Изработено од Симон Ќулумов 142098
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            const string challengeRefererUrl = "http://challenges.enigmagroup.org/programming/3/";
            const string submitChallengeUrl = "http://challenges.enigmagroup.org/programming/3/image.php";
            const string enigmaSessionIdCookie = "PHPSESSID";
            const string enigmaSessionIdCookieValue = "YOUR_COOKIE_VALUE";
            const string enigmaCookieDomain = "enigmagroup.org";

            var cookie = new Cookie(enigmaSessionIdCookie, enigmaSessionIdCookieValue) { Domain = enigmaCookieDomain };
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(cookie);

            try
            {
                //GET REQUEST
                var request = (HttpWebRequest)WebRequest.Create(submitChallengeUrl);
                request.CookieContainer = cookieContainer;
                request.Referer = challengeRefererUrl;
                request.KeepAlive = false;

                Image image;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using var stream = response.GetResponseStream();
                    image = Image.FromStream(stream);
                }

                request.ServicePoint.CloseConnectionGroup(request.ConnectionGroupName);

                //Извади RGB од сликата
                var bitmap = new Bitmap(image);
                var r = bitmap.GetPixel(0, 0).R.ToString();
                var g = bitmap.GetPixel(0, 0).G.ToString();
                var b = bitmap.GetPixel(0, 0).B.ToString();
                var postData = $"color={r};{g};{b}&submit=1";

                Console.WriteLine("RGB: " + postData);

                //POST REQUEST
                var data = Encoding.ASCII.GetBytes(postData);
                var postRequest = (HttpWebRequest)WebRequest.Create(submitChallengeUrl);
                postRequest.CookieContainer = cookieContainer;
                postRequest.Method = WebRequestMethods.Http.Post;
                postRequest.ContentType = "application/x-www-form-urlencoded";
                postRequest.ContentLength = data.Length;
                postRequest.KeepAlive = false;
                postRequest.Referer = submitChallengeUrl;

                using (var stream = postRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var postResponse = (HttpWebResponse)postRequest.GetResponse();
                var responseString = new StreamReader(postResponse.GetResponseStream()).ReadToEnd();

                Console.WriteLine(responseString);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            Console.ReadLine();
        }
    }
}