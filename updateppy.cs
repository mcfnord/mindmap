using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Xml;
using System.Threading;
using System.IO;
using System.Net;



namespace UpdatePPY
{
    // DON'T MESS WITH THIS.
    class LJTrafficCop
    {
        private static void FireWhenReady()
        {
            try
            {
                Semaphore s = Semaphore.OpenExisting("COPS");
                Thread.Sleep(1000);
                s.Release();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString()); // eat it.
            }
        }

        public static void WaitMyTurn()
        {
            Semaphore s = new Semaphore(1, 1, "COPS");
            s.WaitOne();

            Thread newThread = new Thread(LJTrafficCop.FireWhenReady);
            newThread.Start();
        }
    }

    class UpdatePPY
    {
        // Fields
        public static NpgsqlConnection DBConnection ;
        public static DateTime TwoK = new DateTime(0x7d0, 1, 1);

        private static void Main(string[] args)
        {
            if (DateTime.Now.Year < 2012)
            {
                Console.WriteLine("FUCK YOU THE CLOCK IS WRONG.");
                return;
            }

            DBConnection = new NpgsqlConnection(
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\MindMap", "PostgreInitString", null).ToString());
   // "mindmap;Server=mmdb2.ckihnq6kjyi9.us-west-2.rds.amazonaws.com;Port=5432;User Id=postgres;Password=shannonshannon;");
            //             "Database=mindmap;Server=127.0.0.1;Port=5432;User Id=postgres;Password=pwd;");

            DBConnection.Open();

            bool fLoopin = true;
            while (fLoopin)
            {
                int today = DateTime.Now.Subtract(TwoK).Days;

                // give me 1,000 names. 
                string sQ = string.Format(
                    "select name, postsperyear, sampleday, offline_last_detected_on from nameidmap where sampleday < {0} order by sampleday asc limit 1000 ;", today - 60);
                NpgsqlCommand command = new NpgsqlCommand(sQ, DBConnection);
                command.CommandTimeout = 0;
                NpgsqlDataReader reader = command.ExecuteReader();
                List<string> list = new List<string>();
                fLoopin = false;
                while (reader.Read())
                {
                    fLoopin = true; // we have more os we loop more.
                    // that's mobile lj!
                    if (reader.GetString(0).ToUpper() == "M") continue;
                    if (reader.GetString(0).ToUpper() == "POST") continue;
                    if (reader.GetString(0).ToUpper() == "SMTP") continue;
                    if (reader.GetString(0).ToUpper() == "COM") continue;
                    if (reader.GetString(0).ToUpper() == "MAIL") continue;
                    if (reader.GetString(0).ToUpper() == "DASHBOARD") continue;
                    if (reader.GetString(0).ToUpper() == "ROADSPARKLES") continue;
                    if (reader.GetString(0).ToUpper() == "MY") continue;
                    if (reader.GetString(0).ToUpper() == "STATUS___") continue;
                    if (reader.GetString(0).ToUpper() == "FILES") continue;


                    // we know the set we're considering has not been sampled within 60 days.
                    // zero ppy is our only best indicator of inactive accout.
                    // so if the ppy is not zero, OR if it's zero but our last sample was over a year ago,
                    // then we wanna re-sample.
                    bool fAdd = false;
                    // so long as you have a nonzero ppy, you're in if we've never sensed that you're offline, or if that sense was more than a year ago
                    // ppy is -1 by default, which is non-zero.
                    if (reader.GetInt16(1) != 0 && reader.IsDBNull(3))
                        fAdd = true;

                    // you're in if i haven't heard from you in a year (or ever)
                    if (reader.GetInt16(2) < (today - 365))
                        fAdd = true;

                    if (fAdd)
                    {
                        list.Add(reader.GetString(0));
                    }
                }
                reader.Close();

                Console.WriteLine("I'd really like to foaf this many users: " + list.Count);

                foreach (string name in list)
                {
                    today = DateTime.Now.Subtract(TwoK).Days;
                    NpgsqlCommand command2 = new NpgsqlCommand(string.Format("select sampleday, numposts from nameidmap where name='{0}'", name), DBConnection);

                    TRY_DB_AGAIN:
                    NpgsqlDataReader reader2 = null;
                    try {
                        reader2 = command2.ExecuteReader();
                    }
                    catch (Exception)
                    {
                        goto TRY_DB_AGAIN;
                    }
                    reader2.Read();
                    int dayOfLastSample = reader2.GetInt16(0);
                    int numPostsAtLastSample = reader2.GetInt32(1);
                    reader2.Close();
                    string uri = string.Format("http://www.livejournal.com/users/{0}/data/foaf", name);

                    string fd = "";
                    {
                        int iErrors = 0;
                        TryWebCallAgain:
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                        request.UserAgent = "http://ljmindmap.com/; livejournal.mindmap.jrd@xoxy.net";
                        WebResponse response = null;
                        try
                        {
                            LJTrafficCop.WaitMyTurn();
                            LJTrafficCop.WaitMyTurn(); // we just have to move more slowly maybe.

                            response = request.GetResponse();
                        }
                        /*                    catch (Exception e)
                                            {
                                                Console.WriteLine("Pausing due to web error: " + e.ToString());
                                                Console.WriteLine("uri is " + uri);
                                                // implement quick, dirty exponential backoff right here.
                                                iErrors++;
                                                Console.WriteLine("Sleeping: " + iErrors);
                                                Thread.Sleep(5000 * iErrors);

                                                goto TryWebCallAgain;
                                            }
                         * */
                        catch (WebException exception3)
                        {
                            if ((exception3.ToString().Contains("404") || exception3.ToString().Contains("410")) || exception3.ToString().Contains("403"))
                            {
                                SetSampleDayToNow(name);
                                new NpgsqlCommand(
                                    string.Format("update nameidmap set offline_last_detected_on={0}, postsperyear=0, iaddperyear=0 where name='{1}';", today, name),
                                    DBConnection).ExecuteNonQuery();
                                Console.WriteLine("offline_last_detected_on today");

                                continue;
                            }
                            Console.WriteLine(uri);
                            Console.WriteLine(exception3.ToString());
                            Console.WriteLine("instead of trying again, i'm sleeping two seconds and going to the next dude.");
                            Thread.Sleep(2000); // this fires during bandwidth interruptions (and perhaps other times)
                                                //goto DO_OVER;
                            continue;
                        }

                        Stream s = response.GetResponseStream();
                        StreamReader sr = new StreamReader(s);
                        try
                        {
                            fd = sr.ReadToEnd();
                        }
                        catch (WebException exception3)
                        {
                            if ((exception3.ToString().Contains("404") || exception3.ToString().Contains("410")) || exception3.ToString().Contains("403"))
                            {
                                SetSampleDayToNow(name);
                                new NpgsqlCommand(
                                    string.Format("update nameidmap set offline_last_detected_on={0}, postsperyear=0, iaddperyear=0 where name='{1}';", today, name),
                                    DBConnection).ExecuteNonQuery();
                                Console.WriteLine("offline_last_detected_on today");

                                continue;
                            }
                            Console.WriteLine(uri);
                            Console.WriteLine(exception3.ToString());
                            Console.WriteLine("DELAY helps nada. instead of trying again, what if we skip this one?");
                            Thread.Sleep(1000);
                            //goto DO_OVER;
                            continue;
                        }

                        if (fd.Contains("Bot Policy")) // this isn't fucking working and it's scary
                        {
                            Console.WriteLine(fd);
                            Console.WriteLine("Pausing ten seconds due to throttle trigger.");
                            Thread.Sleep(10 * 1000);
                            goto TryWebCallAgain;
                        }
                    }

                    /*
                catch (XmlException exception)
                {
                    if (exception.ToString().Contains("There are multiple root elements."))       { Console.WriteLine("Some failure where the account's gone. Dunno.");                SetSampleDayToNow(name); continue; }
                    if (exception.ToString().Contains("Invalid character in the given encoding")) { Console.WriteLine("Old DBCS or whatever in bio means blown foaf parse. le sigh."); SetSampleDayToNow(name); continue; }
                    if (exception.ToString().Contains("unexpected token")) { Console.WriteLine("unexpected token in foaf. whatevs."); SetSampleDayToNow(name); continue; }
                    if (exception.ToString().Contains("Unexpected end of file has occurred. The following elements are not closed:")) { Console.WriteLine("Whack job bio caused bad xml eof whatever."); SetSampleDayToNow(name); continue; }
                    Console.WriteLine(exception.ToString()); Thread.Sleep(0xea60); goto DO_OVER;
                }
                catch (IOException exception2)
                { Console.WriteLine(exception2.ToString()); goto DO_OVER; }
                catch (WebException exception3)
                {   if ((exception3.ToString().Contains("404") || exception3.ToString().Contains("410")) || exception3.ToString().Contains("403"))
                    {
                        SetSampleDayToNow(name);
                        new NpgsqlCommand(
                            string.Format("update nameidmap set offline_last_detected_on={0}, postsperyear=0, iaddperyear=0 where name='{1}';", today, name),
                            DBConnection).ExecuteNonQuery();
                        Console.WriteLine("offline_last_detected_on today");

                        continue;
                    }
                    Console.WriteLine(uri);
                    Console.WriteLine(exception3.ToString());
                    Console.WriteLine("DELAY helps nada. instead of trying again, what if we skip this one?");
                    Thread.Sleep(1000);
                    //goto DO_OVER;
                    continue;
                }    */

                    XDocument document = null;
                    try
                    {
                        document = XDocument.Parse(fd);
                    }
                    catch (XmlException x)
                    {
                        // don't fuckin know.
                        Console.WriteLine(uri);
                        Console.WriteLine(x.ToString());
                        Console.WriteLine("instead of trying again, i'm sleeping two seconds and going to the next dude.");
                        Thread.Sleep(2000); // this fires during bandwidth interruptions (and perhaps other times)
                        continue;
                    }

                    XElement personElement = document.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF").Element("{http://xmlns.com/foaf/0.1/}Person");

                    // If we don't have a person element, just set the sample day to now so the next sample is futurey.
                    string myUpdateString;

                    if (personElement == null)
                    {
                        myUpdateString = string.Format("update nameidmap set sampleday={0} where name='{1}'", today, name); // fuck you ya time-waster, move ahead.
                    }
                    else
                    {
                        // int count = personElement.Elements("{http://xmlns.com/foaf/0.1/}knows").Count();
                        // yeah it returns data. is that data ok to use in extrapolation fields numiread_sampleday ??? compare and find out!

                        string nick = personElement.Element("{http://xmlns.com/foaf/0.1/}nick").Value;
                        if (nick != name) // rename scenario!?
                        {
                            // have we noted this rename before? WE NOTE IT ON NICK in made_by_rename_detected_on, and on NAME as killed_by_rename_detected_on
                            command = new NpgsqlCommand(string.Format("select made_by_rename_detected_on from nameidmap where name='{0}';", nick), DBConnection);
                            reader = command.ExecuteReader();
                            reader.Read();
                            if (reader.HasRows) // maybe we've never heard of 'em.
                            {
                                bool isDBNull = reader.IsDBNull(0);
                                reader.Close();
                                if (isDBNull)
                                {
                                    Console.WriteLine("Old name: " + name + " New name: " + nick);
                                    // never detected b4. note it!
                                    new NpgsqlCommand(
                                        string.Format("update nameidmap set made_by_rename_detected_on={0}, offline_last_detected_on=null where name='{1}';", today, nick),
                                        DBConnection).ExecuteNonQuery();

                                    new NpgsqlCommand(
                                        string.Format("update nameidmap set offline_last_detected_on={0}, postsperyear=0, iaddperyear=0 where name='{1}';", today, name),
                                        DBConnection).ExecuteNonQuery();

                                    // everything else is ok i suppose. so fall through and keep on
                                }
                            }
                            else
                                Console.WriteLine("Hey, this is clearly a rename so new that it's not in my nameidmap, but i don't add entries so fuck it.");
                        }

                        string city = "";
                        XElement cityNode = personElement.Element("{http://blogs.yandex.ru/schema/foaf/}city");
                        if (cityNode != null)
                        {
                            city = cityNode.Attribute("{http://purl.org/dc/elements/1.1/}title").Value.Replace(@"\", "");
                        }
                        XElement blogActivityNode = personElement.Element("{http://blogs.yandex.ru/schema/foaf/}blogActivity");
                        int numPostsAtThisSample = 0;
                        if (blogActivityNode != null)
                        {
                            XElement postsNode = blogActivityNode.Element("{http://blogs.yandex.ru/schema/foaf/}Posts");
                            XElement postedNode = postsNode.Element("{http://blogs.yandex.ru/schema/foaf/}posted");
                            //                        numPostsAtThisSample = int.Parse(personElement.Element("{http://blogs.yandex.ru/schema/foaf/}blogActivity").Value);
                            numPostsAtThisSample = int.Parse(postedNode.Value); // this is where i dug in deeper... must be tested.
                        }
                        if (numPostsAtLastSample > -1)
                        {
                            short newPostsInSamplePeriod = (short)(numPostsAtThisSample - numPostsAtLastSample);
                            int daysBetweenSamples = today - dayOfLastSample;
                            short postsperyear = (short)(newPostsInSamplePeriod * 365 / daysBetweenSamples);

                            if (postsperyear == 0)
                            {
                                // ppy is zero. i am c urious about the foaf:weblog node, in particular its dateLastUpdated. this is just experimental for now.
                                XElement webLog = personElement.Element("{http://xmlns.com/foaf/0.1/}weblog");
                                if (null != webLog)
                                {
                                    XAttribute weblogNodeUpdateAttribute = webLog.Attribute("{http://www.livejournal.org/rss/lj/1.0/}dateLastUpdated");
                                    if (null != weblogNodeUpdateAttribute)
                                    {
                                        string timeString = weblogNodeUpdateAttribute.Value;
                                        DateTime dt = DateTime.Parse(timeString);
                                        if (DateTime.Now.Subtract(dt).Days <= 365)
                                            postsperyear = 1; // we juice it up.
                                    }
                                }
                            }

                            myUpdateString = string.Format(
                                    "update nameidmap set postsperyear={3}, sampleday={0}, numposts={1}, city='{4}', offline_last_detected_on=null where name='{2}'",
                                    today, numPostsAtThisSample, name, postsperyear, city);

                            if (postsperyear != 0)
                            {
                                // if there's signs of activity, do we have an fdata for this user? if not, then we want to get it.
                                // just getting it and populating three fields--numreadme, numiread_sampleday, and numiread_on_sampleday (being polite)--is adequate
                                // but first is there any fdata for this user?
                                command = new NpgsqlCommand(string.Format("select day from fdata where name='{0}' LIMIT 1;", name), DBConnection);
                                reader = command.ExecuteReader();
                                reader.Read();
                                bool fHasFD = reader.HasRows;
                                reader.Close();
                                if (false == fHasFD)
                                {
                                    Console.WriteLine("hi");  // i have no fdata for this account. i'm totally scared to fuck this up. just pause.
                                }
                            }
                        }
                        else
                        {
                            myUpdateString = string.Format("update nameidmap set sampleday={0}, numposts={1}, city='{3}', offline_last_detected_on=null where name='{2}'",
                                    today, numPostsAtThisSample, name, city);
                        }
                    }
                    Console.WriteLine(myUpdateString);
                    new NpgsqlCommand(myUpdateString, DBConnection).ExecuteNonQuery();
                }
            }
        }

        protected static void SetSampleDayToNow(string seed)
        {
            NpgsqlCommand command = new NpgsqlCommand(string.Format("update nameidmap set sampleday={0} where name='{1}'", DateTime.Now.Subtract(TwoK).Days, seed), DBConnection);
            command.CommandTimeout = 720;
            command.ExecuteNonQuery();
        }
    }
}
