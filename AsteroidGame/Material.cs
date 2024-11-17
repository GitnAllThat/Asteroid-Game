
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace _2DLevelCreator
{
    public class Material
    {
        public static List<Material> list_Material = new List<Material>();                  //Static List to hold any created Materials

        public static UniqueIdentifier_Reference GetUniqueIdentifierByName(string name)
        {
            for (int iCount = Material.list_Material.Count - 1; iCount >= 0; --iCount)
            {
                if (Material.list_Material[iCount].ID.UniqueID == name)
                {
                    return new UniqueIdentifier_Reference(Material.list_Material[iCount].ID);
                }
            }
            return new UniqueIdentifier_Reference(list_Material[0].ID);
        }

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
        public List<Material> list;





        public UniqueIdentifier ID { get; set; }


        public Texture2D Texture2D { get; set; }    //Holds the texture
        public string Location { get; set; }        //Holds the location of the texture
        public int Type { get; set; }               //The type of the Material. E.g. Colour, Normal, Specular etc...

        //Constructor
        public Material(string location, string uniqueID, int type, ContentManager Content, List<Material> list)
        {
            this.Location = location;
            this.Type = type;
            this.Texture2D = Content.Load<Texture2D>(location);


            this.list = list;
            this.ID = new UniqueIdentifier(uniqueID, list.Count, this.GetMyIndex, this.FindID);
            list.Add(this);
        }

    }
}
