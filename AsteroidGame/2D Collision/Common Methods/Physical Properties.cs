using System;
using System.Collections.Generic;
using _2DLevelCreator;

namespace _2d_Objects
{
    public class Physical_Property
    {
        #region Static Data Members & Functions
        public static List<Physical_Property> list_PhysicalProperties = new List<Physical_Property>();
        public static float[] frictionCoefficients;
        public static int physicalPropertiesCount;

        public static Physical_Property Aluminum = new Physical_Property(2.712f, 1, "PP_Aluminum", Physical_Property.list_PhysicalProperties);
        public static Physical_Property Gold = new Physical_Property(19.320f, 0.49f, "PP_Gold", Physical_Property.list_PhysicalProperties);
        public static Physical_Property Ice = new Physical_Property(0.917f, 0.03f, "PP_Ice", Physical_Property.list_PhysicalProperties);
        public static Physical_Property Iron = new Physical_Property(7.850f, 1, "PP_Iron", Physical_Property.list_PhysicalProperties);
        public static Physical_Property Lead = new Physical_Property(11.340f, 0.9f, "PP_Lead", Physical_Property.list_PhysicalProperties);
        public static Physical_Property Leather = new Physical_Property(9.45f, 0.3f, "PP_Leather", Physical_Property.list_PhysicalProperties);
        public static Physical_Property Oak = new Physical_Property(0.705f, 0.54f, "PP_Oak", Physical_Property.list_PhysicalProperties);

        // Calculates all friction coefficients inside of the Physical Properties List. 
        // ie the friction coefficient of Ice & Lead, Ice & Ice, Gold & Leather etc.
        public static void CalculateFrictionCoefficients()
        {
            physicalPropertiesCount = Physical_Property.list_PhysicalProperties.Count;

            frictionCoefficients = new float[physicalPropertiesCount * physicalPropertiesCount];

            for (int i = 0; i < physicalPropertiesCount; ++i)
            {
                for (int j = 0; j < physicalPropertiesCount; ++j)
                {
                    frictionCoefficients[(Physical_Property.list_PhysicalProperties[i].ID.Index * physicalPropertiesCount) + Physical_Property.list_PhysicalProperties[j].ID.Index] =
                        (float)Math.Sqrt(
                            (Physical_Property.list_PhysicalProperties[i].frictionCoeff * Physical_Property.list_PhysicalProperties[i].frictionCoeff) +
                            (Physical_Property.list_PhysicalProperties[j].frictionCoeff * Physical_Property.list_PhysicalProperties[j].frictionCoeff)); //(WARNING) Need to find proper formula
                }
            }
        }

        //Locates the precalculated friction coefficient from the array by using the IDs of the 2 properties.
        public static float GetFrictionCoefficient(int physicalPropertyID_A, int physicalPropertyID_B)
        {
            return frictionCoefficients[(physicalPropertiesCount * physicalPropertyID_A) + physicalPropertyID_B];
        }

        #endregion


        public int GetMyIndex()
        {
            for (int iCount = this.list.Count - 1; iCount >= 0; --iCount)
                if (this.list[iCount] == this) return iCount;
            return 0;               // returns the default thing2D if no match.
        }
        public UniqueIdentifier FindID(int index)
        {
            if (index >= 0 && index < this.list.Count) return this.list[index].ID;
            return this.ID;    // returns current index.
        }
        public List<Physical_Property> list;





        public UniqueIdentifier ID { get; set; }
        public float    density         { get; set; }
        public float    frictionCoeff   { get; set; }


        //Constructor
        public Physical_Property(float density, float frictionCoeff, string uniqueID, List<Physical_Property> list)
        {
            this.density = density;
            this.frictionCoeff = frictionCoeff;

            this.list = list;
            this.ID = new UniqueIdentifier(uniqueID, list.Count, this.GetMyIndex, this.FindID);
            list.Add(this);
        }
    }
}
