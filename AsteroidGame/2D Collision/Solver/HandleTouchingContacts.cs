using System.Collections.Generic;



namespace _2d_Objects
{
    public class TouchingContactContainer
	{
		public RigidBody rb;
		public int num;
		public TouchingContact[] tcArray;

        public TouchingContactContainer(RigidBody rb, int num, TouchingContact[] tcArray)// array:Array ):void
		{
			this.rb = rb;
            this.num = num;
            this.tcArray = tcArray;
		}
		
	}
    public class TouchingContact
	{
		public int m_featurePairKey;
		public float m_accumulatedNormalImpulse;
		public float m_accumulatedTangentImpulse;
		
		/// <summary>
		/// 
		/// </summary>	
		public TouchingContact( int featurePairKey )
		{
			m_featurePairKey = featurePairKey;
			m_accumulatedNormalImpulse = 0;
			m_accumulatedTangentImpulse = 0;
		}
		
		/// <summary>
		/// Something to identify this contact uniquely
        /// supportIndex is the vert index that would match the featureTouse.m_face (ie the know edge ).
		/// </summary>	
		static public int GenerateFeaturePairKey( FeaturePair featureToUse, int supportIndex )
		{
			return featureToUse.m_fp|( featureToUse.m_face<<2 )|( supportIndex<<16 );
		}


	}

    public class FeaturePair
	{
		public float m_dist;
		public int m_vertex;
		public int m_face;
		public int m_fp;

        public FeaturePair(float dist, int vertex, int face, int fp)
		{
			m_dist = dist;
			m_vertex = vertex;
			m_face = face;
			m_fp = fp;
		}
	}

    public class eFeaturePair
    {
        static public int Undefined = 0;
        static public int VertexBEdgeA = 1;
        static public int VertexAEdgeB = 2;
    }

    static partial class Collision_Methods
    {
        public static void HandleTouchingContacts(RigidBody rbA, RigidBody rbB, List<CollisionData> collisionData, int cdElement)
        {

            if (collisionData[cdElement].distance < 0.0625)    //Check to see if rigidBodies are actually touching.
            {
                AddTouchingContact(rbA, rbB, collisionData, cdElement);
            }
            else
            {
                RemoveTouchingContact(rbA, rbB);
            }

        }

        public static void AddTouchingContact(RigidBody rbA, RigidBody rbB, List<CollisionData> collisionData, int cdElement)
        {
            int element = -1;
            //first check to see if rbA already contains rbB in its touchingContacts List.
            for (int i = 0, count = rbA.touchingContactsContainerList.Count; i < count; ++i)
            {
                if (rbA.touchingContactsContainerList[i].rb == rbB)
                {
                    element = i;
                    break;
                }
            }

            if (element == -1)     //if it doesnt then we need to add it.
            {

                TouchingContact[] tcArray = new TouchingContact[4];
                for (int i = cdElement, count = 0; i < collisionData.Count; ++i)
                {
                    TouchingContact tc = new TouchingContact(collisionData[i].featurePairKey);

                    collisionData[i].touchingContact = tc;  //THIS DOES NOT NEED TO EXIST? (CHANGE)
                    tcArray[count++] = tc;
                }
                rbA.touchingContactsContainerList.Add(new TouchingContactContainer(rbB, 0, tcArray));
            }
            else            //if it does contain rbB then we need to check to see if it holds the feature pair
            {

                for (int i = 0; i < rbA.touchingContactsContainerList[element].tcArray.Length; ++i)
                {
                    if (rbA.touchingContactsContainerList[element].tcArray[i] != null)
                    {
                        for (int j = cdElement; j < collisionData.Count; ++j)
                        {
                            if (rbA.touchingContactsContainerList[element].tcArray[i].m_featurePairKey == collisionData[j].featurePairKey && collisionData[j].touchingContact == null) // DOES NOT NEED "&& collisionData[j].touchingContact == null" never a factor (CHANGE)
                            {
                                collisionData[j].impulseN = rbA.touchingContactsContainerList[element].tcArray[i].m_accumulatedNormalImpulse;

                                collisionData[j].impulseT = rbA.touchingContactsContainerList[element].tcArray[i].m_accumulatedTangentImpulse;

                                collisionData[j].touchingContact = rbA.touchingContactsContainerList[element].tcArray[i];
                                break;

                            }
                        }
                    }
                    //else {break;}     //THIS NEEDS TO EXIST? (CHANGE)
                }
            }
        }







        public static void RemoveTouchingContact(RigidBody rbA, RigidBody rbB)
        {
            for (int i = 0, count = rbA.touchingContactsContainerList.Count; i < count; ++i)
            {
                if (rbA.touchingContactsContainerList[i].rb == rbB)
                {
                    rbA.touchingContactsContainerList.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
