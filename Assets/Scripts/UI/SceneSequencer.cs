using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSequencer : MonoBehaviour
{
    [SerializeField] private string sceneName;
    
    public void OnButtonPressed()
    {
        SceneManager.LoadScene(sceneName);
    }
}
