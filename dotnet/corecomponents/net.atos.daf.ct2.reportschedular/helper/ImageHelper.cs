using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.helper
{
    public class ImageSingleton
    {
        private static ImageSingleton _instance;
        private static string _defaultlogoImage;
        private static string _logoImage;
        private static readonly Object _root = new object();
        private ImageSingleton()
        {
        }

        public static ImageSingleton GetInstance()
        {
            lock (_root)
            {
                if (_instance == null)
                {
                    _defaultlogoImage = string.Format("data:image/gif;base64,{0}",
                        Convert.ToBase64String(File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo.png"))));
                    _logoImage = string.Format("data:image/gif;base64,{0}",
                                                    Convert.ToBase64String(File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo_daf.png"))));
                    _instance = new ImageSingleton();
                }
            }
            return _instance;
        }

        public string GetDefaultLogo()
        {
            return _defaultlogoImage;
        }

        public string GetLogo()
        {
            return _logoImage;
        }
    }
}
