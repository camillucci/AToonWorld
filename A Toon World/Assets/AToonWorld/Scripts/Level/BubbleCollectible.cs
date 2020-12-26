using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.AToonWorld.Scripts.Level
{    
    public class BubbleCollectible : MonoBehaviour
    {
        private Collectible _collectible;

        private void Awake()
        {
            _collectible = GetComponentInChildren<Collectible>();            
        }

        private void Start()
        {
            _collectible.PlayerHit += _ => gameObject.SetActive(false);
        }
    }
}
