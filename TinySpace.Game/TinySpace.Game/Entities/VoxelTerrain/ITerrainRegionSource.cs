using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinySpace.Game.Entities
{
	/// <summary>
	/// Provides data points for a terrain region.
	/// </summary>
	public interface ITerrainRegionSource
	{
		#region Properties

		/// <summary>
		/// Gets the number of points both in the width and height.
		/// </summary>
		int Size { get; }

		/// <summary>
		/// Gets if the source has changed.
		/// </summary>
		bool HasChanged { get; }

		/// <summary>
		/// Gets the X offset.
		/// </summary>
		int OffsetX { get; }

		/// <summary>
		/// Gets the Y offset.
		/// </summary>
		int OffsetY { get; }

		/// <summary>
		/// Gets the region data.
		/// </summary>
		/// <remarks>Writing should be done using the <see cref="M:ITerrainRegionSource.SetPoint" /> method.</remarks>
		sbyte[,] Data { get; }

		#endregion

		#region Methods

		///// <summary>
		///// Gets the value of the point at the given coordinates.
		///// </summary>
		///// <param name="x">The X coordinate.</param>
		///// <param name="y">The Y coordinate.</param>
		///// <returns>The value of the point.</returns>
		//sbyte GetPoint(int x, int y);

		/// <summary>
		/// Sets the value of the point at the given coordinates.
		/// </summary>
		/// <param name="x">The X coordinate.</param>
		/// <param name="y">The Y coordinate.</param>
		/// <param name="value">The value of the point.</param>
		void SetPoint(int x, int y, sbyte value);

		#endregion
	}
}
