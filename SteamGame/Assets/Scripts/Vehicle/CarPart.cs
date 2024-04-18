using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPart : MonoBehaviour
{
    public PartType partType;
   public enum PartType
    {
        Boiler,
        Tire,
        WaterTank,
        FireBox,
        Axle,
        None
    }
    public PartType attachableType;
}
