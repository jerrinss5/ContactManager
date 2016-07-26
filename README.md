# ContactManager
A rest API to authenticate a user and let it access contact details from a json file

Details:-
A Restful API using .NET and Visual Studio for users to login and receive a secure authentication token valid for a day.
After successfully being authenticated, users can get a list of contact details.

User and contact data is stored in a json file which is read by the application for its respective calls.

Make sure to update the path of json file in providers/ApplicationOAuthProvider.cs and controller/AccountController.cs for the contacts.json and users.json file.

Users json contains the username and the password(hashed-Bcrypted).
Contacts json contains the contact information.

The main login page URL is http://<domainname>:<port>/login/SecurityInfo
