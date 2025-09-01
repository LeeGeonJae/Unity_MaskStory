using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryLine
{
    public string ID;
    public string Content;
}

public class StoryLoader : MonoBehaviour
{
    public TextAsset csvFile;
    public List<StoryLine> storyList = new List<StoryLine>();

    Dictionary<string, string> storyBoard = new Dictionary<string, string>();

    void Awake()
    {
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            if (row.Length < 2) continue;
            storyList.Add(new StoryLine
            {
                ID = row[0],
                Content = row[1].Trim('"')
            });

            if (!storyBoard.ContainsKey(row[0]))
            {
                storyBoard.Add(row[0], row[1].Trim('"'));
            }
        }
    }
}
