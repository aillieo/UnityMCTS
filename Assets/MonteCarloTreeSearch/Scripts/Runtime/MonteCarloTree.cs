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

        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private MonteCarloTree()
        {
        }

        public static MonteCarloTree<T> CreateTree(IAgent agent, T state)
        {
            //Node<T> node = GetNode();
            Node<T> node = new Node<T>();
            node.state.CopyFrom(state);

            return new MonteCarloTree<T>()
            {
                agent = agent,
                root = node,
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

            readerWriterLock.EnterReadLock();
            try
            {
                if (root.state.IsTerminal())
                {
                    return root;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }

            while (DateTime.Now < end)
            {
                Node<T> child = Selection(root);

                readerWriterLock.EnterWriteLock();
                try
                {
                    if (child.simulateTimes != 0)
                    {
                        child = Expansion(child);
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                    throw;
                }
                finally
                {
                    readerWriterLock.ExitWriteLock();
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

        public Node<T> Selection(Node<T> node)
        {
            readerWriterLock.EnterReadLock();
            try
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
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }

        private Node<T> SelectChild(Node<T> node)
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

        public Node<T> Expansion(Node<T> node)
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
                    //Node<T> newNode = nodePool.Get();
                    //newNode.state = s;
                    //newNode.parent = node;
                    //newNode.depth = node.depth + 1;
                    //node.children.Add(newNode);

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

            float value = default;

            //readerWriterLock.EnterUpgradeableReadLock();
            //readerWriterLock.EnterReadLock();
            try
            {
                value = node.state.Simulate(agent);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
            finally
            {
                //readerWriterLock.ExitUpgradeableReadLock();
                //readerWriterLock.ExitReadLock();
            }

            readerWriterLock.EnterWriteLock();

            try
            {
                node.value += value;
                node.simulateTimes++;
                return value;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        public void BackPropagation(Node<T> node, float value)
        {
            readerWriterLock.EnterWriteLock();

            try
            {
                Node<T> parent = node.parent;
                while (parent != null)
                {
                    parent.simulateTimes++;
                    parent.value += value;
                    parent = parent.parent;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                throw;
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        private static float UCT(Node<T> node)
        {
            if (node.simulateTimes == 0)
            {
                return float.MaxValue;
            }

            float exploit = node.value / node.simulateTimes;
            float explore = Mathf.Sqrt(Mathf.Log(node.parent.simulateTimes) / node.simulateTimes);
            float c = 1.41421356f;
            return exploit + c * explore;
        }
    }
}
