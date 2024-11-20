using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace MapStudio.UI
{
    public class RecentFileHandler
    {
        const int MRUnumber = 6;

        public static void LoadRecentList(string filePath, List<string> recentList)
        {
            recentList.Clear();

            if (File.Exists(filePath))
            {
                StreamReader listToRead = new StreamReader(filePath); //read file stream
                string line;
                while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                {
                    if ((Directory.Exists(line) || File.Exists(line)) &&  !recentList.Contains(line))
                        recentList.Add(line); //insert to list
                }
                listToRead.Close(); //close the stream
            }
        }

        public static void SaveRecentFile(string recentFile, string filePath, List<string> recentList)
        {
            if (recentList.Contains(recentFile))
                return;

            LoadRecentList(filePath, recentList); //load list from file
            if (!(recentList.Contains(recentFile))) //prevent duplication on recent list
                recentList.Insert(0, recentFile); //insert given path into list

            recentList = recentList.Distinct().ToList();

            //keep list number not exceeded the given value
            while (recentList.Count > MRUnumber) {
                recentList.RemoveAt(MRUnumber);
            }

            //writing menu list to file
            //create file called "Recent.txt" located on app folder
            StreamWriter stringToWrite =
            new StreamWriter(filePath);
            foreach (string item in recentList)
            {
                stringToWrite.WriteLine(item); //write list to stream
            }
            stringToWrite.Flush(); //write stream to file
            stringToWrite.Close(); //close the stream and reclaim memory
        }
    }
}
