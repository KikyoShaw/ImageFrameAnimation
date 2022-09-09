using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace TestAnimation
{
    public class SpeakTimeMgr
    {
        private static readonly Lazy<SpeakTimeMgr> Lazy = new Lazy<SpeakTimeMgr>(() => new SpeakTimeMgr());
        public static SpeakTimeMgr Instance => Lazy.Value;

        public Action<object> InvokeSpeakAmiTime { get; set; } = null;
        //动画timer
        public System.Timers.Timer SpeakingAnimTimer;
        public int UserCount = 0;
        SpeakTimeMgr()
        {
            SpeakingAnimTimer = new System.Timers.Timer();
            SpeakingAnimTimer.Interval = 50;
            SpeakingAnimTimer.Elapsed += SpeakingAnimTimer_Elapsed;
        }

        private void SpeakingAnimTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                InvokeSpeakAmiTime?.Invoke(sender);
            }
            catch (Exception e1)
            {
                //LogHelper.LogError($"SpeakingAnimTimer_Elapsed error={e1.Message}");
            }
        }
        public void Start()
        {
            if (!SpeakingAnimTimer.Enabled)
            {
                SpeakingAnimTimer.Start();
            }
            UserCount++;
        }
        public void Stop()
        {
            UserCount--;
            if (UserCount == 0)
            {
                SpeakingAnimTimer.Stop();
            }

        }
    }

    public class SpeakResourceFactory
    {
        private static readonly Lazy<SpeakResourceFactory> Lazy = new Lazy<SpeakResourceFactory>(() => new SpeakResourceFactory());
        public static SpeakResourceFactory Instance => Lazy.Value;

        readonly Dictionary<int, Dictionary<int, BitmapImage>> _speakResource = new
            Dictionary<int, Dictionary<int, BitmapImage>>();

        private readonly int _resourceCount = 27;
        private List<BitmapImage> _imageList = null;

        SpeakResourceFactory()
        {
            _imageList = GetBitmapListFromPath(@"E:/shaw/demo/wpfAnimation/TestAnimation/Resources/test");
            _resourceCount = _imageList.Count;
        }

        //获取下载路径下的资源文件
        private List<BitmapImage> GetBitmapListFromPath(string sPath)
        {
            try
            {
                if (!Directory.Exists(sPath))
                    return null;

                DirectoryInfo cacheDir = new DirectoryInfo(sPath);
                FileInfo[] cacheFiles = cacheDir.GetFiles();
                if ( cacheFiles.Length == 0)
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

        public BitmapImage GetImageSource(int index)
        {
            if ((index < 0 && index >= _resourceCount) || _imageList == null)
                return null;

            return _imageList[index];
        }

        public int GetImageSourceCount()
        {
            return _resourceCount;
        }
    }

    public class SpeakingAnimImageTimer : UserControl
    {

        private readonly SpeakTimeMgr _timeMgr = SpeakTimeMgr.Instance;

        private readonly SpeakResourceFactory _resMgr = SpeakResourceFactory.Instance;

        //当前帧数
        private int _imageIndex = 0;

        //是否播放中
        bool _running = false;

        public static readonly DependencyProperty IsFemaleProperty = DependencyProperty.Register(
             "IsFemale", typeof(bool), typeof(SpeakingAnimImageTimer), new PropertyMetadata(false));


        public static readonly DependencyProperty IsSpeakingProperty = DependencyProperty.Register(
             "IsSpeaking", typeof(bool), typeof(SpeakingAnimImageTimer),
              new PropertyMetadata(false, new PropertyChangedCallback(OnIsSpeakingPropertyChange)));

        public bool IsSpeaking
        {
            get => (bool)GetValue(IsSpeakingProperty);
            set => SetValue(IsSpeakingProperty, value);
        }
        private static void OnIsSpeakingPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SpeakingAnimImageTimer obj = ((SpeakingAnimImageTimer)d as SpeakingAnimImageTimer);
            if(obj != null)
                obj.DoSpeakingStageChange();
        }

        private void DoSpeakingStageChange()
        {
            if (IsSpeaking == true)
            {
                if(!_running)
                {
                    _running = true;
                    _timeMgr?.Start();
                    if (_timeMgr != null) _timeMgr.InvokeSpeakAmiTime += SpeakingAnimTimer;
                }
            }
            else
            {
                if (_running)
                {
                    _running = false;
                    _timeMgr?.Stop();
                    if (_timeMgr != null) _timeMgr.InvokeSpeakAmiTime -= SpeakingAnimTimer;
                    _imageIndex = 0;
                    InvalidateVisual();
                }
            }
        }

        static SpeakingAnimImageTimer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpeakingAnimImage), new FrameworkPropertyMetadata(typeof(SpeakingAnimImage)));
        }

        ~SpeakingAnimImageTimer()
        {
            if(_running)
            {
                _running = false;
                _timeMgr?.Stop();
                if (_timeMgr != null) _timeMgr.InvokeSpeakAmiTime -= SpeakingAnimTimer;
            }
        }

        private void SpeakingAnimTimer(object sender)
        {
            try
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        _imageIndex++;
                        _imageIndex = _imageIndex % _resMgr.GetImageSourceCount();
                        InvalidateVisual();
                    }
                    catch (Exception ex)
                    {
                        //LogHelper.LogError(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                //LogHelper.LogError(ex.Message);
            }
        }
        protected override void OnRender(DrawingContext dc)
        {
            try
            {
                var img = _resMgr.GetImageSource(_imageIndex);
                if (img != null)
                    dc.DrawImage(img, new Rect(0, 0, ActualWidth, ActualHeight));
            }
            catch (Exception ex)
            {
                //LogHelper.LogError(ex.Message);
            }
        }
    }
}
