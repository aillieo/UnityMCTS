using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public class Pool<T> where T : class, new()
    {
        private readonly BlockingCollection<T> stack;

        public Pool(int capacity)
        {
            this.stack = new BlockingCollection<T>(new ConcurrentStack<T>(), capacity);
        }

        public T Get()
        {
            if (stack.TryTake(out T obj))
            {
                return obj;
            }

            return new T();
        }

        public void Recycle(T obj)
        {
            stack.TryAdd(obj);
        }
    }
}
