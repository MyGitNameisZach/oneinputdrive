using UnityEngine;

//Stores the best distance on the device using PlayerPrefs
public static class HighScore
{
    const string BestDistanceKey = "BestDistance";

    public static int Best => PlayerPrefs.GetInt(BestDistanceKey, 0);

    //Returns true if the distance is a new best
    public static bool Submit(int distance)
    {
        if (distance <= Best)
            return false;

        PlayerPrefs.SetInt(BestDistanceKey, distance);
        PlayerPrefs.Save();

        return true;
    }

    public static void ResetBest()
    {
        PlayerPrefs.DeleteKey(BestDistanceKey);
        PlayerPrefs.Save();
    }
}
