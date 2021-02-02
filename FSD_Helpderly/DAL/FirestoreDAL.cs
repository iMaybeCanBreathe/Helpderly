using FSD_Helpderly.Models;
using Google.Cloud.Firestore;
using SendGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DateTime = System.DateTime;
using SendGrid.Helpers.Mail;

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
        //Returns: A dictionary with the keys: [volunteerName, nationality, password, telNo, forms]
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
        //           Elderly            /
        //                              /
        /*******************************/

        //if elderly email exists, send otp to elderly and update otp in firestore
        //if doesn't exist, create elderly in firestore and send otp
        async public void GenerateElderlyOTP(string email)
        {
            Random generator = new Random();
            string otp = generator.Next(100000, 1000000).ToString();

            DocumentReference doc = db.Collection("elderly").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists) //update current document
            {
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "OTP", otp }
                };

                await doc.UpdateAsync(updates);
            }
            else //create new document
            {
                //initialise empty array for forms
                ArrayList forms = new ArrayList();

                Dictionary<string, object> elderly = new Dictionary<string, object>()
                {
                    {"OTP", otp},
                    {"forms", forms },
                    {"location", "" },
                    {"mobileNumber", null },
                    {"name", "" },
                    {"region", "" },
                };

                await doc.SetAsync(elderly);
            }
            
            SendEmailOTP(email, otp);
        }

        async private void SendEmailOTP(string email, string otp)
        {
            //TODO: Call sendgrid api to send the otp
            var sgApiKey = "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"; //Yes, this is super insecure. But I have no other choice. 
            var sendGridClient = new SendGridClient(sgApiKey);
            var from = new EmailAddress("helpderly@gmail.com", "Helpderly");
            var subject = "Your Helpderly OTP";
            var to = new EmailAddress(email);
            var plainTextContent = "Your Helpderly One-Time Password is " + otp;
            var htmlContent = "Your Helpderly Log-in OTP is <strong>" + otp + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await sendGridClient.SendEmailAsync(msg);
        }

        //Returns an empty string "" if email not found
        async public Task<string> GetElderlyOTP(string email)
        {
            string password = "";

            DocumentReference doc = db.Collection("elderly").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists)
            {
                Dictionary<string, object> elderly = snap.ToDictionary();
                password = (string)elderly["OTP"];
            }

            return password;
        }

        async public Task<List<object>> GetElderlyForms(string email)
        {
            List<object> formIds = new List<object>();

            DocumentReference doc = db.Collection("elderly").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();
            if (snap.Exists)
            {
                Dictionary<string, object> elderly = snap.ToDictionary();
                formIds = (List<object>)elderly["forms"];
            }

            return formIds;
        }

        async public void AddFormToElderly(string email, string formId)
        {
            DocumentReference doc = db.Collection("elderly").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                await doc.UpdateAsync("forms", FieldValue.ArrayUnion(formId));
            }
        }

        async public void RemoveFormFromElderly(string email, string formId)
        {
            DocumentReference doc = db.Collection("elderly").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                await doc.UpdateAsync("forms", FieldValue.ArrayRemove(formId));
            }
        }

        async public void UpdateElderlyDetails(ElderlyPost elderlyPost)
        {
            DocumentReference doc = db.Collection("elderly").Document(elderlyPost.Email);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                {"location", elderlyPost.Location },
                {"mobileNumber", elderlyPost.MobileNumber },
                {"name", elderlyPost.Name },
                {"region", elderlyPost.Region },
            };
            await doc.UpdateAsync(updates);
        }

        async public Task<ElderlyPost> GetElderlyDetails(string email)
        {
            DocumentReference doc = db.Collection("elderly").Document(email);
            DocumentSnapshot snap = await doc.GetSnapshotAsync();

            ElderlyPost elderlyPost = new ElderlyPost();

            if (snap.Exists)
            {
                Dictionary<string, object> docDic = snap.ToDictionary();

                elderlyPost = new ElderlyPost()
                {
                    Email = email,
                    Name = (string)docDic["name"],
                    Location = (string)docDic["location"],
                    MobileNumber = (string)docDic["mobileNumber"],
                    Region = (string)docDic["region"],
                };
            }

            return elderlyPost;
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
                    Status = (string)docDic["status"],
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
                    Status = (string)docDic["status"],
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

                if (DateTime.Compare((DateTime)convertedEndTime, endTime) <= 0 && DateTime.Compare(convertedStartTime, startTime) >= 0)
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
                        Status = (string)docDic["status"],
                        StartTime = convertedStartTime,
                    };
                    forms.Add(elderlyPost);
                }
            }

            return forms;
        }

        async public Task<string> AddForm(ElderlyPost ePost)
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
                { "status", "ongoing"},
                { "volunteers", volunteers},
            };

            DocumentReference newForm = await coll.AddAsync(form);
            return newForm.Id;
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
        
        //update form status. pass in "true" for done, "false" for ongoing
        async public void UpdateFormStatus(string formId, bool done)
        {
            DocumentReference doc = db.Collection("forms").Document(formId);
            string status = done ? "done" : "ongoing";
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "status", status }
            };
            await doc.UpdateAsync(updates);
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
            SendEmailNotification(formId, email);
        }

        public void VolunteerCancelForm(string email, string formId)
        {
            RemoveFormFromVolunteer(email, formId);
            RemoveVolunteerFromForm(email, formId);
        }

        async private void SendEmailNotification(string formId, string helperEmail)
        {
            //get helper details
            Dictionary<string, object> volunteer = await GetVolunteer(helperEmail);
            string volName = (string)volunteer["volunteerName"];
            string volTelNo = (string)volunteer["telNo"];
            string volPicLink = (string)volunteer["picture"];

            //get receipient email from form id
            ElderlyPost form = await GetForm(formId);
            string receipient = form.Email;
            string desc = form.Description;

            //send email notification
            var sgApiKey = "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"; //Yes, this is super insecure. But I have no other choice. 
            var sendGridClient = new SendGridClient(sgApiKey);
            var from = new EmailAddress("helpderly@gmail.com", "Helpderly");
            var subject = "*Help*derly is on the way!";
            var to = new EmailAddress(receipient);
            var plainTextContent = "Your Helpderly request for \"" + desc + "\" has been accepted by " + volName + ". ";
            var htmlContent = "Your Helpderly request for <strong>\"" + desc + "\"</strong> has been accepted by <strong>" + volName + "</strong><br/><br/>" +
                "His/her details are as follows: <br/><br/>" +
                "<img src=\"" + volPicLink + "\" alt=\"Volunteer Picture\" style=\"width: 200px\"><br/><br/>" +
                "Name: <strong>" + volName + "</strong><br/>" +
                "Mobile Number: <strong>" + volTelNo + "</strong><br/>" +
                "Email: <strong>" + helperEmail + "</strong><br/><br/>" +
                "He/she will contact you one day before the request to confirm the details with you. ";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await sendGridClient.SendEmailAsync(msg);
        }
    }
}
