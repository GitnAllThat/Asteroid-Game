using System;
using Microsoft.Xna.Framework;


namespace _2DLevelCreator
{
    public static class StringMalarkey
    {

        public static string ExtractString(string s, string tag)               //http://stackoverflow.com/questions/1717611/regex-c-sharp-find-a-string-between-2-known-values
        {
            // You should check for errors in real-world code, omitted for brevity
            var startTag = " #" + tag + "# ";
            int startIndex = s.IndexOf(startTag) + startTag.Length;
            int endIndex = s.IndexOf(" #/" + tag + "#", startIndex);
            return s.Substring(startIndex, endIndex - startIndex);

        }

        public static bool CheckForTags(string s, string[] tags)               //http://stackoverflow.com/questions/1717611/regex-c-sharp-find-a-string-between-2-known-values
        {
            for (int i = 0; i < tags.Length; ++i)
            {
                if (!s.Contains(tags[i])) return false;
            }
            return true;
        }





        public static bool GetBoolFromString(string line, string tag)
        {
            return (Convert.ToInt32(StringMalarkey.ExtractString(line, tag)) != 1) ? false : true;
        }

        public static Vector3 GetVector3FromString(string line, string tag)
        {
            string[] token = (StringMalarkey.ExtractString(line, tag)).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return  new Vector3(float.Parse(token[0]), float.Parse(token[1]), 0);
        }

        public static Vector2[] GetVector2ArrayFromString(string line, string tag)
        {
            string[] token = (StringMalarkey.ExtractString(line, tag)).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            int count = (int)(token.Length * 0.5f);
            Vector2[] v2_Verts = new Vector2[count];
            for (int i = 0, j = 0; i < count; ++i)
            {
                v2_Verts[i].X = float.Parse(token[j++]);
                v2_Verts[i].Y = float.Parse(token[j++]);

            }
            return v2_Verts;
        }

        public static Vector3[] GetVector3ArrayFromString(string line, string tag)
        {

            string[] token = (StringMalarkey.ExtractString(line, tag)).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            int count = (int)(token.Length * 0.5f);
            Vector3[] v3_Verts = new Vector3[count];
            for (int i = 0, j = 0; i < count; ++i)
            {
                v3_Verts[i].X = float.Parse(token[j++]);
                v3_Verts[i].Y = float.Parse(token[j++]);
                v3_Verts[i].Z = 0;
            }
            return v3_Verts;
        }


        public static VertexPositionTextureArray GetVertPosTexArrayFromString(string stringData)
        {
            Vector3[] meshVerts = GetVector3ArrayFromString(stringData, "VERTS");

            Vector2[] meshTexCoords = GetVector2ArrayFromString(stringData, "TEXCOORDS");

            VertexPositionTextureWrapper[] vpt = new VertexPositionTextureWrapper[meshVerts.Length];
            for (int count = 0; count < meshVerts.Length; count++)
                vpt[count] = (new VertexPositionTextureWrapper(meshVerts[count], meshTexCoords[count]));

            return new VertexPositionTextureArray(vpt);
        }

        public static void RemoveTextFromString(string thisString, ref string fromThis)
        {
            fromThis = fromThis.Replace(thisString, string.Empty);
        }
    }
}
