using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Audio
{
    [Serializable]
    public class Sound
    {
        // Editor Fields
        [SerializeField] private string _name;
        [SerializeField] private AudioClip _clip;

        // Initialization
        public Sound(string name, AudioClip clip)
        {
            _name = name;
            _clip = clip;
        }


        // Public Properties
        public string Name => _name;
        public AudioClip Clip => _clip;
    }
}
