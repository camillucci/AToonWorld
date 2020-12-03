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
        private const string DrawingsPath = "Drawing/";
        private const string VictoryPath = "Checkpoint-Collectible-Victory/";
        private const string MovementPath = "Footsteps/";

        private static List<SoundEffect> _sfx;
        private static List<SoundEffect> _deathSounds;
        private static List<SoundEffect> _bulletSounds;
        private static List<SoundEffect> _drawingSounds;
        private static List<SoundEffect> _checkPointsSounds;
        private static List<SoundEffect> _horizontalMovementSounds;
        private static SoundEffect _leftStep;
        private static SoundEffect _rightStep;
        private static SoundEffect _checkPoint;
        private static SoundEffect _victory;
        public const string Pop = nameof(Pop);


        
        // Private Properties
        private static List<SoundEffect> Sfx => _sfx ?? (_sfx = AudioManager.Instance.GetAllSfx().ToList());



        // Public Properties
        public static IReadOnlyList<SoundEffect> DeathSounds => _deathSounds ?? (_deathSounds = GetCategory(DeathPath));
        public static IReadOnlyList<SoundEffect> BulletSounds => _bulletSounds ?? (_bulletSounds = GetCategory(BulletPath));
        public static IReadOnlyList<SoundEffect> DrawingSounds => _drawingSounds ?? (_drawingSounds = GetCategory(DrawingsPath));
        public static IReadOnlyList<SoundEffect> CheckpontSounds => _checkPointsSounds ?? (_checkPointsSounds = GetCategory(VictoryPath));
        public static IReadOnlyList<SoundEffect> HorizontalMovementSounds => _horizontalMovementSounds ?? (_horizontalMovementSounds = GetCategory(MovementPath));

        public static SoundEffect LeftStep => _leftStep ?? (_leftStep = HorizontalMovementSounds.FirstOrDefault(sound => sound.name.Equals("Footsteps/Floor/Floor_step0")));
        public static SoundEffect RightStep => _rightStep ?? (_rightStep = HorizontalMovementSounds.FirstOrDefault(sound => sound.name.Equals("Footsteps/Floor/Floor_step1")));
        public static SoundEffect CheckPoint => _checkPoint ?? (_checkPoint = CheckpontSounds.FirstOrDefault(sound => sound.name.Equals("Checkpoint-Collectible-Victory/2")));
        public static SoundEffect Victory => _victory ?? (_victory = CheckpontSounds.FirstOrDefault(sound => sound.name.Equals("Checkpoint-Collectible-Victory/1")));



        // Private Methods
        private static List<SoundEffect> GetCategory(string relativePath)
        {
            var effects = from soundEffect in Sfx where soundEffect.RelativePath.StartsWith(relativePath) select soundEffect;
            return effects.ToList();
        }
    }
}
