/*
Copyright (c) 2013, Colaboradores do INSTITUTO DE PESQUISAS ELDORADO
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided 
that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the 
following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
following disclaimer in the documentation and/or other materials provided with the distribution.

3. Neither the name of the INSTITUTO DE PESQUISAS ELDORADO nor the names of its contributors may be used to endorse or promote 
products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; 
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;
using System.Security;
using System.Net;
using System.Configuration;

namespace ExchangeBackEndApp.Controllers
{
    public class ExchangeController : Controller
    {
        public string GetIP()
        {
            string Str = "";
            Str = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(Str);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();

        }

        public bool checkIP(string IP)
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\iplist.txt");

            // se encontrou o IP na lista, retorna true
            if (Array.IndexOf(lines, IP) > -1)
                return true;
            return false;
        }

        //
        // GET: /Exchange/
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Exchange/Rooms
        [HttpPost]
        public void Rooms()
        {
            IUserData user = UserDataFromConsole.GetUserData();

            // Exchange Service based on ExchangeVersion from Web.config 
            ExchangeService myservice = new ExchangeService((ExchangeVersion)Enum.Parse(typeof(ExchangeVersion), ConfigurationManager.AppSettings["ExchangeVersion"]));
            myservice.Url = new Uri(ConfigurationManager.AppSettings["ExchangeUrl"]);
            user.AutodiscoverUrl = myservice.Url;

            // Username/Password connnection to Exchange 
            string domain = ConfigurationManager.AppSettings["ADDomain"];
            string username = ConfigurationManager.AppSettings["ADUsername"];
            string password = ConfigurationManager.AppSettings["ADPassword"];
            myservice.Credentials = new NetworkCredential(username, password, domain);

            string json;

            // Get Status from cities based on Request.City, Request.StartMeeting, Request.EndMeeting 
            json = GetRoomListsStatus(myservice, user, Request.Form["cidade"], Request.Form["horaIni"], Request.Form["horaFim"]);

            Response.Clear();
            Response.Write(json);
            Response.ContentType = "application/json; charset=utf-8";

            // Allow access EWS from localhost 
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            Response.End();
        }

        //
        // POST: /Exchange/RoomsId
        [HttpPost]
        public void RoomsId()
        {
            IUserData user = UserDataFromConsole.GetUserData();

            // Exchange Service based on ExchangeVersion from Web.config 
            ExchangeService myservice = new ExchangeService((ExchangeVersion)Enum.Parse(typeof(ExchangeVersion), ConfigurationManager.AppSettings["ExchangeVersion"]));
            myservice.Url = new Uri(ConfigurationManager.AppSettings["ExchangeUrl"]);
            user.AutodiscoverUrl = myservice.Url;

            // Username/Password connnection to Exchange
            string domain = ConfigurationManager.AppSettings["ADDomain"];
            string username = ConfigurationManager.AppSettings["ADUsername"];
            string password = ConfigurationManager.AppSettings["ADPassword"];
            myservice.Credentials = new NetworkCredential(username, password, domain);

            // Request.Room
            EmailAddress roomaddress = new EmailAddress(Request.Form["sala"] + "@eldorado.org.br");
            string json;

            // Get Status(free/busy) from Room based on Request.StartMeeting, Request.EndMeeting 
            json = GetUserFreeBusy(roomaddress, user, myservice, Request.Form["horaIni"], Request.Form["horaFim"]);

            if (json == "livre") // free
            {
                string status = "";
                // TODO: use better this status return
                status = CreateMeeting(roomaddress, user, myservice, Request.Form["user"], Request.Form["horaIni"], Request.Form["horaFim"]);

                json = "{ \"estado\" : \"" + json + "\", \"horario\" : \"" + status + "\" }";
            }
            else
            {
                json = "{ \"estado\" : \"" + json + "\" }";
            }

            Response.Clear();
            Response.Write(json);
            Response.ContentType = "application/json; charset=utf-8";

            // Allow access EWS from localhost 
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            Response.End();

        }

        /* 
         * GetRoomListsStatus
         * Get Status(free/busy) from Rooms of one city based on Request.StartMeeting, Request.EndMeeting
         */
        private static string GetRoomListsStatus(ExchangeService service, IUserData user, string cidade, string horaIni, string horaFim)
        {
            string json = "{ ";

            // Return all the room lists in the organization.
            EmailAddressCollection roomLists = service.GetRoomLists(); // Brasilia is not on a group anymore

            // Get rooms from room lists
            EmailAddress address;
            System.Collections.ObjectModel.Collection<EmailAddress> roomAddresses;

            string status = "";

            if (cidade == "#SalasCampinas")
            {
                address = new EmailAddress("SalasCampinas@eldorado.org.br");
                roomAddresses = service.GetRooms(address);
                // Display the individual rooms. 
                foreach (EmailAddress inneraddress in roomAddresses)
                {
                    status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);

                    json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
                }
            }
            else if (cidade == "#SalasPortoAlegre")
            {
                address = new EmailAddress("SalasdePortoAlegre@eldorado.org.br");
                roomAddresses = service.GetRooms(address);
                // Display the individual rooms. 
                foreach (EmailAddress inneraddress in roomAddresses)
                {
                    status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);

                    json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
                }
            }
            else
            {
                json = json + GetStatusFromBrasilia(service, user, horaIni, horaFim);
            }

            // remove an extra semi-collon
            json = json.Substring(0, json.Length - 2);
            json = json + " }";

            return json;
        }

        /* 
         * GetUserFreeBusy
         * Get Status(free/busy) from Rooms of one city based on Request.StartMeeting, Request.EndMeeting
         */
        private static string GetUserFreeBusy(EmailAddress roomaddress, IUserData user, ExchangeService service, string horaIni, string horaFim)
        {
            // Room status
            string status = "livre";

            // Create a list of attendees 
            List<AttendeeInfo> attendees = new List<AttendeeInfo>();

            attendees.Add(new AttendeeInfo()
            {
                SmtpAddress = roomaddress.Address,
            });

            // Specify availability options 
            AvailabilityOptions availabilityOptions = new AvailabilityOptions();
            availabilityOptions.RequestedFreeBusyView = FreeBusyViewType.FreeBusy;

            // Return a set of of free/busy times 
            GetUserAvailabilityResults freeBusyResults = service.GetUserAvailability(attendees,
                                                                                 new TimeWindow(DateTime.Now, DateTime.Now.AddDays(1)),
                                                                                     AvailabilityData.FreeBusy,
                                                                                     availabilityOptions);

            foreach (AttendeeAvailability availability in freeBusyResults.AttendeesAvailability)
            {
                Console.WriteLine(availability.Result);
                Console.WriteLine();

                // room is free this day
                if (availability.CalendarEvents.Count == 0)
                {
                    // convert time from this format "HH MM TT" to TimeSpan
                    TimeSpan startMeeting = new TimeSpan(Convert.ToInt32(horaIni.Split(':')[0]), Convert.ToInt32(horaIni.Split(':')[1]), 0);
                    TimeSpan endMeeting = new TimeSpan(Convert.ToInt32(horaFim.Split(':')[0]), Convert.ToInt32(horaFim.Split(':')[1]), 0);

                    DateTime nowConfirma = new DateTime();
                    nowConfirma = DateTime.Now;
                    TimeSpan tsNow = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                    if (startMeeting <= tsNow)
                    {
                        startMeeting = new TimeSpan(tsNow.Hours, tsNow.Minutes, tsNow.Seconds);
                    }
                    if (endMeeting <= tsNow)
                    {
                        endMeeting = new TimeSpan(tsNow.Hours, tsNow.Minutes, tsNow.Seconds);
                    }
                    if (startMeeting < endMeeting)
                    {
                        status = "livre";
                    }
                    else
                    {
                        status = "inativa";
                    }
                }

                Collection<CalendarView> ccv = new Collection<CalendarView>();

                // merge meetings that are close
                foreach (CalendarEvent calendarItem in availability.CalendarEvents)
                {
                    TimeSpan ciStart = calendarItem.StartTime.TimeOfDay;
                    TimeSpan ciEnd = calendarItem.EndTime.TimeOfDay;

                    if (ccv.Count > 0)
                    {
                        CalendarView lastCV = ccv[ccv.Count - 1];
                        // merge if event start is the same as last event end
                        if (ciStart == lastCV.EndDate.TimeOfDay)
                        {
                            CalendarView novoCV = new CalendarView(lastCV.StartDate, Convert.ToDateTime(ciEnd.ToString()));
                            ccv.RemoveAt(ccv.Count - 1);
                            ccv.Add(novoCV);
                        }
                        // else, add new event
                        else
                        {
                            CalendarView novoCV = new CalendarView(Convert.ToDateTime(ciStart.ToString()), Convert.ToDateTime(ciEnd.ToString()));
                            ccv.Add(novoCV);
                        }
                    }
                    // if there is only one element
                    else
                    {
                        CalendarView novoCV = new CalendarView(Convert.ToDateTime(ciStart.ToString()), Convert.ToDateTime(ciEnd.ToString()));
                        ccv.Add(novoCV);
                    }
                }

                // Checks for events if there are compatibility with Start and End of Meeting
                foreach (CalendarView calendarItem in ccv)
                {
                    TimeSpan start = calendarItem.StartDate.TimeOfDay;
                    TimeSpan end = calendarItem.EndDate.TimeOfDay;

                    // convert time from this format "HH MM TT" to TimeSpan
                    TimeSpan startMeeting = new TimeSpan(Convert.ToInt32(horaIni.Split(':')[0]), Convert.ToInt32(horaIni.Split(':')[1]), 0);
                    TimeSpan endMeeting = new TimeSpan(Convert.ToInt32(horaFim.Split(':')[0]), Convert.ToInt32(horaFim.Split(':')[1]), 0);

                    DateTime nowConfirma = new DateTime();
                    nowConfirma = DateTime.Now;
                    TimeSpan tsNow = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                    if (startMeeting <= tsNow)
                    {
                        startMeeting = new TimeSpan(tsNow.Hours, tsNow.Minutes, tsNow.Seconds);
                    }
                    if (endMeeting <= tsNow)
                    {
                        endMeeting = new TimeSpan(tsNow.Hours, tsNow.Minutes, tsNow.Seconds);
                    }
                    if (startMeeting < endMeeting)
                    {
                        /* 
                         * busy / ocupada: 
                         *              1) start <= horaIni <= horaFim <= end
                         *          
                         * partial / parcial: 
                         *              1) start <= horaIni <= end <= horaFim
                         *              2) horaIni <= start <= horaFim <= end
                         *              3) horaIni <= start <= end <= horaFim 
                         *          
                         * free / livre:  
                         *              1) end <= horaIni
                         *              2) horaFim <= start 
                         *          
                         */
                        if (start <= startMeeting && endMeeting <= end)
                        {
                            status = "ocupada";
                        }
                        else if (start <= startMeeting && startMeeting < end && end < endMeeting)
                        {
                                status = "parcial";
                        }
                        else if (startMeeting < start && start < endMeeting && endMeeting <= end)
                        {
                                status = "parcial";
                        }
                        else if (startMeeting < start && start < end && end < endMeeting)
                        {
                                status = "parcial";
                        }
                    }
                    // if startMeeting >= endMeeting
                    else
                    {
                        status = "inativa";
                    }
                }
            } 
            return status;
        }

        /*
         * CreateMeeting
         * Create a meeting at <roomadress> from horaIni (StartMeeting) to horaFim (EndMeeting)
         * Also adds <request_user> as a guest attendee
         * Returns: empty string in case of not creating this meeting
         */
        private static string CreateMeeting(EmailAddress roomaddress, IUserData user, ExchangeService service, string request_user, string horaIni, string horaFim)
        {
            bool status = false;
            string horario = "";
 
            Appointment meeting = new Appointment(service);
            meeting.Subject = ConfigurationManager.AppSettings["EmailSubject"];
            meeting.Body = ConfigurationManager.AppSettings["Body"];

            // convert time from this format "HH MM TT" to TimeSpan
            TimeSpan startMeeting = new TimeSpan(Convert.ToInt32(horaIni.Split(':')[0]), Convert.ToInt32(horaIni.Split(':')[1]), 0);
            TimeSpan endMeeting = new TimeSpan(Convert.ToInt32(horaFim.Split(':')[0]), Convert.ToInt32(horaFim.Split(':')[1]), 0);

            DateTime now = new DateTime();
            now = DateTime.Now;
            TimeSpan tsNow = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            // checks if start and end of meeting are valid, comparing to now
            if (startMeeting <= tsNow)
            {
                startMeeting = new TimeSpan(tsNow.Hours, tsNow.Minutes, tsNow.Seconds);
            }
            if (endMeeting <= tsNow)
            {
                endMeeting = new TimeSpan(tsNow.Hours, tsNow.Minutes, tsNow.Seconds);
            }
            if (startMeeting == endMeeting)
            {
                return "";
            }

            meeting.Start = Convert.ToDateTime(startMeeting.ToString());
            meeting.End = Convert.ToDateTime(endMeeting.ToString()); 
            meeting.Location = roomaddress.Address.Split('@')[0];

            // add guest user and room address at meeting
            Attendee attendee = new Attendee();
            attendee.Address = request_user.Split('@')[0] + "@eldorado.org.br";
            //service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.PrincipalName, user.EmailAddress);
            meeting.RequiredAttendees.Add(attendee);
            meeting.RequiredAttendees.Add(roomaddress.Address);

            try 
            {
                if (status)
                {
                    List<Item> meetings = new List<Item>(); 
                    meetings.Add(meeting); 

                    meeting.Save(SendInvitationsMode.SendToAllAndSaveCopy);

                    // Create the batch of meetings. This results in a CreateItem operation call to EWS. 
                    ServiceResponseCollection<ServiceResponse> responses = service.CreateItems(meetings,  
                                                                                              WellKnownFolderName.Calendar,  
                                                                                              MessageDisposition.SendOnly,  
                                                                                              SendInvitationsMode.SendToAllAndSaveCopy); 

                    if (responses.OverallResult == ServiceResult.Success) 
                    { 
                        Console.WriteLine("You've successfully created a couple of meetings in a single call.");
                        horario = meeting.Start.Hour.ToString() + meeting.Start.Minute.ToString();
                    } 
                    else if (responses.OverallResult == ServiceResult.Warning) 
                    { 
                        Console.WriteLine("There are some issues with your batch request."); 
 
                        foreach (ServiceResponse response in responses) 
                        { 
                            if (response.Result == ServiceResult.Error) 
                            { 
                                Console.WriteLine("Error code: " + response.ErrorCode.ToString()); 
                                Console.WriteLine("Error message: " + response.ErrorMessage); 
                            } 
                        } 
                    } 
                    else // responses.OverallResult == ServiceResult.Error 
                    { 
                        Console.WriteLine("There are errors with your batch request."); 
 
                        foreach (ServiceResponse response in responses) 
                        { 
                            if (response.Result == ServiceResult.Error) 
                            { 
                                Console.WriteLine("Error code: " + response.ErrorCode.ToString()); 
                                Console.WriteLine("Error message: " + response.ErrorMessage); 
                            } 
                        } 
                    } 
                } 
                else // Show creation of a single meeting. 
                {
                    // Create a single meeting. This results in a CreateItem operation call to EWS. 
                    meeting.Save(SendInvitationsMode.SendToAllAndSaveCopy);
                    
                    Console.WriteLine("You've successfully created a single meeting.");
                    horario = meeting.Start.Hour.ToString() + meeting.Start.Minute.ToString();
                }

                status = true;
            } 
            catch (Exception ex) 
            {
                horario = "";
                status = false;
            } 

            return horario;
        }

        /*
         * GetStatusFromBrasilia
         * Get status from a city that does not make part of a group
         * getting status of each room individually
         */ 
        private static string GetStatusFromBrasilia(ExchangeService service, IUserData user, string horaIni, string horaFim)
        {
            string json = "";
            EmailAddress inneraddress;
            string status = "";

            inneraddress = new EmailAddress("salaApoio01BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaApoio02BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaApoio03BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaApoio04BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaApoio05BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaApoio06BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaApoio07BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaReuniaoA.BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaReuniaoB.BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaReuniaoC.BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";
            inneraddress = new EmailAddress("salaReuniaoD.BSB@eldorado.org.br");
            status = GetUserFreeBusy(inneraddress, user, service, horaIni, horaFim);
            json = json + "\"" + inneraddress.Address.Split('@')[0] + "\" : \"" + status + "\", ";

            return json;
        }

    }
}
