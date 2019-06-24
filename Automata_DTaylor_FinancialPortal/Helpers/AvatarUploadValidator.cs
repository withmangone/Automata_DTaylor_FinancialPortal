using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Automata_DTaylor_Bugtracker.Helpers
{
    public static class AvatarUploadValidator
    {
        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
                return false;

            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            var isValid = false;

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    isValid= ImageFormat.Jpeg.Equals(img.RawFormat) ||                      
                        ImageFormat.Png.Equals(img.RawFormat) ||
                        ImageFormat.Gif.Equals(img.RawFormat);
                }
                //And/Or conditional evaluation- false || true == true
                isValid = isValid || Path.GetExtension(file.FileName) == ".jpg";
                return isValid;
            }
            catch
            {
                return false;
            }
        }
        
    }
}