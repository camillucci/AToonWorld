using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UnityAnimations
{
    public class GenericAnimations : Singleton<GenericAnimations>
    {
        [SerializeField] private GameObject _inkCloudPrefab = null;


        public static FireAndForgetAnimation InkCloud(Vector2 position)
        {
            var instance = Instantiate(PrefabInstance._inkCloudPrefab);
            instance.transform.position = position;
            return instance.GetComponent<FireAndForgetAnimation>();
        }
    }
}
