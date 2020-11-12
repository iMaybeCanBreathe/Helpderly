using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.VisualBasic;

namespace FSD_Helpderly.DAL
{ 
    public class FirestoreDAL
    {
        private FirestoreDb db;

        public FirestoreDAL() 
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"helpderly-firebase.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            db = FirestoreDb.Create("helpderly");
        }

        /*******************************/
        //                              /
        //          Volunteers          /
        //                              /
        /*******************************/

        //Parameters: email
        //Returns: A dictionary with the keys: [firstName, lastName, password]
        async public Task<Dictionary<string, object>> GetVolunteer(string email)
        {
            Dictionary<string, object> volunteer = new Dictionary<string, object>();

            DocumentReference doc = db.Collection("users").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists)
            {
                volunteer = snap.ToDictionary();
            }
            return volunteer;
        }

        async public void AddVolunteer(string email, string firstName, string lastName, string password)
        {
            DocumentReference doc = db.Collection("users").Document(email);

            Dictionary<string, object> volunteer = new Dictionary<string, object>()
            {
                {"firstName", firstName},
                {"lastName", lastName},
                {"password", password},
            };

            await doc.SetAsync(volunteer);
        }

        /*******************************/
        //                              /
        //            Forms             /
        //                              /
        /*******************************/

        //Returns: A Dictionary where Key is the document ID, and value is another dictionary which 
        //         has the keys [additionalInfo, description, email, endTime, firstName, lastname, location, mobileNumber, startTime]
        async public Task<Dictionary<string, Dictionary<string, object>>> GetAllForms()
        {
            Dictionary<string, Dictionary<string, object>> forms = new Dictionary<string, Dictionary<string, object>>();

            CollectionReference coll = db.Collection("forms");
            QuerySnapshot snap = await coll.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snap)
            {
                forms.Add(doc.Id, doc.ToDictionary());
            }

            return forms;
        }

        async public void AddForm(string additionalInfo, string description, string email, 
            DateTime? endTime, string firstName, string lastname, string location, string mobileNumber, DateTime startTime)
        {
            CollectionReference coll = db.Collection("forms");

            Dictionary<string, object> form = new Dictionary<string, object>()
            {
                { "additionalInfo", additionalInfo },
                { "description", description },
                { "email", email },
                { "endTime", endTime == null? null : (Timestamp?)Timestamp.FromDateTime(System.DateTime.SpecifyKind(Convert.ToDateTime(endTime), DateTimeKind.Utc)) },
                { "firstName", firstName },
                { "lastname", lastname },
                { "location", location },
                { "mobileNumber", mobileNumber },
                { "startTime", Timestamp.FromDateTime(System.DateTime.SpecifyKind(startTime, DateTimeKind.Utc)) },
            };

            await coll.AddAsync(form);
        }
    }
}
