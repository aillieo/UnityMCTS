using System.Collections;
using System.Collections.Generic;
using AillieoUtils.MonteCarloTreeSearch;

namespace Sample
{
    public static class StatePool
    {
        private static readonly Pool<GoBangStateWrapper> statePool = new Pool<GoBangStateWrapper>(32768);
        private static readonly Pool<List<GoBangStateWrapper>> stateListPool = new Pool<List<GoBangStateWrapper>>(4);

        public static GoBangStateWrapper GetState()
        {
            return statePool.Get();
        }

        public static void RecycleState(GoBangStateWrapper toRecycle)
        {
            toRecycle.Reset();
            statePool.Recycle(toRecycle);
        }

        public static List<GoBangStateWrapper> GetStateList()
        {
            return stateListPool.Get();
        }

        public static void RecycleStateList(List<GoBangStateWrapper> toRecycle)
        {
            //foreach (var s in toRecycle)
            //{
            //    RecycleState(s);
            //}
            toRecycle.Clear();
            stateListPool.Recycle(toRecycle);
        }
    }
}
