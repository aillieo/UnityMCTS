using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AillieoUtils.GoBang
{
    public class PlayerHuman : Player
    {
        public override Task<int> Play()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            Action<Vector2> onClick = default;
            onClick = pos =>
            {
                int index = ClickIndexToIndex(pos);
                if (ValidClick(index))
                {
                    InputDispatcher.Instance.onInputRequest -= onClick;
                    tcs.SetResult(index);
                }
            };

            InputDispatcher.Instance.onInputRequest += onClick;
            return tcs.Task;
        }

        private int ClickIndexToIndex(Vector2 clickPos)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(clickPos);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 position = ray.GetPoint(enter);
                int x = Mathf.RoundToInt(position.x);
                int y = Mathf.RoundToInt(position.z);
                return x + GoBangGame.dimension * y;
            }
            else
            {
                return -1;
            }
        }

        private bool ValidClick(int index)
        {
            if (index < 0 || index >= GoBangGame.dimension * GoBangGame.dimension)
            {
                return false;
            }

            GoBangState state = belongingGame.GetCurrentState();
            return state.boardState[index] == BoardValue.Empty;
        }
    }
}
