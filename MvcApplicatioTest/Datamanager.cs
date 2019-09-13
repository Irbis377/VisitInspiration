using FlickrNet;
using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MvcApplicatioTest.Models
{
    public class DataManager
    {
       public static Flickr flickr;

        public DataManager()
        {
            flickr = new Flickr(WebConfigurationManager.AppSettings["apiKey"], WebConfigurationManager.AppSettings["sharedSecret"]);
        }
        /// <summary>
        /// Returns one block of photos items
        /// </summary>
        /// <param name="BlockSize">Items count in a block</param>
        /// <returns></returns>
        public static List<PhotoFlickr> GetPhotos(int BlockSize)
        {

            PhotoSearchOptions options = new PhotoSearchOptions();
            options.HasGeo = true;
            options.Page = 1;
            options.PerPage = BlockSize;
            options.MediaType = MediaType.Photos;
            options.Extras = PhotoSearchExtras.All;
            options.Tags = "travels";

            
            PhotoCollection photoCol = flickr.PhotosSearch(options);


            var photos = new List<PhotoFlickr>();
            for (int i = 0; i < BlockSize; i++)
            {
                GeoContext _geotag = photoCol[i].GeoContext.Value;
                
                photos.Add(new PhotoFlickr
                            {
                                PhotoId = photoCol[i].PhotoId,
                                OwnerName = photoCol[i].OwnerName,
                                Geotag = _geotag.ToString(),
                                
                                MediumUrl = photoCol[i].Medium640Url,
                                LargeUrl = photoCol[i].Large1600Url,
                                SmallUrl = photoCol[i].Small320Url,
                                ThumbnailUrl = photoCol[i].ThumbnailUrl,
                                DateTaken = photoCol[i].DateTaken.ToString("dd MMMM yyyy"),
                                Description = photoCol[i].Description,
                                Title = photoCol[i].Title
                            });
            }
          
            return photos;
        }

        private string GetDescription(string photoId)
        {
            Flickr flickr = new Flickr(WebConfigurationManager.AppSettings["apiKey"], WebConfigurationManager.AppSettings["sharedSecret"]);
            PhotoInfo info = flickr.PhotosGetInfo(photoId);

            return info.Description;
            //PhotoDateTaken.Text = info.DateTaken.ToString("MMMM dd,  yyyy");
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
    
}