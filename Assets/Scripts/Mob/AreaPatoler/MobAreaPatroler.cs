using UnityEngine;

public class MobAreaPatroler : MonoBehaviour
{
    public GameObject patrolingArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool isPlayerInsidePatrolingArea()
    {
        return patrolingArea.GetComponent<PatrolingArea>().isPlayerInside;
    }
}
