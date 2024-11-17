
using Microsoft.Xna.Framework;
namespace _2DLevelCreator;

public class LineEquation
{
    public float multiplier;
    public float constant;
    public bool inTermsOf_X = true;

    //Constructor
    public LineEquation(Vector3 point1, Vector3 point2)
    {

        //Check if the line is Vertical. Means the equation has to be in terms of Y. Ie  X = 4 + 0Y
        if (point1.X == point2.X)       //Ie the line is Vertical
        {
            this.inTermsOf_X = false;
            this.multiplier = 0;
            this.constant = point1.X;
            return;
        }

        //Check if the line is Horizontal. Means the equation has to be in terms of X. Ie  Y = 4 + 0X
        if (point1.Y == point2.Y)       //Ie the line is Horizontal
        {
            this.multiplier = 0;
            this.constant = point1.Y;
            return;
        }

        this.multiplier = (point2.Y - point1.Y) / (point2.X - point1.X);
        this.constant = point1.Y - (this.multiplier * point1.X);


    }

    public static bool LineIntersectionPoint(LineEquation lineEquation1, LineEquation lineEquation2, ref Vector3 intersectPoint)
    {
        //  y = ( xMultiplier1 * X) + constant1       And     y = ( xMultiplier2 * X) + constant2
        //  ( xMultiplier1 * X) -   ( xMultiplier2 * X) = + constant2 - constant1

        
        if (!lineEquation1.inTermsOf_X && !lineEquation2.inTermsOf_X) return false;     // 2 Vertical Lines ( eg x = 4 and x =2 ). No intersection.
        if (lineEquation1 == lineEquation2) return false;                               // Infinite intersections

        float multiplier = lineEquation1.multiplier - lineEquation2.multiplier;     //Get the combined multiplier


        if (!lineEquation2.inTermsOf_X)     //If it is not in terms of X then line 2 is Vertical
        {
            multiplier = lineEquation1.multiplier + lineEquation2.multiplier;   //One will have 0 multiplier and one will have a value. (Easier than doing "if(lineEquation1.multiplier !=0) multiplier = lineEquation1.multiplier; else multiplier = lineEquation2.multiplier
            intersectPoint.X = lineEquation2.constant;
            intersectPoint.Y = (multiplier * intersectPoint.X) + lineEquation1.constant;
        }
        else
        {
            if (!lineEquation1.inTermsOf_X)    //If it is not in terms of X then line 1 is Vertical
            {
                multiplier = lineEquation1.multiplier + lineEquation2.multiplier;   //One will have 0 multiplier and one will have a value. (Easier than doing "if(lineEquation1.multiplier !=0) multiplier = lineEquation1.multiplier; else multiplier = lineEquation2.multiplier
                intersectPoint.X = lineEquation1.constant;
                intersectPoint.Y = (multiplier * intersectPoint.X) + lineEquation2.constant;
            }
            else
            {
                if (multiplier == 0) //Lines are parallel so no intersection
                    return false;

                intersectPoint.X = (lineEquation2.constant - lineEquation1.constant ) / multiplier;
                intersectPoint.Y = (lineEquation1.multiplier * intersectPoint.X) + lineEquation1.constant;
            }
        }
        
        return true;
    }
}
