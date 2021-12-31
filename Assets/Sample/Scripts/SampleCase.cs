using AillieoUtils.Gomoku;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample
{
    public class SampleCase : MonoBehaviour
    {
        private GomokuGame game;

        private async void Start()
        {
            game = new GomokuGame();

            game.CreatePlayer<PlayerHuman>();
            game.CreatePlayer<PlayerAI>();

            game.Init();

            Player winner = await game.Run();

            Debug.Log("游戏结束 胜利方是" + winner);
        }

        private void OnDrawGizmos()
        {
            if (game != null)
            {
                game.DrawGizmos();
            }
        }
    }
}
