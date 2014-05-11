﻿using System;
using System.Collections.Generic;
using System.Linq;
using PhoneGuitarTab.Core.Bootstrap;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.Core.Views;
using PhoneGuitarTab.UI.ViewModels;

namespace PhoneGuitarTab.UI.Bootstrap
{
    /// <summary>
    ///     Processes navigation-specific staff
    /// </summary>
    public class NavigationBootstrapperPlugin : IBootstrapperPlugin
    {
        private readonly Dictionary<Tuple<string, Uri>, Type> _pageMapping = new Dictionary<Tuple<string, Uri>, Type>
        {
            { new Tuple<string, Uri>(Strings.Startup, new Uri(@"/Views/StartupView.xaml", UriKind.Relative)), typeof (StartupViewModel) },
            { new Tuple<string, Uri>(Strings.Search, new Uri(@"/Views/SearchForBandView.xaml", UriKind.Relative)), typeof (SearchViewModel) },
            { new Tuple<string, Uri>(Strings.MainSearch, new Uri(@"/Views/SearchView.xaml", UriKind.Relative)), typeof (SearchViewModel) },
            { new Tuple<string, Uri>(Strings.TextTab, new Uri(@"/Views/TextTabView.xaml", UriKind.Relative)), typeof (TextTabViewModel) },
            { new Tuple<string, Uri>(Strings.StaveTab, new Uri(@"/Views/StaveTabView.xaml", UriKind.Relative)),typeof (StaveTabViewModel) },
            { new Tuple<string, Uri>(Strings.About, new Uri(@"/Views/AboutView.xaml", UriKind.Relative)), typeof (AboutViewModel) },
            { new Tuple<string, Uri>(Strings.Group, new Uri(@"/Views/GroupView.xaml", UriKind.Relative)), typeof (GroupViewModel) },
            { new Tuple<string, Uri>(Strings.Synchronize, new Uri(@"/Views/SynchronizeView.xaml", UriKind.Relative)), typeof (SynchronizeViewModel) },
            // TODO restyling artefact?
            { new Tuple<string, Uri>(Strings.Collection, new Uri(@"/Views/CollectionView.xaml", UriKind.Relative)),typeof (CollectionViewModel) },
        };

        public string Name
        {
            get { return "Navigation"; }
        }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Run()
        {
            // register view models
            _pageMapping.ToList().ForEach(p => Container.Register(Component
                .For<ViewModel>()
                .Use(p.Value, new object[0])
                .Named(p.Key.Item1)
                .Singleton()));

            //register navigation service
            var uriMapping = new Dictionary<string, Uri>();
            var viewModelMapping = new Dictionary<string, Core.Primitives.Lazy<ViewModel>>();
            _pageMapping.ToList()
                .ForEach(p =>
                {
                    uriMapping.Add(p.Key.Item1, p.Key.Item2);
                    viewModelMapping.Add(p.Key.Item1,
                        new Core.Primitives.Lazy<ViewModel>(() => Container.Resolve<ViewModel>(p.Key.Item1)));
                });

            Container.Register(
                Component.For<INavigationService>().Use<NavigationService>(uriMapping, viewModelMapping).Singleton());

            return true;
        }

        public bool Update()
        {
            return true;
        }
    }
}