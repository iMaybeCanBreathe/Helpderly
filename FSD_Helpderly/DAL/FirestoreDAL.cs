using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FSD_Helpderly.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Type;
using Microsoft.VisualBasic;
using DateTime = System.DateTime;

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

        //Returns an empty string "" if email not found
        async public Task<string> GetVolunteerPassword(string email)
        {
            string password = "";

            DocumentReference doc = db.Collection("users").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists)
            {
                Dictionary<string, object> volunteer = snap.ToDictionary();
                password = (string) volunteer["password"];
            }

            return password;
        }
        async public Task<string> GetVolunteerEmail(string email)
        {
            string emailFound = "";
            DocumentReference doc = db.Collection("users").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists)
            {
                Dictionary<string, object> volunteer = snap.ToDictionary();
                emailFound = (string)volunteer["email"];
            }
            return emailFound;
        }

        async public void AddVolunteer(string email, string Nationality, string password, string TelNo, string VolunteerName)
        {
            DocumentReference doc = db.Collection("users").Document(email);
            //initialise empty array for forms
            ArrayList forms = new ArrayList();

            Dictionary<string, object> volunteer = new Dictionary<string, object>()
            {
                {"volunteerName", VolunteerName},
                {"nationality", Nationality},
                {"telNo", TelNo},
                {"password", password},
                {"forms", forms }
            };

            await doc.SetAsync(volunteer);
        }

        async private void AddFormToVolunteer(string email, string formId)
        {
            DocumentReference doc = db.Collection("users").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                await doc.UpdateAsync("forms", FieldValue.ArrayUnion(formId));
            }
        }

        async private void RemoveFormFromVolunteer(string email, string formId)
        {
            DocumentReference doc = db.Collection("users").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                await doc.UpdateAsync("forms", FieldValue.ArrayRemove(formId));
            }
        }

        /*******************************/
        //                              /
        //        Organisation          /
        //                              /
        /*******************************/

        //Returns an empty string "" if email not found
        async public Task<string> GetOrgPassword(string email)
        {
            string password = "";

            DocumentReference doc = db.Collection("organizationUsers").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists)
            {
                Dictionary<string, object> org = snap.ToDictionary();
                password = (string)org["password"];
            }

            return password;
        }

        /*******************************/
        //                              /
        //            Forms             /
        //                              /
        /*******************************/

        //Returns: An ElderlyPost object which 
        //         has the properties [additionalInfo, description, email, endTime, firstName, lastname, location, mobileNumber, startTime]
        async public Task<ElderlyPost> GetForm(string id)
        {
            DocumentReference doc = db.Collection("forms").Document(id);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            ElderlyPost elderlyPost = new ElderlyPost();

            if (snap.Exists)
            {
                Dictionary<string, object> docDic = snap.ToDictionary();

                //Convert endTime
                Timestamp endTime;
                System.DateTime? convertedEndTIme;
                if (docDic["endTime"] != null)
                {
                    endTime = (Timestamp)docDic["endTime"];
                    convertedEndTIme = endTime.ToDateTime();
                }
                else
                {
                    convertedEndTIme = null;
                }

                //Convert startTime
                Timestamp startTime = (Timestamp)docDic["startTime"];
                System.DateTime convertedStartTime = startTime.ToDateTime();

                elderlyPost = new ElderlyPost()
                {
                    FormID = doc.Id,
                    AdditionalInfo = (string)docDic["additionalInfo"],
                    Description = (string)docDic["description"],
                    Email = (string)docDic["email"],
                    EndTime = convertedEndTIme,
                    Name = (string)docDic["name"],
                    Location = (string)docDic["location"],
                    MobileNumber = (string)docDic["mobileNumber"],
                    Region = (string)docDic["region"],
                    StartTime = convertedStartTime,
                };
            }

            return elderlyPost;
        }

        //Returns: A list of ElderlyPosts object which 
        //         has the properties [additionalInfo, description, email, endTime, firstName, lastname, location, mobileNumber, startTime]
        async public Task<List<ElderlyPost>> GetAllForms()
        {
            List<ElderlyPost> forms = new List<ElderlyPost>();

            CollectionReference coll = db.Collection("forms");
            QuerySnapshot snap = await coll.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snap)
            {
                Dictionary<string, object> docDic = doc.ToDictionary();

                //Convert endTime
                Timestamp endTime;
                System.DateTime? convertedEndTIme;
                if (docDic["endTime"] != null)
                {
                    endTime = (Timestamp)docDic["endTime"];
                    convertedEndTIme = endTime.ToDateTime();
                }
                else
                {
                    convertedEndTIme = null;
                }

                //Convert startTime
                Timestamp startTime = (Timestamp)docDic["startTime"];
                System.DateTime convertedStartTime = startTime.ToDateTime();

                ElderlyPost elderlyPost = new ElderlyPost()
                {
                    FormID = doc.Id,
                    AdditionalInfo = (string)docDic["additionalInfo"],
                    Description = (string)docDic["description"],
                    Email = (string)docDic["email"],
                    EndTime = convertedEndTIme,
                    Name = (string)docDic["name"],
                    Location = (string)docDic["location"],
                    MobileNumber = (string)docDic["mobileNumber"],
                    Region = (string)docDic["region"],
                    StartTime = convertedStartTime,
                };
                forms.Add(elderlyPost);
            }

            return forms;
        }

        async public void AddForm(ElderlyPost ePost)
        {
            CollectionReference coll = db.Collection("forms");
            //initlialise empty array for volunteers
            ArrayList volunteers = new ArrayList();

            Dictionary<string, object> form = new Dictionary<string, object>()
            {
                { "additionalInfo", ePost.AdditionalInfo },
                { "description", ePost.Description },
                { "email", ePost.Email },
                { "endTime", ePost.EndTime == null? null : (Timestamp?)Timestamp.FromDateTime(System.DateTime.SpecifyKind(Convert.ToDateTime(ePost.EndTime), DateTimeKind.Utc)) },
                { "name", ePost.Name },
                { "location", ePost.Location },
                { "mobileNumber", ePost.MobileNumber },
                { "quantityVolunteer", ePost.QuantityVolunteer },
                { "region", ePost.Region },
                { "startTime", Timestamp.FromDateTime(System.DateTime.SpecifyKind(ePost.StartTime, DateTimeKind.Utc)) },
                { "volunteers", volunteers},
            };

            await coll.AddAsync(form);
        }
        async private void AddVolunteerToForm(string email, string formId)
        {
            DocumentReference doc = db.Collection("forms").Document(formId);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                await doc.UpdateAsync("volunteers", FieldValue.ArrayUnion(email));
                await doc.UpdateAsync("quantityVolunteer", FieldValue.Increment(1));
            }
        }

        async private void RemoveVolunteerFromForm(string email, string formId)
        {
            DocumentReference doc = db.Collection("forms").Document(formId);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                await doc.UpdateAsync("volunteers", FieldValue.ArrayRemove(email));
                await doc.UpdateAsync("quantityVolunteer", FieldValue.Increment(-1));
            }
        }

        /*******************************/
        //                              /
        //           Others             /
        //                              /
        /*******************************/

        public void VolunteerAcceptForm(string email, string formId)
        {
            AddFormToVolunteer(email, formId);
            AddVolunteerToForm(email, formId);
        }

        public void VolunteerCancelForm(string email, string formId)
        {
            RemoveFormFromVolunteer(email, formId);
            RemoveVolunteerFromForm(email, formId);
        }
    }
}
