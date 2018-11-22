using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Repository
{
    public class IMPRPosterData : IPosterData
    {
        private readonly string _base = "http://impawards.com";

        public async Task<string> GetMoviePoster(int year, string name)
        {
            string imagePath;
            name = TrimMovieName(name);

            string url = $"{_base}/{year}/posters/{name}.jpg";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response;

            try
            {
                response = await request.GetResponseAsync();
            }
            catch (Exception)
            {
                url = $"{_base}/{year}/posters/{name}_ver1.jpg";
                request = (HttpWebRequest)WebRequest.Create(url);

                try
                {
                    response = await request.GetResponseAsync();
                }
                catch (Exception)
                {
                    url = $"{_base}/{year}/posters/{name}_ver2.jpg";
                    request = (HttpWebRequest)WebRequest.Create(url);

                    try
                    {
                        response = await request.GetResponseAsync();
                    }
                    catch (Exception)
                    {
                        response = null;
                    }
                }
            }

            if (response != null)
            {
                imagePath = url;
                response.Close();
            }
            else
                imagePath = "~/images/not_found.jpg";

            return imagePath;
        }

        private string TrimMovieName(string name)
        {
            List<string> list = name.ToLower().Split().ToList();

            if (list.Contains("a"))
                list.Remove("a");

            if (list.Contains("the"))
                list.Remove("the");

            name = String.Join("_", list);

            return name;
        }
    }
}
