using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public class Pool<T> where T : class, new()
    {
        private readonly Stack<T> stack = new Stack<T>();

        public T Get()
        {
            if (stack.Count > 0)
            {
                return stack.Pop();
            }

            return new T();
        }

        public void Recycle(T obj)
        {
            stack.Push(obj);
        }
    }
}
