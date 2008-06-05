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
using System.ComponentModel;
using System.Drawing;

namespace WindowPlugins.GUITVSeries
{
    class FanartChooser : GUIWindow
    {
        [SkinControlAttribute(50)]
        protected GUIFacadeControl m_Facade = null;

        enum menuAction
        {
            use,
            download,
            delete,
            optionRandom
        }

        const int windowID = 9812;
        int seriesID = -1;
        BackgroundWorker loadingWorker = null; // to fetch list and thumbnails
        public static BackgroundWorker downloadingWorker = new BackgroundWorker(); // to do the actual downloading
        static Queue<DBFanart> toDownload = new Queue<DBFanart>();

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
            if (m_Facade != null) m_Facade.Clear();
            loadingWorker.RunWorkerAsync(SeriesID);
        }

        static void downloadingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            setDownloadStatus();
        }

        static void downloadingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                DBFanart f;
                setDownloadStatus();
                lock (toDownload)
                { 
                    f = toDownload.Dequeue();                     
                }
                if (GetFanart.DownloadFanart(f))
                {
                    // was sucessfull, lets set it as the chosen one
                    f.Chosen = true;
                    MPTVSeriesLog.Write("Sucessfully downloaded Fanart: " + f.FullLocalPath);
                    downloadingWorker.ReportProgress(0, f[DBFanart.cIndex]);
                }
                else MPTVSeriesLog.Write("Error downloaded Fanart: " + f.FullLocalPath);

            } while (toDownload.Count > 0 && !downloadingWorker.CancellationPending);
        }

        static void setDownloadStatus()
        {
            lock (toDownload)
            {                 
               TVSeriesPlugin.setGUIProperty("FanArt.DownloadingStatus", string.Format(Translation.FanDownloadingStatus, toDownload.Count));
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

        protected override void OnPageLoad()
        {
            loadingWorker = new BackgroundWorker();            
            loadingWorker.WorkerReportsProgress = true;
            loadingWorker.WorkerSupportsCancellation = true;
            loadingWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
            loadingWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            loadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            if (m_Facade != null)
            {
                m_Facade.View = GUIFacadeControl.ViewMode.LargeIcons;
            }

            base.OnPageLoad();            

            TVSeriesPlugin.setGUIProperty("FanArt.LoadingStatus", string.Empty);
            TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartInfo", string.Empty);
            setDownloadStatus();

            MPTVSeriesLog.Write("Fanartchooser Window initializing");

            fetchList(SeriesID);
            loadingWorker.RunWorkerAsync(SeriesID);

            downloadingWorker.ProgressChanged += new ProgressChangedEventHandler(downloadingWorker_ProgressChanged);            
            
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            TVSeriesPlugin.setGUIProperty("FanArt.LoadingStatus", string.Empty);
            if (totalFanart == 0) TVSeriesPlugin.setGUIProperty("FanArt.LoadingStatus", Translation.FanArtNoneFound);
            totalFanart = 0;
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            if (loadingWorker.IsBusy)
                loadingWorker.CancelAsync();
            while (loadingWorker.IsBusy) ;
            loadingWorker = null;
            ImageAllocator.FlushOthers(false);
            base.OnPageDestroy(new_windowId);
        }

        public void setPageTitle(string Title)
        {
            TVSeriesPlugin.setGUIProperty("Fanart.PageTitle", Title);
        }

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
                dlg.SetHeading("Fanart");

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
                    else // likewise delete obviously only makes sense for available stuff
                    {
                        pItem = new GUIListItem(Translation.FanArtDelete);
                        dlg.Add(pItem);
                        pItem.ItemId = (int)menuAction.delete;
                    }
                }
                else
                {
                    // if we are not random, we can choose available fanart
                    if (selectedFanart.isAvailableLocally)
                    {
                        pItem = new GUIListItem(Translation.FanArtUse);
                        dlg.Add(pItem);
                        pItem.ItemId = (int)menuAction.download;

                        pItem = new GUIListItem(Translation.FanArtDelete);
                        dlg.Add(pItem);
                        pItem.ItemId = (int)menuAction.delete;
                    }
                    else
                    {
                        pItem = new GUIListItem(Translation.FanArtGetAndUse);
                        dlg.Add(pItem);
                        pItem.ItemId = (int)menuAction.download;
                    }
                }

                pItem = new GUIListItem(Translation.FanArtRandom + " (" + (DBOption.GetOptions(DBOption.cFanartRandom) ? "on" : "off") + ")");
                dlg.Add(pItem);
                pItem.ItemId = (int)menuAction.optionRandom;

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
                    case (int)menuAction.use:
                        if (selectedFanart.isAvailableLocally) selectedFanart.Chosen = true;
                        else downloadFanart(selectedFanart);                        
                        break;
                    case (int)menuAction.optionRandom:
                        DBOption.SetOptions(DBOption.cFanartRandom, !DBOption.GetOptions(DBOption.cFanartRandom));
                        break;
                }
            }
            catch (Exception ex)
            {
                MPTVSeriesLog.Write("Except in FanartChooser ContextMenu: " + ex.Message);
                return;
            }
        }

        int totalFanart;
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (m_Facade != null)
            {
                GUIListItem loadedItem = e.UserState as GUIListItem;
                if (loadedItem != null)
                {
                    m_Facade.Add(loadedItem);
                    // we use this to tell the gui how many fanart we are loading
                    TVSeriesPlugin.setGUIProperty("FanArt.LoadingStatus", string.Format(Translation.FanArtOnlineLoading, e.ProgressPercentage, totalFanart));
                }
                else if (e.ProgressPercentage > 0)
                {
                    // we use this to tell the gui how many fanart we are loading
                    TVSeriesPlugin.setGUIProperty("FanArt.LoadingStatus", string.Format(Translation.FanArtOnlineLoading, 0, e.ProgressPercentage));
                    totalFanart = e.ProgressPercentage;
                }                
            }
            if (m_Facade != null) this.m_Facade.Focus = true;
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
                case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
                    {
                        int iControl = message.SenderControlId;
                        if (iControl == (int)m_Facade.GetID && m_Facade.SelectedListItem != null)
                        {
                            DBFanart selectedFanart = m_Facade.SelectedListItem.TVTag as DBFanart;
                            if (selectedFanart != null)
                            {
                                string fanartInfo = selectedFanart.isAvailableLocally ? Translation.FanArtLocal : Translation.FanArtOnline;
                                fanartInfo += Environment.NewLine;

                                foreach (KeyValuePair<string, DBField> kv in selectedFanart.m_fields)
                                {
                                    if(kv.Key == "BannerType2") // resolution
                                        TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartResolution", kv.Value.Value);
                                    fanartInfo += kv.Key + ": " + kv.Value.Value + Environment.NewLine;
                                }

                                TVSeriesPlugin.setGUIProperty("FanArt.SelectedFanartInfo", fanartInfo);

                                string preview = selectedFanart.isAvailableLocally ?
                                    ImageAllocator.GetOtherImage(selectedFanart.FullLocalPath, default(System.Drawing.Size), false) :
                                    m_Facade.SelectedListItem.IconImageBig;
                                setFanartPreviewBackground(preview);
                            }
                        }
                        return true;
                    } 
                default:
                    return base.OnMessage(message);
            }
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            if (actionType != Action.ActionType.ACTION_SELECT_ITEM) return; // some other events raised onClicked too for some reason?
            if (control == this.m_Facade)
            {
                DBFanart chosen;
                if ((chosen = this.m_Facade.SelectedListItem.TVTag as DBFanart) != null)
                {
                    if (chosen.isAvailableLocally)
                    {
                        // if we already have it, we simply set the chosen property (will itself "unchoose" all the others)
                        chosen.Chosen = true;
                        // now it probably makes sense to just get back to tvseries itself, nothing more for the user to do here really
                        GUIWindowManager.ShowPreviousWindow();
                    }
                    else
                    {
                        downloadFanart(chosen);
                    }
                }
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
            // let's fetch a fresh list online and save info about them to the db   
 
            GetFanart gf = new GetFanart(seriesID);
            foreach (DBFanart f in gf.Fanart)
                f.Commit();
        }

        void loadThumbnails(int seriesID)
        {
            if (seriesID > 0)
            {                
                if (loadingWorker.CancellationPending)
                    return;
                GUIListItem item = null;
                List<DBFanart> onlineFanart = DBFanart.GetAll(seriesID, false);
                loadingWorker.ReportProgress(onlineFanart.Count < 100 ? onlineFanart.Count : 100);
                // let's get all the ones we have available locally (from online)
                foreach (DBFanart f in onlineFanart)
                {
                    if(f.isAvailableLocally)
                    {
                        item = new GUIListItem(Translation.FanArtLocal);
                        item.IconImage = item.IconImageBig = GetFanart.GetThumbnail(f);
                        item.TVTag = f;
                        item.IsRemote = false;
                        if (f.Chosen) item.IsRemote = true;
                        else item.IsDownloading = false;
                        loadingWorker.ReportProgress(0, item);
                    }
                    if (loadingWorker.CancellationPending)
                        return;
                }

                // let's get the ones that are online but are not available locally
                foreach (DBFanart f in onlineFanart)
                {
                    if(!f.isAvailableLocally)
                    {
                        item = new GUIListItem(Translation.FanArtOnline);
                        item.IconImage = item.IconImageBig = GetFanart.GetThumbnail(f);
                        item.TVTag = f;
                        item.IsRemote = false;
                        item.IsDownloading = true;
                        loadingWorker.ReportProgress(0, item);
                    }
                    if (loadingWorker.CancellationPending)
                        return;
                }                
            }
        }

        void setFanartPreviewBackground(string image)
        {
            TVSeriesPlugin.setGUIProperty("FanArt.SelectedPreview", image);
        }
    }
}
