using System.IO;
using System.Text;
using UnityEngine;
using System;

namespace TG.IO
{
    public static class IOHelper
    {
        /// <summary>
        /// Saves the file to the platform's default persistent path.
        /// </summary>
        public static bool SaveFile(string fileName, string fileToSave, bool overwrite = true)
        {
            return SaveFile(fileName, fileToSave, "", overwrite);
        }

        /// <summary>
        /// Saves the file to the platform's default persistent path.
        /// </summary>
        /// <returns><c>true</c>, If the file was saved siccessfully <c>false</c> otherwise.</returns>
        /// <param name="fileName">The name to save the file.</param>
        /// <param name="fileToSave">Text to save.</param>
        /// <param name="directory">Sub-directory to save.</param>
        /// <param name="overwrite">If set to <c>true</c> overwrites the file.</param>
        public static bool SaveFile(string fileName, string fileToSave, string directory, bool overwrite = true)
        {
            CheckCreateDirectory(directory);

            string path = GetFullPersistentPath(fileName, directory);

            try
            { //TODO write it in chunks
                //TODO SCRAMBLE key with unique id from device

                using (StreamWriter fileWriter = overwrite ? File.CreateText(path) : File.AppendText(path))
                {
                    fileWriter.Write(fileToSave); //Enconding.UTF8
                    return true;
                }
            }
            catch (Exception ex)
            {
                fileToSave = "Exception: " + ex.Message;
                Debug.Log(fileToSave);
                return false;
            }
        }

        public static string LoadStringFromFile(string p_fileName, string p_directory){
            string output = string.Empty;
            LoadFile(p_fileName, out output, p_directory, null);
            return output;
        }

        /// <summary>
        /// Loads the specified file from the default directory.
        /// </summary>
        /// <returns><c>true</c>, if file was successfully loaded, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="output">Output string.</param>
        public static bool LoadFile(string fileName, out string output, Action<string> p_action = null)
        {
            return LoadFile(fileName, out output, "", p_action);
        }

        /// <summary>
        /// Loads the file from a specified directory.
        /// </summary>
        /// <returns><c>true</c>, if file was loaded successfully, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="output">Output string.</param>
        /// <param name="directory">Directory.</param>
        /// <param name="decrypt">If set to <c>true</c> decrypts.</param>
        public static bool LoadFile(string fileName, out string output, string directory, Action<string> p_action = null)
        {
            if (!DirectoryExists(directory))
            {
                output = string.Format(IOConstants.DIRECTORY_NOT_FOUND, directory);
                return false;
            }

            output = string.Empty;

            string filePath = GetFullPersistentPath(fileName, directory);
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            output += line;
                            p_action?.Invoke(line);
                        }
                        else
                        {
                            break;
                        }

                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                output = "Exception: " + ex.Message;
                return false;
            }
        }

        public static bool LoadFileEncrypted()
        {
            return false;
        }

        public static bool LoadAndParseTextFromResources(string p_path, bool p_ignoresFirstLine = false, Action<string> p_action = null)
        {

            TextAsset txt = (TextAsset)Resources.Load(p_path, typeof(TextAsset));

            string content = "";
            try
            {
                content = txt.text;
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not load file: " + p_path + "  Exception:" + ex);
                return false;
            }

            using (StringReader sr = new StringReader(content))
            {
                string line;

                if (p_ignoresFirstLine)
                {
                    line = sr.ReadLine();
                }

                while ((line = sr.ReadLine()) != null)
                {
                    p_action(line);
                }
            }
            return true;
        }

        #region Binary

        /// <summary>
        /// Saves the bytes as binary to file to the default directory.
        /// </summary>
        /// <returns><c>true</c>, if bytes to file were saved successfully, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="fileToSave">String to save.</param>
        /// <param name="overwrite">If set to <c>true</c> overwrites existing files.</param>
        public static bool SaveBytesToFile(string fileName, string fileToSave, bool overwrite = true)
        {
            return SaveBytesToFile(fileName, fileToSave, "", overwrite);
        }

        /// <summary>
        /// Saves the bytes as binary to file to the default directory.
        /// </summary>
        /// <returns><c>true</c>, if bytes to file were saved successfully, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="fileToSave">String to save.</param>
        /// <param name="directory">Directory to save the file.</param>
        /// <param name="overwrite">If set to <c>true</c> overwrites existing files.</param>
        public static bool SaveBytesToFile(string fileName, string fileToSave, string directory, bool overwrite = true)
        {
            CheckCreateDirectory(directory);

            string path = GetFullPersistentPath(fileName, directory);

            try
            {
                byte[] binaryToSave;
                binaryToSave = Encoding.UTF8.GetBytes(fileToSave);

                FileMode fileMode = overwrite ? FileMode.Create : FileMode.Append;

                using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, fileMode)))
                {
                    binaryWriter.Write(binaryToSave);
                }
                return true;
            }
            catch (Exception ex)
            {
                fileToSave = "Exception: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Loads the binary file.
        /// </summary>
        /// <returns><c>true</c>, if binary file was loaded successfully, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="output">Output string.</param>
        public static bool LoadBinaryFile(string fileName, out string output)
        {
            return LoadBinaryFile(fileName, out output, "");
        }

        /// <summary>
        /// Loads the binary file on the specified directory.
        /// </summary>
        /// <returns><c>true</c>, if binary file was loaded successfully, <c>false</c> otherwise.</returns>
        /// <param name="nameOfFile">Name of file.</param>
        /// <param name="output">Output string.</param>
        /// <param name="directory">Directory to load the file from.</param>
        public static bool LoadBinaryFile(string nameOfFile, out string output, string directory)
        {
            if (!DirectoryExists(directory))
            {
                output = IOConstants.DIRECTORY_NOT_FOUND;
                return false;
            }

            output = String.Empty;

            string fileName = GetFullPersistentPath(nameOfFile, directory);

            if (!FileExists(fileName))
            {
                output = IOConstants.FILE_NOT_FOUND;
                return false;
            }

            try
            {

                byte[] bytesToRead = File.ReadAllBytes(fileName); //TODO Read in chunks
                output = Encoding.UTF8.GetString(bytesToRead);

                return true;

            }
            catch (Exception ex)
            {
                output = "Exception: " + ex.Message;
                return false;
            }
        }
        #endregion Binary


        #region util

        /// <summary>
        /// Checks to see if the sub-directory exists, creates one otherwise.
        /// </summary>
        /// <param name="directory">Directory.</param>
        static void CheckCreateDirectory(string directory)
        {
            if (directory != null)
            {

                Debug.Log("Creating directory: " + Application.persistentDataPath + "/" + directory);
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/" + directory);
            }
        }

        /// <summary>
        /// Checks if the given file exists inside the provided subfolder
        /// </summary>
        /// <returns><c>true</c>, if exists was filed, <c>false</c> otherwise.</returns>
        /// <param name="nameOfFile">Name of file.</param>
        /// <param name="directory">Sub Directory to search.</param>
        /// <param name="extension">File extension.</param>
        public static bool FileExists(string fileName, string directory = "", string extension = "")
        {
            //TODO check what happens if directory does not exist
            string path = Application.persistentDataPath + directory;
            string[] files = extension == "" ? Directory.GetFiles(path) : Directory.GetFiles(path, extension);

            for (int i = 0; i < files.Length; ++i)
            {
                Debug.Log(files[i] + "   " + fileName);
                if (files[i].Equals(fileName))
                { //Path.GetFileName (
                    Debug.Log("File was found!");
                    return true;
                }
            }
            return false;
        }

        public static bool DirectoryExists(string directory)
        {
            return Directory.Exists(Application.persistentDataPath + "/" + directory);
        }

        /// <summary>
        /// Returns the full path for a given file and subdirectory.
        /// </summary>
        /// <returns>The full persistent path.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="directory">Directory.</param>
        public static string GetFullPersistentPath(string fileName, string directory)
        {
            //TODO Safety checks on variables
            return Application.persistentDataPath + "/" + directory + "/" + fileName;
        }

        #endregion util


        #region Delete
        public static bool DeleteFile(string fileName, string directory, out string output)
        {
            string filePath = GetFullPersistentPath(fileName, directory);
            output = String.Empty;
            if (!FileExists(filePath))
            {
                output = IOConstants.FILE_NOT_FOUND;
                return false;
            }
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (IOException ex)
            {
                output += ex.Message.ToString();
                return false;
            }
        }

        public static void DeleteAllFiles(string directory)
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath + "/");
            string output = "";
            for (int i = 0; i < files.Length; i++)
            {
                DeleteFile(files[i], directory, out output);
            }
        }
        #endregion Delete

    }
}