﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TourForEverybuddy.Controllers.Elements;
using TourForEverybuddy.Controllers.Static;
using TourForEverybuddy.Models;
using TourForEverybuddy.Models.Filters;

namespace TourForEverybuddy.Controllers.Membership
{
    [MyAuthentication]
    public class AccountController : Controller
    {
        private DataManager manager;
        //[MyAuthorize(Users = "admin, Alex", Roles = "admin")]
        public AccountController()
        {
            manager = new DataManager();
            //if (user == null)
            //    return RedirectToAction("Index", "Login", new { returnUrl = Request.UrlReferrer != null ? Request.UrlReferrer.OriginalString : "" });
        }
        public ActionResult Index()
        {
            var user = Storage.currentUser;
            ViewBag.UserLanguage = manager.GetUserLanguages(user.id).Select(x => x.Language.name).ToList();

            //ToDo: add Langues
            return View(user);
        }

        #region Edit profile
        [HttpGet]
        public ActionResult Edit()
        {
            var user = Storage.currentUser;

            FillDropDownListCountriesAndLanguages(user.id);

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User user, string[] Language, HttpPostedFileBase Picture)
        {
            if (this.ModelState.IsValid)
            {
                
                GetPictureForUser(user, Picture);
                bool updated = manager.UpdateUser(user, Language);

                if (updated)
                {
                    Storage.currentUser = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    FillDropDownListCountriesAndLanguages(user.id);
                    ModelState.AddModelError("EmailIsHave", "This user is already registered.");
                }
            }
            else
                ModelState.AddModelError("ValidIsFailed", "Fail.");

            return View();

        }
        #endregion

        #region AddTour
        [HttpGet]
        public ActionResult AddTour()
        {
            FillDDLCitiesDurationDay();
            return View();
        }

        [HttpPost]
        public ActionResult AddTour(Tour tour, bool isAnyTime, string DaysOfTheWeek, HttpPostedFileBase[] Pictures)
        {
            var user = Storage.currentUser;

            tour.userID = user.id;
            if (isAnyTime)// || string.IsNullOrEmpty(tour.startsAt))
                tour.startsAt = "Any time";

            GetPictureForTour(tour, Pictures);

            if (!manager.CreateTour(tour, DaysOfTheWeek.Split(',')))
            {
                ModelState.AddModelError(
@"TourIsHave", "Sorry , this tour already exists. Please, change the title of the tour. ");
                return View();
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Edit tour
        public ActionResult EditTour(int id)
        {
            var tour = Storage.currentUser.Tours.FirstOrDefault(x => x.Id == id);

            if (tour != null)
            {
                FillDDLCitiesDurationDay(id);
                return View(tour);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditTour(Tour tour,bool isAnyTime, string DaysOfTheWeek, HttpPostedFileBase[] Pictures)
        {
            if (isAnyTime) // || string.IsNullOrEmpty(tour.startsAt))
                tour.startsAt = "Any time";

            GetPictureForTour(tour, Pictures);

            if (ModelState.IsValid && manager.UpdateTour(tour, DaysOfTheWeek.Split(',')))
                return RedirectToAction("Index"); //, new { id = tour.Id });
            else
                return View(tour);
        }

#region Delete images in tour
        [HttpPost]
        public ActionResult DeleteImages(int id, string PictureArray)
        {
            if (String.IsNullOrEmpty(PictureArray))
            {
                ModelState.AddModelError("ValidIsFailed", "Please, select images for delete.");
                return RedirectToAction("EditTour", new { id = id });
            }

            var array = PictureArray.Split(',');

            if (array.Length != 0)
                if (!manager.DeletePictures(id, array))
                    ModelState.AddModelError("ValidIsFailed", "Sorry, failed to delete the images, please try again later.");

            //Storage.currentUser = null; Нужно ли , что бы обновить информацию ?
            return RedirectToAction("EditTour", new { id = id });
        }
#endregion

#region Delete tour
        [HttpGet]
        public ActionResult DeleteTour(int id)
        {
            if (!manager.DeleteTour(id))
                ModelState.AddModelError("ValidIsFailed", "Sorry, failed to delete the tour, please try again later.");

            return RedirectToAction("Index");
        }
#endregion

        #endregion

        #region Get pictures

        private void GetPictureForTour(Tour tour, HttpPostedFileBase[] Pictures)
        {
            if (Pictures.Count() > 0 && Pictures[0] != null)
            {
                for (int i = 0; i < Pictures.Count(); i++)
                {
                    var picture = new Tour_PictureOfTour();
                    picture.FileName = Pictures[i].FileName;
                    picture.ContentType = Pictures[i].ContentType;

                    using (var reader = new BinaryReader(Pictures[i].InputStream))
                    {
                        //picture.Picture = reader.ReadBytes(Pictures[i].ContentLength);
                        picture.Picture = PictureController.GetCroppedImage(reader.ReadBytes(Pictures[i].ContentLength), PictureController.GetFormatImage(Pictures[i].ContentType));
                    }

                    tour.Tour_PictureOfTour.Add(picture);
                }
            }
        }
        private void GetPictureForUser(User user, HttpPostedFileBase Picture)
        {
            if (Picture != null)
            {
                user.FileName = Picture.FileName;
                user.ContentType = Picture.ContentType;

                using (var reader = new BinaryReader(Picture.InputStream))
                {
                    //picture.Picture = reader.ReadBytes(Pictures[i].ContentLength);
                    user.Photo = PictureController.GetCroppedImage(reader.ReadBytes(Picture.ContentLength), PictureController.GetFormatImage(Picture.ContentType));
                }
            }
        }
        #endregion

        private void FillDropDownListCountriesAndLanguages(int userID)
        {
            ViewBag.Countries =
              manager.GetCountries().Select(x => new SelectListItem { Text = x.country_name, Value = x.id.ToString() }).ToList();

            ViewBag.Languages = manager.GetLanguages();

            var list = manager.GetUserLanguages(userID);
            ViewBag.ArrayLanguages = list;
        }

        private void FillDDLCitiesDurationDay(int id = -1)
        {
            ViewBag.Cities = manager.GetCities();
            ViewBag.Duration = manager.GetDuration();
            ViewBag.DayOfTheWeek = manager.GetDayOfTheWeek().ToList();

            if (id != -1)
                ViewBag.DaysOfTheWeekTour = manager.GetTour_DaysOfTheWeek(id).ToList();

        }


    }

}