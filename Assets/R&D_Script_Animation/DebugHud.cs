using UnityEngine;

/// <summary>
/// Affiche les libellés de debug les uns sous les autres en haut à gauche de l'écran,
/// pour éviter qu'ils se superposent quand il y a plusieurs personnages.
/// </summary>
public static class DebugHud
{
    private static int _lastFrame = -1;
    private static int _line;

    /// <summary>
    /// Affiche une ligne de texte sous la précédente. À appeler depuis OnGUI.
    /// </summary>
    /// <param name="text">Texte à afficher.</param>
    public static void Label(string text)
    {
        // OnGUI est appelé plusieurs fois par frame : on ne dessine qu'au Repaint
        if (Event.current.type != EventType.Repaint) return;

        // Nouvelle frame : on repart de la première ligne
        if (Time.frameCount != _lastFrame)
        {
            _lastFrame = Time.frameCount;
            _line = 0;
        }

        GUI.Label(new Rect(10f, 10f + _line * 20f, 800f, 22f), text);
        _line++;
    }
}