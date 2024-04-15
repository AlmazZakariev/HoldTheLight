using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class SpawnOnCheckpoint : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> checkpoints;
    void Start()
    {
        //string checkpointName = PlayerPrefs.GetString("lastCheckpoint");
        //Debug.Log(checkpointName);
        //if (checkpointName != null && checkpointName.Length > 0)
        //{
        //    var cp = checkpoints.Find(x => x.name == checkpointName);
        //    if (cp == null)
        //    {
        //        Debug.LogError("checkpoint with name " + checkpointName + " not found");
        //        return;
        //    }

        //    transform.position = cp.transform.position;
        //}
    }
    private void Awake()
    {
        string checkpointName = PlayerPrefs.GetString("lastCheckpoint");
        Debug.Log(checkpointName);
        if (checkpointName != null && checkpointName.Length > 0)
        {
            var cp = checkpoints.Find(x => x.name == checkpointName);
            if (cp == null)
            {
                Debug.LogError("checkpoint with name " + checkpointName + " not found");
                return;
            }

            transform.position = cp.transform.position;
        }
    }
}
