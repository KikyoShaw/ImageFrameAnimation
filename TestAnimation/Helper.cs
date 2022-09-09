using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows;
using KeyFrameAnimation;

namespace TestAnimation
{
    static class Helper
    {
        public static List<KeyFrame> GetKeyFrames()
        {
            var duration = 50; // 每一帧间隔
            var k = new List<KeyFrame>();
            var list = GetBitmapListFromPath(@"E:/shaw/demo/wpfAnimation/TestAnimation/Resources/test");
            foreach (var image in list)
            {
                k.Add(new KeyFrame
                {
                    AFrame = image,
                    Duration = duration
                });
            }
            //for (var index = 1; index < 61; ++index)
            //{
            //    var bitmap = new BitmapImage(new Uri($"pack://application:,,,/Resources/fire/{index}.png"));
            //    k.Add(new KeyFrame
            //    {
            //        AFrame = bitmap,
            //        Duration = duration
            //    });
            //}

            return k;
        }

        private static List<BitmapImage> GetBitmapListFromPath(string sPath)
        {
            try
            {
                if (!Directory.Exists(sPath))
                    return null;

                DirectoryInfo cacheDir = new DirectoryInfo(sPath);
                FileInfo[] cacheFiles = cacheDir.GetFiles();
                if (cacheFiles.Length == 0)
                {
                    Directory.Delete(sPath, true);
                    return null;
                }

                List<BitmapImage> vResultList = new List<BitmapImage>();
                foreach (var f in cacheFiles)
                {
                    if (vResultList.Count >= 35)
                        break;

                    string fileType = f.Extension.ToLower();
                    if (fileType != ".png" && fileType != ".bmp" && fileType != ".jpg" &&
                        fileType != ".jpeg" && fileType != ".jpe")
                    {
                        continue;
                    }

                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = new Uri(f.FullName);
                    bmp.EndInit();
                    bmp.Freeze();
                    vResultList.Add(bmp);
                }

                return vResultList;
            }
            catch (Exception ex)
            {
                //LogHelper.LogError($"DownResources error={ex.Message}");
            }

            return null;
        }
    }
}
