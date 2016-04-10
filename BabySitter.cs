using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;


namespace tribe
{
    class BabySitter
    {
        public static void KickUsAlong()
        {
            MMDB.MakeSureDBIsOpen();

            HashSet<Int32> optedIn = FData.IDsInTheyReadMeFData(FData.GetFData("ljfinder", true));
            optedIn.UnionWith(FData.IDsInTheyReadMeFData(FData.GetFData("ljmindmap", true)));

            DateTime TwoK = new DateTime(2000, 1, 1);
            int weeks = (int)(DateTime.Now.Subtract(TwoK).Days / 7.02);
            int months = (int)(DateTime.Now.Subtract(TwoK).Days / 30.4);

            string sqlset = "";
            foreach (var dude in optedIn)
                sqlset += dude.ToString() + ",";
            sqlset = sqlset.Substring(0, sqlset.Length - 1);

            // any opt-ins who haven't been reached this week or last, for whom lastpublishedmonth is not this month, whose status is 'L' or 'N' or 'S'?
            // set them to status 'S' and set statuschanged=null. If any rows are effected, then I don't email anybody this time!
                // HEY fuck's sake i'm changing the value of week because emailing someone i emailed last week is too fucking much.
                weeks = weeks - 1;

            // if mm lastpublishmonth isn't this month, is it status 'L' or 'N' ?
            //   if so, then set it to status 'S' and set statuschanged=null,
            //     and postpone any talk of notifications.

            NpgsqlCommand cmd = new NpgsqlCommand(string.Format(
//            "select count(*) from nameidmap where id in ({0}) and inbox_hit_week < {1} and lastpublishedmonth < {2} and status in ('L', 'N') ;",
              "select count(*) from nameidmap where id in ({0}) and inbox_hit_week < {1} and lastpublishedmonth < {2} and status in ('L', 'N', 'S') ;",
            sqlset, weeks, months), MMDB.DBConnection);

            NpgsqlDataReader myReader = cmd.ExecuteReader();
            myReader.Read();
            Int64? iResult = MMDB.MaybeNullInt64(myReader, 0);
            myReader.Close();

            if (0 == iResult)
            {
                // go ahead and notify!
                // lots of optedIn are excluded because i've reached 'em too recently.
                HashSet<Int32> eligible = new HashSet<Int32>();
                cmd = new NpgsqlCommand(string.Format(
                "select id from nameidmap where id in ({0}) and inbox_hit_week < {1} ;",
                sqlset, weeks, months), MMDB.DBConnection);
                myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    eligible.Add(myReader.GetInt32(0));
                }
                myReader.Close();

           //     eligible.Add(IDMap.NameToID("eclipsenacht"));

                // fuck eligible we're testing mcfnord
//                eligible.Clear();
//                eligible.Add(IDMap.NameToID("mcfnord"));

                Radar2010.RefreshBasedOnRadarSignals_AndPublish(eligible);
                return;
            }

            Console.WriteLine("Waiting in {0} dudes.", iResult);

            MMDB.ExecuteNonQuery(string.Format(
                "update nameidmap set status='S', statuschanged=null where id in ({0}) and inbox_hit_week < {1} and lastpublishedmonth < {2} and status in ('L', 'N', 'S') ;",
            sqlset, weeks, months));

            // kick along.
        }
    }
}


/*

            while (myReader.Read())
                eligible.Add(myReader.GetInt32(0));
            myReader.Close();

            // for these boner eligible-for-contact jerks, is the mindmap fresh? 
//            HashSet<Int32> current = new HashSet<Int32) () ;
            bool fready = true;
            foreach (var dude in eligible)
            {
                // if i never published this dude in this month, 
//                if( false == MMDB.QueryFindsRow(string.Format("select * from nameidmap where id={0} and lastpublishedmonth = {1} LIMIT 1 ;", dude, months )))
                if (true == MMDB.QueryFindsRow(string.Format("select * from nameidmap where id={0} and status in ('L', 'N') and lastpublishedmonth < {1} LIMIT 1 ;", dude, months)))
                {
                    // then i need to publish that dude in this month
                    Console.WriteLine(IDMap.IDToName(dude) + " not ready.");
                    fready = false; // we aren't fready to launch our nut.

                    // and push it through. which means... making statusdate null. and what statuses get pushed back to scraped?
                    MMDB.ExecuteNonQuery(string.Format("update nameidmap set statuschanged = null where id={0} ;", dude)) ; // null-hammered, absolutely!

                    // and if it's NULL (queued) or already calculated, we make it scraped with its fancy null statuschanged refreshed null null null
                    if (MMDB.QueryFindsRow(string.Format("select * from nameidmap where (status='N' or status='L') and lastpublishedmonth < {1} and id={0} LIMIT 1 ;", dude, months)))
                        MMDB.ExecuteNonQuery(string.Format("update nameidmap set status='S' where id={0};", dude));
                }
            }
            if (false == fready)
                return; // no notifications until we're all ready.

            // if i haven't reached 'em too recently, are tehy ready? if they aren't ready, print them, and kick them with a null.

            // if i have contacted any party this week or last, i remove them from consideration.
            // WAIT A SECOND AREN'T A LOT OF OPTED IN PEOPLE REMOVED FROM CONSIDERATION, THEY MUST BE ALL CLEAR.
            // babysitter only calls me once mm is fresh enough and it's cool to notify every ...
            // ugh... crashing.

            // i only wanna notify when status is 'L'
            Debug.Assert(false);// crazy how this critical part seems outta control.
            // let's re-design it, writing the sql in advance.
            // test the inbox_hit_week is too low.
            // if mm lastpublishmonth isn't this month, is it status 'L' or 'N' ?
            //   if so, then set it to status 'S' and set statuschanged=null,
            //     and postpone any talk of notifications.

            foreach (var dude in eligible)
            {
                // maybe i need to carve eligible into a new array.
//                if (true == MMDB.QueryFindsRow(string.Format("select * from nameidmap where id={0} and status='L' LIMIT 1 ;", dude)))
//                {
//                    System.Diagnostics.Debug.Assert(eligible.Count > 0); // if this is zero, maybe i should publish to fresh-mindmap (this month) people who i haven't hit ever in the inbox, by high apy first. ;)
                    Radar2010.RefreshBasedOnRadarSignals_AndPublish(eligible);
//                }
            }

            

            // note week-of-notification in radar2010 so i can exclude it (and the week before it) here.
            
        }
    }
}
            */