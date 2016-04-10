// "I'm really pleased with the way mine came out..." - John, mudo@vt.edu, Vermfont Technical College (get full name from PayPal)
// "I think it's a fabulous and interesting idea!" - Joelle Tjahjadi (pevra@excite.com)
// "I love your mind map so much!!!" Jess Brisch (sawdustsocks@hotmail.com)
// "I'm amazed by the fact you managed to place my boyfriend closest to me (even though he doesn't update his journal and I never told who he is), my other journal (which I kind of keep a secret), my best friend (both screen names), the girl I'm moving in with next week, my other good friend, the girl who knows one of my deepest darkest secrets...all of them in big letters and close to me. that's amazing." - Tiffany
// "this thing is really neat." - Rainmaker2439@aol.com 
// "I think you are very groovy for doing this for everyone.  I appreciate it. :)" - Tammy
// "I threw some money your way. I think the social ramifications of your linked project are interesting. People who come up with projects like this think outside the box. The way the mindmaps work is defining a whole online "culture" and then to turn it into graphics that I think are beautiful is amazing.  Thanks for sharing your project with so many people on here." - feline
// "I just thought the mindmap was just one of the coolest things I've ever seen, you did a great job!" - Kimberly Burton - ksb@ksbdesigns.com  (spacefem, but that's secret)


// using System.Data.SqlClient ;
using System.Xml.Serialization ;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D ;
//using System.Runtime.Serialization.Formatters.Soap ;
using System ;
using System.Net ;
using System.IO ; // for FileStream
using System.Text.RegularExpressions;
using System.Collections ;
using System.Web.Services ;
using System.Threading ;
using System.Diagnostics;
using System.Collections.Generic ;
using System.Net.Mail;
using Npgsql;
using System.Web ;
using System.Text ;
using Microsoft.Win32;
using System.Linq;

// using System.Data;

/*
[WebService (Description="Request Tribe Maps", Namespace="http://tribeserver.gal2k.com:81/")]
public class TribeServer : WebService
{
	public System.Threading.Thread m_workerThread ;
	
	[WebMethod(Description="Given tribe seed, returns useage information.")]
	public string Nudge(  )
	{
	if (m_workerThread == null)
		{
		m_workerThread = new System.Threading.Thread(new System.Threading.ThreadStart(myStartingMethod));
		m_workerThread.Start();
		}

	return "<center>This feature is not implemented yet.  Sure, it'll be cool when it is.  But it's not.  So ask mcfnord and he'll make you a MindMap.</center>" ;
	}

	void myStartingMethod()
	{
	*/
		// load the dataset first time in.
//		Report( "slave thread started." ) ;
//		GrabClass.LoadMasterUserList() ;

		// Read the queue.txt, and find any entries that aren't currently tribal seeds.
			
		
		/*
		for(int iCount = 0 ; iCount < 10 ; iCount++)
		{
			string UseMe = "c:\\temp\\websquirty.txt" ;

			FileInfo fi = new FileInfo(UseMe);
			if (!fi.Exists)
			{
				using (StreamWriter sw = fi.CreateText())
				{
					sw.Write(DateTime.Now.ToString()) ;
					sw.Write("   ") ;
					sw.WriteLine(iCount.ToString()) ;
				}
			}

			using (StreamWriter sw = fi.AppendText())
			{
				sw.Write(DateTime.Now.ToString()) ;
				sw.Write("   ") ;
				sw.WriteLine(iCount.ToString() ) ;
			}

		Thread.Sleep(10000) ;
		}
		*/
			
		
//		Report("Master User List loaded") ;

		// we just loop

/*
		for(;;)
			{
			// Look at the server for seed requests that are not currently calculated tribes.
			}
		
		for(int iCount = 0 ; iCount < 10 ; iCount++)
		{
			string UseMe = // HttpContext.Current.Server.MapPath(
				"c:\\temp\\websquirt.txt" ;
//				) ;

			FileInfo fi = new FileInfo(UseMe);
			if (!fi.Exists)
			{
				using (StreamWriter sw = fi.CreateText())
				{
					sw.Write(DateTime.Now.ToString()) ;
					sw.Write("   ") ;
					sw.WriteLine(iCount.ToString()) ;
				}
			}

			using (StreamWriter sw = fi.AppendText())
			{
				sw.Write(DateTime.Now.ToString()) ;
				sw.Write("   ") ;
				sw.WriteLine(iCount.ToString() ) ;
			}

		Thread.Sleep(10000) ;
		}
	*/
		
//	}
//}



[Serializable]
public class LJUser2 // : IComparable
{
	public LJUser2() { }
//	public LJUser2 ( LJUser lju ) { Name = lju.Name; Location = lju.Location; Readers = lju.Readers ; BDate = lju.BDate ; whoIRead = lju.whoIRead ; }

    private string m_name ;
    private bool fIDCurrent = false ;
    private Int32 m_id;
    public string Name
    {
        get
        {
            return m_name;
        }
        set
        {
            m_name = value;
            fIDCurrent = false;
        }
    }
    public Int32 ID
    {
        get
        {
            if (fIDCurrent)
                return m_id;
            m_id = IDMap.NameToID(m_name);
            fIDCurrent = true;
            return m_id;
        }
    }
	public string		Location ; // Now actually a Cooltip.. Could be location, could be anything.
	public string 		Url ; // a Url to their posted map, if any.
	public int              Readers ;
	public DateTime	BDate ;   // could be null ya know
//	public ArrayList	whoIRead ; // (could include self!)
//	public string          fd ; // the actual fd text.
    public HashSet<Int32> ifd
    {
        get
        {
            return internalifd;
        }
        set
        {
            internalifd = value;
            numericIsValid = false;
        }
    }
    private HashSet<Int32> internalifd;
    private bool numericIsValid = false ;
    private HashSet<Int32> whoIReadNumericInternal;
    public HashSet<Int32> whoIReadNumeric
    {
        get
        {
            if (numericIsValid)
                return whoIReadNumericInternal;
            else
            {
                whoIReadNumericInternal = new HashSet<int>();
                var justwhoiread = from dude in ifd where dude > 0 select dude;
                foreach (var dude in justwhoiread)
                    whoIReadNumericInternal.Add(dude);
                numericIsValid = true;
                return whoIReadNumericInternal;
            }
        }
    }
    /*
    public HashSet<string> whoIRead
    {
        get
        {
            var justwhoiread = from dude in ifd where dude > 0 select IDMap.IDToName( dude);
            return (HashSet<string>)justwhoiread.ToList<string>();


        }
    }
     * */

	public ArrayList      tribe ;

	// these members are used by alternative LJUser lists (but not the master list)
	public int              Tier ; // not even used within user's Tribe, because the tribe list
						// CONTAINS NO LJUser objects!
	public Rectangle     rect ; // also for sublist display and sorting.  not saved in master list.
	public int               color ; // for sublist use only.  not saved.  internal stuff.
//	public ArrayList      whoIReadNumeric ; // used by CalcSingleUser to refer to user by slot in CUserList, not name.  Faster.
	public int               distanceAway ; // used by coloring to order the consideration of items

	public LJUser2( string strName, int tier ) { Name = strName; Tier = tier ; }

	// why does this freakish ffunction exist?
	public void Clone( LJUser2 ljuToClone )
		{
		this.Name = ljuToClone.Name ;
		this.Tier = ljuToClone.Tier ;
//		this.whoIRead = ljuToClone.whoIRead ;
		this.Url = ljuToClone.Url ;
		this.Location = ljuToClone.Location ;
		this.ifd = ljuToClone.ifd ;
		// This clone is incomplete and is amended as needed.
		}

    /*
    public bool ReadsByID(Int32 id)
    {
        return ifd.Contains(id);
    }
     * */

    public bool Reads(LJUser2 lju)
    {
        return ifd.Contains(lju.ID);
    }

	public bool Reads( string strName )
	{
        return ifd.Contains( IDMap.NameToID(strName) );
    }

//		if (-1 != this.fd.IndexOf("> " + strName + "\n"))
//			return true ;
//		return false ;
		/*
//		if (this.Name == "" || strName == "")
//			return true ; // we read everyone!  We are lattice buildwerks.
			
		if (whoIRead == null)
			whoIRead = new ArrayList() ;
		
		foreach( string strIRead in whoIRead)
			{
			if (strIRead.ToUpper() == strName.ToUpper())
				return true ;
			}
		return false ;*/
		


	// IComparable sorts by # of NUMERIC readers, which is not always valid, and is a subset of readership.
	// see .Sort useage for clues why.
    /*
	public int CompareTo(object obj)
	{
		if (obj is LJUser2)
			{
			LJUser2 ljuThem = (LJUser2) obj ;
			if (GrabClass.SB_DISTANCE == GrabClass.m_iSortBy)
				return ljuThem.distanceAway.CompareTo( this.distanceAway ) ;
//				return this.distanceAway.CompareTo( ljuThem.distanceAway ) ;

			if (GrabClass.SB_READERS == GrabClass.m_iSortBy) 
				return ljuThem.whoIReadNumeric.Count.CompareTo( this.whoIReadNumeric.Count ) ;
			
//			return fl.m_iShips.CompareTo(this.m_iShips) ;
			}
		throw new ArgumentException("Object is not a CFleet.") ;
	}
     * */
	
}


// this SHOULD be inherited from terse.cs somehow, so the online version is guaranteed match.
public class TerseLJUser
{
	public string Name ;
	public ArrayList TribesBySeedSlot ; // integers of which seed user slots in the TerseLJUser array I appear in.  ought to be at least one, dontcha think.

	public TerseLJUser() { } 

	public TerseLJUser( string name ) { Name = name ; }
}

[Serializable]
public class CUserList : HashSet<LJUser2> // positions cannot be valid : List<LJUser2>
{
	public LJUser2 GetUser( string strName )
		{
		foreach( LJUser2 lju in this)
			if (lju.Name.ToUpper() == strName.ToUpper())
				return lju ;
		return null ;
		}

    public LJUser2 GetUserByID(int id)
    {
//        return from lju in this where lju.Name.ToUpper() == IDMap.IDToName(id) select lju;
        foreach (LJUser2 lju in this)
            if( lju.Name.ToUpper() == IDMap.IDToName( id ).ToUpper())
                return lju;
        return null;
    }

    /*

	public int? GetUserNumber( string strName )
		{
		LJUser2 lju = this.GetUser( strName ) ;
		if (lju == null)
			{
                return null; // we're letting this ride 2010!
//			throw new Exception() ; // I want to freaking stop the bus. 
//			return -1 ;
			}
		
		return this.IndexOf( lju ) ;
		
		}
     * */
}

class TribeList : IComparable
{
public ArrayList m_tribeMembers ;
public string m_seedUserName ;


public TribeList( ArrayList alTribeMembers, string seedUserName )
{
	m_tribeMembers = alTribeMembers ;
	m_seedUserName = seedUserName ;
}

public int CompareTo(object obj)
{
	if (obj is TribeList)
		{
		TribeList tl = (TribeList) obj ;
		// THIS IS IMPLEMENTED BASS-ACKWARDS BECAUSE I WANT
		// HIGH-TO-LOW SORTING!
		return tl.m_tribeMembers.Count.CompareTo(this.m_tribeMembers.Count) ;
		}
	throw new ArgumentException("Object is not a TribeList.") ;
}
	
}


public class MasterDB
{
	static NpgsqlConnection userDBConnection = null ;
	static CUserList localUserListCache = new CUserList() ;
	public static bool m_fNoDBMode = false ;
	public static int m_fdDayOffset = 0 ; // I can tell fdata archive to pretend it's x days into the past!

	public static void ClobberInternalCache()
		{
		localUserListCache = new CUserList() ; Console.WriteLine("Clobbered internal cache!") ;
		}

	

	// For cases where you want to party all over the database, as with seedmap's needs,
	// you can grab the underlying db.
	static public NpgsqlConnection GetDBConnection()
		{
		if ( userDBConnection == null)
			Init() ;
		return userDBConnection ;
		}

	static public void Init() 
		{

//		userDBConnection = new NpgsqlConnection(  "Database=mindmap;Server=bonkers;Port=5432;User Id=postgres;Password=postgres;") ; // pgsql
            userDBConnection = new NpgsqlConnection(Registry.GetValue("HKEY_CURRENT_USER\\Software\\MindMap", "PostgreInitString", null).ToString());

//		userDBConnection = new SqlConnection(  "initial catalog=mindmap;data source=perky;Integrated Security=SSPI" ) ;
		userDBConnection.Open() ;
		}

	static public void Close() 
		{
		userDBConnection.Close() ;
		}

/* this is the GetUser from tribe4
	static public LJUser2 GetUser( string requestedUser )
	{

	// i've repeated this trick throughout my code, but i do it again here:
	// i maintain a local userlist.  if i can't find the user you want there,
	// then i load it.  it's a typical optimization.
	LJUser2 ljuGot = localUserListCache.GetUser( requestedUser ) ;
	if (null != ljuGot)
		return ljuGot ;

	// I'm going to create a table entry for every user.
	// Load the dataset as usual.

	// first populate the LJUsers table.
	string strCmd = string.Format("select Name, Location from LJUsers where Name='{0}'", requestedUser) ;
	SqlCommand cmd = new SqlCommand(strCmd, userDBConnection) ;
	SqlDataReader myReader = cmd.ExecuteReader() ;

	myReader.Read ( ) ;

	LJUser2 lju = new LJUser2() ;

	try
		{
	lju.Name = myReader.GetString(0).Trim() ;
	lju.Location = myReader.GetString(1).Trim() ;
		}
	catch( Exception )
		{
		myReader.Close() ;
		return null ;
		}
	
	myReader.Close() ;

	// we have to poplate this user's reader list.
	strCmd = string.Format("select UserIRead from WhoIRead where Name='{0}'", requestedUser) ;
	cmd = new SqlCommand(strCmd, userDBConnection) ;
	myReader = cmd.ExecuteReader() ;

	lju.whoIRead = null ;
	while( myReader.Read ( ) )
		{
		if (lju.whoIRead == null)
			lju.whoIRead = new ArrayList() ;

		// in the database, we have all kinds of messes, including duplicate entries.
		string strReader = myReader.GetString( 0 ).Trim() ;

		bool fAlready = false ;
		foreach( string strAlready in lju.whoIRead )
			{
			if (strAlready.ToUpper() == strReader.ToUpper())
				{
				fAlready = true ;
				break ;
				}
			}

		if (false == fAlready)
			lju.whoIRead.Add( strReader ) ;
		}

	myReader.Close() ;
	

	// and if there's a tribe, we add that.  This is where the data will differ from the original.
	// but we'll do our best.  tiers... tears.


	int iTier = 0 ;
	bool fMoreTiers = true ;
	lju.tribe = null ;

	while( fMoreTiers )
		{
		ArrayList alThisTier = new ArrayList() ;
		
		strCmd = string.Format("select Member from Tribes where Name='{0}' and Tier='{1}'", requestedUser, iTier) ;
		cmd = new SqlCommand(strCmd, userDBConnection) ;
		myReader = cmd.ExecuteReader() ;
	
		while( myReader.Read() )
			{
			if (lju.tribe == null)
				lju.tribe = new ArrayList() ;
			alThisTier.Add( myReader.GetString( 0 ).Trim()) ;
			}

		if (alThisTier.Count == 0)
			{
			myReader.Close() ;
			localUserListCache.Add( lju ) ;
			return lju ;
			}

		// FOR HISTORICAL REASONS, we add a layer of indirection:
		ArrayList alNewLayer = new ArrayList() ;
		alNewLayer.Add( alThisTier ) ;
		lju.tribe.Add( alNewLayer ) ;
		iTier++ ;
		myReader.Close() ;
		}
	
	localUserListCache.Add( lju ) ;
	return lju ;
	
	}
	*/

/*
	static public void AddUrl( string name, string strUrl )
	{
		GrabClass.ChokeOnBlankUrl( strUrl ) ;
		string str = string.Format("UPDATE LJUSEREXTRAS SET URL='{1}' where Name='{0}'", name, strUrl) ;
		SqlCommand cmd = new SqlCommand(str, userDBConnection) ;
		cmd.ExecuteNonQuery() ;
	} */

    /*
	static public void AddCooltip( string name, string strCooltip )
	{
		// Do it right.  Query.  If it exists, update.
		string strCmd = string.Format("select count(*) from LJUserExtras where Name='{0}'", name) ;
		MyNpgsqlCommand cmd = new MyNpgsqlCommand( strCmd, userDBConnection ) ;

		NpgsqlDataReader myReader = cmd.ExecuteReader() ;
		myReader.Read ( ) ;

		if (null != strCooltip)
			strCooltip = strCooltip.Replace("'", "''") ;

		strCooltip = HttpUtility.UrlEncodeUnicode( strCooltip ) ;
			
		if (0 == myReader.GetInt64(0))
			{
			myReader.Close() ;
			strCmd = string.Format("INSERT INTO LJUserExtras (Name, Cooltip) Values('{0}', '{1}')", name, strCooltip) ;
			cmd = new MyNpgsqlCommand(strCmd, userDBConnection) ;
			cmd.ExecuteNonQuery() ;
			}
		else
			{
			myReader.Close() ;
			strCmd = string.Format("UPDATE LJUserExtras SET Cooltip='{1}' WHERE Name='{0}'", name, strCooltip) ;
			cmd = new MyNpgsqlCommand(strCmd, userDBConnection) ;
			cmd.ExecuteNonQuery() ; // died once here as deadlock victim.
			}
	}
     * */

	static public void ClobberUrl( string seed )
	{
		string str = string.Format("UPDATE LJUSEREXTRAS SET URL=null where Name='{0}'", seed) ;
		MyNpgsqlCommand cmd = new MyNpgsqlCommand(str, userDBConnection) ;
		cmd.ExecuteNonQuery() ;
	}

    /*
	static public void NukeUser( string seed )
	{	
		// this is for the phantom bug.  not sure how far this will need to go.
		int iNuke = (int) localUserListCache.GetUserNumber( seed ) ;
		localUserListCache.RemoveAt( iNuke ) ;

		string str = string.Format("DELETE FROM TRIBES where Name='{0}'", seed) ;
		MyNpgsqlCommand cmd = new MyNpgsqlCommand(str, userDBConnection) ;
		cmd.ExecuteNonQuery() ;
	}
     * */
    
    /*
	static public bool IsMoneyBit( string seed)
	{
	string strCmd = "select Money from LJUserExtras WHERE NAME='" + seed + "'" ;
	MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, userDBConnection) ;
	NpgsqlDataReader myReader = cmd.ExecuteReader() ;

	myReader.Read ( ) ;

	try
		{
		try
			{
		if (myReader.IsDBNull(0))
			return false ;
			}
		catch(InvalidOperationException)
			{
			return false ;
			}
			

		if (true == myReader.GetBoolean(0))
			return true ;

		return false ;
		}
	finally
		{
		myReader.Close() ;
		}
	}
     * */

	static public LJUser2 GetSlimUser( string user)
		{
		return GetSlimUser( user, false ) ;
		}

    static public LJUser2 GetSlimUser(string user, bool fCooltip)
    {

        // we have to poplate this user's reader list.
        // using a property, we short-circuit the parameter stack
        // and provide the day offset if needed
        string fd = FData.GetFData(user, m_fdDayOffset); // is this the ONLY entrance into fdata.cs in the calc process?

        if (null == fd)
            return null;

        HashSet<Int32> ifd = FData.IDsInIReadFData(fd);
        var oppoSet = from dude in FData.IDsInTheyReadMeFData(fd) select -dude;
        ifd.UnionWith(oppoSet);
        fd = null;

        LJUser2 lju = new LJUser2();
        lju.Name = user;
        lju.ifd = ifd;

        /*
        lju.whoIRead = null;
        Regex rIRead = new Regex(@"> \w+\n");
        Match m = rIRead.Match(fd);

        if (lju.whoIRead == null)
            lju.whoIRead = new ArrayList();

        while (m.Success)
        {
            string who = m.ToString().Trim().Substring(2);
            lju.whoIRead.Add(who);
            m = m.NextMatch();
        }

        if (fCooltip)
        {
            string strCmd = string.Format("select count(*) from LJUserExtras where Name='{0}'", user);
            MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, userDBConnection);
            NpgsqlDataReader myReader = cmd.ExecuteReader();
            myReader.Read(); // deadlock victim. did not cause cascading failure.
            if (0 < myReader.GetInt64(0))
            {
                myReader.Close();

                strCmd = string.Format("select Cooltip from LJUserExtras where Name='{0}'", user);
                cmd = new MyNpgsqlCommand(strCmd, userDBConnection);
                myReader = cmd.ExecuteReader();
                myReader.Read();
                if (false == myReader.IsDBNull(0))
                    //					lju.Location = myReader.GetString(0).Trim() ;
                    lju.Location = HttpUtility.UrlDecode(myReader.GetString(0).Trim());
            }
            myReader.Close();
        }
         * */

        return lju;
    }


/*
	// THIS IS tribe5's GetUser.  
	static public LJUser2 GetUser( string requestedUser )
	{

	// i've repeated this trick throughout my code, but i do it again here:
	// i maintain a local userlist.  if i can't find the user you want there,
	// then i load it.  it's a typical optimization.
	LJUser2 ljuGot = localUserListCache.GetUser( requestedUser ) ;
	if (null != ljuGot)
		return ljuGot ;

	// first populate the LJUsers table.
	string strCmd = string.Format("select A.Name, B.Cooltip, B.Url, A.Readers from LJUsers A LEFT OUTER JOIN LJUserExtras B ON A.Name=B.Name where A.Name='{0}'", requestedUser) ; // CRASH!
	SqlCommand cmd = new SqlCommand(strCmd, userDBConnection) ;
	SqlDataReader myReader = cmd.ExecuteReader() ;

	myReader.Read ( ) ; // 

	LJUser2 lju = new LJUser2() ;

	try
		{
		lju.Name = myReader.GetString(0).Trim() ;
		
		if (false == myReader.IsDBNull(1))
			{
			lju.Location = myReader.GetString(1).Trim() ;
			}

		if(false == myReader.IsDBNull(2))
			{
			lju.Url = myReader.GetString(2).Trim() ;
			}

		lju.Readers = myReader.GetInt32(3) ;

		}
	catch( Exception )
		{
		return null ;
		}
	finally
		{
	myReader.Close() ;
		}

	// we have to poplate this user's reader list.
	string fd = FData.GetFData( requestedUser ) ;

	lju.whoIRead = null ;
	Regex rIRead = new Regex(@"> \w+\n") ;
	Match m = rIRead.Match( fd );
	while (m.Success) 
	{
		if (lju.whoIRead == null )
			lju.whoIRead = new ArrayList() ;
		
		string who = m.ToString().Trim().Substring(2) ;
		lju.whoIRead.Add( who ) ;
		m = m.NextMatch();
	}

	// and if there's a tribe, we add that.  This is where the data will differ from the original.
	// but we'll do our best.  tiers... tears.

	// FIRST we need to know the FULL RANGE OF TIERS.

	strCmd = string.Format("select Tier from Tribes where Name='{0}'", requestedUser) ;
	cmd = new SqlCommand(strCmd, userDBConnection) ;
	myReader = cmd.ExecuteReader() ;

	int iTops = 0 ;

	bool fSomething = false ;
	while( myReader.Read() )
		{
		fSomething = true ;
		int iThisOne =myReader.GetInt32( 0 ) ;
		if (iThisOne > iTops)
			iTops = iThisOne ;
		}
	myReader.Close() ;

//	int iTier = 0 ;
//	bool fMoreTiers = true ;
	lju.tribe = null ;

	if (fSomething)
		{
//		lju.tribe = new ArrayList() ; // null ;

		for(int iTier = 0 ; iTier <= iTops; iTier++)
			{
			ArrayList alThisTier = new ArrayList() ;

			strCmd = string.Format("select Member from Tribes where Name='{0}' and Tier='{1}'", requestedUser, iTier) ;
			cmd = new SqlCommand(strCmd, userDBConnection) ;
			myReader = cmd.ExecuteReader() ;
		
			while( myReader.Read() )
				{
				if (lju.tribe == null)
					lju.tribe = new ArrayList() ;
				alThisTier.Add( myReader.GetString( 0 ).Trim()) ;
//				Console.WriteLine("I'm adding: " + myReader.GetString( 0 ).Trim()) ;
				}

			// FOR HISTORICAL REASONS, we add a layer of indirection:
			ArrayList alNewLayer = new ArrayList() ;
			alNewLayer.Add( alThisTier ) ;
			lju.tribe.Add( alNewLayer ) ;
			myReader.Close() ;
			}

//		if (lju.tribe.Count == 0)
//			lju.tribe = null ;
		}
	
	localUserListCache.Add( lju ) ;

	// seems like we returned a null.  why?  tell me why.
	if (null == lju)
		throw new Exception() ;
	
	return lju ;
	}
*/			

	static public void AddUser( LJUser2 lju, bool fJustAddTribe, CUserList ulForNumericAdd )
		{

		if(m_fNoDBMode)
			{
			localUserListCache.Add( lju ) ;
			return ;
			}
			
		string str = "" ;
		MyNpgsqlCommand cmd ;

		if (false == fJustAddTribe)
			{
			str = string.Format("INSERT INTO LJUsers (Name, Location, Readers, BDate, Refreshed) Values('{0}', '{1}', {2}, null, GETDATE())", lju.Name, lju.Location, lju.Readers, lju.BDate) ;
			cmd = new MyNpgsqlCommand(str, userDBConnection) ;
	//		try
		//		{
				cmd.ExecuteNonQuery() ;
			//	}
//			catch( Exception e )
//				{
				// we are bad people.  we do bad things.
//				Console.WriteLine("FAILED LJUser: " + str) ;
//				Console.WriteLine ( e.ToString()) ;
//				}

			// communities don't have readers (in my dumb world) and probably should be added but here we are.
            /*
			if (lju.whoIRead != null)
				{
				foreach( string strIRead in lju.whoIRead)
					{
					str = string.Format("INSERT INTO WhoIRead (Name, UserIRead) Values('{0}', '{1}')", lju.Name, strIRead) ;
					cmd = new MyNpgsqlCommand(str, userDBConnection) ;
//					try
						{
						cmd.ExecuteNonQuery() ;
						}
// crash me out
//					catch( Exception e )
//						{
//						Console.WriteLine("FAILED WhoIRead: " + str) ;
//						Console.WriteLine( e.ToString()) ;
//						}
					}
				}
             * */
			}

		// i must duplicate the dataset perfectly.  But I don't.  I jumble sets.
		// and so with version five, i stop duplicating saves.  This will make my dataset smaller as we proceed.
		if (lju.tribe != null)
			{
			for( int iLevel = 0; iLevel < lju.tribe.Count; iLevel++)
				{
				ArrayList alWrittenAtThisLevel = new ArrayList() ;
				ArrayList alThisLevel = (ArrayList) lju.tribe[iLevel] ;
				foreach( ArrayList thisSet in alThisLevel )
					foreach( int strUser in thisSet )
					{
					// we're gonna lose the sets.  BOo hoo.
/*					bool fAlreadyDone = false ;
					foreach( int strAlready in alWrittenAtThisLevel)
						{
						if (strUser == strAlready)
							{
							fAlreadyDone = true ;
							break ;
							}
						}

  				if (false == fAlreadyDone)
						{
						alWrittenAtThisLevel.Add( strUser ) ;

						LJUser2 user = (LJUser2) ulForNumericAdd.GetUserByID( strUser ) ;
						str = string.Format("INSERT INTO Tribes (Name, Member, Tier) Values('{0}', '{1}', {2})", lju.Name, user.Name, iLevel) ;
						cmd = new MyNpgsqlCommand(str, userDBConnection) ;
						try
							{
							}
						catch( Exception )
							{
							}
						}
                         * */
					}
				}
			}
}

	static public bool WasSeedAborted( string strSeed )
		{
		string strCmd = string.Format("select Name from Abortions where Name='{0}'", strSeed) ;
		MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, userDBConnection) ;
		NpgsqlDataReader myReader = cmd.ExecuteReader() ;

		bool fRet = myReader.Read ( ) ;
		myReader.Close() ;
		return fRet ;
		}

/*
	static public void AbortSeed( string strSeed )
		{
		string str = string.Format("INSERT INTO Abortions (Name) Values('{0}')", strSeed) ;
		MyNpgsqlCommand cmd = new MyNpgsqlCommand(str, userDBConnection) ;
		cmd.ExecuteNonQuery() ;
		}
		*/
}


class GrabClass
{
    public const string SYSTEM_PASSWORD = "bipSy8!"; // this line makes this source code top secret.
    static public bool m_fYesToTimeouts = false;
    static public bool m_fALWAYSYesToTimeouts = false;
    static public bool m_fAbortNow = false;
    static public bool m_fDudeImDone = false;
    static public bool m_fLiberalCalcTime = false;
    static public bool m_fJustUploadRawSeed = false;
    static public bool m_fRetryAfterDelay = false;
//    static public bool m_fOkBruteForceIt = false;
    static public int m_iNextStepCount = -1;
    static public bool m_fTryTribeAgain = false;
    static public bool m_fDBDown = false;
//    static public int m_iSortBy;
//    public const int SB_READERS = 1;
//    public const int SB_DISTANCE = 2;
    static public int m_iLastHourPlucked = -1;
    // static public string m_topTier = "";
//    static public string m_bottomTier = "";
    // static public bool? fEmailCycle = null ;

    // public const int SB_

    // static public CUserList olMasterUserList = new CUserList() ;
    static public CUserList olCustomUserList = new CUserList(); // ONLY used in CalculateSingleSeed and growSet.
    static DateTime m_timeCheckUploadsAt = DateTime.Now;
    // static public string MASTER_USER_LIST = "LJUserDataset.xml" ; // c:\\temp\\ was removed cuz it wrecked my manual useage.

    /*
    public static void LoadMasterUserList()
    {
        try
        {
            Stream sr = File.OpenRead(MASTER_USER_LIST) ;
            SoapFormatter x = new SoapFormatter() ; // XmlSerializer(typeof(ArrayList), new Type[] { typeof(LJUser) }) ;
            olMasterUserList = (CUserList)x.Deserialize(sr) ;
            sr.Close() ;
        }
        catch( Exception e)
        {
            Console.WriteLine(e.ToString()) ;
        }
    }

    static void SaveMasterUserList()
    {
        try
        {
        Stream sw = File.Create(MASTER_USER_LIST) ;
        SoapFormatter x = new SoapFormatter() ; // XmlSerializer x = new XmlSerializer(typeof(ArrayList),  new Type[] { typeof(LJUser) }) ;
        x.Serialize(sw, olMasterUserList) ;
        sw.Close() ;
        }
        catch( Exception e )
        {
            Console.WriteLine( e.ToString() ) ;
        }
    }
    */

    /*
    static CUserList ConvertToLJ2( CUserList olSource ) 
    {
    CUserList newList = new CUserList () ;

    foreach( LJUser lju in olSource)
        {
        newList.Add ( new LJUser2( lju ) ) ;
        }

        return newList ;
    }
    */


    static public void ChokeOnBlankUrl(string strUrl)
    {
        if (null != strUrl)
            Debug.Assert(strUrl.Trim().Length > 0);
    }



    /* there's nothing wrong with this except my ISP hates me for it
static bool UrlExists( string strUrl )
{
	WebRequest request ;
	WebResponse response ;
	try
		{
		request = WebRequest.Create( strUrl );
		response = request.GetResponse(); 
		response.Close() ;
		}
	catch( Exception )
		{
		return false ;
		}

	return true ;
}
     * */

    static string ProduceMapString(LJUser2 lju, Rectangle rectCore, int iTerseAlertLevel)
    {
        string url = lju.Url;
        //     lju = MasterDB.GetSlimUser(lju.Name, true);
        lju.Url = url;
        string strImageMapText = "";

        /// right now i test the lju.Url.  If it doesn't pass muster, I clobber it dead in the database.  Not sure what I will let through.  But a lot of entries don't exist.
        // so we don't go around repeating them.
        /* THIS CODE IS DUPLICATE AND SHOULD BE PROPERLY DONE BEFORE PLACELJUSER
        if(lju.Url != null)
            {
            WebRequest request ;
            WebResponse response ;
            try
                {
                request = WebRequest.Create( lju.Url );
                response = request.GetResponse(); 
                response.Close() ;
			
    //			Stream s2 = response.GetResponseStream();
    //			StreamReader sr2 = new StreamReader( s2 );
    //			string line2;

    //			while( (line2 = sr2.ReadLine()) != null )
    //				{
    //				Console.WriteLine( line2 ) ;
    //				}
                }
            catch( Exception )
                {
    //			Console.WriteLine( e.ToString()) ;  
                MasterDB.ClobberUrl( lju.Name ) ; // If it's friends only, then friends already saw the stupid thing!
                lju.Url = null ;
                }
            }

        // url is null.  check that pickup/seed.htm exists.  if so, cobble the url to the pickup schtick.  save it in database.

        if (lju.Url == null)
            {
            // if seed.htm exists, then cobble pickup url.
            string strUrl = "http://ljmindmap.com/pickup/" + lju.Name + ".htm" ;
            if (UrlExists( strUrl ))
                {
                strUrl = "http://ljmindmap.com/tr/" + lju.Name + ".gif" ;
                if (UrlExists( strUrl ))
                    {
                    string strUseThis = "http://ljmindmap.com/h.aspx?n=" + lju.Name ;
                    MasterDB.AddUrl( lju.Name, strUseThis ) ;
                    lju.Url = strUseThis ;
                    }
                }
            }
            */

        Debug.Assert(lju.Location == null);
        /*
        if (lju.Location != null)
        {
            string strBriefLocation = lju.Location;

            strBriefLocation = strBriefLocation.Replace(" ,  United States", "");
            strBriefLocation = strBriefLocation.Replace(",  California", ", CA");
            strBriefLocation = strBriefLocation.Replace(" ,  ", ", ");

            // if we are a portal, we say so!
            if (lju.Url != null)
            {
                // if the url is a blank, i want to stop everything.
                Debug.Assert(lju.Url.Length > 0);

                switch (iTerseAlertLevel)
                {
                    case 0:
                        strBriefLocation += string.Format("{0}A Portal to another MindMap!", strBriefLocation.Length > 0 ? " - " : "");
                        break;
                    case 1:
                        strBriefLocation += string.Format("{0}A Portal!", strBriefLocation.Length > 0 ? " - " : "");
                        break;

                    case 2:
                        // add no gibberish
                        break;

                    // could screw up Peep Turf.
                    //					case 3:
                    // pulverize the location details
                    //						strBriefLocation = "" ;
                    //						break ;
                }


                strBriefLocation += string.Format("{0}A Portal to another MindMap!", strBriefLocation.Length > 0 ? " - " : "");
            }

            //			strImageMapText += string.Format("<area href=\"http://livejournal.com/~{0}\" {1} coords=\"{2},{3},{4},{5}\">{6}", // alt=\"{1}\" 
            strImageMapText += string.Format("<area href=\"{0}\" {1} {7} coords=\"{2},{3},{4},{5}\">{6}", // alt=\"{1}\" 
                                    (lju.Url == null) ? "http://livejournal.com/~" + lju.Name : lju.Url,
                                    (strBriefLocation == "" ? "" : "alt=\"" + strBriefLocation + "\""),
                                    rectCore.Left,
                                    rectCore.Top,
                                    rectCore.Right,
                                    rectCore.Bottom,
                                    "", // Environment.NewLine
                                    (strBriefLocation == "" ? "" : "title=\"" + strBriefLocation + "\"")
                                    );
        }

        else
        {
         */
        //			strImageMapText +=  string.Format("<area href=\"http://livejournal.com/~{0}\" coords=\"{2},{3},{4},{5}\">{6}",
        if (lju.Url != null)
        { 
            strImageMapText += string.Format("<area href=\"{0}\" {1} coords=\"{2},{3},{4},{5}\">{6}",
                                (lju.Url == null) ? "http://livejournal.com/~" + lju.Name : lju.Url,
                                (lju.Url == null) ? "" : "alt=\"A Portal to another MindMap!\"",
                                rectCore.Left,
                                rectCore.Top,
                                rectCore.Right,
                                rectCore.Bottom,
                                "" // Environment.NewLine
                                );
        }
     //   Debug.Assert(strImageMapText.Contains("http://ljmindmap.com/h.php?n="));

        return strImageMapText;
    }

    /*
    public static string ATest()
    {
        return "Yehaa!" ;
    }
    */


    public static string GetDangPowPwd()
    {
        string strpwdfile = "DangpowPwd.txt";
        Stream s = File.OpenRead(strpwdfile);
        StreamReader sr = new StreamReader(s);
        return sr.ReadLine();
    }

    public static string GetXferPwd()
    {
        string strpwdfile = "secretpassword.txt";
        Stream s = File.OpenRead(strpwdfile);
        StreamReader sr = new StreamReader(s);
        return sr.ReadLine();
    }

    public static void ExecutionerThreadMethod()
    {
        int iCycles = 135;
        if (m_fLiberalCalcTime)
            iCycles = 270;
        for (int iMin = 0; iMin < iCycles; iMin++) // ONE minute to test (and abort)
        {
            Thread.Sleep(5 * 1000);
            if (m_fDudeImDone || m_fAbortNow)
                return;
        }
//        if (false == m_fOkBruteForceIt)
  //          m_fAbortNow = true;
        // the aborting code notes this event in the database.
    }

    /*
    const string SS_QUEUED = null;
    const string SS_SCRAPED = "SCRAPED";
    const string SS_CALCULATING = "CALCULATING";
    const string SS_SCRAPE_FAILED = "SCRAPE_FAILED";
    const string SS_CALCULATED = "CALCULATED";
    const string SS_TIMEOUT = "TIMEOUT";
    const string SS_TIMEOUT_TWICE = "TIMEOUT_TWICE";
    const string SS_HISTORY_COOKED = "HISTORY_COOKED";
    const string SS_UPLOADED = "UPLOADED";
    const string SS_UPLOAD_CONFIRMED = "CONFIRMED";
    const string SS_COMPLETED = "COMPLETED";
    const string SS_ALL = "SHOW_ALL";
     * */

    const char SS_QUEUED = 'N'; // null;
    const char SS_SCRAPED = 'S'; // "SCRAPED";
    const char SS_CALCULATING = 'C'; // "CALCULATING";
//    const char SS_SCRAPE_FAILED = "SCRAPE_FAILED";
    const char SS_CALCULATED = 'L' ; // "CALCULATED";
//    const char SS_TIMEOUT = 'T'; // "TIMEOUT";
//    const char SS_TIMEOUT_TWICE = '2' ; // "TIMEOUT_TWICE";
    //const char SS_HISTORY_COOKED = "HISTORY_COOKED";
//    const char SS_UPLOADED = "UPLOADED";
//    const char SS_UPLOAD_CONFIRMED = "CONFIRMED";
//    const char SS_COMPLETED = "COMPLETED";
//    const char SS_ALL = "SHOW_ALL";


    const string SLAVE_STATUS_STRING_FILE = "clientStatusFile.txt";
    const string CONSUME_SCRAPED_QUEUE = "CONSUME_SCRAPED_QUEUE";
    const string CALC_CPU_100 = "CALC_CPU_100";

    static DateTime? FetchStatStamp(string seed)
    {
        NpgsqlConnection sqlc = MasterDB.GetDBConnection();
        string strCmd = string.Format("select StatusChanged from nameidmap where Name='{0}'", seed);
        
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, sqlc);
        NpgsqlDataReader myReader = cmd.ExecuteReader();
        myReader.Read();
        bool b = myReader.IsDBNull(0);
        if (b)
        {
            myReader.Close();
            return null;
        }
        DateTime dt = myReader.GetDateTime(0);
        myReader.Close();
        return dt;
    }

    //
    // this fn fetches all the seeds of a given status which DO NOT HAVE AN ERROR REPORTED
    // except i don't fuss with "error" nuances yet.
    // 
    static string[] FetchSeeds(char status)
    {
        NpgsqlConnection sqlc = MasterDB.GetDBConnection();

        string strCmd = "";

        switch (status)
        {
            case 'N' : // null:
                strCmd =
                    //"select seedqueue.Name from SeedQueue, nameidmap where seedqueue.name=nameidmap.name and Status IS NULL ORDER BY CASE WHEN Added IS null THEN '1/1/1999' ELSE Added END ASC, nameidmap.postsperyear desc ;";
                    //    "select Name from nameidmap where status='N' ORDER BY postsperyear desc ;";
"select Name from nameidmap where status = 'N' and (statuschanged is null or postsperyear > 0) order by statuschanged nulls first, postsperyear desc ;";

                break;

                /*
            case SS_ALL:
                //			strCmd = "select Name from SeedQueue ORDER BY Added ASC" ;
                //			strCmd = "select Name from SeedQueue ORDER BY CASE WHEN Added IS null THEN '1/1/1999' ELSE Added END ASC;";
                strCmd = "select Name from nameidmap ;"; // who the f uses this?
                Debug.Assert(false); // what's all this
                break;
                 * */

            //		case SS_SCRAPED:
            //			strCmd = string.Format("select Name from SeedQueue where Status='{0}' OR (Status='{1}' AND ADDED IS null) ORDER BY Added ASC", SS_SCRAPED, SS_TIMEOUT) ;
            //			break ;

            // Remove the special case for confirmed... just don't overload the notification queue with null addeds like radar might.
            // for upload confirmed, we put moneybit notifications first.
            //		case SS_UPLOAD_CONFIRMED:
            //			strCmd = 
            //"select A.Name from SEEDQUEUE A LEFT OUTER JOIN LJUserExtras B ON A.Name=B.Name where (A.STATUS='CONFIRMED') ORDER BY B.Money DESC" ;
            //			break ;


            default:
                //			strCmd = "select Name from SeedQueue where Status='" + status + "' ORDER BY Added ASC" ;
                // strCmd = "select Name from SeedQueue where Status='" + status + "' ORDER BY CASE WHEN Added IS null THEN '1/1/1999' ELSE Added END ASC;" ;
//                strCmd = "select Name from nameidmap where Status='" + status + "' ORDER BY postsperyear desc; ";
                strCmd = "select Name from nameidmap where Status='" + status + "' and (statuschanged is null or postsperyear > 0) ORDER BY statuschanged nulls first, postsperyear desc; ";
                break;
        }

        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, sqlc);
        cmd.CommandTimeout = 400000;// default 20 so... whatever who cares.
        NpgsqlDataReader myReader = cmd.ExecuteReader();

        ArrayList alSeeds = new ArrayList();

        while (myReader.Read())
        {
            string strNoDash = myReader.GetString(0).Trim();
            strNoDash = strNoDash.Replace("-", "_");
            alSeeds.Add(strNoDash);
        }
        myReader.Close();

        if (alSeeds.Count == 0)
            return null;

        string[] s = new string[alSeeds.Count];
        for (int i = 0; i < alSeeds.Count; i++)
            s[i] = (string)alSeeds[i];

        return s;
    }

    /*
static string SeedEmail( string seed )
{
	NpgsqlDataReader myReader ;
	try
		{
		NpgsqlConnection sqlc = MasterDB.GetDBConnection() ;
		string strCmd = "select EMail from SeedQueue where Name='" + seed + "'" ;
		MyNpgsqlCommand cmd = new MyNpgsqlCommand( strCmd, sqlc ) ;
		myReader = cmd.ExecuteReader() ;
		}
	catch( Exception )
		{
		return "" ;
		}

	try
		{
		string ret = "" ;
		if (myReader.Read ())
			{
			ret = myReader.GetString( 0 ).Trim() ;
			return ret ;
			}
		}
	catch( Exception )
		{
		return "" ; // let's hope we're right
		}
	finally
		{
		myReader.Close() ;
		}
	return "" ;
}
     * */



    static string GetSeedStatus(string seed)
    {
        string strCmd = string.Format("select Status from nameidmap where Name='{0}'", seed);
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, MasterDB.GetDBConnection());
        NpgsqlDataReader myReader = cmd.ExecuteReader();

        myReader.Read();
        string strRet = myReader.GetString(0);
        strRet = strRet.Trim();
        myReader.Close();
        return strRet;
    }

    static void SetSeedStatus(string seed, char status)
    {
        Console.WriteLine("Setting " + seed + " to " + status);
        NpgsqlConnection sqlc = MasterDB.GetDBConnection();
        string strCmd = "";
        switch (status)
        {
            // on calculated case, we need to set the ispublished to true
            case SS_CALCULATED:
                DateTime TwoK = new DateTime(0x7d0, 1, 1);
                int months = (int) (DateTime.Now.Subtract(TwoK).Days / 30.4) ;

                strCmd = string.Format(
                    //        "UPDATE SeedQueue SET Status = {0}, StatusChanged=GETDATE(), ispublished=true WHERE Name = '{1}'", status == null ? "null" : "'" + status + "'", seed);
//            "UPDATE nameidmap SET status = '{0}', StatusChanged=GETDATE(), iscolor=false, lastpublishedmonth={2} WHERE Name = '{1}'", status , seed, months);
            "UPDATE nameidmap SET status = '{0}', StatusChanged=now(), iscolor=false, lastpublishedmonth={2} WHERE Name = '{1}'", status, seed, months);
                break;
            default:
                strCmd = string.Format(
                    //        "UPDATE SeedQueue SET Status = {0}, StatusChanged=GETDATE() WHERE Name = '{1}'", status == null ? "null" : "'" + status + "'", seed);
//            "UPDATE nameidmap SET Status = '{0}', StatusChanged=GETDATE() WHERE Name = '{1}'", status, seed);
                            "UPDATE nameidmap SET Status = '{0}', StatusChanged=now() WHERE Name = '{1}'", status, seed);
                break;
        }
        //	UPDATE SEEDQUEUE SET Status='CALCULATING', StatusChanged=GETDATE() WHERE NAME='mcfnord'
        MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, sqlc);
        cmd.ExecuteNonQuery();
    }

    static void MarkError(string seed)
    {
        Console.WriteLine("DAMNED STINKING ERROR!");
        // do nothing! what's a legit error?
    }

    static bool ScrapeSeed(string seed)
    {
    // see iffn' we got it.
    pleaseJustTryAgain:
        LJUser2 lju = getUser(seed);
        if (null == lju)
        {
            if (m_fDBDown)
                goto pleaseJustTryAgain;

            return false;
        }

        if (null == lju.ifd)
            return false;
//        if (null == lju.whoIRead)
//            return false;

        /* We are going to load users during the calculating phase.  this will be faster on the database.  
            this code above just assures the seed is valid, or we error with scrape_failed.
        foreach (string str in lju.whoIRead)
        {
            // i'm going to wrecklessly ignore errors here.
    //		if ( null == 
                getUser( str ) ;
    //			)
    //			{
                // there might be good reasons why users fail, but there are bad ones...
                // and when they do, we error out of here, a failure.
    //			Console.WriteLine("ScrapeSeed's getUser failed.") ;
    //			return false ;
    //			}
        }
        */

        return true; // we'll say we did good.
    }

    const int CS_SUCCESS = 1;
    const int CS_TIMEOUT = 2;

    static int CalcSeed(string seed)
    {
        // if money bit, then fetch dataset of previous generation (if any) from ljuserextras.
        // can't be same here.
        // if same, spawn some infinite beeping nightmare.
        // code not here yet.

        // the ALWAYSYes complexity is probably unneeded.
        m_fYesToTimeouts = m_fALWAYSYesToTimeouts = true;
        m_fDudeImDone = m_fAbortNow = false;

        // clobber the internal intermediate queue cuz
        // i think it's the cause of this horrid bug.
        MasterDB.ClobberInternalCache();

        //	string [] parm = { seed } ;
        if (null != CalculateSingleSeed(seed))
        {
            m_fDudeImDone = false;
            olCustomUserList = new CUserList(); // .RemoveAll() ;

            /* i'm abandoning identifier system, though i might regret it.

            // i need to save the identifier of the current dataset Only if this is a moneybit.
            if (MasterDB.IsMoneyBit( seed ))
                {
                string strCmd = "select Identifier from Dataset" ;
                MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, MasterDB.GetDBConnection()) ;
                NpgsqlDataReader myReader = cmd.ExecuteReader() ;
                myReader.Read ( ) ;
                string id = myReader.GetString(0).Trim() ;
                myReader.Close() ;
			
                string str = string.Format("UPDATE LJUSEREXTRAS SET IDENTIFIER='{0}' where Name='{1}'", id, seed) ;
                cmd = new MyNpgsqlCommand(str, MasterDB.GetDBConnection()) ;
                cmd.ExecuteNonQuery() ;
                }

                */

            return CS_SUCCESS;
        }
        olCustomUserList = new CUserList(); // .RemoveAll() ;

        return CS_TIMEOUT;
    }

    // THIS CODE IS PROBABLY AWESOME BUT UNUSED.
    static List<string> EveryUniqueName(List<string> src)
    {
        List<string> sUnique = new List<string>();

        foreach (string s in src)
        {
            bool fFound = false;
            foreach (string already in sUnique)
            {
                if (already.ToUpper() == s.ToUpper())
                {
                    fFound = true;
                    break;
                }
            }
            if (false == fFound)
                sUnique.Add(s);
        }
        return sUnique;
    }

    static int CalcHistoricSeed(string seed)
    {
        // if money bit, then fetch dataset of previous generation (if any) from ljuserextras.
        // can't be same here.
        // if same, spawn some infinite beeping nightmare.
        // code not here yet.

        // the ALWAYSYes complexity is probably unneeded.
        m_fYesToTimeouts = m_fALWAYSYesToTimeouts = true;
        m_fDudeImDone = m_fAbortNow = false;

        // with historic seed, we set fdata to view from a specialized datasource, which we populate before calculating each frame.
        // but first we use the normal fdata for a current view.
        ArrayList alFinalPlacements = CalculateSingleSeed(seed);

        // I want a feature that grabs all days-back of a name where the friend-friendof (mindmap-visible) list is different than 
        // the prior days-back.
        // this list of days will be my frames.
        // so ask fdata what days we have for name x.

        List<int> fdDays = FData.GetFDataDates(seed);

        // the image represented by the last pull has already been generated.
        fdDays.RemoveAt(fdDays.Count - 1);

        // for now I just want to use these as my frames.
        // maybe someday i'll factor out "effective duplicates" based on two-way information.

        ArrayList alFramePlacements = null;

        foreach (int iDate in fdDays)
        {
            // some mechanism flushes the historical content here.
            TimeSpan ts = DateTime.Now.AddHours(24 * -iDate).Subtract(new DateTime(2000, 1, 1));
            MasterDB.m_fdDayOffset = ts.Days;
            olCustomUserList = new CUserList(); // .RemoveAll() ;		
            alFramePlacements = CalculateSingleSeed(seed, iDate, alFinalPlacements, alFramePlacements);
            MasterDB.m_fdDayOffset = 0; // I'm just paranoid.
        }

        return 1; // what for?
    }

    /*
        foreach( string sFD in sAllFDs )
            {
            Regex rIRead = new Regex(@"> \w+\n") ;
            Match m = rIRead.Match( sFD );

            while (m.Success)
                {
                string who = m.ToString().Trim().Substring(2) ;
                chumpsIveRead.Add( who ) ;
                m = m.NextMatch();
                }
            }

        // so everyChumpIEverRead has all these names in it.
        // let's dedupe this.
        chumpsIveRead = EveryUniqueName( chumpsIveRead ) ;

        // now we constitute every fd for these guys.
        foreach( string chump in chumpsIveRead )
            {
            FData.CopyAllSeedFDFiles( chump, CustomSrcDir ) ;
            FDataArchiveDB.ReconstituteSeedFDFiles( chump, CustomSrcDir ) ;
            }

        // then do the above for each name.

        const string CustomOpsDir = "c:\\temp\\fdata_custom\\" ;

        List<string> fds = FDataArchiveDB.CreateFDFiles( seed, CustomSrcDir ) ;

        foreach( string name in EveryUniqueName(fds) )
            {
            FDataArchiveDB.CreateFDFiles( name, CustomSrcDir ) ;
            }

        // everything's shoved into the source dir.
        // now we move the oldest of everyone into the ops dir.
        FDataArchiveDB.MoveOldestFiles( CustomSrcDir, CustomOpsDir, EveryUniqueName(fds)) ;

        // Swell. Now we tell fdata to use this custom ops dir and never scrape.
        FDataArchiveDB.UseOpsDir( CustomOpsDir ) ;

        // clobber the internal intermediate queue cuz
        // i think it's the cause of this horrid bug.
        MasterDB.ClobberInternalCache() ;

        string [] parm = { seed } ;
        if (null != CalculateSingleSeed( parm ) )
            {
            m_fDudeImDone = false ;
            olCustomUserList = new CUserList() ; // .RemoveAll() ;

            // i need to save the identifier of the current dataset Only if this is a moneybit.
            if (MasterDB.IsMoneyBit( seed ))
                {
                // figure out what the heck this is for.
                string strCmd = "select Identifier from Dataset" ;
                SqlCommand cmd = new SqlCommand(strCmd, MasterDB.GetDBConnection()) ;
                SqlDataReader myReader = cmd.ExecuteReader() ;
                myReader.Read ( ) ;
                string id = myReader.GetString(0).Trim() ;
                myReader.Close() ;
			
                string str = string.Format("UPDATE LJUSEREXTRAS SET IDENTIFIER='{0}' where Name='{1}'", id, seed) ;
                cmd = new SqlCommand(str, MasterDB.GetDBConnection()) ;
                cmd.ExecuteNonQuery() ;
                }

    //		return CS_SUCCESS ;
            }
        olCustomUserList = new CUserList() ; // .RemoveAll() ;

        // in this ultimate and wasteful appraoch, we move just one file (the eldest) from the source dir to the ops dir
        // and re-calc. insane, yes?

        return CS_TIMEOUT ;
    }
    */



    // Yes, do it, why not.
    // generate a BATCH file, and a TXT file.
    //
    // BATCH file is always the same.  so no need to even mention it.
    // the TXT file does change.
    // I can't show the magic data here though.
    // you know, i might not need teh batch file at all.
    // just spawn the text file.  try that.

    
//    static bool UploadSeedMap(string seed)
//    {
        /* OK WE AREN'T GOING TO DO THIS HERE.
        string ftpdriverfile = "ftpcmds.txt" ;

        FileInfo fi = new FileInfo(ftpdriverfile);
        using (StreamWriter sw = fi.CreateText())
            {
            sw.WriteLine("open dangpow.com") ;
            sw.WriteLine("gifs") ;
            sw.WriteLine( GetDangPowPwd() ) ;
            sw.WriteLine("cd public_html") ;
            sw.WriteLine("cd mm_mirror") ;
            sw.WriteLine("cd tr") ;
            sw.WriteLine("bin") ;
            sw.WriteLine("send " + seed + "_t.gif") ;
            sw.WriteLine("send " + seed + ".gif") ;			
            sw.WriteLine("bye") ;
            sw.Close() ;
            }


        // now call this thing.
        System.Diagnostics.Process P = null;
        try 
            {
            P = new System.Diagnostics.Process();
            string WorkingDirectory = "C:\\temp";
            //P.StartInfo.RedirectStandardOutput = true;
            P.StartInfo.CreateNoWindow = true;
            P.StartInfo.WorkingDirectory = WorkingDirectory;
            P.StartInfo.FileName =  "ftp.exe";
            P.StartInfo.Arguments = " -s:ftpcmds.txt";                //P.EnableRaisingEvents = true;

            P.StartInfo.UseShellExecute = false;
            P.Start();
            // THIS DOES NOT APPEAR TO WORK.  AND YET DOES NOT THROW AN EXCEPTION.
             get the ftp to spawn.  or dont.  i can build a script to mass-upload!
            why not do that?  generate a mass-upload script creator,
            and then just run a batch file.  why not?
            }
        catch(Exception er)
            {
            string sMsg = "Error in starting process " + er.GetType() + er.Message;
            }
        */

//        return false; // failed
//    }

    static bool UploadConfirmed(string seed)
    {
        // the upload is confirmed if the seed file exists on gal2k!
        WebRequest request;
        WebResponse response;
        string url = "";
        try
        {
            //		string url = string.Format("http://www.gal2k.com/tr/{0}.gif", seed) ;
            url = string.Format("http://www.ljmindmap.com/tr/{0}.gif", seed);
            request = WebRequest.Create(url);
            response = request.GetResponse();
        }
        catch (Exception)
        {
            return false;
        }

        Console.WriteLine("Entering response.Close() for " + url);
        response.Close();
        Console.WriteLine("Exiting response.Close() for " + url);

        return true;
    }


    static string Urlify(string str)
    {
        str = str.Replace(";", "%3b");
        str = str.Replace("/", "%2f");
        str = str.Replace("?", "%3f");
        str = str.Replace(":", "%3a");
        str = str.Replace("@", "%40");
        str = str.Replace("&", "%26");
        str = str.Replace("=", "%3d");
        str = str.Replace("+", "%2b");
        str = str.Replace("$", "%24");
        str = str.Replace(",", "%2c");
        str = str.Replace(" ", "%20");
        str = str.Replace("#", "%23");
        str = str.Replace('"'.ToString(), "%22");
        //      str = str.Replace( "'", "%27") ;

        return str;
    }

    /*
static bool SendEmailThroughMyISP( string seed, string strTargEmail )
{
	// I could send through my own isp and of course i will again.
	// now i send my own emails.
	try
		{
		EmailFromMe  efm = new EmailFromMe() ;
		string s = efm.SendEmail( 
				SYSTEM_PASSWORD,
				strTargEmail, 
				"Your MindMap is Ready!",
				"http://ljmindmap.com/h.php?n=" + seed + "\r\n\r\nreply with your compliments and complaints.") ;
//				"Please e-friend my brother and I'll give you color overnight.\r\n   http://www.livejournal.com/friends/add.bml?user=peristaltor") ; // reply with your compliments and complaints." ) ;
		if ("OK" == s)
			return true ;

		Console.WriteLine( s ) ;
		
		return false ;
		}
	catch( Exception )
		{
		return false ;
		}

//	return true ; // maybe we are sloopy with the errz.
}
     * */

    /*
static bool SendEmail( string seed, string password )
{
	try
		{
		string strTargEmail = SeedEmail(seed ) ;
		if (strTargEmail.Length > 0)
			{
			string url = 
				"http://www.dangpow.com/~gifs/se.php?pwd=" + password + 
				"&to="  + Urlify( strTargEmail ) + 
				"&subj=Your MindMap Is Ready&content=" + Urlify("http://ljmindmap.com/h.php?n=" + seed) ;
			WebRequest request = WebRequest.Create( url );
			WebResponse response = request.GetResponse();

			}
		}
	catch( Exception e )
		{
		Console.WriteLine("Error" + e.ToString()) ;
		return false ;
		}
	
	return true ;
}
     * */


    public static void AddIfSeedNew(FileInfo fi, ArrayList seeds)
    {
        // strip off the .gif extension.  if there's a _t, we ignore this, because banners don't mean a whole lot.
        string strNam = fi.Name;
        if (-1 == strNam.IndexOf(".gif"))
            return;

        if (-1 != strNam.IndexOf("_t.gif"))
            return;

        strNam = strNam.Substring(0, strNam.IndexOf(".gif"));
        seeds.Add(strNam);
        //	Console.WriteLine( strNam ) ;
        //	Console.WriteLine( seeds );
    }


    public static bool FriendEitherWay(string strDude, string strOtherDude)
    {
        if (strDude.ToUpper() == strOtherDude.ToUpper())
            return false; // don't want this 

        LJUser2 lju1 = MasterDB.GetSlimUser(strDude, false);
        if (null == lju1)
            return false;

        if (lju1.Reads(strOtherDude))
            return true;

        LJUser2 lju2 = MasterDB.GetSlimUser(strOtherDude);
        if (null == lju2)
            return false;

        if (lju2.Reads(strDude))
            return true;

        return false;
    }


    // FireBullet surveys what items we have in the outgoing queue (including postponed items)
    // and orders them by relationships to a chronologically weighted incoming request queue.
    /* this is obsolete right?
    public static void FireBullet()
    {
        // see if a fire.bat file currently exists.  if not, then we're outta here.
        string UseMe = "c:\\temp\\uploads\\fire.bat" ;

        FileInfo fi = new FileInfo(UseMe);
        if (fi.Exists)
            return ; // no action!
	
        // gather up all the seeds in uploads and uploads/postponed
        string strDir = "c:\\temp\\uploads\\postponed"  ;

        DirectoryInfo di = new DirectoryInfo(strDir);
        // Create an array representing the files in the current directory.
        FileInfo[] fia = di.GetFiles();

        ArrayList seedsInPostponed = new ArrayList() ;
	
        // Print out the names of the files in the current directory.
        for(int iFilePos = 0 ; iFilePos < fia.GetLength(0); iFilePos++)
            {
            AddIfSeedNew( fia[iFilePos], seedsInPostponed ) ;
            }

        // rank this list against the incoming queue.  Show me who is most connected into this queue, weighted toward most recent queue additions.
        // emit a fire.bat file and mopup.bat file
        // so i need the incoming queue, sorted by added date.
        NpgsqlConnection sqlc = MasterDB.GetDBConnection() ;
        MyNpgsqlCommand cmd = new MyNpgsqlCommand("select Name from SeedQueue WHERE Added IS NOT null ORDER BY Added DESC", sqlc) ;
        NpgsqlDataReader myReader = cmd.ExecuteReader() ;
        ArrayList alSeedsInReqQ = new ArrayList() ;
        while( myReader.Read ( ) )
            alSeedsInReqQ.Add (myReader.GetString( 0 ).Trim() ) ;
        myReader.Close() ;

        int upToTwenty = alSeedsInReqQ.Count ;
        if (upToTwenty > 20)
            upToTwenty = 20 ;

        int [] score = new int[ seedsInPostponed.Count ] ;

        for( int iPos = 0; iPos < upToTwenty; iPos++ )
            {
            for( int iChoicesPos = 0; iChoicesPos < seedsInPostponed.Count; iChoicesPos++ )
                {
                if (FriendEitherWay( (string) seedsInPostponed[iChoicesPos], (string) alSeedsInReqQ[iPos]))
                    {
                    score[iChoicesPos] += upToTwenty - iPos ;
                    Console.WriteLine( seedsInPostponed[iChoicesPos] + ": " + score[iChoicesPos]) ;
                    }
                }
            Console.WriteLine("Incoming item " + iPos + " considered.") ;
            }

        for( int iDone = 0 ; iDone < seedsInPostponed.Count; iDone++)
            {
            if (score[iDone] > 0)
                Console.WriteLine( score[iDone] + " " + seedsInPostponed[iDone] ) ;
            }

    /*
        using (StreamWriter sw = fi.CreateText())
            {
            sw.WriteLine("") ;
            }
            */

    // the fire.bat uploads the seed file or files (_t if found).
    // the mopup.bat moves the sent files to sent.
    // }





    /*
public static long GrabRemoteFile( string strName, string strLocal )
{
	// 1) fetch new items from server.
	XF2ServiceLjmindmapCom xff = new XF2ServiceLjmindmapCom () ;
	long size = -10 ;
//	const string REQUESTS_CS = "requests.cs" ;
	byte [] b = null ;
	try
		{
		b = xff.Download( GetXferPwd(), strName, ref size ) ;
		}
	catch( WebException )
		{
		// i don't care if the web is broken... i should just move on to the computational work.
		// size begins as -1 so we drop out here and skip the consideration of new requests.
		}

	if (-1 != size)
		{
		// save this file locally.  then parse it to look for new seeds.

//		const string UseMe = "c:\\temp\\mylocalfile.txt" ;

		FileStream  fileStream = new FileStream(strLocal, FileMode.Create) ;
		// Write the data to the file, byte by byte.
		for(int iByte = 0 ; iByte < size; iByte++)
			fileStream.WriteByte( b[ iByte]  ) ;

		fileStream.Close() ;
		return size ;
		}
	
	return size ;
}
     * */

    // We publish a password to ljmindmap that dangpow reads.

    /*
public static string NewPassword()
{
	const string hex = "0123456789ABCDEF" ;
	Random rd = new Random() ;
	string pwd = "" ;
	for(int iChars = 0; iChars < 4; iChars++)
		pwd += hex[ rd.Next() % hex.Length ].ToString() ;
	
	byte [] b = new byte[ pwd.Length ] ;
	for( int iPos = 0; iPos < pwd.Length; iPos++)
		b[iPos] = (byte) pwd[iPos] ;

	TryAgainPass:
	try
		{
		XF2ServiceLjmindmapCom ufs = new XF2ServiceLjmindmapCom () ;
		string ret = ufs.Upload( GetXferPwd(), "/emailpwd.txt", b ) ;
		}
	catch
		{
		Console.WriteLine( "Trying to send my own password again.") ;
		Thread.Sleep( 9000 ) ;
		goto TryAgainPass ;
		}
	return pwd ;
}
     * */

    public static void Main(string[] args)
    {
        if (DateTime.Now.Year < 2012)
        {
            Console.WriteLine("FUCK YOU THE CLOCK IS WRONG.");
            return;
        }
        if (args[0].ToUpper() == "-CLIQUES".ToUpper())
            TCliquesClass.TCliquesMain("mcfnord");

        if(args[0].ToUpper() == "-BABYSITTER".ToUpper())
        {
            tribe.BabySitter.KickUsAlong() ;
            return ;
        }

        if (args[0].ToUpper() == "-RADAR".ToUpper())
        {
            Console.WriteLine("obsolete");
//            Radar2010.RefreshBasedOnRadarSignals_AndPublish();
            return;
        }

        if (args[0].ToUpper() == "-FAST".ToUpper())
        {
//            m_fOkBruteForceIt = true;

            // chop this flag away.
            string[] newArgs = new string[args.GetLength(0) - 1];
            for (int iPos = 1; iPos < args.GetLength(0); iPos++)
                newArgs[iPos - 1] = args[iPos];
            args = newArgs;
        }

        // TIME TO DO A RATHER TRICKY THING.
        if (args[0].ToUpper() == "-NO_DB".ToUpper())
        {
            MasterDB.m_fNoDBMode = true;
            // chop this flag away.
            string[] newArgs = new string[args.GetLength(0) - 1];
            for (int iPos = 1; iPos < args.GetLength(0); iPos++)
                newArgs[iPos - 1] = args[iPos];
            args = newArgs;
        }


        // this here first autocalc pass will stop us if there are stuck seeds.
        // but the main event autocalc pass is later.
        // ONLY on the server... (pity the day server no longer does autocalc runs.)
        if (args[0].ToUpper() == "-AUTOCALC".ToUpper())
        {
            FileInfo fi = new FileInfo(SLAVE_STATUS_STRING_FILE);
            if (false == fi.Exists)
            {
                // we care if a seed says CALCULATING for more than three hours.
                string[] sa = FetchSeeds(SS_CALCULATING);

                if (null != sa)
                {
                    for (int i = 0; i < sa.GetLength(0); i++)
                    {
                        DateTime? dt = FetchStatStamp(sa[i]);
                        if ((dt == null) || ( ((DateTime)dt).AddHours(1) < DateTime.Now)) // you've got one hour.
                        {
                            SetSeedStatus(sa[i], SS_SCRAPED); // return to pool.
                            Console.WriteLine(" Seed " + sa[i] + " considered stuck calc'ing and has been returned to pool.");
                        }
                    }
                }
            }

            /* this way sucks.		
                    // i want a beep and notification if any statuses are currently CALCULATING, because I don't
                    // want such items to get stuck. (they could be legit. I could timestamp them but have not yet)
                    DateTime dtTooOld = DateTime.Now.AddHours( -1 ) ;
                    string[] couldBeStuckSeeds = FetchSeeds( SS_CALCULATING ) ;
                    if (couldBeStuckSeeds != null)
                        {
                        // ok, check the time against now. it needs to be more than an hour old for me to stop.
			
                        for(int i = 0; i < couldBeStuckSeeds.GetLength(0); i++)
                            {
                            string s = couldBeStuckSeeds[ i ] ;
                            DateTime dtLastStatChange = FetchStatStamp( s ) ;
                            if (dtLastStatChange < dtTooOld )
                                {
                                Console.WriteLine(" >>> " + couldBeStuckSeeds[ i ]) ;
                                // For now I prefer to stop
                                Console.WriteLine("Run RSC.EXE WHEN NO OTHER -AUTOCALCs ARE RUNNING! to Release stuck Calculating items. Quitting.") ;
                                return ;
                                }
                            }
                        }
                        */
        }

        //	LoadMasterUserList() ;

        // because i use goto, i get to initialize variables here:
        int iCalculated = 0;
        int iTimedout = 0;
        DateTime dtLastMine = DateTime.Now;
        Dictionary<string, DateTime> lastOutOfISP = new Dictionary<string, DateTime>();
        Dictionary<string, int> skippedOfISP = new Dictionary<string, int>();

    RenewDBFresh:
        if (false == MasterDB.m_fNoDBMode)
            MasterDB.Init();

        // an import mode takes in names.txt entries and adds them to the seedqueue
        /*
        if( args[0].ToUpper() == "-IMPORT_NAMES".ToUpper())
            {
            NpgsqlConnection sqlc = MasterDB.GetDBConnection() ;
		
            Stream s = File.OpenRead( "names.txt" ) ;
            StreamReader sr = new StreamReader( s );
            string line = "" ;
            while( (line = sr.ReadLine()) != null )
                {
                string strCmd = string.Format("INSERT INTO SeedQueue (Name) Values('{0}')", line) ;
                MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, sqlc) ;
                cmd.ExecuteNonQuery() ;
                }

            MasterDB.Close() ;
            return ;
            }
         * */

        // TIME TO DO A RATHER TRICKY THING.
        if (args[0].ToUpper() == "-JUST_UPLOAD_RAW_SEED".ToUpper())
        {
            m_fJustUploadRawSeed = true;
            // chop this flag away.
            string[] newArgs = new string[args.GetLength(0) - 1];
            for (int iPos = 1; iPos < args.GetLength(0); iPos++)
                newArgs[iPos - 1] = args[iPos];
            args = newArgs;
        }

        if (args[0].ToUpper() == "-TIME_LIMIT".ToUpper())
        {
            m_fYesToTimeouts = true;
            Console.WriteLine("SINGLE SEED GENERATION LIMITED TO TEN MINUTES IF NEVER TIMED OUT BEFORE.");
            string[] newArgs = new string[args.GetLength(0) - 1];
            for (int iPos = 1; iPos < args.GetLength(0); iPos++)
                newArgs[iPos - 1] = args[iPos];
            args = newArgs;
        }

        // Automated does it all.
        /* THIS CODE IS NOT BAD.  IT JUST PISSES ME OFF WHEN I TRY TO FIX SOMETHING HERE AND MEAN TO FIX IT ON THE CODE I ACTUALLY RUN.
        //    SO I'M MAKING THIS CODE GO TO SLEEP FOR A WHILE.  A LONG SLEEP.
	    
        if( args[0].ToUpper() == "-AUTOMATED".ToUpper())
            {
            Console.WriteLine("Hey, joker, -automated has been replaced by a two-window -autocalc and -autobits") ;
    //		return ;
		
            for(;;) // we just loop here forever
            {

            FileInfo fi = new FileInfo("c:\\temp\\stop.txt");

            if (fi.Exists)
                {
                Console.WriteLine("\a\a\a\a") ;
                return ;
                }

            FireBullet() ;
	

            string UseMe = "c:\\temp\\mylocalfile.txt" ;
            string REQUESTS_CS = "requests.cs" ;
            long size = GrabRemoteFile( REQUESTS_CS, UseMe) ;
            if (-1 != size)
                {

                // parse and see if anyone is new.  add the seeds.
                // separated by a space, huh.
                Stream s = File.OpenRead( UseMe  ) ;
                StreamReader sr = new StreamReader( s );
                ArrayList alseeds = new ArrayList() ;
                ArrayList emails = new ArrayList() ;
                string line = "" ;
                while( (line = sr.ReadLine()) != null )
                    {
                    string seed = line.Substring( 0, line.IndexOf(" ")).ToLower() ;
                    string email = line.Substring( seed.Length + 1 ).ToLower() ;

                    seed = seed.Replace("-", "_") ;

                    // ha ha no spaces allowed in email addresses, you dork.
                    if (-1 == email.IndexOf(" "))
                        {
                        alseeds.Add( seed ) ;
                        emails.Add( email ) ;
                        }
                    }

                sr.Close() ;
                s.Close() ;
			
                // if this seed is not in the thingie, add it.
                string[] knownSeeds = FetchSeeds( SS_ALL ) ;
                for(int iEach = 0; iEach < alseeds.Count; iEach++)
                    {
                    string strNewOrNot = ((string) alseeds[ iEach ]).ToLower() ;
                    bool fSkip = false ;
				
                    for(int iAlready = 0; iAlready < knownSeeds.GetLength( 0 ) ; iAlready++)
                        {
                        if( knownSeeds[ iAlready ].ToLower() == strNewOrNot )
                            {
                            fSkip = true ;
                            break ;
                            }
                        }
                    if (false == fSkip)
                        {
    //					Console.WriteLine("I think I detected: " + strNewOrNot) ;

                        SqlConnection sqlc = MasterDB.GetDBConnection() ;

                        string seedAdd = alseeds[ iEach ].ToString().ToLower() ;
                        string emailAdd = emails[ iEach ].ToString().ToLower() ;

                        Console.WriteLine("I think I detected: " + seedAdd + " " + emailAdd ) ;					
		
                        string strCmd = string.Format("INSERT INTO SeedQueue (Name, EMail, Added) Values('{0}', '{1}', GETDATE() )", 
                                                    seedAdd,
                                                    emailAdd ) ;
                        SqlCommand cmd = new SqlCommand(strCmd, sqlc) ;
                        cmd.ExecuteNonQuery() ;
                        // refresh this fucker!
                        knownSeeds = FetchSeeds( SS_ALL ) ;
                        }
                    }
			

                // do the secure file removal
                FileXferService xff = new FileXferService() ;
                try
                    {
                xff.DeleteIfUngrown( GetXferPwd(), REQUESTS_CS, size ) ;
                    }
                catch( System.Net.WebException )
                    {
                    // ignore it.  It's just low bw condition. This file gets killed eventually and duplication doesn't hurt me.
                    }
                }

            string url = string.Format("http://www.ljmindmap.com/pluck/pluck.aspx") ;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create( url );
            WebResponse response = null ;
            try
                {
                response = request.GetResponse();
                }
            catch( WebException we )
                {
                Console.WriteLine( we.ToString() ) ;
                }

            if (response == null)
                Console.WriteLine("PLUCK failed to return, perhaps cuz the .aspx code is taking too long to run server-side.") ;
            else
                {
                Stream s2 = response.GetResponseStream();
                StreamReader sr2 = new StreamReader( s2 );
                string line2;
                SqlConnection sqc = MasterDB.GetDBConnection() ;

                while( (line2 = sr2.ReadLine()) != null )
                    {
                    // parse out the item name and its url.  UPdate it in LJUserExtras
        //			Console.WriteLine( line2 ) ;
                    Regex r = new Regex(" ");
                    string [] realContent = r.Split(line2) ;
        //			bool fNew = false ;
                    if (realContent.GetLength(0) > 1)
                        {
                        // Do it right.  Query.  If it exists, update.
                        string strCmd = string.Format("select count(*) from LJUserExtras where Name='{0}'", realContent[0]) ;
                        SqlCommand cmd = new SqlCommand( strCmd, sqc ) ;

                        SqlDataReader myReader = cmd.ExecuteReader() ;
                        myReader.Read ( ) ;
                        if (0 == myReader.GetInt32(0))
                            {
                            myReader.Close() ;
                            if (realContent[1].Length > 127)
                                {
                                realContent[1] = "" ;
    //							throw new Exception() ; // is this a real url?  should we utterly obliterate it?  clipping it is dumb.
    //							realContent[1] = realContent[1].Substring( 0, 127 ) ;
                                }

                            strCmd = string.Format("INSERT INTO LJUserExtras (Name, Url) Values('{0}', '{1}')", realContent[0], realContent[1]) ;
                            cmd = new SqlCommand(strCmd, sqc) ;
                            cmd.ExecuteNonQuery() ;
                            Console.WriteLine(realContent[0] + " posted.") ;
                            }
                        else
                            {
                            myReader.Close() ;
                            if (realContent[1].Length > 127)
                                {
                                realContent[1] = "" ;
    //							throw new Exception() ; // is this a real url?  should we utterly obliterate it?  clipping it is dumb.
    //							realContent[1] = realContent[1].Substring( 0, 127 ) ;
                                }
						
                            strCmd = string.Format("UPDATE LJUserExtras SET Url='{1}' WHERE Name='{0}'", realContent[0], realContent[1]) ;
                            cmd = new SqlCommand(strCmd, sqc) ;
                            cmd.ExecuteNonQuery() ;
                            }
                        }
                    }
                }

            // i want a summary as i go:
            int iTotal = 0 ;
            string[] seeds = FetchSeeds( SS_QUEUED ) ;
            if (null != seeds) 
                {
                iTotal += seeds.GetLength( 0 ) ;
                Console.WriteLine("There are {0} seeds queued, awaiting scrape.", seeds.GetLength( 0 ) ) ;
			
                }

            seeds = FetchSeeds( SS_SCRAPED ) ;
            if ( null != seeds )
                {
                iTotal += seeds.GetLength( 0 ) ;
                Console.WriteLine("There are {0} seeds scraped, awaiting calculation.", seeds.GetLength( 0 ) ) ;
                }

            seeds = FetchSeeds( SS_CALCULATED ) ;
            if (null != seeds )
                {
                iTotal += seeds.GetLength( 0 ) ;
                Console.WriteLine("There are {0} seeds calculated, awaiting upload.", seeds.GetLength( 0 ) ) ;
                }

            seeds = FetchSeeds( SS_UPLOAD_CONFIRMED ) ;
            if (null != seeds )
                {
                iTotal += seeds.GetLength( 0 ) ;
                Console.WriteLine("There are {0} seeds confirmed, awaiting notification.", seeds.GetLength( 0 ) ) ;
                }

		

            Console.WriteLine("THE METRIC OF SUCCESS I LOVE TO TRACK IS: {0} SEEDS IN THE PIPELINE.", iTotal) ;

            // 5) Fetch a seed awaiting confirmation and check it out.
            seeds = FetchSeeds( SS_CALCULATED ) ;
            if ( null != seeds)
                {
                foreach( string seed in seeds )
                    {
                    Console.WriteLine("") ;

                    Console.WriteLine("ConfirmingUpload: " + seed ) ;
                    if ( UploadConfirmed( seed ))
                        SetSeedStatus( seed, SS_UPLOAD_CONFIRMED ) ;
                    else
                        break ; // we don't check any more if we fail.
                    }
                }

            // 6) Send emails for these cats.

            seeds = FetchSeeds( SS_UPLOAD_CONFIRMED ) ;
            if ( null != seeds )
                {
                foreach( string seedy in seeds )
                    {
                    Console.WriteLine("") ;
    //				string seedy = "sebatical" ;
				
                    Console.WriteLine("SendingEmail: " + seedy) ;

                    // nobody's sending out yet.
                    if ( SendEmail( seedy))
                        SetSeedStatus( seedy, SS_COMPLETED ) ;
                    else
                        MarkError( seedy) ;
                    }
                }
		
            // 2) Get a queued seed.

            // 3) Fetch scraped seeds
    // 	I WANT TO DO A PASS ON TIMEOUT ITEMS, because i don't believe they're all timeout items.
    //		seeds = FetchSeeds( SS_TIMEOUT ) ;

            // if no seeds need to be calculated, look for timeouts and give them a hearty try
    // return this line when i want to face the fact that my snowball method screwed up the non-brute-force method:
            seeds = FetchSeeds( SS_SCRAPED ) ;
		
            if (null == seeds )
                {
                seeds = FetchSeeds( SS_TIMEOUT ) ;
                if (null != seeds)
                    {
                    string seed = seeds[ 0 ] ;
                    Console.WriteLine("") ;
                    Console.WriteLine("Calculating a former timeout: " + seed ) ;
                    m_fLiberalCalcTime = true ;
                            m_fOkBruteForceIt = true ;
				
                    switch( CalcSeed( seed ))
                        {
                        case CS_SUCCESS:
                            SetSeedStatus( seed, SS_CALCULATED ) ;
                            break ;
                        case CS_TIMEOUT:
                            SetSeedStatus( seed, SS_TIMEOUT_TWICE ) ;
                            break ;
                        default:
                            MarkError( seed ) ;
                            break ;
                        }
                            m_fOkBruteForceIt = false ;
				
                    }
                }
            else
            if (null != seeds )
                {
                m_fLiberalCalcTime = false ;

    //			foreach( string seed in seeds )
    // for development, we'll just work on the first item.
                // do up to two so i can avoid backlogs yet stay cycling.
                for ( int iItem = 0 ; iItem < 2; iItem++)
                    {
                    string seed = seeds[ iItem ] ; 

                    // If it's a timeout, and it's here, it's cuz it's added=null.  don't ask questions, just bruteforce 
                    if (SS_TIMEOUT == GetSeedStatus( seed ))
                        m_fOkBruteForceIt = true ;

                        {
                        Console.WriteLine("") ;
                        Console.WriteLine("Calculating: " + seed ) ;
                        switch( CalcSeed( seed ))
                            {
                            case CS_SUCCESS:
                                SetSeedStatus( seed, SS_CALCULATED ) ;
                                break ;
                            case CS_TIMEOUT:
                                SetSeedStatus( seed, SS_TIMEOUT ) ;
                                break ;
                            default:
                                MarkError( seed ) ;
                                break ;
                            }
                        m_fOkBruteForceIt = false ;
					
                        }

                    // only do two if we HAVE two.
                    if (seeds.GetLength(0)  < 2)
                        break ;
                    }
                }


            // to eliminate db crashes due to memory,
            // start fresh.
            MasterDB.Close() ;
            goto RenewDBFresh ;

            }
            }
    */



        // Automated does it all.

        bool fCalcOrTimeout = false; // calc
        if (args[0].ToUpper() == "-AUTOCALC_TIMEOUT")
            fCalcOrTimeout = true;

        if ((fCalcOrTimeout) ||
            (args[0].ToUpper() == "-AUTOCALC".ToUpper()))
        {

            Mutex m = new Mutex(false, CONSUME_SCRAPED_QUEUE);

            for (; ; ) // we just loop here forever
            {

                FileInfo fi = new FileInfo("c:\\temp\\stop.txt");

                if (fi.Exists)
                {
                    Console.WriteLine("\a\a\a\a");
                    return;
                }

                // under the new rules, we calculate the oldest item, and if it's a timeout, we brute force it.
                // note that the null in the datestamp facilitates this model, because null is the oldest.
                //
                // therefore we look for timeouts first.

                // after that timeout (if any), a fresh seed please.

                // we loop until we've done all the moneybits... actually we also loop so long as there's a
                // top-tier member... but the primary function here is to hammer through all the moneybits
                // before moving on to timeouts.
                bool fFound = true;

                while (fFound) // why do we loop here? 
                {
                    fFound = false;

                    m.WaitOne();

                    char sScrapedStatus = SS_SCRAPED;
                    /*
                    fi = new FileInfo(SLAVE_STATUS_STRING_FILE);
                    if (fi.Exists)
                    {
                        Stream s = File.OpenRead(SLAVE_STATUS_STRING_FILE);
                        StreamReader sr = new StreamReader(s);
                        sScrapedStatus = sr.ReadLine();
                    }
                    else
                                          * */
                    if (fCalcOrTimeout == true)
                    {
                        Debug.Assert(false); //  wtf
//                        sScrapedStatus = SS_TIMEOUT;
                    }

                    string[] seeds = FetchSeeds(sScrapedStatus);
                    string seed = null;

                    if (null == seeds)
                    {
                        // just take a damn breath right here.
                        Thread.Sleep(60000);
                    }
                    else
                    //			if (null != seeds )
                    {
                        for (int iEach = 0; iEach < seeds.GetLength(0); iEach++)
                        {
                            seed = seeds[iEach];
                            /*
                            if (MasterDB.IsMoneyBit(seed))
                            {
                                Console.WriteLine("Money bit mister! However, I publish b&w now for everyone."); fFound = true;
                                break;
                            }
                             * */

                            /*
                            // if seed appears in the last top tier processed, we roll with it.
                            if (-1 != m_topTier.IndexOf(seed))
                            {
                                Console.WriteLine(seed + " is a top tier member of the last seed. Here's their top tier: " + m_topTier);
                                fFound = true;
                                break;
                            }
                             * */
                        }

                        if (false == fFound)
                        {
                            seed = seeds[0];
                            Console.WriteLine("No seed in queue gets preferred.");
                        }

                        SetSeedStatus(seed, SS_CALCULATING);
                    }
                    m.ReleaseMutex();

                    if (null != seed)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Calculating: " + seed);

                        m_fLiberalCalcTime = false;
//                        m_fOkBruteForceIt = false;

                        if (fCalcOrTimeout) // timeout
                        {
                            m_fLiberalCalcTime = true;
//                            m_fOkBruteForceIt = true;
                        }

                        switch (CalcSeed(seed))
                        {
                            case CS_SUCCESS:
                                SetSeedStatus(seed, SS_CALCULATED);

                                // some more shit happens

                                iCalculated++;
                                break;
/*
                            case CS_TIMEOUT:
                                if (fCalcOrTimeout) // timeout
                                    SetSeedStatus(seed, SS_TIMEOUT_TWICE);
                                else
                                    SetSeedStatus(seed, SS_TIMEOUT);
                                iTimedout++;
                                break;
 * */

                            default:
                                MarkError(seed);
                                break;
                        }
//                        m_fOkBruteForceIt = false;
                        m_fLiberalCalcTime = false;

                        if (iCalculated + iTimedout > 0)
                            Console.WriteLine("Timeout %: " + 100 * iTimedout / (iCalculated + iTimedout));
                    }

                }


                // CLIENTS DO NOT DO TIMEOUTS.
                /* autoCALC doesn't do timeouts anymore.
                fi = new FileInfo(SLAVE_STATUS_STRING_FILE);
                if (false == fi.Exists)
                    {
                    string[] seeds = FetchSeeds( SS_TIMEOUT ) ;
                    int iDoThisMany = seeds.GetLength( 0 ) / 10 ;
                    if (iDoThisMany < 1)
                        iDoThisMany = 1 ;

                    Console.WriteLine("I'm gonna do {0} timeouts now!", iDoThisMany.ToString()) ;
			
                    for(int i = 0; i < iDoThisMany; i++)
                        {
                        m.WaitOne() ;
                        seeds = FetchSeeds( SS_TIMEOUT ) ;
                        string timeoutSeed = null ;
                        if (null != seeds)
                            {
                            timeoutSeed = seeds[0] ;
                            SetSeedStatus( timeoutSeed, SS_CALCULATING ) ;
                            }

                        m.ReleaseMutex() ;
				
                        if (null != timeoutSeed)
                            {
                            Console.WriteLine("") ;
                            Console.WriteLine("Calculating a former timeout: " + timeoutSeed ) ;
                            m_fLiberalCalcTime = true ;
                            m_fOkBruteForceIt = true ;

                            switch( CalcSeed( timeoutSeed ))
                                {
                                case CS_SUCCESS:
                                    SetSeedStatus( timeoutSeed, SS_CALCULATED ) ;
                                    break ;

                                case CS_TIMEOUT:
                                    SetSeedStatus( timeoutSeed, SS_TIMEOUT_TWICE ) ;
                //					throw new Exception() ;
                                    break ;
							
                                default:
                                    MarkError( timeoutSeed ) ;
                                    break ;
                                }
                            m_fOkBruteForceIt = false ;
                            m_fLiberalCalcTime = false ;
                            }
                        }
                    }

                */
                /*
		
                if (null == seeds )
                    {
                    seeds = FetchSeeds( SS_TIMEOUT ) ;
                    if (null != seeds)
                        {
                        string seed = seeds[ 0 ] ;
                        Console.WriteLine("") ;
                        Console.WriteLine("Calculating a former timeout: " + seed ) ;
                        m_fLiberalCalcTime = true ;
                                m_fOkBruteForceIt = true ;
				
                        switch( CalcSeed( seed ))
                            {
                            case CS_SUCCESS:
                                SetSeedStatus( seed, SS_CALCULATED ) ;
                                break ;
                            case CS_TIMEOUT:

						
                                SetSeedStatus( seed, SS_TIMEOUT_TWICE ) ;
                                break ;
                            default:
                                MarkError( seed ) ;
                                break ;
                            }
                                m_fOkBruteForceIt = false ;
				
                        }
                    }
                else
                if (null != seeds )
                    {
                    m_fLiberalCalcTime = false ;

        //			foreach( string seed in seeds )
        // for development, we'll just work on the first item.
                    // do up to two so i can avoid backlogs yet stay cycling.
                    for ( int iItem = 0 ; iItem < 2; iItem++)
                        {
                        string seed = seeds[ iItem ] ; 

                        // If it's a timeout, and it's here, it's cuz it's added=null.  don't ask questions, just bruteforce 
                        if (SS_TIMEOUT == GetSeedStatus( seed ))
                            m_fOkBruteForceIt = true ;

                            {
                            Console.WriteLine("") ;
                            Console.WriteLine("Calculating: " + seed ) ;
                            switch( CalcSeed( seed ))
                                {
                                case CS_SUCCESS:
                                    SetSeedStatus( seed, SS_CALCULATED ) ;
                                    break ;
                                case CS_TIMEOUT:
                                if (m_fDBDown)
                                    {
                                    Console.WriteLine("DB Down.") ;
                                    throw new Exception() ;
                                    }
							
							
                                    SetSeedStatus( seed, SS_TIMEOUT ) ;
                                    break ;
                                default:
                                    MarkError( seed ) ;
                                    break ;
                                }
                            m_fOkBruteForceIt = false ;
					
                            }

                        // only do two if we HAVE two.
                        if (seeds.GetLength(0)  < 2)
                            break ;
                        }
                    }
                    */


                // to eliminate db crashes due to memory,
                // start fresh.
                MasterDB.Close();
                goto RenewDBFresh;
            }
        }











        // Automated does it all.

        /*
        if( args[0].ToUpper() == "-AUTOBITS".ToUpper())
            {
    //		bool fMySMTPIsDead = false ;

            for(;;) // we just loop here forever. BUT ACTUALLY WE USE A GOTO AND THIS MIGHT NOT DO ANYTHING FOR US HERE.
            {

            FileInfo fi = new FileInfo("c:\\temp\\stop.txt");

            if (fi.Exists)
                {
                Console.WriteLine("\a\a\a\a") ;
                return ;
                }

            string UseMe = "c:\\temp\\mylocalfile.txt" ;

            // i will only grab the remote file when my own scraped of null items equals zero again.
                // this was about signups. i'm ditching signups.
            string strCamdy = string.Format("SELECT COUNT(*) FROM SEEDQUEUE WHERE STATUS='SCRAPED' AND Added IS null") ;
            MyNpgsqlCommand camdy = new MyNpgsqlCommand( strCamdy, MasterDB.GetDBConnection() ) ;
            NpgsqlDataReader mayReader = camdy.ExecuteReader() ;
            mayReader.Read ( ) ;

    //		string sfoo = mayReader.GetDataTypeName( 0 ) ;
    //		bool fboo = mayReader.IsDBNull( 0 ) ;
    //	Int64 ijew = mayReader.GetInt64( 0 ) ;
    //		string foodle = mayReader.GetString( 0 ) ;
    //		string fbar = mayReader.GetInt16(0).ToString() ;

            long iCountEmUp = mayReader.GetInt64( 0 ) ;
		
            mayReader.Close() ;

    //		Console.WriteLine("Server query disabled in postgre alpha.") ;
    //		if (false ) 
            if( 0 == iCountEmUp )
                {
                string REQUESTS_CS = "orders/signup.cs" ;// "requests.cs" ;
                long size = GrabRemoteFile( REQUESTS_CS, UseMe) ;
                if (-1 != size)
                    {

                    // parse and see if anyone is new.  add the seeds.
                    // separated by a space, huh.
                    Stream s = File.OpenRead( UseMe  ) ;
                    StreamReader sr = new StreamReader( s );
                    ArrayList alseeds = new ArrayList() ;
                    ArrayList emails = new ArrayList() ;
                    string line = "" ;
                    while( (line = sr.ReadLine()) != null )
                        {
                        if (line.Length > 0)
                            {
                            if( -1 != line.IndexOf(" "))
                                {
                                string seed = line.Substring( 0, line.IndexOf(" ")).ToLower() ;
                                string email = line.Substring( seed.Length + 1 ).ToLower() ;

                                if (-1 == email.IndexOf("@") )
                                    {
                                    Console.WriteLine("Entry " + line + " discarded cuz no valid email address present, really.") ;
                                    }
                                else
                                    {
                                    seed = seed.Replace("-", "_") ;
                                    seed = seed.Replace("'", "") ;

                                    // ha ha no spaces allowed in email addresses, you dork.
                                    if (-1 == email.IndexOf(" "))
                                        {
                                        alseeds.Add( seed ) ;
                                        emails.Add( email ) ;
                                        }
                                    }
                                }
                            }
                        }

                    sr.Close() ;
                    s.Close() ;
				
                    // if this seed is not in the thingie, add it.
                    string[] knownSeeds = FetchSeeds( SS_ALL ) ;
                    for(int iEach = 0; iEach < alseeds.Count; iEach++)
                        {
                        string strNewOrNot = ((string) alseeds[ iEach ]).ToLower() ;
                        bool fSkip = false ;
					
                        for(int iAlready = 0; iAlready < knownSeeds.GetLength( 0 ) ; iAlready++)
                            {
                            if( knownSeeds[ iAlready ].ToLower() == strNewOrNot )
                                {
                                // we think it's known... but we need to see if it's seedless address:
                                    if (false == SeedEmail(strNewOrNot).ToUpper().Contains("SEEDLESS."))
                                    {
                                        fSkip = true;
                                        break;
                                    }
                                    else
                                        break;
                                }
                            }
                        if (false == fSkip)
                            {
        //					Console.WriteLine("I think I detected: " + strNewOrNot) ;

                            NpgsqlConnection sqlc = MasterDB.GetDBConnection() ;

                            string seedAdd = alseeds[ iEach ].ToString().ToLower() ;
                            string emailAdd = emails[ iEach ].ToString().ToLower() ;

                            emailAdd = emailAdd.Replace("'", "") ; // take out the single-quotes.
                            emailAdd = emailAdd.Replace("\\", "") ; // take out the whacks.

                            Console.WriteLine("I think I detected: " + seedAdd + " " + emailAdd ) ;					

                            // because you can only order if your mm doesn't exist online OR the archive bit is unset,
                            // i delete whatever pre-exists for this entry.
                            string strCmd = "DELETE FROM SeedQueue WHERE NAME='" + seedAdd + "'" ;
                            MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, sqlc) ;
                            cmd.ExecuteNonQuery() ;

                            string seedId = "";
                            const string digits = "0123456789abcdefghijkmnopqrstuvwyz";
                            Random rd = new Random();

                            for (int iPos = 0; iPos < 4; iPos++)
                            {
                                seedId += digits[rd.Next() % digits.Length];
                            }

    //                        string str = string.Format("UPDATE seedqueue SET authid='{1}' where Name='{0}'", args[0], seedId);
     //                       MyNpgsqlCommand cmd = new MyNpgsqlCommand(str, MMDB.DBConnection);
      //                      cmd.ExecuteNonQuery();

						
                            strCmd = string.Format("INSERT INTO SeedQueue (Name, EMail, Added, authid) Values('{0}', '{1}', NULL, '{2}' )",  // GETDATE()
                                                        seedAdd,
                                                        emailAdd,
                                                        seedId) ;
                            cmd = new MyNpgsqlCommand(strCmd, sqlc) ;
                            cmd.ExecuteNonQuery() ;
                            // refresh this fucker! ok this causes crashes. two so far... needs to be indexed sorta somehowz.
                            knownSeeds = FetchSeeds( SS_ALL ) ;
                            }
                        }
				

                    // do the secure file removal
                    XF2ServiceLjmindmapCom xff = new XF2ServiceLjmindmapCom () ;
                    try
                        {
                    xff.DeleteIfUngrown( GetXferPwd(), REQUESTS_CS, size ) ;
                        }
                    catch( System.Net.WebException )
                        {
                        // ignore it.  It's just low bw condition. This file gets killed eventually and duplication doesn't hurt me.
                        }
                    }
                }

            // Do we have any satellite scrapes?
    //		/* I want to make this work for any number of clients.

            string[] customClientScrapeStrings = {  
            // "SCRAPED_BONKERS" 
    //		"SCRAPED_MIGUEL", 
    // "SCRAPED_SHH" 
            } ;

            for(int iClient = 0; iClient < customClientScrapeStrings.GetLength( 0 ) ; iClient++ )
                {
                LookAgainForThinky:
                string[] sa = FetchSeeds( customClientScrapeStrings[iClient] ) ;

                // are any older than an hour?
                if (null != sa)
                    { 
                    for(int i=0 ; i < sa.GetLength(0); i++)
                        {
                        DateTime dt = FetchStatStamp( sa[i] ) ;
                        if (dt.AddHours( 1 ) < DateTime.Now )
                            {
                            SetSeedStatus( sa[i], SS_SCRAPED ) ; // return to pool.
                            goto LookAgainForThinky ;
                            }
                        }
                    }

                // If we have less than three, assign three more.
                if ((null == sa) || sa.GetLength(0) < 3)
                    {
                    // Here we need exclusive power...
                    Mutex m = new Mutex(false, CONSUME_SCRAPED_QUEUE) ;
                    m.WaitOne() ;

                    for(int iNew = 0; iNew < 3; iNew++)
                        {
                        string[] seedsQ = FetchSeeds( SS_SCRAPED ) ;
                        if (null != seedsQ )
                            {
                            for(int iEach = 0 ; iEach < seedsQ.GetLength(0); iEach++)
                                {
                                string seed = seedsQ[ iEach ] ;
                                if (false == MasterDB.IsMoneyBit( seed ))
                                    {
                                    SetSeedStatus( seed, customClientScrapeStrings[iClient] ) ;
                                    break ;
                                    }
                                }
                            }
                        }

                    // ok everyone, you can run again.
                    m.ReleaseMutex() ;
                    }
                }
 
            // i want a summary as i go:
            int iTotal = 0 ;
            int iQueued = 0 ;
            string[] seeds = FetchSeeds( SS_QUEUED ) ;
            if (null != seeds) 
                {
                iQueued = seeds.GetLength( 0) ;
                iTotal += iQueued  ;
                Console.WriteLine("There are {0} seeds queued, awaiting scrape.", iQueued ) ;
			
                }

            int iScraped = 0 ; 

            seeds = FetchSeeds( SS_SCRAPED ) ;
            if ( null != seeds )
                {
                iTotal += seeds.GetLength( 0 ) ;
                iScraped = seeds.GetLength( 0 ) ;
                Console.WriteLine("There are {0} seeds scraped, awaiting calculation.", seeds.GetLength( 0 ) ) ;
                }

    //		SELECT COUNT(*) FROM SEEDQUEUE WHERE STATUS='SCRAPED' AND Added IS null
            string strCmdy = string.Format("SELECT COUNT(*) FROM SEEDQUEUE WHERE STATUS='SCRAPED' AND Added IS null") ;
            MyNpgsqlCommand cmdy = new MyNpgsqlCommand( strCmdy, MasterDB.GetDBConnection() ) ;
            NpgsqlDataReader myReader = cmdy.ExecuteReader() ;
            myReader.Read ( ) ;
            long iCountEm = myReader.GetInt64(0) ;
            myReader.Close() ;
            if( 0 != iCountEm )
                Console.WriteLine("Scraped items with an Added of null: " + iCountEm) ;

            seeds = FetchSeeds( SS_TIMEOUT ) ;
            if (null != seeds )
                {
                iTotal += seeds.GetLength( 0 ) ;
                Console.WriteLine("There are {0} seeds marked as timeout, awaiting brute force approach.", seeds.GetLength( 0 ) ) ;

                string strCmdydoo = string.Format("SELECT COUNT(*) FROM SEEDQUEUE WHERE STATUS='TIMEOUT' AND Added IS null") ;
                MyNpgsqlCommand cmdydoo = new MyNpgsqlCommand( strCmdydoo, MasterDB.GetDBConnection() ) ;
                NpgsqlDataReader myReaderdoo = cmdydoo.ExecuteReader() ;
                myReaderdoo.Read ( ) ;
                long iCountEmdoo = myReaderdoo.GetInt64(0) ;
                myReaderdoo.Close() ;
                if( 0 != iCountEmdoo )
                    Console.WriteLine("Timeout items with an Added of null: " + iCountEmdoo) ;
			
                }

            seeds = FetchSeeds( SS_CALCULATED ) ;
            if (null != seeds )
                {
                iTotal += seeds.GetLength( 0 ) ;
                Console.WriteLine("There are {0} seeds calculated, awaiting upload.", seeds.GetLength( 0 ) ) ;
                }

            seeds = FetchSeeds( SS_UPLOAD_CONFIRMED ) ;
            if (null != seeds )
                {
                iTotal += seeds.GetLength( 0 ) ;
                Console.WriteLine("There are {0} seeds confirmed, awaiting notification.", seeds.GetLength( 0 ) ) ;

                string strCmdycane = string.Format("SELECT COUNT(*) FROM SEEDQUEUE WHERE STATUS='CONFIRMED' AND Added IS null") ;
                MyNpgsqlCommand cmdycane = new MyNpgsqlCommand( strCmdycane, MasterDB.GetDBConnection() ) ;
                NpgsqlDataReader myReadercane = cmdycane.ExecuteReader() ;
                myReadercane.Read ( ) ;
                iCountEm = myReadercane.GetInt64(0) ;
                myReadercane.Close() ;
                if( 0 != iCountEm )
                    Console.WriteLine("Pending notification with an Added of null, impeding timeliness: " + iCountEm) ;
                }

            strCmdy = string.Format("SELECT COUNT(*) FROM SEEDQUEUE WHERE STATUS='COMPLETED' AND STATUSCHANGED >= GETDATE() - interval '2 hours'") ;
            cmdy = new MyNpgsqlCommand( strCmdy, MasterDB.GetDBConnection() ) ;
            myReader = cmdy.ExecuteReader() ;
            myReader.Read ( ) ;
            iCountEm = myReader.GetInt64(0) ;
            myReader.Close() ;
            Console.WriteLine("Notifications per hour: " + iCountEm / 48) ;

    //		Console.WriteLine("THE METRIC OF SUCCESS I LOVE TO TRACK IS: {0} SEEDS IN THE PIPELINE.", iTotal) ;

            // 5) Fetch a seed awaiting confirmation and check it out.
            seeds = FetchSeeds( SS_HISTORY_COOKED ) ;
            if ( null != seeds)
                {
                foreach( string seed in seeds )
                    {
                    Console.WriteLine("") ;

                    Console.WriteLine("ConfirmingUpload: " + seed ) ;
                    if ( UploadConfirmed( seed ))
                        SetSeedStatus( seed, SS_UPLOAD_CONFIRMED ) ;
                    else
                        break ; // we don't check any more if we fail.
                    }
                }
		
            // 2) Get a queued seed.
    //		/* SHUT DOWN SCRAPING AND KICK OUT THIS TIMEOUT PHENOM
            seeds = FetchSeeds( SS_QUEUED ) ;
            if (null != seeds )
                {
    //			foreach( string seed in seeds )
            // during development, instead of iterating all these, we just do the first.
                if (seeds.GetLength( 0 ) > 0)
                    {
                    string seed = seeds[ 0 ] ;
		

                    Console.WriteLine("") ;
                    Console.WriteLine(" Scraping: " + seed ) ;
                    if (ScrapeSeed(seed))
                    {
                        SetSeedStatus(seed, SS_SCRAPED);
                        // 
                        // right here we produce the history xml file
                        // i believe it relies on both experimental fdata_pg and linq for xml
                        // linq for xml is a killer app... 
                    }
                    else
                        SetSeedStatus(seed, SS_SCRAPE_FAILED); // MarkError( seed ) ; // an extra field that assures this is skipped.
                    }
                }

            Console.Write("Sleeping... ") ; // give it something (999) to look through with cores.

            // for our emailing hour to move along with less waste.
            iScraped -= 100 ; 
            if (iScraped < 0)
                iScraped = 10 ; // come on, some delay please.

            // if it's email hours, and if there are emails to send, then we delay just ten seconds.


            MasterDB.Close() ;
		
            Thread.Sleep( iScraped * 1000) ; } 
            // else { Console.WriteLine("TinyNap!") ; Thread.Sleep( 5000 ) ; }
            // if there are no seeds queued, take a real nap
    //		if (iQueued == 0) 		// it's likely we have old guys to update.
    //			Thread.Sleep( 60 * 1000 ) ;
		
            Console.WriteLine("Slept!") ;
            goto RenewDBFresh ;

            }
            }
        */

        /*

        bool skimMode = false ;
        if( args[0].ToUpper() == "-AUTOMAIL_SKIM".ToUpper())
            {
            // skim's goal is to ONLY send added=null (priority) mails.
            skimMode = true ;
            }


        if( (args[0].ToUpper() == "-AUTOMAIL".ToUpper()) || (args[0].ToUpper() == "-AUTOMAIL_SKIM".ToUpper()))
            {
    StartAgainSkimmer:		
            for(int iDoThisMany = 0; iDoThisMany < 20; iDoThisMany++)
                {
                string[] seeds = FetchSeeds( SS_UPLOAD_CONFIRMED ) ;
			
                if( null == seeds )
                    return ;  // i mean... right?
			
                if ( null != seeds )
                    {
                    int iThisMany = 0 ;
				
                    foreach( string seedy in seeds )
                        {
                        if( skimMode)
                            {
                            // is this seedy added null?
                            NpgsqlConnection sqlc = MasterDB.GetDBConnection() ;
                            string strCmd = string.Format("select Added from SeedQueue where Name='{0}'", seedy) ;
                            MyNpgsqlCommand cmd = new MyNpgsqlCommand( strCmd, sqlc ) ;
                            NpgsqlDataReader myReader = cmd.ExecuteReader() ;
                            myReader.Read () ;
                            try
                                {
                                if (false == myReader.IsDBNull(0) )
                                    {
                                    return ;
                                    }
                                }
                            finally
                                {
                                myReader.Close() ;
                                }
                            }
					
                        // ONE hours need to pass in the CONFIRMED state so we're sure it's really the latest image.
                        DateTime dt = FetchStatStamp( seedy ) ;
                        if( dt > DateTime.Now.AddHours( -1 ) )
                            {
                            if( skimMode )
                                {
                                // in skim mode, we know we've got an added=null, and if it's in blackout period,
                                // then we just need to sleep.
                                Console.WriteLine("Sleeping 30 minutes to retry skim later.") ;
                                Thread.Sleep( 60 * 1000 * 30 ) ;
                                goto StartAgainSkimmer ;
                                }
                            continue ;
                            }

                        // if this isp has been sent to within the last minute, we're gonna move on.
                        string strTargEmail = SeedEmail(seedy ) ;
                        string isp = strTargEmail.Substring( strTargEmail.IndexOf("@" )).ToLower() ;
                        DateTime dtLastUsed ;
                        try
                            {
                            int iSkipped = 1 ;
                            try { iSkipped = skippedOfISP[ isp ] ; }
                            catch( System.Collections.Generic.KeyNotFoundException ) 
                                { 
                                skippedOfISP[ isp ] = 1 ;
                                }
                            if( iSkipped < 1)
                                iSkipped = 1 ;
						
                            dtLastUsed = lastOutOfISP[ isp ] ;
                            int iSeconds = -5 * 60 / iSkipped ;
    //						Console.WriteLine("Delay for " + isp + ":" + (-iSeconds).ToString() + " skipped: " + iSkipped ) ;
                            if( dtLastUsed > DateTime.Now.AddSeconds( iSeconds ))
                                {
                                Console.Write( ".") ; // so we know when one isp is very backed up in our queue.
                                skippedOfISP[ isp ]++ ;
                                continue ;
                                }
                            }
                        catch( System.Collections.Generic.KeyNotFoundException )
                            {
                            // falls through...
                            }

                        lastOutOfISP[ isp ] = DateTime.Now ;
                        skippedOfISP[ isp ]-- ;

                        Console.WriteLine("") ;
                        Console.WriteLine("SendingEmail:                              " + seedy ) ;

                        // if this is to gmail or hotmail, we can send using my isp. faster and more informative.
                        Console.WriteLine("                                           " + strTargEmail ) ;

                        // does this seed have a unique id value?
                        NpgsqlConnection sqlcon = MasterDB.GetDBConnection() ;
                        string strCmdy = string.Format("select authid from SeedQueue where Name='{0}'", seedy) ;
                        MyNpgsqlCommand cmdy = new MyNpgsqlCommand( strCmdy, sqlcon ) ;
                        NpgsqlDataReader myReadery = cmdy.ExecuteReader() ;
                        myReadery.Read () ;
                        string seedId = "";

                        if (true == myReadery.IsDBNull(0) )
                            {
                            // if i get a null, i'm doing it wrong. these are created at insertion
                                Debug.Assert(false); // nevar!
                            }
                        seedId = myReadery.GetString(0).Trim();

                        myReadery.Close() ;

                        // if fEmailCycle is null, then we have to randomly kick it off.
      

                        // if they have no Id, then I create one for them.
                        if ("" == seedId)
                        {
                            Debug.Assert(false);
                            // i took out l and x cuz those will designate my smtp sender.
                            const string digits = "0123456789abcdefghijkmnopqrstuvwyz" ;
                            Random rd = new Random();

                            for (int iPos = 0; iPos < 4; iPos++)
                            {
                                seedId += digits[ rd.Next() % digits.Length ] ;
                            }

                            // clobber first char with provider indivator
                 

                            string str = string.Format("UPDATE seedqueue SET authid='{1}' where Name='{0}'", seedy, seedId);
                            MyNpgsqlCommand cmd = new MyNpgsqlCommand(str, sqlcon);
                            cmd.ExecuteNonQuery();
                        }

    //					"Please e-friend my brother and I'll give you color overnight.\r\n   http://www.livejournal.com/friends/add.bml?user=peristaltor" ) ; // reply with your compliments and complaints.");
    //									"http://ljmindmap.com/h.aspx?n=" + seed + "\r\n\r\nPlease e-friend my brother and I'll give you color overnight.\r\n   http://www.livejournal.com/friends/add.bml?user=peristaltor") ; // reply with your compliments and complaints." ) ;

    //					SmtpClient client = new SmtpClient("smtp.xeriom.net") ;
    //					client.Host = "smtp.xeriom.net" ;
    //					client.Credentials = new System.Net.NetworkCredential("mcfnord@ljmindmap.com", "gr33n1") ;
    //					SmtpClient client = new SmtpClient("80.68.46.115") ;
    //					client.Host = "80.68.46.115" ;
    //					SmtpClient client = new SmtpClient("80.68.46.77") ; // 80.68.46.77
    //					client.Host = "80.68.46.77" ;

           

                        // choose a client based on email cycle
                        SmtpClient client ;

    //                    if (fEmailCycle == true)
                        {
    //                        fEmailCycle = false;

                            client = new SmtpClient("johnd.vm.xeriom.net");
                            client.Host = "johnd.vm.xeriom.net";
                            client.Credentials = new System.Net.NetworkCredential("johnd", "p0ypl3r");
                            client.Port = 2525; // yeah baby
                            Console.WriteLine("Sending through xeriom.");
                        }
             

                        MailMessage message = new MailMessage("mcfnord@ljmindmap.com", strTargEmail, "Your MindMap is Ready!",
                        "http://ljmindmap.com/h.php?n=" + seedy + "&i=" + seedId +
                            //                    "\r\n\r\nGet the MindMap Supreme with Lovely Ladies on Beautiful Bikes 2008 calendar." + 
                        "\r\n\r\nreply with your compliments and complaints.");


                        client.Send(message);
                        SetSeedStatus( seedy, SS_COMPLETED ) ;

					

                        int iPause = 4000 / seeds.GetLength(0) ; // some constant / # notifications queued, so i move slower as it empties.

                        // we're going ten seconds, period.
                        if (iPause < 10)
                            iPause = 10 ;
					
                        Console.WriteLine(" Nap: " + iPause ) ;
                        Thread.Sleep( iPause * 1000 )  ; 

                        iThisMany++ ;
                        if (iThisMany > 5)
                            break ;
                        }
                    }
                }
                MasterDB.Close() ;
			
                Console.WriteLine("Slept!") ;
                goto RenewDBFresh ;
            }
						
                        // this code used to run
    //					if (fUseMine) //  && false == fMySMTPIsDead )
        */


        if (args[0].ToUpper() == "-ANIMATE".ToUpper())
        {
            m_fLiberalCalcTime = true;

            if (CS_SUCCESS == CalcHistoricSeed(args[1]))
            {
                NpgsqlConnection sqlc = MasterDB.GetDBConnection();
                string strCmd = string.Format("UPDATE SeedQueue SET Status = '{0}' WHERE Name = '{1}'", SS_CALCULATED, args[1]);
                MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, sqlc);
                cmd.ExecuteNonQuery();
            }

            MasterDB.Close();
            m_fDudeImDone = true; // causes sleeping thread to return soon enough, so we can close.
            return;
        }




        if (args[0].ToUpper() == "-GRAB_FROM_SERVER".ToUpper())
        {
            // grab from server mode retrieves cooked tribes from the server,
            // and does ShowTribe, which renders them and also puts them
            // into the database.
            // don't loop		for( ;; )
            {
                string[] dudes = new string[1];
                int[] tiers = new int[1];

                /*
			MindMapNet mmn = new MindMapNet() ;
			if (mmn.DownloadTribe( ref dudes, ref tiers ))
				{
// prepature					if (false == mmn.CullBySeed( SYSTEM_PASSWORD, "DONE_TRIBE", dudes[0] ) )
//						Console.WriteLine("ERROR from EXPUNGE.") ;
				Console.WriteLine("Got one.") ;

					LJUser2 lju = MasterDB.GetSlimUser( dudes[0] ) ;
					// now this dude has a new tribe calculated.
					// the dreadful format of this data lives on!
					lju.tribe = new ArrayList() ;
					int iTier = -1 ;
					ArrayList thisLayer =  null ;
					for( int iEach = 0; iEach < dudes.GetLength( 0 ) ; iEach++)
						{
						if (iTier != tiers[ iEach ])
							{
							if (iTier != -1)
								{
								// we add that wacky extra layer of indirection!
								ArrayList extraLayer = new ArrayList()  ;
								extraLayer.Add( thisLayer ) ;
								lju.tribe.Insert( 0, extraLayer ) ;
								}

							thisLayer = new ArrayList() ;
							// we want to skip blank tiers accurately.
//							iTier-- ;
							iTier = tiers[ iEach ] ; // we figure the seed is truly a top tier dude.
							}
						thisLayer.Add( dudes[ iEach ] ) ;
						}
					// it's always necessary to add the last layer
					ArrayList lastExtraLayer = new ArrayList() ;
					lastExtraLayer.Add( thisLayer ) ;
					lju.tribe.Insert( 0, lastExtraLayer ) ;

// i no save until i like					MasterDB.Add( lju, true, olCustomUserList ) ;

					// finally, in a fit of crap, i have to abstract these into numeric represnetaionts within olcustomuserlist

					// ONCE I CALL TEXTTRIBETONUMERIC, THE OLCUSTOMUSERLIST IS FIXED AND CANNOT BE CHANGED!
					lju.tribe = TextTribeToNumeric( lju.tribe ) ;

					ShowTribe( lju, null ) ;
					
					return ;
				}
                 * */

            }
        }

        // sentinel mode runs until I gracefully tell it to shut down.
        // It delivers requested content as fast as possible.
        // First it examines queue.txt on the server.
        // If there are any users there who are not seeds,
        // then we generate and publish them.
        // we do this until we run out of requests.
        // at which time we generate and publish a seedmap.
        // finally, we save dirtybits (all bits).
        // when there is nothing to do, we sleep 5 minutes.
        // i want to back up and cannot so I am going to fix that now.

        /*
            if (args[0].ToUpper() == "-SENTINEL".ToUpper())
                {
                Console.WriteLine("Starting Sentinel mode.") ;
		
                for(;;)
                {
                    bool fDirty = false ;
			
                    string url = string.Format("http://www.gal2k.com/mindmap/queue.txt") ;
                    WebRequest request = WebRequest.Create( url );	
                    bool fSuccess = false ;
                    Stream s = null ;
                    try
                        {
                        WebResponse response = request.GetResponse();
                        s = response.GetResponseStream();
                        fSuccess = true ;
                        }
                    catch( Exception )
                        {
                        // presumably this is file not found (404) but it kind of doesn't matter to me what it is.
                        // what matters is the value of fSuccess.
                        }
                    if (fSuccess)
                        {
                        StreamReader sr = new StreamReader( s );
                        string requestedUser  ;
                        while( (requestedUser = sr.ReadLine()) != null )
                            {
                            // is this user a seed?  they are if they have a tribe member.
                            LJUser2 ljuSeed = MasterDB.GetUser( requestedUser ) ;
                            if ((null == ljuSeed) || (ljuSeed.tribe == null))
                                {
                                string [] seedArray = { requestedUser } ;
                                Console.WriteLine("Now I calculate: {0}", requestedUser ) ;
                                if (CalculateSingleSeed( seedArray ))
                                    {
                                    // FOR NOW WE ARE GOING TO DO THIS MUCH MORE FREQUENTLY.
                                    // AS WE CRASHED AND LOST DATA.
                    //				SaveMasterUserList() ;					
                                    fDirty = true ;
                                    }
                                }
                            }
                        }
        //			catch( Exception )
        //				{
                        // probably the queue.txt does not exist.  Anyway, we ignore that and proceed.
        //				}

                    // We save
                    if (fDirty)
                        {
        //				SaveMasterUserList() ;
                        Console.WriteLine("SeedMapping...") ;
                // UNTIL THE DATASET IS FULL, WE DON'T PUBLISH THE INCOMPLETE SEED LIST!
                // GET THAT FULL DATASET ONLINE!
        //				BuildAndPublishXmlMapFile() ;

                        }
                    else
                        {
                        // Sleep for ONE minutes.
                        // just TOO MANY REQUESTS to sleep longer.
				
                        Console.WriteLine("Sleeping...") ;
                        Thread.Sleep( 1000 * 60 * 1 ) ;
                        Console.WriteLine("Waking up...") ;
                        }
                }

        //		return ; // we done for now.
                }
                */

        if (args.GetLength(0) == 0)
        {
//            BuildMasterMap();
            return;
        }

        /*
        if (args[0].ToUpper() == "-seedmap".ToUpper())
            {
            BuildAndPublishXmlMapFile() ;
		
            return ;
            }
         * */

        /*
            if (args[0].ToUpper() == "-serverfiles".ToUpper())
                {
                BuildAndPublishImageClickMapFile() ;
                BuildAndPublishXmlMapFile() ;
		
                return ;
                }
                */


        if (args[0].ToUpper() == "-regen_maps".ToUpper())
        {
            // find all tribe seeds and recalculate them.
            // we get the db for this.
            NpgsqlConnection sqlc = MasterDB.GetDBConnection();
            //		strCmd = string.Format("select LJUser from WhoIRead where Name='{0}'", requestedUser) ; 
            string strCmd = "select DISTINCT Name from Tribes";
            MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, sqlc);
            NpgsqlDataReader myReader = cmd.ExecuteReader();
            ArrayList alOurSeeds = new ArrayList();

            while (myReader.Read())
            {
                alOurSeeds.Add(myReader.GetString(0).Trim());
            }
            myReader.Close();

            foreach (string strname in alOurSeeds)
            {
                Console.WriteLine(strname);
                Console.WriteLine("Starting seed: " + DateTime.Now.ToString());
                ///			string [] strParm = { strname } ;

                // perhaps it's not even in the seedqueue!
                if (null != CalculateSingleSeed(strname))
                    SetSeedStatus(strname, SS_CALCULATED);

                Console.WriteLine("Finished seed: " + DateTime.Now.ToString());
            }

            MasterDB.Close();
            return;
        }



        Console.WriteLine("MAKE SURE THE CASE IS EXACTLY THE WAY YOU WANT IT FOR THE USERNAME.");
        Console.WriteLine("Starting seed: " + DateTime.Now.ToString());
        m_fLiberalCalcTime = true;


        // For testing purposes.
        //	m_fOkBruteForceIt = true ;

        if (null != CalculateSingleSeed(args[0]))
        {
            // it's a data protection measure to scan for the completed seed in the seedqueue, setting state to CALCULATED
            NpgsqlConnection sqlc = MasterDB.GetDBConnection();
            string strCmd = string.Format("UPDATE SeedQueue SET Status = '{0}' WHERE Name = '{1}'", SS_CALCULATED, args[0]);
            MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, sqlc);
            cmd.ExecuteNonQuery();
        }

        Console.WriteLine("Finished seed: " + DateTime.Now.ToString());

        MasterDB.Close();
        m_fDudeImDone = true; // causes sleeping thread to return soon enough, so we can close.
        // I do this elsewhere now.
        //	Console.WriteLine("Indidivual user map completed.  Data saved.") ;
    }



    /*
    static ArrayList TextTribeToNumeric(ArrayList textTribe)
    {
        // to show the tribe, we have to populate the olCustomUserList, and then 
        // convert the tribe to its numeric format.

        ArrayList ljuTribeNEW = new ArrayList();
        foreach (ArrayList alThisLevel in textTribe)
        {
            ArrayList alThisLevelNEW = new ArrayList();

            foreach (ArrayList thisSet in alThisLevel)
            {
                ArrayList thisSetNEW = new ArrayList();
                foreach (string strItem in thisSet)
                {
                    bool fAlready = false;
                    foreach (LJUser2 ljuIsMe in olCustomUserList)
                    {
                        if (ljuIsMe.Name.ToUpper() == strItem.ToUpper())
                        {
                            fAlready = true;
                            break;
                        }
                    }

                    if (false == fAlready)
                    {
                        LJUser2 ljuClone = new LJUser2();
                        LJUser2 ljuOriginal = null;
                        ljuOriginal = MasterDB.GetSlimUser(strItem);
                        if (null == ljuOriginal)
                            throw new Exception();

                        ljuClone.Clone(ljuOriginal);
                        olCustomUserList.Add(ljuClone);
                    }

                    // so each item is in one slot in olCustomUserList
                    // and i need to convert the whole messy monster to numerics.
                    /////////////////////////////////////////////////////////////
                    // ONCE YOU REFER TO A USERNUMBER, YOU CAN'T CHANGE THE ORDER OF THE OLCUSOMUSERLIST AFTER THAT POINT
                    /////////////////////////////////////////////////////////////
                    thisSetNEW.Add(olCustomUserList.GetUserNumber(strItem));
                }
                alThisLevelNEW.Add(thisSetNEW);
            }
            ljuTribeNEW.Add(alThisLevelNEW);
        }

        return ljuTribeNEW;
    }
     * */


    static public void PerhapsAddCity(LJUser2 lju)
    {
        // do we already have tooltip?
        if (lju.Location != null)
            //		if (lju.Location != "")
            return;

        string city = FData.GetCity(lju.Name);
        if (city.Length > 0)
            lju.Location = city;
        /*

        LJUser2 ljuReturned = getUserFromWeb( lju.Name ) ;

        if (null != ljuReturned)
            {

            // Our responsibility to save data.

            string strLoc = ljuReturned.Location ;
            if(strLoc != null)
                if (strLoc.Length > 79)
                    strLoc = strLoc.Substring( 0, 79 ) ;

            lju.Location = strLoc ;
            // it may be "" but it's not NULL anymore.  so we don't re-query with "".
            MasterDB.AddCooltip( lju.Name, lju.Location ) ;
            }
         * */
    }


    static public ArrayList CalculateSingleSeed(string seed)
    {
        return CalculateSingleSeed(seed, -1, null, null);
    }

    static public ArrayList CalculateSingleSeed(string seed, int iDate, ArrayList alFinallPlacements, ArrayList alPrevFramePlacements)
    {
    TryAgainJock:

//        if (m_fOkBruteForceIt)
  //          Console.WriteLine("USING BRUTE FORCE.");

        /* CONFLICTS WITH TITLE SYSTEM
            // if we enter two names or more, then we want a sub-master-map
            // which only includes these tribes.  so we see if these tribe seeds
            // have all been created.  if not, that's an error.
            if (args.GetLength(0) > 1)
                {
                foreach( string str in args )
                    {
                    LJUser2 ljuSeed = olMasterUserList.GetUser( str ) ;
                    if ( ljuSeed == null || ljuSeed.tribe == null )
                        {
        //				fFailed = true ;
                        Console.WriteLine("User not calculated as seed: {0}", str ) ;
                        }
                    }
                return ;
		
                }
                */

        LJUser2 lju = getUser(seed);
        if (null != lju)
            olCustomUserList.Add(lju);

        //	foreach( string strBug in lju.whoIRead )
        //		Console.WriteLine("I read: {0}", strBug) ;

        if (lju == null)
            return null;

        lju.Name = seed; // args[0] ; // must retain for case correctness.
        

        // for my test, always re-calc mcfnord tribe.
        if (lju.Name == "mcfnord")
            lju.tribe = null; // force recalc
        //	if  (lju.Name == "themindofjess") 
        //		lju.tribe = null ;
        //	if  (lju.Name == "purgatorius") 
        //		lju.tribe = null ;


        // sometimes people request twice.  We're going to calc twice for these people.  It's a bit dumb but 
        // it's also my #1 bug and I can't wait til it's gone.
        // the issue is that the tribe layout has already been converted to numerics, and that i think
        // the actual numbers are suspect.  so trash it if it's already in numerics.
        // we'll just recalc, crazy as that really is.
        if (lju.tribe != null)
        {
            foreach (ArrayList alThisLevel in lju.tribe)
            {
                foreach (ArrayList thisSet in alThisLevel)
                {
                    object o = thisSet[0];
                    if (o is Int32)
                        lju.tribe = null;
                    // i guess it's not a numeric tribe.  so we keep it.
                    break;
                }
                break;
            }
        }


        if (lju.tribe != null)
        {
            Debug.Assert(false);// i bet this is dead code. but i'm not sure!
            if (m_fJustUploadRawSeed)
            {
                Console.WriteLine("This seed is already calculated into a tribe.  There's no need to proceed.");
                return null;
            }
            // ONCE I CALL TEXTTRIBETONUMERIC, THE OLCUSTOMUSERLIST IS FIXED AND CANNOT BE CHANGED!
            //            lju.tribe = TextTribeToNumeric(lju.tribe);

            string[] args = { seed };
            ArrayList alRet = ShowTribe(lju, args);
            if (m_fTryTribeAgain)
            {
                m_fTryTribeAgain = false;
                goto TryAgainJock;
            }

            return alRet;
        }

        ArrayList olFriends = new ArrayList();

        var whoIRead = from dude in lju.ifd where dude > 0 select dude;
        foreach (int istr in whoIRead )
        {
            string str = IDMap.IDToName(istr);
            int iAttempts = 0;

            while (iAttempts < 3)
            {
                LJUser2 ljuDude = getUser(str);
                if (null == ljuDude)
                {
                    iAttempts++;
                    continue;
                }
                olFriends.Add(ljuDude);
                break;
            }
        }

        // Ok, I have the target and all that target reads, and who THEY read.
        // Based on this information, what stab could I take at calculating the tribe?
        // it starts with this user.

        // build a list of two-way connections between the center
        HashSet<Int32> twoWayReadership = new HashSet<int>(); //  new ArrayList();
        Console.WriteLine("Seed's whoIRead has " + whoIRead.Count() + " members.");

        //	/* the following code is just slow and so I'm going to hack out a faster approach.

        foreach (int istr in whoIRead)
        {
            LJUser2 ljLook = MasterDB.GetSlimUser(IDMap.IDToName(istr));
            if (ljLook != null)
            {
                // We put the user into our list.  But perhaps later we take user out?  How odd.
                olCustomUserList.Add(ljLook); // add them all.  unsure if there's an alternative.
                //									Console.WriteLine("Adding AGAIN: {0}", ljLook.Name ) ;


                if (ljLook.Reads(lju.Name))
                {
                    twoWayReadership.Add(istr);
                }
                else
                    Console.Write(" " + ljLook.Name + " does not read " + lju.Name + " ");
            }
            else
                Console.Write(" " + IDMap.IDToName(istr)  + " not found in database.");
        }
        //	*/	

        // if there are zero readers in twoway, we call this a failure.
        if (twoWayReadership.Count == 0)
        {
            Console.WriteLine("Screwy world.  Nobody reads this dude.  Could be a rename.");
            return null;
        }

        // It's probabl eht a bug has been introduced due to the fact the seed is not necessarily
        // in this twoWay list.  so add the seed if not there yet.
        bool fSeedIsIn = false;
        foreach (int istrTwoWay in twoWayReadership)
        {
            if (lju.ifd.Contains( istrTwoWay ))
            {
                fSeedIsIn = true;
                break;
            }
        }
        if (false == fSeedIsIn)
            twoWayReadership.Add(IDMap.NameToID( lju.Name));

        // HERE'S AN AWESOME OPTIMIZATION, JACK:
        // My olCustomUserList now contains all the user objects that will go into this dataset.
        // but these objects also specify, in their WhoIRead, all their readers. 
        // So I am frequently examining users in these lists who are not germane to this tribe map.
        // but here in this olCustomUserList, I can create clones with subgrouped read lists.  All remaining 
        // calculations can work off these
        // I COULD MODIFY THE UNDERLYING OBJECTS HERE, but I opt to create new clones, as it's
        // bad form to party on objects.  I should const them from GetUser.  But I don't yet.
        // THIS IS SERIOUS DANGER!
        // DANGER!
        // I CAN'T CHANGE OLCUSTOMUSERLIST ONCE I'VE CALLED ANY NUMBERIC FUNCTION!

        CUserList newSubList = new CUserList();
        foreach (LJUser2 ljuDoYouAddMe in olCustomUserList)
        {
            //		LJUser2 subUser = new LJUser2() ;
            //		subUser.Clone( ljuOne ) ;
            HashSet<Int32> newWhoIRead = new HashSet<Int32>();
            // only add the user IF they have a two-way readership relationship
            // with anyone else in our dataset!
            //		newSubList.Add( subUser ) ;

            foreach (Int32 istrIRead in whoIRead)
            {
                foreach (int istrAUserThatMatters in twoWayReadership)
                {
                    if (istrIRead == istrAUserThatMatters) //  strIRead.ToUpper() == strAUserThatMatters.ToUpper())
                    {
                        bool fAddedAlready = false;
                        foreach (int istrAlready in newWhoIRead)
                        {
                            if (istrAlready == istrAUserThatMatters)
                            {
                                fAddedAlready = true;
                                break;
                            }
                        }
                        if (false == fAddedAlready)
                            newWhoIRead.Add(istrAUserThatMatters);
                    }
                }
            }
            if (newWhoIRead.Count > 0)
            {
                whoIRead = newWhoIRead;
                newSubList.Add(ljuDoYouAddMe);
            }
        }
        olCustomUserList = newSubList;

        // A final re-ordering.
        // Once we refer to a numeric position, we can't change the order (or contents) of the olCustomUserList.
        // Therefore, this function runs "twice".  The first time, it merely signals which numeric readers exist.
        // The second time, after culling those users who have no numeric readers, it populates the numeric field properly.
        // SO look for this code twice in a row here...

        foreach (LJUser2 ljuToNum in olCustomUserList)
        {
//            ljuToNum.whoIReadNumeric = new ArrayList();
            var whoIReadNows = from u in ljuToNum.ifd where u > 0 select u;
            foreach (int istrNam in  whoIReadNows)
            {
                // OPTIMIZATION: I am only adding the numeric reader here IF THEY READ ME BACK.
                // THEY MIGHT NOT EVEN BE IN THIS DATASET, as it's a subset of seed readers.
                /* trash it
                LJUser2 ljuThem = olCustomUserList.GetUser(IDMap.IDToName(istrNam));
                if (null != ljuThem)
                    if (ljuThem.Reads(ljuToNum.Name))
                        ljuToNum.whoIReadNumeric.Add("placeholder");
                 * */
            }
        }

        //////////////////////////////////////////////////////

        // no we do it for keeps.  we remove people who have no reader relationships in this seed's subset

        CUserList newList = new CUserList();
        foreach( var guy in olCustomUserList)
            
        foreach (LJUser2 ljuFindZero in olCustomUserList)
        {
            var guysIRead = from aguy in ljuFindZero.ifd where aguy > 0 select aguy;
            if (guysIRead.Count() > 0) 
                newList.Add(ljuFindZero);
        }

        olCustomUserList = newList;

        ////////////////////////////////////
        ////////////////////////////////////
        // DO MY CRAZY THING.  JUST AS A TEST NOW.  CLOBBER TWOWAYREADERSHIP AND OLCUSTOMUSERLIST.

        // SAVE THE ORIGINALS.
        CUserList originalList = olCustomUserList;
        HashSet<Int32> original2WayList = twoWayReadership;

        // NOW MAKE THE SUBSETs.
        // The first subset follows a particular order.
        // I want the # of interrelationships to be at least as much as the # of people in the set.
        // in this way, I am likely to find the largest sets.
        // Anyone in a large set can be removed ENTIRELY from analysis of smaller sets!
        // So we go in reverse!

        // WE START at the top... 

        int iTop = 0;

        foreach (LJUser2 ljuFindTop in olCustomUserList)
        {
//            var whoIReadHere = from guy in ljuFindTop.ifd where guy > 0 select guy;

            if (ljuFindTop.whoIReadNumeric.Count() > iTop)
                iTop = ljuFindTop.whoIReadNumeric.Count();
        }

        // we have the top #.  probably the seed, of course.
    // the next goal is to reduce this Top until we can gather as many
    // possible occupants into one set, as this #.

        LoopTilTopDrops:
        int iTotalOccupants = 0;
        foreach (LJUser2 ljuFindPeeps in olCustomUserList)
        {
            if (ljuFindPeeps.whoIReadNumeric.Count >= iTop)
                iTotalOccupants++;
        }

        if (iTotalOccupants < iTop)
        {
            iTop--;
            goto LoopTilTopDrops;
        }
        ////////////////////////////////////////////

        newList = new CUserList();
        foreach (LJUser2 ljuFindZero in olCustomUserList)
        {
            if (ljuFindZero.whoIReadNumeric.Count >= iTop)
                newList.Add(ljuFindZero);
        }
        olCustomUserList = newList;
        // only two-ways in the custom user list get to live in this crazy test.
        HashSet<Int32> new2Way = new HashSet<Int32>();
//        ArrayList new2Way = new ArrayList();
        foreach (int itwoWayReader in twoWayReadership)
        {
            if( null != olCustomUserList.GetUserByID(itwoWayReader))
//            if (null != olCustomUserList.GetUser(itwoWayReader))
                new2Way.Add(itwoWayReader);
        }
        twoWayReadership = new2Way;

        /* THIS CODE IS PRETTY GREAT, AND I WILL PUT IT BACK IN LATER.
            IT USES THE ABOVE CALCULATED SUBSET OF USERS TO TAKE A SHOT
            AT THE TOP TRIBES.  BUT I'M ROLLING OUT DISTRIBUTED NETWORK NOW,
            AND I DON'T WANT THIS COMPLEXITY ADDED.  *DEFINITELY* weave it into
            the mix later.

        ArrayList alAllLevels = ChurnAndBurnTribes( lju, twoWayReadership, args[0] ) ;

        ArrayList alTopLevel = (ArrayList) alAllLevels[ alAllLevels.Count - 1 ] ;
        foreach( ArrayList thisSet in alTopLevel )
            {
            Console.WriteLine("") ;
            Console.WriteLine("One Top Set: " ) ;
            foreach( int strItem in thisSet )
                {
                Console.Write("{0} ", ((LJUser2)olCustomUserList[ strItem ]).Name) ;
                }
            }
            */

        // this produces GREATNESS.  Now I take the TOP TIER from this, JUST THE TOP, and 
        // offer it as a CLUE to the next call.
        //
        // FIRST restore the proper state.  AND CONFIRM THIS IS CORRECT.

        olCustomUserList = originalList;
        twoWayReadership = original2WayList;


        // FINALLY, we do the same work as above, but this time, we can specify usernumbers.
        /* i don't know what the fuck this does
        foreach (LJUser2 ljuToNum in olCustomUserList)
        {
//            ljuToNum.whoIReadNumeric = new ArrayList();
            foreach (int istrNam in ljuToNum.ifd)
            {
                if (istrNam < 0)
                    continue;
                // OPTIMIZATION: I am only adding the numeric reader here IF THEY READ ME BACK.
                // THEY MIGHT NOT EVEN BE IN THIS DATASET, as it's a subset of seed readers.
                LJUser2 ljuThem = olCustomUserList.GetUserByID(istrNam);
                if (null != ljuThem)
                {
                    if (ljuThem.Reads(ljuToNum.Name))
                    {

                        ljuToNum.whoIReadNumeric.Add(olCustomUserList.GetUserNumber(strNam));
                        /////////////////////////////////////////////////////////////
                        // ONCE YOU REFER TO A USERNUMBER, YOU CAN'T CHANGE THE ORDER OF THE OLCUSOMUSERLIST AFTER THAT POINT!!!!
                        /////////////////////////////////////////////////////////////

                        if (-1 == olCustomUserList.GetUserNumber(strNam))
                            Console.WriteLine("something in my plan is amiss.");
                    }
                }
            }
        }
         * */

        // This is where our traditional calculations are done.
        // Before I'm going to add my switch to circumvent the churn and burn in favor
        // of uploading to the queue, I want to test the validity of this code now.
        if (m_fJustUploadRawSeed)
        {
            Debug.Assert(false);// wtf is this.
            string[] astrNames = new string[olCustomUserList.Count];
            string[] astrWhoIReadHex = new string[olCustomUserList.Count];

            /*
            LJUser2 ljuSeedOrNot = (LJUser2)olCustomUserList[0];
            if (ljuSeedOrNot.Name != seed)
            {
                Console.WriteLine("I AM IN SERIOUS TROUBLE BECAUSE THE FIRST ITEM MUST BE THE SEED AND THAT'S FINAL.");
                Console.WriteLine("I AM IN SERIOUS TROUBLE BECAUSE THE FIRST ITEM MUST BE THE SEED AND THAT'S FINAL.");
            }

            for (int iItem = 0; iItem < olCustomUserList.Count; iItem++)
            {
                LJUser2 ljuItemForName = (LJUser2)olCustomUserList[iItem];
                astrNames[iItem] = ljuItemForName.Name;
                string strHexWhoIRead = "";
                foreach (int iReaderItem in ljuItemForName.whoIReadNumeric)
                {
                    strHexWhoIRead += iReaderItem.ToString("x3");
                }
                astrWhoIReadHex[iItem] = strHexWhoIRead;
            }
             *              * */

        }

        // We have a CALC mutex so all CALCs have 100% cpu. if it works.
        Console.WriteLine("I want the the exclusive calc mutex.");
//        Mutex m = new Mutex(false, CALC_CPU_100 + ("_NOT_BRUTE"));
//        m.WaitOne(); disabled ok???
        Console.WriteLine("I own the the exclusive calc mutex.");

        // If NEVER ABORTED BEFORE, this attempt will be aborted in ten minutes, if not complete.
        // WE ONLY START THE CALC AFTER THE FRIENDS DATA HAS LOADED.
        // OTHERWISE, SLOW BANDWIDTH KILLS US, AND THAT'S NOT THE INTENTION.
        // We postpone starting the timeout to just before jumping into the iterative work.
        if (m_fYesToTimeouts || m_fALWAYSYesToTimeouts)
        {
            if (m_fALWAYSYesToTimeouts || (false == MasterDB.WasSeedAborted(seed)))
            {
                ThreadStart threadStarter = new ThreadStart(ExecutionerThreadMethod);
                Thread t = new Thread(threadStarter);
                t.Start();
            }
        }

        // make the call so I can confirm (ha ha) nothing is clobbered
        Console.WriteLine("twoWayReadership going in: " + twoWayReadership.Count);
        Console.WriteLine("olCUstomUserList going in: " + olCustomUserList.Count);

        Console.WriteLine("The scrape says this user should have this many: " + lju.Readers);
        //	swt.WriteLine("The scrape says this user should have this many: " + lju.Readers) ;

//        ArrayList alAllLevelsCLASSIC = ChurnAndBurnTribes(lju, twoWayReadership, seed);

        // this goofy is that alAllLevels contains offests in olCustomUserList.
        // the zero level is two-wayers i guess.
        // the top level is largest sets.
        // so each entry is itself an arraylist of sets at that level.
        // so ask cliques to create its own version here, and i will carve the offset shit back in.
        List<List<int>> sl = TCliquesClass.TCliquesMain(seed);

        if (null == sl)
            return null;

        // ok, iterate down, fold in, xferring into olCustomerUserList offsets only.
        int size = sl[0].Count;
        ArrayList alAllLevels = new ArrayList(); // ha
        ArrayList thisLevel = new ArrayList();
        
        foreach (List<int> l in sl)
        {
            // transform into the proper flavor
            ArrayList aSet = new ArrayList();

            if (l.Count != size)
            {
//                alAllLevels.Add(thisLevel);
                alAllLevels.Insert(0, thisLevel);
                thisLevel = new ArrayList();
                size = l.Count;
            }

            foreach (int i in l)
            {
/*                int? slotnum = olCustomUserList.GetUserNumber(IDMap.IDToName(i));
                if (null == slotnum)
                    Console.WriteLine("Somehow a set member isn't in the olCustomUserList. Just watch this issue.");
                else 
 * */
                aSet.Add(i);
            }
            thisLevel.Add(aSet);
        }

        alAllLevels.Insert(0, thisLevel);

        // cram in all the two-wayers to a final set.
        thisLevel = new ArrayList();
        foreach (int iss in twoWayReadership)
        {
            ArrayList al = new ArrayList();
            al.Add( iss ) ; //   olCustomUserList.GetUserNumber( IDMap.IDToName( iss) ));
            thisLevel.Add(al);
        }
        alAllLevels.Insert(0, thisLevel);

        // next!
//        m.ReleaseMutex();
        Console.WriteLine("I have released the exclusive calc mutex.");

        ArrayList alShownOnce = new ArrayList();

        // doublecheck my work
        if (null != alAllLevels)
        {
            for (int i = alAllLevels.Count - 1; i >= 0; i--)
            {
                ArrayList alThisLevel = (ArrayList)alAllLevels[i];
                foreach (ArrayList thisSet in alThisLevel)
                {
                    foreach (int strItem in thisSet)
                    {
                        foreach (int shown in alShownOnce)
                        {
                            if (shown == strItem)
                                goto DontShowMe;
                        }

                        /*
                        // if this is the top tier, make sure we have aquired city data
                        if (i == alAllLevels.Count - 1)
                        {
                            LJUser2 ljuJerk = olCustomUserList.GetUserByID(strItem);
                            Debug.Assert(null != ljuJerk);
//                            LJUser2 ljuJerk = (LJUser2)olCustomUserList[strItem];
                            ljuJerk = MasterDB.GetSlimUser(ljuJerk.Name, true);

                            PerhapsAddCity(ljuJerk);
                        }
                         * */

                        // if we get this far, we show!
                  //      Console.Write("{0} ", ((LJUser2)olCustomUserList[strItem]).Name);
                        //				swt.Write("{0} ", ((LJUser2)olCustomUserList[strItem]).Name) ;
                        alShownOnce.Add(strItem);

                    // use master list here in case tribe was already calculated.
                    // custom list is only copy for computationally intensive
                    // tribe determination
                    //			LJUser2 ljMasterCopy = MasterDB.GetUser( ((LJUser2)olCustomUserList[strItem]).Name ) ;
                    //		LJUser2 ljAdd = new LJUser2() ;
                    //	ljAdd.Clone( ljMasterCopy ) ;
                    //ljAdd.Tier = lju.tribe.Count - i ;

                    DontShowMe:
                        ;
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("Items in tiers: " + alShownOnce.Count);
        }

        
        lju.tribe = alAllLevels;
        // SAVE this beautiful tribe where there was mere null before

        // if it's empty, then don't save it!  That'd be bad!  don't show it, either.  Cuz it won't show.
        // perhaps these users need something more but for now they get this.

        // we probably aborted
        if (lju.tribe == null)
            return null;

        if (lju.tribe.Count == 1)
        {
            ArrayList alNext = (ArrayList)alAllLevels[0];
            if (alNext.Count == 1)
            {
                alNext = (ArrayList)alNext[0];

                int iItem = (int)alNext[0];

                if (-1 == iItem)
                    return null;
            }
        }

        m_fDudeImDone = true;
        MasterDB.AddUser(lju, true, olCustomUserList);

        string[] argsy = { seed }; // ShowTribe still has the Title option so I still use the args format.

        ArrayList alRetJack = ShowTribe(lju, argsy, alFinallPlacements, alPrevFramePlacements);
        if (m_fTryTribeAgain)
        {
            m_fTryTribeAgain = false;
            goto TryAgainJock;
        }

        // slap top tier into instance-static string.
        // 
        return alRetJack; //
    }


    static bool SaveImageMapData(string seed, string imageMapData)
    {
         // hello, simple queues

        string strFullPath = "/imagemaps/" + seed + ".htm";
        FileInfo fi = new FileInfo(strFullPath);
        using (StreamWriter sw = fi.CreateText())
        {
            sw.WriteLine(imageMapData);
            sw.Close();
        }

        return true;
    }


    //static void ExamineUrlForPublicAccess( LJUser2 ljuToShow )
    /*static void CheckAndFixRemoteUrl( LJUser2 ljuToShow )
    {
        if(ljuToShow.Url != null)
            {
            if (-1 != ljuToShow.Url.ToUpper().IndexOf("LIVEJOURNAL"))
                ljuToShow.Url = "http://ljmindmap.com/h.aspx?n=" + ljuToShow.Name ;
            }
    }
    */


    static ArrayList SortByReadership(ArrayList newGuys, ArrayList alreadyChosen)
    {
        ArrayList alReturn = new ArrayList();

        while (newGuys.Count > 0)
        {
            int iTopVal = 0;
            int iTopSlot = -1;

            for (int iPos = 0; iPos < newGuys.Count; iPos++)
            {
                LJUser2 ljuTheNewGuy = (LJUser2)newGuys[iPos];
                int yeppers = 0;

                foreach (LJUser2 somePlacedDork in alreadyChosen)
                {
                    if (ljuTheNewGuy.Reads (somePlacedDork))
                        if (somePlacedDork.Reads(ljuTheNewGuy))
                            yeppers++;
                }

                if ((-1 == iTopSlot) || yeppers > iTopVal)
                {
                    iTopVal = yeppers;
                    iTopSlot = iPos;
                }
            }

            alReturn.Add(newGuys[iTopSlot]);
            newGuys.RemoveAt(iTopSlot);
        }

        return alReturn;
    }


    /*
    static void MakeBannerTeaser( string targetDir, string uName, string ext, string deriveFromThisFile )
    {
        // could be a bad thing...
        Bitmap bm = (Bitmap) Bitmap.FromFile( deriveFromThisFile ) ;

        // we output all pixels from 275 - 325 y axis to the new bitmap
        // and we tag on 20 pixels for our click here message.
        int iWide = 500 ;
        int iClickMeZone = 20 ;
        int iShowClean = 50 ;
        int iHigh = iShowClean + iClickMeZone ;
        int iStartYOffset = 275 ;
	
        Bitmap newBitmap = new Bitmap(iWide, iHigh) ;
        Graphics objGraphics = Graphics.FromImage(newBitmap) ;

        for( int iXPos = 0; iXPos < iWide; iXPos++)
            {
            for( int iYPos = iStartYOffset ; iYPos < iStartYOffset + iHigh; iYPos++)
                {
                Color coBit = bm.GetPixel( iXPos, iYPos ) ;
                newBitmap.SetPixel( iXPos, iYPos - iStartYOffset, coBit ) ;
                }
            }

        // lattice toward black so we can draw red text on it.
        for( int iXPos = 0; iXPos < iWide; iXPos++)
            {
            for( int iYPos = iShowClean ; iYPos < iShowClean + iClickMeZone; iYPos++)
                {
                if ( 0 != iXPos % (iYPos - iShowClean + 1 ))
                    newBitmap.SetPixel( iXPos, iYPos, Color.Black ) ;
                }
            }

        Font redFont = new Font("Arial", 12, FontStyle.Bold | FontStyle.Italic) ;
        SolidBrush br = new SolidBrush( Color.Red ) ;
        string strSay = uName + "'s MindMap..." ;
        SizeF stringSize = objGraphics.MeasureString(strSay, redFont) ;

        objGraphics.DrawString(strSay, redFont, br, iWide / 2 - stringSize.Width / 2, iShowClean) ;
	
        Pen pnYellow = new Pen( Color.Yellow ) ;
        Point ptFrom = new Point( 0, iShowClean) ;
        Point ptTo = new Point( iWide, iShowClean ) ;
        objGraphics.DrawLine( pnYellow, ptFrom, ptTo ) ;
	
        newBitmap.Save(targetDir + uName + ext + ".gif" , ImageFormat.Gif) ;
    }
    */




    static Mutex mST = new Mutex(false, "TIERS");

    static ArrayList ShowTribe(LJUser2 lju, string[] args)
    {
        return ShowTribe(lju, args, null, null);
    }


    static ArrayList ShowTribe(LJUser2 lju, string[] args, ArrayList alFinalPlacements, ArrayList alPrevFramePlacements)
    {
        //g4540	for( int iCycle = 0 ; iCycle < iCycles ; iCycle++)
        // parse all levels end to front, showing unique.
        ArrayList alMembersInOrder = new ArrayList();
        ArrayList alShownOnce = new ArrayList();

        string UseMe = "tiers.txt";

        FileInfo fi = new FileInfo(UseMe);

        Console.WriteLine("Want ShowTribe mutex.");
        mST.WaitOne();
        Console.WriteLine("Have ShowTribe mutex.");

        StreamWriter swt = fi.AppendText();
        swt.WriteLine(lju.Name);

        for (int i = lju.tribe.Count - 1; i >= 0; i--)
        {
            Console.WriteLine("Tier {0}", lju.tribe.Count - i);
            swt.WriteLine("Tier {0}", lju.tribe.Count - i);
            ArrayList alThisLevel = (ArrayList)lju.tribe[i];
            ArrayList alIntermediaryMembersInOrder = new ArrayList();

            foreach (ArrayList thisSet in alThisLevel)
            {
                foreach (int strItem in thisSet)
                {
                    foreach (int shown in alShownOnce)
                    {
                        if (shown == strItem)
                            goto DontShow;
                    }
                    alShownOnce.Add(strItem);

                    // use master list here in case tribe was already calculated.
                    // custom list is only copy for computationally intensive
                    // tribe determination
                    LJUser2 ljMasterBater = MasterDB.GetSlimUser(IDMap.IDToName(strItem));
                    if (olCustomUserList.GetUserByID(strItem) == null)
                    {
                        Console.WriteLine("this shit shouldn't happen.");
                    }
                    Debug.Assert(null != ljMasterBater);
                    LJUser2 ljAdd = new LJUser2();
                    ljAdd.Clone(ljMasterBater);
                    ljAdd.Tier = lju.tribe.Count - i;
                    //				alMembersInOrder.Add( ljAdd ) ; 
                    alIntermediaryMembersInOrder.Add(ljAdd);

                DontShow:
                    ;
                }
            }

            alIntermediaryMembersInOrder = SortByReadership(alIntermediaryMembersInOrder, alMembersInOrder);

            foreach (LJUser2 ljuInterm in alIntermediaryMembersInOrder)
            {
                alMembersInOrder.Add(ljuInterm);
            }

            Console.WriteLine("");
            Console.WriteLine("");

            swt.WriteLine("");
            swt.WriteLine("");
        }

        int twoWay = (from fd in lju.ifd
                      where lju.ifd.Contains(-fd)
                      select fd).Count();

        Console.WriteLine("FData two-way: " + twoWay.ToString());
        Console.WriteLine("Code thinks: " + alShownOnce.Count.ToString());

        swt.Close();
        mST.ReleaseMutex();

        // render this tribe into a bitmap
        // based on how choc full o names it is

        int iWide = 500;
        int iHigh = 500;

        if (alShownOnce.Count > 117)
        {
            iWide = 645;
            iHigh = 615;
        }

        Font[] fiveFonts = { new Font("Arial", 16, FontStyle.Bold | FontStyle.Italic), // titles
						new Font("Arial", 16, FontStyle.Bold ),
						new Font("Arial", 14, FontStyle.Bold),
						new Font("Arial", 12, FontStyle.Bold),
						new Font("Arial", 10, FontStyle.Bold) };

        ArrayList olRectList = new ArrayList();
        alShownOnce = new ArrayList();

        ArrayList alLjuPlaced = new ArrayList();

        // some people read themselves, and we want to show them only once
        // I wrote this once and just got confused.  I'm writing it again.  And I kill all occurances
        // of starter user, and adding starter user at the start.
        ArrayList nukeLater = new ArrayList();
        foreach (LJUser2 ljuDupe in alMembersInOrder)
        {
            if (ljuDupe.Name.ToUpper() == lju.Name.ToUpper())
                nukeLater.Add(ljuDupe);
        }
        foreach (LJUser2 ljuNuke in nukeLater)
            alMembersInOrder.Remove(ljuNuke);

        LJUser2 ljMe = new LJUser2();
        ljMe.Clone(lju); // just cuz!
        ljMe.Tier = 1;

        bool fTitle = false;

        if (args == null)
            fTitle = false;
        else
            if (args.GetLength(0) > 1)
                fTitle = true;

        if (fTitle)
        {
            alMembersInOrder.Insert(0, ljMe);
            // print me first place.  but put me in LATER.
            ljMe = new LJUser2(); // make a fake user
            ljMe.Name = args[1];
            ljMe.Tier = 0;
            ljMe.ifd = null; // ""; // nobody
        }

        int fontNum = ljMe.Tier;
        if (fontNum > 4)
            fontNum = 4;

        Bitmap measurementBitmap = new Bitmap(iWide, iHigh);
        Graphics measurementGraphics = Graphics.FromImage(measurementBitmap);

        SizeF stringSize = measurementGraphics.MeasureString(ljMe.Name, fiveFonts[fontNum]);
        Rectangle rectCore = new Rectangle((int)(iWide / 2 - (stringSize.Width / 2)),
                                        (int)(iHigh / 2 - (stringSize.Height / 2)),
                                        (int)(stringSize.Width),
                                        (int)(stringSize.Height));

        DrawLJUser(ljMe, rectCore, alLjuPlaced, fiveFonts[fontNum], measurementGraphics, fTitle, -1, true, ljMe.Name == lju.Name, false, true); // , true ) ; // always free seed
        //	DrawLJUser(ljMe, rectCore, alLjuPlaced, fiveFonts[fontNum], objGraphicsBW, fTitle, -1, true, ljMe.Name == lju.Name, false ) ; // , false ) ;

        foreach (LJUser2 ljuToShow in alMembersInOrder)
        {
            //		static ArrayList ShowTribe(LJUser2 lju, string [] args, ArrayList alFinallPlacements, ArrayList alPrevFramePlacements ) 
            PlaceEm(ljuToShow, alLjuPlaced, measurementGraphics, fiveFonts, iWide, iHigh, null, alFinalPlacements, alPrevFramePlacements);

            ArrayList alJigglers = new ArrayList();
            foreach (LJUser2 ljuPlaced in alLjuPlaced)
            {
                if (ljuPlaced.Name != ljuToShow.Name)
                    if (ljuPlaced.Reads(ljuToShow))
                        if (ljuToShow.Reads(ljuPlaced))
                            if (ljMe.Name != ljuPlaced.Name)
                            {
                                //							Console.WriteLine( ljuPlaced.Name + " gets jiggled due to " + ljuToShow.Name ) ;
                                Console.Write("."); // jiggles are dots.

                                alJigglers.Add(ljuPlaced);
                            }
            }

            foreach (LJUser2 ljuJig in alJigglers)
            {
                // remove from array
                int iPos = alLjuPlaced.IndexOf(ljuJig);
                alLjuPlaced.RemoveAt(iPos);
                PlaceEm(ljuJig, alLjuPlaced, measurementGraphics, fiveFonts, iWide, iHigh, ljuJig.rect, alFinalPlacements, alPrevFramePlacements);
            }

            LJUser2 ljuFullData = MasterDB.GetSlimUser(ljuToShow.Name); // why is this here?
        }

        // so here i render. 
        Bitmap objBitmapBW = new Bitmap(iWide, iHigh);
        Graphics objGraphicsBW = Graphics.FromImage(objBitmapBW);
        Bitmap objBitmapCOL = new Bitmap(iWide, iHigh);
        Graphics objGraphicsCOL = Graphics.FromImage(objBitmapCOL);

        DrawLJUser(ljMe, rectCore, alLjuPlaced, fiveFonts[fontNum], objGraphicsBW, fTitle, -1, false, true, false, false);
        DrawLJUser(ljMe, rectCore, alLjuPlaced, fiveFonts[fontNum], objGraphicsCOL, fTitle, -1, false, true, false, false);

        foreach (LJUser2 ljuRender in alLjuPlaced)
        {
            int fontNumR = ljuRender.Tier;
            if (fontNumR > 4)
                fontNumR = 4;
            bool fSuppressColor = false;

            DrawLJUser(ljuRender, ljuRender.rect, alLjuPlaced, fiveFonts[fontNumR], objGraphicsBW, false, -1, false, false, fSuppressColor, false); // , bool fColorFreebie ) // -1 if no tribenum
            DrawLJUser(ljuRender, ljuRender.rect, alLjuPlaced, fiveFonts[fontNumR], objGraphicsCOL, false, -1, true, false, fSuppressColor, false); // , bool fColorFreebie ) // -1 if no tribenum
        }

        string savedToDisplayEntryFile = ""; // = string.Format("<img src=\"http://www.gal2k.com/~gifs/{0}\" usemap=\"#fruityClickMap{1}\">", strBitmapName, lju.Name.ToUpper()) ;
        //	string strBitmapNameDefault = "" ; // compile and shut up

        string strBitmapNameBW = string.Format("{0}_m.gif", lju.Name); // c:\\temp\\ was removed
        string strBitmapNameCOL = string.Format("{0}_c.gif", lju.Name); // c:\\temp\\ was removed
        string strBitmapNameDefault = string.Format("{0}.gif", lju.Name); // c:\\temp\\ was removed

        string strFullPathBW = "/map_archive/maps/" + strBitmapNameBW;
        objBitmapBW.Save(strFullPathBW, ImageFormat.Gif);
        //	MakeBannerTeaser( "maps/", lju.Name, "_mt", strFullPathBW ) ;

        string strFullPathCol = "/map_archive/maps/" + strBitmapNameCOL;
        objBitmapCOL = Frost(iWide, iHigh, objBitmapCOL);
        Bitmap objBitmapCOLWithCobwebs = AddCobwebs(iWide, iHigh, alLjuPlaced, objBitmapCOL, strFullPathCol); // does the save to maps but i also wanna do uploads_color

        objBitmapCOLWithCobwebs.Save("/uploads_color/" + strBitmapNameDefault, ImageFormat.Gif);

        objBitmapBW.Save("/uploads/" + strBitmapNameDefault, ImageFormat.Gif);

        // Both B&W and color maps have been saved. 
        // put a b&w and a color sash in two different source dirs
        // a command line entry in num will combine these into an animation
        // kill the source files, and then upload the animated sash.
        Rectangle rc = new Rectangle( objBitmapCOL.Size.Width / 2 - 250 ,
                                        objBitmapCOL.Size.Height / 2 - 50 ,
                                        500,
                                        100) ;

        Bitmap colorSash = objBitmapCOLWithCobwebs.Clone(rc, PixelFormat.DontCare);
        colorSash.Save("/sashes/col/" + strBitmapNameDefault, ImageFormat.Gif);

        Bitmap BWSash = objBitmapBW.Clone(rc, PixelFormat.DontCare);
        BWSash.Save("/sashes/bw/" + strBitmapNameDefault, ImageFormat.Gif);

        for (int iPass = 0; iPass <= 2; iPass++)
        {
            savedToDisplayEntryFile += string.Format("<img src=\"http://ljmindmap.com/r/?f={0}\" usemap=\"#fruityClickMap{1}\">", strBitmapNameDefault, lju.Name.ToUpper());
            savedToDisplayEntryFile += string.Format("<map name=\"fruityClickMap{0}\">", lju.Name.ToUpper());

            // we build until we size limit
            // we place .Url items, then non-Url items.
            // all Urls we can find should be populated already by our work prior to DrawLJUser.
            foreach (LJUser2 ljuSplattered in alLjuPlaced)
            {
                if (ljuSplattered.Url != null)
                {
                    savedToDisplayEntryFile += ProduceMapString(ljuSplattered, ljuSplattered.rect, iPass);
                    if (savedToDisplayEntryFile.Length > 15000)
                        break;
                }
            }

            if (savedToDisplayEntryFile.Length < 15000)
            {
                foreach (LJUser2 ljuSplattered in alLjuPlaced)
                {
                    if (ljuSplattered.Url == null)
                    {
                        // if there's a title, uh, is this the title?
                        if (args.GetLength(0) == 2)
                            if (ljuSplattered.Name == args[1])
                                continue;

                        savedToDisplayEntryFile += ProduceMapString(ljuSplattered, ljuSplattered.rect, iPass);
                        if (savedToDisplayEntryFile.Length > 15000)
                            break;
                    }
                }
            }

            savedToDisplayEntryFile += "</map>";

            // On the first pass, a terse file makes me happy. Or second pass will be ultra-terse.
            if (iPass == 0 || iPass == 1)
            {
                if (savedToDisplayEntryFile.Length <= 15000)
                    break;
                else
                    savedToDisplayEntryFile = "";
            }
        }

        // this saveToDisplayEntryFile is part of the tribe (and a specific rendering of it), and is stored in the seed user's database entry, though not in their object.
        SaveImageMapData(lju.Name, savedToDisplayEntryFile); // ) // uploads htm to web site.

        return alLjuPlaced;
    }

    static void PlaceEm(LJUser2 ljuToShow,
                        ArrayList alLjuPlaced,
                        Graphics measurementGraphics,
                        Font[] fiveFonts,
                        int iWide,
                        int iHigh,
                        Rectangle? rcPrevious,
                        ArrayList alFinalPlacements,
                        ArrayList alPrevFramePlacements)
    {
        int fontNum = ljuToShow.Tier;
        if (fontNum > 4)
            fontNum = 4;
        SizeF stringSize = measurementGraphics.MeasureString(ljuToShow.Name, fiveFonts[fontNum]);

        // I want to make this very simple for now. If I can place it in its final position, do so.
        // If not, then if I can place it in the previous position, do so.
        Rectangle rcWinner = new Rectangle(50, 50, 500, 500); // ucky. needed so our bypass works.
        bool bypass = false;
        double shortestDist = 1; // so long asi t's not 10101011
        int? iMandatedColor = null;

        if (null != alFinalPlacements)
        {
            // find this user in the array
            foreach (LJUser2 ljuFinalPlaced in alFinalPlacements)
            {
                if (ljuFinalPlaced.Name.ToUpper() == ljuToShow.Name.ToUpper())
                {
                    if (false == RectCollision(alLjuPlaced, ljuFinalPlaced.rect))
                    {
                        rcWinner = ljuFinalPlaced.rect;
                        iMandatedColor = ljuFinalPlaced.color;
                        bypass = true;
                        break;
                    }
                }
            }
        }

        if (false == bypass)
        {
            if (null != alPrevFramePlacements)
            {
                // find this user in the array
                foreach (LJUser2 ljuPrevPlaced in alPrevFramePlacements)
                {
                    if (ljuPrevPlaced.Name.ToUpper() == ljuToShow.Name.ToUpper())
                    {
                        if (false == RectCollision(alLjuPlaced, ljuPrevPlaced.rect))
                        {
                            rcWinner = ljuPrevPlaced.rect;
                            iMandatedColor = ljuPrevPlaced.color;
                            bypass = true;
                            break;
                        }
                    }
                }
            }
        }


        // We don't know where this user will end up.  To know that we have to
        // consider first all their friends already placed, and then all other objects
        // already placed.

        if (false == bypass)
        {
            ArrayList alPossibleTargets = new ArrayList();
            if (rcPrevious != null)
                alPossibleTargets.Add(rcPrevious);

            bool fFriendsOnly = true;

            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                    fFriendsOnly = false;

                foreach (LJUser2 ljuPlaced in alLjuPlaced)
                {
                    if (fFriendsOnly)
                    {
                        // custom user list is only for computationally intensive portion.
                        // this portion uses master user list.
                        //				LJUser2 ljuTrueUserObj = MasterDB.GetSlimUser( ljuToShow.Name ) ;
                        if (ljuPlaced.Tier == 0) // it's a title
                            goto AcceptableNeighbor;
                        if (ljuToShow.Reads(ljuPlaced))
                            goto AcceptableNeighbor;

                        continue; // this neighbor is not accepable
                    }

                    // outside friends only mode, any neighbor is acceptable.
                // But can a position adjacent to it be found?
                AcceptableNeighbor:

                    // I truly need to randomize this placement technique.
                    Random rd = new Random();
                    string strFour = null;

                    // I want more jiggle!  more clustering.  more passes.
                    for (int iPass = 0; iPass < 5; iPass++)
                    {
                        // we need to favor growing left, slightly.
                        switch (rd.Next() % 4)
                        {
                            case 0:
                                strFour = "abcd";
                                break;
                            case 1:
                                strFour = "bcda";
                                break;
                            case 2:
                                strFour = "cdab";
                                break;
                            case 3:
                                strFour = "dabc";
                                break;
                            /*
                        case 4:
                            strFour = "cdab" ;
                            break ;
                        case 5:
                            strFour = "dbca" ;
                            break ;
                            */

                        }

                        Rectangle rectCore = new Rectangle();
                        for (int iOption = 0; iOption < 4; iOption++)
                        {
                            switch (strFour[iOption])
                            {
                                case 'a':
                                    rectCore = new Rectangle(ljuPlaced.rect.Right,
                                                           ljuPlaced.rect.Bottom,
                                                           (int)stringSize.Width,
                                                           (int)stringSize.Height);
                                    break;

                                case 'b':
                                    rectCore = new Rectangle(ljuPlaced.rect.Right,
                                                           ljuPlaced.rect.Top - (int)stringSize.Height,
                                                           (int)stringSize.Width,
                                                           (int)stringSize.Height);
                                    break;

                                case 'c':
                                    rectCore = new Rectangle(ljuPlaced.rect.Left - (int)stringSize.Width,
                                                           ljuPlaced.rect.Bottom,
                                                           (int)stringSize.Width,
                                                           (int)stringSize.Height);
                                    break;

                                case 'd':
                                    rectCore = new Rectangle(ljuPlaced.rect.Left - (int)stringSize.Width,
                                                           ljuPlaced.rect.Top - (int)stringSize.Height,
                                                           (int)stringSize.Width,
                                                           (int)stringSize.Height);
                                    break;

                                default:
                                    break;
                            }

                            // Now I'm just going to WIGGLE the rect, damn it.
                            int idx = rd.Next() % 50 - 25;
                            int idy = rd.Next() % 50 - 25;
                            Point pt = new Point(rectCore.X + idx, rectCore.Y + idy);
                            rectCore.Location = pt;


                            if ((false == RectCollision(alLjuPlaced, rectCore)) &&
                                (rectCore.Left > 0) &&
                                (rectCore.Top > 0) &&
                                (rectCore.Right < iWide) &&
                                (rectCore.Bottom < iHigh))
                                //						goto FoundTargetRect ;
                                alPossibleTargets.Add(rectCore);
                        }
                    }
                }
            }

            // now evaluate all the possible targets for the shortest distance to all placed friends.
            shortestDist = 10101010;
            rcWinner = new Rectangle(50, 50, 500, 500); // ucky.
            foreach (Rectangle rc in alPossibleTargets)
            {
                double dist = 0;

                ArrayList al = alLjuPlaced;
                //				if( ghostHints != null)
                //g4688					al = ghostHints ;

                //				foreach( LJUser2 ljuPlaced in alLjuPlaced )
                foreach (LJUser2 ljuPlaced in al)
                {
                    if (
                        (ljuPlaced.Tier == 0) ||  // the center.  all seek nearness to the center.
                        (
                            (ljuPlaced.Reads(ljuToShow)) &&  //mutual readership
                            (ljuToShow.Reads(ljuPlaced))
                        )
                        )
                    {
                        double newdist = Distance(ljuPlaced.rect, rc) * Distance(ljuPlaced.rect, rc); // what if we square the distance?

                        // if the seed is involved, loosen this grip.
                        if (alLjuPlaced.IndexOf(ljuPlaced) == 0)
                        {
                            newdist /= 3;
                        }

                        dist += newdist;
                    }
                }

                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    rcWinner = rc;
                }
            }
        }

        // if we do not have a real rcwinner, we do not draw the user.
        if (bypass == true || shortestDist != 10101010)
        {
            // more playing.  nope, doesn't fly.

            //	CheckAndFixRemoteUrl( ljuToShow ) ; // might clobber its url
            // url is null.  check that pickup/seed.htm exists.  if so, cobble the url to the pickup schtick.  save it in database.

            if (ljuToShow.Url == null)
            {
                // if seed.htm exists, then cobble pickup url.
                // BUT NOW WE CHECK OUR OWN LOCAL DATABASE.
                string strCmd = string.Format("select count(*) from nameidmap where lastpublishedmonth>0 and name='{0}'; ", ljuToShow.Name);
                MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, MasterDB.GetDBConnection());
                NpgsqlDataReader myReader = cmd.ExecuteReader();
                myReader.Read();
                if (myReader.HasRows)
                {
                      if (myReader.GetInt64(0)> 0)
                    {
                        string strUseThis = "http://ljmindmap.com/h.php?n=" + ljuToShow.Name;
                        ljuToShow.Url = strUseThis;
                    }
                }
            }

            /**

        string strUrl = "http://ljmindmap.com/pickup/" + ljuToShow.Name + ".htm" ;
        if (UrlExists( strUrl ))
            {
            strUrl = "http://ljmindmap.com/tr/" + ljuToShow.Name + ".gif" ;
            if (UrlExists( strUrl ))
                {
                string strUseThis = "http://ljmindmap.com/h.php?n=" + ljuToShow.Name ;
// just don't wanna do this no more					MasterDB.AddUrl( ljuToShow.Name, strUseThis ) ;
                ljuToShow.Url = strUseThis ;
                }
            }
             * 
        }		
             * */
            // for now, let us examine every item we place, and populate its .Url if we are hosting it.
            // this will quickly populate url fields for our deployed base.  heaven help us.

            // brute force is full color because the bottom tier is often a lot of peoples.
            // but sometimes people from higher tiers appear in the bottom tier
            // and subset names have trouble, too,
            // so for now i just make sure the fontNum is Mr. Small
            // and if so, then ok, we'll let it go gray.
            bool fSuppressColor = false;
    //        if (false == m_fOkBruteForceIt)
    //        {
     //           Debug.Assert(false);
//                if (-1 != m_bottomTier.IndexOf(ljuToShow.Name))
  //                  if (fontNum == 4)
    //                    fSuppressColor = true;
  //          }

            //		DrawLJUser(ljuToShow, rcWinner, alLjuPlaced, fiveFonts[fontNum], measurementGraphics, false, -1, false, false, fSuppressColor ) ; // , fColorFreebie2) ;
            DrawLJUser(ljuToShow, rcWinner, alLjuPlaced, fiveFonts[fontNum], measurementGraphics, false, -1, true, false, fSuppressColor, true, iMandatedColor); // , fColorFreebie2) ;
        }
    }

    static bool AllFriends(ArrayList sb, LJUser2 ljuAddOrPass)
    {
        foreach (LJUser2 lju in sb)
        {
            if (false == lju.Reads(ljuAddOrPass.Name))
                return false;

            if (false == ljuAddOrPass.Reads(lju.Name))
                return false;
        }

        return true;
    }


    static bool IsFirstTrueSubsetOfSecond(ArrayList cluster, ArrayList sb)
    {
        foreach (int iMember in cluster)
        {
            bool found = false;

            foreach (int iSnowItem in sb)
            {
                if (iSnowItem == iMember)
                {
                    found = true;
                    break;
                }
            }
            // was this member unfound in the snowball?
            if (false == found)
                return false;
        }
        return true;
    }


    // return modified nextStep 
    /*
    static ArrayList DropSnowballSubsets(ArrayList nextStep, ArrayList sb)
    {
    // remove any complete subset of this snowball from the nextStep array
    LookAgain:
        for (int i = 0; i < nextStep.Count; i++)
        {
            if (IsFirstTrueSubsetOfSecond((ArrayList)nextStep[i], sb))
            {
                ArrayList dead = (ArrayList)nextStep[i];
                Console.Write(" ( ");

                foreach (int ided in dead)
                {
                    Console.Write(((LJUser2)olCustomUserList[ided]).Name + " ");
                }
                Console.Write(" ) ");

                nextStep.RemoveAt(i);
                goto LookAgain;
            }
        }

        return nextStep;
    }
     * */


    /*
    static ArrayList NumericList(CUserList olCustomUserList, ArrayList sb)
    {
        ArrayList sbInNums = new ArrayList();

        foreach (LJUser2 lju in sb)
        {
            sbInNums.Add(olCustomUserList.GetUserNumber(lju.Name));
        }

        return sbInNums;
    }
     * */


    /*
    static bool IsFirstTrueSubsetOfSecondLJUStyle( ArrayList cluster, ArrayList sb )
    {
        foreach( LJUser2 lju in cluster )
            {
            bool found = false ;

            foreach( LJUser2 ljusb in sb )
            {
                if (ljusb.Name.ToUpper() == lju.Name.ToUpper())
                    {
                    found = true ;
                    break ;
                    }
            }
            // was this member unfound in the snowball?
            if (false == found)
                return false ;
            }
        return true ;
    }
    */

/*

    static ArrayList ChurnAndBurnTribes(LJUser2 lju, ArrayList twoWayReadership, string seedname)
    {
        // Some users... after determining they are irrelivant to this dataset, are removed.
        // The (biggest?) source of irrelivancy here is them having zero NUMERIC readers.
        // NUMERIC readers are limited to MUTUAL READERSHIP relationships.

        // I KNOW THIS IS A FREAKING MESS.  BUT If the user has no readers in the numeric list
        // (which, mind you, unlike the whoIRead data, has just been loaded with ONLY MUTUAL
        // READERSHIPS).  And 

        // HERE I DO AN INCREDIBLE THING.
        // I do a "preliminary pass" on the "major players" in this dataset.
        // If this yields larger sets, I can use them to SIFT OUT SUBSETS in my thorough pass
        // create a new CUserList and sort it by Iread count.

        // we're gonna do a scattershot snowball accumulation 100 times, jack.

        ArrayList snowballs = new ArrayList();

        // ok, we only use snowballs during brute force because we have errors in this algorythm.
        if (m_fOkBruteForceIt == true)
        {
            Random rd = new Random();

            int iEndOnXFailures = 50;
            // now i'm going to calculate the sequence string required for completion
            // by subtracting the number of minutes elapsed from 90.
            DateTime started = DateTime.Now;
            //		if (m_fOkBruteForceIt)
            //			iEndOnXFailures = 100 ;

            for (int i = 0; i < iEndOnXFailures; )
            {
                if (m_fOkBruteForceIt)
                {
                    TimeSpan ts = DateTime.Now.Subtract(started);
                    iEndOnXFailures = 90 - ts.Minutes;
                }

                // we need a fresh list to shoot at.
                CUserList ulForSorting = new CUserList();
                foreach (LJUser2 ljuAdd in olCustomUserList)
                    ulForSorting.Add(ljuAdd);
                m_iSortBy = SB_READERS;
                ulForSorting.Sort();

                ArrayList sb = new ArrayList();

            addAnother:
                for (int iShot = 0; iShot < ulForSorting.Count; iShot++)
                {

                Loopy:
                    int iDrift = rd.Next() % 50 - 25;
                    int iTarget = iShot + iDrift;
                    if (iTarget >= ulForSorting.Count)
                        goto Loopy;
                    if (iTarget < 0)
                        goto Loopy;

                    if (sb.Count == 0 || AllFriends(sb, (LJUser2)ulForSorting[iTarget]))
                    {
                        sb.Add(ulForSorting[iTarget]);
                        ulForSorting.RemoveAt(iTarget);
                        goto addAnother;
                    }
                }

                // throw out all subsets.
                for (int iWhich = 0; iWhich < snowballs.Count; iWhich++)
                {
                    if (IsFirstTrueSubsetOfSecond(NumericList(olCustomUserList, sb), (ArrayList)snowballs[iWhich]))
                    {
                        goto NotGoodSB;
                    }
                    if (IsFirstTrueSubsetOfSecond((ArrayList)snowballs[iWhich], NumericList(olCustomUserList, sb)))
                    {
                        snowballs.RemoveAt(iWhich);
                        //				Console.WriteLine( "Clobbering a flake.") ;
                        Console.Write("/");
                    }
                }

                snowballs.Add(NumericList(olCustomUserList, sb));
                //		Console.Write ("Adding " + sb.Count.ToString() + "-way flake.  " + snowballs.Count + " flakes total.  " ) ;
                //		Console.WriteLine("") ;

                i = 0; // reset the # of failures count.
                Console.Write("+");
                continue;

            NotGoodSB:
                i++; // increment the number of failed passes count.
                Console.Write(".");
                ;
            }
        }

        // run all snowballs across the list one last time to seek growth.
        //	foreach( ArrayList sb in snowballs )
        //		{
        // fuck this for now.  too tired.
        //		}

        Console.WriteLine("Snowballs: ");
        foreach (ArrayList sb in snowballs)
        {
            foreach (int iDude in sb)
            {
                Console.Write(((LJUser2)olCustomUserList[iDude]).Name + " ");
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }

        Console.WriteLine("");
        Console.WriteLine("Two-way readership around: {0}", lju.Name);
        foreach (string str in twoWayReadership)
            Console.Write("{0}, ", str);

        Console.WriteLine("");

        // put an additional level of indirection and you've got it.
        ArrayList alTwoWay = new ArrayList();
        foreach (string str in twoWayReadership)
        {
            ArrayList itemWrapper = new ArrayList();
            itemWrapper.Add(olCustomUserList.GetUserNumber(str));
            alTwoWay.Add(itemWrapper);
        }

        ArrayList alAllLevels = new ArrayList();
        alAllLevels.Add(alTwoWay);

        // BEFORE DIVING IN, convert twoWayReadership into an ArrayList of INT's.
        ArrayList newTwoWayReadership = new ArrayList();
        foreach (string str in twoWayReadership)
        {
            newTwoWayReadership.Add(olCustomUserList.GetUserNumber(str));
        }

        twoWayReadership = newTwoWayReadership;


        // THe proper way to do this is to loop until we run out of goods.
        bool fStillGoing = true;
        ArrayList nextStep = alTwoWay; // threeWayReadership ;
        bool fDroppedSnowFromBinarySets = false;
        while (fStillGoing)
        {

            ArrayList alUnique = new ArrayList();

            int iLevels = ((ArrayList)nextStep[0]).Count;

            if (m_fOkBruteForceIt)
            {
                // clobber first level 
                if (iLevels == 1)
                {
                    ArrayList PlaceMat = new ArrayList();
                    for (int iRepro = 0; iRepro < iLevels; iRepro++)
                        PlaceMat.Add(0);

                    ArrayList EternalGoober = new ArrayList();
                    EternalGoober.Add(PlaceMat);

                    nextStep = EternalGoober;
                }
            }
            // nextStep[0] occasionally doesn't exist post-GrowSetNumeric, even though i need to know its count
            // for slicing snowballs back in.  I guess if it gets disappeared for two steps, well, i might crash here.
            m_iNextStepCount = ((ArrayList)nextStep[0]).Count;

            nextStep = GrowSetNumeric(nextStep, ref twoWayReadership);

            // if it ended, prematurely, spark it up again.
            // REALLY TOUCHY SUBJECT.  MUST MODIFY THIS CODE.  BELIEVE WE TIMED OUT.
            //		if (0 == nextStep.Count && m_fOkBruteForceIt)
            if (false == m_fAbortNow)
                if (0 == nextStep.Count && snowballs.Count > 0)
                {
                    ArrayList PlaceMat = new ArrayList();
                    for (int iRepro = 0; iRepro <= iLevels; iRepro++)
                        PlaceMat.Add(0);

                    ArrayList EternalGoober = new ArrayList();
                    EternalGoober.Add(PlaceMat);

                    nextStep = EternalGoober;
                }


            // the first time out of growset, we throw out two-member sets that are 
            // subsets of snowballs.
            if (false == fDroppedSnowFromBinarySets && false == m_fOkBruteForceIt)
            {
                fDroppedSnowFromBinarySets = true;

                foreach (ArrayList sb in snowballs)
                {
                    nextStep = DropSnowballSubsets(nextStep, sb);
                }
            }

        // Snowballs also get added back into interative steps.  This assures
        // that snowballs grow to their maximum, just like anyone.
        LoopTilNoneMore:
            for (int i = 0; i < snowballs.Count; i++)
            {
                if (((ArrayList)snowballs[i]).Count == m_iNextStepCount + 1)  // ((ArrayList)nextStep[0]).Count )
                {
                    nextStep.Add(snowballs[i]);
                    snowballs.RemoveAt(i);
                    goto LoopTilNoneMore;
                }
            }

            if (m_fAbortNow)
            {
                Console.WriteLine("seed aborted due to timeout.");
                //			MasterDB.AbortSeed( seedname ) ;
                return null;
            }

            if (nextStep.Count == 0)
                break;


            alAllLevels.Add(nextStep);
            fStillGoing = false;
            foreach (ArrayList setItem in nextStep)
            {
                fStillGoing = true;
                foreach (int iMember in setItem)
                {
                    string strMember = ((LJUser2)olCustomUserList[iMember]).Name;
                    // just show unique.  accrue unique
                    foreach (string strUnique in alUnique)
                    {
                        if (strMember.ToUpper() == strUnique.ToUpper())
                            goto NotUnique;
                    }

                    // if we get here, we are unique!  add us!
                    alUnique.Add(strMember);

                NotUnique:
                    ;
                }
            }

            foreach (string strUnique in alUnique)
            {
                Console.Write("{0} ", strUnique);
            }

            Console.WriteLine("");
            Console.WriteLine("__________________________");
            Console.WriteLine("");

            if (m_fOkBruteForceIt)
                if (snowballs.Count == 0)
                {
                    // probably still must add everyone not at a higher levle.. but ok

                    fStillGoing = false;
                }
        }

        return alAllLevels;

    }

    */

static Bitmap Frost(int iWide, int iHigh, Bitmap objBitmapCOL)
    {
        Bitmap objBitmapNEW = new Bitmap(iWide, iHigh);

        Color coBit = new Color();
        Color coNeighborBit = new Color();

        for (int iXPos = 1; iXPos < iWide - 1; iXPos++)
        {
            for (int iYPos = 1; iYPos < iHigh - 1; iYPos++)
            {
                coBit = objBitmapCOL.GetPixel(iXPos, iYPos);
                int iXNeighbor;
                int iYNeighbor;
                bool fDifferent = false;

                for (int iNeighbor = 0; iNeighbor < 4; iNeighbor++)
                {
                    iXNeighbor = iXPos;
                    iYNeighbor = iYPos;

                    switch (iNeighbor)
                    {
                        case 0:
                            iXNeighbor = iXPos - 1;
                            break;

                        case 1:
                            iXNeighbor = iXPos + 1;
                            break;

                        case 2:
                            iYNeighbor = iYPos - 1;
                            break;

                        case 3:
                            iYNeighbor = iYPos + 1;
                            break;
                    }

                    coNeighborBit = objBitmapCOL.GetPixel(iXNeighbor, iYNeighbor);
                    if (coBit != coNeighborBit)
                    {
                        fDifferent = true;
                        break;
                    }
                }

                if (fDifferent)
                {
                    //				coBit = Color.FromArgb(100, (coBit.R + 255 ) / 2, (coBit.G + 255 ) / 2, (coBit.B + 255 ) / 2) ;
                    int newR = FrostyWiggle(coBit.R);
                    int newG = FrostyWiggle(coBit.G);
                    int newB = FrostyWiggle(coBit.B);
                    coBit = Color.FromArgb(100, newR, newG, newB);
                }

                objBitmapNEW.SetPixel(iXPos, iYPos, coBit);
            }
        }
        return objBitmapNEW;
    }

    static int FrostyWiggle(int iStartAt)
    {
        Random rd = new Random();

        int half = iStartAt + 255 / 2;
        int halfhalf = (iStartAt + half) / 2;
        int halfhalfhalf = (iStartAt + halfhalf) / 2;
        int wiggle = halfhalfhalf + (rd.Next() % 30) - 15;
        if (wiggle > 255)
            wiggle = 255;
        if (wiggle < 0)
            wiggle = 0;
        return wiggle;
    }




    static Bitmap AddCobwebs(int iWide, int iHigh, ArrayList alLjuPlaced, Bitmap objBitmap, string outFileName)
    {
        // i want to draw a new bitmap.  a line from each friend to each friend (two-way).  Then I'll bit-copy from the old bitmap, its words.
        Bitmap objBitmapWithLines = new Bitmap(iWide, iHigh);
        Graphics objGraphicsForLines = Graphics.FromImage(objBitmapWithLines);
        Pen pnGray = new Pen(Color.FromArgb(10, 95, 95, 95)); // darker than the 100's from before.
        Pen pnDarkGray = new Pen(Color.FromArgb(10, 40, 40, 40)); // darker than the 100's from before.

        foreach (LJUser2 ljuFromU in alLjuPlaced)
        {
            foreach (LJUser2 ljuToU in alLjuPlaced)
            {
                // cobwebs to the seed are darker.  almost invisible.
                Pen pn = pnGray;
                LJUser2 ljuSeed = (LJUser2)alLjuPlaced[0];
                if (ljuFromU.Name == ljuSeed.Name)
                    pn = pnDarkGray;

                if (ljuToU.Name == ljuSeed.Name)
                    pn = pnDarkGray;

                if (ljuFromU.Reads(ljuToU))
                    if (ljuToU.Reads(ljuFromU))
                    {
                        Point ptFrom = new Point(ljuFromU.rect.X + ljuFromU.rect.Width / 2,
                                                ljuFromU.rect.Y + ljuFromU.rect.Height / 2);
                        Point ptTo = new Point(ljuToU.rect.X + ljuToU.rect.Width / 2,
                                             ljuToU.rect.Y + ljuToU.rect.Height / 2);
                        objGraphicsForLines.DrawLine(pn, ptFrom, ptTo);
                    }
            }
        }

        Color coBit = new Color();
        for (int iTwoPass = 0; iTwoPass < 2; iTwoPass++)
        {
            for (int iXPos = 0; iXPos < iWide; iXPos++)
            {
                for (int iYPos = 0; iYPos < iHigh; iYPos++)
                {
                    coBit = objBitmap.GetPixel(iXPos, iYPos);
                    if (coBit.R == 0)
                        if (coBit.G == 0)
                            if (coBit.B == 0)
                                continue;

                    if (iTwoPass == 0)
                    {
                        if (iXPos < iWide && iYPos < iHigh && iXPos > 0 && iYPos > 0)
                        {
                            objBitmapWithLines.SetPixel(iXPos + 1, iYPos - 1, Color.Black);
                            objBitmapWithLines.SetPixel(iXPos + 1, iYPos, Color.Black);
                            objBitmapWithLines.SetPixel(iXPos + 1, iYPos + 1, Color.Black);
                            objBitmapWithLines.SetPixel(iXPos, iYPos + 1, Color.Black);
                            objBitmapWithLines.SetPixel(iXPos - 1, iYPos + 1, Color.Black);
                        }
                    }
                    else
                    {
                        objBitmapWithLines.SetPixel(iXPos, iYPos, coBit);
                    }
                }
            }
        }
        objBitmapWithLines.Save(outFileName, ImageFormat.Gif);
        return objBitmapWithLines;
    }

    static double Distance(Rectangle rc1, Rectangle rc2)
    {
        int idx = (rc2.Left + rc2.Width / 2) - (rc1.Left + rc1.Width / 2);
        int idy = (rc2.Top + rc2.Height / 2) - (rc1.Top + rc1.Height / 2);
        return System.Math.Sqrt((double)(idx * idx + idy * idy));
    }
    
    static int PullToward(int iAt, int iToward)
    {
        int iMax = Math.Max(iAt, iToward);
        int iMin = Math.Min(iAt, iToward);
        if (iMax > iMin + 128)
            iMin += 256;

        int iHalf = (iMin + iMax) / 2;

        iMax = Math.Max(iAt, iHalf);
        iMin = Math.Min(iAt, iHalf);
        if (iMax > iMin + 128)
            iMin += 256;

        int iRet = (iMin + iMax) / 2;

        //	Console.WriteLine("An item at {0} was pulled toward {1}, resulting in {2}.", iAt, iToward, iRet ) ;

        return iRet;


        // a 255-based colorwheel.
        /*
        // average comes to mind but must be adjusted...
        // 128 = 180degrees
        if (iAt > 128)
            iAt -= 256 ;
        if (iToward > 128)
            iToward =- 256 ;

        int iRet = (iAt + iToward) / 2 ;
        if (iRet < 0)
            iRet += 256 ;

        return iRet ;
        */
    }


    /*
    static bool IsCloser( int iAt, int iPlusDist, int iAwayFrom )
    {
        // does adding the plusDist bring iAt closer to iAwayFrom?
        int iMax = Math.Max( iAt, iAwayFrom ) ;
        int iMin = Math.Min( iAt, iAwayFrom ) ;
        if (iMax > iMin + 128)
            iMin += 256 ;

	
    }
    */

    // PushAway only does something if the two items are within 90 degrees of one another. 
    // if so, half the distance is pushed APART.

    static int PushAway(int iAt, int iAwayFrom)
    {
        int iMax = Math.Max(iAt, iAwayFrom);
        int iMin = Math.Min(iAt, iAwayFrom);
        bool fAtIsMax = false;
        if (iMax == iAt)
            fAtIsMax = true;

        if (iMax > iMin + 128)
            iMin += 256; // and the min is now max, har har.

        // we are dealing in the positive realm now.
        // so we have a numeric relationship.
        // is it within the closeness range we touch?
        if (Math.Abs(iMax - iMin) > 64)
            return iAt; // no change.

        // it's within 90degrees.  push iAt half the distance AWAY from iAwayFrom
        int middle = (iMin + iMax) / 2;

        int iDist = middle - iMin;

        if (fAtIsMax)
            iAt += iDist;
        else
            iAt -= iDist;

        return iAt;
    }




    static int DistanceBetween(Rectangle rc1, Rectangle rc2)
    {
        int x1 = rc1.X + rc1.Width / 2;
        int y1 = rc1.Y + rc1.Height / 2;
        int x2 = rc2.X + rc2.Width / 2;
        int y2 = rc2.Y + rc2.Height / 2;

        int idx = x2 - x1;
        int idy = y2 - y1;

        return (int)System.Math.Sqrt((double)(idx * idx + idy * idy));
    }

    static void DrawLJUser(LJUser2 ljuToShow,
                            Rectangle rectCore,
                            ArrayList alLjuPlaced,
                            Font font,
                            Graphics objGraphics,
                            bool fTitle,
                            int iTribeNum,
                            bool fColor,
                            bool fSeed,
                            bool fSuppressColor,
                            bool fAppendCollection)
    {
        DrawLJUser(ljuToShow,
                    rectCore,
                    alLjuPlaced,
                    font,
                    objGraphics,
                    fTitle,
                    iTribeNum,
                    fColor,
                    fSeed,
                    fSuppressColor,
                    fAppendCollection,
                    null);
    }

    static void DrawLJUser(LJUser2 ljuToShow,
                            Rectangle rectCore,
                            ArrayList alLjuPlaced,
                            Font font,
                            Graphics objGraphics,
                            bool fTitle,
                            int iTribeNum,
                            bool fColor,
                            bool fSeed,
                            bool fSuppressColor,
                            bool fAppendCollection,
                            int? iMandatedColor) // , bool fColorFreebie ) // -1 if no tribenum
    {
        // Once we find the right rect, we store it, and use it.
        LJUser2 ljuPlacing = new LJUser2();
        ljuPlacing.Clone(ljuToShow);
        ljuPlacing.rect = rectCore;
        //	alLjuPlaced.Add( ljuPlacing ) ;   moved down to after color calc

        // black shadow
        SolidBrush br = new SolidBrush(Color.FromArgb(0, 0, 0));
        objGraphics.DrawString(ljuPlacing.Name, font, br, rectCore.Left + 1, rectCore.Top + 1);
        objGraphics.DrawString(ljuPlacing.Name, font, br, rectCore.Left + 1, rectCore.Top);
        objGraphics.DrawString(ljuPlacing.Name, font, br, rectCore.Left, rectCore.Top + 1);
        objGraphics.DrawString(ljuPlacing.Name, font, br, rectCore.Left - 1, rectCore.Top);

        // off-white content
        /*
        int iRedden = 0 ;
        if (fTitle)
            iRedden = 50 ;
            */


        // SolidBrush [] threeBrushes ; //= null ;
        ArrayList threeBrushes = new ArrayList();

        if (-1 == iTribeNum)
        {
            /*
            if (false == fColor)
                {
                threeBrushes.Add ( new SolidBrush( Color.FromArgb (205 + iRedden, 205 - iRedden, 205 - iRedden))) ;
                threeBrushes.Add( new SolidBrush( Color.FromArgb (185 + iRedden, 185 - iRedden, 185 - iRedden))) ;
                threeBrushes.Add( new SolidBrush( Color.FromArgb (155 + iRedden, 155 - iRedden, 155 - iRedden))) ;
                }
                */

            // for color version, i calculate a color based off the colorwheel and friendship relations.
            // i start with a random color.

            int iCol = ljuToShow.color;

            if (iCol == 0)
            {
                Random rd = new Random();
                iCol = rd.Next() % 256;
            }

            // IF we have placed three people, then we start averaging colors
            // place the top tier!
            //		if (alLjuPlaced.Count > 3)
            //		Console.WriteLine( ljuToShow.Tier.ToString()  ) ;
            //		if (ljuToShow.Tier > 1)
            {

                // examine people placed so far.
                // order the list by distance from me!
                List<LJUser2> sortedPlaced = new List<LJUser2>() ;
                foreach (LJUser2 lju in alLjuPlaced)
                {
                    lju.distanceAway = DistanceBetween(lju.rect, rectCore);
                    //				Console.WriteLine(lju.Name + " is " + lju.distanceAway + " from " + ljuToShow.Name) ;
                    sortedPlaced.Add(lju);
                }

                /*
                                 m_iSortBy = SB_DISTANCE;

	public int CompareTo(object obj)
	{
		if (obj is LJUser2)
			{
			LJUser2 ljuThem = (LJUser2) obj ;
			if (GrabClass.SB_DISTANCE == GrabClass.m_iSortBy)
				return ljuThem.distanceAway.CompareTo( this.distanceAway ) ;
//				return this.distanceAway.CompareTo( ljuThem.distanceAway ) ;

			if (GrabClass.SB_READERS == GrabClass.m_iSortBy) 
				return ljuThem.whoIReadNumeric.Count.CompareTo( this.whoIReadNumeric.Count ) ;
			
//			return fl.m_iShips.CompareTo(this.m_iShips) ;
			}
		throw new ArgumentException("Object is not a CFleet.") ;
	}
                 * */

                // sort this list by distance away

                var newSortedPlaced = from item in sortedPlaced orderby item.distanceAway descending select item ;
                                      sortedPlaced = newSortedPlaced.ToList<LJUser2>() ;

//                sortedPlaced.Sort();

                //			Console.WriteLine("") ;
                //			Console.WriteLine("For " + ljuToShow.Name) ;

                foreach (LJUser2 ljuPlaced in sortedPlaced)
                {
                    //			Console.Write( ljuPlaced.Name  + " " ) ;
                    // don't let the color of the seed influence me.
                    // THIS JUST MOVES THE INFLUENCE TO THE SECOND SEED.  REALLY WE NEED THE WHOLE TOP TIER TO BE RANDOM OR NEARLY RANDOM.
                    // Perhaps it's reasonable to be influenced (or not influenced) by the immediately previous placement.
                    // Oh, wait, perhaps this is good.  perhaps it cleaves hemispheres.
                    // Ok, so all tier ones adjust color against each other, not the seed.  but outside tier one, all are considered, giving (perhaps)
                    // some seed tinting to the world.  fat chance.
                    LJUser2 ljuSeed = (LJUser2)alLjuPlaced[0];
                    // so tier 1's ignore the center.  let's stop this for a test.  they should pull toward center but away from each other.
                    //				if (ljuSeed.Name == ljuPlaced.Name)
                    //					if (ljuToShow.Tier == 1)
                    //						continue ;

                    // Items in alLjuPlaced are clones, but they do have WhoIRead data.
                    // SHOULD NEVER HAPPEN.
                    if (ljuPlaced.Name == ljuToShow.Name)
                        continue;

                    if (true == fColor) // || true == fColorFreebie )
                    {

                        int iCloseness = 0;
                        if (ljuPlaced.Reads(ljuToShow))
                            iCloseness++;

                        if (ljuToShow.Reads(ljuPlaced))
                            iCloseness++;

                        // I had thought of pulling 1/2 on 2 pts, 1/4 on 1... but i will just pull 1/2 for each here for now.
                        // or maybe just 1/4, we shall see if this "overtrends"
                        if (iCloseness == 0)
                            iCol = PushAway(iCol, ljuPlaced.color);
                        else
                        {
                            for (int iPullPass = 0; iPullPass < iCloseness; iPullPass++)
                            {
                                iCol = PullToward(iCol, ljuPlaced.color);
                            }
                        }
                    }
                }
            }

            if (true == fColor) // || true == fColorFreebie )
            {
                // reign in the extremes! This could happen in ColorConvertHSVtoRGB, but it doesn't.
                if (iCol >= 256)
                    iCol -= 256;

                if (iCol < 0)
                    iCol += 256;

                if (iMandatedColor != null)
                    iCol = (int)iMandatedColor;

                ljuPlacing.color = iCol;
            }

            // TIME TO TAKE A RISK: We are adding this item twice.  Let's add it only on the COLOR pass.  Always the second pass.  This gets first pass zeros out!
            if (true == fColor && true == fAppendCollection)
                alLjuPlaced.Add(ljuPlacing);

            if (true == fColor && false == fSuppressColor) // || true == fColorFreebie )
            {
                int s1 = 255;
                int s2 = 175;
                int s3 = 145;
                int v1 = 255;
                int v2 = 255;
                int v3 = 255;
                /*			if (fColorFreebie) 
                                {
                                s1 /= 2 ;
                                s2 /= 2 ;
                                s3 /= 2 ;
                                v1 = 205 ;
                                v2 = 185 ;
                                v3 = 155 ;
                                }
                                */

                ColorConvert.RGB col1 = ColorConvert.HSVtoRGB(iCol, s1, v1);
                ColorConvert.RGB col2 = ColorConvert.HSVtoRGB(iCol, s2, v2);
                ColorConvert.RGB col3 = ColorConvert.HSVtoRGB(iCol, s3, v3);
                threeBrushes.Add(new SolidBrush(Color.FromArgb(col1.Red, col1.Green, col1.Blue)));
                threeBrushes.Add(new SolidBrush(Color.FromArgb(col2.Red, col2.Green, col2.Blue)));
                threeBrushes.Add(new SolidBrush(Color.FromArgb(col3.Red, col3.Green, col3.Blue)));
            }
            else
            {
                // the black and white old code
                int iRedden = 0;
                if (fTitle)
                    iRedden = 50;

                threeBrushes.Add(new SolidBrush(Color.FromArgb(205 + iRedden, 205 - iRedden, 205 - iRedden)));
                threeBrushes.Add(new SolidBrush(Color.FromArgb(185 + iRedden, 185 - iRedden, 185 - iRedden)));
                threeBrushes.Add(new SolidBrush(Color.FromArgb(155 + iRedden, 155 - iRedden, 155 - iRedden)));
            }
        }
        else
        {
            Color[] colMap = { 
				Color.AliceBlue,
				Color.AntiqueWhite,
				Color.Aqua,
				Color.Aquamarine,
				Color.Brown,
				Color.BurlyWood,
				Color.CadetBlue,
				Color.Chartreuse,
				Color.Chocolate,
				Color.CornflowerBlue,
				Color.Crimson,
				Color.DarkCyan,
				Color.DarkGoldenrod,
				Color.DarkGray,
				Color.DarkKhaki,
				Color.DarkOrange,
				Color.DarkSalmon,
				Color.DarkSeaGreen,
				Color.DarkTurquoise,
				Color.DeepPink,
				Color.DeepSkyBlue,
				Color.DimGray,
				Color.DodgerBlue,
				Color.Firebrick,
				Color.FloralWhite,
				Color.ForestGreen,
				Color.Fuchsia,
				Color.Gainsboro,
				Color.GhostWhite,
				Color.Gold,
				Color.Goldenrod,
				Color.Gray,
				Color.Green,
				Color.GreenYellow,
				Color.Honeydew,
				Color.HotPink,
				Color.IndianRed,
				Color.Indigo,
				Color.Ivory,
				Color.Khaki,
				Color.Lavender,
				Color.LavenderBlush,
				Color.LawnGreen,
				Color.LemonChiffon,
				Color.LightBlue,
				Color.LightCoral,
				Color.LightCyan,
				Color.LightGoldenrodYellow,
				Color.LightGray,
				Color.LightGreen,
				Color.LightPink,
				Color.LightSalmon,
				Color.LightSeaGreen,
				Color.LightSkyBlue,
				Color.LightSlateGray,
				Color.LightSteelBlue,
				Color.LightYellow,
				Color.Lime,
				Color.LimeGreen,
				Color.Linen,
				Color.Magenta,
				Color.Maroon,
				Color.MediumAquamarine,
				Color.MediumBlue,
				Color.MediumOrchid,
				Color.MediumPurple,
				Color.MediumSeaGreen,
				Color.MediumSlateBlue,
				Color.MediumSpringGreen,
				Color.MediumTurquoise,
				Color.MediumVioletRed,
				Color.MidnightBlue,
				Color.MintCream,
				Color.MistyRose,
				Color.Moccasin,
				Color.NavajoWhite,
				Color.Navy,
				Color.OldLace,
				Color.Olive,
				Color.OliveDrab,
				Color.Orange,
				Color.OrangeRed,
				Color.Orchid,
				Color.PaleGoldenrod,
				Color.PaleGreen,
				Color.PaleTurquoise,
				Color.PaleVioletRed,
				Color.PapayaWhip,
				Color.PeachPuff,
				Color.Peru,
				Color.Pink,
				Color.Plum,
				Color.PowderBlue,
				Color.Purple,
				Color.Red,
				Color.RosyBrown,
				Color.RoyalBlue,
				Color.SaddleBrown,
				Color.Salmon,
				Color.SandyBrown,
				Color.SeaGreen,
				Color.SeaShell,
				Color.Sienna,
				Color.Silver,
				Color.SkyBlue,
				Color.SlateBlue,
				Color.SlateGray,
				Color.Snow,
				Color.SpringGreen,
				Color.SteelBlue,
				Color.Tan,
				Color.Teal,
				Color.Thistle,
				Color.Tomato,
				Color.Transparent,
				Color.Turquoise,
				Color.Violet,
				Color.Wheat,
				Color.White,
				Color.WhiteSmoke,
				Color.Yellow,
				Color.YellowGreen };

            // eventually these break when there are too many placed tribes.
            threeBrushes.Add(new SolidBrush(colMap[iTribeNum]));
            threeBrushes.Add(new SolidBrush(colMap[iTribeNum]));
            threeBrushes.Add(new SolidBrush(colMap[iTribeNum]));
        }

        // kind of a wacko just-in-case moment
        int iBrush = 0;
        iBrush = ljuToShow.Tier;
        if (iBrush > 2)
            iBrush = 2;
        objGraphics.DrawString(ljuPlacing.Name, font, (SolidBrush)threeBrushes[iBrush], rectCore.Left, rectCore.Top);

        /*
        if (ljuPlacing.Location != null)
            if (ljuPlacing.Location != "")
                {
                Pen pn = new Pen( Color.FromArgb(10, 64, 128, 128) ) ;
                Rectangle innerRect = rectCore ;
                innerRect.X += 1 ;
                innerRect.Y += 1 ;
                innerRect.Width -= 1 ;
                innerRect.Height -= 1 ;
		
                objGraphics.DrawRectangle( pn, innerRect ) ;
                }
                */

        if (false == fSeed)
            if (ljuPlacing.Url != null)
            /* THE OLD BOX.
            {
            Pen pn = new Pen( Color.FromArgb(10, 115, 110, 110) ) ;
            objGraphics.DrawRectangle( pn, rectCore ) ;
            }
            */

            ///* I don't like the way they look.
            //	if (ljuPlacing.Location != null)
            //		if (ljuPlacing.Location != "")
            // THE NEW BOX (FIRST TRIED FOR COOLTIPS)
            {
                Pen pn2 = new Pen(Color.FromArgb(10, 40, 90, 90));
                Rectangle smaller = Rectangle.Inflate(rectCore, -1, -1);

                // move everyone one pixel left.
                smaller.X -= 1;

                // just do four corners.  DO THEM 9 PIXELS NOW
                objGraphics.DrawLine(pn2, smaller.X, smaller.Y, smaller.X + 9, smaller.Y);
                objGraphics.DrawLine(pn2, smaller.X, smaller.Y, smaller.X, smaller.Y + 9);

                objGraphics.DrawLine(pn2, smaller.X + smaller.Width, smaller.Y, smaller.X + smaller.Width - 9, smaller.Y);
                objGraphics.DrawLine(pn2, smaller.X + smaller.Width, smaller.Y, smaller.X + smaller.Width, smaller.Y + 9);

                objGraphics.DrawLine(pn2, smaller.X, smaller.Y + smaller.Height, smaller.X + 9, smaller.Y + smaller.Height);
                objGraphics.DrawLine(pn2, smaller.X, smaller.Y + smaller.Height, smaller.X, smaller.Y + smaller.Height - 9);

                objGraphics.DrawLine(pn2, smaller.X + smaller.Width, smaller.Y + smaller.Height, smaller.X + smaller.Width - 9, smaller.Y + smaller.Height);
                objGraphics.DrawLine(pn2, smaller.X + smaller.Width, smaller.Y + smaller.Height, smaller.X + smaller.Width, smaller.Y + smaller.Height - 9);

                //			objGraphics.DrawRectangle( pn2, smaller ) ;
            }


        // + iTribeNum.ToString()
        if (fTitle)
        {
            Pen pn = new Pen(Color.FromArgb(10, 200, 200, 200));
            objGraphics.DrawRectangle(pn, rectCore);
        }

    }


    static bool RectCollision(ArrayList alLjuObjects, Rectangle rc)
    {
        foreach (LJUser2 lju in alLjuObjects)
            if (lju.rect.IntersectsWith(rc))
                return true;

        return false;
    }


    /*
    static ArrayList GrowSet( ArrayList set, ref ArrayList coreTwoWayers )
    {
            Console.WriteLine( "There are {0} sets containing and {1} twoWayers still alive.", set.Count, coreTwoWayers.Count ) ;
            ArrayList firstSet = (ArrayList) set[0] ;
            Console.WriteLine("Each set has {0} members.", firstSet.Count ) ;
		
            ArrayList newSet = new ArrayList() ;
            ArrayList stillAliveTwoWays = new ArrayList() ;
		
            foreach ( string name in coreTwoWayers)
                {
                foreach( ArrayList oneSet in set )
                    {
                    foreach( string strMember in oneSet)
                        {
                        if (m_fAbortNow)
                            return null ;

                        // let's determine if this user is in this set.  that prevents us from seeing if
                        // any users read any other, an expensive operation.

                        if ( strMember.ToUpper() == name.ToUpper() )
                            goto SkipSelfHereNow ;
                        }

                    // Having examined the entire set, we determined this name is not
                    // in this set.  Now we may consider whether the set members are all
                    // mutual readers for the name.
                    // as an optimization, we break this into two steps:
                    // first, does this name read all the members of this set?
                    // the reader list is likely to be lengthier, so we iterate through it,
                    // checking each item.  if the item is a member, we note that.
                    // if not everyone gets read, then we know this name doesn't
                    // read at least one of these members... a failure.

                    ArrayList alFoundMembers = new ArrayList() ;
                    LJUser2 lju = olCustomUserList.GetUser ( name ) ;
                    foreach( string strIRead in lju.whoIRead )
                        {
                        if (m_fAbortNow)
                            return null ;
					
                        foreach( string strMember in oneSet )
                            if (strIRead.ToUpper() == strMember.ToUpper())
                                {
                                alFoundMembers.Add( strMember ) ;
                                break ;
                                }
                        }
				
                    if (alFoundMembers.Count < oneSet.Count)
                        goto NotConnectedHereNow ;

                    // If we get this far, then the name in quesiton reads all the
                    // users within this set.  The next question is: do they all read
                    // this name?

                    foreach( string strMember in oneSet )
                        {
                        if (m_fAbortNow)
                            return null ;
					
                        LJUser2 ljUs = olCustomUserList.GetUser( strMember ) ;
                        if (false == ljUs.Reads( name ))
                            goto NotConnectedHereNow ;
                        }


                    // if we pop out without skipping this part, then we have gold. Document it.
                    ArrayList grouping = new ArrayList() ;
                    grouping.Add( name ) ;
                    foreach( string strMember in oneSet)
                        {
                        if (m_fAbortNow)
                            return null ;
					
                        grouping.Add( strMember ) ;

                        // we also protect these members from being removed from the coreTwoWay list.
                        // or rather, add them to the pool for the next go-round
                        foreach( string strAlready in stillAliveTwoWays)
                            {
                            if (strAlready.ToUpper() == strMember.ToUpper())
                                goto DontAddThis ;
                            }
                        stillAliveTwoWays.Add( strMember ) ;
                        DontAddThis:
                            ;
                        }

                    // we have a major issue with complexity caused by duplicate entries.
                    // so see if this grouping appears in this set in any order, before adding
                    foreach( ArrayList setsWeHaveSoFar in newSet)
                        {
                        if (m_fAbortNow)
                            return null ;
					
                        if (SubsetsEssentiallySame( setsWeHaveSoFar, grouping ))
                            goto SkipThisHereNow ;
                        }
                    newSet.Add( grouping ) ;

                    // So actually just three in a "quad".

                    NotConnectedHereNow:
                    SkipSelfHereNow:
                    SkipThisHereNow:
                    ;
                    }
                }

        coreTwoWayers = stillAliveTwoWays ; 
        return newSet ;
    }

    */



    static ArrayList GrowSetNumeric(ArrayList set, ref ArrayList coreTwoWayers)
    {
        Console.WriteLine("There are {0} sets containing and {1} twoWayers still alive.", set.Count, coreTwoWayers.Count);
        ArrayList firstSet = (ArrayList)set[0];
        Console.WriteLine("Each set has {0} members.", firstSet.Count);

        // listen, liberal calc time is dead today.
        m_fLiberalCalcTime = false;
        if (false == m_fLiberalCalcTime)
            if (3000000 < set.Count * coreTwoWayers.Count * firstSet.Count)
            {
                Console.WriteLine("Calculation aborted due to complexity failure.");
                // m_fAbortNow is never SET when brute forcing it.
                //				if (false == m_fOkBruteForceIt )
                m_fAbortNow = true;
                return null;
            }

        ArrayList newSet = new ArrayList();
        ArrayList stillAliveTwoWays = new ArrayList();

        foreach (int name in coreTwoWayers)
        {
            foreach (ArrayList oneSet in set)
            {
                foreach (int strMember in oneSet)
                {
                    if (m_fAbortNow)
                        return null;

                    // let's determine if this user is in this set.  that prevents us from seeing if
                    // any users read any other, an expensive operation.

                    if (strMember == name)
                        goto SkipSelfHereNow;
                }

                // Having examined the entire set, we determined this name is not
                // in this set.  Now we may consider whether the set members are all
                // mutual readers for the name.
                // as an optimization, we break this into two steps:
                // first, does this name read all the members of this set?
                // the reader list is likely to be lengthier, so we iterate through it,
                // checking each item.  if the item is a member, we note that.
                // if not everyone gets read, then we know this name doesn't
                // read at least one of these members... a failure.

                ArrayList alFoundMembers = new ArrayList();
                //				LJUser2 lju = olCustomUserList.GetUserNumeric ( name ) ;
                LJUser2 lju = (LJUser2)olCustomUserList.GetUserByID(name); // this is the user's number
                foreach (int strIRead in lju.whoIReadNumeric)
                {
                    if (m_fAbortNow)
                        return null;

                    foreach (int strMember in oneSet)
                        if (strIRead == strMember)
                        {
                            alFoundMembers.Add(strMember);
                            break;
                        }
                }

                if (alFoundMembers.Count < oneSet.Count)
                    goto NotConnectedHereNow;

                // If we get this far, then the name in quesiton reads all the
                // users within this set.  The next question is: do they all read
                // this name?

                foreach (int strMember in oneSet)
                {
                    if (m_fAbortNow)
                        return null;

                    LJUser2 ljUs = (LJUser2)olCustomUserList.GetUserByID(strMember);
                    LJUser2 ljThem = (LJUser2)olCustomUserList.GetUserByID(name);
                    if (false == ljUs.Reads(ljThem.Name))
                        goto NotConnectedHereNow;
                }


                // if we pop out without skipping this part, then we have gold. Document it.
                ArrayList grouping = new ArrayList();
                grouping.Add(name);
                foreach (int strMember in oneSet)
                {
                    if (m_fAbortNow)
                        return null;

                    grouping.Add(strMember);

                    // we also protect these members from being removed from the coreTwoWay list.
                    // or rather, add them to the pool for the next go-round
                    foreach (int strAlready in stillAliveTwoWays)
                    {
                        if (strAlready == strMember)
                            goto DontAddThis;
                    }
                    stillAliveTwoWays.Add(strMember);
                DontAddThis:
                    ;
                }

                // we have a major issue with complexity caused by duplicate entries.
                // so see if this grouping appears in this set in any order, before adding
                foreach (ArrayList setsWeHaveSoFar in newSet)
                {
                    if (m_fAbortNow)
                        return null;

                    if (SubsetsEssentiallySameNumeric(setsWeHaveSoFar, grouping))
                        goto SkipThisHereNow;
                }
                newSet.Add(grouping);

            // So actually just three in a "quad".

            NotConnectedHereNow:
            SkipSelfHereNow:
            SkipThisHereNow:
                ;
            }
        }

        coreTwoWayers = stillAliveTwoWays;
        return newSet;

    }



    static bool SubsetsEssentiallySameNumeric(ArrayList al1, ArrayList al2)
    {
        if (al1.Count != al2.Count)
            Console.WriteLine("This assumes arraylists same legnth so will screw up for you now.");

        foreach (int str1 in al1)
        {
            foreach (int str2 in al2)
            {
                if (str1 == str2)
                    goto DoNext1;
            }

            // item from set one was not found in set two.
            // fail.
            // WE DO NOT CHECK IF ITEM IN STR1 IS NOT IN STR2... WE ASSUME CONSTRAINTS OF THIS APP.
            return false;

        DoNext1:
            ;
        }

        return true;
    }



    static bool SubsetsEssentiallySame(ArrayList al1, ArrayList al2)
    {
        if (al1.Count != al2.Count)
            Console.WriteLine("This assumes arraylists same legnth so will screw up for you now.");

        foreach (string str1 in al1)
        {
            foreach (string str2 in al2)
            {
                if (str1 == str2)
                    goto DoNext1;
            }

            // item from set one was not found in set two.
            // fail.
            // WE DO NOT CHECK IF ITEM IN STR1 IS NOT IN STR2... WE ASSUME CONSTRAINTS OF THIS APP.
            return false;

        DoNext1:
            ;
        }

        return true;
    }


    static LJUser2 getUser(string strName)
    {
        //	if (strName.ToUpper() == "HEPKITTEN")
        //		return getUserFromWebFDATA( strName ) ;

        LJUser2 lju = MasterDB.GetSlimUser(strName);
        if (null != lju)
            return lju;
        else
        //		return getUserFromWebFOAF( strName ) ;
        {
            return getUserFromWebFDATA(strName);
        }
    }


    public static LJUser2 getUserFromWebFDATA(string strName)
    {
        // try_again:
        /*
            string url = string.Format("http://www.livejournal.com/misc/fdata.bml?user=" + strName) ;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create( url );

            request.UserAgent = "http://ljmindmap.com/; ljm.x.jrd@neverbox.com" ;
            */

        LJUser2 lju = new LJUser2();
        lju.Name = strName;

        // we have to poplate this user's reader list.
        string fd = FData.GetFData(strName);
        if (null == fd)
            return null;

        HashSet<Int32> ifd = FData.IDsInIReadFData(fd);
        var oppoSet = from dude in FData.IDsInTheyReadMeFData(fd) select -dude;
        ifd.UnionWith(oppoSet);
        fd = null;

        lju.ifd = ifd;

        /*
        lju.whoIRead = null;
        Regex rIRead = new Regex(@"> \w+\n");
        Match m = rIRead.Match(fd);

        if (lju.whoIRead == null)
            lju.whoIRead = new ArrayList();

        while (m.Success)
        {
            string who = m.ToString().Trim().Substring(2);
            lju.whoIRead.Add(who);
            m = m.NextMatch();
        }
         * */

        return lju;
    }

    /*
    static int WaitOnRequestFDATA( LJUser2 lju, WebRequest request )
    {
        m_fRetryAfterDelay = false ;
	
        WebResponse response ;
        try
            {
        response = request.GetResponse();
            }
        catch( WebException we )
            {
            Console.WriteLine("While scraping for: " + lju.Name) ;
            string strErr = we.ToString() ;
            Console.WriteLine( strErr ) ;
            if (-1 != we.ToString().IndexOf("The operation has timed-out."))
                {
                Console.WriteLine("I DETECTED THE TIMEOUT OPERATION.") ;
                }

            if (-1 != we.ToString().IndexOf("Internal Server Error."))
                {
                Console.WriteLine("I DETECTED INTERNAL SERVER ERROR.") ;
                }

            if (-1 != we.ToString().IndexOf("Bad Gateway."))
                {
                Console.WriteLine("I DETECTED BAD GATEWAY.") ;
    //			m_fRetryAfterDelay = true ;
                }

            m_fRetryAfterDelay = true ; // let's go for it always.
		
            return -1 ;
            }
	
        Stream s = response.GetResponseStream();

        StreamReader sr = new StreamReader( s );
        string line;
    //	string strAllData = "" ;
    //	string strRawData = "" ;

    //	bool fNamePassed = false ;
        ArrayList theyreadme = new ArrayList()  ;

        try
            {
            while( (line = sr.ReadLine()) != null )
                {
    //			Console.WriteLine(">>>" + line ) ; // for diagnostic of the http failure.
                if (-1 != line.IndexOf("Sorry, database temporarily unavailable."))
                    {
        //			throw new Exception() ; // may as well just stop.  Need to determine if I detecct this error.  Cuz it appears to cause Scrape_failed
                    m_fDBDown = true ;
                    return -1 ;

                    }
			
                if (-1 != line.IndexOf("> "))
                    {
                    int iStart = 2 ; // line.IndexOf(">") ;
                    int iEnd = line.Length ; // line.IndexOf("</") ;
                    if (-1 == iEnd)
                        continue ;
                    if (-1 == iStart)
                        continue ;

    //				iStart++ ;
				
                    string nick = line.Substring(iStart, iEnd - iStart) ;
                    if (lju.whoIRead == null)
                        lju.whoIRead = new ArrayList() ;
                    lju.whoIRead.Add( nick ) ;
                    }

                // grab the otherways.  we can then determine how many items must bei n this
                // user's mindmap.
                if (-1 != line.IndexOf("< "))
                    {
                    int iStart = 2 ; // line.IndexOf(">") ;
                    int iEnd = line.Length ; // line.IndexOf("</") ;
                    if (-1 == iEnd)
                        continue ;
                    if (-1 == iStart)
                        continue ;

    //				iStart++ ;
				
                    string nick = line.Substring(iStart, iEnd - iStart) ;
                    theyreadme.Add( nick ) ;
                    }
                }

            // we read all the names.  now determine how many folks we talkin about.
            int count = 0 ;

            if (null == lju.whoIRead)
                return 0 ;
		
            foreach( string strIReadEm in lju.whoIRead )
                foreach( string theyme in theyreadme )
                    {
                    if (strIReadEm.ToUpper() == theyme.ToUpper())
                        count++ ;
                    }

            return count ;
            }
        catch( ArgumentOutOfRangeException e )
            {
            // this we eat but fail
            Console.WriteLine( e.ToString()) ;
            return -1 ;
            }
    // isn't working	catch( System.Web.WebException we )
        catch( Exception we )
            {
            Console.WriteLine( we.ToString()) ;
            return -1 ;
            }

    //	Console.WriteLine("Logic error!") ;
    //	return -1 ; // code paths must return a value.
    }
    */


    /*

    static LJUser2 getUserFromWebFOAF( string strName ) 
    {
    // www.livejournal.com/users/username/data/foaf
        string url = string.Format("http://www.livejournal.com/users/{0}/data/foaf", strName) ;
        WebRequest request = WebRequest.Create( url );

        LJUser2 lju = new LJUser2() ;
        lju.Name = strName ;

        if (WaitOnRequestFOAF( lju, request ))
            {
            MasterDB.Add( lju, false, null ) ; // from web, user has no tribe calculated.
            return lju ;
            }

        return null ;
    }


    static bool WaitOnRequestFOAF( LJUser2 lju, WebRequest request )
    {
        WebResponse response ;
        try
            {
        response = request.GetResponse();
            }
        catch( WebException we )
            {
            Console.WriteLine("While scraping for: " + lju.Name) ;
            Console.WriteLine( we.ToString()) ;
            if (-1 != we.ToString().IndexOf("The operation has timed-out."))
                {
                Console.WriteLine("I DETECTED THE TIMEOUT OPERATION.") ;
                }

            if (-1 != we.ToString().IndexOf("Internal Server Error."))
                {
                Console.WriteLine("I DETECTED INTERNAL SERVER ERROR.") ;
                }
		
            return false ;
            }
	
        Stream s = response.GetResponseStream();

        StreamReader sr = new StreamReader( s );
        string line;
    //	string strAllData = "" ;
    //	string strRawData = "" ;

    //	bool fNamePassed = false ;

        try
            {
            while( (line = sr.ReadLine()) != null )
                {
                // We care about foaf:nick lines between from first > to following <
                if (-1 != line.IndexOf("foaf:nick"))
                    {
                    int iStart = line.IndexOf(">") ;
                    int iEnd = line.IndexOf("</") ;
                    if (-1 == iEnd)
                        continue ;
                    if (-1 == iStart)
                        continue ;

                    iStart++ ;
				
                    string nick = line.Substring(iStart, iEnd - iStart) ;
                    if (lju.whoIRead == null)
                        lju.whoIRead = new ArrayList() ;
                    lju.whoIRead.Add( nick ) ;
                    }
                }
            }
        catch( ArgumentOutOfRangeException e )
            {
            // this we eat but fail
            Console.WriteLine( e.ToString()) ;
            return false ;
            }
	
        return true ;
    }
    */

    /*		

            if (false == fNamePassed)
                if (-1 != line.IndexOf("<b>User:</b>")) // Not everyone has a Name:, but does everyone have a User?
                    fNamePassed = true ;

            if (line == "  <title>Error</title>")
                return  false ;

            if (fNamePassed == true)
            {
                strRawData += line ;

                Regex r = new Regex("<[^>]*>");
                string [] realContent = r.Split(line) ;
                for (int i=0; i < realContent.GetLength(0); i++)
                {
                    strAllData += realContent[i] ;
                    strAllData += " " ;
                }
            }
        }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString()) ;
            return true ;
        }

        Console.Write("{0}, ", lju.Name) ;

        // First we parse Interests.  We like the fact that Interests with matches have a URL, so we parse from the
        // raw data.
        Regex ix = new Regex("<b><a href='/interests.bml'>Interests</a>:</b>") ;
        MatchCollection mp = ix.Matches(strRawData) ;
        int iAddMe = 10 ;
        if (mp.Count != 1)
            Console.Write("(NO INTERESTS?  MISSING USER?)  ") ;
        else
            iAddMe = mp[0].Index ;

        int iPos = iAddMe + "<b><a href='/interests.bml'>Interests</a>:</b>".Length ;

        // this user is dead, removed, gone.
        if (strRawData.Length == 0)
            return false ;

        int iPosEnd = strRawData.Substring(iPos).IndexOf("/friends'>Friends</a>:") ;
        if (-1 == iPosEnd)
            {
            iPosEnd = strRawData.Substring(iPos).IndexOf("/friends'>Members</a>:") ;
            if (-1 == iPosEnd)
                {
                // I bet this is a syndicated feed, which isn't going to work well for us!  bail!
                Console.Write("(SYNDICATED FEED?  BAIL!) ") ;
                return false ;
                }
            }
        string strInterests = strRawData.Substring(iPos, iPosEnd) ;

        Regex n = new Regex("(Friend of|Friends :|Birthdate:|Location:)") ;
        mp = n.Matches(strAllData) ;

        for( int i=0; i<mp.Count; i++)
        {
            // For each match, we yank out the yummy part.

            switch (mp[i].Value)
                {
                case "Interests":
                    iPos = mp[i].Index + "Interests".Length + 9 ;
                    iPosEnd = strAllData.Substring(iPos).IndexOf(":") ;
                    strInterests = strAllData.Substring(iPos, iPosEnd) ;
                    Console.WriteLine("Interests: {0}", strInterests) ;

                    break ;

                case "Friend of:":
                    iPos = mp[i].Index + "Friend of:".Length ;
                    iPosEnd = strAllData.Substring(iPos).IndexOf(":") ;
                    string strCount = strAllData.Substring(iPos, iPosEnd) ;
                    lju.Readers = int.Parse(strCount) ;

                    break ;

                case "Friends :":
                    iPos = mp[i].Index + "Friends :".Length ;
                    iPos = iPos + strAllData.Substring(iPos).IndexOf(":") + 1 ;
                    iPosEnd = strAllData.Substring(iPos).IndexOf(":") ;
                    string strFriends = strAllData.Substring(iPos, iPosEnd) ;
                    Regex rx = new Regex(" ?, ?") ;
                    string [] friendz = rx.Split(strFriends) ;
                    for (int iEach=0; iEach < friendz.GetLength(0); iEach++)
                    {
                        // if a friend starts with two spaces, they are dead.
    //					if ("  " == friendz[iEach].Substring(0, 2))
    //						continue ;

                        // the friend text is the first word, buttressed by whitespace.
                        Match m = Regex.Match(friendz[iEach], @"\s+\w+\s?") ;
                        if (m.Success)
                            {
                            string strFName = m.Value ;
                            strFName = strFName.TrimStart() ;
                            strFName = strFName.TrimEnd() ;
                            if (lju.whoIRead == null)
                                lju.whoIRead = new ArrayList() ;
                            lju.whoIRead.Add(strFName) ;
                            }
                    }
                    break ;
                case "Birthdate:":
                    iPos = mp[i].Index + "Birthdate:".Length ;
                    string strDate = strAllData.Substring(iPos, 13) ;
                    strDate = strDate.TrimStart() ;
                    try
                    {
                    lju.BDate = DateTime.Parse(strDate) ;
                    }
                    catch(Exception ) {}

                    break ;

                case "Location:":
                    iPos = mp[i].Index + "Location:".Length ;
    //				iPos = iPos + strAllData.Substring(iPos).IndexOf(":") + 1 ;
                    iPosEnd = strAllData.Substring(iPos).IndexOf(":") ;
                    string strLocation = strAllData.Substring(iPos, iPosEnd) ;
                    strLocation = strLocation.Substring(0, strLocation.LastIndexOf(' ')) ;
                    strLocation = strLocation.TrimStart() ;
                    strLocation = strLocation.TrimEnd() ;
                    lju.Location = strLocation ;
                    break ;
                }
        }

        return true ;
    //	lju.wr = null ;
    */




    static protected LJUser2 g_passLJU = null;

    static LJUser2 getUserFromWeb(string strName)
    {
        string url = string.Format("http://www.livejournal.com/userinfo.bml?user={0}&mode=full", strName);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        request.UserAgent = "http://ljmindmap.com/; ljm.x.jrd@neverbox.com";

        LJUser2 lju = new LJUser2();
        lju.Name = strName;
        //	lju.wr = request ;

        if (WaitOnRequest(lju, request))
        {
            // this function revived to get locations.  responsibility of caller to save this data to db.
            //		MasterDB.Add( lju, false, null ) ; // from web, user has no tribe calculated.
            return lju;
        }

        return null;
    }


    static bool WaitOnRequest(LJUser2 lju, WebRequest request)
    {
        WebResponse response;
        try
        {
            response = request.GetResponse();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }

        Stream s = response.GetResponseStream();

        StreamReader sr = new StreamReader(s);
        string line;
        string strAllData = "";
        string strRawData = "";

        try
        {
            bool fNamePassed = false;

            while ((line = sr.ReadLine()) != null)
            {
                if (false == fNamePassed)
                    if (-1 != line.IndexOf("<b>User:</b>")) // Not everyone has a Name:, but does everyone have a User?
                        fNamePassed = true;

                if (line == "  <title>Error</title>")
                    return false;

                if (fNamePassed == true)
                {
                    strRawData += line;

                    Regex r = new Regex("<[^>]*>");
                    string[] realContent = r.Split(line);
                    for (int i = 0; i < realContent.GetLength(0); i++)
                    {
                        strAllData += realContent[i];
                        strAllData += " ";
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return true;
        }

        Console.Write("{0}, ", lju.Name);

        // First we parse Interests.  We like the fact that Interests with matches have a URL, so we parse from the
        // raw data.
        Regex ix = new Regex("<b><a href='/interests.bml'>Interests</a>:</b>");
        MatchCollection mp = ix.Matches(strRawData);
        int iAddMe = 10;
        if (mp.Count != 1)
            Console.Write("(NO INTERESTS?  MISSING USER?)  ");
        else
            iAddMe = mp[0].Index;

        int iPos = iAddMe + "<b><a href='/interests.bml'>Interests</a>:</b>".Length;

        // this user is dead, removed, gone.
        if (strRawData.Length == 0)
            return false;

        int iPosEnd = strRawData.Substring(iPos).IndexOf("/friends'>Friends</a>:");
        if (-1 == iPosEnd)
        {
            iPosEnd = strRawData.Substring(iPos).IndexOf("/friends'>Members</a>:");
            if (-1 == iPosEnd)
            {
                // I bet this is a syndicated feed, which isn't going to work well for us!  bail!
                Console.Write("(SYNDICATED FEED?  BAIL!) ");
                return false;
            }
        }
        string strInterests = strRawData.Substring(iPos, iPosEnd);

        Regex n = new Regex("(Friend of|Friends :|Birthdate:|Location:)");
        mp = n.Matches(strAllData);

        for (int i = 0; i < mp.Count; i++)
        {
            // For each match, we yank out the yummy part.

            switch (mp[i].Value)
            {
                case "Interests":
                    iPos = mp[i].Index + "Interests".Length + 9;
                    iPosEnd = strAllData.Substring(iPos).IndexOf(":");
                    strInterests = strAllData.Substring(iPos, iPosEnd);
                    Console.WriteLine("Interests: {0}", strInterests);

                    break;

                case "Friend of:":
                    iPos = mp[i].Index + "Friend of:".Length;
                    iPosEnd = strAllData.Substring(iPos).IndexOf(":");
                    string strCount = strAllData.Substring(iPos, iPosEnd);
                    lju.Readers = int.Parse(strCount);

                    break;

                case "Friends :":
                    iPos = mp[i].Index + "Friends :".Length;
                    iPos = iPos + strAllData.Substring(iPos).IndexOf(":") + 1;
                    iPosEnd = strAllData.Substring(iPos).IndexOf(":");
                    string strFriends = strAllData.Substring(iPos, iPosEnd);
                    Regex rx = new Regex(" ?, ?");
                    string[] friendz = rx.Split(strFriends);
                    for (int iEach = 0; iEach < friendz.GetLength(0); iEach++)
                    {
                        // if a friend starts with two spaces, they are dead.
                        //					if ("  " == friendz[iEach].Substring(0, 2))
                        //						continue ;

                        // the friend text is the first word, buttressed by whitespace.
                        Match m = Regex.Match(friendz[iEach], @"\s+\w+\s?");
                        if (m.Success)
                        {
                            string strFName = m.Value;
                            strFName = strFName.TrimStart();
                            strFName = strFName.TrimEnd();
//                            if (lju.whoIRead == null)
  //                              lju.whoIRead = new ArrayList();
    //                        lju.whoIRead.Add(strFName);
                        }
                    }
                    break;
                case "Birthdate:":
                    iPos = mp[i].Index + "Birthdate:".Length;
                    string strDate = strAllData.Substring(iPos, 13);
                    strDate = strDate.TrimStart();
                    try
                    {
                        lju.BDate = DateTime.Parse(strDate);
                    }
                    catch (Exception) { }

                    break;

                case "Location:":
                    iPos = mp[i].Index + "Location:".Length;
                    //				iPos = iPos + strAllData.Substring(iPos).IndexOf(":") + 1 ;
                    iPosEnd = strAllData.Substring(iPos).IndexOf(":");

                    // i do not coddle this code. i just patch it up and hope.
                    if (iPosEnd == 0)
                        iPosEnd = 1; // whatever!

                    string strLocation = strAllData.Substring(iPos, iPosEnd);

                    // just a damn stupid error.
                    if (":" == strLocation)
                    {
                        Console.WriteLine("\0Stupid error of just a colon parsing out location and failing.");
                        break;
                    }

                    strLocation = strLocation.Substring(0, strLocation.LastIndexOf(' '));
                    strLocation = strLocation.Replace("Gizmo/LJ Talk", "");
                    strLocation = strLocation.TrimStart();
                    strLocation = strLocation.TrimEnd();

                    strLocation = strLocation.Replace(" ,  United States", "");
                    strLocation = strLocation.Replace("United States", "");
                    strLocation = strLocation.Replace(",  California", ", CA");
                    strLocation = strLocation.Replace(" ,  ", ", ");

                    strLocation = strLocation.Replace("\"", "'");

                    lju.Location = strLocation;
                    break;
            }
        }

        return true;
        //	lju.wr = null ;
    }

    /*
    static bool SameCity(string loc1, string loc2)
    {
        if (loc1 == loc2)
            return true ;

        return false ;
    }

    static bool SameDayAnyYear(DateTime dt1, DateTime dt2)
    {
        if (dt1.Year == 1)
            if (dt1.Day == 1)
                if (dt1.Month == 1)
                    return false ;

        if (dt1.Day == dt2.Day)
            if(dt1.Month == dt2.Month)
                return true ;

        return false ;
    }


    static void TalkPrettyAboutUs( LJUser2 lju, ArrayList olFriends )
    {
        DateTime MostRecentBirth = DateTime.Parse("1111-11-11") ;
        DateTime BirthLongestAgo = DateTime.Now ;
        foreach(LJUser2 ljuf in olFriends)
        {
            if (lju.Name == ljuf.Name)
                continue ;

            if (null != lju.Location)
                if (SameCity(lju.Location, ljuf.Location))
                    Console.WriteLine("Same City: {0}", ljuf.Name) ;

            if (SameDayAnyYear(lju.BDate, ljuf.BDate))
                Console.WriteLine("Same birthday! {0}", ljuf.Name) ;

            if (ljuf.BDate.Year != DateTime.Now.Year)
            {
                if (ljuf.BDate > MostRecentBirth)
                    MostRecentBirth = ljuf.BDate ;

                if (ljuf.BDate < BirthLongestAgo)
                    BirthLongestAgo = ljuf.BDate ;
            }
        }

        Console.WriteLine("Friends range in age: {0} {1}", BirthLongestAgo, MostRecentBirth) ;

        "Old-timer"
        "Compatable signs"
        "These Neighbors could buy you beer"
        "Youngest friend"
        "Oldest friend"
        "Whatever happened to"
        "Sure gets lots of comments" (responses / sinceCreated)
        "Same age as..."

    }
    */



    /*
    static public void BuildAndPublishImageClickMapFile() 
    {
        SqlConnection sqlc = MasterDB.GetDBConnection() ;

        string strCmd = string.Format("select ImageClickMap from LJUsers") ;
        SqlCommand cmd = new SqlCommand(strCmd, sqlc) ;
        SqlDataReader myReader = cmd.ExecuteReader() ;

        const string CLICKMAPFILE = "fixednamemapdata.htm" ;
        FileInfo finf = new FileInfo(CLICKMAPFILE);
        StreamWriter sw = finf.CreateText() ;

        sw.WriteLine("<html>") ;
        sw.WriteLine("<body>") ;
        sw.WriteLine("") ;

        while( myReader.Read ( ) )
            {
                if (false == myReader.IsDBNull(0))
                    {
                    sw.WriteLine(myReader.GetString(0).Trim()) ;
                    sw.WriteLine("") ;
                    }
            }

        sw.WriteLine("</body>") ;
        sw.WriteLine("</html>") ;

        sw.Close() ;
	
        myReader.Close() ;

        FileInfo fi = new FileInfo(CLICKMAPFILE);
        byte [] b = new byte[ fi.Length ] ;

        using (FileStream fs = File.OpenRead(CLICKMAPFILE)) 
            {
            fs.Read(b,0,b.Length) ;
            try
                {
                FileXferService ufs = new FileXferService() ;
                Console.WriteLine( ufs.Upload(GetXferPwd(), CLICKMAPFILE, b) ) ; // "mindmap/" + 
                }
            catch( Exception e )
                {
                Console.WriteLine( e.ToString() ) ;
                // if this fails, we are going to ignore the fact, except for emitting a note.
                Console.WriteLine(CLICKMAPFILE + " FAILED TO UPLOAD!") ;
                }
            }
    }
    */
    /*

static public void BuildAndPublishXmlMapFile()
{
	// We need to build a terse representation of tribe data.
	// we create a massive array of TerseLJUsers.  then each user
	// gets a slot # of those seed tribes he's in.

	// we could save more memory using bits to represent slots.
	// but we don't yet.

	ArrayList alTerseLJUserList = new ArrayList() ;


	// we're gonna need a terse array of everyone, so let's get started.
	NpgsqlConnection dbc = MasterDB.GetDBConnection () ;

	string strCmd = string.Format("select Name from LJUsers") ;
	MyNpgsqlCommand cmd = new MyNpgsqlCommand(strCmd, dbc) ;
	NpgsqlDataReader myReader = cmd.ExecuteReader() ;

	while( myReader.Read ( ) )
		{
			TerseLJUser tlju = new TerseLJUser ( myReader.GetString(0).Trim()) ;
			alTerseLJUserList.Add( tlju ) ;
		}

	myReader.Close() ;
		
	for(int iSlot = 0; iSlot < alTerseLJUserList.Count; iSlot++)
		{
		// We need to get this actual user, to see if this a tribe seed.
		TerseLJUser tljuAmISeed = (TerseLJUser) alTerseLJUserList[ iSlot ] ;
		LJUser2 lju = MasterDB.GetSlimUser( tljuAmISeed.Name ) ;

		if (lju.tribe != null)
			{
			// i appear in my own tribe.
			if (((TerseLJUser) alTerseLJUserList[iSlot]).TribesBySeedSlot == null)
				{
				((TerseLJUser) alTerseLJUserList[iSlot]).TribesBySeedSlot = new ArrayList() ;
				}
			
			((TerseLJUser) alTerseLJUserList[iSlot]).TribesBySeedSlot.Add( iSlot ) ;

	
			foreach( string strIRead in lju.whoIRead )
				{
				LJUser2 ljuThem = MasterDB.GetSlimUser( strIRead ) ;
				// apparently, sometimes ljuTHem is missing.  We aren'g going to flip if it is.
				if (null != ljuThem)
				if ( ljuThem.Reads( lju.Name ) )
					{
					foreach( TerseLJUser tlju in alTerseLJUserList )
						{
						if ( tlju.Name == strIRead )
							{
							if (tlju.TribesBySeedSlot == null)
								{
								tlju.TribesBySeedSlot = new ArrayList() ;
								}
							else
								{
								foreach( int iSeedSlot in tlju.TribesBySeedSlot )
									{
									if (iSeedSlot == iSlot)
										goto DontAddAgain ;
									}
								}
							
							tlju.TribesBySeedSlot.Add( iSlot ) ;
							DontAddAgain:
								;
							}
						}
					}
				}
			}
		}

	// if user appears in no maps, throw out user
	for (int i = alTerseLJUserList.Count - 1; i >= 0 ; i--)
		{
		TerseLJUser tlju = (TerseLJUser) alTerseLJUserList[i] ;
		if (tlju.TribesBySeedSlot == null)
			tlju.Name = "" ; // .RemoveAt( i ) ;
		}


	foreach( TerseLJUser tlju in alTerseLJUserList )
		{
		Console.WriteLine("") ;
		Console.WriteLine("User: {0}", tlju.Name) ;
		if (tlju.TribesBySeedSlot != null)
			{
			Console.Write("Appears in these user maps: ") ;
			foreach( int iSeedSlot in tlju.TribesBySeedSlot )
			{
				Console.Write("{0} ", ((TerseLJUser)alTerseLJUserList[iSeedSlot]).Name) ;
			}
			}
		}

	// Serialize the beautiful results.

	const string MAPFNAME = "seedmap.xml" ;
	try
	{
	Stream sw = File.Create( MAPFNAME ) ; // GAME_SAVE_WITH_LAUNCH) ;
	XmlSerializer x = new XmlSerializer(typeof(ArrayList),  new Type[] { typeof(TerseLJUser) }) ;
	x.Serialize(sw, alTerseLJUserList) ;
	sw.Close() ;
	}
	catch( Exception e )
	{
		Console.WriteLine( e.ToString()) ;
		return ;
	}

	FileInfo fi = new FileInfo(MAPFNAME);
	byte [] b = new byte[ fi.Length ] ;

	using (FileStream fs = File.OpenRead(MAPFNAME)) 
	{
		fs.Read(b,0,b.Length) ;
		try
			{
			
		XF2ServiceLjmindmapCom ufs = new XF2ServiceLjmindmapCom () ;
		Console.WriteLine( ufs.Upload(GetXferPwd(), MAPFNAME, b) ) ; // "mindmap/" + 
			}
		catch( Exception )
			{
			// if this fails, we are going to ignore the fact, except for emitting a note.
			Console.WriteLine(MAPFNAME + " FAILED TO UPLOAD!") ;
			}
	}

}
			
}

     * */


    public class ColorConvert
    {
        // Handle conversions between RGB and HSV    
        // (and Color types, as well).

        public struct RGB
        {
            // All values are between 0 and 255.
            public int Red;
            public int Green;
            public int Blue;

            public RGB(int R, int G, int B)
            {
                Red = R;
                Green = G;
                Blue = B;
            }

            public override string ToString()
            {
                return String.Format("({0}, {1}, {2})", Red, Green, Blue);
            }
        }

        public struct HSV
        {
            // All values are between 0 and 255.
            public int Hue;
            public int Saturation;
            public int value;

            public HSV(int H, int S, int V)
            {
                Hue = H;
                Saturation = S;
                value = V;
            }

            public override string ToString()
            {
                return String.Format("({0}, {1}, {2})", Hue, Saturation, value);
            }
        }

        public static RGB HSVtoRGB(int H, int S, int V)
        {
            // H, S, and V must all be between 0 and 255.
            return HSVtoRGB(new HSV(H, S, V));
        }

        public static Color HSVtoColor(HSV hsv)
        {
            RGB RGB = HSVtoRGB(hsv);
            return Color.FromArgb(RGB.Red, RGB.Green, RGB.Blue);
        }

        public static Color HSVtoColor(int H, int S, int V)
        {
            return HSVtoColor(new HSV(H, S, V));
        }

        public static RGB HSVtoRGB(HSV HSV)
        {
            // HSV contains values scaled as in the color wheel:
            // that is, all from 0 to 255. 

            // for ( this code to work, HSV.Hue needs
            // to be scaled from 0 to 360 (it//s the angle of the selected
            // point within the circle). HSV.Saturation and HSV.value must be 
            // scaled to be between 0 and 1.

            double h;
            double s;
            double v;

            double r = 0;
            double g = 0;
            double b = 0;

            // Scale Hue to be between 0 and 360. Saturation
            // and value scale to be between 0 and 1.
            h = ((double)HSV.Hue / 255 * 360) % 360;
            s = (double)HSV.Saturation / 255;
            v = (double)HSV.value / 255;

            if (s == 0)
            {
                // If s is 0, all colors are the same.
                // This is some flavor of gray.
                r = v;
                g = v;
                b = v;
            }
            else
            {
                double p;
                double q;
                double t;

                double fractionalSector;
                int sectorNumber;
                double sectorPos;

                // The color wheel consists of 6 sectors.
                // Figure out which sector you//re in.
                sectorPos = h / 60;
                sectorNumber = (int)(Math.Floor(sectorPos));

                // get the fractional part of the sector.
                // That is, how many degrees into the sector
                // are you?
                fractionalSector = sectorPos - sectorNumber;

                // Calculate values for the three axes
                // of the color. 
                p = v * (1 - s);
                q = v * (1 - (s * fractionalSector));
                t = v * (1 - (s * (1 - fractionalSector)));

                // Assign the fractional colors to r, g, and b
                // based on the sector the angle is in.
                switch (sectorNumber)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    case 5:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }
            // return an RGB structure, with values scaled
            // to be between 0 and 255.
            return new RGB((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        public static HSV RGBtoHSV(RGB RGB)
        {
            // In this function, R, G, and B values must be scaled 
            // to be between 0 and 1.
            // HSV.Hue will be a value between 0 and 360, and 
            // HSV.Saturation and value are between 0 and 1.
            // The code must scale these to be between 0 and 255 for
            // the purposes of this application.

            double min;
            double max;
            double delta;

            double r = (double)RGB.Red / 255;
            double g = (double)RGB.Green / 255;
            double b = (double)RGB.Blue / 255;

            double h;
            double s;
            double v;

            min = Math.Min(Math.Min(r, g), b);
            max = Math.Max(Math.Max(r, g), b);
            v = max;
            delta = max - min;
            if (max == 0 || delta == 0)
            {
                // R, G, and B must be 0, or all the same.
                // In this case, S is 0, and H is undefined.
                // Using H = 0 is as good as any...
                s = 0;
                h = 0;
            }
            else
            {
                s = delta / max;
                if (r == max)
                {
                    // Between Yellow and Magenta
                    h = (g - b) / delta;
                }
                else if (g == max)
                {
                    // Between Cyan and Yellow
                    h = 2 + (b - r) / delta;
                }
                else
                {
                    // Between Magenta and Cyan
                    h = 4 + (r - g) / delta;
                }

            }
            // Scale h to be between 0 and 360. 
            // This may require adding 360, if the value
            // is negative.
            h *= 60;
            if (h < 0)
            {
                h += 360;
            }

            // Scale to the requirements of this 
            // application. All values are between 0 and 255.
            return new HSV((int)(h / 360 * 255), (int)(s * 255), (int)(v * 255));
        }
    }
}