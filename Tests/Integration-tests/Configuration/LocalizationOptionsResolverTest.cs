using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Localization.Configuration;

namespace IntegrationTests.Configuration
{
	[TestClass]
	public class LocalizationOptionsResolverTest : IntegrationTest
	{
		#region Methods

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Resolve_IfTheLocalizationOptionsParameterContainsAnInvalidEmbeddedResourceAssemblyName_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptionsResolver = (LocalizationOptionsResolver)serviceProvider.GetRequiredService<ILocalizationOptionsResolver>();

			var localizationOptions = new LocalizationOptions();

			localizationOptions.EmbeddedResourceAssemblies.Add("Invalid-Assembly-Name");

			try
			{
				localizationOptionsResolver.Resolve(localizationOptions);
			}
			catch(InvalidOperationException invalidOperationException)
			{
				const string message = "Embedded-resource-assemblies-exception: The patterns-collection contains invalid values. Values: \"Invalid-Assembly-Name\"";
				const string innerExceptionMessage = "The assembly \"Invalid-Assembly-Name\" is not loaded at runtime.";

				if(invalidOperationException.Message.Equals(message, StringComparison.OrdinalIgnoreCase) && invalidOperationException.InnerException is InvalidOperationException && invalidOperationException.InnerException.Message.Equals(innerExceptionMessage, StringComparison.OrdinalIgnoreCase))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Resolve_IfTheLocalizationOptionsParameterContainsDuplicateEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptionsResolver = (LocalizationOptionsResolver)serviceProvider.GetRequiredService<ILocalizationOptionsResolver>();

			var localizationOptions = new LocalizationOptions();

			localizationOptions.EmbeddedResourceAssemblies.Add("System");
			localizationOptions.EmbeddedResourceAssemblies.Add("System");

			try
			{
				localizationOptionsResolver.Resolve(localizationOptions);
			}
			catch(ArgumentException argumentException)
			{
				const string message = "Embedded-resource-assemblies-exception: The patterns-collection can not contain duplicate values. Values: \"System\", \"System\"";

				if(argumentException.Message.StartsWith(message, StringComparison.OrdinalIgnoreCase) && argumentException.ParamName.Equals("patterns", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Resolve_IfTheLocalizationOptionsParameterContainsEmptyValuedEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptionsResolver = (LocalizationOptionsResolver)serviceProvider.GetRequiredService<ILocalizationOptionsResolver>();

			var localizationOptions = new LocalizationOptions();

			localizationOptions.EmbeddedResourceAssemblies.Add(string.Empty);

			try
			{
				localizationOptionsResolver.Resolve(localizationOptions);
			}
			catch(ArgumentException argumentException)
			{
				const string message = "Embedded-resource-assemblies-exception: The patterns-collection can not contain empty values. Values: \"\"";

				if(argumentException.Message.StartsWith(message, StringComparison.OrdinalIgnoreCase) && argumentException.ParamName.Equals("patterns", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Resolve_IfTheLocalizationOptionsParameterContainsNullValuedEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptionsResolver = (LocalizationOptionsResolver)serviceProvider.GetRequiredService<ILocalizationOptionsResolver>();

			var localizationOptions = new LocalizationOptions();

			localizationOptions.EmbeddedResourceAssemblies.Add(null);

			try
			{
				localizationOptionsResolver.Resolve(localizationOptions);
			}
			catch(ArgumentException argumentException)
			{
				const string message = "Embedded-resource-assemblies-exception: The patterns-collection can not contain null-values. Values: null";

				if(argumentException.Message.StartsWith(message, StringComparison.OrdinalIgnoreCase) && argumentException.ParamName.Equals("patterns", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Resolve_IfTheLocalizationOptionsParameterContainsWhiteSpaceOnlyValuedEmbeddedResourceAssemblies_ShouldThrowAnInvalidOperationException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptionsResolver = (LocalizationOptionsResolver)serviceProvider.GetRequiredService<ILocalizationOptionsResolver>();

			var localizationOptions = new LocalizationOptions();

			localizationOptions.EmbeddedResourceAssemblies.Add(" ");

			try
			{
				localizationOptionsResolver.Resolve(localizationOptions);
			}
			catch(ArgumentException argumentException)
			{
				const string message = "Embedded-resource-assemblies-exception: The patterns-collection can not contain white-space-only values. Values: \" \"";

				if(argumentException.Message.StartsWith(message, StringComparison.OrdinalIgnoreCase) && argumentException.ParamName.Equals("patterns", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(DirectoryNotFoundException))]
		public void Resolve_IfTheLocalizationOptionsParameterHasAnAbsoluteFileResourcesDirectoryPathThatDoesNotExist_ShouldThrowADirectoryNotFoundException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptionsResolver = (LocalizationOptionsResolver)serviceProvider.GetRequiredService<ILocalizationOptionsResolver>();

			var fileResourcesDirectoryPath = "Z:\\" + Guid.NewGuid();

			var localizationOptions = new LocalizationOptions
			{
				FileResourcesDirectoryPath = fileResourcesDirectoryPath
			};

			try
			{
				localizationOptionsResolver.Resolve(localizationOptions);
			}
			catch(DirectoryNotFoundException directoryNotFoundException)
			{
				var message = $"File-resources-directory-exception: The directory \"{fileResourcesDirectoryPath}\" does not exist.";

				if(directoryNotFoundException.Message.Equals(message, StringComparison.OrdinalIgnoreCase))
					throw;
			}
		}

		[TestMethod]
		public void Resolve_IfTheLocalizationOptionsParameterHasANullValuedFileResourcesDirectoryPath_ShouldReturnAResolvedLocalizationOptionsWithANullValuedFileResourcesDirectory()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptionsResolver = (LocalizationOptionsResolver)serviceProvider.GetRequiredService<ILocalizationOptionsResolver>();

			var localizationOptions = new LocalizationOptions
			{
				FileResourcesDirectoryPath = null
			};

			var resolvedLocalizationOptions = localizationOptionsResolver.Resolve(localizationOptions);

			Assert.IsNotNull(resolvedLocalizationOptions);
			Assert.IsNull(resolvedLocalizationOptions.FileResourcesDirectory);
		}

		[TestMethod]
		[ExpectedException(typeof(DirectoryNotFoundException))]
		public void Resolve_IfTheLocalizationOptionsParameterHasARelativeFileResourcesDirectoryPathThatDoesNotExist_ShouldThrowADirectoryNotFoundException()
		{
			var serviceProvider = this.BuildServiceProvider("Configuration-Empty.json");

			var localizationOptionsResolver = (LocalizationOptionsResolver)serviceProvider.GetRequiredService<ILocalizationOptionsResolver>();

			var localizationOptions = new LocalizationOptions
			{
				FileResourcesDirectoryPath = Guid.NewGuid().ToString()
			};

			try
			{
				localizationOptionsResolver.Resolve(localizationOptions);
			}
			catch(DirectoryNotFoundException directoryNotFoundException)
			{
				const string message = "File-resources-directory-exception: The directory ";

				if(directoryNotFoundException.Message.StartsWith(message, StringComparison.OrdinalIgnoreCase))
					throw;
			}
		}

		#endregion
	}
}