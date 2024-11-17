using System;
using Microsoft.Xna.Framework;
using System.IO;

using _2DLevelCreator;
using Microsoft.Xna.Framework.Content;

namespace LoadSpace
{
    public partial class Load
    {
        public static void Load_Game_Materials(ContentManager contentManager)
        {
            new Material("Images\\Checker", "Mat_Checker", 0, contentManager, Material.list_Material);    //Add a default image to the list.
            
            

            string line;
            StreamReader sr = new StreamReader("Asset Data//01 Materials.txt");

            while ((line = sr.ReadLine()) != null)
            {
                if (!StringMalarkey.CheckForTags(line, new string[] { "PATH", "NAME", "TYPE" })) continue;        //Check that the string contains the appropriate tags.

                new Material(   StringMalarkey.ExtractString(line, "PATH"),
                                StringMalarkey.ExtractString(line, "NAME"),
                                Convert.ToInt32(StringMalarkey.ExtractString(line, "TYPE")),
                                contentManager, 
                                Material.list_Material);


            }

            sr.Close();
        }
    }
}
