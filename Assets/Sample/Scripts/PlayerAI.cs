using System;
using AillieoUtils.Gomoku;
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
            MonteCarloTree<GomokuStateWrapper> tree = MonteCarloTree<GomokuStateWrapper>.CreateTree(this, new GomokuStateWrapper(belongingGame.GetCurrentState()));
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
                    Node<GomokuStateWrapper> node = tree.Run(1);
                    GomokuStateWrapper gbsw = node.state;
                    tcs.SetResult(gbsw.gomokuState.lastPlaced);
                });
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                tcs.SetException(e);
            }
            finally
            {
                MonteCarloTree<GomokuStateWrapper>.Recycle(tree);
            }
            return tcs.Task;
        }
    }
}
