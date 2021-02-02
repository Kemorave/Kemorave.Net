using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Kemorave.Win.Properties;
namespace Kemorave.Win.Effects
{
    public class DeeperColorEffect : ShaderEffect
    {

        public static DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(DeeperColorEffect), 0);

        public DeeperColorEffect()
        {
            PixelShader = new PixelShader();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Resources.DeeperColor))
            {
                PixelShader.SetStreamSource(ms);
            }
            UpdateShaderValue(InputProperty);
        }
        public Brush Input
        {
            get
            {
                return ((Brush)(GetValue(InputProperty)));
            }
            set
            {
                SetValue(InputProperty, value);
            }
        }
    }
}
