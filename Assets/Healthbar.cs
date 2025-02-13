using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Slider))]
public class Healthbar : MonoBehaviour
{
    [HideInInspector]
    public Slider slider;
    public float maxvalue;

    public Image bar;
    public Image fill;
    public float transparency;

    [Tooltip("is the healthbar suppost")] public bool isPlayer;

    void Start()
    {
        slider = GetComponent<Slider>();
        if (!bar || !fill)
        {
            Debug.LogError("Health bar missing and image variable, wake up");
        }
    }

    public void ChangeValue(float value)
    {
        slider.value = Mathf.InverseLerp(0, maxvalue, value);
        transparency = 1;
    }

    //the same as change value but without setting the trasparency
    public void SmugleValue(float value, float max)
    {
        maxvalue = max;
        slider.value = Mathf.InverseLerp(0, maxvalue, value);
    }

    public void Update()
    {
        if (isPlayer)
        {
            if (transparency > 0f)
            {
                transparency -= Mathf.Pow(Time.deltaTime, 1.1f);
            }

            bar.color = new Color(bar.color.r, bar.color.g, bar.color.b, transparency);
            fill.color = new Color(fill.color.r, fill.color.g, fill.color.b, transparency);
        }
    }
}
