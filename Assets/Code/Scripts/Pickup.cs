using Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickupInstantiateData : IPoolableInstantiateData 
{
    public PickupInstantiateData(Color color) 
    {
        this.color = color;
    }

    private Color color;

    public Color Color { get { return color; } }
}


public class Pickup : Poolable
{
    // TODO: reference visual

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Init(IPoolableInstantiateData stats)
    {
        PickupInstantiateData data = stats as PickupInstantiateData;
    }

    public override void Reset()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: Disappear on contact with player
    }
}
