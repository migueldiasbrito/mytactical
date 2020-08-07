using UnityEngine;
using UnityEngine.SceneManagement;

namespace mdb.Tools
{
    public class LoadScene : MonoBehaviour
    {
        public void LoadSceneByIndex(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}