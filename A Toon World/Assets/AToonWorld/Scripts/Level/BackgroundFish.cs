using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Level
{
    public class BackgroundFish : RandomMovement
    {
        // Private Fields
        private float _bouncingSpeed = 0.2f;
        private float _direction = 1;
        private Transform _bubbleFontainTransform;

        protected override void Awake()
        {
            base.Awake();
            _bubbleFontainTransform = _transform.GetChild(0).transform;
        }

        protected override void Start()
        {
            base.Start();
            this.Invoke(() => ScaleAnimation().Forget(), UnityEngine.Random.Range(0, 2));
        }

        protected override Vector2 ChangeDestination()
        {                        
            //_transform.localScale = new Vector3(-_transform.localScale.x, transform.localScale.y, transform.localScale.z);
            var ret =_start + _movementRadius * Vector2.right * _direction;
            _direction *= -1;
            _bubbleFontainTransform.localScale = new Vector3(_bubbleFontainTransform.localScale.x * _direction, _bubbleFontainTransform.localScale.y, _bubbleFontainTransform.localScale.z);
            return ret;
        }

        // Animation
        private async UniTask ScaleAnimation()
        {
            UniTask ScaleTo(Vector3 to) => this.Transition
            (
                from: new Vector3(Mathf.Abs(_transform.localScale.x), Mathf.Abs(_transform.localScale.y), Mathf.Abs(transform.localScale.z)),
                to: to,
                callback: val => transform.localScale = new Vector3(-_direction * val.x, val.y, to.z),
                speed: _bouncingSpeed,
                smooth: false
            );

            while (true)
            {
                var startScale = new Vector3(Mathf.Abs(_transform.localScale.x), Mathf.Abs(_transform.localScale.y), Mathf.Abs(transform.localScale.z));
                await ScaleTo(startScale * 1.2f);
                await ScaleTo(startScale);
            }
        }
    }
}
