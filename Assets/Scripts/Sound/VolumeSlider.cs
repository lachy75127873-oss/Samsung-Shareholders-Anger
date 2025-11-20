using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] AudioMixerGroupName groupName;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = ManagerRoot.Instance.audioManager;
        slider.value = audioManager.GetVolume(groupName);
    }
    private void OnEnable()
    {
        slider.onValueChanged.AddListener(OnSliderChanged);
    }
    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
    }
    private void OnSliderChanged(float value)
    {
        audioManager.SetVolume(groupName, value);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        slider = GetComponent<Slider>();
        slider.minValue = -30;
        slider.maxValue = 5;
    }
#endif
}
