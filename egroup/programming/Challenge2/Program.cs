using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace Challenge2
{
    /// <summary>
    /// Изработено од Симон Ќулумов 142098
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            const string challengeUrl = "http://challenges.enigmagroup.org/programming/2/";
            const string submitChallengeUrl = "http://challenges.enigmagroup.org/programming/2/index.php";
            const string enigmaSessionIdCookie = "PHPSESSID";
            const string enigmaSessionIdCookieValue = "YOUR_COOKIE_VALUE";
            const string enigmaCookieDomain = "enigmagroup.org";
            const string answerInputName = "answer";
            const string randomNumberInputName = "E[number]";
            const string timeInputName = "E[time]";
            const string hashInputName = "hash";
            const string submitButtonInputName = "submit";

            var answer = 0;
            var randomNumber = 0;
            var time = string.Empty;
            var hash = string.Empty;
            var submitAnswer = string.Empty;

            var cookie = new Cookie(enigmaSessionIdCookie, enigmaSessionIdCookieValue) { Domain = enigmaCookieDomain };
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(cookie);

            try
            {
                //GET Request
                var getRequest = (HttpWebRequest)WebRequest.Create(challengeUrl);
                getRequest.CookieContainer = cookieContainer;

                using (var response = (HttpWebResponse)getRequest.GetResponse())
                {
                    using var responseStream = response.GetResponseStream();
                    var readStream = string.IsNullOrWhiteSpace(response.CharacterSet) 
                        ? new StreamReader(responseStream) 
                        : new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.Load(readStream);

                    var numberInput = htmlDoc.DocumentNode
                        .Descendants("input")
                        .ToList()
                        .FirstOrDefault(x => x.Attributes["name"].Value == randomNumberInputName);
                    randomNumber = Convert.ToInt32(numberInput?.Attributes["value"].Value);

                    var timeInput = htmlDoc.DocumentNode
                        .Descendants("input")
                        .ToList()
                        .FirstOrDefault(x => x.Attributes["name"].Value == timeInputName);
                    time = timeInput?.Attributes["value"].Value;

                    var hashInput = htmlDoc.DocumentNode.Descendants("input")
                        .ToList()
                        .FirstOrDefault(x => x.Attributes["name"].Value == hashInputName);
                    hash = hashInput?.Attributes["value"].Value;

                    var submitButtonInput = htmlDoc.DocumentNode.Descendants("input")
                        .ToList()
                        .FirstOrDefault(x => x.Attributes["name"].Value == submitButtonInputName);
                    submitAnswer = submitButtonInput?.Attributes["value"].Value;

                    answer = randomNumber * 4;

                    Console.WriteLine(htmlDoc.ToString());
                    response.Close();
                    readStream.Close();
                }

                Console.WriteLine("Printam GET form data i hidden input fields dobieni od server: ");
                Console.WriteLine("Random number: " + randomNumber);
                Console.WriteLine("Answer (random number * 4): " + answer);
                Console.WriteLine("E[time] " + time);
                Console.WriteLine("Hash: " + hash);
                Console.WriteLine("Submit button: " + submitAnswer);
                Console.WriteLine("===========================================================");
                Console.WriteLine("Printam POST REQUEST - response od server: ");

                //POST request
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"{answerInputName}={answer}");
                stringBuilder.Append($"&{randomNumberInputName}={randomNumber}");
                stringBuilder.Append($"&{timeInputName}={time}");
                stringBuilder.Append($"&{hashInputName}={hash}");
                stringBuilder.Append($"&{submitButtonInputName}={submitAnswer}");

                var data = Encoding.ASCII.GetBytes(stringBuilder.ToString());
                var postRequest = (HttpWebRequest)WebRequest.Create(submitChallengeUrl);
                postRequest.CookieContainer = cookieContainer;
                postRequest.Method = WebRequestMethods.Http.Post;
                postRequest.ContentType = "application/x-www-form-urlencoded";
                postRequest.ContentLength = data.Length;

                using (var stream = postRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var postResponse = (HttpWebResponse)postRequest.GetResponse();

                var responseString = new StreamReader(postResponse.GetResponseStream()).ReadToEnd();

                Console.WriteLine(responseString);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}