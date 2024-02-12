using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
	[TestClass]
	[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
	public static class Global
	{
		#region Fields

		private static IHostEnvironment _hostEnvironment;
		private static ILoggerFactory _loggerFactory;

		// ReSharper disable PossibleNullReferenceException
		public static readonly string ProjectDirectoryPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
		// ReSharper restore PossibleNullReferenceException

		public static readonly string TestRootDirectoryRelativePath = @"Test-resources\Root";

		#endregion

		#region Properties

		public static IHostEnvironment HostEnvironment
		{
			get
			{
				// ReSharper disable InvertIf
				if(_hostEnvironment == null)
				{
					const string applicationName = "Integration-tests";

					_hostEnvironment = new HostingEnvironment
					{
						ApplicationName = applicationName,
						ContentRootPath = ProjectDirectoryPath,
						ContentRootFileProvider = new PhysicalFileProvider(ProjectDirectoryPath),
						EnvironmentName = applicationName
					};
				}
				// ReSharper restore InvertIf

				return _hostEnvironment;
			}
		}

		public static ILoggerFactory LoggerFactory
		{
			get
			{
				// ReSharper disable InvertIf
				if(_loggerFactory == null)
				{
					var loggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() });

					_loggerFactory = loggerFactory;
				}
				// ReSharper restore InvertIf

				return _loggerFactory;
			}
		}

		#endregion

		#region Methods

		public static ServiceCollection CreateDefaultServices()
		{
			var services = new ServiceCollection();

			services.AddSingleton(HostEnvironment);
			services.AddSingleton(LoggerFactory);

			return services;
		}

		public static string GetDirectoryPath(Type type)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			var globalType = typeof(Global);

			if(type.Assembly != globalType.Assembly)
				throw new InvalidOperationException("It is not possible to get the directory-path for a type outside this assembly.");

			var @namespace = type.Namespace;

			var relativePath = @namespace.Substring(globalType.Namespace.Length).TrimStart('.').Replace(".", @"\", StringComparison.OrdinalIgnoreCase);

			return Path.Combine(ProjectDirectoryPath, relativePath);
		}

		[AssemblyInitialize]
		public static void Initialize(TestContext testContext)
		{
			if(testContext == null)
				throw new ArgumentNullException(nameof(testContext));

			var testRootDirectoryPath = Path.Combine(ProjectDirectoryPath, TestRootDirectoryRelativePath);

			if(Directory.Exists(testRootDirectoryPath))
			{
				Directory.Delete(testRootDirectoryPath, true);

				Thread.Sleep(200);
			}

			Directory.CreateDirectory(testRootDirectoryPath);
		}

		#endregion
	}
}