using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Animator anim;
    public bool transition;
    public bool win;
    public Canvas canvas;

    public GameObject button1;
    public GameObject button2;
    // Start is called before the first frame update
    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
        {
            button1.SetActive(false);
            button2.SetActive(false);
            if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
            {
                StartCoroutine(deathScreen());
            }
        }   else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            button1.SetActive(true);
            button2.SetActive(false);
        }   else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
        {
            canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            button1.SetActive(false);
            button2.SetActive(true);
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3))
        {
            canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            button1.SetActive(false);
            button2.SetActive(true);
        }   else {
            button1.SetActive(false);
            button2.SetActive(false);
        }

        if (win)
        {
            StartCoroutine(Win());
            win = false;
        }

    }

    IEnumerator deathScreen()
    {
        
        anim.SetBool("fadeout", true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Death");
        anim.SetBool("fadeout", false);
    }

    public void PlayButton()
    {
        if (!transition)
        {
            StartCoroutine(Play());
        }
    }

    public IEnumerator Win()
    {
        transition = true;
        anim.SetBool("fadeout", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("fadeout", false);
        SceneManager.LoadScene("Win");
        transition = false;
    }
    public IEnumerator Play()
    {
        transition = true;
        anim.SetBool("fadeout", true);
        yield return new WaitForSeconds(1);
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0)){
            SceneManager.LoadScene("IntroText");
                
            anim.SetBool("fadeout", false);
            yield return new WaitForSeconds(3);
            
            anim.SetBool("fadeout", true);
            yield return new WaitForSeconds(1);
        }
        anim.SetBool("fadeout", false);
        SceneManager.LoadScene("Game");
        transition = false;
    }
}
