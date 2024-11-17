using Microsoft.Xna.Framework;
using Asteroid_Game;


namespace _2d_Objects
{
    static partial class Collision_Methods
    {
        public static CollisionDataBasic GetSolver_PointVsLine(Vector3 point, RainbowLine line)
        {
            Vector3 dist = point - line.position;                           //Puts the points position relative to the line's position

            Vector3 lineNormalised = line.GetLineVector();
            lineNormalised.Normalize();

            float dot = Vector3.Dot(lineNormalised, dist);

            Vector3 projectedPointOnLine = (dot * lineNormalised);


            if (line.GetLineVector().X > 0)                                 //Clamp projectedPoint to the Line
            {
                if (projectedPointOnLine.X > line.GetLineVector().X)
                    projectedPointOnLine.X = line.GetLineVector().X;
                else
                    if (projectedPointOnLine.X < -line.GetLineVector().X)
                        projectedPointOnLine.X = -line.GetLineVector().X;
            }
            else
                if (projectedPointOnLine.X > -line.GetLineVector().X)
                    projectedPointOnLine.X = -line.GetLineVector().X;
                else
                    if (projectedPointOnLine.X < line.GetLineVector().X)
                        projectedPointOnLine.X = line.GetLineVector().X;

            if (line.GetLineVector().Y > 0)
            {
                if (projectedPointOnLine.Y > line.GetLineVector().Y)
                    projectedPointOnLine.Y = line.GetLineVector().Y;
                else
                    if (projectedPointOnLine.Y < -line.GetLineVector().Y)
                        projectedPointOnLine.Y = -line.GetLineVector().Y;
            }
            else
            {
                if (projectedPointOnLine.Y > -line.GetLineVector().Y)
                    projectedPointOnLine.Y = -line.GetLineVector().Y;
                else
                    if (projectedPointOnLine.Y < line.GetLineVector().Y)
                        projectedPointOnLine.Y = line.GetLineVector().Y;
            }

            
            dist = dist - projectedPointOnLine;
            float distance = dist.Length();


            Vector3 finalNormal = new Vector3(-lineNormalised.Y, lineNormalised.X, 0);              //Perpendicular to line
            dot = Vector3.Dot(dist, finalNormal);
            if (dot < 0) finalNormal *= -1;                                                         //Flip normal direction to face the point

           //Collision_Methods._contactPoints.Add(new Circle(projectedPointOnLine + line.position, 0.1f,Circle.DrawStyle.Both, Color.Red, Color.Red));

            CollisionDataBasic cdb= new CollisionDataBasic(distance, finalNormal, point, line, projectedPointOnLine + line.position);
            return cdb;
        }
    }
}


