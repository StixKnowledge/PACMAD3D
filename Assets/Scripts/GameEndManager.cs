using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameEndManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    double videoLength;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        videoLength = videoPlayer.length;
        StartCoroutine(WaitForVideoToEnd());
    }

    IEnumerator WaitForVideoToEnd()
    {
        float timeElapsed = 0f;

        Debug.Log("Waiting for " + videoLength + " seconds");

        while (timeElapsed < videoLength)
        {
            timeElapsed += Time.deltaTime;
            yield return null; // wait for next frame
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
}
