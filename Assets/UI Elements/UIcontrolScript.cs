using UnityEngine;

public class UIcontrolScript : MonoBehaviour
{
    [SerializeField] GameObject pausePanelGO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1.0f;
        pausePanelGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            pausePanelGO.SetActive(true);
        }
    }
}
