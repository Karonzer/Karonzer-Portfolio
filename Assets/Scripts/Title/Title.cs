using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
	public void OnStartButton()
	{
		SceneManager.LoadScene("LoadingScene");
	}
}
