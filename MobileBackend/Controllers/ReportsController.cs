﻿using MobileBackend.DataAccess;
using MobileBackend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MobileBackend.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult HoursPerWorkAssingment()


        {
            //tähän luotu luokka
            TimesheetEntities entities = new TimesheetEntities();
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                // haetaan kaikki kuluvan päivän tuntikirjaukset
                List<Timesheet> allTimesheetsToday = (from sh in entities.Timesheets
                                                      where (sh.StartTime > today) &&
                                                        (sh.StartTime < tomorrow) &&                       
                                                        (sh.WorkCompleted == true)
                                                      select sh).ToList();

                // ryhmitellään kirjaukset tehtävittäin ja lasketaan kestot

                List<HoursPerWorkAssingmentsModel> model = new List<HoursPerWorkAssingmentsModel>();

                foreach (Timesheet timesheet in allTimesheetsToday)
                {
                    int assignmentId = timesheet.id_WorkAssignment.Value;
                    HoursPerWorkAssingmentsModel existing = model.Where(
                        m => m.WorkAssignmentId == assignmentId).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.TotalHours += (timesheet.StopTime.Value - timesheet.StartTime.Value).TotalHours;
                    }
                    else
                    {
                        existing = new HoursPerWorkAssingmentsModel()
                        {
                            WorkAssignmentId = assignmentId,
                            WorkAssigmentName = timesheet.WorkAssignment.Title,
                            TotalHours = (timesheet.StopTime.Value - timesheet.StartTime.Value).TotalHours
                        };
                        model.Add(existing);
                    }
                }

                return View(model);

            }
            finally
            {
                entities.Dispose();
            }


        }

        public ActionResult HoursPerWorkAssingmentAsExcel()
        {
            // TODO: Hae tiedot tietokannasta!
            //StrinBuilder nopeuttaa pitkien merkkijonojen generoimiseen. Nopeuttaa toimintaa
            StringBuilder csv = new StringBuilder();

            //Luodaan CSV-muotoinen tiedosto
            csv.AppendLine("Matti;123,5");
            csv.AppendLine("Jesse;86,25");
            csv.AppendLine("Kaisa;99,00");

            //palautetaan CSV-tiedot selaimelle
            //Content palauttaa mitä tahansa merkkijono tiedostoa
            //return Content();
            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "Text/csv", "Työtunnit.csv");

        }
        public ActionResult HoursPerWorkAssingmentAsExcel2()
        { 
            StringBuilder csv = new StringBuilder();

            TimesheetEntities entities = new TimesheetEntities();
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                // haetaan kaikki kuluvan päivän tuntikirjaukset
                List<Timesheet> allTimesheetsToday = (from sh in entities.Timesheets
                                                      where (sh.StartTime > today) &&
                                                        (sh.StartTime < tomorrow) &&
                                                        (sh.WorkCompleted == true)
                                                      select sh).ToList();

                // ryhmitellään kirjaukset tehtävittäin ja lasketaan kestot

                List<HoursPerWorkAssingmentsModel> model = new List<HoursPerWorkAssingmentsModel>();

                foreach (Timesheet timesheet in allTimesheetsToday)
                {
                    csv.AppendLine(timesheet.Id_Employee + "," +
                    timesheet.StartTime + "," + timesheet.StopTime + ",");
                }

            }

            finally
            {
                entities.Dispose();
            }
            //palautetaan CSV-tiedot selaimelle
            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "Text/csv", "Työtunnit.csv");

            }

    }
}
    