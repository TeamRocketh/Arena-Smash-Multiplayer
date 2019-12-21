using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKey && Time.timeSinceLevelLoad > 2)
        {
            ScoreManager.instance.ResetScore();
            SceneManager.LoadScene("CircularArenaGame");
        }
    }
}
