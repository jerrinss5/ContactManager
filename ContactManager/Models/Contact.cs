using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactManager.Models
{
    public class Contact
    {
        public string ContactID { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
    }

    public class ContactDB : List<Contact>
    {
        public ContactDB()
        {
            AddRange(ReadContacts());
        }

        public static List<Contact> ReadContacts()
        {
            /// reading json file
            string jsonString = System.IO.File.ReadAllText(@"G:\contacts.json");
            JArray jsonVal = JArray.Parse(jsonString) as JArray;
            dynamic contacts = jsonVal;
            List<Contact> contactList = new List<Contact>();
            foreach (dynamic contact_data in contacts)
            {
                contactList.Add
                (
                    new Contact { ContactID = contact_data.ContactID.ToString(), ContactName = contact_data.ContactName.ToString(), ContactNumber = contact_data.ContactNumber.ToString() }
                );
            }
            return contactList;
        }
    }
}