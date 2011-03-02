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
    };

    public class CDescriptor
    {
        public String m_sTitle = "Choose item";             // dialog title (if possible)
        public String m_sItemToMatchLabel = "Looking for:";  // label of the thing to match
        public String m_sItemToMatch = String.Empty;       // the thing to match
        public String m_sListLabel = "Select matching item:";         // label of the list
        public List<CItem> m_List;  // the list items (names + descriptions)

        // buttons: no label => no button shown
        public String m_sbtnOKLabel = "Ok";
        public String m_sbtnOKLabelAlternate = "Search Again";
        public String m_sbtnCancelLabel = "Cancel";
        public String m_sbtnIgnoreLabel = "Ignore";
    };

    public enum ReturnCode
    {
        OK,
        Cancel,
        Ignore
    }

    public interface Interface
    {
        ReturnCode ChooseFromSelection(CDescriptor descriptor, out CItem selected);
    }
}
