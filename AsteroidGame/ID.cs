using System;
using System.Collections.Generic;


namespace _2DLevelCreator
{

    public class UniqueIdentifier_Reference
    {
        public UniqueIdentifier ID;
        public int Index 
        { 
            get { return ID.Index; } 
            set { ID = ID.FindIDFunction(value); } 
        }
        public string UniqueID 
        { 
            get { return ID.UniqueID; } 
            set { ID.UniqueID = value; } 
        }

        //Constructor
        public UniqueIdentifier_Reference(UniqueIdentifier ID)
        {
            this.ID = ID;
        }

        public UniqueIdentifier_Reference(UniqueIdentifier_Reference ID_Reference)
        {
            this.ID = ID_Reference.ID;
        }
    }



    public class UniqueIdentifier
    {
        #region Data Members

        private int _Index;
        public int Index 
        { 
            get { return _Index; } 
            set { UpdateIndex(); }              //Stops the User setting the index manually. Instead it will refresh itself
        }

        private string _UniqueID;
        public string UniqueID 
        { 
            get { return this._UniqueID; }
            set { this._UniqueID = GetUniqueIdentifier(value, this._UniqueID); }    //Will set the UniqueID to the value if it is a unique value.
        }

        #endregion

        //Needed for updating the index (ie if one is deleted from the list)
        public delegate int GetIndex();
        public GetIndex GetIndexFunction = null;

        //Needed if the index was changed on the property field panel
        public delegate UniqueIdentifier FindID(int index);
        public FindID FindIDFunction = null;

        public void UpdateIndex(){this._Index = GetIndexFunction();} 



        //Constructor
        public UniqueIdentifier(string uniqueID, int index, GetIndex GetIndexFunction, FindID FindIndexFunction)
        {
            this.GetIndexFunction = GetIndexFunction;
            this.FindIDFunction = FindIndexFunction;
            
            this.UniqueID = uniqueID;
            this._Index = index;
        }








        public static List<String> list_UniqueIdentifier = new List<string>();

        #region Static Functions For The Creation & Checking of Uniqie Identifiers

        public static string GetUniqueIdentifier(string newIdentifier, string oldIdentifier)
        {
            bool notCreated = true;

            RemoveUniqueIdentifier(oldIdentifier);

            if (newIdentifier == "") newIdentifier += "newIdentifier";   //Dont want empty Unique Identifier

            while (notCreated)
            {
                notCreated = CheckIfUniqueIdentifierExists(newIdentifier);
                if (notCreated) newIdentifier = AddNumber(newIdentifier);
            }


            list_UniqueIdentifier.Add(newIdentifier);
            return newIdentifier;
        }

        public static bool CheckIfUniqueIdentifierExists(string name)
        {
            for (int iCount = list_UniqueIdentifier.Count - 1; iCount >= 0; --iCount)
            {
                if (list_UniqueIdentifier[iCount] == name) return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a number to the end of a string to create a completely unique identifier.
        /// Checks for any current numbers tagged on the end and will increase by 1. This new string
        /// will then also be checked.
        /// </summary>
        public static string AddNumber(string name)
        {
            string[] token = name.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

            if (token.Length <= 1) name += "_1";
            else
            {
                float number;
                if (float.TryParse(token[token.Length - 1], out number))
                {
                    ++number;
                    name = "";
                    for (int iCount = 0, iCountMax = token.Length - 1; iCount < iCountMax; ++iCount)
                    {
                        name += token[iCount] + "_";
                    }

                    name += number;
                }
                else
                {
                    name += "_1";
                }
            }


            return name;
        }

        public static void RemoveUniqueIdentifier(string name)
        {
            for (int iCount = list_UniqueIdentifier.Count - 1; iCount >= 0; --iCount)
            {
                if (list_UniqueIdentifier[iCount] == name) list_UniqueIdentifier.RemoveAt(iCount);
            }
        }

        #endregion
    }






}
