using System.Collections.Generic;
using System.Xml;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    class GetActors
    {
        public List<DBActor> Actors
        {
            get { return mActors; }
        }

        readonly List<DBActor> mActors = new List<DBActor>();

        public GetActors(int seriesID)
        {
            // check if local
            mActors = DBActor.GetAll(seriesID);

            // success
            if (mActors.Count != 0) return;

            // get cached actors or download

            //TODO: Get TMDb Actors

            //XmlNode node = OnlineAPI.GetActorsList(seriesID);
            //if (node == null) return;

            //// add actors to database
            //foreach (XmlNode actorNode in node.SelectNodes("/Actors/Actor"))
            //{
            //    DBActor actor = new DBActor();

            //    actor[DBActor.cSeriesID] = seriesID;

            //    foreach (XmlNode propertyNode in actorNode)
            //    {
            //        actor[propertyNode.Name] = propertyNode.InnerText;
            //    }
            //    actor.Commit();
            //    mActors.Add(actor);
            //}

        }
    }

}
