using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace WindowPlugins.GUITVSeries.Configuration
{    
    public partial class ImportPanelEpID : UserControl, Feedback.IEpisodeMatchingFeedback
    {
        public event UserFinishedEditingDelegate UserFinishedEditing;
        bool isFinished = false;
        Dictionary<DBSeries, List<DBEpisode>> localeps = new Dictionary<DBSeries, List<DBEpisode>>();
        Dictionary<DBSeries, List<DBOnlineEpisode>> onlineeps = new Dictionary<DBSeries, List<DBOnlineEpisode>>();
        
        List<KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>> matches = new List<KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>>();
        List<DBEpisode> displayedEps = new List<DBEpisode>();
        List<DBOnlineEpisode> displayedOEps = new List<DBOnlineEpisode>();

        delegate void MatchEpisodesForSeriesDelegate(DBSeries series, List<DBEpisode> localEpisodes, List<DBOnlineEpisode> onlineCandidates);
        delegate List<KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>> GetResultDelegate(bool showonly);

        public ImportPanelEpID()
        {
            InitializeComponent();
        }

        public void MatchEpisodesForSeries(DBSeries series, List<DBEpisode> localEpisodes, List<DBOnlineEpisode> onlineCandidates)
        {
            if (this.InvokeRequired)
                this.Invoke(new MatchEpisodesForSeriesDelegate(MatchEpisodesForSeries), series, localEpisodes, onlineCandidates);
            
            // only those not id'ed before
            tryAddToList(series, localEpisodes, localeps);
            tryAddToList(series, onlineCandidates, onlineeps);             
        }

        private void FillSeriesList()
        {
            this.listBoxSeries.Items.Clear();
            this.listBoxOnline.Items.Clear();
            this.listBoxLocal.Items.Clear();
            foreach (var s in matches.Where(leps => leps.Value.Count > 0).Select(m => m.Key))
            {
                if (!listBoxSeries.Items.Contains(s))
                {
                    if(!checkBoxFilter.Checked || !seriesHasAllEpsMatched(s))
                        this.listBoxSeries.Items.Add(s);
                }
            }
            if (listBoxSeries.SelectedIndex < 0 && listBoxSeries.Items.Count > 0) 
                listBoxSeries.SelectedIndex = 0;

            if (listBoxSeries.Items.Count == 0)
            {
                comboMatchOptions.Enabled = false;
                comboMatchOptions.Items.Clear();
                buttonMatchAgain.Enabled = false;
                txtBoxStatusBar.Text = "There are no episodes requiring manual selection...";
            }
            else
            {
                comboMatchOptions.Enabled = true;
                buttonMatchAgain.Enabled = true;
            }
        }

        private bool seriesHasAllEpsMatched(DBSeries series)
        {
            int unMatchedCount = matches.Single(s => s.Key == series).Value.Count(p => p.Value == null);
            return unMatchedCount == 0;
        }

        private void tryAddToList<T>(DBSeries series, List<T> episodes, Dictionary<DBSeries, List<T>> dic)
        {
            if (dic.ContainsKey(series))
                dic[series] = episodes;
            else dic.Add(series, episodes);
        }

        public WindowPlugins.GUITVSeries.Feedback.ReturnCode GetResult(out List<KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>> result)
        {
            if (this.InvokeRequired)
                this.Invoke(new GetResultDelegate(getResultImpl), true);
            else getResultImpl(true);

            while (!isFinished) System.Threading.Thread.Sleep(1000); // block thread till we are done
            
            if (this.InvokeRequired)
                result = this.Invoke(new GetResultDelegate(getResultImpl), false) as List<KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>>;
            else result = getResultImpl(false);
            return WindowPlugins.GUITVSeries.Feedback.ReturnCode.OK;
        }

        private List<KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>> getResultImpl(bool requestShowOnly)
        {
            if (requestShowOnly)
            {
                if (UserFinishedEditing != null)
                {
                    UserFinishedEditing(null, UserFinishedRequestedAction.ShowMe);
                    ImportWizard.OnWizardNavigate += new ImportWizard.WizardNavigateDelegate(ImportWizard_OnWizardNavigate);
                }

                DoAutoMatchingForAll();
                FillSeriesList();
                return null;
            }
            else
            {
                bool isSecondPart = false;

                // update local episode ids with chosen episode ids
                foreach (var series in matches)
                {
                    foreach (var pair in series.Value)
                    {
                        if (pair.Value != null)
                        {
                            // check if its a double episode
                            isSecondPart = false;
                            if (!string.IsNullOrEmpty(pair.Key[DBEpisode.cCompositeID2]))
                            {
                                // check if its the second part of a double episode
                                if (pair.Key[DBEpisode.cEpisodeIndex] == pair.Key[DBEpisode.cEpisodeIndex2])
                                    isSecondPart = true;
                            }
                            pair.Key.ChangeIndexes(pair.Value[DBOnlineEpisode.cSeasonIndex], pair.Value[DBOnlineEpisode.cEpisodeIndex], isSecondPart);
                        }
                    }
                }

                return matches;
            }
        }

        private void DoAutoMatchingForAll()
        {
            foreach (var series in localeps)
            {
                DoAutoMatching(series.Key, null);
            }
        }

        private void DoAutoMatching(DBSeries series, string orderingOption)
        { 
            var seriesMatches = matches.SingleOrDefault(kv => kv.Key == series);
            List<DBEpisode> localEps = localeps[series];

            var newseriesMatches = new List<KeyValuePair<DBEpisode, DBOnlineEpisode>>();
            foreach (var localEp in localEps)
            {
                var bestMatchVal = from oe in onlineeps.Single(s => s.Key == series).Value
                                   select new { Episode = oe, MatchValue = OnlineParsing.matchOnlineToLocalEpisode(series, localEp, oe, orderingOption) };
                var matchedEp = bestMatchVal.OrderBy(me => me.MatchValue).FirstOrDefault(me => me.MatchValue < int.MaxValue);
                if (matchedEp != null)
                    newseriesMatches.Add(new KeyValuePair<DBEpisode, DBOnlineEpisode>(localEp, matchedEp.Episode));
                else newseriesMatches.Add(new KeyValuePair<DBEpisode, DBOnlineEpisode>(localEp, null));

            }
            if (seriesMatches.Key != null)
                matches.Remove(seriesMatches);
            matches.Add(new KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>(series, newseriesMatches));                     
        }

        private void ImportWizard_OnWizardNavigate(UserFinishedRequestedAction reqAction)
        {
            if (reqAction == UserFinishedRequestedAction.Next) isFinished = true;
            
            if (UserFinishedEditing != null)
                UserFinishedEditing(null, reqAction);

            // we no longer need to listen to navigate event
            ImportWizard.OnWizardNavigate -= new ImportWizard.WizardNavigateDelegate(ImportWizard_OnWizardNavigate);
        }

        private void listBoxSeries_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedSeries = listBoxSeries.SelectedItem as DBSeries;
            var selectedSeriesListLocal = localeps[selectedSeries];
            var selectedSeriesListOnline = onlineeps[selectedSeries];
            fillEpGrid(selectedSeriesListLocal, selectedSeriesListOnline);

            // set the available sort options
            this.comboMatchOptions.Items.Clear();
            foreach (var ordering in selectedSeries[DBOnlineSeries.cEpisodeOrders].ToString().Split(new char[]{'|'}, StringSplitOptions.RemoveEmptyEntries))
	        {
                comboMatchOptions.Items.Add(ordering); 
	        }
            
            // Always add 'Aired' as a match option
            if (comboMatchOptions.Items.Count == 0) comboMatchOptions.Items.Add("Aired");
            // 'Title' can also be matched against so add that
            comboMatchOptions.Items.Add("Title");
            
            string preChosen = selectedSeries[DBOnlineSeries.cChosenEpisodeOrder];
            if (string.IsNullOrEmpty(preChosen))
                comboMatchOptions.SelectedIndex = 0;
            else comboMatchOptions.SelectedItem = preChosen;
        }

        private string getDisplayString(DBTable ep)
        {
            if (ep is DBEpisode)
            {
                return string.Format("{0,2:D}x{1,2:D}{2} {3}",
                    ep[DBEpisode.cSeasonIndex],
                    ep[DBEpisode.cEpisodeIndex] > 0 ? (string)ep[DBEpisode.cEpisodeIndex] : "?",
                    ep[DBEpisode.cEpisodeIndex2] > 0 ? "-" + ep[DBEpisode.cEpisodeIndex2] : string.Empty,
                    ep[DBEpisode.cEpisodeName].ToString());
            }
            else if (ep is DBOnlineEpisode)
            {
                return string.Format("{0,2:D}{1,2:D}{2} - {3} [{4}]",
                    ep[DBEpisode.cSeasonIndex] > 0 ? (string)ep[DBEpisode.cSeasonIndex] + "x" : "Special: ",
                    ep[DBEpisode.cEpisodeIndex] > 0 ? (string)ep[DBEpisode.cEpisodeIndex] : "?",
                    ep[DBEpisode.cEpisodeIndex2] > 0 ? "-" + ep[DBEpisode.cEpisodeIndex2] : string.Empty,
                    ep[DBOnlineEpisode.cEpisodeName], ep[DBOnlineEpisode.cFirstAired]);
            }
            return string.Empty;
        }

        private void fillEpGrid(List<DBEpisode> localEps, List<DBOnlineEpisode> onlineEps)
        {
            listBoxLocal.Items.Clear();
            listBoxOnline.Items.Clear();
            displayedEps.Clear();
            displayedOEps.Clear();

            displayedOEps.Add(null);
            listBoxOnline.Items.Add("No Match");
            foreach (var online in onlineEps)
            {
                displayedOEps.Add(online);
                listBoxOnline.Items.Add(getDisplayString(online));
            }
            foreach (var local in localEps.OrderBy( e => e[DBEpisode.cSeasonIndex] * 100 + e[DBEpisode.cEpisodeIndex]))
            {
                if (checkBoxFilter.Checked)
                {
                    // only display those we couldnt match
                    var pair = matches.Single(s => s.Key == listBoxSeries.SelectedItem as DBSeries).Value.SingleOrDefault(e => e.Key == local);
                    if (pair.Value != null)
                        continue;
                }
                displayedEps.Add(local);
                listBoxLocal.Items.Add(local[DBEpisode.cFilenameWOPath]);
                //listBoxLocal.Items.Add(getDisplayString(local));                
            }

            if (listBoxLocal.SelectedIndex < 0 && listBoxLocal.Items.Count > 0)
                listBoxLocal.SelectedIndex = 0;
        }

        private void listBoxLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            var series = listBoxSeries.SelectedItem as DBSeries;
            var localEp = displayedEps[listBoxLocal.SelectedIndex];            
            var matchedOnlineEp = matches.Single(s => s.Key == series).Value.Single(eps => eps.Key == localEp).Value;          
            
            string selectedItem = string.Empty;
            if (matchedOnlineEp != null)
                listBoxOnline.SelectedItem = getDisplayString(matchedOnlineEp);
            else listBoxOnline.SelectedIndex = 0;
       
            txtBoxStatusBar.Text = localEp[DBEpisode.cFilename];
        }

        private void listBoxOnline_SelectedIndexChanged(object sender, EventArgs e)
        {
            var series = listBoxSeries.SelectedItem as DBSeries;
            var localEp = displayedEps[listBoxLocal.SelectedIndex];
            var onlineEp = displayedOEps[listBoxOnline.SelectedIndex];
            
            // update the pair
            var pairS = matches.Single(s => s.Key == series).Value;
            int toReplace = pairS.FindIndex(kv => kv.Key == localEp);
            pairS[toReplace] = new KeyValuePair<DBEpisode, DBOnlineEpisode>(localEp, onlineEp);
        }

        private void checkBoxFilter_CheckedChanged(object sender, EventArgs e)
        {
            txtBoxStatusBar.Text = string.Empty;
            FillSeriesList();
        }

        private void buttonMatchAgain_Click(object sender, EventArgs e)
        {
            string selected = comboMatchOptions.SelectedItem.ToString();
            DBSeries series = listBoxSeries.SelectedItem as DBSeries;
            series[DBOnlineSeries.cChosenEpisodeOrder] = selected;
            // default sort order should correspond to the chosen episode order
            series[DBOnlineSeries.cEpisodeSortOrder] = selected == "DVD" ? "DVD" : "Aired";
            series.Commit();
            DoAutoMatching(series, selected);
            listBoxSeries_SelectedIndexChanged(listBoxSeries, null);
            comboMatchOptions.SelectedItem = selected;
        }
    }
}
