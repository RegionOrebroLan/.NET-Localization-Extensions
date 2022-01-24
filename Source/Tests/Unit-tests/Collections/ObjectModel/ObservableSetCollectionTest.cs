using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Collections.ObjectModel;

namespace UnitTests.Collections.ObjectModel
{
	[TestClass]
	public class ObservableSetCollectionTest
	{
		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Add_ItemParameter_IfTheParameterAlreadyExists_ShouldThrowAnArgumentException()
		{
			// ReSharper disable UseObjectOrCollectionInitializer
			var set = new ObservableSetCollection<string> { "Test" };
			// ReSharper restore UseObjectOrCollectionInitializer

			set.Add("Test");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Add_ItemParameter_IfTheParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			new ObservableSetCollection<string>().Add((string)null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		[SuppressMessage("Style", "IDE0028:Simplify collection initialization")]
		public void Add_ItemsParameter_IfTheParameterContainDuplicates_ShouldThrowAnArgumentException()
		{
			// ReSharper disable UseObjectOrCollectionInitializer
			var set = new ObservableSetCollection<string>();
			// ReSharper restore UseObjectOrCollectionInitializer

			set.Add(new[] { "First", "Second", "Second", "Third" });
		}

		[TestMethod]
		[SuppressMessage("Style", "IDE0028:Simplify collection initialization")]
		public void Add_ItemsParameter_IfTheParameterContainNoDuplicatesAndContainNoNullValues_ShouldAddTheItems()
		{
			// ReSharper disable UseObjectOrCollectionInitializer
			var set = new ObservableSetCollection<string>();
			// ReSharper restore UseObjectOrCollectionInitializer

			set.Add(new[] { "First", "Second", "Third", "Fourth" });

			Assert.AreEqual(4, set.Count);
			Assert.AreEqual("First", set[0]);
			Assert.AreEqual("Second", set[1]);
			Assert.AreEqual("Third", set[2]);
			Assert.AreEqual("Fourth", set[3]);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		[SuppressMessage("Style", "IDE0028:Simplify collection initialization")]
		public void Add_ItemsParameter_IfTheParameterContainNullValues_ShouldThrowAnArgumentException()
		{
			// ReSharper disable UseObjectOrCollectionInitializer
			var set = new ObservableSetCollection<string>();
			// ReSharper restore UseObjectOrCollectionInitializer

			set.Add(new[] { "First", "Second", null, "Third" });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		[SuppressMessage("Style", "IDE0004:Remove Unnecessary Cast")]
		[SuppressMessage("Style", "IDE0028:Simplify collection initialization")]
		public void Add_ItemsParameter_IfTheParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			// ReSharper disable UseObjectOrCollectionInitializer
			var set = new ObservableSetCollection<string>();
			// ReSharper restore UseObjectOrCollectionInitializer

			set.Add((IEnumerable<string>)null);
		}

		[TestMethod]
		public void Case_Test()
		{
			var set = new ObservableSetCollection<string>
			{
				"Test"
			};

			Assert.IsTrue(set.Contains("Test"));
			Assert.IsFalse(set.Contains("test"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Set_IfTheParameterAlreadyExists_ShouldThrowAnArgumentException()
		{
			// ReSharper disable UseObjectOrCollectionInitializer
			var set = new ObservableSetCollection<string>
			{
				"Test"
			};
			// ReSharper restore UseObjectOrCollectionInitializer

			set.Add("Test");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Set_IfTheParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			// ReSharper disable CollectionNeverQueried.Local
			var set = new ObservableSetCollection<string>()
			{
				"Test"
			};
			// ReSharper restore CollectionNeverQueried.Local

			set[0] = null;
		}

		#endregion
	}
}