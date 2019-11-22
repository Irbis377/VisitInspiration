using FlickrNet;
using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace MvcApplicatioTest.Models
{
    public class DataManagerFlickr
    {
       public static Flickr flickr;
       private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
       private static int counterpage = 1; 


        public DataManagerFlickr()
        {
            flickr = new Flickr(WebConfigurationManager.AppSettings["apiKey"], WebConfigurationManager.AppSettings["sharedSecret"]);
            flickr.AuthOAuthGetAccessToken();

        }
        /// <summary>
        /// Returns one block of photos items
        /// </summary>
        /// <param name="BlockSize">Items count in a block</param>
        /// <returns></returns>
        public static List<PhotoFlickr> GetPhotos(int BlockSize)
        {
          //  flickr.photos.getWithGeoData

            PhotoSearchOptions options = new PhotoSearchOptions();
            options.HasGeo = true;
            options.Page = counterpage;
            options.PerPage = BlockSize;
            options.MediaType = MediaType.Photos;
            options.Extras = PhotoSearchExtras.All;
            options.Tags = "turism,travels,traveler,sea,ocean,city,town,mountains,country";

            try
            {
                PhotoCollection photoCol = flickr.PhotosSearch(options);
                DataManagerAviaSales.SendRequestPrice();
                try
                {
                    flickr.AuthOAuthCheckToken();
                    PhotoCollection photoCol2 = flickr.PhotosGetWithGeoData();
                    PhotoCollection photoCol3 = flickr.PhotosGetWithGeoData(options);
                }
                catch(Exception e)
                {

                }

                var photos = new List<PhotoFlickr>();
                if (photoCol.Count != 0)
                {
                    for (int i = 0; i < BlockSize; i++)
                    {
                        // GeoContext _geotag = photoCol[i].GeoContext.Value;

                        photos.Add(new PhotoFlickr
                                    {
                                        PhotoId = photoCol[i].PhotoId,
                                        OwnerName = photoCol[i].OwnerName,
                                        //   Geotag = _geotag.ToString(),

                                        MediumUrl = photoCol[i].Medium640Url,
                                        LargeUrl = photoCol[i].Large1600Url,
                                        SmallUrl = photoCol[i].Small320Url,
                                        ThumbnailUrl = photoCol[i].ThumbnailUrl,
                                        DateTaken = photoCol[i].DateTaken.ToString("dd.MM.yyyy"),
                                        Description = photoCol[i].Description,
                                        Title = photoCol[i].Title
                                    });
                    }
                }
                counterpage++;
                return photos;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return null;
            }

        }

        private string GetDescription(string photoId)
        {
            Flickr flickr = new Flickr(WebConfigurationManager.AppSettings["apiKey"], WebConfigurationManager.AppSettings["sharedSecret"]);
            PhotoInfo info = flickr.PhotosGetInfo(photoId);
            return info.Description;
        }
    }

    public class PhotoFlickr
    {
        public string PhotoId { get; set; }

        public string LargeUrl { get; set; }
        public string MediumUrl { get; set; }
        public string SmallUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        
        public string OwnerName { get; set; }
        public string DateTaken { get; set; }
        public string Geotag { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }   
    }


    public class DataManagerAviaSales
    {
        string DataFormatMain = "YYYY-MM-DD";
        string DataFormatShort = "YYYY-MM";

        static string hostAviaCheapestPrice = "https://api.travelpayouts.com/v1/prices/cheap?origin={origin}&destination={destination}&depart_date={departDate}&return_date={returnDate}&token={token}";

        //апи токен юзать тут X-Access-Token
        //Accept-Encoding: gzip, deflate. юзать для быстрого ответа

        public static string SendRequestPrice()
        {
            string resp = string.Empty;
            WebRequest request = WebRequest.Create(hostAviaCheapestPrice.Replace("{origin}", "MOW").Replace("{destination}", "AMS").Replace("{departDate}", "2019-10-25").Replace("{returnDate}", "2019-11-06").Replace("{token}",  WebConfigurationManager.AppSettings["apiAvia"]));
            request.Method = "GET"; 
          
          // string data = "sName=Hello world!";
          // byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Encoding: gzip, deflate");
            request.Headers.Add("X-Access-Token: " + WebConfigurationManager.AppSettings["apiAvia"]);
            //request.ContentLength = byteArray.Length;

            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.  
                StreamReader sr = new StreamReader(dataStream);
                // Read the content.  
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jsonObject = serializer.DeserializeObject(sr.ReadToEnd());
                resp = jsonObject.ToString();
            }
            response.Close();

            return resp;
        }
       
    }

    public class AviaSalesPriceRequest
    {
        string currency = "rub";
        string origin; //2 или 3 символа 
        string destination;
        string beginning_of_period;
        string period_type; //year, month 
        Boolean one_way;
        int page;
        int limit; //не более 1000
        Boolean show_to_affiliates;
        string sorting; //price, route, distance_unit_price 
        int trip_duration; //в неделях
    }


    public class AviaSalesResponse
    {
        Boolean success;
        string data;
        string error;
    }

    public class JsonModel
    {
        public string HTMLString { get; set; }
        public bool NoMoreData { get; set; }
    } 
}