﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Views.Commands;
using PhoneGuitarTab.Data;
using PhoneGuitarTab.UI.Entities;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.ViewModels
{
    public class StartupViewModel : DataContextViewModel
    {
        #region Fields

        private readonly RatingService _ratingService;
        private bool _isSelectionEnabled;
        private string _backGroundImage;

        #endregion Fields

        #region Constructors

        [Dependency]
        public StartupViewModel(IDataContextService database, RatingService ratingService, MessageHub hub)
            : base(database, hub)
        {
            _ratingService = ratingService;
            RegisterEvents();
            CreateCommands();
            ProductVersion = App.Version;
        }

        #endregion Constructors

        #region Properties

        public string ProductVersion { get; set; }

        public bool IsSelectionEnabled
        {
            get { return _isSelectionEnabled; }
            set
            {
                _isSelectionEnabled = value;
                RaisePropertyChanged("IsSelectionEnabled");
            }
        }

        public string BackGroundImage
        {
            get { return _backGroundImage; }
            set
            {
                _backGroundImage = value;
                RaisePropertyChanged("BackGroundImage");
            }
        }

        #endregion Properties

        #region Commands

        public ExecuteCommand<string> GoTo { get; private set; }

        public ExecuteCommand<object> GoToTabView { get; private set; }

        public ExecuteCommand Review { get; private set; }


        public ExecuteCommand<int> RemoveTab { get; private set; }

        #endregion Commands

        #region Command handlers

        private void DoGoTo(string pageName)
        {
            NavigationService.NavigateTo(pageName);
        }

        private void DoGoToTabView(object args)
        {
            var tabEntity = (args as TabEntity);
            if (tabEntity != null)
            {
                Tab tab = (from Tab t in Database.Tabs
                    where t.Id == tabEntity.Id
                    select t).Single();
                NavigationService.NavigateToTab(new Dictionary<string, object> {{"Tab", tab}});
            }
        }

        private void DoReview()
        {
            _ratingService.RateApp();
            new MarketplaceReviewTask().Show();
        }

        #endregion Command handlers

        #region Override members

        protected override void DataBind()
        {
        }

        public override void LoadStateFrom(IDictionary<string, object> state)
        {
            base.LoadStateFrom(state);
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            if (state.ContainsKey("TabUrl"))
            {
                String tabUrl = (String) state["TabUrl"];
                string[] parsed = tabUrl.Split('?');
                if (parsed.Length > 1) //If Url contains a tab ID array lenght will be 2.
                {
                    var tabId = System.Convert.ToInt16(parsed[1]);
                    NavigationService.NavigateToTab(new Dictionary<string, object> {{"Tab", Database.GetTabById(tabId)}});
                }
            }
        }

        public override void SaveStateTo(IDictionary<string, object> state)
        {
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Enabled;

            base.SaveStateTo(state);
        }

        #endregion Override members

        #region Helper methods

        private void CreateCommands()
        {
            GoTo = new ExecuteCommand<string>(DoGoTo);
            GoToTabView = new ExecuteCommand<object>(DoGoToTabView);
            Review = new ExecuteCommand(DoReview);
        }

        private void RegisterEvents()
        {
            Hub.SelectorIsSelectionEnabled += (o, enabled) => { IsSelectionEnabled = enabled; };
            Hub.BackGroundImageChangeActivity += (o, image) => { BackGroundImage = image; };
        }

        #endregion Helper methods
    }
}