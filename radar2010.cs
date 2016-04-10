using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.IO;
using Npgsql;

class Radar2010
{
    public static void RefreshBasedOnRadarSignals_AndPublish( HashSet<Int32> considered )
    {
        //        SendPostToLJ();
        //        SendPostToLJ("For <lj user=mcfnord>", "some kibble");
        MMDB.MakeSureDBIsOpen();

        // first trash watches entirely? so no need to drop people.
        // then populate watches entirely. build it up! readers of ljreader. not who ljreader reads!
        new MyNpgsqlCommand("delete from watches;", MMDB.DBConnection).ExecuteNonQuery();

//        string fdataLJReader = FData.GetFData("ljfinder", true);
//        string fdataLJReader = FData.GetFData("mcfnord", true);
//        HashSet<Int32> whoReadsTheRobot = FData.IDsInTheyReadMeFData(fdataLJReader);
        considered.Remove(5731095); // no self!

        // ok, kill all adds that are oldder than 119 days. cuz i don't want to pblish offlines. or something.
        MMDB.ExecuteNonQuery(string.Format("delete from adds where daydetected < {0};", DateTime.Now.AddDays(-119).Subtract(Extras.TwoK).Days));

        // MAYBE SOMEDAY I EXCLUDE SOME PEOPLE WHO READ THE ROBOT BECAUSE THEY SUCK ASS AND DON'T PICK UP THE SHIT.
        foreach (var jokerWhoReadsTheRobot in considered)
        {
            foreach (var someoneJokerReads in FData.IDsInIReadFData(FData.GetFData(IDMap.IDToName(jokerWhoReadsTheRobot))))
            {
                MMDB.ExecuteNonQuery(string.Format("INSERT INTO watches (watcher, watched) Values({0}, {1});",
                    jokerWhoReadsTheRobot,
                    someoneJokerReads),false);
            }
        }

        // gather all actors in add events
        HashSet<Int32> watchers = new HashSet<int>();
        NpgsqlCommand cmd = new NpgsqlCommand("select distinct(watcher) from watches where watched IN (select distinct(actor) from adds) ;", MMDB.DBConnection);
        cmd.CommandTimeout = 0; // forever!
        NpgsqlDataReader myReader = cmd.ExecuteReader();
        while (myReader.Read())
            watchers.Add(myReader.GetInt32(0));
        myReader.Close();

        HashSet<Int32> everyoneImmaUpdate = new HashSet<int>();
        // I HATE RECOMMENDING RENAMES. I HATE IT SO MUCH THAT I WANT TO MAXIMALLY DETECT THEM IN A PRE-STEP.
        foreach (var watcher in watchers)
        {
            everyoneImmaUpdate.UnionWith(FData.IDsInIReadFData(FData.GetFData(IDMap.IDToName(watcher))));
        }
        everyoneImmaUpdate.ExceptWith(watchers);
        foreach (var someParty in everyoneImmaUpdate)
        {
            if( false == FData.FDataConfirmedCurrentEnufOn( IDMap.IDToName(someParty) ))
                FData.GetFData(IDMap.IDToName(someParty));
        }
        everyoneImmaUpdate = null;

        // imma gonna order watchers into orderedWatchers, based on last-published-to date
        // but first it's ok i'll run this through...

        string sqlset = "";
        foreach( var dude in watchers )
            sqlset += dude.ToString() + "," ;
        sqlset = sqlset.Substring(0, sqlset.Length - 1) ;
        myReader = new NpgsqlCommand(string.Format("select id from nameidmap where id in ({0}) order by inbox_hit_week ;", sqlset), MMDB.DBConnection).ExecuteReader();
        List<Int32> sortedWatchers = new List<int>();
        while( myReader.Read())
            sortedWatchers.Add( myReader.GetInt32(0)) ;
        myReader.Close() ;
        watchers = null;

        foreach (var watcher in sortedWatchers)
        {
            if (watcher == 1179796)
                Console.WriteLine("The bastardo.");

            HashSet<Int32> whoWatcherReads = FData.IDsInIReadFData(FData.GetFData(IDMap.IDToName(watcher)));

            if (whoWatcherReads.Contains(watcher)) // ditch read-self!
                whoWatcherReads.Remove(watcher);

            // tell me who was added by all the parties we watch
            Dictionary<Int32, HashSet<Int32>> everyTarget = new Dictionary<Int32, HashSet<Int32>>();
            string whoWatcherReadsInSQLSet = "";
            foreach (var someone in whoWatcherReads)
                whoWatcherReadsInSQLSet += someone.ToString() + ",";
            whoWatcherReadsInSQLSet = whoWatcherReadsInSQLSet.Substring(0, whoWatcherReadsInSQLSet.Length - 1);

            // rule out anyone if i ever ever read them before.
            // my encyclopedic archive rules i guess!
            List<int> daysOfTheFData = FData.GetFDataDates(IDMap.IDToName(watcher));
            HashSet<Int32> everyoneIEverRead = new HashSet<Int32>();
            foreach (var day in daysOfTheFData)
            {
                string anFD = FData.FDataBy2kDay(IDMap.IDToName(watcher), day);
                everyoneIEverRead.UnionWith(FData.IDsInIReadFData(anFD));
            }

            if (everyoneIEverRead.Contains(watcher))
                everyoneIEverRead.Remove(watcher); // remove watcher manually

            if (watcher == 1)
                if (everyoneIEverRead.Contains(578))
                    Console.WriteLine("I read scribble before.");

            cmd = new NpgsqlCommand(string.Format("select target, actor from adds where actor IN ({0});", whoWatcherReadsInSQLSet), MMDB.DBConnection);
            myReader = cmd.ExecuteReader();
            while (myReader.Read())
            {
                Int32 iTarget = myReader.GetInt32(0);

                // skip self-whatevers.
                if (iTarget == watcher)
                    continue;

                // skip ljfinder!
                if (iTarget == 5731095)
                    continue;

                // detected as offline in last 120 days?
                NpgsqlDataReader innerReader = new MyNpgsqlCommand(
                    string.Format("select offline_last_detected_on from nameidmap where id={0};", iTarget), MMDB.DBConnection).ExecuteReader();
                innerReader.Read();
                Int16? offline_detected_on = MMDB.MaybeNullInt16(innerReader, 0); // overload with a no-local-reader option! rolls royce db!
                innerReader.Close();
                if (null != offline_detected_on)
                    if (offline_detected_on > DateTime.Now.AddDays(-120).Subtract(Extras.TwoK).Days)
                        continue;

                if (everyoneIEverRead.Contains(iTarget))
                    continue;

                // MORITORIUM ALSO ON ANYTHING THAT IS DEAD WTF
                // KILL IT WITH ANY FUCKING FEATURE... UPDATEPPY? 404!

                // moritorium on any target that has been detected as a rename in the last 120 days
                innerReader = new MyNpgsqlCommand(
                    string.Format("select made_by_rename_detected_on from nameidmap where id={0};", iTarget), MMDB.DBConnection).ExecuteReader();
                innerReader.Read();
                Int16? day_rename_detected = MMDB.MaybeNullInt16(innerReader, 0); // overload with a no-local-reader option! rolls royce db!
                innerReader.Close();
                if (null != day_rename_detected)
                    if (day_rename_detected > DateTime.Now.AddDays(-120).Subtract(Extras.TwoK).Days)
                        continue;

                // create and add
                if (false == everyTarget.ContainsKey(iTarget))
                {
                    // if it's known offline i'm gonna hit this way too hard. i just need to remember somehow that i hit it.
               //     Extras.CheckForRenameOrOffline(iTarget);
                        everyTarget.Add(iTarget, new HashSet<Int32>());
                }
                int iActor = myReader.GetInt32(1);

                ////////////////////// diagnose how actor == watcher, ever.
//                Console.WriteLine("Target: {0}, Actor: {1}, Watcher: {2}", IDMap.IDToName(iTarget), IDMap.IDToName(iActor), IDMap.IDToName(watcher));
                ////////////////////// diagnose how actor == watcher, ever.

                System.Diagnostics.Debug.Assert(iActor != watcher); // watcher should not be in the IN set that does this query!
                everyTarget[iTarget].Add(iActor); // add the actor. this is why we can't just use a big IN sql statement. or can we?
            }
            myReader.Close();

            // only publish if i've never published to this user, or barring that, at least one target must have more than one actor.
            cmd = new NpgsqlCommand(string.Format("select count(*) from radarpicks where userid={0} ;", watcher), MMDB.DBConnection);
            myReader = cmd.ExecuteReader();
            myReader.Read();
            Int64? any = MMDB.MaybeNullInt64(myReader, 0);
            myReader.Close();

            if (everyTarget.Count > 0) // else we crash so fuck this situation.
            {
                if (any == 0 || everyTarget.Max(targ => targ.Value.Count) > 1)
                {
                    List<HashSet<Int32>> actors = new List<HashSet<Int32>>();
                    List<HashSet<Int32>> targets = new List<HashSet<Int32>>();

                    var sortedGroups =
                        (from g in everyTarget
                         where g.Value.Count > ((any == 0) ? 0 : 1) // if we've never published anything, lower the standard to just 1
                         orderby g.Value.Count descending
                         select g).Take(12);

                    // fold the sorted groups into the two dictionaries.
                    foreach (var thisG in sortedGroups)
                    {
                        // if these actors are already in my actor list, then we skip
                        foreach (var existing in actors)
                        {
                            if (thisG.Value.SetEquals(existing)) // wha'ts the right way??? cuz it's not this.
                                goto ALREADY;
                        }

                        // i know its key, so i can look for equal sets that aren't it.
                        var sameActors =
                            from x in everyTarget
                            where (x.Key != thisG.Key && x.Value.SetEquals(thisG.Value)) // so same actors, different key, so excludes thisG!
                            select x;

                        // All of them are added as a same 'entry' slot into actors and targets sets.
                        actors.Add(thisG.Value);
                        System.Diagnostics.Debug.Assert(false == thisG.Value.Contains(watcher)); // watchers aren't actors in their deeds! 

                        HashSet<Int32> targs = new HashSet<int>();
                        targs.Add(thisG.Key);
                        foreach (var same in sameActors)
                            targs.Add(same.Key);
                        targets.Add(targs);

                    ALREADY:
                        ;
                    }

                    // divide into two groups, based on whether i've published this name to this user before.]
                    // builds a logic array of whether anyone in this set has been published before.
                    List<bool> publishedBefore = new List<bool>();
                    foreach (var targSet in targets)
                    {
                        bool fPublishedBefore = true;
                        foreach (var t in targSet)
                        {
                            cmd = new NpgsqlCommand(string.Format("select count(*) from radarpicks where userid={0} and userid_recommended={1};", watcher, t), MMDB.DBConnection);
                            myReader = cmd.ExecuteReader();
                            myReader.Read();
                            try
                            {
                                fPublishedBefore = (myReader.GetInt64(0) > 0);
                                if (false == fPublishedBefore)
                                    break; // just one "news" in the set means NO, not published before!
                            }
                            finally
                            {
                                myReader.Close();
                            }
                        }
                        publishedBefore.Add(fPublishedBefore);
                    }

                    //// TOTAL EXPERIMENT BEFORE THE PUBLISHED B4 CHECK.
                    //// TOTAL EXPERIMENT BEFORE THE PUBLISHED B4 CHECK.
                    /*
                    foreach (var set in actors)
                    {
                        List<List<Int32>> bunches = TCliquesClass.CustomGroupMain(watcher, set); // love this but need specialized "top set matters" timeout mode.
                        // that clique finder puts the watcher into the set. i take it out!
                        set.Remove(watcher);

                        Console.WriteLine("Largest actor clique: ");
                        foreach (var party in bunches[0])
                        {
                            Console.Write(IDMap.IDToName(party) + " ");
                        }
                        Console.WriteLine();
                    }
                     * */
                    //// TOTAL EXPERIMENT BEFORE THE PUBLISHED B4 CHECK.
                    //// TOTAL EXPERIMENT BEFORE THE PUBLISHED B4 CHECK.

                    // is there anything new to publish?
                    if (any > 0) // if any == 0, then we publish cuz we never published b4
                        if (false == publishedBefore.Contains(false))
                        {
                            Console.WriteLine("Nothin new to publish, just stuff I already found.");
                            continue;
                        }

                    string title = "Hey <lj user=" + IDMap.IDToName(watcher) + ">";
                    string archiveContent = "";
                    string newContent = "";

                    string names = "";
                    foreach (var t in targets)
                        foreach (var item in t)
                            names += IDMap.IDToName(item) + "|";
                    names = names.Substring(0, names.Length - 1);

                    // in conclusion, a link to the visual
                    // string content = "<ul><li><a href='http://ljmindmap.com/h.php?n=" + IDMap.IDToName(watcher) + "'>&#1058;&#1077;&#1089;&#1077;&#1085; &#1052;&#1080;&#1088; / MindMap</a>";
                    string content = "<table><tr><td><center><a href='http://ljmindmap.com/h.php?n=" + IDMap.IDToName(watcher) + "'><img width='500' src='http://ljmindmap.com/s/?f="
                        + IDMap.IDToName(watcher) + ".gif'><br>&#1058;&#1077;&#1089;&#1077;&#1085; &#1052;&#1080;&#1088; / MindMap</a></td></tr></table><br>";
                    /*
                     * 
                     * 
                     * 
        <table><tr><td><center><a href="http://ljmindmap.com/h.php?n=micaturtle"><img width="500" src="http://ljmindmap.com/s/?f=micaturtle.gif"><br>&#1058;&#1077;&#1089;&#1077;&#1085; &#1052;&#1080;&#1088; / MindMap</a></td></tr></table><br>
                     */

                    //                content += string.Format("<li><a href='http://ljmindmap.com/mass_add.php?a={0}'>Surf these LJs</a></ul>", names);
                    content += "<table border='1'><tr><th><u>New LJs</u><th><u>Found by</u></tr>";

                    System.Diagnostics.Debug.Assert(actors.Count == targets.Count);
                    for (int iPos = 0; iPos < actors.Count; iPos++)
                    {
                        string contentFragment = "<tr><td>";
                        foreach (var t in targets[iPos])
                            contentFragment += "<lj user=" + IDMap.IDToName(t) + "> ";

                        contentFragment += "<td><b>";
                        foreach (var a in actors[iPos])
                        {
                            System.Diagnostics.Debug.Assert(a != watcher);
                            contentFragment += IDMap.IDToName(a) + " ";
                        }
                        contentFragment += "</b></tr>";

                        if (publishedBefore[iPos])
                            archiveContent += contentFragment;
                        else
                            newContent += contentFragment;
                    }

                    content += newContent;
                    content += "</table>";
                    if (archiveContent.Length > 0)
                    {
                        content += "<br><h3>Archive</h3><table>";
                        content += archiveContent;
                        content += "</table>";
                    }

                    string url = AddOrUpdateLJPost(watcher, title, content);

                    if (null == url)
                    {
                        // need to just re-post.
                    }

                    System.Diagnostics.Debug.Assert(url.Length > 0);

                    Console.WriteLine(url);

                    string emailcontent = "\r\nI've made your own custom MindMap and found some LJs your friends are adding. Yay!\r\n\r\n" + url + "\r\n\r\n- lil ljfinder";
                    NotifyViaLJInbox(watcher, emailcontent, url);
                    NoteRecommendeds(watcher, targets);
                    //NotifyViaLJBanner(watcher, emailcontent, url);
                }
            }
        }
        // still need to kill old events somewhere! at 110 days (rename prevention stops at 120)
    }

    static string AddOrUpdateLJPost(Int32 user, string title, string content)
    {
        // figger out if this watcher has a post because if so, then i update that
        string strCmd = string.Format("select radar_post_id from nameidmap where id={0};", user);
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, MMDB.DBConnection);
        NpgsqlDataReader myReader = null;
        myReader = cmd.ExecuteReader();
        myReader.Read();
        Int32? itemId = MMDB.MaybeNullInt32(myReader, 0);
        myReader.Close();

        string url = "";
    JUST_MAKE_ONE:
        if (itemId == null)
        {
            Int32 newItemId = 0 ;
            url = CreateLJPost(title, content, ref newItemId);
            // store this itemID for this watcher.
            MMDB.ExecuteNonQuery(string.Format("update nameidmap set radar_post_id={0} where id={1};", newItemId, user));
        }
        else
        {
            url = UpdateLJPost( title, content, itemId );
            if (null == url)
            {
                itemId = null;
                goto JUST_MAKE_ONE;
            }
        }

        return url;
    }


    static void NoteRecommendeds(Int32 userid, List<HashSet<Int32>>  targets_recommended)
    {
        // i want one entry for each watcher-target combination, but i'll let the unique index assure it right? eventually.
        foreach (var t in targets_recommended)
        {
            foreach (var one in t)
            {
                try
                {
                    MMDB.ExecuteNonQuery(string.Format("INSERT INTO radarpicks (userid, userid_recommended) Values({0}, {1});",
                            userid,
                            one));
                }
                catch (NpgsqlException)
                {
                    // ok i'm crazy but just move on it's probably a unique constraint failure which is by design
                }
            }
        }
    }


    static void NotifyViaLJInbox(Int32 toWhom, string content, string url)
    {
        XDocument xd = new XDocument();
        xd.Add(new XElement("methodCall",
                    new XElement("methodName", "LJ.XMLRPC.sendmessage"),
                        new XElement("params",
                            new XElement("param",
                                new XElement("value",
                                    new XElement("struct",
                                        new XElement("member",
                                            new XElement("name", "username"),
                                            new XElement("value", new XElement("string", "ljfinder"))),
//                                            new XElement("value", new XElement("string", "mcfnord"))),
                                        new XElement("member",
                                            new XElement("name", "password"),
                                            new XElement("value", new XElement("string", "n00dleplex"))),
                                        new XElement("member",
                                            new XElement("name", "subject"),
                                            new XElement("value", new XElement("string", "New LJs and your MindMap at " + url ))),
                                        new XElement("member",
                                            new XElement("name", "body"),
                                            new XElement("value", new XElement("string", content))),
                                        new XElement("member",
                                            new XElement("name", "to"),
                                            new XElement("value", new XElement("string", IDMap.IDToName(toWhom))))))))));

        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] data = encoding.GetBytes("<?xml version=\"1.0\"?>" + xd.ToString());
        xd = null;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.livejournal.com/interface/xmlrpc");
        request.UserAgent = "http://ljmindmap.com/; livejournal.mindmap.jrd@xoxy.net";
        request.Method = "POST";
        request.ContentType = "text/xml"; //  "application/x-www-form-urlencoded";
        request.ContentLength = data.GetLength(0);

        Stream newStream = request.GetRequestStream();
        newStream.Write(data, 0, data.GetLength(0));
        newStream.Close();

        WebResponse response = null;
        response = request.GetResponse();
        Stream s = response.GetResponseStream();
        StreamReader sr = new StreamReader(s);
        string fd = sr.ReadToEnd();
        Console.WriteLine(fd);
//        System.Diagnostics.Debug.Assert(false == fd.Contains("ault"));
        if (fd.Contains("ault"))
        {
            if (false == fd.Contains("privacy options"))
                System.Diagnostics.Debug.Assert(false);// i hate any other fault.
        }
        // remember this day for this user
        //MMDB.ExecuteNonQuery(string.Format("update nameidmap set 
        // do we have a week indicator?
        DateTime TwoK = new DateTime(0x7d0, 1, 1);
        int weeks = (int)(DateTime.Now.Subtract(TwoK).Days / 7.02);
        MMDB.ExecuteNonQuery(string.Format("update nameidmap set inbox_hit_week={1} where id={0};", toWhom, weeks));


    }


    static string UpdateLJPost(string subject, string content, Int32? itemId) // returns url
    {
        System.Diagnostics.Debug.Assert(itemId != null);

        XDocument xd = new XDocument();
        xd.Add(new XElement("methodCall",
                    new XElement("methodName", "LJ.XMLRPC.editevent"),
                        new XElement("params",
                            new XElement("param",
                                new XElement("value",
                                    new XElement("struct",
                                        new XElement("member",
                                            new XElement("name", "username"),
                                            new XElement("value", new XElement("string", "ljfound"))),
                                        new XElement("member",
                                            new XElement("name", "password"),
                                            new XElement("value", new XElement("string", "n00dzzz"))),
                                        new XElement("member",
                                            new XElement("name", "ver"),
                                            new XElement("value",
                                                new XElement("int", "1"))),
                                        new XElement("member",
                                            new XElement("name", "itemid"),
                                            new XElement("value", new XElement("string", itemId.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "subject"),
                                            new XElement("value", new XElement("string", subject))),
                                        new XElement("member",
                                            new XElement("name", "event"),
                                            new XElement("value", new XElement("string", content))),
                                        new XElement("member",
                                            new XElement("name", "lineendings"),
                                            new XElement("value", new XElement("string", "pc"))),
                                        new XElement("member",
                                            new XElement("name", "year"),
                                            new XElement("value", new XElement("int", DateTime.Now.Year.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "mon"),
                                            new XElement("value", new XElement("int", DateTime.Now.Month.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "day"),
                                            new XElement("value", new XElement("int", DateTime.Now.Day.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "hour"),
                                            new XElement("value", new XElement("int", DateTime.Now.Hour.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "min"),
                                            new XElement("value", new XElement("int", DateTime.Now.Minute.ToString())))))))));

        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] data = encoding.GetBytes("<?xml version=\"1.0\"?>" + xd.ToString());
        xd = null;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.livejournal.com/interface/xmlrpc");
        request.UserAgent = "http://ljmindmap.com/; livejournal.mindmap.jrd@xoxy.net";
        request.Method = "POST";
        request.ContentType = "text/xml"; //  "application/x-www-form-urlencoded";
        request.ContentLength = data.GetLength(0);

        Stream newStream = request.GetRequestStream();
        newStream.Write(data, 0, data.GetLength(0));
        newStream.Close();

        WebResponse response = null;
        response = request.GetResponse();
        Stream s = response.GetResponseStream();
        StreamReader sr = new StreamReader(s);
        string fd = sr.ReadToEnd();
        Console.WriteLine(fd);
        if (fd.Contains("ault"))
            return null; // repost me!

        System.Diagnostics.Debug.Assert(false == fd.Contains("ault"));

        /*
        const string SIGNAL = "itemid</name><value><int>";
        string itemidstr = fd.Substring(fd.IndexOf(SIGNAL) + SIGNAL.Length);
        itemidstr = itemidstr.Substring(0, itemidstr.IndexOf("<"));
        itemid = Int32.Parse(itemidstr);
         * 
         * */

        const string SIG = "url</name><value><string>";
        string url = fd.Substring(fd.IndexOf(SIG) + SIG.Length);
        url = url.Substring(0, url.IndexOf("<"));
        return url;
    }

    static string CreateLJPost(string subject, string content, ref Int32 itemid) // returns url
    {
        XDocument xd = new XDocument();
        xd.Add(new XElement("methodCall",
                    new XElement("methodName", "LJ.XMLRPC.postevent"),
                        new XElement("params",
                            new XElement("param",
                                new XElement("value",
                                    new XElement("struct",
                                        new XElement("member",
                                            new XElement("name", "username"),
                                            new XElement("value", new XElement("string", "ljfound"))),
                                        new XElement("member",
                                            new XElement("name", "password"),
                                            new XElement("value", new XElement("string", "n00dzzz"))),
                                        new XElement("member",
                                            new XElement("name", "subject"),
                                            new XElement("value", new XElement("string", subject))),
                                        new XElement("member", 
                                            new XElement("name", "ver"),
                                            new XElement("value", 
                                                new XElement("int", "1"))),
                                        new XElement("member",
                                            new XElement("name", "event"),
                                            new XElement("value", new XElement("string", content))),
                                        new XElement("member",
                                            new XElement("name", "lineendings"),
                                            new XElement("value", new XElement("string", "pc"))),
                                        new XElement("member",
                                            new XElement("name", "year"),
                                            new XElement("value", new XElement("int", DateTime.Now.Year.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "mon"),
                                            new XElement("value", new XElement("int", DateTime.Now.Month.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "day"),
                                            new XElement("value", new XElement("int", DateTime.Now.Day.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "hour"),
                                            new XElement("value", new XElement("int", DateTime.Now.Hour.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "min"),
                                            new XElement("value", new XElement("int", DateTime.Now.Minute.ToString()))),
                                        new XElement("member",
                                            new XElement("name", "props"),
                                            new XElement("value",
                                                new XElement("struct",
                                                    new XElement("member",
                                                        new XElement("name", "opt_backdated"),
                                                        new XElement("value", 
                                                            new XElement("boolean", "1"))))))))))));

        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] data = encoding.GetBytes("<?xml version=\"1.0\"?>" + xd.ToString());
        xd = null;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.livejournal.com/interface/xmlrpc");
        request.UserAgent = "http://ljmindmap.com/; livejournal.mindmap.jrd@xoxy.net";
        request.Method = "POST";
        request.ContentType = "text/xml"; //  "application/x-www-form-urlencoded";
        request.ContentLength = data.GetLength(0);

        Stream newStream = request.GetRequestStream();
        newStream.Write(data, 0, data.GetLength(0));
        newStream.Close();

        WebResponse response = null;
        response = request.GetResponse();
        Stream s = response.GetResponseStream();
        StreamReader sr = new StreamReader(s);
        string fd = sr.ReadToEnd();
        Console.WriteLine(fd);
        System.Diagnostics.Debug.Assert(false == fd.Contains("ault"));

        const string SIGNAL = "itemid</name><value><int>";
        string itemidstr = fd.Substring(fd.IndexOf(SIGNAL) + SIGNAL.Length);
        itemidstr = itemidstr.Substring(0, itemidstr.IndexOf("<"));
        itemid = Int32.Parse(itemidstr);

        const string SIG = "url</name><value><string>";
        string url = fd.Substring(fd.IndexOf(SIG) + SIG.Length);
        url = url.Substring(0, url.IndexOf("<"));
        return url;
    }
}





/*
if (args[0].ToUpper() == "-WATCHFOR".ToUpper())
{
    FData.SetWatchesFor(args[1]);
    return;
}

if (args[0].ToUpper() == "-RADAR".ToUpper())
{
    FData.RefreshBasedOnRadarSignals_AndPublish();
    return;
}
 * */
