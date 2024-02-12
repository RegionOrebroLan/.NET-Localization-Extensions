using System.ComponentModel;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RegionOrebroLan.Localization.Reflection;

namespace IntegrationTests.Reflection
{
	[TestClass]
	public class AssemblyInterfaceTypeConverterTest : SerializationTest
	{
		#region Fields

		private static readonly IAssemblyHelper _assemblyHelper = new AssemblyHelper(new RootNamespaceResolver());

		#endregion

		#region Properties

		protected internal virtual IAssemblyHelper AssemblyHelper => _assemblyHelper;
		protected internal virtual string Quote => "\"";

		#endregion

		#region Methods

		[TestMethod]
		public void CanConvertFrom_WithTypeParameter_IfTheTypeParameterIsOfTypeAssembly_ShouldReturnTrue()
		{
			Assert.IsTrue(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertFrom(typeof(Assembly)));
		}

		[TestMethod]
		public void CanConvertFrom_WithTypeParameter_IfTheTypeParameterIsOfTypeAssemblyWrapper_ShouldReturnTrue()
		{
			Assert.IsTrue(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertFrom(typeof(AssemblyWrapper)));
		}

		[TestMethod]
		public void CanConvertFrom_WithTypeParameter_IfTheTypeParameterIsOfTypeIAssembly_ShouldReturnTrue()
		{
			Assert.IsTrue(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertFrom(typeof(IAssembly)));
		}

		[TestMethod]
		public void CanConvertFrom_WithTypeParameter_IfTheTypeParameterIsOfTypeInteger_ShouldReturnFalse()
		{
			Assert.IsFalse(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertFrom(typeof(int)));
		}

		[TestMethod]
		public void CanConvertFrom_WithTypeParameter_IfTheTypeParameterIsOfTypeString_ShouldReturnTrue()
		{
			Assert.IsTrue(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertFrom(typeof(string)));
		}

		[TestMethod]
		public void CanConvertTo_WithTypeParameter_IfTheTypeParameterIsOfTypeAssembly_ShouldReturnTrue()
		{
			Assert.IsTrue(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertTo(typeof(Assembly)));
		}

		[TestMethod]
		public void CanConvertTo_WithTypeParameter_IfTheTypeParameterIsOfTypeAssemblyWrapper_ShouldReturnTrue()
		{
			Assert.IsTrue(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertTo(typeof(AssemblyWrapper)));
		}

		[TestMethod]
		public void CanConvertTo_WithTypeParameter_IfTheTypeParameterIsOfTypeIAssembly_ShouldReturnTrue()
		{
			Assert.IsTrue(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertTo(typeof(IAssembly)));
		}

		[TestMethod]
		public void CanConvertTo_WithTypeParameter_IfTheTypeParameterIsOfTypeInteger_ShouldReturnFalse()
		{
			Assert.IsFalse(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertTo(typeof(int)));
		}

		[TestMethod]
		public void CanConvertTo_WithTypeParameter_IfTheTypeParameterIsOfTypeString_ShouldReturnTrue()
		{
			Assert.IsTrue(this.GetRegisteredAssemblyInterfaceTypeConverter().CanConvertTo(typeof(string)));
		}

		[TestMethod]
		public void ConvertFrom_WithObjectParameter_IfTheValueParameterIsAnAssembly_ShouldReturnAnAssemblyWrapper()
		{
			var assembly = typeof(object).Assembly;

			var result = this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertFrom(assembly) as AssemblyWrapper;

			Assert.IsNotNull(result);
			Assert.IsTrue(ReferenceEquals(assembly, result.Assembly));
			Assert.AreEqual(assembly.FullName, result.FullName);
			Assert.AreEqual(assembly.GetName().Name, result.Name);
		}

		[TestMethod]
		public void ConvertFrom_WithObjectParameter_IfTheValueParameterIsAnAssemblyWrapper_ShouldReturnAnotherInstanceOfAnAssemblyWrapper()
		{
			var assembly = typeof(object).Assembly;
			var assemblyWrapper = (AssemblyWrapper)this.AssemblyHelper.Wrap(assembly);

			var result = this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertFrom(assemblyWrapper) as AssemblyWrapper;

			Assert.IsNotNull(result);
			Assert.IsFalse(ReferenceEquals(assemblyWrapper, result));
			Assert.AreEqual(assembly.FullName, result.FullName);
			Assert.AreEqual(assembly.GetName().Name, result.Name);
		}

		[TestMethod]
		public void ConvertFrom_WithObjectParameter_IfTheValueParameterIsAnEmptyString_ShouldReturnNull()
		{
			Assert.IsNull(this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertFrom(string.Empty));
		}

		[TestMethod]
		public void ConvertFrom_WithObjectParameter_IfTheValueParameterIsAnIAssembly_ShouldReturnAnAssemblyWrapperNotReferenceEqualTheValueParameter()
		{
			var assembly = typeof(object).Assembly;
			var assemblyWrapper = this.AssemblyHelper.Wrap(assembly);

			var result = this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertFrom(assemblyWrapper) as AssemblyWrapper;

			Assert.IsNotNull(result);
			Assert.IsFalse(ReferenceEquals(assemblyWrapper, result));
			Assert.AreEqual(assembly.FullName, result.FullName);
			Assert.AreEqual(assembly.GetName().Name, result.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void ConvertFrom_WithObjectParameter_IfTheValueParameterIsAnInteger_ShouldThrowANotSupportedException()
		{
			this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertFrom(5);
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void ConvertFrom_WithObjectParameter_IfTheValueParameterIsAnInvalidAssemblyNameString_ShouldThrowAFileNotFoundException()
		{
			this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertFrom("Invalid-assembly-name");
		}

		[TestMethod]
		public void ConvertFrom_WithObjectParameter_IfTheValueParameterIsAValidAssemblyFullNameString_ShouldReturnAnAssemblyWrapper()
		{
			var assembly = typeof(object).Assembly;

			var result = this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertFrom(assembly.FullName) as AssemblyWrapper;

			Assert.IsNotNull(result);
			Assert.AreEqual(assembly.FullName, result.FullName);
			Assert.AreEqual(assembly.GetName().Name, result.Name);
		}

		[TestMethod]
		public void ConvertFrom_WithObjectParameter_IfTheValueParameterIsAValidAssemblyNameString_ShouldReturnAnAssemblyWrapper()
		{
			var assembly = typeof(object).Assembly;

			var result = this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertFrom(assembly.GetName().Name) as AssemblyWrapper;

			Assert.IsNotNull(result);
			Assert.AreEqual(assembly.FullName, result.FullName);
			Assert.AreEqual(assembly.GetName().Name, result.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void ConvertFrom_WithObjectParameter_IfTheValueParameterIsNull_ShouldThrowANotSupportedException()
		{
			this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertFrom(null);
		}

		[TestMethod]
		public void ConvertTo_WithObjectAndTypeParameter_IfTheValueParameterIsAnIAssemblyAndTheDestinationTypeIsOfTypeAssembly_ShouldReturnTheCorrespondingAssembly()
		{
			var assembly = typeof(object).Assembly;

			var result = this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertTo(this.AssemblyHelper.Wrap(assembly), typeof(Assembly)) as Assembly;

			Assert.IsNotNull(result);
			Assert.IsTrue(ReferenceEquals(assembly, result));
			Assert.AreEqual(assembly.FullName, result.FullName);
			Assert.AreEqual(assembly.GetName().Name, result.GetName().Name);
		}

		[TestMethod]
		public void ConvertTo_WithObjectAndTypeParameter_IfTheValueParameterIsAnIAssemblyAndTheDestinationTypeIsOfTypeAssemblyWrapper_ShouldReturnAnAssemblyWrapperNotReferenceEqualTheValueParameter()
		{
			var assembly = typeof(object).Assembly;
			var assemblyWrapper = this.AssemblyHelper.Wrap(assembly);

			var result = this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertTo(assemblyWrapper, typeof(AssemblyWrapper)) as AssemblyWrapper;

			Assert.IsNotNull(result);
			Assert.IsFalse(ReferenceEquals(assemblyWrapper, result));
			Assert.AreEqual(assembly.FullName, result.FullName);
			Assert.AreEqual(assembly.GetName().Name, result.Name);
		}

		[TestMethod]
		public void ConvertTo_WithObjectAndTypeParameter_IfTheValueParameterIsAnIAssemblyAndTheDestinationTypeIsOfTypeIAssembly_ShouldReturnAnAssemblyWrapper()
		{
			var assembly = typeof(object).Assembly;

			var result = this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertTo(this.AssemblyHelper.Wrap(assembly), typeof(IAssembly)) as AssemblyWrapper;

			Assert.IsNotNull(result);
			Assert.IsTrue(ReferenceEquals(assembly, result.Assembly));
			Assert.AreEqual(assembly.FullName, result.FullName);
			Assert.AreEqual(assembly.GetName().Name, result.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void ConvertTo_WithObjectAndTypeParameter_IfTheValueParameterIsAnIAssemblyAndTheDestinationTypeIsOfTypeInteger_ShouldThrowANotSupportedException()
		{
			// ReSharper disable AssignNullToNotNullAttribute
			this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertTo(this.AssemblyHelper.Wrap(typeof(object).Assembly), typeof(int));
			// ReSharper restore AssignNullToNotNullAttribute
		}

		[TestMethod]
		public void ConvertTo_WithObjectAndTypeParameter_IfTheValueParameterIsAnIAssemblyAndTheDestinationTypeIsOfTypeString_ShouldReturnTheFullNameOfTheAssembly()
		{
			var assembly = typeof(object).Assembly;

			var result = this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertTo(this.AssemblyHelper.Wrap(assembly), typeof(string)) as string;

			Assert.IsNotNull(result);
			Assert.AreEqual(assembly.FullName, result);
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void ConvertTo_WithObjectAndTypeParameter_IfTheValueParameterIsNotAnIAssembly_ShouldThrowANotSupportedException()
		{
			this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertTo(5, typeof(string));
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void ConvertTo_WithObjectAndTypeParameter_IfTheValueParameterIsNull_ShouldThrowANotSupportedException()
		{
			// ReSharper disable AssignNullToNotNullAttribute
			this.GetRegisteredAssemblyInterfaceTypeConverter().ConvertTo(null, typeof(string));
			// ReSharper restore AssignNullToNotNullAttribute
		}

		[TestMethod]
		[ExpectedException(typeof(JsonSerializationException))]
		public void DeserializeFromJson_IfTheTypeIsAssemblyInterfaceAndTheJsonValueIsAnInvalidAssemblyName_ShouldThrowAJsonSerializationException()
		{
			try
			{
				this.JsonDeserialize<IAssembly>(this.Quote + "Invalid-assembly-name" + this.Quote);
			}
			catch(JsonSerializationException jsonSerializationException)
			{
				const string expectedMessage = "Error converting value \"Invalid-assembly-name\" to type 'RegionOrebroLan.Localization.Reflection.IAssembly'. Path '', line 1, position 23.";

				if(jsonSerializationException.Message.Equals(expectedMessage, StringComparison.Ordinal) && jsonSerializationException.InnerException is ArgumentException)
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(JsonSerializationException))]
		public void DeserializeFromJson_IfTheTypeIsAssemblyInterfaceAndTheJsonValueIsAnInvalidToken_ShouldThrowAJsonSerializationException()
		{
			try
			{
				this.JsonDeserialize<IAssembly>("1");
			}
			catch(JsonSerializationException jsonSerializationException)
			{
				const string expectedMessage = "Error converting value 1 to type 'RegionOrebroLan.Localization.Reflection.IAssembly'. Path '', line 1, position 1.";

				if(jsonSerializationException.Message.Equals(expectedMessage, StringComparison.Ordinal) && jsonSerializationException.InnerException is ArgumentException)
					throw;
			}
		}

		[TestMethod]
		public void DeserializeFromJson_IfTheTypeIsAssemblyInterfaceAndTheJsonValueIsAValidAssemblyFullName_ShouldDeserializeCorrectly()
		{
			var assembly = typeof(string).Assembly;
			var name = assembly.GetName().Name;

			var deserializedAssembly = this.JsonDeserialize<IAssembly>(this.Quote + assembly.FullName + this.Quote);
			Assert.AreEqual(assembly.FullName, deserializedAssembly.FullName);
			Assert.AreEqual(name, deserializedAssembly.Name);
		}

		[TestMethod]
		public void DeserializeFromJson_IfTheTypeIsAssemblyInterfaceAndTheJsonValueIsAValidAssemblyName_ShouldDeserializeCorrectly()
		{
			var assembly = typeof(string).Assembly;
			var name = assembly.GetName().Name;

			var deserializedAssembly = this.JsonDeserialize<IAssembly>(this.Quote + name + this.Quote);
			Assert.AreEqual(assembly.FullName, deserializedAssembly.FullName);
			Assert.AreEqual(name, deserializedAssembly.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(JsonReaderException))]
		public void DeserializeFromJson_IfTheTypeIsAssemblyInterfaceAndTheJsonValueIsNotWithinQuotes_ShouldThrowAJsonReaderException()
		{
			try
			{
				this.JsonDeserialize<IAssembly>("Invalid-assembly-name");
			}
			catch(JsonReaderException jsonReaderException)
			{
				const string expectedMessage = "Error parsing Infinity value. Path '', line 1, position 2.";

				if(jsonReaderException.Message.Equals(expectedMessage, StringComparison.Ordinal) && jsonReaderException.InnerException == null)
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(JsonSerializationException))]
		public void DeserializeFromJson_IfTheTypeIsAssemblyWrapperAndTheJsonValueIsAnInvalidAssemblyName_ShouldThrowAJsonSerializationException()
		{
			try
			{
				this.JsonDeserialize<AssemblyWrapper>(this.Quote + "Invalid-assembly-name" + this.Quote);
			}
			catch(JsonSerializationException jsonSerializationException)
			{
				const string expectedMessage = "Error converting value \"Invalid-assembly-name\" to type 'RegionOrebroLan.Localization.Reflection.AssemblyWrapper'. Path '', line 1, position 23.";

				if(jsonSerializationException.Message.Equals(expectedMessage, StringComparison.Ordinal) && jsonSerializationException.InnerException is ArgumentException)
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(JsonSerializationException))]
		public void DeserializeFromJson_IfTheTypeIsAssemblyWrapperAndTheJsonValueIsAnInvalidToken_ShouldThrowAJsonSerializationException()
		{
			try
			{
				this.JsonDeserialize<AssemblyWrapper>("1");
			}
			catch(JsonSerializationException jsonSerializationException)
			{
				const string expectedMessage = "Error converting value 1 to type 'RegionOrebroLan.Localization.Reflection.AssemblyWrapper'. Path '', line 1, position 1.";

				if(jsonSerializationException.Message.Equals(expectedMessage, StringComparison.Ordinal) && jsonSerializationException.InnerException is ArgumentException)
					throw;
			}
		}

		[TestMethod]
		public void DeserializeFromJson_IfTheTypeIsAssemblyWrapperAndTheJsonValueIsAValidAssemblyFullName_ShouldDeserializeCorrectly()
		{
			var assembly = typeof(string).Assembly;
			var name = assembly.GetName().Name;

			var deserializedAssemblyWrapper = this.JsonDeserialize<AssemblyWrapper>(this.Quote + assembly.FullName + this.Quote);
			Assert.AreEqual(assembly.FullName, deserializedAssemblyWrapper.FullName);
			Assert.AreEqual(name, deserializedAssemblyWrapper.Name);
		}

		[TestMethod]
		public void DeserializeFromJson_IfTheTypeIsAssemblyWrapperAndTheJsonValueIsAValidAssemblyName_ShouldDeserializeCorrectly()
		{
			var assembly = typeof(string).Assembly;
			var name = assembly.GetName().Name;

			var deserializedAssemblyWrapper = this.JsonDeserialize<AssemblyWrapper>(this.Quote + name + this.Quote);
			Assert.AreEqual(assembly.FullName, deserializedAssemblyWrapper.FullName);
			Assert.AreEqual(name, deserializedAssemblyWrapper.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(JsonReaderException))]
		public void DeserializeFromJson_IfTheTypeIsAssemblyWrapperAndTheJsonValueIsNotWithinQuotes_ShouldThrowAJsonReaderException()
		{
			try
			{
				this.JsonDeserialize<AssemblyWrapper>("Invalid-assembly-name");
			}
			catch(JsonReaderException jsonReaderException)
			{
				const string expectedMessage = "Error parsing Infinity value. Path '', line 1, position 2.";

				if(jsonReaderException.Message.Equals(expectedMessage, StringComparison.Ordinal) && jsonReaderException.InnerException == null)
					throw;
			}
		}

		protected internal virtual AssemblyInterfaceTypeConverter GetRegisteredAssemblyInterfaceTypeConverter()
		{
			return (AssemblyInterfaceTypeConverter)TypeDescriptor.GetConverter(typeof(IAssembly));
		}

		[TestMethod]
		public void SerializeToJson_IfTheInstanceParameterIsAnAssemblyInterface_ShouldReturnTheAssemblyInterfaceFullNameWithinQuotes()
		{
			var assembly = this.AssemblyHelper.Wrap(typeof(string).Assembly);
			Assert.AreEqual(this.Quote + assembly.FullName + this.Quote, this.JsonSerialize(assembly));

			assembly = this.AssemblyHelper.Wrap(this.GetType().Assembly);
			Assert.AreEqual(this.Quote + assembly.FullName + this.Quote, this.JsonSerialize(assembly));
		}

		#endregion
	}
}