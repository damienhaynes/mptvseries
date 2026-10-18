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
using System.Linq;
using System.Xml;
using WindowPlugins.GUITVSeries.Online_Parsing_Classes;

namespace WindowPlugins.GUITVSeries
{
  class GetSeries
  {
    /// <summary>
    /// The maximum string Levenshtein Distance for an auto-match.
    /// </summary>
    public static int FuzzyMatching_MaxLSDistance = 2;

    public List<DBOnlineSeries> Results
    {
      get
      {
        return mListSeries;
      }
    }
    private readonly List<DBOnlineSeries> mListSeries = new List<DBOnlineSeries>();

    public DBOnlineSeries PerfectMatch { get; protected set; }

    DBOnlineSeries RankSearchResults( string name, IList<DBOnlineSeries> candidates, out List<DBOnlineSeries> orderedCandidates )
    {
      // remove any non alphabet characters
      string cleanedName = name.ToLowerInvariant().Trim().RemoveSpecialCharacters();

      // calculate distances basis the series names
      var bestMatch = ( from candidate in candidates
                        select new
                        {
                          LSDistance = MediaPortal.Util.Levenshtein.Match( cleanedName, candidate[ DBOnlineSeries.cPrettyName ].ToString().ToLowerInvariant().RemoveSpecialCharacters() ),
                          Series = candidate
                        } );

      // calculate distances basis series alias names
      bestMatch = bestMatch.Union( from candidate in candidates.Where( c => !string.IsNullOrEmpty( c[ DBOnlineSeries.cAliasNames ] ) )
                                   from alias in candidate[ DBOnlineSeries.cAliasNames ].ToString().Split( '|' )
                                   select new
                                   {
                                     LSDistance = MediaPortal.Util.Levenshtein.Match( cleanedName, alias.ToLowerInvariant().RemoveSpecialCharacters() ),
                                     Series = candidate
                                   } );

      // make results unique
      var uniqueResults = from candidate in bestMatch
                          group candidate by (int)candidate.Series[ DBOnlineSeries.cID ];

      // now order the series by their minLSDistance (each ID can have several series and thus names)
      // we dont care which one won, we just want the minimum it scored
      // we also pick out the series in the users language, and the English language
      var weightedUniqueResults = from ur in uniqueResults
                                  select new
                                  {
                                    MinLSDistance = ur.Min( r => r.LSDistance ),
                                    SeriesScored = ur.OrderBy( r => r.LSDistance ).FirstOrDefault(),
                                    SeriesUserLang = ur.FirstOrDefault( r => r.Series[ "language" ] == r.Series[ DBOnlineSeries.cLanguage ] ),
                                    SeriesEng = ur.FirstOrDefault( r => r.Series[ "language" ] == "en" ),
                                  };

      // now decide which one to display
      // 1) users language 2) English 3) whichever scored our best result, this has to exist
      var weightedDisplayResults = from dr in weightedUniqueResults
                                   orderby dr.MinLSDistance
                                   select new
                                   {
                                     LSDistance = dr.MinLSDistance,
                                     Series = ( dr.SeriesUserLang != null && dr.SeriesUserLang.Series != null ) ? dr.SeriesUserLang.Series
                                                  : ( dr.SeriesEng != null && dr.SeriesEng.Series != null ) ? dr.SeriesEng.Series
                                                  : dr.SeriesScored.Series,
                                   };

      // give the ordered results back (for displaying)
      orderedCandidates = weightedDisplayResults.Select( r => r.Series ).ToList();

      // get the best one that's under a certain distance, which is a bit more fuzzy than the perfect requirement before
      if ( Settings.isConfig || DBOption.GetOptions( DBOption.cAutoChooseSeries ) == 1 )
      {
        var best = weightedDisplayResults.FirstOrDefault( m => m.LSDistance <= FuzzyMatching_MaxLSDistance );
        if ( best != null )
          return best.Series;
      }
      return null;
    }

    public GetSeries( String aSeriesName, int aTVDBId = -1 )
    {
      // if we already have a TVDBId, fetch the series via the existing UpdateSeries parser
      if ( aTVDBId > 0 )
      {
        var updateSeries = new UpdateSeries( aTVDBId.ToString() );
        if ( updateSeries.Results.Count > 0 )
        {
          mListSeries.Add( updateSeries.Results[ 0 ] );
          PerfectMatch = updateSeries.Results[ 0 ];
        }
        return;
      }

      string lUserLanguage = DBOption.GetOptions( DBOption.cOnlineLanguage );

      if ( DBOption.GetOptions( DBOption.cOverrideSearchLanguageToEnglish ) )
        lUserLanguage = "en";

      // search for series using TVDB v4 API
      try
      {
        // convert language to 3-letter ISO code for TVDB v4 API search
        var ci = new System.Globalization.CultureInfo( lUserLanguage );
        lUserLanguage = ci.ThreeLetterISOLanguageName.ToLowerInvariant();

        string bearer = DBOption.GetOptions( DBOption.cTVDBv4Token );
        if ( string.IsNullOrEmpty( bearer ) )
        {
          // attempt to login synchronously if we don't have a token yet
          bearer = TheTvdbV4Client.Login();
        }

        List<TheTvdbV4Client.TvdbSeries> tvResults = TheTvdbV4Client.SearchSeries( bearer, aSeriesName );
        if ( tvResults == null || tvResults.Count == 0 )
        {
          // nothing found
          return;
        }

        // map v4 results into DBOnlineSeries instances
        foreach ( TheTvdbV4Client.TvdbSeries tv in tvResults )
        {
          var lSeries = new DBOnlineSeries();

          // map core fields
          if ( !string.IsNullOrEmpty( tv.TvdbId ) ) lSeries[ DBOnlineSeries.cID ] = tv.TvdbId;

          // prefer the user's configured language for the display name/summary when available
          string prettyName = tv.Name;
          if ( tv.Translations != null && tv.Translations.Count > 0 && !string.IsNullOrEmpty( lUserLanguage ) )
          {
            if ( tv.Translations.TryGetValue( lUserLanguage, out string translatedName ) )
            {
               prettyName = translatedName;
            }
          }
          lSeries[ DBOnlineSeries.cPrettyName ] = prettyName;

          // prefer overview in user's language if available
          string summary = tv.Overview;
          if ( tv.Overviews != null && tv.Overviews.Count > 0 && !string.IsNullOrEmpty( lUserLanguage ) )
          {
            if ( tv.Overviews.TryGetValue( lUserLanguage, out string localizedOverview ) )
            {
              summary = localizedOverview;
            } 
          }
          if ( !string.IsNullOrEmpty( summary ) ) lSeries[ DBOnlineSeries.cSummary ] = summary;

          if ( !string.IsNullOrEmpty( tv.Slug ) ) lSeries[ DBOnlineSeries.cSlug ] = tv.Slug;
          if ( !string.IsNullOrEmpty( tv.Network ) ) lSeries[ DBOnlineSeries.cNetwork ] = tv.Network;
          if ( !string.IsNullOrEmpty( tv.Year ) ) lSeries[ DBOnlineSeries.cFirstAired ] = tv.Year;
          if ( !string.IsNullOrEmpty( tv.PrimaryLanguage ) ) lSeries[ DBOnlineSeries.cLanguage ] = tv.PrimaryLanguage; // <= convert back to 2 letter?

          // store original/english name when available
          if ( tv.Translations != null && tv.Translations.Count > 0 )
          {
            if ( tv.Translations.TryGetValue( "eng", out string engName ) )
            {
              if ( engName != prettyName )
                lSeries[ DBOnlineSeries.cOriginalName ] = engName;
              else if ( tv.Name != prettyName )
                lSeries[ DBOnlineSeries.cOriginalName ] = tv.Name;
            }
          }

          // lists
          if ( tv.Aliases != null && tv.Aliases.Count > 0 ) lSeries[ DBOnlineSeries.cAliasNames ] = string.Join( "|", tv.Aliases );
          if ( tv.Genres != null && tv.Genres.Count > 0 ) lSeries[ DBOnlineSeries.cGenre ] = string.Join( "|", tv.Genres );

          // images
          if ( !string.IsNullOrEmpty( tv.ImageUrl ) ) lSeries[ DBOnlineSeries.cPoster ] = tv.ImageUrl;
          else if ( !string.IsNullOrEmpty( tv.Thumbnail ) ) lSeries[ DBOnlineSeries.cPoster ] = tv.Thumbnail;

          // remote ids mapping (e.g. imdb, tmdb)
          if ( tv.RemoteIds != null )
          {
            foreach ( TheTvdbV4Client.TvdbRemoteId rid in tv.RemoteIds )
            {
              if ( rid == null || string.IsNullOrEmpty( rid.SourceName ) || string.IsNullOrEmpty( rid.Id ) )
                continue;

              string src = rid.SourceName.ToLowerInvariant();
              if ( src.Contains( "imdb" ) )
                lSeries[ DBOnlineSeries.cIMDBID ] = rid.Id;
              else if ( src.Contains( "themoviedb.com" ) )
                lSeries[ DBOnlineSeries.cTmdbId ] = rid.Id;
            }
          }

          mListSeries.Add( lSeries );
        }
      }
      catch ( Exception ex )
      {
        MPTVSeriesLog.Write( "TVDB v4 search failed: " + ex.Message );
        return;
      }

      // Rank and pick best match from mapped results
      PerfectMatch = RankSearchResults( aSeriesName, mListSeries, out mListSeries );
    }
  }
}
