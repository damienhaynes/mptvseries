using System.Collections.Generic;
using WindowPlugins.GUITVSeries.TmdbAPI.DataStructures;

namespace WindowPlugins.GUITVSeries.Online_Parsing_Classes
{
    class GetActors
    {
        private readonly List<DBActor> mActors = new List<DBActor>();

        public List<DBActor> Actors
        {
            get { return mActors; }
        }

        public GetActors(int aSeriesId)
        {
            // check if local
            mActors = DBActor.GetAll(aSeriesId);

            // success
            if (mActors.Count != 0) return;

            // get cached actors or download
            string lLanguage = OnlineAPI.GetSeriesLanguage(aSeriesId);

            TmdbShowDetail lShowDetail = TmdbAPI.TmdbCache.LoadSeriesFromCache(aSeriesId, lLanguage);
            if (lShowDetail == null || lShowDetail.Credits == null) return;

            foreach(TmdbCast person in lShowDetail.Credits.Cast)
            {
                var lActor = new DBActor();

                lActor[DBActor.cSeriesID] = aSeriesId;
                lActor[DBActor.cName] = person.Name;
                lActor[DBActor.cRole] = person.Character;
                lActor[DBActor.cImage] = person.ProfilePath != null ? "original" + person.ProfilePath : string.Empty;
                lActor[DBActor.cIndex] = person.Id;
                lActor[DBActor.cSortOrder] = person.Order;
                lActor[DBActor.cOriginalName] = person.OriginalName;
                lActor[DBActor.cPopularity] = person.Popularity;
                lActor[DBActor.cKnownForDepartment] = person.KnownForDepartment;
                lActor[DBActor.cGender] = person.Gender ?? 0;

                lActor.Commit();
                mActors.Add(lActor);
            }
        }
    }

}
