using System.Collections;
using System.Collections.Generic;
using MyScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour
{
    public GameObject arrowPrefab;

    private GameObject leftCircle;

    private GameObject rightCircle;

    private Text combo;

    private Text score;

    public float period;

    private AudioSource audioSource;

    private float timer = 0;

    private void GenerateArrows(GameObject target)
    {
        var instantiate = Instantiate(arrowPrefab, target.transform, false);
        instantiate.transform.localScale = new Vector3(1f, 1f, 1f);
        instantiate.transform.localPosition = new Vector3(0f, Random.value > 0.5f ? 170f : -170f, 0f);

        var script = instantiate.GetComponent<Arrow>();
        script.target = target;
        script.combo = combo;
        script.score = score;

        if (Random.value < 0.25)
            script.keyCode = KeyCode.RightArrow;
        else if (Random.value < 0.5)
            script.keyCode = KeyCode.DownArrow;
        else if (Random.value < 0.75)
            script.keyCode = KeyCode.LeftArrow;
        else
            script.keyCode = KeyCode.UpArrow;
    }


    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();

        leftCircle = GameObject.Find("Target Circle Left");
        rightCircle = GameObject.Find("Target Circle Right");
        combo = GameObject.Find("Combo Count").GetComponent<Text>();
        score = GameObject.Find("Score Count").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying)
        {
            timer += Time.deltaTime;
            if (timer > period)
            {
                timer = 0;
                GenerateArrows(Random.value > 0.5f ? leftCircle : rightCircle);
            }
        }
        else
        {
            SceneManager.LoadScene("MyScenes/MenuScene");
        }
    }
}