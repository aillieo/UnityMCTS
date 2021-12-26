using System;
using AillieoUtils.GoBang;
using AillieoUtils.MonteCarloTreeSearch;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Sample
{
    public class PlayerAI : Player, IAgent
    {
        public float maxPlanningSeconds = 300;
        public int workingThreads = 4;

        public override float maxOperationSeconds => maxPlanningSeconds + 1;

        public override Task<int> Play()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            MonteCarloTree<GoBangStateWrapper> tree = MonteCarloTree<GoBangStateWrapper>.CreateTree(this, new GoBangStateWrapper(belongingGame.GetCurrentState()));
            try
            {
                int ms = (int)Math.Round(maxPlanningSeconds * 1000);
                IEnumerable<Task> tasks = Enumerable.Range(1, workingThreads).Select(i => Task.Run(() => tree.Run(ms)));

                Task taskAll = Task.WhenAll(tasks);

                if (taskAll.Exception != null)
                {
                    throw taskAll.Exception;
                }

                taskAll.ContinueWith(o =>
                {
                    Node<GoBangStateWrapper> node = tree.Run(1);
                    GoBangStateWrapper gbsw = node.state;
                    tcs.SetResult(gbsw.goBangState.lastPlaced);
                });
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                tcs.SetException(e);
            }
            finally
            {
                MonteCarloTree<GoBangStateWrapper>.Recycle(tree);
            }
            return tcs.Task;
        }
    }
}
