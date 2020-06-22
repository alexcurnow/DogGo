using System.Collections.Generic;
using DogGo.Models;
using DogGo.Repositories;
using DogGo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DogWalker.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DogGo.Controllers
{
    public class WalkersController : Controller
    {
        private readonly WalkerRepository _walkerRepo;
        private readonly WalkRepository _walkRepo;
        private readonly NeighborhoodRepository _neighborhoodRepo;
        private readonly OwnerRepository _ownerRepo;
        private readonly DogRepository _dogRepo;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkersController(IConfiguration config)
        {
            _walkerRepo = new WalkerRepository(config);
            _walkRepo = new WalkRepository(config);
            _neighborhoodRepo = new NeighborhoodRepository(config);
            _ownerRepo = new OwnerRepository(config);
            _dogRepo = new DogRepository(config);
        }
        // GET: WalkersController
        public ActionResult Index()
        {
            try
            {
                int OwnerId = GetCurrentUserId();
                Owner owner = _ownerRepo.GetOwnerById(OwnerId);
                List<Walker> walkers = _walkerRepo.GetWalkersInNeighborhood(owner.NeighborhoodId);
                return View(walkers);
            }
            catch
            {
                List<Walker> walkers = _walkerRepo.GetAllWalkers();
                return View(walkers);

            }
        }

        // GET: WalkersController/Details/5
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Walk> walks = _walkRepo.GetWalksByWalkerId(id);
            Neighborhood neighborhood = _neighborhoodRepo.GetNeighborhoodById(walker.NeighborhoodId);

            WalkerDetailsViewModel vm = new WalkerDetailsViewModel()
            {
                Walker = walker,
                Walks = walks,
                Neighborhood = neighborhood
            };

            return View(vm);
        }

        // GET: WalkersController/Create
        public ActionResult Create()
        {
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();
            WalkerFormViewModel vm = new WalkerFormViewModel()
            {
                Walker = new Walker(),
                Neighborhoods = neighborhoods
            };
            return View(vm);
        }

        // POST: WalkersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Walker walker)
        {
            
            try
            {
                _walkerRepo.AddWalker(walker);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(walker);
            }
        }

        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();
            WalkerFormViewModel vm = new WalkerFormViewModel()
            {
                Walker = walker,
                Neighborhoods = neighborhoods
            };
            return View(vm);
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Walker walker)
        {
            try
            {
                _walkerRepo.UpdateWalker(walker);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(walker);
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            return View(walker);
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Walker walker)
        {
            try
            {
                _walkerRepo.DeleteWalker(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(walker);
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
