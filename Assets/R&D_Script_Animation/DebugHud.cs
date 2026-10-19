using UnityEngine;

public static class DebugHud
{
    static int lastFrame = -1;
    static int line;

    public static void Label(string text)
    {
        if (Event.current.type != EventType.Repaint) return;

        if (Time.frameCount != lastFrame)
        {
            lastFrame = Time.frameCount;
            line = 0;
        }

        GUI.Label(new Rect(10f, 10f + line * 20f, 800f, 22f), text);
        line++;
    }
}