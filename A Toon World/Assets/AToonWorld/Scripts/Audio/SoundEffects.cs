using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Audio
{
    public static class SoundEffects
    {
        private const string DeathPath = "Death/";
        private const string BulletPath = "Bullets/";

        private static List<SoundEffect> _sfx;
        private static List<SoundEffect> _deathSounds;
        private static List<SoundEffect> _bulletSounds;
        public const string Pop = nameof(Pop);


        
        // Private Properties
        private static List<SoundEffect> Sfx => _sfx ?? (_sfx = AudioManager.Instance.GetAllSfx().ToList());



        // Public Properties
        public static IReadOnlyList<SoundEffect> DeathSounds => _deathSounds ?? (_deathSounds = GetCategory(DeathPath));
        public static IReadOnlyList<SoundEffect> BulletSounds => _bulletSounds ?? (_bulletSounds = GetCategory(BulletPath));


        
        // Private Methods
        private static List<SoundEffect> GetCategory(string relativePath)
        {
            var effects = from soundEffect in Sfx where soundEffect.RelativePath.StartsWith(relativePath) select soundEffect;
            return effects.ToList();
        }
    }
}
