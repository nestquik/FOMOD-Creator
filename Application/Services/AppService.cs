﻿using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using FomodInfrastructure;
using FomodInfrastructure.Interfaces;
using FomodModel.Base;
using Microsoft.Practices.ServiceLocation;
using Module.Editor.View;
using Module.Editor.ViewModel;
using Prism.Regions;

namespace MainApplication.Services
{
    public class AppService : IAppService
    {
        public AppService(IServiceLocator serviceLocator, IRegionManager regionManager)
        {
            _serviceLocator = serviceLocator;
            _regionManager = regionManager;
        }
        
        #region Services

        private readonly IRegionManager _regionManager;

        private readonly IServiceLocator _serviceLocator;

        #endregion

        #region IAppService

        public void CloseApp()
        {
            Application.Current.MainWindow.Close();
        }
        
        public string[] CommandLineArgs { get; } = Environment.GetCommandLineArgs();

        public Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

        public bool IsOpenProjectsFromCommandLine { get; set; } 

        public void CreateEditorModule<T>(IRepository<T> repository) where T : IData
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var o in _regionManager.Regions[Names.MainContentRegion].Views)
            {
                var element = o as MainEditorView;
                var b = (element?.DataContext as MainEditorViewModel)?.Data.FirstOrDefault(i => i.DataSource == repository.Data?.DataSource);
                if (b == null)
                    continue;
                _regionManager.Regions[Names.MainContentRegion].Activate(element);
                return;
            }
            var view = _serviceLocator.GetInstance<object>(nameof(MainEditorView)) as FrameworkElement;
            var detailsRegion = _regionManager.Regions[Names.MainContentRegion];
            var detailsRegionManager = detailsRegion.Add(view, null, true);
            if (view == null)
                return;
            var mainEditorViewModel = view.DataContext as MainEditorViewModel;
            mainEditorViewModel?.ConfigurateViewModel(detailsRegionManager, repository as IRepository<ProjectRoot>);
            detailsRegion.Activate(view);
        }

        public XElement GetXElementResource(string path)
        {
            var assembly = Assembly.GetAssembly(typeof(ProjectRoot));
            using (var stream = assembly.GetManifestResourceStream(path))
                return XElement.Load(stream);
        }

        #endregion
    }
}