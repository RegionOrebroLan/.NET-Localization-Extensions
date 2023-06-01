using System;
using System.Reflection;

namespace Investigation.Extensions
{
	public static class TypeExtension
	{
		#region Methods

		public static T GetNonPublicInstanceFieldValue<T>(this Type type, string fieldName, object instance)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			// ReSharper disable PossibleNullReferenceException
			return (T)type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
			// ReSharper restore PossibleNullReferenceException
		}

		#endregion
	}
}