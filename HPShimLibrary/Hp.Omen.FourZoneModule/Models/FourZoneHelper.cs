using System.Collections.Generic;
using System.Drawing;

namespace Hp.Omen.FourZoneModule.Models
{
    public static class FourZoneHelper
    {
        public static Color[] ParseColorArray(byte[] byteArray)
        {
            var colors = new List<Color>();

            for (var i = 0; i < 4; i++)
            {
                int n = 25 + i * 3;
                var color = Color.FromArgb(byteArray[n], byteArray[n + 1], byteArray[n + 2]);
                colors.Add(color);
            }

            return colors.ToArray();
        }

        public static void SetColorArray(Color[] c, ref byte[] originalColorArray)
        {
            for (var i = 0; i < 4; i++)
            {
                int n = 25 + i * 3;
                originalColorArray[n] = c[i].R;
                originalColorArray[n + 1] = c[i].G;
                originalColorArray[n + 2] = c[i].B;
            }
        }
    }
}