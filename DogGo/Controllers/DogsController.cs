using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DogGo.Models;
using DogGo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DogGo.Controllers
{
    public class DogsController : Controller
    {
        private readonly DogRepository _dogRepo;
        public DogsController(IConfiguration config)
        {   
            _dogRepo = new DogRepository(config);
        }
        // GET: DogsController
        [Authorize]
        public ActionResult Index()
        {
            int ownerId = GetCurrentUserId();
            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);
            return View(dogs);
        }

        // GET: DogsController/Details/5
        public ActionResult Details(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);
            if (dog == null)
            {
                return NotFound();
            }
            return View(dog);
        }

        // GET: DogsController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: DogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(Dog dog)
        {
            try
            {
                dog.OwnerId = GetCurrentUserId();
                _dogRepo.AddDog(dog);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(dog);
            }
        }

        // GET: DogsController/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            int OwnerId = GetCurrentUserId();
            Dog dog = _dogRepo.GetDogById(id);
            if (dog.OwnerId != OwnerId)
            {
                return NotFound();
            }

            return View(dog);
        }

        // POST: DogsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Dog dog)
        {
           int OwnerId = GetCurrentUserId();
           
           if (dog.OwnerId == OwnerId)
            {
                _dogRepo.UpdateDog(dog);
                return RedirectToAction("Index");
            }
            else
            {
                return View(dog);
            }
        }

        // GET: DogsController/Delete/5
        public ActionResult Delete(int id)
        {
            int OwnerId = GetCurrentUserId();
            Dog dog = _dogRepo.GetDogById(id);
            if (dog.OwnerId != OwnerId)
            {
                return NotFound();
            }
            return View(dog);
        }

        // POST: DogsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Dog dog)
        {
            int OwnerId = GetCurrentUserId();
            dog.OwnerId = OwnerId;
            if (dog.OwnerId == OwnerId)
            {
                _dogRepo.DeleteDog(id);
                return RedirectToAction(nameof(Index));
            }
            
                return View(dog);
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
