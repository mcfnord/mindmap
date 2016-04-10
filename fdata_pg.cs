using System.Web ;
using System.Data.SqlClient ;
using System.IO ;
using System.Threading ;
using System;
using System.Net ;
using System.Collections ;
using System.Collections.Generic ;
using Npgsql ;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;
using System.Text;


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


public class MyNpgsqlCommand 
{
NpgsqlCommand m_npgsqlCmd = null ;

	public MyNpgsqlCommand ( string strCmd, NpgsqlConnection userDBConnection )
		{
		if( strCmd[ strCmd.Length -1 ] != ';')
			strCmd += ";" ;

		int iDate  = strCmd.ToUpper().IndexOf("GETDATE()") ;
		if (iDate != -1)
			{
			strCmd = strCmd.Replace("GETDATE()", "now()") ;
//			Console.WriteLine("Watch us crash.") ;
			}
		
		m_npgsqlCmd = new NpgsqlCommand( strCmd, userDBConnection ) ;
		}

	public NpgsqlDataReader ExecuteReader() 
		{
		return m_npgsqlCmd.ExecuteReader() ;
		}

	public int ExecuteNonQuery() 
		{
		return m_npgsqlCmd.ExecuteNonQuery() ;
		}

	public int CommandTimeout
		{
		get { return m_npgsqlCmd.CommandTimeout ; }
		set { m_npgsqlCmd.CommandTimeout = value ; }
		}
	
} ;

public class MMDB
{
    private static NpgsqlConnection m_DBConnection = null;
    public static NpgsqlConnection DBConnection
    {
        get
        {
            return m_DBConnection;
        }
    }

    public static void MakeSureDBIsOpen()
    {
        if (m_DBConnection == null)
        {
            
            //		m_DBConnection = new NpgsqlConnection(  "Database=mindmap;Server=localhost;Port=5432;User Id=postgres;Password=postgres;") ; // pgsql
            m_DBConnection = new NpgsqlConnection(Registry.GetValue("HKEY_CURRENT_USER\\Software\\MindMap", "PostgreInitString", null).ToString());

            m_DBConnection.Open();
        }
    }

    public static void ExecuteNonQuery(string sql, bool showIt = true )
    {
        MakeSureDBIsOpen();
        if( showIt) 
            Console.WriteLine(sql);
        NpgsqlCommand cmd = new NpgsqlCommand(sql, DBConnection);
        cmd.ExecuteNonQuery();
    }

    public static Int64? MaybeNullInt64(NpgsqlDataReader reader, int iPosition)
    {
        if (reader.IsDBNull(iPosition))
            return null;
        return reader.GetInt64(iPosition);
    }


    public static Int32? MaybeNullInt32(NpgsqlDataReader reader, int iPosition)
    {
        if (reader.IsDBNull(iPosition))
            return null;
        return reader.GetInt32(iPosition);
    }

    public static Int16? MaybeNullInt16(NpgsqlDataReader reader, int iPosition)
    {
        if (reader.IsDBNull(iPosition))
            return null;
        return reader.GetInt16(iPosition);
    }

    public static bool QueryFindsRow(string sql)
    {
        System.Diagnostics.Debug.Assert(sql.ToUpper().Contains("LIMIT 1")); // should force optimize

        NpgsqlCommand cmd = new NpgsqlCommand(sql, DBConnection);
        NpgsqlDataReader myReader = cmd.ExecuteReader();
        myReader.Read();
        bool ret = myReader.HasRows;
        myReader.Close();
        return ret;
    }
}

public class Extras
{
    public static DateTime TwoK = new DateTime(2000, 1, 1);

    public static void CheckForRenameOrOffline(Int32 checkTarget)
    {
        // is this already an offline?
        NpgsqlDataReader myReader = new MyNpgsqlCommand(string.Format("select offline_last_detected_on from nameidmap where id={0}", checkTarget), MMDB.DBConnection).ExecuteReader();
        myReader.Read();
        Int16? offlineDetectedOn = MMDB.MaybeNullInt16(myReader, 0);
        myReader.Close();
        if (offlineDetectedOn != null)
        {
            if (offlineDetectedOn >= DateTime.Now.Subtract(TwoK).Days - 120)
                return; // i don't believe an additioan lcheck is needed
        }

        // hey this better not be an existing offline or maybe it's an update check?
        string uri = string.Format("http://www.livejournal.com/users/{0}/data/foaf", IDMap.IDToName(checkTarget));
        Console.WriteLine("Loadin: " + uri);
        XDocument document = null;
        try
        {
            LJTrafficCop.WaitMyTurn(); // And we've learned the foaf must wait twice.
            LJTrafficCop.WaitMyTurn(); // And we've learned the foaf must wait twice.
            document = XDocument.Load(uri);
        }
        catch (System.Xml.XmlException )
        {
            return;
        }
        catch (WebException we)
        {
            if ((we.ToString().Contains("404") || we.ToString().Contains("410")) || we.ToString().Contains("403"))
            {
                Console.WriteLine(IDMap.IDToName(checkTarget) + " offline.");
                new NpgsqlCommand(
                    string.Format("update nameidmap set offline_last_detected_on={0}, postsperyear=0, iaddperyear=0 where name='{1}';",
                                        DateTime.Now.Subtract(TwoK).Days,
                                        IDMap.IDToName(checkTarget)),
                    MMDB.DBConnection).ExecuteNonQuery();
            }
            else
                Console.WriteLine("JERK: " + we.ToString());
            return;
        }
        XElement personElement = document.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF").Element("{http://xmlns.com/foaf/0.1/}Person");
        if (personElement != null)
        {
            string nick = personElement.Element("{http://xmlns.com/foaf/0.1/}nick").Value;
            string name = IDMap.IDToName(checkTarget);
            if (nick != name)
            {
                Console.WriteLine(name + " >> " + nick);
                new NpgsqlCommand(
                    string.Format("update nameidmap set made_by_rename_detected_on={0}, offline_last_detected_on=null where name='{1}';", DateTime.Now.Subtract(TwoK).Days, nick),
                    MMDB.DBConnection).ExecuteNonQuery();

                new NpgsqlCommand(
                    string.Format("update nameidmap set offline_last_detected_on={0}, postsperyear=0, iaddperyear=0 where name='{1}';", DateTime.Now.Subtract(TwoK).Days, name),
                    MMDB.DBConnection).ExecuteNonQuery();
            }
        }
    }



    public static List<string> DinkyPeople
    {
        get
        {
            MMDB.MakeSureDBIsOpen();

            string strCmd = string.Format("SELECT name from ljuserextras where dinky=true");
            MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, MMDB.DBConnection);
            NpgsqlDataReader myReader = null;
            try
            {
                myReader = cmd.ExecuteReader();
                List<string> lsDinkyPeople = new List<string>() ;

                while (myReader.Read())
                    lsDinkyPeople.Add(myReader.GetString(0).Trim());

                return lsDinkyPeople;
            }
            finally
            {
                myReader.Close();
            }
        }
    }
}

public class IDMap
{

    //    private static SortedList<string, Int32> m_nameIdMap = new SortedList<string, Int32>();
    //    private static SortedList<Int32, string> m_idNameMap = new SortedList<Int32, string>();
    //   private static long m_cleanPopulateAt = 0;


    // i am ok with IDMap controlling the memcached instance, 
    protected static BeIT.MemCached.MemcachedClient cache = null;
    private static bool m_fMemcacheDInitialized = false;

    private static void InitializeMCD()
    {
        m_fMemcacheDInitialized = true;

        Console.Out.WriteLine("Setting up Memcached Client.");
        //        BeIT.MemCached.MemcachedClient.Setup("NameIdMaps", new string[] { "127.0.0.1:11211" });
        

        BeIT.MemCached.MemcachedClient.Setup("NameIdMaps", new string[] { "127.0.0.1:11211" });

        //Get the instance we just set up so we can use it. You can either store this reference yourself in
        //some field, or fetch it every time you need it, it doesn't really matter.
        cache = BeIT.MemCached.MemcachedClient.GetInstance("NameIdMaps");

        //It is also possible to set up clients in the standard config file. Check the section "beitmemcached" 
        //in the App.config file in this project and you will see that a client called "MyConfigFileCache" is defined.
        //            MemcachedClient configFileCache = MemcachedClient.GetInstance("MyConfigFileCache");

        //Change client settings to values other than the default like this:
        cache.SendReceiveTimeout = 5000;
        cache.MinPoolSize = 1;
        cache.MaxPoolSize = 5;
    }

    public static bool Set(string key, object value)
    {
        if (false == m_fMemcacheDInitialized)
            InitializeMCD();

        return cache.Set(key, value);
    }

    public static object Gets(string key, out ulong unique)
    {
        if (false == m_fMemcacheDInitialized)
            InitializeMCD();

        return cache.Gets(key, out unique);
    }

    internal static int NameToID(string name)
    {
        Debug.Assert(false == name.Contains("-"));// use underscore
        Debug.Assert(name == name.ToLower());
        Debug.Assert(name.Contains(" ") == false);

        if (false == m_fMemcacheDInitialized)
            InitializeMCD();

        // we want an id from a name.
    // the key will be # plus the ID, vs @ plus a name.
    TRY_AGAIN_JOE:
        ulong unique;
        Int32? id = Gets("@" + name.ToUpper(), out unique) as Int32?;
        if (id != null)
            return (int)id;
        else
        {   // id is null so unfound in cache.
            // if i can't find him in my db, then he needs to get created.
            string strGetEm = string.Format("SELECT id from nameidmap where name='{0}'; ", name);
            MyNpgsqlCommand cmdGetEm = new MyNpgsqlCommand(strGetEm, MMDB.DBConnection);
            NpgsqlDataReader myReader = cmdGetEm.ExecuteReader();
            myReader.Read();
            if (myReader.HasRows)
            {
                Set("@" + name.ToUpper(), myReader.GetInt32(0));
        // don't need        Set("#" + myReader.GetInt32(0).ToString(), name.ToUpper());
                myReader.Close();
                goto TRY_AGAIN_JOE;
            }
            else
            {
                myReader.Close();

                // if the id doesn't exist, we need to create it.
                MMDB.ExecuteNonQuery(string.Format("INSERT INTO nameidmap (name) Values('{0}') ", name));
                goto TRY_AGAIN_JOE; // return NameToID(name); // me so sloppy lazy strange
            }
        }
    }

//        if (id == null)
            // if the id is not known from the name, then the cache is probably brand new
            // so use the existing code to pre-populate it for screamin good throughputs
            // so long as we have the first stab and wait for these to populate,
            // assuring one party does so.

        /*
            Console.Write("Populating whole name-id cache via database...");

            string strCmdPrePop = string.Format("SELECT name, id from nameidmap order by name asc");
            MyNpgsqlCommand cmdPrePop = new MyNpgsqlCommand(strCmdPrePop, MMDB.DBConnection);
            cmdPrePop.CommandTimeout = 180; // hmmm slow db?
            NpgsqlDataReader myReaderPrePop = cmdPrePop.ExecuteReader();
            while (myReaderPrePop.Read())
            {
                Set(myReaderPrePop.GetString(0).Trim().ToUpper(), myReaderPrePop.GetInt32(1));
            }
            myReaderPrePop.Close();
            Console.WriteLine(" Done.");
        }
        m.ReleaseMutex();
        goto TRY_AGAIN_JOE;
    }
*/


        /*
        try
        {
            if (m_nameIdMap.Count == 0)
            {
                // ok we just started up. do we pre-populate?
          //      if (0 != int.Parse(Registry.GetValue("HKEY_CURRENT_USER\\Software\\MindMap", "DelayCachePrePop", null).ToString()))
                {
                    // we are in debug mode. determine the size of the big list...
                    m_cleanPopulateAt = 200; // fixed at 400 GET_NAMEIDMAP_RECORD_COUNT() / 100; // at 1%
                }
            }

            // if we are not in debug, then we will prepopulate right off the top ( 0 == 0 )
            if (m_cleanPopulateAt != -1)
            {
                if (m_nameIdMap.Count >= m_cleanPopulateAt)
                //            if (m_nameIdMap.Count == 0)
                {
                    Console.Write("Populating whole name-id cache via database...");

                    string strCmdPrePop = string.Format("SELECT name, id from nameidmap order by name asc");
                    MyNpgsqlCommand cmdPrePop = new MyNpgsqlCommand(strCmdPrePop, MMDB.DBConnection);
                    cmdPrePop.CommandTimeout = 180; // hmmm slow db?
                    NpgsqlDataReader myReaderPrePop = cmdPrePop.ExecuteReader();

                    m_nameIdMap.Clear(); // we dump it, in case we've been delay-cache functionin'.

                    while (myReaderPrePop.Read())
                    {
                        m_nameIdMap[myReaderPrePop.GetString(0)] = myReaderPrePop.GetInt32(1);
                    }
                    myReaderPrePop.Close();
                    m_cleanPopulateAt = -1; // never do this again for this run
                    Console.WriteLine(" Done.");
                }
            }

            return m_nameIdMap[name];
        }
        catch (KeyNotFoundException)
        {
            // query as usual
        }

        // if the name exists, we return it.
        // if we must create it, the database enforces uniqueness
        // so isn't there some kind of "ask" for such data?

        string strCmd = string.Format("SELECT id from nameidmap where name='{0}'", name);
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, MMDB.DBConnection);
        cmd.CommandTimeout = 240;
        NpgsqlDataReader myReader = null;
        try
        {
            myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                m_nameIdMap[name] = myReader.GetInt32(0);
                return myReader.GetInt32(0);
            }
        }
        finally
        {
            myReader.Close();
        }

        // if the id doesn't exist, we need to create it.
        strCmd = string.Format("INSERT INTO nameidmap (name) Values('{0}') ", name);
        cmd = new MyNpgsqlCommand(strCmd, MMDB.DBConnection);
        cmd.ExecuteNonQuery();
        return NameToID(name); // me so sloppy lazy strange
    }
    */

    internal static string IDToName(int id)
    {
        if (false == m_fMemcacheDInitialized)
            InitializeMCD();

    TRY_AGAIN_BOB:
        ulong unique;
        string name = Gets("#" + id.ToString(), out unique) as string;
        if (name != null)
        {
            return name.ToLower() ;
        }
        else
        {   // name is null so unfound in cache.
            // if i can't find him in my db, then how doe he exist?
            string strGetEm = string.Format("SELECT name from nameidmap where id={0} ", id);
            MyNpgsqlCommand cmdGetEm = new MyNpgsqlCommand(strGetEm, MMDB.DBConnection);
            NpgsqlDataReader myReader = cmdGetEm.ExecuteReader();
            myReader.Read();
            Debug.Assert(myReader.HasRows);
            Set("@" + myReader.GetString(0).Trim().ToUpper(), id);
            Set("#" + id.ToString(), myReader.GetString(0).Trim().ToUpper());
            myReader.Close();
            goto TRY_AGAIN_BOB;
        }
    }


        /*
        try
        {
            if (m_idNameMap.Count == 0)
            {
                string strCmdPrePop = string.Format("SELECT name, id from nameidmap order by id asc");
                MyNpgsqlCommand cmdPrePop = new MyNpgsqlCommand(strCmdPrePop, MMDB.DBConnection);
                cmdPrePop.CommandTimeout = 120;
                Console.Write("Pre-populating id-name cache...");
                NpgsqlDataReader myReaderPrePop = cmdPrePop.ExecuteReader();

                while (myReaderPrePop.Read())
                {
                    m_idNameMap[myReaderPrePop.GetInt32(1)] = myReaderPrePop.GetString(0);
                }
                myReaderPrePop.Close();
                Console.WriteLine(" Done.");
            }

            return m_idNameMap[id];
        }
        catch (KeyNotFoundException)
        {
            // query as usual
        }

        // if the item does not exist, explode, cuz it should.
        string strCmd = string.Format("SELECT name from nameidmap where id='{0}'", id);
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, MMDB.DBConnection);
        NpgsqlDataReader myReader = null;
        try
        {
            myReader = cmd.ExecuteReader();
            myReader.Read();
            m_idNameMap[id] = myReader.GetString(0);
            return myReader.GetString(0);
        }
        finally
        {
            myReader.Close();
        }
         * */
    }

    /*
    public static void FLUSH_NAMEIDMAP_TABLE(  )
    {
        MMDB.MakeSureDBIsOpen();

        MyNpgsqlCommand cmd = new MyNpgsqlCommand("DELETE FROM nameidmap", MMDB.DBConnection);
        cmd.ExecuteNonQuery();
    }
     * */

    /* slow
    public static long GET_NAMEIDMAP_RECORD_COUNT()
    {
        MMDB.MakeSureDBIsOpen();

        MyNpgsqlCommand cmd = new MyNpgsqlCommand("SELECT count(*) from nameidmap", MMDB.DBConnection);
        NpgsqlDataReader myReader = null;
        try
        {
            myReader = cmd.ExecuteReader();
            myReader.Read();
            return myReader.GetInt64(0);
        }
        finally
        {
            myReader.Close();
        }
    }
     * */


public class Incident
{
    public Incident(int theDay, string theSubj, string theObj, bool whetherAddOrDrop) 
    {
        day = theDay;
        subj = theSubj;
        obj = theObj;
        addOrDrop = whetherAddOrDrop;
    }
  
    public int day;
    public string subj;
    public string obj;
    public bool addOrDrop;
    public bool mutual = false; // false by default
} ;

public class FEvents :  MMDB
{

    /*
    public static void FLUSH_FEVENTS_TABLE()
    {
        MyNpgsqlCommand cmd = new MyNpgsqlCommand("DELETE FROM fevents", MMDB.DBConnection);
        cmd.ExecuteNonQuery();

    }
     */
    /*
    public static long GET_RECORD_COUNT()
    {
        MyNpgsqlCommand cmd = new MyNpgsqlCommand("SELECT count(*) from fevents", DBConnection);
        NpgsqlDataReader myReader = null;
        try
        {
            myReader = cmd.ExecuteReader();
            myReader.Read();
            return myReader.GetInt64(0);
        }
        finally
        {
            myReader.Close();
        }
    }
     * */

public static bool EverReading(string actor, string target)
{
    // in fevents table, did actor ever add *or just remove???* target?
    // in this implementation, just one-way question, that simple.

    // we only run if highestdayprocessed for actor is today
    // select highestdayprocessed from nameidmap where name='actor';
    Debug.Assert( GetHighestDayProcessed(actor) > -1);
    // or just that it's ever been done ha ha lazy

    // select from fevents where name=actor and target=target
    //  look up his number? can i do that
    // can't be sloppy// can i get someone's #?
    int iactor = IDMap.NameToID(actor);
    int itarget = IDMap.NameToID(target);
    string strCmd = string.Format("SELECT count(*) FROM fevents where fevents.subj={0} and fevents.obj={1}",
        iactor, itarget);

    MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
    NpgsqlDataReader myReader = null;
    try
    {
        myReader = cmd.ExecuteReader();
        while (myReader.Read())
        {
            if (myReader.GetInt64(0) > 0)
                return true ;
        }
    }
    finally
    {
        myReader.Close();
    }

    return false ;
}

public static List<string> EveryAssociateEver(string name)
{
    MakeSureDBIsOpen();

    List<string> everyone = new List<string>() ;

    /* this is the old, slow style:
    string strCmd = string.Format("select DISTINCT nameidmap.name from nameidmap, fevents WHERE " +
        "(nameidmap.id = fevents.obj OR nameidmap.id = fevents.subj) AND (fevents.obj = {0} or fevents.subj = {0});", IDMap.NameToID(name));
     * */

    string strCmd = string.Format("(select nameidmap.name from nameidmap, fevents WHERE (fevents.subj = {0}) AND " +
        "(nameidmap.id = fevents.obj)) union (select nameidmap.name from nameidmap, fevents_by_obj " +
        "WHERE (fevents_by_obj.obj = {0}) AND (nameidmap.id = fevents_by_obj.subj))", IDMap.NameToID(name)); 

    MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
    cmd.CommandTimeout = 190;
    NpgsqlDataReader myReader = null;
    try
    {
        myReader = cmd.ExecuteReader();
        while (myReader.Read())
        {
            everyone.Add(myReader.GetString(0));
        }
        return everyone;
    }
    finally
    {
        myReader.Close();
    }
}

static void SortByDayThenSeed(List<Incident> il, string seed)
{
    il.Sort(
        delegate(Incident left, Incident right)
        {
            if (left == null)
                if (right == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            else
                {
                if (right == null)
                    return 1;
                }

            // same-days show seed deeds first
            if (left.day == right.day)
            {
                if (left.subj == seed && right.subj == seed)
                    return 0;

                if (left.subj == seed)
                    return -1;

                if (right.subj == seed)
                    return 1;
            }             
            /*

                if (left.obj == seed)
                    return -1;

                if (right.obj == seed)
                    return 1;

                return 0;  // same spot
            }
             * */

            return left.day.CompareTo(right.day);
        });
}


// give me names, and i give you every event involving one of those names IN BOTH FIELDS.

public static List<Incident> GetEvents(List<string> names, string seed, List<Incident> liRadarClues)
{
    Debug.Assert(names.Count > 0);

    // if it's just one name, use the old function.
    if (names.Count == 1)
        return GetEvents(names[0]);

    // ok, i'll create a temporary table that contains the id's for all these names
    // and it needs a random table name that starts with 'temp' like 'temp123'
    // but what if it already exists? then we should crash i guess. whatever.

    // mutex time!
    Mutex m = new Mutex(false, "ONE_TEMP_TABLE_USER");
    m.WaitOne();

    string strCmd = "CREATE TABLE id_set_TEMPORARY ( id integer NOT NULL, CONSTRAINT id_key UNIQUE (id) )  WITHOUT OIDS";
    MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, MMDB.DBConnection);
    cmd.ExecuteNonQuery();

    strCmd = "ALTER TABLE id_set_TEMPORARY OWNER TO postgres";
    cmd = new MyNpgsqlCommand(strCmd, MMDB.DBConnection);
    cmd.ExecuteNonQuery();

    // now stuff it full.
    foreach (string name in names)
    {
        strCmd = string.Format("INSERT INTO id_set_TEMPORARY(id) Values({0})", IDMap.NameToID(name));
        cmd = new MyNpgsqlCommand(strCmd, MMDB.DBConnection);
        cmd.ExecuteNonQuery();
    }

Try_Again_Joker2:
    List<Incident> il = new List<Incident>();

// old style strCmd = "select distinct fevents.day, fevents.subj, fevents.obj, fevents.add_or_drop from id_set, fevents where (fevents.subj=id_set.id or fevents.obj=id_set.id) order by day asc";
    strCmd = "(select fevents.day, fevents.subj, fevents.obj, fevents.add_or_drop from id_set_TEMPORARY, fevents " +
    "where (fevents.subj=id_set_TEMPORARY.id )) UNION " +
    "(select fevents_by_obj.day, fevents_by_obj.subj, fevents_by_obj.obj, fevents_by_obj.add_or_drop from id_set_TEMPORARY, fevents_by_obj " +
    "where (fevents_by_obj.obj=id_set_TEMPORARY.id )) order by day asc";
    cmd = new MyNpgsqlCommand(strCmd, DBConnection);
    cmd.CommandTimeout = 720 * 4; // it's big baby
    NpgsqlDataReader myReader = null;
    try
    {
        try
        {
            myReader = cmd.ExecuteReader();
        }
        catch (Npgsql.NpgsqlException ne)
        {
            Console.WriteLine("Npgsql.NpgsqlException in GetEvents[] on : " + strCmd);
            Console.WriteLine(ne.ToString());
            Console.WriteLine("Gonna try again...");
            goto Try_Again_Joker2; // FAILS CUZ IT DOES THE FREAKIN FINALLY FIRST AND THAT NUKES THE TABLE I THINK.
        }

//        int iScrapsForRadar = 0;

        while (myReader.Read())
        {
            // IF BOTH FIELDS AREN'T IN THE LIST OF NAMES, WE DON'T ADD.
            string actor = IDMap.IDToName(myReader.GetInt32(1));
            string target = IDMap.IDToName(myReader.GetInt32(2));
            if (names.Contains(actor))
            {
                Incident i = new Incident(myReader.GetInt16(0),
                                            actor,
                                            target,
                                            myReader.GetBoolean(3));

                if (names.Contains(target))
                {
                    il.Add(i);
                }
                else
                {
                    // if actor is not seed, 
                    // and event occured within last 180 days,
                    // we store this incident for possible use in new radar.
                    // appraising cost first.
                    // (adds only)
                    if( liRadarClues != null)
//                        if (myReader.GetBoolean(3) == true) // we want + and -'s
                            if (actor != seed)
                                if (myReader.GetInt16(0) > DateTime.Now.AddDays(-180).Subtract(Extras.TwoK).Days)
                                    liRadarClues.Add(i);
                }
            }
        }

        SortByDayThenSeed(il, seed);

        // well i have a radar clue... let's investigate its potential:
        // see who people i read added... to know that i need to pass this back.
        // ok, test...

        return il;
    }
    finally
    {
        myReader.Close();

        strCmd = "DROP TABLE id_set_TEMPORARY ";
        cmd = new MyNpgsqlCommand(strCmd, DBConnection);
        cmd = new MyNpgsqlCommand(strCmd, MMDB.DBConnection);
        cmd.ExecuteNonQuery();

        m.ReleaseMutex();
    }
}

// the old school
public static List<Incident> GetEvents( string name )
{
Try_Again_Joker:

    List<Incident> il = new List<Incident>();

// old style string strCmd = string.Format("SELECT day, subj, obj, add_or_drop FROM fevents where subj={0} or obj={0} order by day", IDMap.NameToID(name));
    string strCmd = string.Format("(SELECT day, subj, obj, add_or_drop FROM fevents " +
    "where subj={0}) UNION " + 
    "(SELECT day, subj, obj, add_or_drop FROM fevents_by_obj where obj={0})", IDMap.NameToID(name));
    MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
    cmd.CommandTimeout = 70;  // if it suppresses a crash, i'll take it.
    NpgsqlDataReader myReader = null;
    try
    {
        try
        {
            myReader = cmd.ExecuteReader();
        }
        catch (Npgsql.NpgsqlException ne)
        {
            Console.WriteLine("Npgsql.NpgsqlException in GetEvents on " + name + "\r\n " + ne.ToString() + "\r\nTrying again...") ;
            goto Try_Again_Joker;
        }

        while (myReader.Read())
        {
            il.Add(new Incident(myReader.GetInt16(0), 
                                                IDMap.IDToName( myReader.GetInt32(1)), 
                                                IDMap.IDToName( myReader.GetInt32(2)), 
                                                myReader.GetBoolean(3)));
        }

        SortByDayThenSeed(il, name);

        /* I REFUSE TO DO THIS HERE. TOO RISKY.
        // CAN'T USE FOR-EACH WHEN IT CHAGES THE CONTENTS
        for (int iPos = 0; iPos < il.Count; iPos++)
        {
            Incident i = il[iPos];
//            Incident iConverse = new Incident(i.day, i.obj, i.subj, i.addOrDrop);
            for (int iPosOfDupe = iPos + 1; iPosOfDupe < il.Count; iPosOfDupe++)
            {
                if ((il[iPosOfDupe].day == i.day) &&
                    (il[iPosOfDupe].subj == i.obj) &&
                    (il[iPosOfDupe].obj == i.subj) &&
                    (il[iPosOfDupe].addOrDrop == i.addOrDrop))
                {
                    il.RemoveAt(iPosOfDupe);
                    i.mutual = true;
                    break;
                }
            }
        }
         * */

        return il;
    }
    finally
    {
        myReader.Close();
    }
}

public static void RemoveEvents(List<Incident> il)
{
    foreach (Incident i in il)
    {
        string strCmd = string.Format("DELETE FROM fevents where day={0} and subj={1} and obj={2} and add_or_drop={3}", 
            i.day, IDMap.NameToID(i.subj), IDMap.NameToID(i.obj), i.addOrDrop) ;
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
        cmd.ExecuteNonQuery();

        strCmd = strCmd.Replace("FROM fevents", "FROM fevents_by_obj");
        cmd = new MyNpgsqlCommand(strCmd, DBConnection);
        cmd.ExecuteNonQuery();

    }
}

public static void SetHighestDayProcessed(string name, int i)
{
    string strCmd = string.Format("update nameidmap set highestdayprocessed='{0}' where name='{1}'",
        i, name);
    MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
    cmd.ExecuteNonQuery();
}

public static int GetHighestDayProcessed(string name)
{
    // calling nametoid creates this name if it doesn't already exist
    string strCmd = string.Format("select highestdayprocessed from nameidmap where id='{0}'", IDMap.NameToID(name));
    MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
    NpgsqlDataReader myReader = null;
    try
    {
        myReader = cmd.ExecuteReader();
        myReader.Read();

        return myReader.GetInt16(0);
    }
    finally
    {
        myReader.Close();
    }
}

public static void Add(int day, string subj, string obj, bool addedOrDropped)
{
    //        Debug.Assert(subj != obj); // i can add or drop myself. totally legit i guess.
    // except you know what? it might play havoc with my otherwise-pure data model
    if (subj == obj)
        return; 

    Debug.Assert(subj == subj.ToLower());
    Debug.Assert(obj == obj.ToLower());
    Debug.Assert(subj.Contains(" ") == false);
    Debug.Assert(obj.Contains(" ") == false);

    MakeSureDBIsOpen();
    int iSubj = IDMap.NameToID(subj);
    int iObj = IDMap.NameToID(obj);

    string strCmd = string.Format("INSERT INTO fevents (day, subj, obj, add_or_drop) Values('{0}', '{1}', '{2}', '{3}')",
                            day, iSubj, iObj, addedOrDropped);
    MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
    try
    {
        cmd.ExecuteNonQuery();
    }
    catch (Npgsql.NpgsqlException e )
    {  
        // my code migh detect a duplicate row from two angles. i ignore this occurance.
        Debug.Assert( e.Code == "23505") ;
    }

    // now do the object-ordered copy
     strCmd = string.Format("INSERT INTO fevents_by_obj (day, subj, obj, add_or_drop) Values('{0}', '{1}', '{2}', '{3}')",
                        day, iSubj, iObj, addedOrDropped);
     cmd = new MyNpgsqlCommand(strCmd, DBConnection);
    try
    {
        cmd.ExecuteNonQuery();
    }
    catch (Npgsql.NpgsqlException e)
    {
        // my code migh detect a duplicate row from two angles. i ignore this occurance.
        Debug.Assert(e.Code == "23505");
    }

    // work in progress somewhere here
//        strCmd = string.Format("update FEvents set select Day,fdata from FData where Name='{0}' ORDER BY DAY DESC", seed) ;

}
}

public class FData : MMDB
{
    public static string GetCity(string seed) // also not FData but who cares?  This unencodes!
    {
        // note: i don't fetch a city if i don't already have one. i just return the default empty string.
        MakeSureDBIsOpen();

        string strCmdPrePop = string.Format("SELECT city from nameidmap where name='{0}'", seed);
        MyNpgsqlCommand cmdPrePop = new MyNpgsqlCommand(strCmdPrePop, MMDB.DBConnection);
        NpgsqlDataReader myReaderPrePop = cmdPrePop.ExecuteReader();
        try
        {
            if (myReaderPrePop.Read())
            {

                return myReaderPrePop.GetString(0);
            }
        }
        finally
        {
            myReaderPrePop.Close();
        }
        return "";
    }

    public static bool KnownOffline(string seed)
    {
        MakeSureDBIsOpen();
        string strCmdPrePop = string.Format("SELECT offline_last_detected_on from nameidmap where name='{0}';", seed);
        MyNpgsqlCommand cmdPrePop = new MyNpgsqlCommand(strCmdPrePop, MMDB.DBConnection);
        NpgsqlDataReader myReaderPrePop = cmdPrePop.ExecuteReader();
        myReaderPrePop.Read();
        bool ret = (false == myReaderPrePop.IsDBNull(0));
        myReaderPrePop.Close();
        return ret;
    }

    protected static Int16? IAddPerYear(string seed)
    {
        MakeSureDBIsOpen();
        NpgsqlDataReader reader = new MyNpgsqlCommand(string.Format("select iaddperyear from nameidmap where name='{0}'", seed), DBConnection).ExecuteReader();
        reader.Read();
        try
        {
            return MMDB.MaybeNullInt16(reader, 0);
        }
        finally
        {
            reader.Close();
        }
    }
    

    public static int PostsPerYear (string seed) // ok this is not FData, this is PData, but who cares?
    {
        MakeSureDBIsOpen();
        string strCmdPrePop = string.Format("SELECT postsperyear from nameidmap where name='{0}';", seed);
        MyNpgsqlCommand cmdPrePop = new MyNpgsqlCommand(strCmdPrePop, MMDB.DBConnection);
        NpgsqlDataReader myReaderPrePop = cmdPrePop.ExecuteReader();
        myReaderPrePop.Read();
        int ret = myReaderPrePop.GetInt16(0) ;
        myReaderPrePop.Close();
        return ret;
        /*
        try
        {
            if (null == m_namePPYMap)
            {
                MakeSureDBIsOpen();
                m_namePPYMap = new SortedList<string, Int16>();
                string strCmdPrePop = string.Format("SELECT name, postsperyear from nameidmap where postsperyear = 0 order by name asc");
                MyNpgsqlCommand cmdPrePop = new MyNpgsqlCommand(strCmdPrePop, MMDB.DBConnection);
                Console.Write("Pre-populating zero-only name-PPY cache...");
                cmdPrePop.CommandTimeout = 120; // whatever it takes for this one.
                NpgsqlDataReader myReaderPrePop = cmdPrePop.ExecuteReader();

                while (myReaderPrePop.Read())
                {
                    m_namePPYMap[myReaderPrePop.GetString(0)] = myReaderPrePop.GetInt16(1);
                }
                myReaderPrePop.Close();
                Console.WriteLine(" Done.");
            }

            return (m_namePPYMap[seed] == 0);
        }
        catch (KeyNotFoundException)
        {
            return false;
            // query as usual
        }
         * */
    }

    public static string GetFData(string seed, int iDateOffset)
    {
        if (0 == iDateOffset)
            return GetFData(seed); // does special things for "new" requests that historical grabs don't do.

        MakeSureDBIsOpen();

        if (iDateOffset > 0)
            iDateOffset = -iDateOffset;
        TimeSpan ts = DateTime.Now.AddHours(24 * iDateOffset).Subtract(Extras.TwoK);

        // Do we have a Fresh entry for this name?
        string strCmd = string.Format("select Day,fdata from FData where Name='{0}' ORDER BY DAY DESC", seed);
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);

        NpgsqlDataReader myReader = null;
        int iNearnessWinnerSoFar = 99999;

        try
        {
            myReader = cmd.ExecuteReader();

            string fd = null;

            while (myReader.Read())
            {
                int iDay = myReader.GetInt16(0);
                int iNearness = iDay - ts.Days;
                if (iNearness < 0)
                    iNearness = -iNearness;

                if (iNearness < iNearnessWinnerSoFar)
                {
                    iNearnessWinnerSoFar = iNearness;
                    fd = myReader.GetString(1).Trim();
                }
            }

            Console.Write(iNearnessWinnerSoFar + "  ");

            return FilterCommunityArtifacts(HttpUtility.UrlDecode(fd)); // null will kill this maybe but i wanna watch.
        }
        finally
        {
            myReader.Close();
        }

        //	return null ; // we should never get here.
    }

    public static bool FDataConfirmedCurrentEnufOn(string seed)
    {
        ulong unique;
        Int16? dayFDataConfirmedGoodEnuf = IDMap.Gets(">" + seed, out unique) as Int16?;
        if (dayFDataConfirmedGoodEnuf != null)
        {
            Int16? today = (Int16?) DateTime.Now.Subtract(Extras.TwoK).Days;
            if (dayFDataConfirmedGoodEnuf == today)
                return true;
        }
        return false;
    }

    protected static void PushToLJLoner(string seed)
    {
        XDocument xd = new XDocument();
        xd.Add(new XElement("methodCall",
                    new XElement("methodName", "LJ.XMLRPC.editfriends"),
                        new XElement("params",
                            new XElement("param",
                                new XElement("value",
                                    new XElement("struct",
                                        new XElement("member",
                                            new XElement("name", "username"),
                                            new XElement("value", new XElement("string", "ljloner"))),
                                        new XElement("member",
                                            new XElement("name", "password"),
                                            new XElement("value", new XElement("string", "n00dleplex"))),
                                        new XElement("member",
                                            new XElement("name", "add"),
                                            new XElement("value", 
                                                new XElement("array", 
                                                    new XElement("data", 
                                                        new XElement("value", 
                                                            new XElement("string", seed))))))))))));

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
        AGAIN_JOE:
        try
        {
            response = request.GetResponse(); // WebException unhandled ( a timeout )
        }
        catch (WebException)
        {
            Console.WriteLine("i'm pausing due to a web exception, for ten seconds.");
            Thread.Sleep(10 * 1000);
            goto AGAIN_JOE;
        }

        Stream s = response.GetResponseStream();
        StreamReader sr = new StreamReader(s);
        string fd = sr.ReadToEnd();
        Console.WriteLine(fd);
// defaultpicurl contains "ault"        Debug.Assert(false == fd.Contains("ault"));
    }


    public static string GetFData(string seed, bool fAbsolutelyCurrentPlease = false )
    {
        MakeSureDBIsOpen();

        if (FData.KnownOffline(seed))
            return null;                // updateppy will re-sample later in case this account comes back.

        if (fAbsolutelyCurrentPlease)
            Console.WriteLine("Absolutely fresh scrape for: " + seed);
        Debug.Assert(seed == seed.ToLower());

        Int16? today = (Int16?)DateTime.Now.Subtract(Extras.TwoK).Days;
        IDMap.Set(">" + seed, today); // this process ends with fdata that is "current enough" so I set the "current enough" reading to today.
        
        // Do we have a Fresh entry for this name?
        string strCmd = string.Format("select Day,fdata from FData where Name='{0}' AND Fresh ORDER BY DAY DESC limit 2", seed);
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);

        bool fFreshBitsFound = false;

        string firstFreshFD = null;
        NpgsqlDataReader myReader = null;

        try
        {
            myReader = cmd.ExecuteReader();
            if (myReader.Read())
            {
                fFreshBitsFound = true;

                int iDay = myReader.GetInt16(0);

                firstFreshFD = myReader.GetString(1).Trim();

                if (null == firstFreshFD)
                {
                    Console.WriteLine("Nooo that crash?");
                }

                // fresh bit is not fresh enough. want to make sure there is only one
                if (myReader.Read()) Console.WriteLine("\0\0\0\0\0\0\0 More than one Freshbit for a loser. " + seed);

                if (false == fAbsolutelyCurrentPlease)
                {
                    // won't hit more often than 28 days if this is a known-zero ppy, or 7 if it's a nonzero ppy. 14 if it's ppy of 1
                    int iWindowOfFreshness = -10 ;
                    switch (FData.PostsPerYear(seed))
                    {
                        case 0:
                            if (0 == FData.IAddPerYear(seed))
                                iWindowOfFreshness = -34; // our strongest case for inactive: zero ppy, zero iadd
                            else
                                iWindowOfFreshness = -28;
                            break;

                        case 1: // probably based on foaf activity date rather than post in snapshot period. hybrid.
                            iWindowOfFreshness = -14;
                            break;
                    }

                    TimeSpan ts = DateTime.Now.AddDays( iWindowOfFreshness ).Subtract(Extras.TwoK);
                    if (iDay >= ts.Days) // still fresh enough?
                    {
                        Debug.Assert(firstFreshFD != null);
                        Console.Write("+");
                        return FilterCommunityArtifacts(HttpUtility.UrlDecode(firstFreshFD));
                    }
                }
                else
                    Console.WriteLine("pause");
            }
        }
        finally
        {
            myReader.Close();
        }

        // we get to scrape and save.
        ROLL_AGAIN:
        Console.Write("!");
        string sfd = GetFDataAndFoafFromWeb(seed);

        if (sfd == null)
            return null; // sigh

        if (null != sfd)
        {
            Mutex m = new Mutex(false, "STRUGGLE_TO_SERIALIZE");
            m.WaitOne();

            HashSet<Int32> fdJustIRead_fromInternet = IDsInIReadFData(sfd);
            Int16 fdJustReadMe_count = (Int16) IDsInTheyReadMeFData(sfd).Count;  // this call assures every name appearing in any fdata is hoovered into a nameidmap entry

            // now that I know the counts, is this a possible loner?
            if( fdJustIRead_fromInternet.Count < 5 )
                if( fdJustReadMe_count < 5 )
                    if (FData.PostsPerYear(seed) > 1)
                        if (false == MMDB.QueryFindsRow(string.Format("select * from ljloner where id={0} LIMIT 1;",  IDMap.NameToID(seed))))
                        {
                            PushToLJLoner(seed); // skip me xxx
                            // string.Format("INSERT INTO nameidmap (name) Values('{0}') "
                            MMDB.ExecuteNonQuery(string.Format("INSERT INTO ljloner (id) Values({0});", IDMap.NameToID(seed)));
                        }

            if (fFreshBitsFound)
            {
                // a fresh bit was found that was not fresh enough.
                // find the oldest non-fresh...
                strCmd = string.Format("SELECT FDATA FROM FDATA WHERE NAME='{0}' AND FRESH=FALSE ORDER BY Day DESC LIMIT 1", seed);
                cmd = new MyNpgsqlCommand(strCmd, DBConnection);
                myReader = cmd.ExecuteReader();
                string newestNonFreshFD = null;
                if (myReader.Read())
                    newestNonFreshFD = myReader.GetString(0).Trim();
                myReader.Close();
                cmd = null;

                // if the fresh-bit fdata differs from the newest non-fresh bit fdata, 
                // then keep it on without the fresh bit.
                // but if it is the same data, delete the record, 
                // cuz the older one suffices, and a new one will now be created.
                string str;
                if ((null != newestNonFreshFD) && (newestNonFreshFD == firstFreshFD))
                    str = string.Format("DELETE FROM FDATA WHERE Fresh=TRUE AND Name='{0}'", seed);
                else
                    str = string.Format("UPDATE FDATA SET Fresh=FALSE where Name='{0}'", seed);

                MMDB.ExecuteNonQuery(str, false);

                // so we either just killed the fresh-bit fdata (cuz it was identical to the next-oldest fdata)
                // or we set Fresh to false on it, and will now add the new fresh data.
                // in either instance it's time to compare the previous "fresh" fdata to the current fdata,
                // and perform other tasks regarding extrapolating adds-per-year

                // the adds determination:
                // we run to LINQ asap here, by exposing the fdata to a regex and building an integer list of parties for each fdata.
                HashSet<Int32> fdJustIRead_FromDB = IDsInIReadFData(firstFreshFD);

                // now prepend to the adds array.
                var adds = fdJustIRead_fromInternet.Except(fdJustIRead_FromDB);
                if (adds.ToArray().GetLength(0) > 0)
                {
                    Int32 seedID = IDMap.NameToID(seed);

                    // for each event, put it into the adds table.
                    foreach (Int32 ia in adds)
                    {
                        MMDB.ExecuteNonQuery(string.Format("INSERT INTO adds (actor, target, daydetected) Values({0}, {1}, {2});",
                            seedID,
                            ia,
                            DateTime.Now.Subtract(Extras.TwoK).Days), false);
                    }
                }

                // drops are major clues for renames.
                var drops = fdJustIRead_FromDB.Except(fdJustIRead_fromInternet);
                // squish them all into a table
                foreach (var drop in drops)
                {
                    MMDB.ExecuteNonQuery(string.Format("INSERT INTO drops (target) Values({0});", drop), false);
                }

                if( adds.ToList().Count > 0)
                    if (drops.ToList().Count > 0)
                    {
                        int iPepPlease = 0;
                        foreach (var drop in drops)
                        {
                            if (false == FData.KnownOffline(IDMap.IDToName(drop)))
                            {
                                Extras.CheckForRenameOrOffline(drop);
                                iPepPlease++;
                                if (iPepPlease > 1)
                                    break;
                            }
                        }
                    }

                fdJustIRead_FromDB = null; // don't need it after this.
            }

            // insert the new fdata.
            strCmd = string.Format("INSERT INTO fdata (name, day, fdata, fresh) Values('{0}', '{1}', '{2}', true)",
                                            seed,
                                            today,
                                            HttpUtility.UrlEncode(sfd));

            cmd = new MyNpgsqlCommand(strCmd, DBConnection);
            try { cmd.ExecuteNonQuery(); }
            catch (System.Data.SqlClient.SqlException e)
            {
                Console.WriteLine(" due to what I hope is a unique key constraint failure, i'm skipping out, no blood no foul.");
                Console.WriteLine(e.ToString());
            }
            catch (NpgsqlException nx)
            {
                // this happens and when it does, it signals a bad scrape. so re-scrape.
                if (nx.ToString().Contains("propertrail"))
                {
                    Console.WriteLine("database add of fdata failed, gonna rescrape");
                    m.ReleaseMutex();
                    Thread.Sleep(1000);
                    goto ROLL_AGAIN;
                }
                Console.WriteLine(nx.ToString());
                Console.WriteLine(sfd);
                throw nx;
            }

            // do we have any extrapolation to do for addsperyear ?
            // addsperyear will be our manicure method.
            // so when was our last reading? ever?
            NpgsqlCommand command2 = new NpgsqlCommand(
                string.Format("select numiread_sampleday, numiread_on_sampleday, iaddperyear from nameidmap where name='{0}'", seed), DBConnection);
            NpgsqlDataReader reader2 = command2.ExecuteReader();
            reader2.Read();

            Int16? dayOfIFollowBASESample = MMDB.MaybeNullInt16(reader2, 0);
            Int16? numIFollowAtBASESample = MMDB.MaybeNullInt16(reader2, 1);
            Int16? addsperyear = MMDB.MaybeNullInt16(reader2, 2);

            reader2.Close();

            // if we've never sampled the numiread, then this is our first sample.
            if (null == dayOfIFollowBASESample)
            {
                int day = DateTime.Now.Subtract(Extras.TwoK).Days;

                MMDB.ExecuteNonQuery(string.Format("update nameidmap set numiread_sampleday={0}, numiread_on_sampleday={1}, numreadme={2} where name='{3}'",
                    day,
                    fdJustIRead_fromInternet.Count,
                    fdJustReadMe_count,
                    seed), false);
            }
            else 
            {
                // this is not our first sample. is this sample far enough from the base day for me to comfortably extrapolate iaddperyear?
                if (dayOfIFollowBASESample + 28 < DateTime.Now.Subtract(Extras.TwoK).Days) 
                {
                    Debug.Assert(dayOfIFollowBASESample != null);

                    double yearsBetweenSamples = (DateTime.Now.Subtract(Extras.TwoK).Days - (Int16)dayOfIFollowBASESample) / 365.0;
                    double iAddPerYear = (fdJustIRead_fromInternet.Count - (Int32)numIFollowAtBASESample) / yearsBetweenSamples;
                    // also preserve the non-zero signal on hose_itweetpermonth
                    if (iAddPerYear > 0.0) 
                        if (iAddPerYear < 1.0) iAddPerYear = 1.0;

                    Int32 intiAdd = (Int32)iAddPerYear;

                    MMDB.ExecuteNonQuery(string.Format("update nameidmap set iaddperyear={0} where name='{1}';",
                        intiAdd, seed), false);

                    // if our base day was more than 90 days ago we reset it to now!
                    if (dayOfIFollowBASESample + 90 < DateTime.Now.Subtract(Extras.TwoK).Days)
                    {
                        MMDB.ExecuteNonQuery(string.Format(
                               "update nameidmap set numiread_sampleday={0}, numiread_on_sampleday={1}, numreadme={2} where name='{3}';",
                               DateTime.Now.Subtract(Extras.TwoK).Days,
                               fdJustIRead_fromInternet.Count,
                               fdJustReadMe_count,
                               seed));
                    }
                }
            }

            m.ReleaseMutex();
        }

        return FilterCommunityArtifacts(sfd);
        }

    public static HashSet<Int32> IDsInTheyReadMeFData(string fd)
    {
        HashSet<Int32> li = new HashSet<Int32>();

        Regex rIRead = new Regex(@"< \w+\n");
        Match mx = rIRead.Match(HttpUtility.UrlDecode(fd));
        while (mx.Success)
        {
            li.Add(IDMap.NameToID(mx.ToString().Trim().Substring(2)));
            mx = mx.NextMatch();
        }
        return li;
    }


    // gimme some fdata and i'll tell you some id's of > 
    public static HashSet<Int32> IDsInIReadFData(string fd)
    {
        HashSet<Int32> li = new HashSet<Int32>();

        Regex rIRead = new Regex(@"> \w+\n");
        Match mx = rIRead.Match(HttpUtility.UrlDecode(fd));
        while (mx.Success)
        {
            li.Add(IDMap.NameToID(mx.ToString().Trim().Substring(2)));
            mx = mx.NextMatch();
        }
        return li;
    }

    public static string FilterCommunityArtifacts(string sourceFdata)
    {
        if (null == sourceFdata)
            return null;

        // if contains no C> or C< or P> or P<, return it unmodified.
        if (sourceFdata.Contains("C> ") || sourceFdata.Contains("C< ") ||
             sourceFdata.Contains("P> ") || sourceFdata.Contains("P< "))
        {
            // filter out community and also the P designator
            string filteredFData = "";
            bool fSuppressing = false;
            for (int iPos = 0; iPos < sourceFdata.Length; iPos++)
            {
                if (sourceFdata[iPos] == '\n')
                {
                    // we're on the \n, but look ahead, then save the \n if we aren't suppressing.
                    if (sourceFdata.Substring(iPos + 1).StartsWith("C> ") || sourceFdata.Substring(iPos + 1).StartsWith("C< "))
                        fSuppressing = true;
                    else
                    {
                        fSuppressing = false;
                        filteredFData += sourceFdata[iPos];

                        iPos++;
                    }
                }

                if (false == fSuppressing)
                {
                    filteredFData += sourceFdata[iPos];
                }
            }

            // now replace away the P stuff.
            filteredFData = filteredFData.Replace("P> ", "> ");
            filteredFData = filteredFData.Replace("P< ", "< ");
            return filteredFData;
        }
        else
            return sourceFdata;
    }


    public static string FDataBy2kDay(string seed, int day)
    {
        if (day == 1)
            return ""; // special case: everyone's day one is an empty fdata, so the next fdata is a big change.

        MakeSureDBIsOpen();

        string strCmd = string.Format("select FData from FData where Name='{0}' AND Day={1}", seed, day);
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);

        NpgsqlDataReader myReader = null;

        try
        {
            myReader = cmd.ExecuteReader();
            List<int> days = new List<int>();

            myReader.Read();

            return FilterCommunityArtifacts(HttpUtility.UrlDecode(myReader.GetString(0)));
        }
        finally
        {
            myReader.Close();
        }
    }

    public static List<int> GetFDataDates(string seed)
    {
        MakeSureDBIsOpen();

        string strCmd = string.Format("select Day from FData where Name='{0}' ORDER BY DAY ASC", seed);
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);

        NpgsqlDataReader myReader = null;

        try
        {
            myReader = cmd.ExecuteReader();
            List<int> days = new List<int>();

            while (myReader.Read())
            {
                days.Add(myReader.GetInt16(0));
            }

            // everyone has a day 1
            days.Insert(0, 1);
            return days;
        }
        finally
        {
            myReader.Close();
        }
    }

    protected static void SetSampleDayToNow(string seed)
    {
        string strCmd = string.Format("update nameidmap set sampleday={0} where name='{1}'", DateTime.Now.Subtract(Extras.TwoK).Days, seed);
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
        cmd.ExecuteNonQuery();
    }

    /*
    protected static string GetFDataAndFoafFromWeb(string seed)
    {
        return GetFDataAndFoafFromWeb(seed, false);// by default, we are in online mode.
    }
     * */

    protected static string GetFDataAndFoafFromWeb(string seed) //, bool fOffline)
    {
        Debug.Assert(false == seed.Contains("-"));// use underscore

        // We only get foaf up to every 60 days
        // First one's immediate.

        // NameToID is how we assure this entry exists in our name-id map
        // shouldn't NameToID do this itself?
        IDMap.NameToID(seed); // rather nasty, we just discard the results. we're crude.
        // i'm keeping this for the devious reason that i want 
        // everyone to have a nameidmap entry 
        // so i will roll around to ppy measurement
        // but i could optimize it further
        // just to assure the entry exists
        // cuz i don't need this...
        // fevents use it but that's fine for now.

        /*

    string strCmd = string.Format("select sampleday, numposts from nameidmap where name='{0}'", seed);
    MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
    NpgsqlDataReader myReader = cmd.ExecuteReader();
    myReader.Read() ;
    int iLastSampleDay = myReader.GetInt16(0) ;
    Int32 iPostsAtLastSample = myReader.GetInt32(1);
    myReader.Close();

    int TODAYS_DAY = DateTime.Now.Subtract(Extras.TwoK).Days ;

    // shutting off the parser.
    if( false)
        if (TODAYS_DAY - 60 > iLastSampleDay)
        {
            // get foaf and store its message
            string foafurl = string.Format("http://www.livejournal.com/users/{0}/data/foaf", seed);
        ReFoaf:
            XDocument xdFoaf = null;
            try
            {
                xdFoaf = XDocument.Load(foafurl);
            }
            catch (System.Xml.XmlException xe)
            {
                if (xe.ToString().Contains("Invalid character in the given encoding"))
                {
                    Console.WriteLine("Old DBCS or whatever in bio means blown foaf parse. le sigh.");
                    SetSampleDayToNow(seed);
                    goto NowFData; // get on to the fdata.
                }

                if (xe.ToString().Contains("unexpected token"))
                {
                    Console.WriteLine("unexpected token in foaf. whatevs.");
                    SetSampleDayToNow(seed);
                    goto NowFData; // get on to the fdata.
                }

                Console.WriteLine(xe.ToString());
                Thread.Sleep(60 * 1000); /// one minute
                /// 
                goto ReFoaf; // re-do
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.ToString());
                goto ReFoaf; // re-do

            }
            catch (WebException we)
            {
                if (we.ToString().Contains("404") ||
                    we.ToString().Contains("410") ||
                    we.ToString().Contains("403"))
                {
                    // this sample did tell us something... we want to note this date,
                    // so that we don't constantly query this crapper for another 30 days.
                    // i want to carefully check these scenarios.
                    SetSampleDayToNow(seed);
                    goto NowFData;
                }
                // try again
                Console.WriteLine(we.ToString());
                Console.WriteLine("DELAY helps nada.");
                Thread.Sleep(60 * 1000); /// one minute
                goto ReFoaf;
            }

            // if the load fails, probably cycle... but should i delay this task? if fdata's are happening, then so is foaf scrapes.
            // yes, cycle. until the data comes.

            // why am i struggling the parse this???
            XElement xee = xdFoaf.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF");
            XElement xePerson = xee.Element("{http://xmlns.com/foaf/0.1/}Person");

            if (xePerson == null)
                return null; // this happens when user accounts became community accounts and i just don't fucking care about this rare case
            // maybe later i'll sense and parse communities if that's even possible, yeah i think it is, but for now, fuck it, skip those fucks.

            string city = "";

            XElement xeCity = xePerson.Element("{http://blogs.yandex.ru/schema/foaf/}city");
            if (xeCity != null)
            {
                city = xeCity.Attribute("{http://purl.org/dc/elements/1.1/}title").Value;
                // ok, whatever
                city = city.Replace("\\", "");
            }

            XElement xeActivity = xePerson.Element("{http://blogs.yandex.ru/schema/foaf/}blogActivity");
            Int32 iPostsNow = 0;
            if (null != xeActivity)
                iPostsNow = Int32.Parse(xePerson.Element("{http://blogs.yandex.ru/schema/foaf/}blogActivity").Value);

            // we know we'll save it, but we don't know if we'll extrapolate (perhaps it's our first to save)
            if (iPostsAtLastSample > -1)
            {
                // OK, IN TRIBE, SOMETIMES WE HAVE NOT POPULATED m_namePPYMap YET...
                if (m_namePPYMap == null)
                {
                    GetPostsPerYear("mcfnord");// i'm sloppy!
                }
                // we'll extrapolate
                Int16 iPostsInPeriod = (Int16)(iPostsNow - iPostsAtLastSample);
                int iDaysInPeriod = TODAYS_DAY - iLastSampleDay;
                Int16 iPostsPerYear = (Int16)(iPostsInPeriod * (365 / iDaysInPeriod));
                strCmd = string.Format("update nameidmap set postsperyear={3}, sampleday={0}, numposts={1}, city='{4}' where name='{2}'",
                    TODAYS_DAY,
                    iPostsNow,
                    seed,
                    iPostsPerYear,
                    city);

                if (iPostsPerYear < 0)
                {
                    Console.WriteLine("Has a negative PPY: " + seed);
                }
                m_namePPYMap[seed] = iPostsPerYear;
            }
            else
            {
                // we don't have two samples so we don't have a posts-per-year
                strCmd = string.Format("update nameidmap set sampleday={0}, numposts={1}, city='{3}' where name='{2}'",
                    TODAYS_DAY,
                    iPostsNow,
                    seed,
                    city);
            }

            // save it. no extrapolation for now
            cmd = new MyNpgsqlCommand(strCmd, DBConnection);
            cmd.ExecuteNonQuery();
        }
    */

        // NowFData:
        // in offline mode, we return null here rather than do a web query. it's creepy but i'll try it.
        //   if (fOffline)
        //  return null;

        /*
        Mutex mMaster = null;
        mMaster = new Mutex(false, "OneThatDragsxx");
        mMaster.WaitOne();
         * */

        string url = string.Format("http://www.livejournal.com/misc/fdata.bml?user=" + seed);
    // when new fdata_pg is deployed, use the community scrape instead:
    // string url = string.Format("http://www.livejournal.com/misc/fdata.bml?user={0}&comm=1", seed);

        TryWebCallAgain:
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.UserAgent = "http://ljmindmap.com/; livejournal.mindmap.jrd@xoxy.net";
        WebResponse response = null;
        try
        {
            // massave concession to livejournal: sleep 400ms
            // when what matters here is synchronization.
            // a universal mutex for livejournal.com contacts.
//            Thread.Sleep(440);
            LJTrafficCop.WaitMyTurn();
            response = request.GetResponse();
            Debug.Assert(request.Address == response.ResponseUri); // doesn't signal renames unforts
        }
        catch (Exception e)
        {
            Console.WriteLine("Pausing due to web error: " + e.ToString());
            Thread.Sleep(5000);
            goto TryWebCallAgain;
        }

        Stream s = response.GetResponseStream();
        StreamReader sr = new StreamReader(s);
        string fd = "";
        try
        {
            fd = sr.ReadToEnd();
            if (fd.Contains("Bot Policy")) // this isn't fucking working and it's scary
            {
                Console.WriteLine(fd);
                Console.WriteLine("Pausing ten seconds due to throttle trigger.");
                Thread.Sleep(10 * 1000);
                goto TryWebCallAgain;
            }
            if (null == fd)
            {
                Console.WriteLine("Noooooo!");
            }


        }
        // this is one way intermittent bw availability causes crashes.
        catch (System.ArgumentOutOfRangeException)
        {
            goto TryWebCallAgain;
        }
        catch (System.IO.IOException)
        {
            goto TryWebCallAgain;
        }
        catch (System.Net.WebException)
        {
            Console.WriteLine(" Sleeping five seconds on a web exception. ");
            Thread.Sleep(5000);
            goto TryWebCallAgain;
        }

//        mMaster.ReleaseMutex();

        fd = fd.Replace("# Note: Polite data miners cache on their end.  Impolite ones get banned.", "");

        // if invalid, then return null.
        if (-1 != fd.IndexOf("invalid user"))
            return null;

        // if invalid, then return null.
        if (-1 != fd.IndexOf("not a person"))
            return null;

        if (-1 != fd.IndexOf("bogus 'user' argument"))
            return null;

        if (-1 != fd.IndexOf("user is not active"))
            return null;

        // if lengthless, then return null.
        if (fd.Length == 0)
        {
            Console.WriteLine("Nooooo");
            return null;
        }

        /*
        // i want to harvest "really short" fdata's.
        // i should probably count < and > signs somehow instead of length!!!!
        int i = 0;
        foreach (char ch in fd)
        {
            if ((ch == '>') || (ch == '<'))
                i++;
        }

        if (i < 3)
        {
            MakeSureNameExistsInLJUserExtras(seed);
            string strCmd = string.Format("update ljuserextras set dinky=true where name='{0}'", seed);
            MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, DBConnection);
            cmd.ExecuteNonQuery();
    
         }
         * */

        Debug.Assert(false == fd.Contains("Access Forbidden"));
        return fd;
    }
}



