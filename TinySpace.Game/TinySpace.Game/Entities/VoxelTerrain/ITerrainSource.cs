using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinySpace.Game.Entities
{
	/// <summary>
	/// Provides the data that represents a terrain. The data is loadable through so called regions.
	/// </summary>
	public interface ITerrainSource
	{
		#region Properties

		/// <summary>
		/// Gets the number of regions on the X axis.
		/// </summary>
		int RegionCountX { get; }

		/// <summary>
		/// Gets the number of regions on the Y axis.
		/// </summary>
		int RegionCountY { get; }

		/// <summary>
		/// Gets if the source has changed.
		/// </summary>
		bool HasChanged { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets the source for the region at the given coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>The region source.</returns>
		ITerrainRegionSource GetRegionSource(int x, int y);

		#endregion
	}
}
