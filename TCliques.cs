using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using Npgsql;
using Microsoft.Win32;
using System.Net;
using System.Threading;
using System.IO;
using System.Xml.Linq;


class TCliquesClass //  : DB
{

    //    protected static NpgsqlConnection DBConnection = null; // master sets once if not set

    /*
    protected static Dictionary<Int32, string> m_idsToNames = new Dictionary<int, string>();

    // duplciated in whole cloth from tspider, where it probably should be localized somehow.
    static protected HashSet<Int32> GetWhoIReadFromWeb(Int32 id)
    {
        bool stillRollin = true;
        HashSet<Int32> iRead = new HashSet<Int32>();
        for (Int64 iCursor = -1; stillRollin && iCursor != 0; )
        {
        HIT_AGAIN:
            string sURL = string.Format("http://twitter.com/friends/ids.xml/?user_id={0}&cursor={1}", id, iCursor);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);
            request.Method = "GET";
            request.ContentType = "application/xml";
            request.AllowWriteStreamBuffering = true;
            request.UserAgent = "FlockToo";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("mcfnord:oldpwd")));
            request.ServicePoint.Expect100Continue = false;

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException we)
            {
                if (we.ToString().Contains("403") || we.ToString().Contains("404") || we.ToString().Contains("401"))
                {
                    stillRollin = false;
                    //  iRead.Add(-1);
                    continue;
                }
                Console.WriteLine(we.ToString());
                Thread.Sleep(10 * 60000); // a whole minute ha ha . ok go ten minutes and that means i rapidly consumne my quota and then basically go offline
                goto HIT_AGAIN;
            }

            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            XDocument xdMyFriends = null;
        LOD_GIN:
            try
            {
                xdMyFriends = XDocument.Load(readStream);
            }
            catch (System.IO.IOException ix)
            {
                if (ix.ToString().Contains("Unable to read"))
                {
                    Thread.Sleep(1000);
                    goto HIT_AGAIN;
                }

            }
            catch (System.Net.WebException wx)
            {
                Console.WriteLine(wx.ToString());
                if (wx.ToString().Contains("The operation has timed out."))
                {
                    Thread.Sleep(1000);
                    goto LOD_GIN;
                }
                Debug.Assert(false); // what's up here?
            }
            catch (System.Xml.XmlException ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex.ToString().Contains("Root element is missing"))
                {
                    stillRollin = false;
                    iRead.Add(-1); // THIS ONE IS SUSPICIOUS.
                    continue;
                }
                Console.WriteLine(readStream.ReadToEnd().ToString());
                goto LOD_GIN;
            }

            readStream.Close();
            receiveStream.Close();
            response.Close();

            int iCount = xdMyFriends.Element("id_list").Element("ids").Elements("id").Count();
            Console.Write(">" + iCount + "<");
            if (iCount < 4000)
            {
                stillRollin = false;
                Debug.Assert(0 == Int64.Parse(xdMyFriends.Element("id_list").Element("next_cursor").Value)); // it better be
            }

            foreach (XElement xe in xdMyFriends.Element("id_list").Element("ids").Elements("id"))
            {
                iRead.Add(Int32.Parse(xe.Value));
            }

            iCursor = Int64.Parse(xdMyFriends.Element("id_list").Element("next_cursor").Value);
        }
        return iRead;
    }


    static protected string NameFromId(Int32 id)
    {
    // just do a healthy cache for now
    // ramcache brad save me later
    GO_MORE:
        string name = null;
        m_idsToNames.TryGetValue(id, out name);
        if (null != name)
            return name;

        // Look it up
        System.Xml.Linq.XDocument xd = GetDetailsFromWeb(id);
        if (null == xd)
            return ""; // wtf
        m_idsToNames.Add(id, xd.Element("user").Element("screen_name").Value);
        goto GO_MORE;
    }

    static protected System.Xml.Linq.XDocument GetDetailsFromWeb(Int32 id)
    {
        Debug.Assert(id != -1);

        string sURL = string.Format("http://api.twitter.com/1/users/show.xml?user_id={0}", id);
        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sURL);
        request.Method = "GET";
        request.ContentType = "application/xml";
        request.AllowWriteStreamBuffering = true;
        request.UserAgent = "FlockToo";
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("mcfnord:oldpwd")));
        request.ServicePoint.Expect100Continue = false;

    HIT_AGAIN_JERK:
        System.Net.HttpWebResponse response = null;
        try
        {
            response = (System.Net.HttpWebResponse)request.GetResponse();
        }
        catch (System.Net.WebException we)
        {
            if (we.ToString().Contains("404"))
            {
                //                    Console.WriteLine("Bad gateway drug.");
                return null;
            }

            Console.WriteLine(we.ToString());
            System.Threading.Thread.Sleep(1000);
            goto HIT_AGAIN_JERK;
        }


        System.IO.Stream receiveStream = response.GetResponseStream();
        System.IO.StreamReader readStream = new System.IO.StreamReader(receiveStream, Encoding.UTF8);
        System.Xml.Linq.XDocument xdMyDetails = System.Xml.Linq.XDocument.Load(readStream);
        return xdMyDetails;
    }

    enum TargetSetProfile
    {
        NEVER_SCRAPED,  // always active
        SCRAPED_ONCE,   // always active
        ACTIVE_NOT_OPTED_IN,
        INACTIVE_NOT_OPTED_IN,
        ACTIVE_OPTED_IN,
        INACTIVE_OPTED_IN,
    }

    static DateTime TwoK = new DateTime(2000, 1, 1);
    static Int32? m_hourIthinkItIs = null;

    static Dictionary<TargetSetProfile, string> m_SqlMatchSequence =
new Dictionary<TargetSetProfile, string> {
    {TargetSetProfile.NEVER_SCRAPED, " scrapedatleastonce = false " },
    {TargetSetProfile.SCRAPED_ONCE,  " scrapedatleastonce = true and scrapedatleasttwice = false "}, /// wtf!!!! scraped JUST once can't be found
    {TargetSetProfile.INACTIVE_NOT_OPTED_IN,  " active = false and optedin = false "}, /// wtf!!!! scraped JUST once can't be found
    {TargetSetProfile.ACTIVE_NOT_OPTED_IN,  " active = true and optedin = false "}, /// wtf!!!! scraped JUST once can't be found
                                                                  /// // unless i call the bit that.
        //and active = true and and allocated=true ;"
        };

    // IN VERY BAD FORM, THIS IS A PASTE FROM TSPIDER!!!
    static Int16 MyTargetSetID(TargetSetProfile profileRequested) // , bool allocated)
    {
        // we have to mutex somewher ein here
        //            Debug.Assert(allocated == true); // when should it be false? this all seems screwed up

        // we really do ask the db
        // and only if there's no answer do we create an answer
        // and if there is an answer, every minute we do a census
        // and to translate profile request into a set id, we need a logic pattern.

        // serialize starting here
        Mutex m = new Mutex(false, "SERIALIZE_NEWSETs");
        m.WaitOne();

        try
        {
            string sql = "select * from sets where " + m_SqlMatchSequence[profileRequested] + " and stillgrowing=true LIMIT 1;";
            Int16 iExistinSet = DB.QueryInt16OrNullIfNoResult(sql) ?? -1;
            if (iExistinSet != -1)
            {
                // exists. should we retire it?
                if (m_hourIthinkItIs == DateTime.Now.Hour)
                    return (Int16)iExistinSet;

                // the times differ. do a staleness and census check
                m_hourIthinkItIs = DateTime.Now.Hour;

                bool fRetireThisSet = false;
                // let's FIRST test if the queue is just plain stale. like, more than one day old.
                Int16? age = DB.QueryInt16OrNullIfNoResult(string.Format("select bornon from sets where setid={0} LIMIT 1;", iExistinSet));
                if (age + 1 < DateTime.Now.Subtract(TwoK).Days)
                {
                    fRetireThisSet = true;
                }
                else
                {
                    // if the census says it's full, retuire this shit
                    // but only ask every even hour
                    if (DateTime.Now.Hour % 2 == 0)
                    {
                        Console.WriteLine("Doing the pricey count(*) on setid " + iExistinSet);
                        Int64 iCensus = DB.QueryAnInt64(string.Format("select count(*) from users where setid={0};", iExistinSet));
                        if (iCensus > 200000) // i'm cutting this in half sets are too large to finish in 3 days that's too large.
                        {
                            fRetireThisSet = true;
                        }
                    }
                }

                if (false == fRetireThisSet)
                    return iExistinSet;

                // we retire this set, then fall through for creation of a new one.
                DB.ExecuteNonQuery(string.Format("update sets set stillgrowing=false, allocatedtospider=false where setid={0};", iExistinSet));
            }

            // no still-growing set of this type exists. so i create one.
            //    Int16 iTopSetID = (Int16) DB.QueryInt16OrNull("select setid from sets order by setid desc limit 1;");
            //  Debug.Assert(iTopSetID != null);

            Int16 iNewSetID = (Int16)(1 + DB.QueryInt16OrNullIfNoResult("select setid from sets order by setid desc limit 1;")); // iTopSetID + 1; // obviously we run off the end of int16 fast enough cuz we only roll upward

            // construct the insertion for this new set
            bool scrapedatleastonce = profileRequested == TargetSetProfile.NEVER_SCRAPED ? false : true;
            bool scrapedatleasttwice = true;
            switch (profileRequested)
            {
                case TargetSetProfile.NEVER_SCRAPED: scrapedatleasttwice = false; break;
                case TargetSetProfile.SCRAPED_ONCE: scrapedatleasttwice = false; break;
            }

            bool active = false;
            switch (profileRequested)
            {
                case TargetSetProfile.NEVER_SCRAPED: active = true; break;
                case TargetSetProfile.SCRAPED_ONCE: active = true; break;  // GEEZ, ONLY IF I HAVE PROOF, AND I DON'T.
                case TargetSetProfile.ACTIVE_NOT_OPTED_IN: active = true; break;
                case TargetSetProfile.ACTIVE_OPTED_IN: active = true; break;
            }
            bool optedin = false;
            switch (profileRequested)
            {
                // you cannot be opted in until you are past your second scrape
                // you are merely speculated to be active
                case TargetSetProfile.ACTIVE_OPTED_IN: optedin = true; break;
                case TargetSetProfile.INACTIVE_OPTED_IN: optedin = true; break;
            }

            sql = string.Format(
"INSERT INTO sets (setid, bornon, optedin, scrapedatleastonce, scrapedatleasttwice, active, stillgrowing, allocatedtospider) " +
"Values({0}, {1}, {2}, {3}, {4}, {5}, true, true);",
                // defaults: we are still growing (we're being born here), and we are allocated to a spider (all really use it!)
iNewSetID,
DateTime.Now.Subtract(TwoK).Days,
optedin,
scrapedatleastonce,
scrapedatleasttwice,
active);
            DB.ExecuteNonQuery(sql);
            return iNewSetID;
        }
        finally
        {
            m.ReleaseMutex();
        }
    }


    // THIS IS PLUM DUPLICATED ELSEWHERE AND SHOULD BE MIGRATED INTO A SINGLE FILE THAT DON'T CHANGE MUCH
    static protected HashSet<int> ReadArrayFromUserField(int id, string field, HashSet<int> optionalScopeLimitation = null)
    {
        HashSet<int> array = new HashSet<int>();
    READ_REALLY:
        string sQ = string.Format("select {1} from users where userid ={0} ;", id, field);
        NpgsqlCommand cmd = new NpgsqlCommand(sQ, DBConnection);
        NpgsqlDataReader myReader = cmd.ExecuteReader();
        myReader.Read();
        string x = myReader.GetDataTypeName(0);

        if (myReader.HasRows == false)
        {
            myReader.Close();

            // yeah i can totally create life here also. using the hose code.
            string birthSql = string.Format("INSERT INTO users (userid, snapshot, setid) Values({0}, NULL, {1});", id, 0);
            DB.ExecuteNonQuery(birthSql);
            // now i fucking exist. 
            goto READ_REALLY;
        }


        if (myReader.IsDBNull(0))
        {
            // no snapshot? no problem!
            HashSet<Int32> iRead = GetWhoIReadFromWeb(id);
            Debug.Assert(false == iRead.Contains(-1)); // nip it! this got hit! wtf!
            if (iRead.Count == 0)
                iRead.Add(id);
            TargetSetProfile tsp = TargetSetProfile.SCRAPED_ONCE;
            Int16 targetSet = MyTargetSetID(tsp);
            DB.WriteArrayToUserField(id, "snapshot", iRead.ToArray());
            DB.ExecuteNonQuery(string.Format("update users set setid={0} where userid={1};",
                targetSet, id));
            return iRead;
        }

        string so1 = (string)myReader.GetValue(0);
        so1 = so1.Substring(so1.IndexOf('=') + 1);
        Debug.Assert(false == so1.Contains("["));
        myReader.Close();

        string so = so1.Substring(1); // chop foirst element

        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\D"); // match non-decimal content
        try
        {
            string[] substrings = r.Split(so);
            foreach (string s in substrings)
            {
                if (s.Length > 0)
                {
                    int iss = int.Parse(s);
                    if (optionalScopeLimitation != null)
                    {
                        if (false == optionalScopeLimitation.Contains(iss))
                            continue;
                    }
                    array.Add(iss);
                }
            }
        }
        catch (OutOfMemoryException)
        {
            so = so.Replace("}", ","); // a final comma helps my parsing!
            int iPos = so.IndexOf(',');
            int iLastPos = 0;

            while (iPos != -1)
            {
                string lilNum = so.Substring(iLastPos, iPos - iLastPos);
                lilNum = lilNum.Replace(",", "");
                //            Console.WriteLine(lilNum);
                int iss = int.Parse(lilNum);
                if (optionalScopeLimitation != null)
                    if (false == optionalScopeLimitation.Contains(iss))
                        continue;

                array.Add(iss);
                iLastPos = iPos;
                if (iLastPos + 1 >= so.Length)
                {
                    iPos = -1;
                    continue;
                }
                iPos = so.IndexOf(',', iLastPos + 1) + 1;
            }
        }


        //Exception of type 'System.OutOfMemoryException' was thrown.
        // not a good sign, but i'll play ball: parse manually.
        // get the {} out of there.

        return array;
    }
     */


    static void SortByCountDesc(List<List<int>> publishedCliques)
    {
        publishedCliques.Sort(
            delegate(List<int> left, List<int> right)
            {
                /*
                if (left.Count == right.Count)
                {
                    // when the counts are the same, we try to order by similarness of groups, which entails the impossible
                    // which i implement insanely, by returning equality when they're similar, otherwise return -1 or make something else up
                    // so ask ourselves if the two contents differ by just one name.
                    int mismatches = 0 ;
                    foreach (string s in left)
                    {
                        if (false == right.Contains(s))
                            mismatches++;
                    }
                    if (mismatches == 1)
                    {
                        Console.WriteLine("THese are similar. returning zero.");
                        return 0;
                    }
                    return -1; // not sure what to return but try this.

                }
                else */
                return right.Count.CompareTo(left.Count);
            });
    }


    static List<int> TopDown(Dictionary<int, int>.ValueCollection values)
    {
        var topdown = (from theVals in values
                       orderby theVals descending
                       select theVals).Distinct();

        return new List<int>(topdown.ToArray());
    }

    static public List<List<int>>  CliquesFromBits(SortedList eachToOthers, int seed, List<int> everyone, SortedList<int, BitArray> hints)
    {
        // and if i could easily determine two-way relationships, i'm left with a derivative bitfield set that 
        // i can use to hueristically determine the cliques. like so...
        Dictionary<int, BitArray> onlyTwoWayEachToOthers = new Dictionary<int, BitArray>();
        BitArray baSeed = (BitArray)eachToOthers[seed];

        // in the interest of bullshit prevention, seed reads self.
        baSeed.Set(everyone.IndexOf(seed), true);

        foreach (int dood in eachToOthers.Keys)
        {
            // weed out doods who i don't have two-way relations with?
            if (baSeed[everyone.IndexOf(dood)])
            {
                BitArray baDoodReadsThese = (BitArray)eachToOthers[dood];

                // weed out doods who i don't have two-way relations with?
                if (baDoodReadsThese[everyone.IndexOf(seed)])
                {
                    Debug.Assert(false == onlyTwoWayEachToOthers.ContainsKey(dood));
                    onlyTwoWayEachToOthers.Add(dood, new BitArray(everyone.Count));
                    // for each bit, is that other dood's bit set 4 me?
                    for (int iPos = 0; iPos < baDoodReadsThese.Length; iPos++)
                    {
                        if (baDoodReadsThese[iPos])
                        {
                            // we read someone. dop they read us?
                            BitArray otherDoodReads = (BitArray)eachToOthers[everyone[iPos]];
                            if (otherDoodReads.Get(everyone.IndexOf(dood)))
                                onlyTwoWayEachToOthers[dood].Set(iPos, true); // Console.WriteLine("Two-way: " + dood + " " + everyone[iPos]);
                        }
                    }
                }
            }
        }
        // now we have the two-way static bitarray called onlyTwoWayEachToOthers
        // produce clique knowledge. this is static for the last frame at this point.

        // i'm going to try a queue or stack or something.
        // LOOK THROUGH TWO-WAY DATASET. ALL FUCKED UP?!

        List<List<int>> publishedCliques = new List<List<int>>();
        List<List<int>> leafyLeafs = new List<List<int>>();
        List<int> starter = new List<int>();
        starter.Add(seed);
        leafyLeafs.Add(starter);

        // if there are hints, we need to examine them and determine if they are currently accurate.
        // perhaps some names are not part of the clique hint anymore... and they must be removed.
        if (hints != null)
        {
            // this is a bit of an exercise, but the hints have a key of a name, and a bitarray of what cliques they were in.
            // so i fetch up the members of each bitfield, and re-construct them into lists. then i must confirm they're valid now.
            // so the bit value (or position) is the key, and i populate a List<string> there.
            SortedList<int, List<int>> possibleHints = new SortedList<int, List<int>>();
            foreach (int name in hints.Keys)
            {
                BitArray ba = hints[name];

                for (int iPos = 0; ; iPos++)
                {
                    try
                    {
                        if (ba.Get(iPos))
                        {
                            if (false == possibleHints.ContainsKey(iPos))
                                possibleHints.Add(iPos, new List<int>());
                            possibleHints[iPos].Add(name);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        break; // we're done!
                    }
                }
            }

            // now we tackle those possibleHints... are they still valid? or is a subset valid? and what if i derive the same subset two different ways?
            foreach (int iKey in possibleHints.Keys)
            {
                bool validHint = true;
                // for now, if it's valid now, we add it to our starter list. no subset hassle.
                foreach (int aMember in possibleHints[iKey])
                {
                    // is this member e-friends with every other member?
                    BitArray whoIRead = (BitArray)eachToOthers[aMember];

                    foreach (int otherMembers in possibleHints[iKey])
                    {
                        // i hate the reads-self question
                        if (aMember == otherMembers)
                            continue;

                        if (whoIRead[everyone.IndexOf(otherMembers)] == false)
                        {
                            validHint = false;
                            break;
                        }
                    }
                    if (false == validHint)
                        break;
                }
                if (validHint)
                    leafyLeafs.Add(possibleHints[iKey]);
            }
        }

        Console.WriteLine("");
        Console.WriteLine("#STARTING " + DateTime.Now.ToString());

        DateTime weTimeout = DateTime.Now.AddMinutes(5); // ACTUALLY WHENEVER I SENSE A CLIQUE, I GIVE 4 MORE MINUTES TOWARD TRYING...

        while (leafyLeafs.Count > 0)
        {
            //      if (_shouldStop)
            //            return;

            if (DateTime.Now > weTimeout)
            {
                Console.WriteLine("Timed out.");
                break;
            }

            // pop one off and advance it. if it doesn't advance, then it's maximally grown
            // POP OFF THE END SO I GROW MAXIMALLY FAST, SO I QUICKLY HAVE
            List<int> doinMe = leafyLeafs[leafyLeafs.Count - 1];
            leafyLeafs.RemoveAt(leafyLeafs.Count - 1);

            Dictionary<int, int> numInCommon = new Dictionary<int, int>();

            // construct its membership 
            BitArray baThisClique = new BitArray(everyone.Count, true); // and away
            foreach (int s in doinMe)
            {
                baThisClique.And(onlyTwoWayEachToOthers[s]);
            }

            // now see how this advances...
            foreach (int thatCat in onlyTwoWayEachToOthers.Keys)
            {
                // perhaps here i need to check if thatCat is anyone ALREADY IN THE CLIQUE, not just the seed.
                //        if (thatCat == args[0])
                //           continue;

                if (doinMe.Contains(thatCat))
                    continue;

                BitArray thatCatsCats = onlyTwoWayEachToOthers[thatCat];
                // i need to work with a copy, cuz .And screws the original
                BitArray baInCommon = new BitArray(thatCatsCats);
                //                        BitArray baInCommon = thatCatsCats.And(ba2WaySeed);

                baInCommon.And(baThisClique);

                int inCommon = 0;
                for (int i = 0; i < baInCommon.Count; i++)
                {
                    if (baInCommon.Get(i))
                        inCommon++;
                }
                //                        Console.WriteLine(args[0] + " and " + thatCat + " have this many e-friends in common: " + inCommon);
                numInCommon.Add(thatCat, inCommon);
            }

            List<int> topdown = TopDown(numInCommon.Values);
            /*
            var topdown = (from theVals in numInCommon.Values
                           orderby theVals descending
                           select theVals).Distinct();
            */
            // all membership at the top three levelz are considered as the next member in the clique
            // and i push on a new, "proposed" clique based on this...
            bool maximallyGrown = true;
            foreach (int joker in numInCommon.Keys)
            {
                // just try the bulky shit first.
                if (
                (numInCommon[joker] == topdown[0]) ||
                (numInCommon[joker] == topdown[1]) ||
                (numInCommon[joker] == topdown[2])
                    )
                {
                    // is the joker e-friends with all the clique members we're doin?
                    bool success = true;
                    foreach (int alreadyIn in doinMe)
                    {
                        if (false == onlyTwoWayEachToOthers[alreadyIn].Get(everyone.IndexOf(joker)))
                        {
                            success = false;
                            break;
                        }
                    }
                    if (false == success)
                        continue; // next joker please

                    List<int> aNewCliqueToTry = new List<int>(doinMe);
                    aNewCliqueToTry.Add(joker);

                    // if I only push sorted lists, i can more easily preven duplicates
                    aNewCliqueToTry.Sort();

                    // is this already in my list? look fast.
                    bool heyItsNew = true;

                    // it's probably same as previous in the queue, so examine backwards, but for now just this...
                    for (int iSlotOfLeafyLeaf = 0; iSlotOfLeafyLeaf < leafyLeafs.Count; iSlotOfLeafyLeaf++)
                    //                            foreach (List<string> existing in leafyLeafs)
                    {
                        List<int> existing = leafyLeafs[iSlotOfLeafyLeaf];
                        if (aNewCliqueToTry.Count == existing.Count)
                        {
                            bool theyDiffer = false;
                            for (int iEachDood = 0; iEachDood < aNewCliqueToTry.Count; iEachDood++)
                            {
                                if (aNewCliqueToTry[iEachDood] != existing[iEachDood])
                                {
                                    theyDiffer = true;
                                    break;
                                }
                            }
                            if (false == theyDiffer)
                            {
                                // this should get trigglered... but if not, that's curious
                                Console.WriteLine("This proposed entry is already present in the queue, at slot " + iSlotOfLeafyLeaf);

                                heyItsNew = false;
                                break;
                            }
                        }
                    }

                    if (heyItsNew)
                    {
                        maximallyGrown = false;
                        //                                Console.WriteLine("");
                        //                              foreach (string sInIt in aNewCliqueToTry)
                        //                            {
                        //                              Console.Write(sInIt + " ");
                        //                        }

                        leafyLeafs.Add(aNewCliqueToTry);
                    }
                }
            }
            if (maximallyGrown)
            {
                bool unique = true;
                // is this equal to or a subset of a set we have already published?
                // (A SUBSET SHOULD NEVER APPEAR TO BE MAXIMALLY GROWN... SO THAT'S A STOP!)
                foreach (List<int> set in publishedCliques)
                {
                    // can i get some help here?
                    if (set.Count == doinMe.Count)
                    {
                        bool aMatch = true; // proove me wrong...

                        for (int iPos = 0; iPos < set.Count; iPos++)
                        {
                            // thank heaven for alpha sort
                            if (set[iPos] != doinMe[iPos])
                            {
                                aMatch = false;
                                break;
                            }
                        }
                        if (aMatch == false)
                            continue;

                        // if it's a match, then we don't re-add it.
                        unique = false;
                        break;
                    }
                }

                if (unique)
                {
                    publishedCliques.Add(doinMe);

                    Console.Write("#PUBLISHED AT " + DateTime.Now.ToString() + " " + doinMe.Count + ": ");
                    foreach (int s in doinMe)
                    {
                        Console.Write(IDMap.IDToName(s) + " ");
                    }
                    Console.WriteLine("");

                    weTimeout = DateTime.Now.AddMinutes(1); // We will try for 1 minute
                }
            }
        }
        Console.WriteLine("#FINISHED AT " + DateTime.Now.ToString());
        Console.WriteLine("");

        SortByCountDesc(publishedCliques);

        return publishedCliques;

        /* THE FOLLOWING CODE IS VERY LOVELY BUT MOTHBALLED.
        // i love this data just as it is, skip this later work, which remains cool
        SortedList<int, BitArray> groupMemberships = null;


        // from here, every one in these top three crossover points needs consideration, recursively
        // and perhaps i will choose to go deeper,
        // but first i implement this...
        // so capture... each name-set, and the current .And result?
        if (null != publishedCliques)
        {
            // sort them.
            //                publishedSets.Sort(delegate(string s1, string s2) { return (s1 > s2) ? s1 : s2; ; });
            SortByCountDesc(publishedCliques);

            // so i clobber any clique below the 63rd
//            while (publishedCliques.Count > 64)
//                publishedCliques.RemoveAt(64); // 0 to 63 are valid.

            // now they're ordered. i want to place by top-down as size, but...
            // really all the size breakdown administers is
            // font information.
            // so i will have a bitfield for each name in everyone...
            groupMemberships = new SortedList<int, BitArray>();
            for (int iClique = 0; iClique < publishedCliques.Count; iClique++)
            {
                Debug.Assert(iClique < 64);

                foreach (int name in publishedCliques[iClique])
                {
                    // assign membership. do i know this name yet?
                    BitArray ba = null;
                    if (groupMemberships.TryGetValue(name, out ba))
                    {
                        ba.Set(iClique, true);
                    }
                    else
                    {
                        BitArray bitArrayOfCliques = new BitArray(publishedCliques.Count);

                        bitArrayOfCliques.Set(iClique, true);
                        groupMemberships.Add(name, bitArrayOfCliques);
                    }
                }
            }
        }

        // published cliques, if ever valid, are now trashed, and all we have is groupMemberships, if those.
        publishedCliques = null;

        return groupMemberships;
         * */
    }

    //         ArrayList alAllLevels = ChurnAndBurnTribes(lju, twoWayReadership, seed);


    public static List<List<int>> TCliquesMain(string seed)
    {
        MMDB.MakeSureDBIsOpen();

        int iUser = IDMap.NameToID(seed);  
        string fd = FData.GetFData(seed, true ) ; // absolutely new fdata.
        if (null == fd)
            return null ; // fuck it
        HashSet<int> whoIRead = FData.IDsInIReadFData(fd);

        if (false == whoIRead.Contains(iUser))         // i read my damn self ok
            whoIRead.Add(iUser);

        // what would i see if i scanned these for offlines?
        HashSet<int> whoIReadTrimmed = new HashSet<int>();
        foreach (var v in whoIRead)
        {
            if( false == FData.KnownOffline(IDMap.IDToName( v )))
            {
                whoIReadTrimmed.Add(v);
            }
        }

        whoIRead = whoIReadTrimmed; // original list clobbered!
        whoIReadTrimmed = null;



        List<int> everyone = new List<int>(whoIRead);

    AGAIN_WITHOUT_THE_DINKS:

        // get an everyone List... by reading everyone's arrays? add-if-unique to a list of unique keys
        // HEY AM I IN THIS LIST???
        Debug.Assert(everyone.IndexOf(iUser) != -1);
        // I WILL NEED TO ADD MYSELF TO THE EVERYONE LIST EVERY TIME OK?

        Dictionary<int, HashSet<int>> whoAllRead = new Dictionary<int, HashSet<int>>();
        whoAllRead.Add(iUser, whoIRead);

        foreach (int iUserIRead in whoIRead)
        {
            // if i read myself, i was already added, skip
            if (iUserIRead == iUser) // meee!
                continue;

            string fd2 = FData.GetFData(IDMap.IDToName(iUserIRead));
            if (fd2 == null)
                continue;

            HashSet<int> whoTheyRead = FData.IDsInIReadFData( fd2 ); //  ReadArrayFromUserField(iUserIRead, "snapshot", whoIRead);
            if (null == whoTheyRead)
                whoTheyRead = new HashSet<int> { };

            // and for every unique id, we need to store their snapshot
            // so we can make the bitfield later
            whoAllRead.Add(iUserIRead, whoTheyRead);
        }

        // it's gonna blow my mind if core user is not in everyone set.
        Debug.Assert(-1 != everyone.IndexOf(iUser));

        // i want to know every case where whoAllRead lacks a key that appears in everyone list.
        foreach (int i in everyone)
        {
            if (false == whoAllRead.ContainsKey(i))
            {
                Console.WriteLine("whoAllRead has no key: " + i); // these are offline and so no fdata is provided for them. they still could be in everyone. fix that.
            }
        }

        // ATTENTION. EVERYONE I READ IS LOADED INTO whoAllRead.
        // For each, a sifted subset (containing only people I read) is constructed.
        // For too-large groups, I will now proceed to throw out parties who only read one other party in this set.
        // Maybe then I try throwing out two. 
        // THIS INVOLVES CAREFUL WHATEVER... everyone is a slot-based scenario.
        // oh what the fucks please what the fucks ugh.
        // just cull the everyone list based on whoAllRead and GOTO up there to the whoAllRead.
        Console.WriteLine("of course i only want this for fucking biggies not for me!");
        if (whoAllRead.Count > 5000)
        {
            foreach (int iDude in whoAllRead.Keys)
            {
                HashSet<int> check = whoAllRead[iDude];

                // two? this must iterate up until it's a confirmed manageable subset size!
                if (check.Count < 2)
                    everyone.Remove(iDude);
            }
            Debug.Assert(false); //wtf is this? this is experimental two-wayer removal optimization that i never quite approved.
            goto AGAIN_WITHOUT_THE_DINKS;
        }


        // i better have a list who i read, right?
        //            Debug.Assert(null != eachToOthers[iUser]);

        // POPULATE THE BITFIELDS, which are GRIDS of two-way readership realationships
        // RITE? 
        // well i'm one of those bitfields
        // bit i think i am always in my own set of whoiRead (self)
        // i only include the set of those i read, i think.
        // DO I INCLUDE MYSELF???? i want a bitarray also!?

        // prep 
        SortedList eachToOthers = new SortedList(); // the userid yields a bitarray
        foreach (int i in everyone)
        {
            eachToOthers.Add(i, new BitArray(everyone.Count()));
        }
        // that's the fucking way to create the bit array!

        // what is the fucking way to populate it?
        // per each bit array.
        foreach (int iKey in eachToOthers.Keys)
        {
            BitArray ba = (BitArray)eachToOthers[iKey];

            // if whoALlRead[ikey] doesn't exist, emit a warning and skip it. can i fucking do that?
            // somehow there's no entry for iKey in this whoAllRead, where do eachToOthers diverge in keys from whoAllRead?

            foreach (int iSomeUser in whoAllRead[iKey]) // whoAllRead[stooge] is a way of saying "who stooge reads"
            {
                // if i have never associated with them, they don't get in.
                if (everyone.IndexOf(iSomeUser) != -1)
                    ba.Set(everyone.IndexOf(iSomeUser), true);// all sets
            }
        }

        // IS THERE A REQUIREMENT THAT I ONLY OR OPTIMALLY POPULATE TWO-WAY ONLY?

        List<List<int>> sl = CliquesFromBits(eachToOthers, iUser, everyone, null);
        // if i save this output, i can use it as hints later. could be valuable / essential. we shall see.
        // so this is a bit array of many groupings? everyone has a line in this bit array.

        return sl;
    }

    public static List<List<int>> CustomGroupMain(Int32 iSeed, HashSet<Int32> members)
    {
        MMDB.MakeSureDBIsOpen();

        if (false == members.Contains(iSeed))
        {
            Console.WriteLine("I crammed the seed back into the set.");
            members.Add(iSeed);
        }

        List<int> everyone = new List<int>(members);
        Dictionary<int, HashSet<int>> master = new Dictionary<int, HashSet<int>>();

        foreach (int iUserIRead in members)
        {
            string fd2 = FData.GetFData(IDMap.IDToName(iUserIRead));
            if (fd2 == null)
                continue;

            HashSet<int> whoTheyRead = FData.IDsInIReadFData(fd2); //  ReadArrayFromUserField(iUserIRead, "snapshot", whoIRead);
            if (null == whoTheyRead)
                whoTheyRead = new HashSet<int> { };

            // and for every unique id, we need to store their snapshot
            // so we can make the bitfield later
            master.Add(iUserIRead, whoTheyRead);
        }

        // i want to know every case where whoAllRead lacks a key that appears in everyone list.
        foreach (int i in everyone)
        {
            if (false == master.ContainsKey(i))
            {
                Console.WriteLine("whoAllRead has no key: " + i); // these are offline and so no fdata is provided for them. they still could be in everyone. fix that.
            }
        }

        // POPULATE THE BITFIELDS, which are GRIDS of two-way readership realationships
        // RITE? 
        // well i'm one of those bitfields
        // bit i think i am always in my own set of whoiRead (self)
        // i only include the set of those i read, i think.
        // DO I INCLUDE MYSELF???? i want a bitarray also!?

        // prep 
        SortedList eachToOthers = new SortedList(); // the userid yields a bitarray
        foreach (int i in everyone)
        {
            eachToOthers.Add(i, new BitArray(everyone.Count()));
        }
        // that's the fucking way to create the bit array!

        // what is the fucking way to populate it?
        // per each bit array.
        foreach (int iKey in eachToOthers.Keys)
        {
            BitArray ba = (BitArray)eachToOthers[iKey];

            // if whoALlRead[ikey] doesn't exist, emit a warning and skip it. can i fucking do that?
            // somehow there's no entry for iKey in this whoAllRead, where do eachToOthers diverge in keys from whoAllRead?

            foreach (int iSomeUser in master[iKey]) // whoAllRead[stooge] is a way of saying "who stooge reads"
            {
                // if i have never associated with them, they don't get in.
                if (everyone.IndexOf(iSomeUser) != -1)
                    ba.Set(everyone.IndexOf(iSomeUser), true);// all sets
            }
        }

        // IS THERE A REQUIREMENT THAT I ONLY OR OPTIMALLY POPULATE TWO-WAY ONLY?

        List<List<int>> sl = CliquesFromBits(eachToOthers, iSeed, everyone, null);
        // if i save this output, i can use it as hints later. could be valuable / essential. we shall see.
        // so this is a bit array of many groupings? everyone has a line in this bit array.

        return sl;

    }

}




