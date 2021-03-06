﻿using System;
using System.Linq;
using PhoneGuitarTab.Core.Bootstrap;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.ViewModels;

namespace PhoneGuitarTab.UI.Bootstrap
{
    /// <summary>
    ///     Represents an application bootstrapper.
    ///     NOTE I prefer here "Code over Configuration" approach
    /// </summary>
    public class ComponentRoot
    {
        private readonly IContainer _container;

        public ComponentRoot()
        {
            try
            {
                _container = new Container();

                _container.Register(Component.For<IBootstrapperPlugin>().Use<CoreBootstrapperPlugin>().Named("Core").Singleton());
                _container.Register(Component.For<IBootstrapperPlugin>().Use<DataBootstrapperPlugin>().Named("Data").Singleton());
                _container.Register(Component.For<IBootstrapperPlugin>().Use<NavigationBootstrapperPlugin>().Named("Navigation").Singleton());
                _container.Register(Component.For<IBootstrapperPlugin>().Use<CloudBootstrapperPlugin>().Named("Cloud").Singleton());

                _container.ResolveAll<IBootstrapperPlugin>().ToList()
                    .Aggregate(true, (current, task) => current & task.Run());
            }
            catch (Exception fatalException)
            {
                // suppress any error here, let fail when app is initialized
                App.FatalException = fatalException;
            }
        }

        #region View models

        /// <summary>
        ///     Gets the Main property which defines the main viewmodel.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public StartupViewModel Startup
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.Startup) as StartupViewModel; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GroupViewModel Group
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.Group) as GroupViewModel; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SearchViewModel Search
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.Search) as SearchViewModel; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TextTabViewModel TextTab
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.TextTab) as TextTabViewModel; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AboutViewModel About
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.About) as AboutViewModel; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public CollectionViewModel Collection
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.Collection) as CollectionViewModel; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public StaveTabViewModel StaveTab
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.StaveTab) as StaveTabViewModel; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SynchronizeViewModel Synchronize
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.Synchronize) as SynchronizeViewModel; }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DiscoverViewModel Discover
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.Discover) as DiscoverViewModel; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public SuggestedGroupViewModel SuggestedGroup
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.SuggestedGroup) as SuggestedGroupViewModel; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1822:MarkMembersAsStatic",
         Justification = "This non-static member is needed for data binding purposes.")]
        public GenreGroupsViewModel GenreGroups
        {
            get { return _container.Resolve<ViewModel>(NavigationViewNames.GenreGroups) as GenreGroupsViewModel; }
        }

        #endregion View models
    }
}