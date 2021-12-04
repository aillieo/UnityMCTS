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
            MonteCarloTree<GoBangStateWrapper> tree = MonteCarloTree<GoBangStateWrapper>.CreateTree(new GoBangStateWrapper(belongingGame.GetCurrentState()));

            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    Node<GoBangStateWrapper> node = tree.Run(2000);
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
            });
            return tcs.Task;
        }
    }
}
