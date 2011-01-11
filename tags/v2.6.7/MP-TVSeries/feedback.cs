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

namespace WindowPlugins.GUITVSeries.Feedback
{
    public class CItem
    {
        public CItem(String sName, String sDescription, object Tag)
        {
            m_sName = sName;
            m_sDescription = sDescription;
            m_Tag = Tag;
        }

        public String m_sName = String.Empty;
        public String m_sDescription = String.Empty;
        public object m_Tag;

        public override string ToString()
        {
            return m_sName;
        }
    }

    public class ChooseFromSelectionDescriptor
    {
        public String m_sTitle = Translation.CFS_Choose_Item;              // dialog title (if possible)
        public String m_sItemToMatchLabel = Translation.CFS_Looking_For;   // label of the thing to match
        public String m_sItemToMatch = String.Empty;                       // the thing to match
        public String m_sListLabel = Translation.CFS_Select_Matching_Item; // label of the list
        public List<CItem> m_List;                                         // the list items (names + descriptions)

        // buttons: no label => no button shown
        public String m_sbtnOKLabel = Translation.OK;
        public String m_sbtnOKLabelAlternate = Translation.CFS_Search_Again;
        public String m_sbtnSkipLabel = Translation.CFS_Skip;
        public String m_sbtnCancelLabel = Translation.Cancel;
        public String m_sbtnIgnoreLabel = Translation.Ignore;

        public bool m_useRadioToSelect = false;                            // just looks nicer for instance for OrderOption, ignored inside MP
        public bool m_allowAlter = true;                                   // if true user will be able to alter what was searched for 
    }

    public class ChooseFromYesNoDescriptor
    {
        public String m_sTitle = Translation.CFS_Choose_Item;              // dialog title (if possible)
        public String m_sLabel = Translation.CFS_Looking_For;              // label of the thing to mat
        public DialogButtons m_dialogButtons = DialogButtons.OK;
        public ReturnCode m_dialogDefaultButton = ReturnCode.OK;
    }

    public class GetStringFromUserDescriptor
    {
		public enum KeyboardStyles {
			KEYBOARD,
			SMS
		}
		
		/// <summary>
		/// Text to display when invoking the keyboard
		/// </summary>
		public String Text { get; set; }

		/// <summary>
		/// Set to TRUE to enable the Shift button 
		/// </summary>
        public bool ShiftEnabled { get; set; }

		/// <summary>
		/// Set to TRUE to Mask the Input		
		/// </summary>
		public bool IsPassword { get; set; }

		/// <summary>
		/// Set Keyboard style to NORMAL or SMS Style
		/// </summary>
		public KeyboardStyles KeyboardStyle { get; set; }
			
    }

    public enum DialogButtons
    {
        OK,
        YesNo,
        YesNoCancel
    }

    public enum ReturnCode
    {
        OK,
        Yes,
        No,
        Cancel,
        Ignore,
        NotReady
    }

    public interface IFeedback
    {
        ReturnCode ChooseFromSelection(ChooseFromSelectionDescriptor descriptor, out CItem selected);
        ReturnCode YesNoOkDialog(ChooseFromYesNoDescriptor descriptor);
        ReturnCode GetStringFromUser(GetStringFromUserDescriptor descriptor, out String input);
    }

    public interface IEpisodeMatchingFeedback
    {
        void MatchEpisodesForSeries(DBSeries series, List<DBEpisode> localEpisodes, List<DBOnlineEpisode> onlineCandidates);

        ReturnCode GetResult(out List<KeyValuePair<DBSeries, List<KeyValuePair<DBEpisode, DBOnlineEpisode>>>> result);
    }
}
