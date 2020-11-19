using FSD_Helpderly.Models;
using Google.Cloud.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        async public Task<List<object>> GetVolunteerForms(string email)
        {
            List<object> formIds = new List<object>();

            DocumentReference doc = db.Collection("users").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists)
            {
                Dictionary<string, object> volunteer = snap.ToDictionary();
                formIds = (List<object>)volunteer["forms"];
            }

            return formIds;
        }

        async public void AddVolunteer(string email, string Nationality, string password, string TelNo, string VolunteerName)
        {
            DocumentReference doc = db.Collection("users").Document(email);
            DocumentSnapshot snapshot = await doc.GetSnapshotAsync();
            if (snapshot.Exists)
            {

            }
            else
            {
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
        }

        async public void UpdateVolunteerPassword(string ConfirmPassword, string Email)
        {
            string email = Email;
            DocumentReference doc = db.Collection("users").Document(email);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "password", ConfirmPassword }
            };
            await doc.UpdateAsync(updates);
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
        async public void AddOrg(string email, string address, string organizationname, string password, string telno)
        {
            DocumentReference doc = db.Collection("organizationUsers").Document(email);
            DocumentSnapshot snapshot = await doc.GetSnapshotAsync();
            if (snapshot.Exists)
            {

            }
            else
            {
                //initialise empty array for forms
                ArrayList forms = new ArrayList();

                Dictionary<string, object> organization = new Dictionary<string, object>()
            {
                {"address", address},
                {"organizationName", organizationname},
                {"password", password},
                {"telno", telno},
                {"forms", forms }
            };

                await doc.SetAsync(organization);
            }
        }

        /*******************************/
        //                              /
        //           Admin              /
        //                              /
        /*******************************/

        //Returns an empty string "" if email not found
        async public Task<string> GetAdminPassword(string email)
        {
            string password = "";
            DocumentReference doc = db.Collection("admin").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists)
            {
                Dictionary<string, object> admin = snap.ToDictionary();
                password = (string)admin["password"];
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
                    CurrentQuantityVolunteer = (int)(long)docDic["currentQuantityVolunteer"],
                    Description = (string)docDic["description"],
                    Email = (string)docDic["email"],
                    EndTime = convertedEndTIme,
                    Name = (string)docDic["name"],
                    Location = (string)docDic["location"],
                    QuantityVolunteer = (int)(long)docDic["quantityVolunteer"],
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
                    CurrentQuantityVolunteer = (int)(long)docDic["currentQuantityVolunteer"],
                    Description = (string)docDic["description"],
                    Email = (string)docDic["email"],
                    EndTime = convertedEndTIme,
                    Name = (string)docDic["name"],
                    Location = (string)docDic["location"],
                    QuantityVolunteer = (int)(long)docDic["quantityVolunteer"],
                    MobileNumber = (string)docDic["mobileNumber"],
                    Region = (string)docDic["region"],
                    StartTime = convertedStartTime,
                };
                forms.Add(elderlyPost);
            }

            return forms;
        }

        async public Task<List<ElderlyPost>> GetFormsByDate(DateTime startTime, DateTime endTime)
        {
            List<ElderlyPost> forms = new List<ElderlyPost>();

            CollectionReference coll = db.Collection("forms");
            QuerySnapshot snap = await coll.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snap)
            {
                Dictionary<string, object> docDic = doc.ToDictionary();

                //Convert endTime
                Timestamp docEndTime;
                System.DateTime? convertedEndTime;
                if (docDic["endTime"] != null)
                {
                    docEndTime = (Timestamp)docDic["endTime"];
                    convertedEndTime = docEndTime.ToDateTime();
                }
                else
                {
                    convertedEndTime = null;
                }

                //Convert startTime
                Timestamp docStartTime = (Timestamp)docDic["startTime"];
                System.DateTime convertedStartTime = docStartTime.ToDateTime();

                if (convertedEndTime < endTime && convertedStartTime > startTime)
                {
                    ElderlyPost elderlyPost = new ElderlyPost()
                    {
                        FormID = doc.Id,
                        AdditionalInfo = (string)docDic["additionalInfo"],
                        CurrentQuantityVolunteer = (int)(long)docDic["currentQuantityVolunteer"],
                        Description = (string)docDic["description"],
                        Email = (string)docDic["email"],
                        EndTime = convertedEndTime,
                        Name = (string)docDic["name"],
                        Location = (string)docDic["location"],
                        QuantityVolunteer = (int)(long)docDic["quantityVolunteer"],
                        MobileNumber = (string)docDic["mobileNumber"],
                        Region = (string)docDic["region"],
                        StartTime = convertedStartTime,
                    };
                    forms.Add(elderlyPost);
                }
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
                { "currentQuantityVolunteer", 0 },
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

        async public void DeleteForm(string formId)
        {
            DocumentReference doc = db.Collection("forms").Document(formId);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                //Remove form from any existing users
                List<object> volunteers = (List<object>)snap.ToDictionary()["volunteers"];
                foreach (string email in volunteers)
                {
                    RemoveFormFromVolunteer(email, formId);
                }
                await snap.Reference.DeleteAsync();
            }
        }

        async private void AddVolunteerToForm(string email, string formId)
        {
            DocumentReference doc = db.Collection("forms").Document(formId);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                await doc.UpdateAsync("volunteers", FieldValue.ArrayUnion(email));

                snap = await doc.GetSnapshotAsync();
                List<object> volunteers = (List<object>)snap.ToDictionary()["volunteers"];

                await doc.UpdateAsync("currentQuantityVolunteer", volunteers.Count());
            }
        }

        async private void RemoveVolunteerFromForm(string email, string formId)
        {
            DocumentReference doc = db.Collection("forms").Document(formId);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                await doc.UpdateAsync("volunteers", FieldValue.ArrayRemove(email));

                snap = await doc.GetSnapshotAsync();
                List<object> volunteers = (List<object>)snap.ToDictionary()["volunteers"];

                await doc.UpdateAsync("currentQuantityVolunteer", volunteers.Count());
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
