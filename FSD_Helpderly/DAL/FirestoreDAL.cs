﻿using System;
using System.Collections.Generic;
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
                password = (string) volunteer["Password"];
            }

            return password;
        }

        async public void AddVolunteer(string email, string Nationality, string password, string TelNo, string VolunteerName)
        {
            DocumentReference doc = db.Collection("users").Document(email);

            Dictionary<string, object> volunteer = new Dictionary<string, object>()
            {
                {"volunteerName", VolunteerName},
                {"nationality", Nationality},
                {"telNo", TelNo},
                {"password", password}
            };

            await doc.SetAsync(volunteer);
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
        async public Task<List<ElderlyPost>> GetAllForms()
        {
            List<ElderlyPost> forms = new List<ElderlyPost>();

            CollectionReference coll = db.Collection("forms");
            QuerySnapshot snap = await coll.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snap)
            {
                //Convert endTime
                Timestamp endTime;
                System.DateTime? convertedEndTIme;
                if (doc.ToDictionary()["endTime"] != null)
                {
                    endTime = (Timestamp)doc.ToDictionary()["endTime"];
                    convertedEndTIme = endTime.ToDateTime();
                }
                else
                {
                    convertedEndTIme = null;
                }

                //Convert startTime
                Timestamp startTime = (Timestamp)doc.ToDictionary()["startTime"];
                System.DateTime convertedStartTime = startTime.ToDateTime();

                ElderlyPost elderlyPost = new ElderlyPost()
                {
                    FormId = doc.Id,
                    AdditionalInfo = (string)doc.ToDictionary()["additionalInfo"],
                    Description = (string)doc.ToDictionary()["description"],
                    Email = (string)doc.ToDictionary()["email"],
                    EndTime = convertedEndTIme,
                    Name = (string)doc.ToDictionary()["name"],
                    Location = (string)doc.ToDictionary()["location"],
                    MobileNumber = (string)doc.ToDictionary()["mobileNumber"],
                    StartTime = convertedStartTime,
                };
                forms.Add(elderlyPost);
            }

            return forms;
        }

        async public void AddForm(ElderlyPost ePost)
        {
            CollectionReference coll = db.Collection("forms");

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
                { "startTime", Timestamp.FromDateTime(System.DateTime.SpecifyKind(ePost.StartTime, DateTimeKind.Utc)) },
            };

            await coll.AddAsync(form);
        }
    }
}
