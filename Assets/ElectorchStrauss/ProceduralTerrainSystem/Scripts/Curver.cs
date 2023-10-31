using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
public class Curver : MonoBehaviour {
     //arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
     public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve,int smoothness){
         List<Vector3> points;
         List<Vector3> curvedPoints;
         int pointsLength = 0;
         int curvedLength = 0;
         
         if(smoothness < 1) smoothness = 1;
         
         pointsLength = arrayToCurve.Length;
         curvedLength = (pointsLength*Mathf.RoundToInt(smoothness))-1;
         
         curvedPoints = new List<Vector3>(curvedLength*3);
         
         float t = 0.0f;
         
         for(int i = 0; i <= curvedLength; i++){
             
             t = Mathf.InverseLerp(0,curvedLength,i);
             
             points = new List<Vector3>(arrayToCurve);
             
             for(int j = pointsLength-1; j > 0; j--){
                 for (int k = 0; k < j; k++){
                     points[k] = (1-t)*points[k] + t*points[k+1];
                 }
             }
             
             curvedPoints.Add(points[0]);
         }
         
         return(curvedPoints.ToArray());
     }
 }