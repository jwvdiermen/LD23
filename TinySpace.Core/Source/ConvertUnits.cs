using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace TinySpace.Core
{
	///// <summary>
	///// Convert units between display and game units.
	///// </summary>
	//public static class ConvertUnits
	//{
	//    #region Fields

	//    private static float ms_displayUnitsToGameUnitsRatio = 100f;
	//    private static float ms_gameUnitsToDisplayUnitsRatio = 1 / ms_displayUnitsToGameUnitsRatio;

	//    #endregion

	//    #region Methods

	//    public static void SetDisplayUnitToGameUnitRatio(float displayUnitsPerGameUnit)
	//    {
	//        ms_displayUnitsToGameUnitsRatio = displayUnitsPerGameUnit;
	//        ms_gameUnitsToDisplayUnitsRatio = 1 / displayUnitsPerGameUnit;
	//    }

	//    public static float ToDisplayUnits(float gameUnits)
	//    {
	//        return gameUnits * ms_displayUnitsToGameUnitsRatio;
	//    }

	//    public static float ToDisplayUnits(int gameUnits)
	//    {
	//        return gameUnits * ms_displayUnitsToGameUnitsRatio;
	//    }

	//    public static Vector2 ToDisplayUnits(Vector2 gameUnits)
	//    {
	//        return gameUnits * ms_displayUnitsToGameUnitsRatio;
	//    }

	//    public static void ToDisplayUnits(ref Vector2 gameUnits, out Vector2 displayUnits)
	//    {
	//        Vector2.Multiply(ref gameUnits, ms_displayUnitsToGameUnitsRatio, out displayUnits);
	//    }

	//    public static Vector3 ToDisplayUnits(Vector3 gameUnits)
	//    {
	//        return gameUnits * ms_displayUnitsToGameUnitsRatio;
	//    }

	//    public static Vector2 ToDisplayUnits(float x, float y)
	//    {
	//        return new Vector2(x, y) * ms_displayUnitsToGameUnitsRatio;
	//    }

	//    public static void ToDisplayUnits(float x, float y, out Vector2 displayUnits)
	//    {
	//        displayUnits = Vector2.Zero;
	//        displayUnits.X = x * ms_displayUnitsToGameUnitsRatio;
	//        displayUnits.Y = y * ms_displayUnitsToGameUnitsRatio;
	//    }

	//    public static float ToGameUnits(float displayUnits)
	//    {
	//        return displayUnits * ms_gameUnitsToDisplayUnitsRatio;
	//    }

	//    public static float ToGameUnits(double displayUnits)
	//    {
	//        return (float)displayUnits * ms_gameUnitsToDisplayUnitsRatio;
	//    }

	//    public static float ToGameUnits(int displayUnits)
	//    {
	//        return displayUnits * ms_gameUnitsToDisplayUnitsRatio;
	//    }

	//    public static Vector2 ToGameUnits(Vector2 displayUnits)
	//    {
	//        return displayUnits * ms_gameUnitsToDisplayUnitsRatio;
	//    }

	//    public static Vector3 ToGameUnits(Vector3 displayUnits)
	//    {
	//        return displayUnits * ms_gameUnitsToDisplayUnitsRatio;
	//    }

	//    public static void ToGameUnits(ref Vector2 displayUnits, out Vector2 gameUnits)
	//    {
	//        Vector2.Multiply(ref displayUnits, ms_gameUnitsToDisplayUnitsRatio, out gameUnits);
	//    }

	//    public static Vector2 ToGameUnits(float x, float y)
	//    {
	//        return new Vector2(x, y) * ms_gameUnitsToDisplayUnitsRatio;
	//    }

	//    public static Vector2 ToGameUnits(double x, double y)
	//    {
	//        return new Vector2((float)x, (float)y) * ms_gameUnitsToDisplayUnitsRatio;
	//    }

	//    public static void ToGameUnits(float x, float y, out Vector2 gameUnits)
	//    {
	//        gameUnits = Vector2.Zero;
	//        gameUnits.X = x * ms_gameUnitsToDisplayUnitsRatio;
	//        gameUnits.Y = y * ms_gameUnitsToDisplayUnitsRatio;
	//    }

	//    #endregion
	//}
}
