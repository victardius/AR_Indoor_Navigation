using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds menu related methods used in the options panel.
/// </summary>
public class MenuController : MonoBehaviour
{
    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Restarts the scene.
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
