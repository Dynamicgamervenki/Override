using System;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class SplineJunction : MonoBehaviour
{
   [SerializeField] public SplineContainer leftSplineContainer;
   [SerializeField] public SplineContainer rightSplineContainer;
       
   [SerializeField] private int leftPriority = 1;
   [SerializeField] private int rightPriority = 1;
   public float direction = 1;

   public SplineContainer GetLeftSplineContaier() => leftSplineContainer;
   public SplineContainer GetRightSplineContaier() => rightSplineContainer;
   
   public SplineContainer GetRandomSplineContainer()
   {
       var total = leftPriority + rightPriority;
       var roll = Random.Range(0, total);

       return roll < leftPriority ? leftSplineContainer : rightSplineContainer;
   }

}