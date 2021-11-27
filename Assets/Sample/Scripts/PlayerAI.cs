using AillieoUtils.GoBang;
using AillieoUtils.MonteCarloTreeSearch;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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

            while (true)
            {
                int x = Random.Range(0, GoBangGame.dimension);
                int y = Random.Range(0, GoBangGame.dimension);
                int index = x + y * GoBangGame.dimension;

                GoBangState state = this.belongingGame.GetCurrentState();

                if (state.boardState[index] == BoardValue.Empty)
                {
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        // Node node = tree.Run(10);
                        // if (node.state is GoBangStateWrapper gbsw)
                        // {
                        //    tcs.SetResult(gbsw.goBangState.lastPlaced);
                        // }
                        // else
                        // {
                        //     tcs.SetResult(index);
                        // }

                        Thread.Sleep(1000);
                        tcs.SetResult(index);
                    });
                    break;
                }
            }

            return tcs.Task;
        }
    }
}
