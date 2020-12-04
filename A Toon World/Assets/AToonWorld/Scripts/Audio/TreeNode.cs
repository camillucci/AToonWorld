using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Audio
{
    [Serializable]
    public class TreeNode<T> 
    {
        [SerializeField] private string _category;
        [SerializeField] private List<T> _data;
        [SerializeField] private List<TreeNode<T>> _nodes;
        
        
        public TreeNode(string name, IEnumerable<T> data, IEnumerable<TreeNode<T>> nodes)
        {
            _category = name;
            _data = data.ToList();
            _nodes = nodes.ToList();
        }
    }
}
