using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shelf : Wall
{
    public GameObject[] ShelfVariants;
    public GameObject CurrentShelfObject;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
    }
}
