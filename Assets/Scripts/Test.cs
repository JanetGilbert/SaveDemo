using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct Point
{
    int x;
    int y;

    public Point(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public static Point operator +(Point left, Point right)
    {
        return new Point(left.x + right.x, left.y + right.y);
    }

    public override string ToString()
    {
        return x.ToString() + " " + y.ToString();
    }
}



public class Test : MonoBehaviour
{
    
    void Start()
    {
        Point pt1 = new Point(5, 10);
        Point pt2 = new Point(2, 2);

        Point pt3 = pt1 + pt2;

        Debug.Log(pt3);
    }

   
    void Update()
    {
        
    }
}
