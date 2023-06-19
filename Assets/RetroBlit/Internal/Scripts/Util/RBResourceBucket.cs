namespace RetroBlitInternal
{
    using UnityEngine;

    /// <summary>
    /// A bucket that holds references to internal RetroBlit resources
    /// </summary>
    public class RBResourceBucket : MonoBehaviour
    {
        /// <summary>
        /// List of all internal texture resources
        /// </summary>
        public Texture2D[] Textures;

        /// <summary>
        /// List of all internal material resources
        /// </summary>
        public Material[] Materials;

        /// <summary>
        /// Gets Texture2D with the given name
        /// </summary>
        /// <param name="name">Name of texture</param>
        /// <returns>Texture or null if not found</returns>
        public Texture2D LoadTexture2D(string name)
        {
            if (Textures != null)
            {
                for (int i = 0; i < Textures.Length; i++)
                {
                    if (Textures[i] != null && Textures[i].name == name)
                    {
                        return Textures[i];
                    }
                }
            }

            Debug.Log("Could not find texture: " + name);

            return null;
        }

        /// <summary>
        /// Gets Material with the given name
        /// </summary>
        /// <param name="name">Name of material</param>
        /// <returns>Material or null if not found</returns>
        public Material LoadMaterial(string name)
        {
            if (Materials != null)
            {
                for (int i = 0; i < Materials.Length; i++)
                {
                    if (Materials[i] != null && Materials[i].name != null)
                    {
                        if (Materials[i].name.Split(' ')[0] == name)
                        {
                            return Materials[i];
                        }
                    }
                }
            }

            Debug.Log("Could not find material: " + name);

            return null;
        }
    }
}
