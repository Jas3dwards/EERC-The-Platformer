using UnityEngine;

public class PlayerSpecial : MonoBehaviour
{
    public int maxEnergy = 100;
    private int currentEnergy;
    public SpecialGauge specialGauge;
    private float timer = 0f;

    private void Start()
    {
        currentEnergy = 0;
        specialGauge.SetMaxEnergy(maxEnergy);
    }

    private void Update()
    {
        // 10 seconds max
        timer += Time.deltaTime;
        if (timer >= 10f)
        {
            currentEnergy = maxEnergy;
            specialGauge.SetEnergy(currentEnergy);
            timer = 0f;
        }

        // E ultimate key
        if (Input.GetKeyDown(KeyCode.E) && currentEnergy >= maxEnergy)
        {
            Debug.Log("Ultimate Activated!");
            currentEnergy = 0;
            specialGauge.SetEnergy(currentEnergy);

            
            StartCoroutine(UltimateEffect());
        }
    }

    private System.Collections.IEnumerator UltimateEffect()
    {
        
        GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        glow.transform.position = transform.position;
        glow.transform.localScale = new Vector3(2, 2, 2);
        glow.GetComponent<Renderer>().material.color = Color.yellow;
        Destroy(glow.GetComponent<Collider>()); 
        yield return new WaitForSeconds(0.5f);
        Destroy(glow);
    }
}
