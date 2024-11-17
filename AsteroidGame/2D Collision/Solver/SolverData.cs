using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Global_Data;


namespace _2d_Objects
{
    public class SurfaceData
    {
        public Vector3 vContactPoint { get; set; }
        public Vector3 vPerpCToCp { get; set; }

        public SurfaceData(Vector3 vContactPoint, Vector3 vPerpCToCp)
        {
            this.vContactPoint = vContactPoint;
            this.vPerpCToCp = vPerpCToCp;
        }
    }


    public class CollisionData
    {
        public RigidBody rigidBody_A    { get; set; }
        public RigidBody rigidBody_B    { get; set; }

        public Vector3  vNormal         { get; set; }
        public Vector3  vTangent        { get; set; }
        public Vector3  vContactPointA  { get; set; }
        public Vector3  vContactPointB  { get; set; }
        public Vector3  vPerpCToCpA     { get; set; }
        public Vector3  vPerpCToCpB     { get; set; }

        public int  featurePairKey      { get; set; }

        public float    impulseN        { get; set; }
        public float    impulseT        { get; set; }
        public float    distance        { get; set; }
        public float    invDenominatorN { get; set; }       //This stores part of the impulse calculation. http://chrishecker.com/images/e/e7/Gdmphys3.pdf ie the deniminator for calc jN.
        public float    invDenominatorT { get; set; }       //This stores part of the impulse calculation. http://chrishecker.com/images/e/e7/Gdmphys3.pdf ie the deniminator for calc jT.
        public float    frictionCoeff   { get; set; }

        public TouchingContact touchingContact;



        public CollisionData(float distance, Vector3 vNormal, RigidBody rigidBody_A, RigidBody rigidBody_B, int featurePairKey, Vector3 contactpointA, Vector3 contactpointB)
        {
            this.distance = distance;
            this.vNormal = vNormal;
            this.vTangent = Vector3.Cross(this.vNormal, new Vector3(0, 0, -1));
            this.rigidBody_A = rigidBody_A;
            this.rigidBody_B = rigidBody_B;

            this.vContactPointA = contactpointA;
            this.vContactPointB = contactpointB;

            this.impulseN = 0;
            this.impulseT = 0;

            this.featurePairKey = featurePairKey;

            this.vPerpCToCpA = Vector3.Cross(vContactPointA, new Vector3(0, 0, -1));
            this.vPerpCToCpB = Vector3.Cross(vContactPointB, new Vector3(0, 0, -1));

            this.frictionCoeff = Physical_Property.GetFrictionCoefficient(this.rigidBody_A.PhysicalPropertyID.Index, this.rigidBody_B.PhysicalPropertyID.Index);

            //////////////////////////////////////////
            // Calculate inverse denominator for jN //
            //////////////////////////////////////////
                float Aimp = Vector3.Dot(this.vPerpCToCpA, this.vNormal); Aimp *= Aimp; Aimp *= this.rigidBody_A.invMoI_zAxis;
                float Bimp = Vector3.Dot(this.vPerpCToCpB, this.vNormal); Bimp *= Bimp; Bimp *= this.rigidBody_B.invMoI_zAxis;

                this.invDenominatorN = 1 / ((this.rigidBody_A.invMass + this.rigidBody_B.invMass) + Aimp + Bimp);
            //////////////////////////////////////////
            // Calculate inverse denominator for jN //
            //////////////////////////////////////////

            //////////////////////////////////////////
            // Calculate inverse denominator for jT //
            //////////////////////////////////////////
                Aimp = Vector3.Dot(this.vPerpCToCpA, this.vTangent); Aimp *= Aimp; Aimp *= this.rigidBody_A.invMoI_zAxis;
                Bimp = Vector3.Dot(this.vPerpCToCpB, this.vTangent); Bimp *= Bimp; Bimp *= this.rigidBody_B.invMoI_zAxis;

                this.invDenominatorT = 1 / ((this.rigidBody_A.invMass + this.rigidBody_B.invMass) + Aimp + Bimp);
            //////////////////////////////////////////
            // Calculate inverse denominator for jT //
            //////////////////////////////////////////
        }


        public void SwapA_B()
        {
            RigidBody tempRB = this.rigidBody_A;
            this.rigidBody_A = this.rigidBody_B;
            this.rigidBody_B = tempRB;


            Vector3 tempV = this.vContactPointA;
            this.vContactPointA = this.vContactPointB;
            this.vContactPointB = tempV;


            tempV = this.vPerpCToCpA;
            this.vPerpCToCpA = this.vPerpCToCpB;
            this.vPerpCToCpB = tempV;


            this.vNormal = -this.vNormal;
        }

    }






    static partial class Collision_Methods
    {




        public static void resolveCollisions(List<CollisionData> list_CD, int iterations) //pass in a bitfield to tell whether an object is moveable or rotatable.
        {
            float relNv, remove, mag, newImpulse, change;
            Vector3 n, imp,relVel;
            float invTimeDifference = 1 / Globals.timeDifference;
            CollisionData con;



            //Pre solve
            for (int i = 0; i < list_CD.Count; ++i)
            {
                if (list_CD[i].touchingContact != null)
                {
                    if (list_CD[i].impulseN == 0) continue;

                    imp = list_CD[i].vNormal * list_CD[i].impulseN;
                    imp += list_CD[i].vTangent * list_CD[i].impulseT;


                    list_CD[i].rigidBody_A.motion.vVelocityPS += imp * list_CD[i].rigidBody_A.invMass;
                    list_CD[i].rigidBody_B.motion.vVelocityPS -= imp * list_CD[i].rigidBody_B.invMass;

                    list_CD[i].rigidBody_A.motion.zRotPS += Vector3.Dot(imp, list_CD[i].vPerpCToCpA) * list_CD[i].rigidBody_A.invMoI_zAxis;
                    list_CD[i].rigidBody_B.motion.zRotPS -= Vector3.Dot(imp, list_CD[i].vPerpCToCpB) * list_CD[i].rigidBody_B.invMoI_zAxis;
                }
			}






            //Solve
            for (int j = 0; j < iterations; ++j)
            {
                for (int i = 0; i < list_CD.Count; ++i)
                {
                    con = list_CD[i];
                    n = con.vNormal;

                    //if (con.impulseN == 0 && j !=0) continue;
                    relVel = ((con.rigidBody_B.motion.vVelocityPS) + (con.vPerpCToCpB * (con.rigidBody_B.motion.zRotPS))) - ((con.rigidBody_A.motion.vVelocityPS) + (con.vPerpCToCpA * (con.rigidBody_A.motion.zRotPS)));
                    
                    relNv = Vector3.Dot(relVel, n);                                         // get all of relative normal velocity
                    remove = relNv + Math.Max(con.distance * invTimeDifference, 0);        //Was: remove = relNv + Math.Max(con.distance / Globals.timeDifference, 0);

                        


                    // compute impulse
                    mag = remove * con.invDenominatorN;


                    newImpulse = Math.Min(mag + con.impulseN, 0);
                    change = newImpulse - con.impulseN;
                    con.impulseN = newImpulse;

                    if (change == 0) continue;

                    imp = con.vNormal * change;


                    con.rigidBody_A.motion.vVelocityPS += imp * con.rigidBody_A.invMass;
                    con.rigidBody_B.motion.vVelocityPS -= imp * con.rigidBody_B.invMass;

                    con.rigidBody_A.motion.zRotPS += Vector3.Dot(imp, con.vPerpCToCpA) * con.rigidBody_A.invMoI_zAxis;
                    con.rigidBody_B.motion.zRotPS -= Vector3.Dot(imp, con.vPerpCToCpB) * con.rigidBody_B.invMoI_zAxis;

                    //Collision_Methods._vectorsToDraw.Add(new Line(con.rigidBody_B.transform.vPosition, con.rigidBody_B.transform.vPosition - relVel, Color.White, 5));

                    //Friction http://gamedev.tutsplus.com/tutorials/implementation/how-to-create-a-custom-2d-physics-engine-friction-scene-and-jump-table/
                    //If friction fucks up.. its probs to do with the distance. ie if i have just "con.distance <= 0" no fric will be applied even though it should be
                    if (con.distance <= 0.001f && con.distance >= -0.4f)
                    {
                        //relVel = ((con.rigidBody_B.motion.vVelocityPS) ) - ((con.rigidBody_A.motion.vVelocityPS) );
                        //relVel = ((con.rigidBody_B.motion.vVelocityPS) + (con.vPerpCToCpB * (con.rigidBody_B.motion.motion.zRotPS))) - ((con.rigidBody_A.motion.vVelocityPS) + (con.vPerpCToCpA * (con.rigidBody_A.motion.zRotPS)));
                        //relVel *= Globals.timeDifference;
                        



                        //Collision_Methods._vectorsToDraw.Add(new Line(con.rigidBody_B.transform.vPosition, con.rigidBody_B.transform.vPosition  -relVel, Color.Blue, 15));
                        //Collision_Methods._vectorsToDraw.Add(new Line(con.rigidBody_B.transform.vPosition + con.vContactPointB, con.rigidBody_B.transform.vPosition + con.vContactPointB + (con.vTangent), Color.Red, 5)); 


                        float jt = Vector3.Dot(relVel, con.vTangent);// *(1 + e);
                        jt *= con.invDenominatorT;




                        float absMag = con.impulseN * con.frictionCoeff;         //replace 0.5f with friction calc from both objects.
                        jt += con.impulseT;
                        if (absMag > 0)                         // Dont think absMag is ever positive. might be able to remove  and just leave the else statement.(CHANGE)
                        {
                            if (jt  > absMag) jt = absMag; 
                            if (jt  < -absMag) jt = -absMag;
                        }
                        else
                        {
                            if (jt  > -absMag) jt = -absMag;
                            if (jt  < absMag) jt = absMag;
                        }




                        newImpulse = jt;

                        change = newImpulse - con.impulseT;
                        imp = change * con.vTangent;
                        con.impulseT = newImpulse;





    



                        







                        con.rigidBody_A.motion.vVelocityPS += imp * con.rigidBody_A.invMass;
                        con.rigidBody_B.motion.vVelocityPS -= imp * con.rigidBody_B.invMass;

                        con.rigidBody_A.motion.zRotPS += Vector3.Dot(imp, con.vPerpCToCpA) * con.rigidBody_A.invMoI_zAxis;
                        con.rigidBody_B.motion.zRotPS -= Vector3.Dot(imp, con.vPerpCToCpB) * con.rigidBody_B.invMoI_zAxis;
                    }
                }
            }



            // Anti Penetration
            for (int i = 0; i < list_CD.Count; ++i)
            {
                con = list_CD[i];

                if ( con.distance < 0)
			    {
                    remove = con.distance * 10;     //(CHANGE) can get rid of "* 10" doesnt seem to make much difference.


                    // compute impulse
                    mag = remove * con.invDenominatorN;

                    imp = con.vNormal * mag;



                    con.rigidBody_A.motion.vVelocityPS_AntiPenetration += imp * con.rigidBody_A.invMass;
                    con.rigidBody_B.motion.vVelocityPS_AntiPenetration -= imp * con.rigidBody_B.invMass;

                    con.rigidBody_A.motion.zRotPS_AntiPenetration += Vector3.Dot(imp, con.vPerpCToCpA) * con.rigidBody_A.invMoI_zAxis;
                    con.rigidBody_B.motion.zRotPS_AntiPenetration -= Vector3.Dot(imp, con.vPerpCToCpB) * con.rigidBody_B.invMoI_zAxis;
			    }

                if (list_CD[i].touchingContact != null)
                {
                    list_CD[i].touchingContact.m_accumulatedNormalImpulse = list_CD[i].impulseN;
                    list_CD[i].touchingContact.m_accumulatedTangentImpulse = list_CD[i].impulseT;
                }
            }










        }
    }
}
