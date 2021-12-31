using System.Collections;
using System.Collections.Generic;
using AillieoUtils.MonteCarloTreeSearch;

namespace Sample
{
    public static class StatePool
    {
        private static readonly Pool<GomokuStateWrapper> statePool = new Pool<GomokuStateWrapper>(32768); // 32768,65536,131072
        private static readonly Pool<List<GomokuStateWrapper>> stateListPool = new Pool<List<GomokuStateWrapper>>(8);
        private static readonly Pool<HashSet<int>> actionsPool = new Pool<HashSet<int>>(8);

        public static GomokuStateWrapper GetState()
        {
            return statePool.Get();
        }

        public static void RecycleState(GomokuStateWrapper toRecycle)
        {
            toRecycle.Reset();
            statePool.Recycle(toRecycle);
        }

        public static List<GomokuStateWrapper> GetStateList()
        {
            return stateListPool.Get();
        }

        public static void RecycleStateList(List<GomokuStateWrapper> toRecycle)
        {
            //foreach (var s in toRecycle)
            //{
            //    RecycleState(s);
            //}
            toRecycle.Clear();
            stateListPool.Recycle(toRecycle);
        }

        public static HashSet<int> GetActionSet()
        {
            return actionsPool.Get();
        }

        public static void RecycleActionSet(HashSet<int> toRecycle)
        {
            toRecycle.Clear();
            actionsPool.Recycle(toRecycle);
        }
    }
}
