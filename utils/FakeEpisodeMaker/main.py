import urllib
import os
import openAnything
from xml.dom import minidom

def replace_all(text, dic):
    for i, j in dic.iteritems():
        text = text.replace(i, j)
    return text

def parseShow(seriesID, show_name):
    reps =  {'\\':'-', '/':'-', ':':'', '?':'', '"':'\'', '*':'', '<':'', '>':'', '|':'', 'U+201C':'\'', 'U+201D':'', '\t':''}
    safe_show_name = replace_all(show_name, reps)
    details_url = "http://thetvdb.com/api/EB49E8B9E78EBEE1/series/"+seriesID+"/all/en.xml"
    details = openAnything.fetch(details_url)
    details_xml = minidom.parseString(details['data'])
    epnames = details_xml.getElementsByTagName("EpisodeName")
    seasons = details_xml.getElementsByTagName("SeasonNumber")
    episodes = details_xml.getElementsByTagName("EpisodeNumber")    
    # check to see if parent show path needs to be made
    if not os.access(safe_show_name, os.F_OK):
        os.makedirs(safe_show_name)
    i = 0
    for item in episodes:
        season = seasons[i].firstChild.data
        episode = item.firstChild.data        
        try:
			epname = epnames[i].firstChild.data
        except:
            epname = "Season "+season+" Episode "+episode
        safe_episode_name = replace_all(epname, reps)
        filename = safe_show_name+" - S"+season.zfill(2)+"E"+episode.zfill(2)+" - "+safe_episode_name        
        path = safe_show_name + "\\Season " + season
        # seeif season path exists or not, and make it if not
        if os.access(path, os.F_OK):
            # path/filename may exceed 255 chars
            fullpath = path + "\\" + filename
            safepath = fullpath[0:200]+".avi"
            # just go ahead and create the file
            file = open(safepath, "w")
            file.close()
        else:
            os.makedirs(path)
            # path/filename may exceed 255 chars
            fullpath = path + "\\" + filename
            safepath = fullpath[0:200]+".avi"
            file = open(safepath, "w")
            file.close()
        try:
            print "Creating %s" % safepath
        except:
            print "Creating %s" % "Season "+season+" Episode "+episode
        i = i + 1
        
show_file = open("shows.txt")
shows = show_file.read().split("\n")
show_file.close()
for item in shows:
    show_url = "http://thetvdb.com/api/GetSeries.php?"+urllib.urlencode({"seriesname":item})
    print "Building "+item+"..."
    show_xml = openAnything.fetch(show_url)
    xmldoc = minidom.parseString(show_xml['data'])
    node = xmldoc.getElementsByTagName("seriesid")
    if ("node" in dir()):
        seriesID = node[0].firstChild.data
        parseShow(seriesID, item)
    else:
        print "Could not find any data for "+show_name+" on TVDB.\nURL: "+show_url
