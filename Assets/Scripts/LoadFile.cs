using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LoadFile
{
    public static List<Step> LoadSteps(string filePath)
    {
        var list = new List<Step>();

        foreach (string line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string dir = line.Substring(0, 1);
            int value  = int.Parse(line.Substring(1));

            list.Add(new Step(dir, value));
        }

        return list;
    }
}
