using System;
using System.IO;
using System.Text;
using UnityEditor;

public class PackageUpdater {

    [MenuItem("Tools/TG Core/Refresh TG Core Package")]
    static void RefreshPackage() {

        string path = "Assets/../Packages/manifest.json";
        string lockGitPackageLine = "    \"com.tarcisiogames.core\": {";
        string hashReplace = "\"hash\" : \"\"";

        bool hasSeenPackage = false;
        bool isDone = false;

        StringBuilder sb = new StringBuilder();

        using (StreamReader reader = new StreamReader(path)) {
            while (true) {
                string line = reader.ReadLine();

                if (line == null) { break; }

                if (!isDone && hasSeenPackage && line.Contains("hash")) {
                    line = hashReplace;
                    isDone = true;
                }

                if (!hasSeenPackage && line.Contains(lockGitPackageLine)) {
                    hasSeenPackage = true;
                }
                sb.Append(line + Environment.NewLine);
            }
        }

        using (StreamWriter fileWriter = File.CreateText(path)) {
            fileWriter.Write(sb);
        }

        AssetDatabase.Refresh();
    }

}