using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegionOrebroLan.Localization.UnitTests
{
	[TestClass]
	public class LocalizedStringFactoryTest
	{
		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_IfTheCultureParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			try
			{
				new LocalizedStringFactory().Create(null, "Test", "Test", "Test");
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName.Equals("culture", StringComparison.Ordinal))
					throw;
			}
		}

		#endregion
	}
}