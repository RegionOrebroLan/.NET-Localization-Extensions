using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Localization;
using RegionOrebroLan.Localization.Reflection;

namespace UnitTests
{
	[TestClass]
	public class LocalizationPathResolverTest
	{
		#region Fields

		private static readonly LocalizationPathResolver _localizationPathResolver = new();

		#endregion

		#region Properties

		protected internal virtual LocalizationPathResolver LocalizationPathResolver => _localizationPathResolver;

		#endregion

		#region Methods

		[TestMethod]
		public void Combine_IfAllParametersAreNull_ShouldReturnNull()
		{
			this.CombineTest();
		}

		[TestMethod]
		public void Combine_IfOneParametersIsNotNull_ShouldReturnTheValueOfThatParameterResolvedRegardingSeparators()
		{
			this.CombineTest();
		}

		[TestMethod]
		public void Combine_IfTheArrayParameterIsNull_ShouldReturnNull()
		{
			this.CombineTest();
		}

		[TestMethod]
		public void Combine_IfTheParametersContainsItemsThatAreNotNullNorEmpty_ShouldReturnThoseItemsResolvedRegardingSeparatorsAndSeparatedByADot()
		{
			this.CombineTest();
		}

		[TestMethod]
		public void Combine_IfThereAreNoParameters_ShouldReturnNull()
		{
			this.CombineTest();
		}

		[TestMethod]
		public void Combine_IfThereAreParametersIncludingSeparatorsToReplace_ShouldResolveThoseParametersRegardingSeparators()
		{
			this.CombineTest();
		}

		protected internal virtual void CombineTest()
		{
			Assert.IsNull(this.LocalizationPathResolver.Combine(null));

			Assert.IsNull(this.LocalizationPathResolver.Combine(Enumerable.Empty<string>().ToArray()));

			Assert.IsNull(this.LocalizationPathResolver.Combine(null, null));
			Assert.IsNull(this.LocalizationPathResolver.Combine(null, null, null, null, null));

			Assert.AreEqual(string.Empty, this.LocalizationPathResolver.Combine(null, null, string.Empty, null, null));
			Assert.AreEqual("Test", this.LocalizationPathResolver.Combine(null, null, null, null, "Test"));

			Assert.AreEqual("first.Second", this.LocalizationPathResolver.Combine(@"first\Second", null, null, null));
			Assert.AreEqual(".First.second.Third.fourth.Fifth..", this.LocalizationPathResolver.Combine(null, @"\First.second/Third\fourth.Fifth/.", null, null, null));
			Assert.AreEqual("...First..second...", this.LocalizationPathResolver.Combine("...First..second..."));

			Assert.AreEqual("first.Second.....third....Fourth.fifth", this.LocalizationPathResolver.Combine(@"first\Second/", null, @"./.third\/.", "Fourth", "fifth"));
			Assert.AreEqual("...First...second...Third...fourth...Fifth...", this.LocalizationPathResolver.Combine(@"/.\First\\\second///Third.\/fourth.\.Fifth.//"));
		}

		protected internal virtual IAssembly CreateAssembly(string rootNamespace)
		{
			var assemblyMock = new Mock<IAssembly>();

			assemblyMock.SetupAllProperties();
			assemblyMock.Setup(assembly => assembly.RootNamespace).Returns(rootNamespace);

			return assemblyMock.Object;
		}

		[TestMethod]
		public void NameIsRooted_IfTheNameParameterIsAnEmptyString_ShouldReturFalse()
		{
			Assert.IsFalse(this.LocalizationPathResolver.NameIsRooted(string.Empty));
		}

		[TestMethod]
		public void NameIsRooted_IfTheNameParameterIsAWhiteSpace_ShouldReturFalse()
		{
			Assert.IsFalse(this.LocalizationPathResolver.NameIsRooted(" "));
		}

		[TestMethod]
		public void NameIsRooted_IfTheNameParameterIsNull_ShouldReturFalse()
		{
			Assert.IsFalse(this.LocalizationPathResolver.NameIsRooted(null));
		}

		[TestMethod]
		public void NameIsRooted_IfTheNameParameterStartsWithAColon_ShouldReturTrue()
		{
			Assert.IsTrue(this.LocalizationPathResolver.NameIsRooted(":"));
			Assert.IsTrue(this.LocalizationPathResolver.NameIsRooted(": "));
			Assert.IsTrue(this.LocalizationPathResolver.NameIsRooted(":Test"));

			Assert.IsFalse(this.LocalizationPathResolver.NameIsRooted(" :"));
		}

		[TestMethod]
		public void Resolve_IfKeyIsEmptyAndPathIsEmpty_ShouldReturnOneResult()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("Test"), string.Empty, string.Empty).ToArray();

			Assert.AreEqual(1, paths.Length);
			Assert.AreEqual(string.Empty, paths.ElementAt(0));
		}

		[TestMethod]
		public void Resolve_IfKeyIsEmptyAndPathIsNull_ShouldReturnOneResult()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("Test"), string.Empty, null).ToArray();

			Assert.AreEqual(1, paths.Length);
			Assert.AreEqual(string.Empty, paths.ElementAt(0));
		}

		[TestMethod]
		public void Resolve_IfKeyIsNotRooted_ShouldReturnTwoValues()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("First.Second"), "Test", "First.Second.Third").ToArray();

			Assert.AreEqual(2, paths.Length);
			Assert.AreEqual("First.Second.Third.Test", paths.ElementAt(0));
			Assert.AreEqual("Third.Test", paths.ElementAt(1));
		}

		[TestMethod]
		public void Resolve_IfKeyIsNotRootedAndKeyContainsBackSlash_ShouldReturnTwoValues()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("First"), @"Test\Key", "First.Second.Third").ToArray();

			Assert.AreEqual(2, paths.Length);
			Assert.AreEqual("First.Second.Third.Test.Key", paths.ElementAt(0));
			Assert.AreEqual("Second.Third.Test.Key", paths.ElementAt(1));
		}

		[TestMethod]
		public void Resolve_IfKeyIsNotRootedAndKeyContainsSlash_ShouldReturnTwoValues()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("First"), "Test/Key", "First.Second.Third").ToArray();

			Assert.AreEqual(2, paths.Length);
			Assert.AreEqual("First.Second.Third.Test.Key", paths.ElementAt(0));
			Assert.AreEqual("Second.Third.Test.Key", paths.ElementAt(1));
		}

		[TestMethod]
		public void Resolve_IfKeyIsNotRootedAndPathContainsBackSlashes_ShouldReturnTwoValues()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("First.Second.Third"), "Test.Key", @"First\Second\Third").ToArray();

			Assert.AreEqual(2, paths.Length);
			Assert.AreEqual("First.Second.Third.Test.Key", paths.ElementAt(0));
			Assert.AreEqual("Test.Key", paths.ElementAt(1));
		}

		[TestMethod]
		public void Resolve_IfKeyIsNotRootedAndPathContainsSlash_ShouldReturnTwoValues()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("First.Second.Third"), "Test.Key", "First.Second/Third").ToArray();

			Assert.AreEqual(2, paths.Length);
			Assert.AreEqual("First.Second.Third.Test.Key", paths.ElementAt(0));
			Assert.AreEqual("Test.Key", paths.ElementAt(1));
		}

		[TestMethod]
		public void Resolve_IfKeyIsNullAndPathIsEmpty_ShouldReturnOneResult()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("Test"), null, string.Empty).ToArray();

			Assert.AreEqual(1, paths.Length);
			Assert.AreEqual(string.Empty, paths.ElementAt(0));
		}

		[TestMethod]
		public void Resolve_IfKeyIsNullAndPathIsNull_ShouldReturnAnEmptyResult()
		{
			Assert.IsFalse(this.LocalizationPathResolver.Resolve(this.CreateAssembly("Test"), null, null).Any());
		}

		[TestMethod]
		public void Resolve_IfKeyIsNullAndPathIsWhiteSpace_ShouldReturnTwoResults()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("Test"), null, " ").ToArray();

			Assert.AreEqual(2, paths.Length);
			Assert.AreEqual(" ", paths.ElementAt(0));
			Assert.AreEqual("Test. ", paths.ElementAt(1));
		}

		[TestMethod]
		public void Resolve_IfKeyIsRootedAndPathIsNotNull_ShouldReturnTwoResults()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("First.Second.Third"), ":Test.Key", "Testing").ToArray();

			Assert.AreEqual(2, paths.Length);
			Assert.AreEqual("Test.Key", paths.ElementAt(0));
			Assert.AreEqual("First.Second.Third.Test.Key", paths.ElementAt(1));
		}

		[TestMethod]
		public void Resolve_IfKeyIsRootedAndPathIsNull_ShouldReturnTwoResults()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("First.Second.Third"), ":Test.Key", null).ToArray();

			Assert.AreEqual(2, paths.Length);
			Assert.AreEqual("Test.Key", paths.ElementAt(0));
			Assert.AreEqual("First.Second.Third.Test.Key", paths.ElementAt(1));
		}

		[TestMethod]
		public void Resolve_IfKeyIsWhiteSpaceAndPathIsNull_ShouldReturTwoResults()
		{
			var paths = this.LocalizationPathResolver.Resolve(this.CreateAssembly("Test"), " ", null).ToArray();

			Assert.AreEqual(2, paths.Length);
			Assert.AreEqual(" ", paths.ElementAt(0));
			Assert.AreEqual("Test. ", paths.ElementAt(1));
		}

		#endregion
	}
}