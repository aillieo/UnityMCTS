using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AillieoUtils.GoBang
{
    public class InputDispatcher : MonoBehaviour
    {
        private static InputDispatcher ins;

        public event Action<Vector2> onInputRequest;

        public static InputDispatcher Instance
        {
            get
            {
                if (ins == null)
                {
                    GameObject go = new GameObject($"[{nameof(InputDispatcher)}]");
                    ins = go.AddComponent<InputDispatcher>();
                    DontDestroyOnLoad(go);
                }

                return ins;
            }
        }

        private void Awake()
        {
            if (ins != null && ins != this)
            {
                Destroy(this);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                onInputRequest?.Invoke(Input.mousePosition);
            }
        }
    }
}
