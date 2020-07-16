using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyScripts
{
    public class Arrow : MonoBehaviour
    {
        public GameObject target;

        public GameObject perfectEffect;

        public GameObject missEffect;

        public Text combo;

        public Text score;

        public KeyCode keyCode;

        public bool isPressEnable = false;

        public float speed = 0.5f;

        private Vector3 direction;

        // Start is called before the first frame update
        void Start()
        {
            if (keyCode == KeyCode.RightArrow || keyCode == KeyCode.D)
            {
                gameObject.name = "Right Arrow";
            }
            else if (keyCode == KeyCode.DownArrow || keyCode == KeyCode.S)
            {
                gameObject.name = "Down Arrow";
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
            else if (keyCode == KeyCode.LeftArrow || keyCode == KeyCode.A)
            {
                gameObject.name = "Left Arrow";
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            }
            else if (keyCode == KeyCode.UpArrow || keyCode == KeyCode.W)
            {
                gameObject.name = "Up Arrow";
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }

            direction = target.transform.position - transform.position;

            // CreateEffect(perfectEffect);
        }

        // Update is called once per frame
        void Update()
        {
            var step = speed * Time.deltaTime;
            transform.position += step * direction;

            if (isPressEnable && Input.anyKey)
            {
                if (Input.GetKey(keyCode))
                {
                    HandleSuccessEvent();
                }
                else
                {
                    HandleFailEvent();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            isPressEnable = true;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (isPressEnable)
            {
                HandleFailEvent();
            }
        }

        private void CreateEffect(GameObject effect)
        {
            GameObject instantiate = Instantiate(effect, target.transform);
            instantiate.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            instantiate.transform.localPosition = transform.position;
            instantiate.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        private void HandleSuccessEvent()
        {
            Debug.Log("success");
            isPressEnable = false;
            CreateEffect(perfectEffect);

            // 连击
            var comboText = combo.GetComponent<Text>().text;
            combo.GetComponent<Text>().text = (int.Parse(comboText) + 1).ToString("000");

            // 分数
            var scoreText = score.GetComponent<Text>().text;
            score.GetComponent<Text>().text = (int.Parse(scoreText) + 100).ToString("000000");

            Destroy(gameObject);
        }

        private void HandleFailEvent()
        {
            Debug.Log("fail");
            CreateEffect(missEffect);
            // 连击
            combo.GetComponent<Text>().text = "000";

            Destroy(gameObject);
        }
    }
}