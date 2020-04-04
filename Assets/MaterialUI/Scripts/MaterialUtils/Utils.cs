//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Static class containing miscellanous utilities.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Sets a given bool value, if the other bool returns true. Otherwise does nothing.
        /// </summary>
        /// <param name="boolean">The boolean to modify.</param>
        /// <param name="otherBool">The boolean to check.</param>
        public static void SetBoolValueIfTrue(ref bool boolean, bool otherBool)
        {
            if (otherBool)
            {
                boolean = true;
            }
        }

        /// <summary>
        /// Gets the screen's physical diagonal width from the current screen resolution and pixel density.
        /// </summary>
        /// <returns>The screen's physical diagonal width.</returns>
        public static float GetScreenDiagonal()
        {
            return GetScreenDiagonal(new Vector2(Screen.width, Screen.height), Screen.dpi);
        }

        /// <summary>
        /// Gets a screen's physical diagonal width from a resolution and pixel density.
        /// </summary>
        /// <param name="screenResolution">The screen resolution.</param>
        /// <param name="screenDpi">The screen pixel density.</param>
        /// <returns>A screen's physical diagonal width.</returns>
        public static float GetScreenDiagonal(Vector2 screenResolution, float screenDpi)
        {
            return Mathf.Sqrt(screenResolution.x + screenResolution.y) / screenDpi;
        }
    }
}