using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace YLocalization
{
    public class LocalizationManager : ILocalizationManager
    {
        /// <summary>
        /// Cached ResourceManagers for each ResourceSet supported requested.       
        /// </summary>
        internal readonly List<ResourceManager> ResourceManagers = new List<ResourceManager>();

        private CultureInfo culture;
        public void AddAssembly(string assemblyName, string resourcePath = "Resources.LocalizableResources")
        {
            var assembly = Assembly.Load(new AssemblyName(assemblyName));
            AddAssembly(assembly, resourcePath);
        }

        public event EventHandler CultureChanged;

        public CultureInfo Culture
        {
            get => culture;
            set
            {
                if (!Equals(culture, value))
                {
                    culture = value;
                    CultureChanged?.Invoke(this, EventArgs.Empty);
                }

            }
        }

        public virtual void ChangeCulture(CultureInfo cultureInfo)
        {
            Culture = cultureInfo;
        }

        public string GetString(string key, params object[] parameters)
        {
            foreach (var resourceManager in ResourceManagers)
            {
                var str = GetString(resourceManager, key, Culture, parameters);
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }
            return $"?{key}?";
        }

        private static string GetString(ResourceManager manager, string stringName, CultureInfo cultureInfo, params object[] parameters)
        {
            if (manager == null)
            {
                return null;
            }
            string str;
            string unFormattedString = string.Empty;
            try
            {
                unFormattedString = manager.GetString(stringName, cultureInfo) ?? String.Empty;
            }
            catch (MissingManifestResourceException)
            {
            }
            catch (NullReferenceException)
            {
            }
            try
            {
                str = string.Format(unFormattedString, parameters);
            }
            catch (FormatException)
            {
                str = unFormattedString;
            }
            return str;
        }

        public void AddResourceManager(ResourceManager resourceManager)
        {
            if (resourceManager != null)
            {
                ResourceManagers.Add(resourceManager);
            }
        }

        public void AddAssembly(Assembly assembly, string resourcePath = "Resources.LocalizableResources")
        {
            var resName = GetDefaultResourceName(assembly.ManifestModule.Name, resourcePath);
            ResourceManagers.Add(new ResourceManager(resName, assembly));
        }
        private static string GetDefaultResourceName(string assemblyModuleName, string resourcePath)
        {
            string stringResourceName;
            if (assemblyModuleName.ToLower().Contains(".exe"))
            {
                stringResourceName = $"{assemblyModuleName.Remove(assemblyModuleName.ToLower().LastIndexOf(".exe", StringComparison.Ordinal))}.{resourcePath}";
                return stringResourceName;
            }
            stringResourceName = $"{assemblyModuleName.Remove(assemblyModuleName.ToLower().LastIndexOf(".dll", StringComparison.Ordinal))}.{resourcePath}";
            return stringResourceName;
        }

    }
}