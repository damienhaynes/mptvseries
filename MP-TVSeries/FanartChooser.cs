#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.GUI.Library;
using Action = MediaPortal.GUI.Library.Action;
using System.ComponentModel;
using System.Drawing;

namespace WindowPlugins.GUITVSeries
{
    class FanartChooser : GUIWindow
    {
        [SkinControlAttribute(50)]
        protected GUIFacadeControl m_Facade = null;

        [SkinControlAttribute(2)]
        protected GUIButtonControl buttonLayouts = null;

        [SkinControlAttribute(11)]
        protected GUILabelControl labelResolution = null;

        [SkinControlAttribute(12)]
        protected GUIButtonControl buttonFilters = null;

        [SkinControlAttribute(13)]
        protected GUICheckButton togglebuttonRandom = null;

        [SkinControlAttribute(14)]
        protected GUILabelControl labelDisabled = null;

        [SkinControlAttribute(15)]
        protected GUILabelControl labelChosen = null;

        enum menuAction
        {
            use,
            download,
            delete,
            optionRandom,
            disable,
            enable,
            filters,
            interval,
            clearcache
        }

        enum menuFilterAction
        {
            all,
            hd,
            fullhd
        }

        enum menuIntervalAction {
            FiveSeconds,
            TenSeconds,
            FifteenSeconds,
            ThirtySeconds,
            FortyFiveSeconds,
            SixtySeconds
        }

        public enum View
        {
            List = 0,
            Icons = 1,
            LargeIcons = 2,
            FilmStrip = 3,
            AlbumView = 4,
            PlayList = 5
        }

        const int windowID = 9812;
        int seriesID = -1;
        BackgroundWorker loadingWorker = null; // to fetch list and thumbnails
        public static BackgroundWorker downloadingWorker = new BackgroundWorker(); // to do the actual downloading
        static Queue<DBFanart> toDownload = new Queue<DBFanart>();
        private object locker = new object();
        int m_PreviousSelectedItem = -1;
        private View currentView = View.LargeIcons;
        bool m_bQuickSelect = false;

        # region DownloadWorker
        static FanartChooser()
        {         
            // lets set up the downloader            
            downloadingWorker.WorkerSupportsCancellation = true;
            downloadingWorker.WorkerReportsProgress = true;
            downloadingWorker.DoWork += new DoWorkEventHandler(downloadingWorker_DoWork);
            downloadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(downloadingWorker_RunWorkerCompleted);
            
            setDownloadStatus();
        }

        void downloadingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (loadingWorker != null && !loadingWorker.IsBusy)
            {                
                m_PreviousSelectedItem = m_Facade.SelectedListItemIndex;

                if (m_Facade != null) m_Facade.Clear();
                loadingWorker.RunWorkerAsync(SeriesID);
            }
        }

        static void downloadingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            setDownloadStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void downloadingWorker_DoWork(object sender, DoWorkEventArgs e)
        {            
            do
            {
                DBFanart fanart;
                setDownloadStatus();
                lock (toDownload)
                { 
                    fanart = toDownload.Dequeue();                     
                }

                bool bDownloadSuccess = true;
                // ZF: async download of the fanart. Cancelling now works
                if (fanart != null && !fanart.isAvailableLocally)
                {
                    string filename = fanart[DBFanart.cBannerPath];
                    string localFilename = string.Empty;
                    filename = filename.Replace("/", @"\");

                    // we depend on fanart names containing the series ID
                    // if it does not exist, prefix the existing one with it
                    if (!filename.Contains(fanart[DBFanart.cSeriesID]))
                    {
                        try
                        {
                            // banner path looks like fanart\original\5b64ef95b86b2.jpg
                            string[] filePaths = filename.Split('\\');
                            localFilename = filename.Replace(filePaths[2], $"{fanart[DBFanart.cSeriesID]}-{filePaths[2]}");
                        }
                        catch
                        {
                            MPTVSeriesLog.Write("Error normalising fanart path");
                        }
                    }
                    else
                        localFilename = filename;

                    string fullURL = (DBOnlineMirror.Banners.EndsWith("/") ? DBOnlineMirror.Banners : (DBOnlineMirror.Banners + "/")) + filename;
                    int nDownloadGUID = Online_Parsing_Classes.OnlineAPI.StartFileDownload(fullURL, Settings.Path.fanart, localFilename);
                    while (Online_Parsing_Classes.OnlineAPI.CheckFileDownload(nDownloadGUID))
                    {
                        if (downloadingWorker.CancellationPending) 
                        {
                            // Cancel, clean up pending download
                            bDownloadSuccess = false;
                            Online_Parsing_Classes.OnlineAPI.CancelFileDownload(nDownloadGUID);
                            MPTVSeriesLog.Write("Cancel Fanart download: " + fanart.FullLocalPath);
                        }
                        System.Windows.Forms.Application.DoEvents();
                    }
                    // Download is either completed or canceled
                    if (bDownloadSuccess) 
                    {
                        fanart[DBFanart.cLocalPath] = localFilename.Replace(Settings.GetPath(Settings.Path.fanart), string.Empty);
                        fanart.Commit();
                        MPTVSeriesLog.Write("Successfully downloaded Fanart: " + fanart.FullLocalPath);
                        downloadingWorker.ReportProgress(0, fanart[DBFanart.cIndex]);                      
                    }
                    else 
                        MPTVSeriesLog.Write("Error downloading Fanart: " + fanart.FullLocalPath);
                }
            } 
            while (toDownload.Count > 0 && !downloadingWorker.CancellationPending);
        }

        static void setDownloadStatus()
        {
            lock (toDownload)
            {
                if (toDownload.Count > 0)
                {
                    TVSeriesPlugin.setGUIProperty("FanArt.DownloadingStatus", string.Format(Translation.FanDownloadingStatus, toDownload.Count));                    
                }
                else                
                    TVSeriesPlugin.setGUIProperty("FanArt.DownloadingStatus", " ");
            }
        }

        #endregion

        public static int GetWindowID
        { get { return windowID; } }
        
        public override int GetID
        { get { return windowID; } }

        public int GetWindowId()
        { return windowID; }

        public override bool Init()
        {
            String xmlSkin = GUIGraphicsContext.Skin + @"\TVSeries.FanArt.xml";            
            return Load(xmlSkin);
        }

		/// <summary>
		/// MediaPortal will set #currentmodule with GetModuleName()
		/// </summary>
		/// <returns>Localized Window Name</returns>
		//public override string GetModuleName() {
		//	return Translation.FanArt;
		//}

        protected View CurrentView
        {
            get { return currentView; }
            set { currentView = value; }
        }

        protected override void OnPageLoad()
        {            
            AllocResources();

            MediaPortal.GUI.Library.GUIPropertyManager.SetProperty("#currentmodule", Translation.FanArt);

            loadingWorker = new BackgroundWorker();            
            loadingWorker.WorkerReportsProgress = true;
            loadingWorker.WorkerSupportsCancellation = true;
            loadingWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
            loadingWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            loadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            if (m_Facade != null)
            {
                int defaultView = 2;
                if (int.TryParse(DBOption.GetOptions(DBOption.cFanartCurrentView), out defaultView))
                {
                    CurrentView = (View)defaultView;                    
                }                
                m_Facade.CurrentLayout = (GUIFacadeControl.Layout)CurrentView;                
            }            

            base.OnPageLoad();
            
            Helper.disableNativeAutoplay();

            // update skin controls
            UpdateLayoutButton();
            if (labelResolution != null) labelResolution.Label = Translation.LabelResolution;
            if (labelChosen != null) labelChosen.Label = Translation.LabelChosen;
            if (labelDisabled != null) labelDisabled.Label = Translation.LabelDisabled;            
            if (buttonFilters != null) buttonFilters.Label = Translation.FanArtFilter;
            if (togglebuttonRandom != null)
            {
                togglebuttonRandom.Label = Translation.ButtonRandomFanart;
                togglebuttonRandom.Selected = DBOption.GetOptions(DBOption.cFanartRandom);
            }

            ClearProperties();
            UpdateFilterProperty(false);

            setDownloadStatus();
			
            MPTVSeriesLog.Write("Fanart Chooser Window initializing");            
               
            fetchList(SeriesID);
            loadingWorker.RunWorkerAsync(SeriesID);            

            downloadingWorker.ProgressChanged += new ProgressChangedEventHandler(downloadingWorker_ProgressChanged);            
            
        }

        protected bool AllowView(View view)
        {
            if (view == View.List)
                return false;

            if (view == View.AlbumView)
                return false;

            if (view == View.PlayList)
                return false;
            
            return true;
        }

        private void UpdateLayoutButton()
        {
            string strLine = string.Empty;
            View view = CurrentView;
            switch (view)
            {
                case View.List:
                    strLine = GUILocalizeStrings.Get(101);
                    break;
                case View.Icons:
                    strLine = GUILocalizeStrings.Get(100);
                    break;
                case View.LargeIcons:
                    strLine = GUILocalizeStrings.Get(417);
                    break;
                case View.FilmStrip:
                    strLine = GUILocalizeStrings.Get(733);
                    break;
                case View.PlayList:
                    strLine = GUILocalizeStrings.Get(101);
                    break;
            }
            if (buttonLayouts != null)
                GUIControl.SetControlLabel(GetID, buttonLayouts.GetID, strLine);
        }

        private void ClearProperties()
        {
            TVSeriesPlugin.setGUIProperty("FanArt.Count", " ");
            TVSeriesPlugin.setGUIProperty("FanArt.LoadingStatus", " ");
            TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartInfo", " ");
            TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartResolution", " ");
            TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartIsChosen", " ");
            TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartIsDisabled", " ");
            TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartColors", " ");            
        }

        private void UpdateFilterProperty(bool btnEnabled)
        {
            if (buttonFilters != null)
                buttonFilters.IsEnabled = btnEnabled;

            string resolution = string.Empty;
            if (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) == "0")
            {
                resolution = Translation.FanArtFilterAll;
            }
            else if (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) == "1")
            {
                resolution = "1280x720";
            }
            else
                resolution = "1920x1080";

            TVSeriesPlugin.setGUIProperty("FanArt.FilterResolution", resolution);            
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            TVSeriesPlugin.setGUIProperty("FanArt.LoadingStatus", string.Empty);
            TVSeriesPlugin.setGUIProperty("FanArt.Count", totalFanart.ToString());

            if (totalFanart == 0)
            {
                TVSeriesPlugin.setGUIProperty("FanArt.LoadingStatus", Translation.FanArtNoneFound);
                // Enable Filters button in case fanart is filtered
                if (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) != "0" && buttonFilters != null)
                {
                    OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
                    OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));                    
                }
            }
            totalFanart = 0;

            // Load the selected facade so it's not black by default
            if (m_Facade != null && m_Facade.SelectedListItem != null && m_Facade.SelectedListItem.TVTag != null)
            {
                if (m_Facade.Count > m_PreviousSelectedItem)
                {
                    if (m_PreviousSelectedItem <= 0)
                        m_Facade.SelectedListItemIndex = 0;
                    else
                        m_Facade.SelectedListItemIndex = m_PreviousSelectedItem;                    

                    // Work around for Filmstrip not allowing to programmatically select item
                    if (m_Facade.CurrentLayout == GUIFacadeControl.Layout.Filmstrip)
                    {
                        m_bQuickSelect = true;
                        for (int i = 0; i < m_PreviousSelectedItem; i++)
                        {
                            OnAction(new Action(Action.ActionType.ACTION_MOVE_RIGHT, 0, 0));
                        }
                        m_bQuickSelect = false;
                        // Note: this is better way, but Scroll offset wont work after set
                        //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, m_Facade.WindowId, 0, m_Facade.FilmstripLayout.GetID, m_PreviousSelectedItem, 0, null);
                        //GUIGraphicsContext.SendMessage(msg);
                        //MPTVSeriesLog.Write("Sending a selection postcard to FilmStrip.", MPTVSeriesLog.LogLevel.Debug);
                    }                   
                    m_PreviousSelectedItem = -1;
                }


                DBFanart selectedFanart = m_Facade.SelectedListItem.TVTag as DBFanart;
                if (selectedFanart != null)
                {
                    setFanartPreviewBackground(selectedFanart);
                }
            }
            UpdateFilterProperty(true);            
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            DBOption.SetOptions(DBOption.cFanartCurrentView, (int)CurrentView);

            if (loadingWorker.IsBusy)
                loadingWorker.CancelAsync();
            while (loadingWorker.IsBusy)
              System.Windows.Forms.Application.DoEvents();

            loadingWorker = null;
            
            Helper.enableNativeAutoplay();
            
            base.OnPageDestroy(new_windowId);
        }

        public void setPageTitle(string Title)
        {            
            TVSeriesPlugin.setGUIProperty("FanArt.PageTitle", Title);
        }

        #region Context Menu
        protected override void OnShowContextMenu()
        {
            try
            {
                GUIListItem currentitem = this.m_Facade.SelectedListItem;
                if (currentitem == null || !(currentitem.TVTag is DBFanart)) return;
                DBFanart selectedFanart = currentitem.TVTag as DBFanart;

                IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(Translation.FanArt);

                GUIListItem pItem;
                if (DBOption.GetOptions(DBOption.cFanartRandom))
                {
                    // if random it doesnt make sense to offer an option to explicitally use an available fanart
                    if (!selectedFanart.isAvailableLocally)
                    {
                        pItem = new GUIListItem(Translation.FanArtGetAndUse);
                        dlg.Add(pItem);
                        pItem.ItemId = (int)menuAction.download;
                    }        
                }
                else
                {
                    // if we are not random, we can choose available fanart
                    if (selectedFanart.isAvailableLocally && !selectedFanart.Disabled)
                    {
                        pItem = new GUIListItem(Translation.FanArtUse);
                        dlg.Add(pItem);
                        pItem.ItemId = (int)menuAction.use;
                    }
                    else if (!selectedFanart.isAvailableLocally)
                    {
                        pItem = new GUIListItem(Translation.FanArtGet);
                        dlg.Add(pItem);
                        pItem.ItemId = (int)menuAction.download;
                    }
                }

                if (selectedFanart.isAvailableLocally)
                {
                    if (selectedFanart.Disabled)
                    {
                        pItem = new GUIListItem(Translation.FanartMenuEnable);
                        dlg.Add(pItem);
                        pItem.ItemId = (int)menuAction.enable;
                    }
                    else
                    {
                        pItem = new GUIListItem(Translation.FanartMenuDisable);
                        dlg.Add(pItem);
                        pItem.ItemId = (int)menuAction.disable;
                    }
                }

                pItem = new GUIListItem(Translation.FanArtRandom + " (" + (DBOption.GetOptions(DBOption.cFanartRandom) ? Translation.on : Translation.off) + ")");
                dlg.Add(pItem);
                pItem.ItemId = (int)menuAction.optionRandom;

                // Dont allowing filtering until DB has all data
                if (!loadingWorker.IsBusy)
                {
                    pItem = new GUIListItem(Translation.FanArtFilter + " ...");
                    dlg.Add(pItem);
                    pItem.ItemId = (int)menuAction.filters;
                }

                pItem = new GUIListItem(Translation.FanartRandomInterval + " ...");
                dlg.Add(pItem);
                pItem.ItemId = (int)menuAction.interval;

                if (!loadingWorker.IsBusy) {
                    pItem = new GUIListItem(Translation.ClearFanartCache);
                    dlg.Add(pItem);
                    pItem.ItemId = (int)menuAction.clearcache;
                }

                // lets show it
                dlg.DoModal(GUIWindowManager.ActiveWindow);
                switch (dlg.SelectedId) // what was chosen?
                {
                    case (int)menuAction.delete:
                        if (selectedFanart.isAvailableLocally)
                        {                            
                            selectedFanart.Delete();  
                            // and reinit the display to get rid of it
                            m_Facade.Clear();
                            loadingWorker.RunWorkerAsync(SeriesID);
                        }
                        break;
                    case (int)menuAction.download:
                        if (!selectedFanart.isAvailableLocally)
                            downloadFanart(selectedFanart);                        
                        break;
                    case (int)menuAction.use:
                        if (selectedFanart.isAvailableLocally)
                        {                           
                            TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartIsChosen", Translation.Yes);
                            SetFacadeItemAsChosen(m_Facade.SelectedListItemIndex);

                            selectedFanart.Chosen = true;
                            Fanart.RefreshFanart(SeriesID);
                        }                        
                        break;
                    case (int)menuAction.optionRandom:
                        DBOption.SetOptions(DBOption.cFanartRandom, !DBOption.GetOptions(DBOption.cFanartRandom));
                        if (togglebuttonRandom != null)
                            togglebuttonRandom.Selected = DBOption.GetOptions(DBOption.cFanartRandom);
                        break;
                    case (int)menuAction.disable:
                        selectedFanart.Disabled = true;
                        selectedFanart.Chosen = false;
                        currentitem.Label = Translation.FanartDisableLabel;
                        TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartIsDisabled", Translation.Yes);
                        TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartIsChosen", Translation.No);
                        break;
                    case (int)menuAction.enable:
                        selectedFanart.Disabled = false;                        
                        currentitem.Label = Translation.FanArtLocal;
                        TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartIsDisabled", Translation.No);
                        break;
                    case (int)menuAction.filters:
                        dlg.Reset();
                        ShowFiltersMenu();
                        break;
                    case (int)menuAction.interval:
                        dlg.Reset();
                        ShowIntervalMenu();
                        break;
                    case (int)menuAction.clearcache:
                        dlg.Reset();
                        Fanart.ClearFanartCache(SeriesID);                                               
                        m_Facade.Clear();                                  
                        fetchList(SeriesID);
                        loadingWorker.RunWorkerAsync(SeriesID);
                        break;

                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Exception in Fanart Chooser Context Menu: " + ex.Message);
                return;
            }
        }
        #endregion

        #region Context Menu - Random Fanart Interval
        private void ShowIntervalMenu() {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;

            dlg.Reset();
            dlg.SetHeading(Translation.FanartRandomInterval);

            GUIListItem pItem = new GUIListItem(Translation.FanartIntervalFiveSeconds);            
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.FiveSeconds;

            pItem = new GUIListItem(Translation.FanartIntervalTenSeconds);
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.TenSeconds;

            pItem = new GUIListItem(Translation.FanartIntervalFifteenSeconds);
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.FifteenSeconds;

            pItem = new GUIListItem(Translation.FanartIntervalThirtySeconds);
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.ThirtySeconds;

            pItem = new GUIListItem(Translation.FanartIntervalFortyFiveSeconds);
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.FortyFiveSeconds;

            pItem = new GUIListItem(Translation.FanartIntervalSixtySeconds);
            dlg.Add(pItem);
            pItem.ItemId = (int)menuIntervalAction.SixtySeconds;

            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId >= 0) {
                switch (dlg.SelectedId) {
                    case (int)menuIntervalAction.FiveSeconds:
                        DBOption.SetOptions(DBOption.cRandomFanartInterval, "5000");
                        break;
                    case (int)menuIntervalAction.TenSeconds:
                        DBOption.SetOptions(DBOption.cRandomFanartInterval, "10000");
                        break;
                    case (int)menuIntervalAction.FifteenSeconds:
                        DBOption.SetOptions(DBOption.cRandomFanartInterval, "15000");
                        break;
                    case (int)menuIntervalAction.ThirtySeconds:
                        DBOption.SetOptions(DBOption.cRandomFanartInterval, "30000");
                        break;
                    case (int)menuIntervalAction.FortyFiveSeconds:
                        DBOption.SetOptions(DBOption.cRandomFanartInterval, "45000");
                        break;
                    case (int)menuIntervalAction.SixtySeconds:
                        DBOption.SetOptions(DBOption.cRandomFanartInterval, "60000");
                        break;                    
                }               
            }
        }
        #endregion

        #region Context Menu - Filters
        private void ShowFiltersMenu()
        {
            IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;

            dlg.Reset();
            dlg.SetHeading(Translation.FanArtFilter);

            GUIListItem pItem = new GUIListItem(Translation.FanArtFilterAll);
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.all;            

            pItem = new GUIListItem("1280x720");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.hd;            

            pItem = new GUIListItem("1920x1080");
            dlg.Add(pItem);
            pItem.ItemId = (int)menuFilterAction.fullhd;            
            
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedId >= 0)
            {
                switch (dlg.SelectedId)
                {
                    case (int)menuFilterAction.all:
                        DBOption.SetOptions(DBOption.cFanartThumbnailResolutionFilter, "0");
                        break;
                    case (int)menuFilterAction.hd:
                        DBOption.SetOptions(DBOption.cFanartThumbnailResolutionFilter, "1");
                        break;
                    case (int)menuFilterAction.fullhd:
                        DBOption.SetOptions(DBOption.cFanartThumbnailResolutionFilter, "2");
                        break;                  
                }
                m_Facade.Clear();
                DBFanart.ClearAll();
                ClearProperties();

                UpdateFilterProperty(false);
                loadingWorker.RunWorkerAsync(SeriesID);                   
            }
        }
        #endregion

        int totalFanart;
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (m_Facade != null)
                {
                    GUIListItem loadedItem = e.UserState as GUIListItem;
                    if (loadedItem != null)
                    {
                        m_Facade.Add(loadedItem);
                        if (m_Facade != null)
                        {
                            this.m_Facade.Focus = true;

                            if (loadedItem.Selected)
                            {
                                onFacadeItemSelected(loadedItem, m_Facade);
                            }
                        }
                    }
                }

                // report progress to GUI
                TVSeriesPlugin.setGUIProperty("FanArt.LoadingPercentage", e.ProgressPercentage.ToString());
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Error: Fanart Chooser worker_ProgressChanged() experienced an error: " + ex.Message);
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadThumbnails((int)e.Argument);
        }

        public int SeriesID
        { 
            get { return seriesID; }
            set { seriesID = value; }
        }

        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                // Can't use OnMessage when using Filmstrip - it doesn't work!!
                case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED: {
                    int iControl = message.SenderControlId;
                    if (iControl == (int)m_Facade.GetID && m_Facade.SelectedListItem != null) {
                        DBFanart selectedFanart = m_Facade.SelectedListItem.TVTag as DBFanart;
                        if (selectedFanart != null) {
                            setFanartPreviewBackground(selectedFanart);
                        }
                    }
                    return true;
                } 
                default:
                    return base.OnMessage(message);
            }
        }

        // triggered when a selection change was made on the facade
        private void onFacadeItemSelected(GUIListItem item, GUIControl parent)
        {
            if (m_bQuickSelect) return;

            // if this is not a message from the facade, exit
            if (parent != m_Facade && parent != m_Facade.FilmstripLayout &&
                parent != m_Facade.ThumbnailLayout && parent != m_Facade.ListLayout)
                return;
           
            DBFanart selectedFanart = item.TVTag as DBFanart;
            if (selectedFanart != null)
            {
                setFanartPreviewBackground(selectedFanart);
            }      
            
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            if (control == buttonFilters)
            {
                ShowFiltersMenu();
                buttonFilters.Focus = false;
                return;
            }
            
            if (control == togglebuttonRandom)
            {
                DBOption.SetOptions(DBOption.cFanartRandom, togglebuttonRandom.Selected);
                togglebuttonRandom.Focus = false;
                return;
            }

            if (control == buttonLayouts)
            {
                bool shouldContinue = false;
                do
                {
                    shouldContinue = false;
                    switch (CurrentView)
                    {
                        case View.List:
                            CurrentView = View.PlayList;
                            if (!AllowView(CurrentView) || m_Facade.PlayListLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.Playlist;
                            }
                            break;

                        case View.PlayList:
                            CurrentView = View.Icons;
                            if (!AllowView(CurrentView) || m_Facade.ThumbnailLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
                            }
                            break;

                        case View.Icons:
                            CurrentView = View.LargeIcons;
                            if (!AllowView(CurrentView) || m_Facade.ThumbnailLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
                            }
                            break;

                        case View.LargeIcons:
                            CurrentView = View.FilmStrip;
                            if (!AllowView(CurrentView) || m_Facade.FilmstripLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.Filmstrip;
                            }
                            break;

                        case View.FilmStrip:
                            CurrentView = View.List;
                            if (!AllowView(CurrentView) || m_Facade.ListLayout == null)
                            {
                                shouldContinue = true;
                            }
                            else
                            {
                                m_Facade.CurrentLayout = GUIFacadeControl.Layout.List;
                            }
                            break;
                    }
                } while (shouldContinue);
                UpdateLayoutButton();
                GUIControl.FocusControl(GetID, controlId);
            }

            if (actionType != Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
            if (control == this.m_Facade)
            {
                DBFanart chosen;
                if ((chosen = this.m_Facade.SelectedListItem.TVTag as DBFanart) != null)
                {
                    if (chosen.isAvailableLocally && !chosen.Disabled)
                    {                        
                        TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartIsChosen", Translation.Yes);                        
                        SetFacadeItemAsChosen(m_Facade.SelectedListItemIndex);
                        
                        // if we already have it, we simply set the chosen property (will itself "unchoose" all the others)
                        chosen.Chosen = true;
                        // ZF: be sure to update the list of downloaded data in the cache - otherwise the selected fanart won't show up for new fanarts until restarted
                        Fanart.RefreshFanart(SeriesID);                        

                    }
                    else if (!chosen.isAvailableLocally)
                    {
                        downloadFanart(chosen);
                    }
                }
            }
        }

        void SetFacadeItemAsChosen(int iSelectedItem)
        {
            try
            {
                for (int i = 0; i < m_Facade.Count; i++)
                {
                    if (i == iSelectedItem)
                        m_Facade[i].IsRemote = true;
                    else
                    {
                        m_Facade[i].IsRemote = false;
                        DBFanart item;
                        item = m_Facade[i].TVTag as DBFanart;
                        item.Chosen = false;
                    }
                }
            }
            catch ( Exception ex )
            {
                MPTVSeriesLog.Write("Failed to set Facade Item as chosen: " + ex.Message);
            }
        }

        void downloadFanart(DBFanart fanart)
        {
            // we need to get it, let's queue them up and download in the background
            lock (toDownload)
            {
                toDownload.Enqueue(fanart);
            }
            setDownloadStatus();
            // don't return, user can queue up multiple fanart to download
            // the last he selects to download will be the chosen one by default

            // finally lets check if the downloader is already running, and if not start it
            if (!downloadingWorker.IsBusy)
                downloadingWorker.RunWorkerAsync();
        }

        void fetchList(int seriesID)
        {
            // Fetch a fresh list online and save info about them to the db 
            GetFanart gf = new GetFanart(seriesID);
            foreach (DBFanart f in gf.Fanart) {
                f.Commit();
            }
        }

        void loadThumbnails(int seriesID)
        {
            if (seriesID > 0)
            {                
                if (loadingWorker.CancellationPending)
                    return;

                GUIListItem item = null;
                List<DBFanart> onlineFanart = DBFanart.GetAll(seriesID, false);

                // sort fanart by highest rated
                onlineFanart.Sort();

                // Filter Fanart Thumbnails to be displayed by resolution
                if (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) != 0)
                {
                    string filteredRes = (DBOption.GetOptions(DBOption.cFanartThumbnailResolutionFilter) == "1" ? "1280x720" : "1920x1080");
                    for (int j = onlineFanart.Count - 1; j >= 0; j--)
                    {
                        if (onlineFanart[j][DBFanart.cResolution] != filteredRes)
                            onlineFanart.Remove(onlineFanart[j]);
                    }
                }

                // Inform skin message how many fanarts are online
                totalFanart = onlineFanart.Count;
                TVSeriesPlugin.setGUIProperty("FanArt.Count", totalFanart.ToString());
                loadingWorker.ReportProgress(0);
                
                // let's get all the ones we have available locally (from online)
                int i = 0;
                foreach (DBFanart fanart in onlineFanart)
                {                    
                    if(fanart.isAvailableLocally)
                    {
                        if (fanart.Disabled)
                            item = new GUIListItem(Translation.FanartDisableLabel);
                        else
                            item = new GUIListItem(Translation.FanArtLocal);
                        item.IsRemote = false;
                        
                        if (fanart.Chosen) 
                            item.IsRemote = true;
                        else 
                            item.IsDownloading = false;
                    }
                    else 
                    {
                        item = new GUIListItem(Translation.FanArtOnline);
                        item.IsRemote = false;
                        item.IsDownloading = true;
                    }                    
                    string filename = fanart[DBFanart.cThumbnailPath];
                    string localFilename = string.Empty;
                    filename = filename.Replace("/", @"\");

                    // we depend on fanart names containing the series ID
                    // if it does not exist, prefix the existing one with it
                    if (!filename.Contains(fanart[DBFanart.cSeriesID]))
                    {
                        try
                        {
                            // banner path looks like _cache\fanart\original\5b64ef95b86b2.jpg
                            string[] filePaths = filename.Split('\\');
                            localFilename = filename.Replace(filePaths[3], $"{fanart[DBFanart.cSeriesID]}-{filePaths[3]}");

                            // update path
                            fanart[DBFanart.cThumbnailPath] = localFilename;
                            fanart.Commit();
                        }
                        catch
                        {
                            MPTVSeriesLog.Write("Error normalising fanart thumbnail path");
                        }
                    }
                    else
                        localFilename = filename;


                    string fullURL = (DBOnlineMirror.Banners.EndsWith("/") ? DBOnlineMirror.Banners : (DBOnlineMirror.Banners + "/")) + filename;

                    bool bDownloadSuccess = true;
                    int nDownloadGUID = Online_Parsing_Classes.OnlineAPI.StartFileDownload(fullURL, Settings.Path.fanart, localFilename);
                    while (Online_Parsing_Classes.OnlineAPI.CheckFileDownload(nDownloadGUID))
                    {
                        if (loadingWorker.CancellationPending)
                        {
                            // ZF: Cancel, clean up pending download
                            bDownloadSuccess = false;
                            Online_Parsing_Classes.OnlineAPI.CancelFileDownload(nDownloadGUID);
                            MPTVSeriesLog.Write("Cancelling fanart thumbnail download: " + localFilename);
                        }
                        System.Windows.Forms.Application.DoEvents();
                    }

                    // ZF: should be downloaded now
                    localFilename = Helper.PathCombine(Settings.GetPath(Settings.Path.fanart), localFilename);
                    if (bDownloadSuccess)
                    {
                        item.IconImage = item.IconImageBig = ImageAllocator.GetOtherImage(localFilename, new Size(0, 0), false);
                    }
                    item.TVTag = fanart;
                    
                    if (i == 0)
                    {
                        item.Selected = true;
                    }

                    // Subscribe to Item Selected Event
                    item.OnItemSelected += new GUIListItem.ItemSelectedHandler(onFacadeItemSelected);

                    int progress = (int)((double)++i / totalFanart * 100);
                    loadingWorker.ReportProgress(progress, item);

                    TVSeriesPlugin.setGUIProperty("FanArt.LoadingStatus", string.Format(Translation.FanArtOnlineLoading, i, totalFanart));

                    if (loadingWorker.CancellationPending)
                        return;
                }
            }
        }

        void setFanartPreviewBackground(DBFanart fanart)
        {
            string fanartInfo = fanart.isAvailableLocally ? Translation.FanArtLocal : Translation.FanArtOnline;
            fanartInfo += Environment.NewLine;

            foreach (KeyValuePair<string, DBField> kv in fanart.m_fields)
            {
                switch (kv.Key)
                {
                    case DBFanart.cResolution:                 
                        TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartResolution", kv.Value.Value);                        
                        break;

                    case DBFanart.cColors:
                        TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartColors", kv.Value.Value);
                        break;

                    case DBFanart.cChosen:
                        TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartIsChosen", kv.Value.Value?Translation.Yes:Translation.No);
                        break;

                    case DBFanart.cDisabled:
                        TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartIsDisabled", kv.Value.Value ? Translation.Yes : Translation.No);
                        break;
                    
                }
                fanartInfo += kv.Key + ": " + kv.Value.Value + Environment.NewLine;
            }

            TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartInfo", fanartInfo);

            string preview = string.Empty;
            
            lock (locker)
            {
                if (fanart.isAvailableLocally)
                {
                    // Ensure Fanart on Disk is valid as well
                    if (ImageAllocator.LoadImageFastFromFile(fanart.FullLocalPath) == null)
                    {
                        MPTVSeriesLog.Write("Fanart is invalid, deleting...");
                        fanart.Delete();
                        fanart.Chosen = false;
                        m_Facade.SelectedListItem.Label = Translation.FanArtOnline;
                    }                    
                
                    // Should be safe to assign fullsize fanart if available
                    preview = fanart.isAvailableLocally ?
                              ImageAllocator.GetOtherImage(fanart.FullLocalPath, default(System.Drawing.Size), false) :
                              m_Facade.SelectedListItem.IconImageBig;
                }
                else
                    preview = m_Facade.SelectedListItem.IconImageBig;
                      
                TVSeriesPlugin.setGUIProperty("FanArt.SelectedPreview", preview);
            }
        }

    }
}
