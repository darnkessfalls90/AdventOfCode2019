using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceImageFormat
{
    public class SpaceImageDecoder
    {
        public static List<int[,]> GetLayers(int[] input, int height, int width)
        {
            int layerCount = 0;
            List<int[,]> layers = new List<int[,]>();
            while ((height * width) * layerCount < input.Length)
            {
                var layer = new int[height, width];

                var flatLayer = input.Skip((height * width) * layerCount).Take(height * width);
                for (var i = 0; i < height; i++)
                {
                    var line = flatLayer.Skip(width * i).Take(width).ToArray();
                    for (int n = 0; n < width; n++)
                    {
                        layer[i, n] = line[n];
                    }
                }
                layers.Add(layer);
                layerCount++;
            }

            return layers;
        }
    }
}
