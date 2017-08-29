using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private void Awake()
    {
        InheritedAwake();
    }
    
    public virtual void InheritedAwake()
    {
    }

    // Use this for initialization
    private void Start()
    {
        InheritedStart();
    }

    public virtual void InheritedStart()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        InheritedUpdate();
    }

    public virtual void InheritedUpdate()
    {
    }
}