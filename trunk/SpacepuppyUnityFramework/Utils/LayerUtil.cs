using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{
    public static class LayerUtil
    {

        private static List<int> layerNumbers;
        private static List<string> layerNames;
        private static long lastUpdateTick;

        private static void TestUpdateLayers()
        {
            if (layerNumbers == null || (System.DateTime.UtcNow.Ticks - lastUpdateTick > 10000000L && Event.current.type == EventType.Layout))
            {
                lastUpdateTick = System.DateTime.UtcNow.Ticks;
                if (layerNumbers == null)
                {
                    layerNumbers = new List<int>();
                    layerNames = new List<string>();
                }
                else
                {
                    layerNumbers.Clear();
                    layerNames.Clear();
                }

                for (int i = 0; i < 32; i++)
                {
                    string layerName = LayerMask.LayerToName(i);

                    if (layerName != "")
                    {
                        layerNumbers.Add(i);
                        layerNames.Add(layerName);
                    }
                }
            }
        }

        public static string[] GetLayerNames()
        {
            TestUpdateLayers();

            return layerNames.ToArray();
        }

        public static string[] GetAllLayerNames()
        {
            TestUpdateLayers();

            string[] names = new string[32];
            for (int i = 0; i < 32; i++)
            {
                if (layerNumbers.Contains(i))
                {
                    names[i] = LayerMask.LayerToName(i);
                }
                else
                {
                    names[i] = "Layer " + i.ToString();
                }
            }
            return names;
        }

    }
}
