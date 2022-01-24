using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
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

		private static IHostingEnvironment _hostingEnvironment;
		private static ILoggerFactory _loggerFactory;

		// ReSharper disable PossibleNullReferenceException
		public static readonly string ProjectDirectoryPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
		// ReSharper restore PossibleNullReferenceException

		public static readonly string TestRootDirectoryRelativePath = @"Test-resources\Root";

		#endregion

		#region Properties

		public static IHostingEnvironment HostingEnvironment
		{
			get
			{
				// ReSharper disable InvertIf
				if(_hostingEnvironment == null)
				{
					const string applicationName = "Integration-tests";

					_hostingEnvironment = new HostingEnvironment
					{
						ApplicationName = applicationName,
						ContentRootPath = ProjectDirectoryPath,
						ContentRootFileProvider = new PhysicalFileProvider(ProjectDirectoryPath),
						EnvironmentName = applicationName
					};
				}
				// ReSharper restore InvertIf

				return _hostingEnvironment;
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

		[AssemblyCleanup]
		public static void Cleanup()
		{
			var testRootDirectoryPath = Path.Combine(ProjectDirectoryPath, TestRootDirectoryRelativePath);

			Thread.Sleep(200);

			if(Directory.Exists(testRootDirectoryPath))
				Directory.Delete(testRootDirectoryPath, true);
		}

		public static ServiceCollection CreateDefaultServices()
		{
			var services = new ServiceCollection();

			services.AddSingleton(HostingEnvironment);
			services.AddSingleton(LoggerFactory);

			return services;
		}

		[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
		public static string GetDirectoryPath(Type type)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			if(type.Assembly != typeof(Global).Assembly)
				throw new InvalidOperationException("It is not possible to get the directory-path for a type outside this assembly.");

			var @namespace = type.Namespace;
			var assemblyName = type.Assembly.GetName().Name;

			if(!@namespace.StartsWith(assemblyName, StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("The namespace must start with the assembly-name.");

			var relativePath = @namespace.Substring(assemblyName.Length).TrimStart('.').Replace(".", @"\", StringComparison.OrdinalIgnoreCase);

			return Path.Combine(ProjectDirectoryPath, relativePath);
		}

		[AssemblyInitialize]
		[SuppressMessage("Usage", "CA1801:Review unused parameters")]
		public static void Initialize(TestContext testContext)
		{
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