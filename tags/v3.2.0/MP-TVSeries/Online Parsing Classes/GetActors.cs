using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    class GetActors
    {
        public List<DBActor> Actors
        {
            get { return _actors; }
        }
        List<DBActor> _actors = new List<DBActor>();

        public GetActors(int seriesID)
        {
            // check if local
            _actors = DBActor.GetAll(seriesID);

            // success
            if (_actors.Count != 0) return;

            // get cached actors or download
            XmlNode node = OnlineAPI.GetActorsList(seriesID);
            if (node == null) return;

            // add actors to database
            foreach (XmlNode actorNode in node.SelectNodes("/Actors/Actor"))
            {
                DBActor actor = new DBActor();

                actor[DBActor.cSeriesID] = seriesID;

                foreach (XmlNode propertyNode in actorNode)
                {
                    actor[propertyNode.Name] = propertyNode.InnerText;
                }
                actor.Commit();
                _actors.Add(actor);
            }

        }
    }

}
