using UnityEngine;
using ProtoCasual.Core.GameLoop;
using ProtoCasual.Core.Managers;

namespace ProtoCasual.Sample.GameModes
{
    public class RunnerController : MonoBehaviour
    {
        [SerializeField] private DifficultySystem difficulty;

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing)
                return;

            transform.Translate(Vector3.forward *
                difficulty.CurrentSpeed * Time.deltaTime);
        }
    }
}
