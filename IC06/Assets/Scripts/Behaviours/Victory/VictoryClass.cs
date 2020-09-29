using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VictoryClass
{

    private static string victoryName = "Player1";

    public static string VictoryName
    {
        get { return victoryName; }
        set { victoryName = value; }
    }

    private static string looserName = "Player2";

    public static string LooserName
    {
        get { return looserName; }
        set { looserName = value; }
    }

    private static Color victoryColor = Color.red;

    public static Color VictoryColor
    {
        get { return victoryColor; }
        set { victoryColor = value; }
    }

    private static Color looserColor = Color.blue;

    public static Color LooserColor
    {
        get { return looserColor; }
        set { looserColor = value; }
    }

}
