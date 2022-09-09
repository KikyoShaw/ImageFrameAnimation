using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using KeyFrameAnimation;

namespace TestAnimation
{
    public sealed class SpeakingAnimImage :UserControl
    {
        private Image _image = null;
        private AnimationController _controller = null;
        public SpeakingAnimImage()
        {
            _image = new Image
            {
                Visibility = Visibility.Collapsed
            };
            this.AddChild(_image);
            ImageBehavior.AddAnimationLoadedHandler(_image, Handler);
            AnimationCache.Instance.AddKeyFrames("test", Helper.GetKeyFrames());
            ImageBehavior.SetSourceKey(_image, "test");
        }

        private void Handler(object sender, RoutedEventArgs e)
        {
            _controller = ImageBehavior.GetAnimationController(_image);
        }

        public static readonly DependencyProperty IsSpeakingProperty = DependencyProperty.Register(
            "IsSpeaking", typeof(bool), typeof(SpeakingAnimImage),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSpeakingPropertyChange)));

        public bool IsSpeaking
        {
            get => (bool)GetValue(IsSpeakingProperty);
            set => SetValue(IsSpeakingProperty, value);
        }
        private static void OnIsSpeakingPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SpeakingAnimImage obj = ((SpeakingAnimImage)d as SpeakingAnimImage);
            if (obj != null)
                obj.DoSpeakingStageChange();
        }

        private void DoSpeakingStageChange()
        {
            _image.Visibility = IsSpeaking ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
