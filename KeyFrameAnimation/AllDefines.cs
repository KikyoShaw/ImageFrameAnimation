using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace KeyFrameAnimation
{
    public struct KeyFrame
    {
        /// <summary>
        /// 当前帧的bitmap
        /// </summary>
        public BitmapSource AFrame;
        /// <summary>
        /// 当前帧具前一帧的间隔，单位-毫秒，第一帧的时候是 0
        /// </summary>
        public double Duration;
    }
}
