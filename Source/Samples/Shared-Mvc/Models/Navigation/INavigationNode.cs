﻿using System;
using System.Collections.Generic;

namespace Company.WebApplication.Models.Navigation
{
	public interface INavigationNode
	{
		#region Properties

		bool Active { get; }
		IEnumerable<INavigationNode> Children { get; }
		INavigationNode Parent { get; }
		string Text { get; }
		string Tooltip { get; }
		Uri Url { get; }

		#endregion
	}
}