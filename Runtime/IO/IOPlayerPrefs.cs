using UnityEngine;

namespace TG.IO{
    public static class IOPlayerPrefs {

        #region PlayerPrefs


        #region Save
        /// <summary>
        /// Sets the bool and saves it (as int) to PlayerPrefs.
        /// </summary>
        /// <param name="p_key">The string key.</param>
        /// <param name="p_value">The boolean value.</param>
        public static void SaveBool(string p_key, bool p_value){
            SaveInt(p_key, p_value ? 1 : 0);
        }

        /// <summary>
        /// Sets the float and saves it to PlayerPrefs.
        /// </summary>
        /// <param name="p_key">The string key.</param>
        /// <param name="p_value">The float value.</param>
        public static void SaveFloat(string p_key, float p_value){
            PlayerPrefs.SetFloat(p_key, p_value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the integer and saves it to PlayerPrefs.
        /// </summary>
        /// <param name="p_key">The string key.</param>
        /// <param name="p_value">The integer value.</param>
        public static void SaveInt(string p_key, int p_value){
            PlayerPrefs.SetInt(p_key, p_value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the string and saves it to PlayerPrefs.
        /// </summary>
        /// <param name="p_key">The string key.</param>
        /// <param name="p_value">The string value.</param>
        public static void SaveString(string p_key, string p_value){
            PlayerPrefs.SetString(p_key, p_value);
            PlayerPrefs.Save();
        }
        #endregion Save


        #region Load
        /// <summary>
        /// Loads a float, if it exists, from the PlayerPrefs.
        /// </summary>
        /// <returns>The float value.</returns>
        /// <param name="p_key">The string key.</param>
        /// <param name="p_defaultValue">The default float value.</param>
        public static bool LoadBool(string p_key, bool p_defaultValue){
            return PlayerPrefs.GetInt(p_key, p_defaultValue == false ? 0 : 1) == 1 ? true : false;          
        }

        /// <summary>
        /// Loads a float, if it exists, from the PlayerPrefs.
        /// </summary>
        /// <returns>The float value.</returns>
        /// <param name="p_key">The string key.</param>
        /// <param name="p_defaultValue">The default float value.</param>
        public static float LoadFloat(string p_key, float p_defaultValue = 0f){
            return PlayerPrefs.GetFloat(p_key, p_defaultValue);          
        }

        /// <summary>
        /// Loads an int, if it exists, from the PlayerPrefs.
        /// </summary>
        /// <returns>The int value.</returns>
        /// <param name="p_key">The string key.</param>
        /// <param name="p_defaultValue">The default int value.</param>
        public static int LoadInt(string p_key, int p_defaultValue = 0){
            return PlayerPrefs.GetInt(p_key, p_defaultValue);          
        }

        /// <summary>
        /// Loads an int, if it exists, from the PlayerPrefs.
        /// </summary>
        /// <returns>The float value.</returns>
        /// <param name="p_key">The string key.</param>
        /// <param name="p_defaultValue">The default value float.</param>
        public static string LoadString(string p_key, string p_defaultValue = ""){
            return PlayerPrefs.GetString(p_key, p_defaultValue);          
        }
        #endregion Load


        #region Delete

        /// <summary>
        /// Deletes all data saved to PlayerPrefs.
        /// </summary>
        public static void DeleteAll(){
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// Deletes the key saved to PlayerPrefs.
        /// </summary>
        /// <param name="p_key">The key to be deleted.</param>
        public static void DeleteKey(string p_key){
            PlayerPrefs.DeleteKey(p_key);
        }

        #endregion Delete

        #endregion PlayerPrefs
    }
}
