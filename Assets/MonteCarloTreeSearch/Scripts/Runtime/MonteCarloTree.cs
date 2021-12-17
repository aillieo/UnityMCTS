using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public class MonteCarloTree<T> where T : class, IState<T>, new()
    {
        // Selection
        // Expansion
        // Simulation
        // Back propagation

        private Node<T> root;

        private IAgent agent;

        // private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

        private MonteCarloTree()
        {
        }

        public static MonteCarloTree<T> CreateTree(IAgent agent, T state)
        {
            Node<T> node = GetNode();
            node.state.CopyFrom(state);

            return new MonteCarloTree<T>()
            {
                agent = agent,
                root = new Node<T>()
                {
                    state = state,
                    depth = 0,
                },
            };
        }

        private static Pool<Node<T>> nodePool;

        static MonteCarloTree()
        {
            nodePool = new Pool<Node<T>>(32768);
        }

        public static void Recycle(MonteCarloTree<T> toRecycle)
        {
            RecycleNode(toRecycle.root);
        }

        public static Node<T> GetNode()
        {
            return nodePool.Get();
        }

        public static void RecycleNode(Node<T> toRecycle)
        {
            toRecycle.Reset();
            nodePool.Recycle(toRecycle);
        }

        public Node<T> Run(int milliseconds)
        {
            DateTime start = DateTime.Now;
            DateTime end = start.AddMilliseconds(milliseconds);

            if (root == null)
            {
                throw new Exception();
            }

            if (root.state.IsTerminal())
            {
                return root;
            }

            while (DateTime.Now < end)
            {
                Node<T> child = Selection(root);

                //if (child.simulateTimes != 0 || child.children.Any(c => c.simulateTimes == 0))
                if (child.simulateTimes != 0)
                {
                    child = Expansion(child);
                }

                float value = Simulation(child);
                BackPropagation(child, value);
            }

            UnityEngine.Debug.Log($"will sel from {root}");
            foreach (var c in root.children)
            {
                UnityEngine.Debug.Log($"{c}");
            }

            return SelectChild(root);
        }

        public static Node<T> Selection(Node<T> node)
        {
            while (node.children != null && node.children.Count > 0)
            {
                node = SelectChild(node);

                if (node.depth > 225)
                {
                    throw new Exception("too deep");
                }
            }

            //UnityEngine.Debug.Log($"Selection: {node}");

            return node;
        }

        private static Node<T> SelectChild(Node<T> node)
        {
            Node<T> best = default;
            float max = float.MinValue;
            foreach (var n in node.children)
            {
                float uct = UCT(n);
                if (uct > max)
                {
                    max = uct;
                    best = n;
                }
            }

            return best;
        }

        public static Node<T> Expansion(Node<T> node)
        {
            //UnityEngine.Debug.Log($"Expansion: {node}");

            if (node.state.IsTerminal())
            {
                return node;
            }

            if (node.children == null || node.children.Count == 0)
            {
                foreach (var s in node.state.Expand())
                {
                    node.children.Add(new Node<T>()
                    {
                        state = s,
                        parent = node,
                        depth = node.depth + 1,
                    });
                }
            }

            var newChildren = node.children.Where(c => c.simulateTimes == 0);
            if (newChildren.Any())
            {
                return newChildren.First();
            }

            return node.children.FirstOrDefault();
        }

        public float Simulation(Node<T> node)
        {
            //UnityEngine.Debug.Log($"Simulation: {node}");

            float value = node.state.Simulate(agent);
            node.value += value;
            node.simulateTimes++;
            return value;
        }

        public static void BackPropagation(Node<T> node, float value)
        {
            //UnityEngine.Debug.Log($"BackPropagation: {node}");

            Node<T> parent = node.parent;
            while (parent != null)
            {
                parent.simulateTimes++;
                parent.value += value;
                parent = parent.parent;
            }
        }

        private static float UCT(Node<T> node)
        {
            float exploit = node.simulateTimes == 0 ? 0 : node.value / node.simulateTimes;
            float explore = Mathf.Sqrt(Mathf.Log(node.parent.simulateTimes) / node.simulateTimes);
            float c = 1.41421356f;
            return exploit + c * explore;
        }
    }
}
