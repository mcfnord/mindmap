using System;
using System.Collections.Generic;
// using System.Linq;
// using System.Text;
using Npgsql;
// using Microsoft.Win32;
using System.Xml.Linq;
using System.Xml;
using System.Threading;
using System.IO;
//using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

// using Microsoft.ManagementConsole.Internal;


namespace UpdatePPY
{
    class UpdatePPY
    {
        // Fields
        public static NpgsqlConnection DBConnection;
        public static DateTime TwoK = new DateTime(0x7d0, 1, 1);

        static async Task RunAsync()
        {
            m_response = "";
            using (var client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                    HttpResponseMessage responsey = await client.GetAsync(m_uri);
                    responsey.EnsureSuccessStatusCode();
                    string responseBody = await responsey.Content.ReadAsStringAsync();
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);
                    // Console.WriteLine(responseBody);
                    m_response = responseBody;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                    m_response = e.Message; // we hack
                }
                // Need to call dispose on the HttpClient object
                // when done using it, so the app doesn't leak resources
                client.Dispose();
            }
        }

        static string m_uri;
        static string m_response;

        private static void Main(string[] args)
        {
            RESTART:
            try
            {
                if (DateTime.Now.Year < 2012)
                {
                    Console.WriteLine("FUCK YOU THE CLOCK IS WRONG.");
                    return;
                }

                DBConnection = new NpgsqlConnection("Database=trim_mm;Server=ec2-54-70-214-46.us-west-2.compute.amazonaws.com;Port=5432;User Id=postgres;Password=shannonshannon;");
                START_FROM_SCRATCH:

                DBConnection.Open();

                bool fLoopin = true;
                while (fLoopin)
                {
                    int today = DateTime.Now.Subtract(TwoK).Days;

                    string sQ = string.Format("select name, postsperyear, sampleday, offline_last_detected_on from nameidmap where sampleday < {0} order by sampleday asc {1} ;", today - 60, (args.GetLength(0) > 1) ? args[1] : "limit 50");
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
                        else // tiniest optimization in the world.

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

                        NpgsqlDataReader reader2 = null;
                        try
                        {
                            command2.CommandTimeout = 30;
                            reader2 = command2.ExecuteReader();
                        }
                        catch (Exception e)
                        {
                            Console.Write("!STARTING FROM SCRATCH!" + e.ToString());
                            DBConnection.Close();
                            goto START_FROM_SCRATCH;
                        }
                        reader2.Read();
                        int dayOfLastSample = reader2.GetInt16(0);
                        int numPostsAtLastSample = reader2.GetInt32(1);
                        reader2.Close();
                        string uri = string.Format("http://www.livejournal.com/users/{0}/data/foaf", name);

                        m_uri = uri;
                        RunAsync().Wait();
                        Thread.Sleep(10000); // WAIT ten SECONDS

                        if (m_response == "Response status code does not indicate success: 410 (Gone).")
                        {
                            Console.WriteLine("Ditch this guy!");
                            new NpgsqlCommand(
                            string.Format("update nameidmap set offline_last_detected_on={0}, sampleday={0}, postsperyear=0, iaddperyear=0 where name='{1}';", today, name),
                            DBConnection).ExecuteNonQuery();
                            continue;
                        }

                        XDocument document = null;
                        try
                        {
                            document = XDocument.Parse(m_response);
                        }
                        catch (XmlException x)
                        {
                            Console.WriteLine(uri);
                            Console.WriteLine(x.ToString());
                            Console.WriteLine("I've seen these xml errors. I'm gonna set sample day ahead, fail style.");
                            string myXmlFailureString = string.Format("update nameidmap set sampleday={0} where name='{1}'", today, name); // fuck you ya time-waster, move ahead.
                            new NpgsqlCommand(myXmlFailureString, DBConnection).ExecuteNonQuery();
                            continue;
                        }

                        Console.Write("FAB");

                        XElement personElement = document.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF").Element("{http://xmlns.com/foaf/0.1/}Person");

                        // If we don't have a person element, just set the sample day to now so the next sample is futurey.
                        string myUpdateString;

                        Console.Write("VVV");

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
                                try
                                {
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
                                catch (Exception) { Console.WriteLine("I don't give a fuck."); }
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
                                Console.Write("#");
                                short postsperyear = (short)(newPostsInSamplePeriod * 365 / daysBetweenSamples);
                                Console.Write("!");
                                Console.WriteLine("   {0} days between the two samples.", daysBetweenSamples);

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
                                                postsperyear = 1; // it's at least one, but we don't know more than that.
                                        }
                                    }
                                }
                                myUpdateString = string.Format(
                                        "update nameidmap set postsperyear={3}, sampleday={0}, numposts={1}, city='{4}', offline_last_detected_on=null where name='{2}'",
                                        today, numPostsAtThisSample, name, postsperyear, city);
                            }
                            else
                            {
                                myUpdateString = string.Format(
                                        "update nameidmap set sampleday={0}, numposts={1}, city='{3}', offline_last_detected_on=null where name='{2}'",
                                        today, numPostsAtThisSample, name, city);
                            }
                        }
                        Console.WriteLine(myUpdateString);
                        NpgsqlCommand cmdy = new NpgsqlCommand(myUpdateString, DBConnection);
                        cmdy.ExecuteNonQuery();

                        bool anyNew = false;
                        foreach (var knows in personElement.Elements("{http://xmlns.com/foaf/0.1/}knows"))
                        {
                            var person = knows.Element("{http://xmlns.com/foaf/0.1/}Person");
                            XElement theirname = person.Element("{http://xmlns.com/foaf/0.1/}nick");

                            try
                            {
                                command = new NpgsqlCommand(string.Format("INSERT INTO nameidmap(name) VALUES('{0}');", theirname.Value), DBConnection);
                                command.ExecuteNonQuery();
                                if(anyNew == false)
                                {
                                    anyNew = true;
                                    Console.Write("{");
                                }
                                Console.Write(" " + theirname.Value.ToString());
                            }
                            catch (Exception ex) // called different things on different platforms so instead i rely on the text of the fialure.
                            {
                                if (ex.ToString().Contains("violates unique"))
                                    continue;
                                throw ex;
                            }
                        }
                        if(anyNew)
                            Console.WriteLine("}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("For some crash, we are counting to ten, and starting again.");
                Thread.Sleep(10000);
                //                DBConnection.Close();
                DBConnection = null;
                goto RESTART;
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

