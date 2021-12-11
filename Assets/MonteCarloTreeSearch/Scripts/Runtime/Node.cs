using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public class Node<T> where T : class, IState<T>, new()
    {
        public T state = new T();

        public float value;

        public Node<T> parent;
        public readonly List<Node<T>> children = new List<Node<T>>();

        public int simulateTimes;
        public int depth;

        public override string ToString()
        {
            return $"v/t={(simulateTimes > 0 ? value / simulateTimes : 0)}({value}/{simulateTimes}) d={depth} cc={children.Count} s={state}";
        }

        public void Reset()
        {
            value = 0;
            parent = null;
            foreach (var c in children)
            {
                MonteCarloTree<T>.RecycleNode(c);
            }

            children.Clear();
            simulateTimes = 0;
            depth = 0;
            state.Reset();
        }
    }
}
