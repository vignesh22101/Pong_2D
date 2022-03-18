using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AgeHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private Slider ageSlider;
    [SerializeField] private int nextSceneIndex;

    private GameObject agePanel;
    private int age;

    private void Awake()
    {
        agePanel = transform.GetChild(0).gameObject;

        if (PlayerPrefs.HasKey("Age"))
        {
            agePanel.SetActive(false);
            Load_NextScene();
        }
        else
        {
            agePanel.SetActive(true);
            OnSlideValueChanged();
        }
    }

    public void OnSlideValueChanged()
    {
        age = (int)ageSlider.value;
        ageText.text = "AGE : " + age.ToString();
    }

    public void ProceedBtn_Clicked()
    {
        PlayerPrefs.SetInt("Age", age);
        Load_NextScene();
    }

    private void Load_NextScene()
    {
        SceneManager.LoadScene(nextSceneIndex);
    }
}

