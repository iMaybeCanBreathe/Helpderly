using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace FSD_Helpderly.DAL
{ 
    public class VolunteerDAL
    {
        private FirestoreDb db;

        public VolunteerDAL() 
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"helpderly-firebase.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            db = FirestoreDb.Create("helpderly");
        }

        async public Task<Dictionary<string, object>> GetVolunteer()
        {
            Dictionary<string, object> volunteer = new Dictionary<string, object>();

            DocumentReference doc = db.Collection("users").Document("s10195143@connect.np.edu.sg");
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists)
            {
                volunteer = snap.ToDictionary();
            }
            return volunteer;
        }
    }
}
