using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DerelictComputer
{
    /// <summary>
    /// Container for wavetables used in WavetableOscillator
    /// </summary>
    [Serializable]
    public class WavetableSet
    {
        [Serializable]
        public class Wavetable
        {
            public readonly double TopFrequency;
            public readonly float[] Table;

            public Wavetable(double topFrequency, float[] table)
            {
                TopFrequency = topFrequency;
                Table = table;
            }
        }

        public const string WavetableFileName = "wavetables.dat";

        private static WavetableSet[] _allWavetableSets;

        public readonly Wavetable[] Wavetables;

        public WavetableSet(Wavetable[] wavetables)
        {
            Wavetables = wavetables;
        }

        /// <summary>
        /// Retrieve the wavetable sets from StreamingAssets, and cache them for future retrievals
        /// </summary>
        /// <returns>the WavetableSet collection</returns>
        public static WavetableSet[] GetWavetableSets()
        {
            if (_allWavetableSets == null)
            {
                var formatter = new BinaryFormatter();
                FileStream file;
                var path = Path.Combine(Application.streamingAssetsPath, WavetableFileName);
                try
                {
                    file = File.Open(path, FileMode.Open);
                }
                catch (Exception)
                {
                    Debug.LogWarning("No wavetable data yet. Make it via the editor menu!");
                    return null;
                }

                _allWavetableSets = (WavetableSet[]) formatter.Deserialize(file);
                file.Close();
            }

            return _allWavetableSets;
        }
    }
}
