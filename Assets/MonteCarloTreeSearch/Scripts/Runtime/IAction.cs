using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public interface IAction<T> where T : IAction<T>
    {
    }
}
