using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using Action = MediaPortal.GUI.Library.Action;
using MediaPortal.Util;
using System.Xml;
using System.IO;
using WindowPlugins.GUITVSeries.Online_Parsing_Classes;

namespace WindowPlugins.GUITVSeries.GUI
{
    public class GUIActors : GUIWindow
    {
        #region Skin Controls

        [SkinControlAttribute(50)]
        protected GUIFacadeControl FacadeActors = null;

        [SkinControlAttribute(2)]
        protected GUIButtonControl ButtonLayouts = null;

        [SkinControlAttribute(3)]
        protected GUIButtonControl ButtonRefresh = null;

        #endregion

        #region Enums
        
        public enum Layout
        {
            List = 0,
            SmallIcons = 1,
            LargeIcons = 2,
            Filmstrip = 3, 
        }

        #endregion

        #region Constructor

        public GUIActors() { } 

        #endregion

        #region Private Properties

        private bool StopDownload { get; set; }
        private Layout CurrentLayout { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// set series id before loading window
        /// </summary>
        public static int SeriesId { get; set; }

        #endregion

        #region Base Overrides

        public override int GetID
        {
            get { return 9816; }
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\TVSeries.Actors.xml");
        }

        protected override void OnPageLoad()
        {
            // set window name
            GUIPropertyManager.SetProperty("#currentmodule", Translation.Actors);

            // clear any properties from previous series
            ClearProperties();

            // set default layout
            int defaultLayout = 0;
            int.TryParse(DBOption.GetOptions(DBOption.cActorLayout), out defaultLayout);
            CurrentLayout = (Layout)defaultLayout;
            
            // update button label
            GUIControl.SetControlLabel(GetID, ButtonLayouts.GetID, GetLayoutTranslation(CurrentLayout));

            // retrieve actors and load into facade
            DownloadActorsList();
        }

        protected override void OnPageDestroy(int newWindowId)
        {
            // stop any background tasks
            StopDownload = true;            
            GUIConnector.Instance.StopBackgroundTask();

            // save current layout
            DBOption.SetOptions(DBOption.cActorLayout, (int)CurrentLayout);
        }

        protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
        {
            // wait for any background action to finish
            if (GUIConnector.Instance.IsBusy) return;

            if (control == ButtonLayouts)
            {
                ShowLayoutsMenu();
            }

            if (control == ButtonRefresh)
            {
                RefreshActors();
            }

            base.OnClicked(controlId, control, actionType);
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                    break;
            }
            base.OnAction(action);
        }
        #endregion

        #region Private Methods

        private void DownloadActorsList()
        {
            GUIConnector.Instance.ExecuteInBackgroundAndCallback(() =>
            {
                return GetActors();
            },
            delegate(bool success, object result)
            {
                if (success)
                {
                    List<DBActor> actors = result as List<DBActor>;
                    LoadFacade(actors);
                }
            }, Translation.GettingActors, true);
        }

        private List<DBActor> GetActors()
        {
            // Download actor list
            GetActors actors = new GetActors(SeriesId);
            return actors.Actors;
        }
        
        private void RefreshActors()
        {
            // clear facade
            GUIControl.ClearControl(GetID, FacadeActors.GetID);

            // delete thumbs
            DeleteActorThumbs();

            // clear properties
            ClearProperties();
            
            // delete this series actors from database
            DBActor.ClearDB(SeriesId);

            // reload
            DownloadActorsList();
        }

        private void DeleteActorThumbs()
        {
            GetActors actors = new GetActors(SeriesId);

            foreach (DBActor actor in actors.Actors)
            {
                string fileName = actor.Image;
                if (string.IsNullOrEmpty(fileName)) continue;

                try
                {
                    File.Delete(actor.Image);
                }
                catch (Exception e)
                {
                    MPTVSeriesLog.Write("Unable to delete actor image '{0}', {1}", fileName, e.Message);
                }
            }
        }

        private void LoadFacade(List<DBActor> actors)
        {
            // clear facade
            GUIControl.ClearControl(GetID, FacadeActors.GetID);
            
            // set number of actors property
            GUIPropertyManager.SetProperty("#itemcount", actors.Count().ToString());

            // notify user if no actors to display and backout of window
            if (actors.Count == 0)
            {
                SetProperty("NoActors", Translation.NoActors);
                TVSeriesPlugin.ShowNotifyDialog(Translation.Actors, Translation.NoActors);
                GUIWindowManager.ShowPreviousWindow();
            }

            // Add each actor to the list
            foreach (var actor in actors)
            {
                GUIActorListItem actorItem = new GUIActorListItem(actor.ToString());

                actorItem.Item = actor;
                actorItem.IconImage = "defaultActor.png";
                actorItem.IconImageBig = "defaultActor.png";
                actorItem.ThumbnailImage = "defaultActor.png";
                actorItem.OnItemSelected += OnActorSelected;
                Utils.SetDefaultIcons(actorItem);
                FacadeActors.Add(actorItem);
            }

            FacadeActors.SelectedListItemIndex = 0;

            // Download actor images async and set to facade
            GetImages(actors);

            // Set Facade Layout
            FacadeActors.CurrentLayout = (GUIFacadeControl.Layout)CurrentLayout;
            GUIControl.FocusControl(GetID, FacadeActors.GetID);
        }

        private void SetProperty(string property, string value)
        {
            string propertyValue = string.IsNullOrEmpty(value) ? "N/A" : value;
            string propertyKey = string.Concat("#TVSeries.Actor.", property);
            GUIPropertyManager.SetProperty(propertyKey, propertyValue);
        }

        private void PublishSkinProperties(DBActor actor)
        {
            SetProperty("Name", actor.Name);
            SetProperty("Role", actor.Role);
            SetProperty("SeriesID", actor.SeriesId.ToString());
            SetProperty("Image", actor.Image);
        }

        private void ClearProperties()
        {
            SetProperty("Name", " ");
            SetProperty("Role", " ");
            SetProperty("SeriesID", " ");
            SetProperty("Image", " ");
        }

        private void OnActorSelected(GUIListItem item, GUIControl parent)
        {
            PublishSkinProperties((item as GUIActorListItem).Item as DBActor);
        }

        private void GetImages(List<DBActor> itemsWithThumbs)
        {
            StopDownload = false;

            // split the downloads in 5+ groups and do multithreaded downloading
            int groupSize = (int)Math.Max(1, Math.Floor((double)itemsWithThumbs.Count / 5));
            int groups = (int)Math.Ceiling((double)itemsWithThumbs.Count / groupSize);

            for (int i = 0; i < groups; i++)
            {
                List<DBActor> groupList = new List<DBActor>();
                for (int j = groupSize * i; j < groupSize * i + (groupSize * (i + 1) > itemsWithThumbs.Count ? itemsWithThumbs.Count - groupSize * i : groupSize); j++)
                {
                    groupList.Add(itemsWithThumbs[j]);
                }

                new System.Threading.Thread(delegate(object o)
                {
                    List<DBActor> items = (List<DBActor>)o;
                    foreach (DBActor item in items)
                    {
                        // stop download if we have exited window
                        if (StopDownload) break;

                        string remoteThumb = item.ImageRemotePath;
                        if (string.IsNullOrEmpty(remoteThumb)) continue;

                        string localThumb = item.Image;
                        if (string.IsNullOrEmpty(localThumb)) continue;

                        if (Helper.DownloadFile(remoteThumb, localThumb))
                        {
                            // notify that thumbnail image has been downloaded
                            item.ThumbnailImage = localThumb;
                            item.NotifyPropertyChanged("ThumbnailImage");
                        }
                    }
                })
                {
                    IsBackground = true,
                    Name = "Actor Image Downloader" + i.ToString()
                }.Start(groupList);
            }
        }

        private string GetLayoutTranslation(Layout layout)
        {
            string strLine = string.Empty;
            switch (layout)
            {
                case Layout.List:
                    strLine = GUILocalizeStrings.Get(101);
                    break;
                case Layout.SmallIcons:
                    strLine = GUILocalizeStrings.Get(100);
                    break;
                case Layout.LargeIcons:
                    strLine = GUILocalizeStrings.Get(417);
                    break;
                case Layout.Filmstrip:
                    strLine = GUILocalizeStrings.Get(733);
                    break;
            }
            return strLine;
        }

        private void ShowLayoutsMenu()
        {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dlg.Reset();
            dlg.SetHeading(GetLayoutTranslation(CurrentLayout));

            foreach(Layout layout in Enum.GetValues(typeof(Layout)))
            {
                string menuItem = GetLayoutTranslation(layout);
                GUIListItem pItem = new GUIListItem(menuItem);
                if (layout == CurrentLayout) pItem.Selected = true;
                dlg.Add(pItem);
            }
      
            dlg.DoModal(GUIWindowManager.ActiveWindow);

            if (dlg.SelectedLabel >= 0)
            {
                CurrentLayout = (Layout)dlg.SelectedLabel;
                FacadeActors.CurrentLayout = (GUIFacadeControl.Layout)CurrentLayout;
                GUIControl.SetControlLabel(GetID, ButtonLayouts.GetID, GetLayoutTranslation(CurrentLayout));
            }
        }

        #endregion

    }

    public class GUIActorListItem : GUIListItem
    {
        #region Facade Item
        public GUIActorListItem(string strLabel) : base(strLabel) { }

        public object Item
        {
            get { return _Item; }
            set
            {
                _Item = value;
                INotifyPropertyChanged notifier = value as INotifyPropertyChanged;
                if (notifier != null)
                {
                    notifier.PropertyChanged += (s, e) =>
                    {
                        if (s is DBActor && e.PropertyName == "ThumbnailImage")
                            SetImageToGui((s as DBActor).ThumbnailImage);
                    };
                }
            }
        } protected object _Item;

        protected void SetImageToGui(string imageFilePath)
        {
            if (string.IsNullOrEmpty(imageFilePath)) return;

            string texture = GetTextureFromFile(imageFilePath);

            if (GUITextureManager.LoadFromMemory(ImageFast.FromFile(imageFilePath), texture, 0, 0, 0) > 0)
            {
                ThumbnailImage = texture;
                IconImage = texture;
                IconImageBig = texture;
            }

            // if selected and GUIActors is current window force an update of thumbnail
            GUIActors actorWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow) as GUIActors;
            if (actorWindow != null)
            {
                GUIListItem selectedItem = GUIControl.GetSelectedListItem(9816, 50);
                if (selectedItem == this)
                {
                    GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, 50, ItemId, 0, null));
                }
            }
        }

        private string GetTextureFromFile(string filename)
        {
            return "[TVSeries:" + filename.GetHashCode() + "]";
        }

        #endregion
    }
}
