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
        private MonteCarloTree tree;

        public override Task<int> Play()
        {
            if (tree == null)
            {
                tree = MonteCarloTree.CreateTree(new GoBangStateWrapper(belongingGame.GetCurrentState()));
            }

            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    Node node = tree.Run(5);
                    GoBangStateWrapper gbsw = node.state as GoBangStateWrapper;
                    tcs.SetResult(gbsw.goBangState.lastPlaced);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            return tcs.Task;
        }
    }
}
