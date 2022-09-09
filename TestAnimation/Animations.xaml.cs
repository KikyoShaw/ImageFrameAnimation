using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KeyFrameAnimation;

namespace TestAnimation
{
    /// <summary>
    /// Interaction logic for Animations.xaml
    /// </summary>
    public partial class Animations : UserControl
    {
        public Animations()
        {
            InitializeComponent();
            var placeHolders = new List<int>();
            var index = 0;
            while (index < 10)
            {
                placeHolders.Add(index);
                ++index;
            }


            // 先添加进缓存，再将sourceKey设置到 ImageBehavior.SourceKey
            //AnimationCache.Instance.AddKeyFrames("test", Helper.GetKeyFrames());

            ItemsControl.ItemsSource = placeHolders;
        }
    }
}
