// ***********************************************************************
// Assembly         : WhitePagesPhoneLookup
// Author           : Kushal Shah
// Created          : 08-06-2014
//
// Last Modified By : Kushal Shah
// Last Modified On : 11-18-2014
// ***********************************************************************
// <copyright file="Phonelookup.aspx.cs" company="Whitepages Pro">
//     . All rights reserved.
// </copyright>
// <summary>PhoneLookup class is code behind of Phonelookup page.</summary>
// ***********************************************************************

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;
using WebService;
using System.Globalization;

namespace WhitePagesPhoneLookup
{
    public partial class PhoneLookup : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //LiteralMessage.Text = string.Empty;
                correct.Visible = false;
                incorrect.Visible = false;

                FindPhoneResults();
            }

        }

        /// <summary>
        /// Event occurs when the Button control is clicked.
        /// Here we will call phone lookup API, parse result and populate result on UI
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">event data.</param>
        protected void FindPhoneResults()
        {
            try
            {
                //errorBox.Visible = false;
                //LiteralMessage.Text = string.Empty;

                string phoneQuery = Request.QueryString["phone"];

                if (!string.IsNullOrEmpty(phoneQuery))
                {
                    int statusCode = -1;
                    string description = string.Empty;
                    string errorMessage = string.Empty;

                    NameValueCollection nameValues = new NameValueCollection();

                    nameValues["phone"] = phoneQuery;
                    nameValues["api_key"] = WhitePagesConstants.ApiKey;

                    WhitePagesWebService webService = new WhitePagesWebService();

                    // Call method ExecuteWebRequest to execute backend API and return response stream.
                    Stream responseStream = webService.ExecuteWebRequest(nameValues, ref statusCode, ref description, ref errorMessage);

                    // Checking respnseStream null and status code.
                    if (statusCode == 200 && responseStream != null)
                    {
                        // Reading response stream to StreamReader.
                        StreamReader reader = new StreamReader(responseStream);

                        // Convert stream reader to string JSON.
                        string responseInJson = reader.ReadToEnd();

                        // Dispose response stream
                        responseStream.Dispose();

                        // Calling ParsePhoneLookupResult to parse the response JSON in data Result class.
                        Result resultData = ParsePhoneLookupResult(responseInJson);

                        if (resultData != null)
                        {
                            PopulateRaffleTickets(resultData);
                            // Calling function to populate data on UI.
                            PopulateDataOnUI(resultData);
                        }
                    }
                    else
                    {
                      
                    }
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        protected void ButtonAccurateClick(object sender, EventArgs e)
        {
            AddForAccuracyData(true);
            Response.Redirect("Index.aspx");
        }


        protected void ButtonInAccurateClick(object sender, EventArgs e)
        {
            AddForAccuracyData(false);
            Response.Redirect("Index.aspx");
        }

        private void PopulateRaffleTickets(Result resultData)
        {
            string raffleTicketNumbers = GetRaffleTicketNumbers(resultData.DataCounters);
            
            //Also add the newly generate raffles intot he file
            AddRaffleTicketNumbers(resultData.Phone.PhoneNumber, raffleTicketNumbers.Split(':')[1]);

            LiteralResultCounter.Text = resultData.DataCounters + " POINTS.";
            LiteralRaffleTickets.Text = raffleTicketNumbers;
        }

        private void AddForAccuracyData(bool accurate)
        {
            if (accurate)
            {
                File.AppendAllText("C:\\WhitePages-PhoneLookup\\Phone-Data-Accurate.csv", Request.QueryString["phone"] + "," + this.LiteralPeopleDetails.Text + "," + this.LiteralLocationDetails.Text + Environment.NewLine);
            }
            else
            {
                File.AppendAllText("C:\\WhitePages-PhoneLookup\\Phone-Data-Incomplete-InAccurate.csv", Request.QueryString["phone"] + "," + this.LiteralPeopleDetails.Text + "," + this.LiteralLocationDetails.Text + Environment.NewLine);
            }


        }
            

        /// <summary>
        /// This method parse the Phone Lookup data to class Result.
        /// </summary>
        /// <param name="responseInJson">responseInJson</param>
        /// <returns>Result</returns>
        private Result ParsePhoneLookupResult(string responseInJson)
        {
            // Creating PhoneLookupData object to fill the phone lookup data.
            Result resultData = new Result();

            if (Convert.ToBoolean(Request.QueryString["IsCallerId"]) == true)
            {
                resultData.DataCounters++;
            }

            try
            {
                // responseInJson to DeserializeObject
                dynamic jsonObject = JsonConvert.DeserializeObject(responseInJson);

                if (jsonObject != null)
                {
                    // Take the dictionary object from jsonObject.
                    dynamic dictionaryObj = jsonObject.dictionary;

                    if (dictionaryObj != null)
                    {
                        string phoneKey = string.Empty;

                        
                        // Take the phone key from result node of jsonObject
                        foreach (var data in jsonObject.results)
                        {
                            phoneKey = data.Value;
                            break;
                        }

                        #region Phone Data

                        // Checking phone key null or empty.
                        if (!string.IsNullOrEmpty(phoneKey))
                        {
                            // Get phone key object from dictionaryObj using phoneKey.
                            dynamic phoneKeyObject = dictionaryObj[phoneKey];

                            if (phoneKeyObject != null)
                            {
                                // Creating phoneData object to fill the phone lookup data.
                                Phone phoneData = new Phone();

                                // Extracting lineType,phoneNumber, countryCallingCode, carrier, doNotCall status, spamScore from phoneKeyObject.

                                if (phoneKeyObject["line_type"] != null)
                                {
                                    phoneData.PhoneType = (string)phoneKeyObject["line_type"];
                                    
                                }

                                if (phoneKeyObject["phone_number"] != null)
                                {
                                    phoneData.PhoneNumber = (string)phoneKeyObject["phone_number"];
                                    
                                    //Increment counter here Once
                                    resultData.DataCounters++;
                                }

                                if (phoneKeyObject["country_calling_code"] != null)
                                {
                                    phoneData.CountryCallingCode = (string)phoneKeyObject["country_calling_code"];
                                    
                                }

                                if (phoneKeyObject["carrier"] != null)
                                {
                                    phoneData.Carrier = (string)phoneKeyObject["carrier"];
                                    
                                }
                                if (phoneKeyObject["do_not_call"] != null)
                                {
                                    phoneData.DndStatus = (bool)(phoneKeyObject["do_not_call"]);
                                    
                                }
                                

                                dynamic spamScoreObj = phoneKeyObject.reputation;
                                if (spamScoreObj != null)
                                {
                                    phoneData.SpamScore = (string)spamScoreObj["spam_score"];
                                    
                                }

                                if (phoneKeyObject["is_prepaid"] != null)
                                {
                                    phoneData.IsPrepaid = (bool)(phoneKeyObject["is_prepaid"]);
                                    
                                }

                                resultData.Phone = phoneData;

                        #endregion

                                // Starting to extarct the person information.
                                dynamic phoneKeyObjectBelongsToObj = phoneKeyObject.belongs_to;

                                List<string> personKeyListFromBelongsTo = new List<string>();
                                List<string> locationKeyList = new List<string>();


                                // Lets get the basic location from associated location of phone.
                                string phoneAssociatedLocation = "";
                                // Extracting location key from Phone details under best_location object.
                                dynamic bestLocationFromPhoneObj = phoneKeyObject.best_location;
                                if (bestLocationFromPhoneObj != null)
                                {
                                    dynamic bestLocationIdFromPhoneObj = bestLocationFromPhoneObj.id;
                                    if (bestLocationIdFromPhoneObj != null)
                                    {
                                        phoneAssociatedLocation = ((string)bestLocationIdFromPhoneObj["key"]);
                                    }
                                }

                                #region Person and Business
                                if (phoneKeyObjectBelongsToObj != null)
                                {
                                    
                                    // Creating list of person key from phoneKeyObjectBelongsToObj.
                                    foreach (var data in phoneKeyObjectBelongsToObj)
                                    {
                                        dynamic belongsToObj = data.id;
                                        if (belongsToObj != null)
                                        {
                                            string personKeyFromBelongsTo = (string)belongsToObj["key"];

                                            if (!string.IsNullOrEmpty(personKeyFromBelongsTo))
                                            {
                                                personKeyListFromBelongsTo.Add(personKeyFromBelongsTo);
                                            }
                                            
                                        }
                                    }
                                }

                                List<People> peopleList = new List<People>();

                                if (personKeyListFromBelongsTo.Count > 0)
                                {
                                    //Increment counter twice
                                    resultData.DataCounters++;

                                    People people = null;

                                    dynamic personKeyObject = null;

                                    foreach (string personKey in personKeyListFromBelongsTo)
                                    {
                                        people = new People();

                                        personKeyObject = dictionaryObj[personKey];

                                        if (personKeyObject != null)
                                        {
                                            // Get phoneKeyIdObj from personKeyObject.
                                            dynamic phoneKeyIdObj = personKeyObject.id;

                                            if (phoneKeyIdObj != null)
                                            {
                                                // Get person type from phoneKeyIdObj.
                                                people.PersonType = phoneKeyIdObj["type"];
                                            }

                                            // phoneKeyNamesObj from name node of personKeyObject.
                                            dynamic phoneKeyNamesObj = personKeyObject.names;

                                            if (phoneKeyNamesObj != null)
                                            {
                                                string firstName = string.Empty;
                                                string lastName = string.Empty;
                                                string middleName = string.Empty;

                                                foreach (var name in phoneKeyNamesObj)
                                                {
                                                    firstName = (string)name["first_name"];
                                                    middleName = (string)name["middle_name"];
                                                    lastName = (string)name["last_name"];
                                                }

                                                people.PersonName = firstName + " " + lastName;
                                            }
                                            else
                                            {
                                                people.PersonName = personKeyObject.name;
                                            }

                                            if (personKeyObject.gender != null) 
                                            {     
                                                people.Gender = personKeyObject.gender;
                                            }
                                            else
                                            {
                                                people.Gender = "Unknown";
                                            }

                                            dynamic phoneKeyAgeObj = personKeyObject.age_range;
                                            if (phoneKeyAgeObj != null)
                                            {
                                                people.AgeRange += phoneKeyAgeObj["start"];
                                                people.AgeRange += "-";
                                                people.AgeRange += phoneKeyAgeObj["end"];
                                            }
                                            else
                                            {
                                                people.AgeRange += "Unknown";
                                            }

                                #endregion

                                            // Collecting Locations Key. if best_location node exist other wise will take location key from locations node
                                            string locationKey = string.Empty;
                                            if (personKeyObject["best_location"] != null)
                                            {
                                                //Increment counter thrice
                                                resultData.DataCounters++;

                                                dynamic personBestLocationObj = personKeyObject.best_location;
                                                if (personBestLocationObj != null)
                                                {
                                                    //Empty the Phone associated location we added earlier
                                                    dynamic bestLocationIdObj = personBestLocationObj.id;
                                                    if (bestLocationIdObj != null)
                                                    {
                                                        locationKey = (string)bestLocationIdObj["key"];
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (personKeyObject["locations"] != null)
                                                {
                                                    resultData.DataCounters++;
                                                    dynamic locationsPerPersonObj = personKeyObject.locations;
                                                    if (locationsPerPersonObj != null)
                                                    {
                                                        foreach (var personLocation in locationsPerPersonObj)
                                                        {
                                                            dynamic locationIdObj = personLocation.id;
                                                            if (locationIdObj != null)
                                                            {
                                                                locationKey = (string)locationIdObj["key"];
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            locationKeyList.Add(locationKey);

                                            peopleList.Add(people);
                                        }
                                    }
                                    
                                    resultData.SetPeople(peopleList.ToArray());
                                    List<Location> locationList = ParseLocationData(dictionaryObj, locationKeyList);

                                    if (locationList.Count > 0)
                                    {
                                        resultData.SetLocation(locationList.ToArray());
                                    }

                                }

                                if (resultData.GetLocation() == null && phoneAssociatedLocation.Length > 0)
                                {
                                    locationKeyList.Add(phoneAssociatedLocation);
                                    List<Location> locationList = ParseLocationData(dictionaryObj, locationKeyList);

                                    resultData.SetLocation(locationList.ToArray());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //this.ResultDiv.Visible = false;
                //this.errorBox.Visible = true;
                //this.LiteralErrorMessage.Text = ex.Message;
            }

            return resultData;
        }

        private static List<Location> ParseLocationData(dynamic dictionaryObj, List<string> locationKeyList)
        {
            List<Location> locationList = new List<Location>();
            Location location = null;

            // Extracting all location for all locationKeyList from locationKeyObject.
            foreach (string locationKey in locationKeyList)
            {
                location = new Location();

                dynamic locationKeyObject = dictionaryObj[locationKey];

                if (locationKeyObject != null)
                {
                    location.StandardAddressLine1 = (string)locationKeyObject["standard_address_line1"];
                    location.StandardAddressLine2 = (string)locationKeyObject["standard_address_line2"];
                    location.StandardAddressLocation = (string)locationKeyObject["standard_address_location"];
                    if (locationKeyObject["is_receiving_mail"] != null)
                    {
                        location.ReceivingMail = (bool)(locationKeyObject["is_receiving_mail"]);
                    }

                    if ((string)locationKeyObject["usage"] != null)
                    {
                        location.Usage = (string)locationKeyObject["usage"];
                    }
                    else
                    {
                        location.Usage = "Unknown";
                    }
                    if ((string)locationKeyObject["delivery_point"] != null)
                    {
                        location.DeliveryPoint = (string)locationKeyObject["delivery_point"];
                    }
                    else
                    {
                        location.DeliveryPoint = "Unknown";
                    }
                    locationList.Add(location);
                }
            }
            return locationList;
        }

        /// <summary>
        /// This method populates data on UI.
        /// </summary>
        /// <param name="resultData">resultData</param>
        private void PopulateDataOnUI(Result resultData)
        {

            //Kushal Comment
            correct.Visible = true;
            incorrect.Visible = true;


            //Re-initialize People and Location Literals
            this.LiteralLocationDetails.Text = "";
            this.LiteralPeopleDetails.Text = "";

            // populating phone details on UI.
            bool dndStatus = resultData.Phone.DndStatus;

            string dndStatusText = string.Empty;

            if (dndStatus)
            {
                dndStatusText = WhitePagesConstants.RegisteredText;
            }
            else
            {
                dndStatusText = WhitePagesConstants.NotRegisteredText;
            }

            string spamScore = string.Empty;

            if (!string.IsNullOrEmpty(resultData.Phone.SpamScore))
            {
                spamScore = resultData.Phone.SpamScore + WhitePagesConstants.PercentText;
            }
            else
            {
                spamScore = WhitePagesConstants.ZeroPercentText;
            }

            // Concatenate country code and phone number.
            string phoneNumber = resultData.Phone.CountryCallingCode + resultData.Phone.PhoneNumber;

            // Formating phone number.
            string formatedPhoneNumber = String.Format(CultureInfo.CurrentCulture, "{0:#-###-###-####}", Convert.ToInt64(phoneNumber, CultureInfo.CurrentCulture));
            formatedPhoneNumber = "+" + formatedPhoneNumber;

            this.LitralPhoneNumber.Text = formatedPhoneNumber;
            this.LiteralPhoneCarrier.Text = resultData.Phone.Carrier;
            this.LiteralPhoneType.Text = resultData.Phone.PhoneType;
            this.LiteralDndStatus.Text = dndStatusText;
            this.LiteralSpamScore.Text = spamScore;
            this.LiteralIsPrepaid.Text = resultData.Phone.IsPrepaid.ToString();

            //this.ResultDiv.Visible = true;

            // Creating UI template for people and populate on UI.
            if (resultData.GetPeople() != null && resultData.GetPeople().Count() > 0)
            {
                string peopleDetails = string.Empty;

                foreach (People people in resultData.GetPeople())
                {
                    string peopleData = WhitePagesConstants.PeopleDataTemplates;

                    peopleData = peopleData.Replace(WhitePagesConstants.PeopleNameKey, people.PersonName);
                    peopleData = peopleData.Replace(WhitePagesConstants.TypeKey, people.PersonType);

                    peopleData = peopleData.Replace(WhitePagesConstants.AgeRangeKey, people.AgeRange);
                    peopleData = peopleData.Replace(WhitePagesConstants.GenderKey, people.Gender);

                    peopleDetails += peopleData;
                }

                this.LiteralPeopleDetails.Text = peopleDetails;

            }
            else
            {
               this.personDataDiv.Visible = false;

            }

            // Creating UI template for location and populate on UI.
            if (resultData.GetLocation() != null && resultData.GetLocation().Count() > 0)
            {
                string locationDetails = string.Empty;
                foreach (Location location in resultData.GetLocation())
                {
                    string locationDataTemplate = WhitePagesConstants.LocationDataTemplates;

                    string receivingMail = location.ReceivingMail ? WhitePagesConstants.YesText : WhitePagesConstants.NoText;

                    string fullAddress = string.Empty;
                    fullAddress += string.IsNullOrEmpty(location.StandardAddressLine1) ? string.Empty : location.StandardAddressLine1 + "<br />";
                    fullAddress += string.IsNullOrEmpty(location.StandardAddressLine2) ? string.Empty : location.StandardAddressLine2 + "<br />";
                    fullAddress += string.IsNullOrEmpty(location.StandardAddressLocation) ? string.Empty : location.StandardAddressLocation;

                    locationDataTemplate = locationDataTemplate.Replace(WhitePagesConstants.AddressKey, fullAddress);
                    locationDataTemplate = locationDataTemplate.Replace(WhitePagesConstants.ReceivingMailKey, receivingMail);
                    locationDataTemplate = locationDataTemplate.Replace(WhitePagesConstants.UageEKey, location.Usage);
                    locationDataTemplate = locationDataTemplate.Replace(WhitePagesConstants.DeliveryPointKey, location.DeliveryPoint);

                    locationDetails += locationDataTemplate;
                }

                this.LiteralLocationDetails.Text = locationDetails;
            }
        }

        private void AddRaffleTicketNumbers(string p, string raffleTicketNumbers)
        {
            File.AppendAllText("C:\\WhitePages-PhoneLookup\\RaffleTicketNumbers.csv", p + ":" + raffleTicketNumbers + Environment.NewLine);
        }

        private string GetRaffleTicketNumbers(int dataPointsCounter)
        {
            string raffleTicketNumbers = "None, try another number! ";

            if (dataPointsCounter > 0)
            {
                var fileReader = new StreamReader(File.OpenRead("C:\\WhitePages-PhoneLookup\\RaffleTicketNumbers.csv"));
                string lastReadLine = "";
                while (!fileReader.EndOfStream)
                {
                    lastReadLine = fileReader.ReadLine();
                }

                int lastRaffleNumber = Convert.ToInt32(lastReadLine.Split(':')[1].Split('-')[1]);
                
                raffleTicketNumbers = "Your raffle numbers: " + Convert.ToString(lastRaffleNumber+1);
                raffleTicketNumbers += "-" + Convert.ToString(lastRaffleNumber + dataPointsCounter);

                fileReader.Close();

            }

            return raffleTicketNumbers;
        }
    }
}