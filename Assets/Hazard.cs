using UnityEngine;
using UnityEngine.SceneManagement;
public class Hazard : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         RestartLevel();
      }
   }
   private void RestartLevel()
   {
      Scene currentScene = SceneManager.GetActiveScene();
      SceneManager.LoadScene(currentScene.name);
   }
}
