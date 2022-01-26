using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegionOrebroLan.Localization.Collections.ObjectModel;
using RegionOrebroLan.Localization.Configuration;
using RegionOrebroLan.Localization.Reflection;
using RegionOrebroLan.Localization.Validation;
using RegionOrebroLan.Logging.Extensions;

namespace RegionOrebroLan.Localization
{
	public class LocalizationSettings : ILocalizationSettings
	{
		#region Fields

		private bool _alphabeticalSorting;
		private IEnumerable<IAssembly> _configuredEmbeddedResourceAssemblies;
		private IDirectoryInfo _fileResourcesDirectory;
		private bool _includeParentCultures;
		private bool _throwErrors;

		#endregion

		#region Constructors

		[CLSCompliant(false)]
		[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
		public LocalizationSettings(IFileResourcesDirectoryValidator fileResourcesDirectoryValidator, ILoggerFactory loggerFactory, IOptionsMonitor<LocalizationOptions> optionsMonitor, ILocalizationOptionsResolver optionsResolver)
		{
			this.FileResourcesDirectoryValidator = fileResourcesDirectoryValidator ?? throw new ArgumentNullException(nameof(fileResourcesDirectoryValidator));
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());

			if(optionsMonitor == null)
				throw new ArgumentNullException(nameof(optionsMonitor));

			var options = optionsMonitor.CurrentValue ?? throw new ArgumentException("The current value of the options-monitor can not be null.", nameof(optionsMonitor));

			this.OptionsResolver = optionsResolver ?? throw new ArgumentNullException(nameof(optionsResolver));

			var resolvedOptions = optionsResolver.Resolve(options);

			this._alphabeticalSorting = resolvedOptions.AlphabeticalSorting;
			this._configuredEmbeddedResourceAssemblies = resolvedOptions.EmbeddedResourceAssemblies;
			this._fileResourcesDirectory = resolvedOptions.FileResourcesDirectory;
			this._includeParentCultures = resolvedOptions.IncludeParentCultures;
			this._throwErrors = resolvedOptions.ThrowErrors;

			var observableCollection = new ObservableSetCollection<IAssembly>
			{
				resolvedOptions.EmbeddedResourceAssemblies
			};

			observableCollection.CollectionChanged += this.OnEmbeddedResourceAssembliesCollectionChanged;

			this.EmbeddedResourceAssemblies = observableCollection;

			this.OptionsChangeListener = optionsMonitor.OnChange(this.OnOptionsChanged);
		}

		#endregion

		#region Events

		public event EventHandler AlphabeticalSortingChanged;
		public event EventHandler<NotifyCollectionChangedEventArgs> EmbeddedResourceAssembliesChanged;
		public event EventHandler FileResourcesDirectoryChanged;
		public event EventHandler IncludeParentCulturesChanged;

		#endregion

		#region Properties

		public virtual bool AlphabeticalSorting
		{
			get => this._alphabeticalSorting;
			set
			{
				if(this._alphabeticalSorting == value)
					return;

				this._alphabeticalSorting = value;
				this.OnAlphabeticalSortingChanged();
			}
		}

		[SuppressMessage("Usage", "CA2227:Collection properties should be read only")]
		protected internal virtual IEnumerable<IAssembly> ConfiguredEmbeddedResourceAssemblies
		{
			get => this._configuredEmbeddedResourceAssemblies;
			set => this._configuredEmbeddedResourceAssemblies = value;
		}

		public virtual IList<IAssembly> EmbeddedResourceAssemblies { get; }

		public virtual IDirectoryInfo FileResourcesDirectory
		{
			get => this._fileResourcesDirectory;
			set
			{
				const char directorySeparator = '\\';

				if(string.Equals(this._fileResourcesDirectory?.FullName?.TrimEnd(directorySeparator), value?.FullName?.TrimEnd(directorySeparator), StringComparison.OrdinalIgnoreCase))
					return;

				this.FileResourcesDirectoryValidator.Validate(value);

				this._fileResourcesDirectory = value;
				this.OnFileResourcesDirectoryChanged();
			}
		}

		protected internal virtual IFileResourcesDirectoryValidator FileResourcesDirectoryValidator { get; }

		public virtual bool IncludeParentCultures
		{
			get => this._includeParentCultures;
			set
			{
				if(this._includeParentCultures == value)
					return;

				this._includeParentCultures = value;
				this.OnIncludeParentCulturesChanged();
			}
		}

		[CLSCompliant(false)]
		protected internal virtual ILogger Logger { get; }

		protected internal virtual IDisposable OptionsChangeListener { get; }
		protected internal virtual ILocalizationOptionsResolver OptionsResolver { get; }
		public virtual IDictionary<Exception, DateTime> RuntimeConfigurationExceptions { get; } = new Dictionary<Exception, DateTime>();

		public virtual bool ThrowErrors
		{
			get => this._throwErrors;
			set => this._throwErrors = value;
		}

		#endregion

		#region Methods

		protected internal virtual void OnAlphabeticalSortingChanged()
		{
			this.AlphabeticalSortingChanged?.Invoke(this, EventArgs.Empty);
		}

		protected internal virtual void OnEmbeddedResourceAssembliesChanged(NotifyCollectionChangedEventArgs e)
		{
			this.EmbeddedResourceAssembliesChanged?.Invoke(this, e);
		}

		protected internal virtual void OnEmbeddedResourceAssembliesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.OnEmbeddedResourceAssembliesChanged(e);
		}

		protected internal virtual void OnFileResourcesDirectoryChanged()
		{
			this.FileResourcesDirectoryChanged?.Invoke(this, EventArgs.Empty);
		}

		protected internal virtual void OnIncludeParentCulturesChanged()
		{
			this.IncludeParentCulturesChanged?.Invoke(this, EventArgs.Empty);
		}

		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
		protected internal virtual void OnOptionsChanged(LocalizationOptions localizationOptions, string name)
		{
			this.Logger.LogDebugIfEnabled("Options changed - begin");

			try
			{
				var resolvedOptions = this.OptionsResolver.Resolve(localizationOptions);

				var embeddedResourceAssemblies = resolvedOptions.EmbeddedResourceAssemblies.ToList();

				foreach(var assembly in this.EmbeddedResourceAssemblies.Except(this.ConfiguredEmbeddedResourceAssemblies))
				{
					if(!embeddedResourceAssemblies.Contains(assembly))
						embeddedResourceAssemblies.Add(assembly);
				}

				this.Logger.LogDebugIfEnabled("Options changed - setting new options");

				this.AlphabeticalSorting = resolvedOptions.AlphabeticalSorting;
				this.ConfiguredEmbeddedResourceAssemblies = resolvedOptions.EmbeddedResourceAssemblies;
				this.FileResourcesDirectory = resolvedOptions.FileResourcesDirectory;
				this.IncludeParentCultures = resolvedOptions.IncludeParentCultures;
				this.ThrowErrors = resolvedOptions.ThrowErrors;

				if(this.EmbeddedResourceAssemblies.SequenceEqual(embeddedResourceAssemblies))
				{
					this.Logger.LogDebugIfEnabled("Options changed - the embedded-resource-assemblies have not changed, no reason to set");

					return;
				}

				var previousEmbeddedResourceAssemblies = this.EmbeddedResourceAssemblies.ToArray();

				var observableSetCollection = (ObservableSetCollection<IAssembly>)this.EmbeddedResourceAssemblies;

				observableSetCollection.CollectionChanged -= this.OnEmbeddedResourceAssembliesCollectionChanged;

				observableSetCollection.Clear();

				foreach(var assembly in embeddedResourceAssemblies)
				{
					observableSetCollection.Add(assembly);
				}

				var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, embeddedResourceAssemblies, previousEmbeddedResourceAssemblies);

				this.OnEmbeddedResourceAssembliesChanged(e);

				observableSetCollection.CollectionChanged += this.OnEmbeddedResourceAssembliesCollectionChanged;
			}
			catch(Exception exception)
			{
				const string message = "Error on options-changed.";

				this.Logger.LogErrorIfEnabled(exception, message);

				this.RuntimeConfigurationExceptions.Add(new InvalidOperationException(message, exception), DateTime.Now);
			}

			this.Logger.LogDebugIfEnabled("Options changed - end");
		}

		#endregion

		#region Other members

		#region Finalizers

		~LocalizationSettings()
		{
			this.OptionsChangeListener?.Dispose();

			var observableSetCollection = (ObservableSetCollection<IAssembly>)this.EmbeddedResourceAssemblies;

			if(observableSetCollection != null)
				observableSetCollection.CollectionChanged -= this.OnEmbeddedResourceAssembliesCollectionChanged;
		}

		#endregion

		#endregion
	}
}