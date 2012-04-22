/*
 *  TinySpace by Crealuz - TinySpace.Core
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinySpace.Core
{
	/// <summary>
	/// This class wraps a list, only exposing its enumerator.
	/// </summary>
	public class ReadOnlyList<T>
	{
		#region Constructors

		/// <summary>
		/// Public constructor.
		/// </summary>
		/// <param name="list">The list to wrap.</param>
		public ReadOnlyList(List<T> list)
		{
			m_list = list;
		}

		#endregion

		#region Fields

		private List<T> m_list;

		#endregion

		#region Methods

		/// <summary>
		/// This method returns the enumerator of the wrapped list.
		/// </summary>
		/// <returns>An enumerator for the wrapped list.</returns>
		public List<T>.Enumerator GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		#endregion
	}
}
