using System.Collections;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        StartCoroutine(Blowing());
    }
    private void SetFacingDirection(string direction)
    {
        if (tag == "WindLeft")
        {
            Debug.Log("left");
            gameObject.transform.localScale = new Vector3(-6, 6, 1);
        }
        if (tag == "WindRight")
        {
            gameObject.transform.localScale = new Vector3(6,6,1);
        }
    }
    IEnumerator Blowing()
    {

        
        animator.SetBool("Is_Blowing", true);
        GetComponent<Collider2D>().enabled = true;
        
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        animator.SetBool("Is_Blowing", false);
        GetComponent<Collider2D>().enabled = false;

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);
        StartCoroutine(Blowing());
    }
    // Update is called once per frame
    void Update()
    {

    }
}
