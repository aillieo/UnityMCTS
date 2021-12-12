//#define BLOCKING
using System;
using AillieoUtils.GoBang;
using AillieoUtils.MonteCarloTreeSearch;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sample
{
    public class PlayerAI : Player
    {
        public override Task<int> Play()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
#if BLOCKING
            InternalPlay(tcs);
#else
            ThreadPool.QueueUserWorkItem(o =>
            {
                InternalPlay(tcs);
            });
#endif
            return tcs.Task;
        }

        private void InternalPlay(TaskCompletionSource<int> tcs)
        {
            MonteCarloTree<GoBangStateWrapper> tree = MonteCarloTree<GoBangStateWrapper>.CreateTree(new GoBangStateWrapper(belongingGame.GetCurrentState()));
            try
            {
                Node<GoBangStateWrapper> node = tree.Run(60_000);
                GoBangStateWrapper gbsw = node.state;
                tcs.SetResult(gbsw.goBangState.lastPlaced);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                tcs.SetException(e);
            }
            finally
            {
                //MonteCarloTree<GoBangStateWrapper>.Recycle(tree);
            }
        }
    }
}
