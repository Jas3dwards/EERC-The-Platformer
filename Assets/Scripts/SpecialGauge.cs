using UnityEngine;
using UnityEngine.UI;

public class SpecialGauge : MonoBehaviour
{
    public Slider slider;

    public void SetMaxEnergy(int energy)
    {
        slider.maxValue = energy;
        slider.value = 0;
    }

    public void SetEnergy(int energy)
    {
        slider.value = energy;
    }
}
